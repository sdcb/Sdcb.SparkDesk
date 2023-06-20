using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Sdcb.SparkDesk;

public class SparkDeskClient
{
    private const string HostUrl = "wss://spark-api.xf-yun.com/v1.1/chat";
    private readonly string _appId;
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public SparkDeskClient(string appId, string apiKey, string apiSecret)
    {
        _appId = appId;
        _apiKey = apiKey;
        _apiSecret = apiSecret;
    }

    public async IAsyncEnumerable<string> ChatAsync(ChatRequestParameters parameters, ChatMessage[] messages, string? uid = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string authUrl = GetAuthorizationUrl(HostUrl, _apiKey, _apiSecret);

        await foreach (string item in WebSocketConnection(new ChatApiRequest
        {
            Header = new Header { AppId = _appId, Uid = uid },
            Parameter = new Parameters { Chat = parameters },
            Payload = new Payload
            {
                Message = new Message { Text = messages }
            },
        }, authUrl, cancellationToken))
        {
            yield return item;
        }
    }

    public static string GetAuthorizationUrl(string apiKey, string apiSecret, string hostUrl = HostUrl)
    {
        var url = new Uri(hostUrl);

        string dateString = DateTime.UtcNow.ToString("r");

        byte[] signatureBytes = Encoding.ASCII.GetBytes($"host: {url.Host}\ndate: {dateString}\nGET {url.AbsolutePath} HTTP/1.1");

        using HMACSHA256 hmacsha256 = new(Encoding.ASCII.GetBytes(apiSecret));
        byte[] computedHash = hmacsha256.ComputeHash(signatureBytes);
        string signature = Convert.ToBase64String(computedHash);

        string authorizationString = $"api_key=\"{apiKey}\",algorithm=\"hmac-sha256\",headers=\"host date request-line\",signature=\"{signature}\"";
        string authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(authorizationString));

        string query = $"authorization={authorization}&date={dateString}&host={url.Host}";

        return new UriBuilder(url) { Scheme = url.Scheme, Query = query }.ToString();
    }

    private static async IAsyncEnumerable<string> WebSocketConnection(ChatApiRequest request, string url, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using ClientWebSocket webSocket = new();
        Uri uri = new(url);

        await webSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);

        var messageBuffer = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(request));
        await webSocket.SendAsync(messageBuffer, WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);

        byte[] buffer = new byte[65536];
        WebSocketReceiveResult result;

        do
        {
            ArraySegment<byte> arraySegment = new(buffer);
            result = await webSocket.ReceiveAsync(arraySegment, cancellationToken).ConfigureAwait(false);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("服务器连接关闭.");
            }
            else
            {
                ApiResponse resp = (await JsonSerializer.DeserializeAsync<ApiResponse>(new MemoryStream(buffer, 0, result.Count), cancellationToken: cancellationToken))!;
                yield return string.Concat(resp.Payload!.Choices.Text.Select(x => x.Content));
                if (resp.Header.Status == 2)
                {
                    break;
                }
            }

        } while (!cancellationToken.IsCancellationRequested);

        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "客户端主动断开链接", cancellationToken);
        }
    }
}
