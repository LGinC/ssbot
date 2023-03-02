namespace ssbot.Models.ChatGPT;

/// <summary>
/// 
/// </summary>
public class CompletionMessage
{
    /// <summary>
    /// 角色 有 system user assistant  默认为user
    /// </summary>
    public string Role { get; set; } = "user";
    /// <summary>
    /// 内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public CompletionMessage()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <param name="content"></param>
    public CompletionMessage(string role, string? content)
    {
        Role = role;
        Content = content;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    public CompletionMessage(string? content) : this("user", content){}
}