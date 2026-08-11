using System.Text;
using System.Text.Json;

using MCmoderSD.BdsmTestApi.Data;
using MCmoderSD.BdsmTestApi.Enums;
using MCmoderSD.BdsmTestApi.Exceptions;
using MCmoderSD.BdsmTestApi.Internal;

namespace MCmoderSD.BdsmTestApi.Core;

public sealed class BdsmTestApi
{
    private const string Homepage = "https://bdsmtest.org/";
    private const string ResultEndpoint = "ajax/getresult";
    private const string MatchEndpoint = "ajax/match";
    private const string FormContentType = "application/x-www-form-urlencoded";
    private const string DefaultAuthSig = "814a69afc15258000678f00526b0c107ac271b5ea997beb4f7c1e81c861c972b";

    private static readonly Uri ResultUri = new(Homepage + ResultEndpoint);
    private static readonly Uri MatchUri = new(Homepage + MatchEndpoint);
    
    private static readonly HttpClient SharedHttpClient = new();

    private readonly HttpClient _httpClient;

    private string AuthSig { get; }

    public BdsmTestApi(string authSig = DefaultAuthSig, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(authSig)) throw new ArgumentException("Auth signature cannot be null or blank.", nameof(authSig));

        AuthSig = authSig;
        _httpClient = httpClient ?? SharedHttpClient;
    }

    public async Task<TestResult> FetchResultAsync(string resultId, Language language = Language.English, CancellationToken cancellationToken = default)
    {
        ValidateId(resultId, nameof(resultId));

        var body = 
            $"uauth[authsig]={Uri.EscapeDataString(AuthSig)}" +
            $"&rauth[rid]={Uri.EscapeDataString(resultId)}" +
            $"&lang={Uri.EscapeDataString(language.GetCode())}";

        var response = await SendAsync(ResultUri, body, $"result '{resultId}'", cancellationToken).ConfigureAwait(false);
        var dto = Deserialize(response, BdsmJsonContext.Default.ResultDto, $"result '{resultId}'");

        // BDSMTest.org answers with 200 and an empty payload for ids it does not know.
        if (dto.Scores is not { Length: > 0 }) throw new BdsmTestApiException($"BDSMTest.org returned no scores for result '{resultId}'. The id is probably unknown.", null, response);

        try
        {
            return new TestResult(resultId, dto);
        }
        catch (ArgumentException exception)
        {
            throw new BdsmTestApiException($"Failed to interpret the response for result '{resultId}'.", null, response, exception);
        }
    }

    // Performs three requests: the match itself plus both underlying results.
    public async Task<MatchResult> FetchMatchAsync(string resultId, string partnerId, Language language = Language.English, CancellationToken cancellationToken = default)
    {
        ValidateId(resultId, nameof(resultId));
        ValidateId(partnerId, nameof(partnerId));

        var body = 
            $"rauth[rid]={Uri.EscapeDataString(resultId)}" + 
            $"&uauth[authsig]={Uri.EscapeDataString(AuthSig)}" +
            $"&partner={Uri.EscapeDataString(partnerId)}";

        var description = $"match between '{resultId}' and '{partnerId}'";

        var response = await SendAsync(MatchUri, body, description, cancellationToken).ConfigureAwait(false);
        var dto = Deserialize(response, BdsmJsonContext.Default.MatchDto, description);

        var result = await FetchResultAsync(resultId, language, cancellationToken).ConfigureAwait(false);
        var partner = await FetchResultAsync(partnerId, language, cancellationToken).ConfigureAwait(false);

        return new MatchResult(dto.Score, result, partner);
    }

    public Task<MatchResult> FetchMatchAsync(TestResult result, TestResult partner, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(partner);

        return FetchMatchAsync(result.Id, partner.Id, result.Language, cancellationToken);
    }

    public Task<MatchResult> FetchMatchAsync(TestResult result, string partnerId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        return FetchMatchAsync(result.Id, partnerId, result.Language, cancellationToken);
    }

    public Task<MatchResult> FetchMatchAsync(string resultId, TestResult partner, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(partner);

        return FetchMatchAsync(resultId, partner.Id, partner.Language, cancellationToken);
    }

    private async Task<string> SendAsync(Uri uri, string body, string description, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;

        try
        {
            using var content = new StringContent(body, Encoding.UTF8, FormContentType);
            response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            throw new BdsmTestApiException($"Failed to reach BDSMTest.org while fetching the {description}.", exception);
        }

        using (response)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            return !response.IsSuccessStatusCode ? throw new BdsmTestApiException($"BDSMTest.org returned status {(int) response.StatusCode} for the {description}.", (int) response.StatusCode, responseBody) : responseBody;
        }
    }

    private static T Deserialize<T>(string body, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo, string description) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize(body, typeInfo)
                   ?? throw new BdsmTestApiException($"BDSMTest.org returned an empty response for the {description}.", null, body);
        }
        catch (JsonException exception)
        {
            throw new BdsmTestApiException($"BDSMTest.org returned a malformed response for the {description}.", null, body, exception);
        }
    }

    // Result ids appear verbatim in the request body, so reject anything that is not a single token.
    private static void ValidateId(string id, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Any(char.IsWhiteSpace)) throw new ArgumentException($"Invalid result id: {id}", parameterName);
    }
}