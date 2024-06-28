# Sdcb.SparkDesk [![NuGet](https://img.shields.io/nuget/v/Sdcb.SparkDesk.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![NuGet](https://img.shields.io/nuget/dt/Sdcb.SparkDesk.svg?style=flat-square)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![GitHub](https://img.shields.io/github/license/sdcb/Sdcb.SparkDesk.svg?style=flat-square&label=license)](https://github.com/sdcb/Sdcb.SparkDesk/blob/master/LICENSE.txt)

**English** | **[简体中文](README.md)**

`Sdcb.SparkDesk` is an unofficial open-source project that provides a .NET client for SparkDesk WebSocket API(https://console.xfyun.cn/services/cbm). Now, it supports the ModelVersion allowing users to choose between different versions of models including V3 model. The upstream document is at: https://www.xfyun.cn/doc/spark/Guide.html

This project can be used to build chatbots and virtual assistants that can communicate with users in natural language.

## Features

- Provides a .NET client for the SparkDesk API
- Supports both synchronous and asynchronous communication
- Implements streaming APIs for real-time communication
- Provides a simple and intuitive API for chatbot development
- Supports ModelVersion(Lite, V2_0, Pro, Max, V4_0_Ultra) allowing users to choose between different versions of models

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

### Example 1: Chatting with a virtual assistant (Lite model)

The following example shows how to use the `ChatAsync` method to chat with a virtual assistant:

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);
ChatResponse response = await client.ChatAsync(ModelVersion.Lite, new ChatMessage[] 
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

### Example 3: Chatting with a virtual assistant using streaming API and callback (Pro model)

The following example shows how to use the `ChatAsStreamAsync` method to chat with a virtual assistant using V3 model, streaming API and callback:

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

### Example 4: A console chatting bot using streaming API (Lite model):

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
    await foreach (string c in client.ChatAsStreamAsync(ModelVersion.Lite, conversation.ToArray()))
    {
        resp.Append(resp);
        Console.Write(c);
    }
    Console.WriteLine();
    conversation.Add(ChatMessage.FromAssistant(resp.ToString()));
}
```

## Breaking Changes

In the latest version of `Sdcb.SparkDesk`, we have made significant changes to the naming and configuration of model versions. Here are the main changes that may affect existing implementations:

- **Model Version Renaming**:
  - The previous `V1_5` version is now named `Lite`.
  - The previous `V2` version is now named `V2_0`.
  - The previous `V3` version is now named `Pro`.
  - The previous `V3_5` version is now named `Max`.

- **New Model Version Added**:
  - A new `V4_0_Ultra` model version has been added, which represents the most advanced model configuration available.

- **Changes in Model Configuration**:
  - In the new version, each `ModelVersion` instance includes `DisplayName`, `Domain`, and `AddressPart` attributes, rather than just a simple enum value. This improves the flexibility of the model and enhances code extensibility.

- **Backward Compatibility**:
  - If your code previously directly used the enum values `V1_5`, `V2`, `V3`, `V3_5`, you will need to update your code to use the new model version objects. This will ensure you can take advantage of the new API structure and expanded functionalities.

These changes are intended to align with the official latest model names and enhance the flexibility and future scalability of the library. After applying these changes, you will be able to more easily configure and use models of different complexities and capabilities to meet your specific needs.

Please review your application to confirm these changes and update your code to accommodate the new library version. If you have any issues or need further clarification, please refer to the [official documentation](https://www.xfyun.cn/doc/spark/Web.html) or contact our support team.

## License

Sdcb.SparkDesk is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for more information.
