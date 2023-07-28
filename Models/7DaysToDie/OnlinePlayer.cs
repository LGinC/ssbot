namespace ssbot.Models._7DaysToDie;

/// <summary>
/// 在线玩家
/// </summary>
public class OnlinePlayer
{
    /// <summary>
    /// steam id
    /// </summary>
    public string Steamid { get; set; }
    /// <summary>
    /// 实体 id
    /// </summary>
    public int Entityid { get; set; }
    /// <summary>
    /// ip
    /// </summary>
    public string Ip { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 是否在线
    /// </summary>
    public bool Online { get; set; }
    /// <summary>
    /// 位置
    /// </summary>
    public Position Position { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public double Level { get; set; }
    /// <summary>
    /// 生命值
    /// </summary>
    public int Health { get; set; }
    /// <summary>
    /// 耐力
    /// </summary>
    public double Stamina { get; set; }
    /// <summary>
    /// 击杀丧尸数
    /// </summary>
    public int Zombiekills { get; set; }
    /// <summary>
    /// 击杀玩家数
    /// </summary>
    public int Playerkills { get; set; }
    /// <summary>
    /// 死亡数
    /// </summary>
    public int Playerdeaths { get; set; }
    /// <summary>
    /// 当前经验
    /// </summary>
    public int Score { get; set; }
    /// <summary>
    /// 游玩总时长
    /// </summary>
    public long Totalplaytime { get; set; }
    /// <summary>
    /// 最近在线时间
    /// </summary>
    public string? Lastonline { get; set; }
    /// <summary>
    /// ping值
    /// </summary>
    public int Ping { get; set; }
}

public class Position
{
    /// <summary>
    /// 
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Y { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Z { get; set; }
}
