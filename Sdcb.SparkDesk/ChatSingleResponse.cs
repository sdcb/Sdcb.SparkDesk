namespace Sdcb.SparkDesk;

public record struct ChatSingleResponse(string Text, TextUsage? Usage)
{
    public static implicit operator string(ChatSingleResponse r) => r.Text;
}
