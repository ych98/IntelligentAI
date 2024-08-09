﻿using IntelligentAI.Models;
using IntelligentAI.Records;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace IntelligentAI.Abstraction;

public interface IAiModelEventManager
{
    IAsyncEnumerable<AiProgressResult> StartTasksAsync(
        AiModelBase model,
        Guid eventId,
        Guid parentTaskId,
        IEnumerable<AiArguments> tasks,
        string taskName = "EventTasks",
        CancellationToken cancellation = default);

    Task<bool> IsBusy(AiModelBase model, CancellationToken cancellation = default);

}

