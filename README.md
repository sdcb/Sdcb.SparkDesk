# Sdcb.SparkDesk [![NuGet](https://img.shields.io/nuget/v/Sdcb.SparkDesk.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![NuGet](https://img.shields.io/nuget/dt/Sdcb.SparkDesk.svg?style=flat-square)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![GitHub](https://img.shields.io/github/license/sdcb/Sdcb.SparkDesk.svg?style=flat-square&label=license)](https://github.com/sdcb/Sdcb.SparkDesk/blob/master/LICENSE.txt)

**[English](README_EN.md)** | **简体中文**

`Sdcb.SparkDesk`是一个非官方的开源项目，提供SparkDesk WebSocket API (https://console.xfyun.cn/services/cbm) 的 .NET 客户端。现在已支持ModelVersion，用户可以选择包括V3模型在内的各种版本模型。上游文档地址：https://www.xfyun.cn/doc/spark/Guide.html

这个项目可以用来开发能够用自然语言与用户交流的聊天机器人和虚拟助手。

## 功能

- 为 SparkDesk API 提供 .NET 客户端。
- 支持同步和异步通信。
- 实现了流API，以实现实时通信。
- 为聊天机器人开发提供了简单直观的API。
- 支持Function Call API，支持模型：Pro/Max/V4_0_Ultra，允许用户调用自定义函数。
- 支持 ModelVersion（Lite, V2_0, Pro, Max, V4_0_Ultra），允许用户在不同版本的模型中选择。

## 安装

通过NuGet可以安装 `Sdcb.SparkDesk`。 在程序包管理器控制台中运行以下命令以安装软件包：

```
Install-Package Sdcb.SparkDesk
```

## 使用方法

要使用 Sdcb.SparkDesk，您需要创建一个 `SparkDeskClient` 类实例。您可以通过将SparkDesk API凭据传递给构造函数来创建客户端：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
```

### 示例1：与虚拟助手聊天（Lite模型）

以下示例显示了如何使用 `ChatAsync` 方法与虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
ChatResponse response = await client.ChatAsync(ModelVersion.Lite, new ChatMessage[] 
{
    ChatMessage.FromUser("系统提示：你叫张三，一名5岁男孩，你在金色摇篮幼儿园上学，你的妈妈叫李四，是一名工程师"),
    ChatMessage.FromUser("你好小朋友，我是周老师，你在哪上学？"),
});
Console.WriteLine(response.Text);
```

### 示例2：使用流API与虚拟助手聊天（V2.0模型）

以下示例显示了如何使用 `ChatAsStreamAsync` 方法和V2模型以及流API与虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
await foreach (StreamedChatResponse msg in client.ChatAsStreamAsync(ModelVersion.V2_0, new ChatMessage[] { ChatMessage.FromUser("湖南的省会在哪？") }, new ChatRequestParameters
{
    ChatId = "test",
    MaxTokens = 20,
    Temperature = 0.5f,
    TopK = 4,
}))
{
    Console.WriteLine(msg);
}
```

### 示例3：使用流API和回调与虚拟助手聊天 (Pro模型)

以下示例显示了如何使用 `ChatAsStreamAsync` 方法以及Pro模型、流API和回调与虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
StringBuilder sb = new();
TokensUsage usage = await client.ChatAsStreamAsync(ModelVersion.Pro, new ChatMessage[] 
{ 
    ChatMessage.FromUser("1+1=?"),
    ChatMessage.FromAssistant("1+1=3"),
    ChatMessage.FromUser("不对啊，请再想想？")
}, s => sb.Append(s), uid: "zhoujie");

string realResponse = sb.ToString();
Console.WriteLine(realResponse);
```

### 示例4：使用流API的控制台聊天机器人（Lite模型）

以下示例显示了如何跟踪会话历史并使用流API与Lite模型的虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);

