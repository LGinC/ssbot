namespace ssbot.Models.ChatGPT;

public class ConversationCacheItem
{
    public Guid? ConversationId { get; set; }

    public Guid ParentId { get; set; }
}