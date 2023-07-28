using System.Text.Json.Serialization;

namespace ssbot.Models.Cqhttp
{
    /// <summary>
    /// go-cqhttp请求
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(RequestJsonConverter))]
    public class RequestBase
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long Time { get; set; }
    
        /// <summary>
        /// 收到事件的机器人 QQ 号
        /// </summary>
        public long Self_id { get; set; }
    
        /// <summary>
        /// 上报类型 
        /// </summary>
        public PostType Post_type { get; set; }
    }

    /// <summary>
    /// 请求类型
    /// </summary>
    public enum PostType
    {
        /// <summary>
        /// 群聊消息或私聊消息
        /// </summary>
        message = 1,
        
        /// <summary>
        /// 消息发送，例如，bot发送在群里的消息
        /// </summary>
        message_sent,
        
        /// <summary>
        /// 请求, 例如, 好友申请
        /// </summary>
        request,
        /// <summary>
        /// 通知, 例如, 群成员增加
        /// </summary>
        notice,
        /// <summary>
        /// 元事件, 例如, go-cqhttp 心跳包
        /// </summary>
        meta_event,
    }
    
    
}