List<ChatMessage> conversation = new List<ChatMessage>();
while (true)
{
    string? prompt = Console.ReadLine();
    if (prompt == null || prompt == "q")
    {
        break;
    }

    conversation.Add(ChatMessage.FromUser(prompt));
    Console.WriteLine($"> {prompt}");
    
    StringBuilder resp = new StringBuilder();
    await foreach (string c in client.ChatAsStreamAsync(ModelVersion.Lite, conversation.ToArray()))
    {
        resp.Append(resp);
        Console.Write(c);
    }
    Console.WriteLine();
    conversation.Add(ChatMessage.FromAssistant(resp.ToString()));
}
```

### 示例5：使用Function Call API（V3.0/V3.5/V4.0_Ultra模型支持）

为了创建一个使用Function Call API的示例，我们首先需要定义一个或多个`FunctionDef`对象，代表我们要调用的函数。之后，我们会在`ChatAsync`的调用中使用这些`FunctionDef`对象，并处理返回的`ChatResponse`以获取`FunctionCall`的详情。

我们将创建一个Function Call来查询天气，并验证返回的`FunctionCall`对象含有正确的函数名和参数。

下面是如何实现此示例的代码：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);

ChatResponse response = await client.ChatAsync(ModelVersion.Max,
    new ChatMessage[]
    {
        ChatMessage.FromSystem("你是用户助理，当前日期：2024-03-09"),
        ChatMessage.FromUser("请问明天长沙天气如何？"),
    },
    functions: new FunctionDef[]
    {
        new FunctionDef("queryWeather", "查询天气",
        new []
        {
            new FunctionParametersDef("city", "string", "城市名"),
            new FunctionParametersDef("date", "date", "日期，必须是yyyy-MM-dd这样的形式"),
            new FunctionParametersDef("unit", "enum", "温度单位，只能是：[摄氏度,华氏度]二选一"),
        }),
        new FunctionDef("googleSearch", "谷歌搜索",
        new []
        {
            new FunctionParametersDef("query", "string", "搜索关键词"),
        })
    },
    uid: "zhoujie");

// 处理返回的响应
if (response.FunctionCall != null)
{
    Console.WriteLine($"Function Call: {response.FunctionCall.Name}");
    var args = response.FunctionCall.Arguments; // 这是一个JSON字符串
    Console.WriteLine($"Arguments: {args}");
}
else
{
    Console.WriteLine("Function Call not triggered.");
}
```

根据代码，我们期望`response.FunctionCall`中包含的函数名是`"queryWeather"`，而且参数应该包含城市名`"长沙"`，日期`"2024-03-10"`和温度单位`"摄氏度"`。

这个示例中，我们没有实际实现Function Call的真正查询天气逻辑。假设你已经将天气查询完成，我们可以使用一个带有Function Call结果的`ChatMessage.FromUser`，这样假设Function Call已经被触发并完成了，然后注入结果用于接下来的聊天。

```csharp
StringBuilder sb = new StringBuilder();

await foreach (StreamedChatResponse msg in client.ChatAsStreamAsync(ModelVersion.V4_0_Ultra, new ChatMessage[]
{
    ChatMessage.FromSystem("你是用户助理，当前日期：2024-03-09"),
    ChatMessage.FromUser("请问明天长沙天气如何？"),
    ChatMessage.FromUser(@"
        function call result: 
        {
            ""city"": ""长沙"",
            ""date"": ""2024-03-10"",
            ""weather"": ""晴"",
            ""temperature"": ""20"",
            ""unit"": ""摄氏度""
        }
    ")
}, uid: "zhoujie"))
{
    Console.Write(msg.Text + msg.Usage);
}
```

输出示例如下：

```
明天，也就是2024年3月10日，长沙的天气将会是晴朗的。
预计的温度会达到20摄氏度。
TokensUsage { QuestionTokens = 68, PromptTokens = 91, CompletionTokens = 30, TotalTokens = 121 }
```

## Breaking Changes

在最新版本的 `Sdcb.SparkDesk` 中，我们对模型版本的命名和配置进行了一些重大更改。以下是主要的更改点，这些更改可能会影响到已有的代码实现：

- **模型版本重命名**：
  - 之前的 `V1_5` 版本现在被命名为 `Lite`。
  - 之前的 `V2` 版本现在被命名为 `V2_0`。
  - 之前的 `V3` 版本现在被命名为 `Pro`。
  - 之前的 `V3_5` 版本现在被命名为 `Max`。

- **新增模型版本**：
  - 添加了新的 `V4_0_Ultra` 模型版本，这是目前提供的最高级的模型配置。

- **模型配置方式更改**：
  - 新版本中每个 `ModelVersion` 实例都包含 `DisplayName`，`Domain` 和 `AddressPart` 三个属性，而不仅仅是一个简单的枚举值。这改善了模型的灵活性并增强了代码的可扩展性。

- **向后兼容性**：
  - 如果您的代码之前直接使用了 `V1_5`, `V2`, `V3`, `V3_5` 这些枚举值，您将需要更新代码，以使用新的模型版本对象。这将确保您可以利用新的 API 结构和扩展的功能。

这些更改旨在与官方最新的模型名字相对应，并提升库的灵活性和未来的可扩展性。应用这些更改后，您将能够更容易地配置和使用不同复杂度和能力的模型，以符合您的具体需求。

请检查您的应用程序以确认这些更改，并更新您的代码以适应新的库版本。如果有任何问题或需要进一步的说明，请查阅[官方文档](https://www.xfyun.cn/doc/spark/Web.html)或联系我们的支持团队。

## 许可证

Sdcb.SparkDesk 遵循 MIT 许可证。 请参阅[LICENSE.txt](LICENSE.txt)文件以获取更多信息。
