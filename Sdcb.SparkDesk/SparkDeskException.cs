using System;

namespace Sdcb.SparkDesk;

/// <summary>
/// Exception class used for SparkDesk API.
/// </summary>
[Serializable]
public class SparkDeskException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for exception.</param>
    public SparkDeskException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskException"/> class with the specified sid and error message.
    /// </summary>
    /// <param name="sid">The SID of the error.</param>
    /// <param name="message">The error message that explains the reason for exception.</param>
    public SparkDeskException(string sid, string? message) : base(message)
    {
        Sid = sid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskException"/> class with the specified error code, sid and error message.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="sid">The SID of the error.</param>
    /// <param name="message">The error message that explains the reason for exception.</param>
    public SparkDeskException(int errorCode, string sid, string? message) : base(message)
    {
        HResult = errorCode;
        Sid = sid;
    }

    /// <summary>
    /// Gets the SID of the error.
    /// </summary>
    public string? Sid { get; }
}
