namespace ssbot.Models.ChatGPT
{
    /// <summary>
    /// 请求
    /// </summary>
    public class Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string Model { get; set; } = "gpt-3.5-turbo";

        /// <summary>
        /// 消息列表
        /// </summary>
        public List<CompletionMessage>? Messages { get; set; }

        /// <summary>
        /// 模型上下文长度
        /// </summary>
        public int Max_tokens { get; set; } = 2500;


        /// <summary>
        /// 采样温度 0-2 默认为1
        /// </summary>
        public double? Temperature { get; set; } = 0.9;

        /// <summary>
        /// 用于替换采样温度，叫做原子采样 0.1 则只取前10%的token
        /// </summary>
        public double? Top_p { get; set; } = 1;

        /// <summary>
        /// 每个指令生成的token数 默认1
        /// </summary>
        public int? N { get; set; }

        /// <summary>
        /// 是否流式分布处理
        /// </summary>
        public bool Stream { get; set; }

        /// <summary>
        /// -2.0 到 2.0 之间 如果是正数会分析新token，根据它到目前为止是否出现在文本中，在后续的新主题时增加模型的可能性
        /// </summary>
        public double? Presence_penalty { get; set; }

        /// <summary>
        /// -2.0 到 2.0 之间 如果为正数则分析新token，根据它在文本中出现的频率，以降低模型出现重复语句的可能
        /// </summary>
        public bool? Frequency_penalty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[]? Stop { get; set; }

        /// <summary>
        /// 修改指定token在对话中出现的可能性 -100到100
        /// </summary>
        public Dictionary<string,int>? Logit_bias { get; set; }

        /// <summary>
        /// 终端用户的唯一标识
        /// </summary>
        public string? User { get; set; }
    }
}
