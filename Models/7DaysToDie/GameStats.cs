namespace ssbot.Models._7DaysToDie;

/// <summary>
/// 时间
/// </summary>
public class Gametime
{
    /// <summary>
    /// 天
    /// </summary>
    public int Days { get; set; }
    /// <summary>
    /// 时
    /// </summary>
    public int Hours { get; set; }
    /// <summary>
    /// 分
    /// </summary>
    public int Minutes { get; set; }
}

/// <summary>
/// 游戏状态
/// </summary>
public class GameStats
{
    /// <summary>
    /// 时间
    /// </summary>
    public Gametime Gametime { get; set; }
    /// <summary>
    /// 在线玩家数
    /// </summary>
    public int Players { get; set; }
    /// <summary>
    /// 敌对生物数
    /// </summary>
    public int Hostiles { get; set; }
    /// <summary>
    /// 动物数
    /// </summary>
    public int Animals { get; set; }
}