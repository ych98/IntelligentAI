# IntelligentAI

`IntelligentAI` 是一个面向多家大模型服务商的 .NET 封装库。当前版本基于 .NET 10，使用配置文件注册模型，并通过稳定的模型编号调用具体模型。

## 安装

```bash
dotnet add package IntelligentAI
```

当前主要依赖：

```xml
<PackageReference Include="FluentHttpFactory" Version="1.2.2" />
<PackageReference Include="Microsoft.Extensions.Http" Version="10.0.0" />
```

## 注册服务

在应用启动时注册 AI Client 工厂：

```csharp
builder.Services.AddAiClients();
```

注册后可以通过 `IAiClientFactory` 获取模型 Client：

```csharp
public class DemoService
{
    private readonly IAiClientFactory _aiClientFactory;

    public DemoService(IAiClientFactory aiClientFactory)
    {
        _aiClientFactory = aiClientFactory;
    }
}
```

## 配置模型

在 `appsettings.json` 中配置 `AI` 节点。

```json
{
  "AI": [
    {
      "Service": "Kimi",
      "Host": "https://api.moonshot.cn",
      "ApiKey": "sk-kimi",
      "Models": [
        {
          "Id": 1,
          "Name": "moonshot-v1-8k"
        }
      ]
    },
    {
      "Service": "Aliyun",
      "Host": "https://dashscope.aliyuncs.com",
      "ApiKey": "sk-aliyun",
      "ChatUrl": "/api/v1/services/aigc/text-generation/generation",
      "Models": [
        {
          "Id": 6,
          "Name": "qwen-long"
        }
      ]
    },
    {
      "Service": "OpenAI",
      "Host": "https://dashscope.aliyuncs.com",
      "ApiKey": "sk-aliyun",
      "ChatUrl": "/compatible-mode/v1/chat/completions",
      "Models": [
        {
          "Id": 17,
          "Name": "qwen-long"
        }
      ]
    },
    {
      "Service": "OpenAI",
      "Host": "https://api.openai.com",
      "ApiKey": "sk-openai",
      "ChatUrl": "/v1/chat/completions",
      "Models": [
        {
          "Id": 19,
          "Name": "gpt-4o"
        }
      ]
    }
  ]
}
```

### 配置说明

- `Service` 决定使用哪个 Client。
- `Host` 是服务商 API 根地址。
- `ApiKey` 是服务商密钥。
- `ChatUrl` 是聊天接口路径。
- `Models[].Id` 是对调用方稳定暴露的模型编号，必须唯一。
- `Models[].Name` 是实际传给服务商的模型名称。

当前内置 Service 映射：

```text
Aliyun  -> AliyunAiClient
Kimi    -> KimiAiClient
Azure   -> AzureAiClient
OpenAI  -> OpenAiClient
Huoshan -> HuoshanAiClient
Baidu   -> BaiduAiClient
Google  -> GoogleAiClient
```

只有当 `Service` 配置为 `OpenAI` 时，才会使用 OpenAI-compatible 请求格式。也就是说，阿里云、火山云等服务如果要通过 OpenAI 兼容接口调用，应把该配置项的 `Service` 写为 `OpenAI`，再自行配置对应的 `Host`、`ApiKey` 和 `ChatUrl`。

## 文本问答

通过模型编号创建 Client：

```csharp
var client = _aiClientFactory.CreateClient(6);

var request = new AiArguments("hello");

var answer = await client.AnswerText(
    request.Question,
    request.ToDictionary(),
    request.Messages,
    cancellation);
```

也可以通过 `Service + ModelName` 创建 Client：

```csharp
var client = _aiClientFactory.CreateClient("Aliyun", "qwen-long");
```

## 流式问答

```csharp
var client = _aiClientFactory.CreateClient(6);
var request = new AiArguments("请介绍一下杭州西湖");

await foreach (var message in client.AnswerStream(
    request.Question,
    request.ToDictionary(),
    request.Messages,
    cancellation))
{
    Console.Write(message);
}
```

## 获取已配置模型

```csharp
var models = _aiClientFactory.GetClients();

foreach (var model in models)
{
    Console.WriteLine($"{model.Id}: {model.Service} - {model.Name}");
}
```

## 稳定模型编号

推荐把 `Models[].Id` 视为对外契约，而不是把服务商模型名暴露给调用方。

例如调用方长期传入：

```text
modelId = 6
```

服务端可以在配置中把编号 `6` 从旧模型切到新模型：

```json
{
  "Id": 6,
  "Name": "qwen-long-2026"
}
```

调用方无需感知这次模型升级。

## 自定义 Client

如果要扩展新的服务商，可以继承 `AiClientBase`：

```csharp
public class MyAiClient : AiClientBase
{
    public MyAiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public override Task<string> AnswerText(
        string question,
        Dictionary<string, object>? parameters = null,
        Message[]? messages = null,
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override IAsyncEnumerable<string> AnswerStream(
        string question,
        Dictionary<string, object>? parameters = null,
        Message[]? messages = null,
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<string[]> AnswerImages(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<string> AnswerVideo(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, object> GetParameters(
        string method,
        Dictionary<string, object>? overrides = null,
        MissingKeyBehavior missingKeyBehavior = MissingKeyBehavior.Error)
    {
        return new Dictionary<string, object>();
    }
}
```

然后注册：

```csharp
builder.Services.AddCustomAiClient<MyAiClient>(
    "MyService",
    new AIProviderSettings
    {
        Service = "MyService",
        Host = "https://api.example.com",
        ApiKey = "sk-example",
        ChatUrl = "/v1/chat/completions",
        Models =
        [
            new AIModelSettings
            {
                Id = 100,
                Service = "MyService",
                Name = "my-model"
            }
        ]
    });
```

## 注意事项

- `Id` 必须唯一，否则启动注册模型时会抛出异常。
- `Service` 必须是已内置或已自定义注册的服务名。
- `ChatUrl` 不会被库自动修正；如果服务商接口路径变化，请改配置。
- `ApiKey` 请放在用户机密、环境变量或部署平台密钥中，不建议提交到仓库。
