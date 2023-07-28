namespace ssbot.Models.ChatGPT;

/// <summary>
/// 聊天请求
/// </summary>
public class TalkRequest
{
    /// <summary>
    /// 提问的内容
    /// </summary>
    public string Prompt { get; set; }

    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public Guid Message_id { get; set; }

    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public Guid Parent_message_id { get; set; }

    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public bool Stream { get; set; }

    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public string Model { get; set; } = "text-davinci-002-render-sha";
    /// <summary>
    /// 会话id
    /// </summary>
    public Guid? Conversation_id { get; set; }
}

/// <summary>
/// 继续对话请求，针对未生成完的对话
/// </summary>
public class GoOnRequest
{
    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public Guid Parent_message_id { get; set; }
    /// <summary>
    /// 会话id
    /// </summary>
    public Guid Conversation_id { get; set; }
    
    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public string Model { get; set; } = "text-davinci-002-render-sha";
    
    /// <summary>
    /// 对话使用的模型
    /// </summary>
    public bool Stream { get; set; }
}