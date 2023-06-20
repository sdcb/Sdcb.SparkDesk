using System;

namespace Sdcb.SparkDesk;

[Serializable]
public class SparkDeskException : Exception
{
    public SparkDeskException(string? message) : base(message)
    {
    }

    public SparkDeskException(string sid, string? message) : base(message)
    {
        Sid = sid;
    }

    public SparkDeskException(int errorCode, string sid, string? message) : base(message)
    {
        HResult = errorCode;
        Sid = sid;
    }

    public string? Sid { get; }
}