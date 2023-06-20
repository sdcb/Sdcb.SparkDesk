using System;

namespace Sdcb.SparkDesk;

public class SparkDeskException : Exception
{
    public SparkDeskException(string? message) : base(message)
    {
    }
}
