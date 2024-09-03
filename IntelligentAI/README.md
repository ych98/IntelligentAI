# Code Setup

## Getting Started

### Using our dotnet package

To start using the Intelligent SDK from scratch,  you first need to install the main Nuget package in the project you want to use the components. You can use the NuGet package manager in your IDE for that or use the following command when using a CLI:

```CSharp

dotnet add package IntelligentAI

```

### Register Services

Add the following in Program.cs:

```CSharp 

builder.Services.AddAiService(new AiOptions("intelligent api address"));

```

### Quick Start

This is literally all you need in your services  to use intelligent components.

```CSharp 

private readonly IAiModelService _model;

public YourClass(IAiModelService model)
{
    _model = model;
}

```

**Argument**

```CSharp 

var request = new AiArguments("杭州西湖哪里好玩？");

int model = 6;  // 编号为 6 代表阿里云在线 qwen-long 模型

var _cts = new CancellationTokenSource();

var promptId = "prompt10";

var replaces = new Dictionary<string, string>()
{
    ["正文"] = Content  // 提示词占位符替换
};

```

**Text answer**

```CSharp 

var answer = await  _model.AnswerTextAsync(request, model, cancellationToken: _cts.Token);

```

**Streaming answer**

```CSharp 

StringBuilder streamingContentBuilder = new StringBuilder();

string botAnswer;

// ReturnType: IAsyncEnumerable<string>
// Requirements: .net 8.0 or above
var stream = _model.AnswerStringsAsync(request, model, cancellationToken: _cts.Token);

// ContentType: text/event-stream
var stream = _model.AnswerStreamAsync(request, model, cancellationToken: _cts.Token);

await foreach (var message in stream)
{
    streamingContentBuilder.Append(message);

    // You can get the answer here
    botAnswer = streamingContentBuilder.ToString(); 
}

// Or you can get the full answer here
botAnswer = streamingContentBuilder.ToString(); 

```

**Prompt answer**

```CSharp 

var answer = await _model.AnswerTextByPromptAsync(promptId, replaces, model, cancellationToken);

```
