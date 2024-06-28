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
using System.Collections;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Sdcb.SparkDesk.Tests")]

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents a client for interacting with the SparkDesk API.
/// </summary>
public class SparkDeskClient
{
    private readonly string _appId;
    private readonly string _apiKey;
    private readonly string _apiSecret;

    private readonly static JsonSerializerOptions _defaultJsonEncodingOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

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
    /// Sends messages to the backend and returns the response via callback. 
    /// Concatenate the string in the <paramref name="chatCallback"/> to get the whole response message.
    /// </summary>
    /// <param name="modelVersion">The version of SparkDesk model.</param>
    /// <param name="messages">The messages to send.</param>
    /// <param name="functions">The supported function calls to send.</param>
    /// <param name="chatCallback">The callback to receive the response.</param>
    /// <param name="parameters">Optional parameters for the request.</param>
    /// <param name="uid">Optional uid for the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The usage of the tokens.</returns>
    public async Task<TokensUsage> ChatAsStreamAsync(
        ModelVersion modelVersion,
        ChatMessage[] messages,
        Action<string> chatCallback,
        ChatRequestParameters? parameters = null,
        FunctionDef[]? functions = null,
        string? uid = null,
        CancellationToken cancellationToken = default)
    {
        TokensUsage? usage = null;
        await foreach (StreamedChatResponse msg in ChatAsStreamAsync(modelVersion, messages, parameters, functions, uid, cancellationToken))
        {
            chatCallback(msg.Text);
            usage ??= msg.Usage;
        }

        return usage!;
    }

    /// <summary>
    /// Sends chat messages to SparkDesk API through websockets and receives response streams asynchronously.
    /// </summary>
    /// <param name="modelVersion">The version of SparkDesk model.</param>
    /// <param name="messages">Array of chat messages to send to SparkDesk API.</param>
    /// <param name="functions">Array of supported function calls to sent to SparkDesk API.</param>
    /// <param name="parameters">Optional parameters to modify chat request.</param>
    /// <param name="uid">Optional user ID to associate with the chat messages.</param>
    /// <param name="cancellationToken">Optional cancellation token to stop the operation.</param>
    /// <returns>Asynchronous task that returns a <see cref="ChatResponse"/> object containing the streamed chat response.</returns>
    public async Task<ChatResponse> ChatAsync(
        ModelVersion modelVersion,
        ChatMessage[] messages,
        ChatRequestParameters? parameters = null,
        FunctionDef[]? functions = null,
        string? uid = null,
        CancellationToken cancellationToken = default)
    {
        List<StreamedChatResponse> resps = new();
        await foreach (StreamedChatResponse msg in ChatAsStreamAsync(modelVersion, messages, parameters, functions, uid, cancellationToken))
        {
            resps.Add(msg);
        }

        return new ChatResponse(resps);
    }

    /// <summary>
    /// Sends chat messages to SparkDesk API through websockets and receives response streams asynchronously.
    /// </summary>
    /// <param name="modelVersion">The version of SparkDesk model.</param>
    /// <param name="messages">Array of chat messages to send to SparkDesk API.</param>
    /// <param name="functions">Array of supported function calls to send to SparkDesk API.</param>
    /// <param name="parameters">Optional parameters to modify chat request.</param>
    /// <param name="uid">Optional user ID to associate with the chat messages.</param>
    /// <param name="cancellationToken">Optional cancellation token to stop the operation.</param>
    /// <returns>Asynchronous stream of responses from SparkDesk API.</returns>
    public async IAsyncEnumerable<StreamedChatResponse> ChatAsStreamAsync(
        ModelVersion modelVersion,
        ChatMessage[] messages,
        ChatRequestParameters? parameters = null,
        FunctionDef[]? functions = null,
        string? uid = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using ClientWebSocket webSocket = new();

        await webSocket.ConnectAsync(new Uri(GetAuthorizationUrl(_apiKey, _apiSecret, modelVersion.WebsocketUrl)), cancellationToken);

        ArraySegment<byte> messageBuffer = new(JsonSerializer.SerializeToUtf8Bytes(new ChatApiRequest
        {
            Header = new ChatRequestHeader { AppId = _appId, Uid = uid },
            Parameter = new RequestInternals.ChatRequestParameters { Chat = new ChatRequestParametersInternal(modelVersion, parameters) },
            Payload = new Payload
            {
                Message = new Message { Text = messages },
                Functions = functions is null ? null : FunctionCallDto.FromFunctionDefs(functions)
            },
        }, _defaultJsonEncodingOptions));
        await webSocket.SendAsync(messageBuffer, WebSocketMessageType.Text, true, cancellationToken);

        byte[] buffer = new byte[4096];
        do
        {
            ArraySegment<byte> arraySegment = new(buffer);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(arraySegment, cancellationToken);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                ChatApiResponse resp = null!;
                try
                {
                    resp = await JsonSerializer.DeserializeAsync<ChatApiResponse>(new MemoryStream(buffer, 0, result.Count), cancellationToken: cancellationToken)
                        ?? throw new SparkDeskException($"Can't deserialize response from spark desk API, raw response: {Encoding.UTF8.GetString(buffer, 0, result.Count)}.");
                }
                catch (JsonException ex)
                {
                    throw new SparkDeskException($"Can't deserialize response from spark desk API, reason: {ex.Message}, raw response: {Encoding.UTF8.GetString(buffer, 0, result.Count)}.");
                }

                if (resp.Header.Code != 0)
                {
                    throw new SparkDeskException(resp.Header.Code, resp.Header.Sid, resp.Header.Message);
                }

                ResponseChatMessage choice = resp.Payload!.Choices.Text[0];
                yield return new StreamedChatResponse(
                    choice.Content, 
                    resp.Payload.Usage?.Text, 
                    choice.ContentType, 
                    choice.FunctionCall);

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
    public static string GetAuthorizationUrl(string apiKey, string apiSecret, string hostUrl)
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
