namespace ssbot.Models.ChatGPT
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// 错误类型
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object? Param { get; set; }
    }
}
