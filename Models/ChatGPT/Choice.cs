namespace ssbot.Models.ChatGPT
{
    /// <summary>
    /// 选项
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// 消息列表
        /// </summary>
        public  CompletionMessage? Message { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// 完成原因
        /// </summary>
        public string? Finish_reason { get; set; }
    }
}
