
namespace IntelligentAI.Models;

public class AiProgressResult
{
    public Guid EventId { get; private set; }

    public Guid SubTaskId { get; private set; }

    public int SortId { get; private set; }

    public int Count { get; private set; }

    public string TaskName { get; private set; }

    public AiArguments Parameters { get; private set; }

    public string Data { get; private set; }

    public Guid? ParentTaskId { get; private set; }

    public bool IsException { get; private set; }

    [System.Text.Json.Serialization.JsonConstructor]
    public AiProgressResult(
        Guid eventId, 
        Guid subTaskId, 
        int sortId, 
        int count, 
        string taskName, 
        AiArguments parameters, 
        Guid? parentTaskId,
        string data,
        bool isException)
    {
        EventId = eventId;
        SubTaskId = subTaskId;
        SortId = sortId;
        Count = count;
        TaskName = taskName;
        Parameters = parameters;
        ParentTaskId = parentTaskId;
        Data = data;
        IsException = isException;
    }

    public AiProgressResult(
        Guid eventId,
        Guid subTaskId,
        int sortId,
        int count,
        string taskName,
        AiArguments parameters,
        Guid? parentTaskId) : this(eventId, subTaskId, sortId, count, taskName, parameters, parentTaskId, string.Empty, false)
    {
        #region 参数校验

        if (eventId == Guid.Empty) throw new ArgumentNullException($"任务 Guid 不能为空，请确保 eventId 参数的有效性");

        if (subTaskId == Guid.Empty)
        {
            subTaskId = Guid.NewGuid();
        }

        if (sortId < 1 || sortId > count) throw new ArgumentOutOfRangeException($"'{sortId}' 不是一个有效值，请确保 sortId 参数的有效性");

        if (count < 1) throw new ArgumentOutOfRangeException($"'{count}' 不是一个有效值，请确保 count 参数的有效性");

        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException($"任务名称不能为空，请确保 taskName 参数的有效性");


        #endregion

        EventId = eventId;
        SubTaskId = subTaskId;
        SortId = sortId;
        Count = count;
        TaskName = taskName;
        Parameters = parameters;
        ParentTaskId = parentTaskId;
    }

    public void SetResult(string data, bool exception = false)
    {
        Data = data;
        IsException = exception;
    }

    public override bool Equals(object obj)
    {
        if (obj is AiProgressResult other)
        {
            return (EventId == other.EventId && SubTaskId == other.SubTaskId) ||
                   (EventId == other.EventId && Parameters == other.Parameters);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked // 溢出时不抛出异常
        {
            int hash = 17;
            hash = hash * 23 + EventId.GetHashCode();
            hash = hash * 23 + (SubTaskId != Guid.Empty ? SubTaskId.GetHashCode() : 0);
            hash = hash * 23 + (Parameters != null ? Parameters.GetHashCode() : 0);
            return hash;
        }
    }

    public static bool operator ==(AiProgressResult left, AiProgressResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AiProgressResult left, AiProgressResult right)
    {
        return !(left == right);
    }

}

public class AiProgressResult<TParameters, TData>
{
    public Guid EventId { get; private set; }

    public Guid SubTaskId { get; private set; }

    public int SortId { get; private set; }

    public int Count { get; private set; }

    public string TaskName { get; private set; }

    public TParameters Parameters { get; private set; }

    public TData Data { get; private set; }

    public Guid? ParentTaskId { get; private set; }

    public AiProgressResult(Guid eventId, Guid subTaskId, int sortId, int count, string taskName, TParameters parameters, Guid? parentTaskId)
    {
        #region 参数校验

        if (eventId == Guid.Empty) throw new ArgumentNullException($"任务 Guid 不能为空，请确保 eventId 参数的有效性");

        if (subTaskId == Guid.Empty)
        {
            subTaskId = Guid.NewGuid();
        }

        if (sortId < 1 || sortId > count) throw new ArgumentOutOfRangeException($"'{sortId}' 不是一个有效值，请确保 sortId 参数的有效性");

        if (count < 1) throw new ArgumentOutOfRangeException($"'{count}' 不是一个有效值，请确保 count 参数的有效性");

        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException($"任务名称不能为空，请确保 taskName 参数的有效性");


        #endregion

        EventId = eventId;
        SubTaskId = subTaskId;
        SortId = sortId;
        Count = count;
        TaskName = taskName;
        Parameters = parameters;
        ParentTaskId = parentTaskId;
    }

    public void SetResult(TData data)
    {
        Data = data;
    }

}
