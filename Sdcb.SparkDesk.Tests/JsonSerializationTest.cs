using System.Text.Json;
using Sdcb.SparkDesk.RequestInternals;
using Sdcb.SparkDesk.ResponseInternals;

namespace Sdcb.SparkDesk.Tests;

public class JsonSerializationTest
{
    [Fact]
    public void RequestCanBeSerialized()
    {
        string req = """
            {
                "header": {
                    "app_id": "12345",
                    "uid": "12345"
                },
                "parameter": {
                    "chat": {
                        "domain": "general",
                        "temperature": 0.5,
                        "max_tokens": 1024
                    }
                },
                "payload": {
                    "message": {
                        "text": [
                            {"role": "user", "content": "你是谁"},
                            {"role": "assistant", "content": "....."},
                            {"role": "user", "content": "你会做什么"}
                        ]
                    }
                }
            }
            """;
        ChatApiRequest? request = JsonSerializer.Deserialize<ChatApiRequest>(req);
        Assert.NotNull(req);
        Assert.Equal("12345", request!.Header.AppId);
    }

    [Fact]
    public void ResponseCanBeSerialized()
    {
        string req = """
            {
                "header":{
                    "code":0,
                    "message":"Success",
                    "sid":"cht000cb087@dx18793cd421fb894542",
                    "status":2
                },
                "payload":{
                    "choices":{
                        "status":2,
                        "seq":0,
                        "text":[
                            {
                                "content":"我可以帮助你的吗？",
                                "role":"assistant",
                                "index":0
                            }
                        ]
                    },
                    "usage":{
                        "text":{
                            "question_tokens":4,
                            "prompt_tokens":5,
                            "completion_tokens":9,
                            "total_tokens":14
                        }
                    }
                }
            }
            """;
        ChatApiResponse? resp = JsonSerializer.Deserialize<ChatApiResponse>(req);
        Assert.NotNull(resp);
        Assert.Equal("我可以帮助你的吗？", resp!.Payload!.Choices.Text[0].Content);
    }
}
