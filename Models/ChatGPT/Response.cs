namespace ssbot.Models.ChatGPT
{
    /// <summary>
    /// 请求结果
    /// </summary>
    public class Response
    {
        /// <summary>
        /// 结果id
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Object { get; set; }
        /// <summary>
        /// 创建时间对应时间戳
        /// </summary>
        public long Created { get; set; }
        /// <summary>
        /// 训练模型
        /// </summary>
        public string? Model { get; set; }
        /// <summary>
        /// 选项列表
        /// </summary>
        public List<Choice>? Choices { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Usage? Usage { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public ErrorMessage? Error { get; set; }
    }
}
