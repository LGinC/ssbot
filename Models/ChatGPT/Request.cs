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
        public string Model { get; set; } = "text-davinci-003";

        /// <summary>
        /// 指令 即输入的信息
        /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string Prompt { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

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
        /// 
        /// </summary>
        public int? Logprobs { get; set; }

        /// <summary>
        /// 是否在结果中增加输入的指令
        /// </summary>
        public bool? Echo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[]? Stop { get; set; }
    }
}
