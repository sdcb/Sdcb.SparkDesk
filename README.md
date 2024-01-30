# Sdcb.SparkDesk [![NuGet](https://img.shields.io/nuget/v/Sdcb.SparkDesk.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![NuGet](https://img.shields.io/nuget/dt/Sdcb.SparkDesk.svg?style=flat-square)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![GitHub](https://img.shields.io/github/license/sdcb/Sdcb.SparkDesk.svg?style=flat-square&label=license)](https://github.com/sdcb/Sdcb.SparkDesk/blob/master/LICENSE.txt)

**[English](README_EN.md)** | **简体中文**

`Sdcb.SparkDesk`是一个非官方的开源项目，提供SparkDesk WebSocket API (https://console.xfyun.cn/services/cbm) 的 .NET 客户端。现在已支持ModelVersion，用户可以选择包括V3模型在内的各种版本模型。上游文档地址：https://www.xfyun.cn/doc/spark/Guide.html

这个项目可以用来开发能够用自然语言与用户交流的聊天机器人和虚拟助手。

## 功能

- 为 SparkDesk API 提供 .NET 客户端。
- 支持同步和异步通信。
- 实现了流API，以实现实时通信。
- 为聊天机器人开发提供了简单直观的API。
- 支持 ModelVersion（V1_5, V2, V3, V3_5），允许用户在不同版本的模型中选择。

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

### 示例1：与虚拟助手聊天（V1.5模型）

以下示例显示了如何使用 `ChatAsync` 方法与虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
ChatResponse response = await client.ChatAsync(ModelVersion.V1_5, new ChatMessage[] 
{
    ChatMessage.FromUser("系统提示：你叫张三，一名5岁男孩，你在金色摇篮幼儿园上学，你的妈妈叫李四，是一名工程师"),
    ChatMessage.FromUser("你好小朋友，我是周老师，你在哪上学？"),
});
Console.WriteLine(response.Text);
```

### 示例2：使用流API与虚拟助手聊天（V2模型）

以下示例显示了如何使用 `ChatAsStreamAsync` 方法和V2模型以及流API与虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
await foreach (StreamedChatResponse msg in client.ChatAsStreamAsync(ModelVersion.V2, new ChatMessage[] { ChatMessage.FromUser("湖南的省会在哪？") }, new ChatRequestParameters
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

### 示例3：使用流API和回调与虚拟助手聊天 (V3模型)

以下示例显示了如何使用 `ChatAsStreamAsync` 方法以及V3模型、流API和回调与虚拟助手聊天：

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
StringBuilder sb = new();
TokensUsage usage = await client.ChatAsStreamAsync(ModelVersion.V3, new ChatMessage[] 
{ 
    ChatMessage.FromUser("1+1=?"),
    ChatMessage.FromAssistant("1+1=3"),
    ChatMessage.FromUser("不对啊，请再想想？")
}, s => sb.Append(s), uid: "zhoujie");

string realResponse = sb.ToString();
Console.WriteLine(realResponse);
```

### 示例4：使用流API的控制台聊天机器人（V1.5模型）

以下示例显示了如何跟踪会话历史并使用流API与V1.5模型的虚拟助手聊天：

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
    await foreach (string c in client.ChatAsStreamAsync(ModelVersion.V1_5, conversation.ToArray()))
    {
        resp.Append(resp);
        Console.Write(c);
    }
    Console.WriteLine();
    conversation.Add(ChatMessage.FromAssistant(resp.ToString()));
}
```

## 许可证

Sdcb.SparkDesk 遵循 MIT 许可证。 请参阅[LICENSE.txt](LICENSE.txt)文件以获取更多信息。
