using IntelligentAI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace IntelligentAI.Aggregates;

public class AiModelEventManager : IAiModelEventManager
{
    private readonly ConcurrentDictionary<string, Channel<AiProgressResult>> _todoTasks = new ConcurrentDictionary<string, Channel<AiProgressResult>>();
    private readonly ConcurrentDictionary<string, Channel<AiProgressResult>> _finishedTasks = new ConcurrentDictionary<string, Channel<AiProgressResult>>();

    private readonly ILogger<AiModelEventManager> _logger;

    public AiModelEventManager(ILogger<AiModelEventManager> logger)
    {
        _logger = logger;
    }

    public async IAsyncEnumerable<AiProgressResult> StartTasksAsync(
        AiModelBase model,
        Guid eventId,
        Guid parentTaskId,
        IEnumerable<AiArguments> tasks,
        string taskName = "EventTasks",
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        var channelName = $"{model.ServiceKey}-{eventId}";

        // 创建 CancellationTokenSource 用于定期检查任务
        var periodicCheckCts = new CancellationTokenSource();



        // 启动定期检查任务
        StartPeriodicCheck(channelName, periodicCheckCts.Token);

        // 监听 cancellation 的取消请求
        cancellation.Register(() =>
        {
            // 当 cancellation 被取消时，首先取消 todoChannel 中的所有任务
            if (_todoTasks.TryGetValue(channelName, out var todoChannel))
            {
                todoChannel.Writer.TryComplete(new OperationCanceledException());
            }

            // 然后取消定期检查任务
            periodicCheckCts.Cancel();
        });

        #region Channel初始化

        Channel<AiProgressResult> todoChannel = Channel.CreateBounded<AiProgressResult>(100);

        _todoTasks[channelName] = todoChannel;

        Channel<AiProgressResult> finishedChannel = Channel.CreateBounded<AiProgressResult>(100);

        _finishedTasks[channelName] = finishedChannel;

        #endregion

        AiProgressResult[] progresses = tasks.Select((task, i) =>
                    new AiProgressResult(eventId, Guid.NewGuid(), i + 1, tasks.Count(), taskName, task, parentTaskId))
                    .ToArray();

        // 启动生产者任务
        await ProduceAsync(todoChannel.Writer, progresses, cancellation);

        while (await todoChannel.Reader.WaitToReadAsync(cancellation))
        {
            // 用于跟踪任务和消费者名称
            var consumers = new Dictionary<Task, string>();

            int count = Math.Min(model.ConcurrentNumber, todoChannel.Reader.Count);

            for (int i = 1; i <= count; i++)
            {
                var consumerName = $"{eventId}-Consumer-{i}";

                var consumerTask = ConsumeAsync(todoChannel.Reader, finishedChannel.Writer, model, consumerName, cancellation);

                consumers.Add(consumerTask, consumerName);
            }

            while (consumers.Any())
            {
                cancellation.ThrowIfCancellationRequested();

                var completedTask = await Task.WhenAny(consumers.Keys);

                // 获取完成的消费者名称
                var completedConsumerName = consumers[completedTask];

                consumers.Remove(completedTask);

                // 在这里添加检查以确定是否可以启动新的消费者任务
                if (todoChannel.Reader.Count > 0 && consumers.Count < model.ConcurrentNumber)
                {
                    var newConsumerTask = ConsumeAsync(todoChannel.Reader, finishedChannel.Writer, model, completedConsumerName, cancellation);

                    consumers.Add(newConsumerTask, completedConsumerName);
                }

                // Yield results from finishedChannel if available
                while (finishedChannel.Reader.Count > 0)
                {
                    finishedChannel.Reader.TryRead(out var progress);

                    yield return progress;
                }
            }

        }

        finishedChannel.Writer.Complete();

        periodicCheckCts.Cancel();
    }

    /// <summary>
    /// 在开启任务前调用
    /// 1.队列未创建或无任务 => false
    /// 2.队列中 任务数/并发数 <= 3 且 超时 < 15s => false
    /// 3.队列中 任务数/并发数 <= 3 超时 >= 15s => true
    /// 4.队列中 任务数/并发数 > 3 => true
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="modelName"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<bool> IsBusy(AiModelBase model, CancellationToken cancellation = default)
    {
        var count = GetAllTasks(model);

        if (count == 0) return false;

        // 计算任务数与并发数的比例
        var ratio = (double)count / model.ConcurrentNumber;

        if (ratio > 3) return true;

        using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
        {
            linkedCts.CancelAfter(TimeSpan.FromSeconds(15));

            try
            {

                while (count > 0)
                {
                    await Task.Delay(1000, linkedCts.Token);

                    count = GetAllTasks(model);
                }

                return false;
            }
            catch (TaskCanceledException)
            {
                return true;
            }
        }

    }

