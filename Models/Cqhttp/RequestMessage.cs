namespace ssbot.Models.Cqhttp
{
    /// <summary>
    /// 聊天消息请求
    /// </summary>
    public class RequestMessage : RequestBase
    {
        /// <summary>
        /// 消息类型
        /// <list>private 私聊</list>
        /// <list>group 群消息</list>
        /// </summary>
        public MessageType Message_type { get; set; }
    
        /// <summary>
        /// 消息子类型
        /// <list>friend 好友</list>
        /// <list>group 群临时会话</list>
        /// <list>group_self 群中自身发送</list>
        /// <list>normal 正常群聊消息</list>
        /// <list>anonymous 匿名消息</list>
        /// <list>notice 系统提示  如「管理员已禁止群内匿名聊天」</list>
        /// </summary>
        public string? Sub_type { get; set; }
    
        /// <summary>
        /// 消息 ID
        /// </summary>
        public long Message_id { get; set; }
    
        /// <summary>
        /// 发送者 QQ 号
        /// </summary>
        public long User_id { get; set; }
    
        /// <summary>
        /// 消息内容
        /// </summary>
        public string? Message { get; set; }
    
        /// <summary>
        /// 原始消息内容
        /// </summary>
        public string? Raw_message { get; set; }
    
        /// <summary>
        /// 字体
        /// </summary>
        public int Font { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 私聊消息
        /// </summary>
        Private = 1,
        /// <summary>
        /// 群聊消息
        /// </summary>
        Group,
    }
    
    
/// <summary>
/// 私聊消息
/// </summary>
public class PrivateRequestMessage : RequestMessage
{
    /// <summary>
    /// 发送人信息
    /// </summary>
    public PrivateMsgSender? Sender { get; set; }

    /// <summary>
    /// 临时会话来源
    /// </summary>
    public int? Temp_source { get; set; }
}

/// <summary>
/// 群消息请求
/// </summary>
public class GroupRequestMessage : RequestMessage
{
    /// <summary>
    /// 群号
    /// </summary>
    public int Group_id { get; set; }

    /// <summary>
    /// 发送人信息
    /// </summary>
    public GroupMessageSender? Sender { get; set; }
}

/// <summary>
/// 私聊消息发送者信息
/// </summary>
public class PrivateMsgSender
{
    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string? Sex { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 发送者id
    /// </summary>
    public long User_id { get; set; }
}

/// <summary>
/// 群聊消息发送者信息
/// </summary>
public class GroupMessageSender : PrivateMsgSender
{
    /// <summary>
    /// 群名片／备注
    /// </summary>
    public string? Card { get; set; }

    /// <summary>
    /// 地区
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// 成员等级
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// 角色
    /// <list>owner 群主</list>
    /// <list>admin 管理员</list>
    /// <list>member 成员</list>
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// 专属头衔
    /// </summary>
    public string? Title { get; set; }
}
}