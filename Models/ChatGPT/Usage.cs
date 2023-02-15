namespace ssbot.Models.ChatGPT
{
    /// <summary>
    /// 
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// 
        /// </summary>
        public int Prompt_tokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Completion_tokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Total_tokens { get; set; }
    }
}