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
using System.Threading.Tasks;
using Sdcb.SparkDesk.ResponseInternals;
using Sdcb.SparkDesk.RequestInternals;

[assembly: InternalsVisibleTo("Sdcb.SparkDesk.Tests")]

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents a client for interacting with the SparkDesk API.
/// </summary>
public class SparkDeskClient
{
    private const string HostUrl = "wss://spark-api.xf-yun.com/v1.1/chat";
    private readonly string _appId;
    private readonly string _apiKey;
    private readonly string _apiSecret;

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskClient"/> class with specified parameters.
    /// </summary>
    /// <param name="appId">The app ID.</param>
    /// <param name="apiKey">The API key.</param>
    /// <param name="apiSecret">The API Secret.</param>
    public SparkDeskClient(string appId, string apiKey, string apiSecret)
    {
        _appId = appId ?? throw new ArgumentNullException(nameof(appId));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
    }

    /// <summary>
    /// Sends chat messages to SparkDesk API through websockets and receives response streams asynchronously.
    /// </summary>
    /// <param name="messages">Array of chat messages to send to SparkDesk API.</param>
    /// <param name="parameters">Optional parameters to modify chat request.</param>
    /// <param name="uid">Optional user ID to associate with the chat messages.</param>
    /// <param name="cancellationToken">Optional cancellation token to stop the operation.</param>
    /// <returns>Asynchronous task that returns a ChatResponse object containing the streamed chat response.</returns>
    public async Task<ChatResponse> ChatAsync(ChatMessage[] messages, ChatRequestParameters? parameters = null, string? uid = null, CancellationToken cancellationToken = default)
    {
        List<StreamedChatResponse> resps = new();
        await foreach (StreamedChatResponse msg in ChatAsStreamAsync(messages, parameters, uid, cancellationToken))
        {
            resps.Add(msg);
        }

        return new ChatResponse(resps);
    }

    /// <summary>
    /// Sends chat messages to SparkDesk API through websockets and receives response streams asynchronously.
    /// </summary>
    /// <param name="messages">Array of chat messages to send to SparkDesk API.</param>
    /// <param name="parameters">Optional parameters to modify chat request.</param>
    /// <param name="uid">Optional user ID to associate with the chat messages.</param>
    /// <param name="cancellationToken">Optional cancellation token to stop the operation.</param>
    /// <returns>Asynchronous stream of responses from SparkDesk API.</returns>
    public async IAsyncEnumerable<StreamedChatResponse> ChatAsStreamAsync(ChatMessage[] messages, ChatRequestParameters? parameters = null, string? uid = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using ClientWebSocket webSocket = new();

        await webSocket.ConnectAsync(new Uri(GetAuthorizationUrl(_apiKey, _apiSecret, HostUrl)), cancellationToken);

        var messageBuffer = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(new ChatApiRequest
        {
            Header = new ChatRequestHeader { AppId = _appId, Uid = uid },
            Parameter = new Parameters { Chat = parameters ?? new ChatRequestParameters() },
            Payload = new Payload
            {
                Message = new Message { Text = messages }
            },
        }));
        await webSocket.SendAsync(messageBuffer, WebSocketMessageType.Text, true, cancellationToken);

        byte[] buffer = new byte[4096];
        do
        {
            ArraySegment<byte> arraySegment = new(buffer);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(arraySegment, cancellationToken);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                ChatApiResponse resp = await JsonSerializer.DeserializeAsync<ChatApiResponse>(new MemoryStream(buffer, 0, result.Count), cancellationToken: cancellationToken)
                    ?? throw new SparkDeskException($"Can't deserialize response from spark desk API, raw response: {Encoding.UTF8.GetString(buffer, 0, result.Count)}.");
                if (resp.Header.Code != 0)
                {
                    throw new SparkDeskException(resp.Header.Code, resp.Header.Sid, resp.Header.Message);
                }

                yield return new StreamedChatResponse(string.Concat(resp.Payload!.Choices.Text.Select(x => x.Content)), resp.Payload.Usage?.Text);

                if (resp.Header.Status == 2)
                {
                    break;
                }
            }
            else
            {
                throw new SparkDeskException($"Unexpected websocket message type: \"{result.MessageType}\", expect \"text\".");
            }
        } while (!cancellationToken.IsCancellationRequested);

        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client request disconnect.", cancellationToken);
        }
    }

    /// <summary>
    /// Generates authorization URL for SparkDesk API.
    /// </summary>
    /// <param name="apiKey">SparkDesk API key.</param>
    /// <param name="apiSecret">SparkDesk API secret.</param>
    /// <param name="hostUrl">Host URL. Optional, default is value from const field.</param>
    /// <returns>Authorization URL.</returns>
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
}
