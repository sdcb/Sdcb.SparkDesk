# Sdcb.SparkDesk [![NuGet](https://img.shields.io/nuget/v/Sdcb.SparkDesk.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![NuGet](https://img.shields.io/nuget/dt/Sdcb.SparkDesk.svg?style=flat-square)](https://www.nuget.org/packages/Sdcb.SparkDesk/) [![GitHub](https://img.shields.io/github/license/sdcb/Sdcb.SparkDesk.svg?style=flat-square&label=license)](https://github.com/sdcb/Sdcb.SparkDesk/blob/master/LICENSE.txt)

**English** | **[简体中文](README.md)**

`Sdcb.SparkDesk` is an unofficial open-source project that provides a .NET client for SparkDesk WebSocket API(https://console.xfyun.cn/services/cbm). Now, it supports the ModelVersion allowing users to choose between different versions of models including V3 model. The upstream document is at: https://www.xfyun.cn/doc/spark/Guide.html

This project can be used to build chatbots and virtual assistants that can communicate with users in natural language.

## Features

- Provides a .NET client for the SparkDesk API
- Supports both synchronous and asynchronous communication
- Implements streaming APIs for real-time communication
- Provides a simple and intuitive API for chatbot development
- Supports ModelVersion(V1_5, V2, V3, V3_5) allowing users to choose between different versions of models
- Supports Function Call API in V3.0/V3.5 models

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

### Example 1: Chatting with a virtual assistant (V1.5 model)

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

### Example 3: Chatting with a virtual assistant using streaming API and callback (V3 model)

The following example shows how to use the `ChatAsStreamAsync` method to chat with a virtual assistant using V3 model, streaming API and callback:

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

### Example 5: Using Function Call API (Supported in V3.0/V3.5 Models)

To create an example using the Function Call API, we first need to define one or more `FunctionDef` objects, representing the functions we want to invoke. Afterward, we will use these `FunctionDef` objects in our call to `ChatAsync` and handle the returned `ChatResponse` to obtain the details of the `FunctionCall`.

We will create a Function Call to check the weather and verify that the returned `FunctionCall` object contains the correct function name and parameters.

Below is the code to implement this example:

```csharp
SparkDeskClient client = new SparkDeskClient(appId, apiKey, apiSecret);

ChatResponse response = await client.ChatAsync(ModelVersion.V3_5,
    new ChatMessage[]
    {
        ChatMessage.FromSystem("You are the user assistant, current date: 2024-03-09"),
        ChatMessage.FromUser("How is the weather in Changsha tomorrow?"),
    },
    functions: new FunctionDef[]
    {
        new FunctionDef("queryWeather", "Query weather",
        new []
        {
            new FunctionParametersDef("city", "string", "City name"),
            new FunctionParametersDef("date", "date", "Date, must be in the form of yyyy-MM-dd"),
            new FunctionParametersDef("unit", "enum", "Temperature unit, can only be one of the following: [Celsius, Fahrenheit]"),
        }),
        new FunctionDef("googleSearch", "Google search",
        new []
        {
            new FunctionParametersDef("query", "string", "Search keywords"),
        })
    },
    uid: "zhoujie");

// Handle the returned response
if (response.FunctionCall != null)
{
    Console.WriteLine($"Function Call: {response.FunctionCall.Name}");
    var args = response.FunctionCall.Arguments; // This is a JSON string
    Console.WriteLine($"Arguments: {args}");
}
else
{
    Console.WriteLine("Function Call not triggered.");
}
```

According to the code, we expect the function name in `response.FunctionCall` to be `"queryWeather"`, and the parameters should include the city name `"Changsha"`, the date `"2024-03-10"`, and the temperature unit `"Celsius"`.

In this example, we do not implement the actual weather query logic of the Function Call. Assuming that the weather query has been completed, we can use a `ChatMessage.FromUser` with the Function Call result, as if the Function Call has been triggered and completed, and then inject the outcome for the subsequent chat.

```csharp
StringBuilder sb = new StringBuilder();

await foreach (StreamedChatResponse msg in client.ChatAsStreamAsync(ModelVersion.V3_5, new ChatMessage[]
{
    ChatMessage.FromSystem("You are the user assistant, current date: 2024-03-09"),
    ChatMessage.FromUser("How is the weather in Changsha tomorrow?"),
    ChatMessage.FromUser(@"
        function call result: 
        {
            ""city"": ""Changsha"",
            ""date"": ""2024-03-10"",
            ""weather"": ""Clear"",
            ""temperature"": ""20"",
            ""unit"": ""Celsius""
        }
    ")
}, uid: "zhoujie"))
{
    sb.AppendLine(msg.Text);
    Console.WriteLine(msg.Text + msg.Usage);
}
```

The output example is as follows:

```
Tomorrow, which is March 10, 2024, the weather in Changsha will be clear.
The expected temperature will reach 20 degrees Celsius.
TokensUsage { QuestionTokens = 68, PromptTokens = 91, CompletionTokens = 30, TotalTokens = 121 }
```

## License

Sdcb.SparkDesk is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for more information.
