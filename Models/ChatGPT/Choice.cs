namespace ssbot.Models.ChatGPT
{
    /// <summary>
    /// 选项
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string? Text { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Logprobs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Finish_reason { get; set; }
    }
}
