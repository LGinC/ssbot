
namespace ssbot.Models.ChatGPT;

/// <summary>
/// 对话响应
/// </summary>
public class TalkResponse
{
    /// <summary>
    /// 会话id
    /// </summary>
    public Guid Conversation_id { get; set; }
    /// <summary>
    /// 错误信息
    /// </summary>
    public ErrorMessage Detail { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    public TalkMessage Message { get; set; }
    
}

/// <summary>
/// 聊天消息
/// </summary>
public class TalkMessage
{
    /// <summary>
    /// 消息id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 是否完成
    /// </summary>
    public bool End_turn { get; set; }
    /// <summary>
    /// 内容
    /// </summary>
    public TalkContent Content { get; set; }
}

/// <summary>
/// 聊天返回内容
/// </summary>
public class TalkContent
{
    /// <summary>
    /// 内容类型
    /// <list>text	文本</list>
    /// <list>image	图片</list>
    /// </summary>
    public string Content_type { get; set; }

    /// <summary>
    /// 内容分段
    /// </summary>
    public string[] Parts { get; set; }
}