    public int GetAllTasks(AiModelBase model)
    {
        int count = 0;

        var channels = _todoTasks
           .Where(kvp => kvp.Key.Contains(model.ServiceKey))? // 过滤出包含 ServiceKey 的键
           .Select(kvp => kvp.Value); // 选择对应的值

        if (channels is null || !channels.Any()) return count;

        foreach (var channel in channels)
        {
            count += channel.Reader.Count;
        }

        // 返回新的Channel
        return count;
    }

    private async Task ProduceAsync(ChannelWriter<AiProgressResult> writer, IEnumerable<AiProgressResult> tasks, CancellationToken cancellation = default)
    {
        // 启动生产者任务
        foreach (var task in tasks)
        {
            // 生产消息
            await writer.WriteAsync(task, cancellation);
        }

        writer.Complete();

        _logger
            .LogInformation($"{tasks.Count()} tasks were successfully produced.");
    }

    private async Task ConsumeAsync(
        ChannelReader<AiProgressResult> reader,
        ChannelWriter<AiProgressResult> writer,
        AiModelBase model,
        string consumerName,
        CancellationToken cancellation)
    {
        if (!reader.TryRead(out var progress)) return;

        var request = progress.Parameters;

        _logger.LogInformation($"Consumer: {consumerName} starts consuming the {progress.SortId}/{progress.Count} task.");

        try
        {
            var result = await model.AnswerText(
                request.Question,
                request.ToDictionary(),
                cancellation: cancellation);

            progress.SetResult(result);
        }
        catch (ApplicationException e)
        {
            progress.SetResult($"An application exception occurred during the answering process: {e.Message}", true);

            _logger.LogError(e, $"An application exception occurred during the answering process: {e.Message}");

        }
        catch (Exception e)
        {
            progress.SetResult($"An exception occurred during the answering process: {e.Message}", true);

            _logger.LogWarning(e, $"An exception occurred during the answering process: {e.Message}");
        }

        _logger.LogInformation($"Consumer: {consumerName} finishes the {progress.SortId}/{progress.Count} task.");

        await writer.WriteAsync(progress);
    }


    // 新增一个方法来启动定时检查

    private async Task StartPeriodicCheck(string channelName, CancellationToken cancellation)
    {
        _logger.LogInformation($"Starting periodic check for Channel-{channelName}.");

        int previousTaskCount = -1;

        Guid? previousFirstEventId = null;

        try
        {
            while (!cancellation.IsCancellationRequested)
            {

                await Task.Delay(TimeSpan.FromMinutes(1), cancellation);

                if (_todoTasks.TryGetValue(channelName, out var todoChannel))
                {
                    // 获取当前通道中的任务数量
                    int currentTaskCount = todoChannel.Reader.Count;

                    // 尝试读取队列中的第一个任务的 EventId，但不从队列中移除它
                    if (todoChannel.Reader.TryPeek(out var firstTask))
                    {
                        Guid currentFirstEventId = firstTask.EventId;

                        // 检查任务数量和第一个任务的 EventId 是否与上一次检查相同
                        if (currentTaskCount == previousTaskCount && currentFirstEventId == previousFirstEventId)
                        {
                            _logger.LogWarning($"Detected abnormal task retention for Channel-{channelName}. Cancelling tasks.");

                            CancelAllTasks(channelName);
                        }

                        previousFirstEventId = currentFirstEventId;
                    }
                    else
                    {
                        // 如果无法读取第一个任务，则重置 previousFirstEventId
                        previousFirstEventId = null;
                    }

                    previousTaskCount = currentTaskCount;
                }
                else
                {
                    _logger.LogInformation($"No tasks found for model Channel-{channelName} to check.");

                    // 如果没有找到任务，则重置计数和 EventId
                    previousTaskCount = -1;

                    previousFirstEventId = null;

                    break;
                }
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation($"Periodic check for Channel-{channelName} was cancelled.");

            CancelAllTasks(channelName);
        }


    }


    /// <summary>
    /// 取消所有任务
    /// </summary>
    /// <param name="channelName"> ServiceKey-EventId </param>
    private void CancelAllTasks(string channelName)
    {
        // Attempt to retrieve the channels for the given model
        if (_todoTasks.TryGetValue(channelName, out var todoChannel) &&
            _finishedTasks.TryGetValue(channelName, out var finishedChannel))
        {
            while (todoChannel.Reader.TryRead(out var progress))
            {
                // You can log the cancellation or perform other clean-up operations here
                _logger.LogInformation($"Task {progress.SortId}/{progress.Count} was cancelled.");
            }

            while (finishedChannel.Reader.TryRead(out var progress))
            {
                // Log or clean up finished tasks if necessary
                _logger.LogInformation($"Finished task {progress.SortId}/{progress.Count} was cleared.");
            }

            // Remove the channels from the dictionaries to reset the state
            _todoTasks.TryRemove(channelName, out _);
            _finishedTasks.TryRemove(channelName, out _);

            // Log the completion of the cancellation process
            _logger.LogInformation($"All tasks for Channel-{channelName} have been cancelled.");

        }
        else
        {
            // Log that there were no tasks to cancel for the given model
            _logger.LogInformation($"No tasks found for Channel-{channelName} to cancel.");
        }
    }

}
