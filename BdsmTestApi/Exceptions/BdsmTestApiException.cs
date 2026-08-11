namespace MCmoderSD.BdsmTestApi.Exceptions;

public sealed class BdsmTestApiException : Exception
{
    public int? StatusCode { get; }
    public string? ResponseBody { get; }

    public BdsmTestApiException(string message) : base(message) { }

    public BdsmTestApiException(string message, Exception? innerException) : base(message, innerException) { }

    public BdsmTestApiException(string message, int? statusCode, string? responseBody, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}