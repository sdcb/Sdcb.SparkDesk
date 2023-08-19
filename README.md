# Sdcb.SparkDesk [![NuGet](https://img.shields.io/nuget/v/Sdcb.SparkDesk.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![NuGet](https://img.shields.io/nuget/dt/Sdcb.SparkDesk.svg?style=flat-square)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![GitHub](https://img.shields.io/github/license/sdcb/Sdcb.SparkDesk.svg?style=flat-square&label=license)](https://github.com/sdcb/Sdcb.SparkDesk/blob/master/LICENSE.txt)

`Sdcb.SparkDesk` is an unofficial open-source project that provides a .NET client for SparkDesk WebSocket API(https://console.xfyun.cn/services/cbm). Now, it supports the ModelVersion allowing users to choose between different versions of models including V2 model. The upstream document is at: https://www.xfyun.cn/doc/spark/Guide.html

This project can be used to build chatbots and virtual assistants that can communicate with users in natural language.

`Sdcb.SparkDesk`是星火大模型WebSocket API(https://console.xfyun.cn/services/cbm)的非官方开源.NET客户端。现在，它支持ModelVersion，允许用户选择不同版本的模型，包括V2模型。

## Features

- Provides a .NET client for the SparkDesk API
- Supports both synchronous and asynchronous communication
- Implements streaming APIs for real-time communication
- Provides a simple and intuitive API for chatbot development
- Supports ModelVersion allowing users to choose between different versions of models

## Installation

`Sdcb.SparkDesk` can be installed using NuGet. To install the package, run the following command in the Package Manager Console:

```
Install-Package Sdcb.SparkDesk
```

## Usage

To use Sdcb.SparkDesk, you need to create an instance of the `SparkDeskClient` class. You can create the client by passing your SparkDesk API credentials to the constructor:

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
```

### Example 1: Chatting with a virtual assistant (V1 model)

The following example shows how to use the `ChatAsync` method to chat with a virtual assistant:

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
ChatResponse response = await client.ChatAsync(ModelVersion.V1_5, new ChatMessage[] 
{
    ChatMessage.FromUser("系统提示：你叫张三，一名5岁男孩，你在金色摇篮幼儿园上学，你的妈妈叫李四，是一名工程师"),
    ChatMessage.FromUser("你好小朋友，我是周老师，你在哪上学？"),
});
Console.WriteLine(response.Text);
```

### Example 2: Chatting with a virtual assistant using streaming API (V2 model)

The following example shows how to use the `ChatAsStreamAsync` method to chat with a virtual assistant using V2 model and streaming API:

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

### Example 3: Chatting with a virtual assistant using streaming API and callback (V1.5 model)

The following example shows how to use the `ChatAsStreamAsync` method to chat with a virtual assistant using V1.5 model, streaming API and callback:

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
StringBuilder sb = new();
TokensUsage usage = await client.ChatAsStreamAsync(ModelVersion.V1_5, new ChatMessage[] 
{ 
    ChatMessage.FromUser("1+1=?"),
    ChatMessage.FromAssistant("1+1=3"),
    ChatMessage.FromUser("不对啊，请再想想？")
}, s => sb.Append(s), uid: "zhoujie");

string realResponse = sb.ToString();
Console.WriteLine(realResponse);
```

### Example 4: A console chatting bot using streaming API (V1.5 model):

The following example shows how to self track the conversation history and chat with a virtual assistant using streaming API and V1.5 model:

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

## License

Sdcb.SparkDesk is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for more information.
