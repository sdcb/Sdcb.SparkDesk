using System.Text.Json;
using Xunit.Abstractions;

namespace Sdcb.SparkDesk.Tests;

public class FunctionCallTests
{
    private readonly ITestOutputHelper _console;

    public FunctionCallTests(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task V3_ShouldSupportFunctionCall()
    {
        SparkDeskClient c = UnitTest1.CreateSparkDeskClient();
        await foreach (StreamedChatResponse resp in c.ChatAsStreamAsync(ModelVersion.V3,
        [
            ChatMessage.FromSystem("你是用户助理，当前日期：2024-03-09"),
            ChatMessage.FromUser("请问明天长沙天气如何？"),
        ], functions:
        [
            new FunctionDef("queryWeather", "查询天气",
            [
                new ("city", "string", "城市名"),
                new ("date", "date", "日期，必须是yyyy-MM-dd这样的形式"),
            ]),
            new FunctionDef("googleSearch", "谷歌搜索",
            [
                new ("query", "string", "搜索关键词"),
            ])
        ], uid: "zhoujie"))
        {
            Assert.NotNull(resp.FunctionCall);
            Assert.Equal("queryWeather", resp.FunctionCall.Name);
            JsonElement args = JsonDocument.Parse(resp.FunctionCall.Arguments).RootElement;
            Assert.Equal("长沙", args.GetProperty("city").GetString());
            Assert.Equal("2024-03-10", args.GetProperty("date").GetString());
        }
    }

    [Fact]
    public async Task V3_5_ShouldSupportFunctionCall_Direct()
    {
        SparkDeskClient c = UnitTest1.CreateSparkDeskClient();
        ChatResponse resp = await c.ChatAsync(ModelVersion.V3,
        [
            ChatMessage.FromSystem("你是用户助理，当前日期：2024-03-09"),
            ChatMessage.FromUser("请问明天长沙天气如何？"),
        ], functions:
        [
            new FunctionDef("queryWeather", "查询天气",
            [
                new ("city", "string", "城市名"),
                new ("date", "date", "日期，必须是yyyy-MM-dd这样的形式"),
                new ("unit", "enum", "温度单位，只能是：[摄氏度,华氏度]二选一"),
            ]),
            new FunctionDef("googleSearch", "谷歌搜索",
            [
                new ("query", "string", "搜索关键词"),
            ])
        ], uid: "zhoujie");

        Assert.NotNull(resp.FunctionCall);
        Assert.Equal("queryWeather", resp.FunctionCall.Name);
        JsonElement args = JsonDocument.Parse(resp.FunctionCall.Arguments).RootElement;
        Assert.Equal("长沙", args.GetProperty("city").GetString());
        Assert.Equal("2024-03-10", args.GetProperty("date").GetString());
        Assert.Equal("摄氏度", args.GetProperty("unit").GetString());
        _console.WriteLine(resp.Usage.ToString());
    }

    [Fact]
    public async Task V3_5_ShouldSupportFunctionCall_Response()
    {
        SparkDeskClient c = UnitTest1.CreateSparkDeskClient();
        ChatResponse resp = await c.ChatAsync(ModelVersion.V3,
        [
            ChatMessage.FromSystem("你是用户助理，当前日期：2024-03-09"),
            ChatMessage.FromUser("请问明天长沙天气如何？"),
            ChatMessage.FromUser("""
                function call result: 
                {
                    "city": "长沙",
                    "date": "2024-03-10",
                    "weather": "晴",
                    "temperature": "20",
                    "unit": "摄氏度"
                }
                """)
        ], uid: "zhoujie");

        _console.WriteLine(resp.Text);
        _console.WriteLine(resp.Usage.ToString());
    }

    [Fact]
    public async Task V3_5_ShouldSupportFunctionCall_ResponseStreamed()
    {
        SparkDeskClient c = UnitTest1.CreateSparkDeskClient();
        await foreach (StreamedChatResponse resp in c.ChatAsStreamAsync(ModelVersion.V3,
        [
            ChatMessage.FromSystem("你是用户助理，当前日期：2024-03-09"),
            ChatMessage.FromUser("请问明天长沙天气如何？"),
            ChatMessage.FromUser("""
                function call result: 
                {
                    "city": "长沙",
                    "date": "2024-03-10",
                    "weather": "晴",
                    "temperature": "20",
                    "unit": "摄氏度"
                }
                """)
        ], uid: "zhoujie"))
        {
            _console.WriteLine(resp.Text + resp.Usage);
        }
    }
}
