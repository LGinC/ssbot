using hyjiacan.py4n;
using System.Security.Cryptography;
using System.Text.Json;

const string ClientName = "go-cqhttp";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(ClientName, options => options.BaseAddress = new Uri(builder.Configuration["Cqhttp:Host"]));
var secret = builder.Configuration["Cqhttp:secret"];
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/recv", async (HttpRequest request, IHttpClientFactory factory) =>
{
    JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    string body;
    using (StreamReader reader = new(request.Body))
    {
        body = await reader.ReadToEndAsync();
    }
    var baseRequest = JsonSerializer.Deserialize<CqhttpBaseRequest>(body, serializerOptions)!;
    if (baseRequest.Post_type != "message") return string.Empty;
    return baseRequest.Post_type switch
    {
        "message" => JsonSerializer.Deserialize<CqhttpGroupMsgRequest>(body, serializerOptions)!.Message_type switch
        {
            "group" => await GroupMessage(JsonSerializer.Deserialize<CqhttpGroupMsgRequest>(body, serializerOptions)!, factory.CreateClient(ClientName)),
            "private" => string.Empty,//JsonSerializer.Deserialize< CqhttpGroupMsgRequest >(body,serializerOptions),
            _ => string.Empty,
        },
        "request" => string.Empty,
        "notice" => string.Empty,
        "meta_event" => string.Empty,
        _ => string.Empty,
    };
})
.WithName("ReceveCommand");

app.Run();

async Task<string> GroupMessage(CqhttpGroupMsgRequest request, HttpClient client)
{   
    var msg = request.Message;
    if (string.IsNullOrWhiteSpace(msg)) return string.Empty;
    if (!msg.StartsWith("!!") && !msg.StartsWith("！！")) return string.Empty;
    var cmd = msg[2..];
    var isHanzi = PinyinUtil.IsHanzi(cmd[0]);
    if (!isHanzi && cmd == "setu")
    {
        //发送涩图
        await SendMsg(client, $"/send_group_msg?access_token={secret}", new
        {
            group_id = request.Group_id,
            message = $"[CQ:image,file={GetRandomPic()}]",
        });
        return cmd;
    }
    else if (!isHanzi) return string.Empty;
    switch (Pinyin4Net.GetPinyin(cmd[..2], PinyinFormat.LOWERCASE))
    {
        case "se4 tu2":
            //发送涩图
            var img = GetRandomPic();
            if (img == default) return cmd;
            await SendMsg(client, $"/send_group_msg?access_token={secret}", new
            {
                group_id = request.Group_id,
                message = $"[CQ:image,file={img}]",
            });
            break;
        default:
            break;
    }
    return cmd;
}

const string imagePath = "images";
string GetRandomPic()
{
    var p = Path.GetFullPath(imagePath);
    var setuPath = Path.Combine(p, "setu");
    if (!Directory.Exists(setuPath))
    {
        Directory.CreateDirectory(setuPath);
    }
    var files = Directory.GetFiles(setuPath);
    if(files.Length == 0)
    {
        Console.WriteLine($"{setuPath} 下没有任务图片哦");
        return string.Empty;
    }
    return files[RandomNumberGenerator.GetInt32(0, files.Length)][p.Length..];
}

async Task<bool> SendMsg(HttpClient client, string url, object body)
{
    var r = await client.PostAsJsonAsync(url, body);
    if (!r.IsSuccessStatusCode)
    {
        Console.WriteLine($"返回结果: {await r.Content.ReadAsStringAsync()}");
        return false;
    }
    return true;
}


/// <summary>
/// go-cqhttp请求
/// </summary>
public class CqhttpBaseRequest
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
    /// <list>message	消息, 例如, 群聊消息</list>
    /// <list>request	请求, 例如, 好友申请</list>
    /// <list>notice	通知, 例如, 群成员增加</list>
    /// <list>meta_event	元事件, 例如, go-cqhttp 心跳包</list>
    /// </summary>
    public string? Post_type { get; set; }
}

/// <summary>
/// 聊天消息请求
/// </summary>
public class CqhttpMsgRequest : CqhttpBaseRequest
{
    /// <summary>
    /// 消息类型
    /// <list>private 私聊</list>
    /// <list>group 群消息</list>
    /// </summary>
    public string? Message_type { get; set; }
    /// <summary>
    /// 消息子类型
    /// <list>friend 好友</list>
    /// <list>group 群临时会话</list>
    /// <list>group_self 群中自身发送</list>
    /// <list>normal 正常群聊消息</list>
    /// <list>anonymous 匿名消息</list>
    /// <list>notice 系统提示  如「管理员已禁止群内匿名聊天」</list>
    /// </summary>
    public string? Sub_type { get; set; }
    /// <summary>
    /// 消息 ID
    /// </summary>
    public int Message_id { get; set; }
    /// <summary>
    /// 发送者 QQ 号
    /// </summary>
    public long User_id { get; set; }
    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// 原始消息内容
    /// </summary>
    public string? Raw_message { get; set; }
    /// <summary>
    /// 字体
    /// </summary>
    public int Font { get; set; }
}

/// <summary>
/// 私聊消息
/// </summary>
public class CqhttpPrivateMsgRequest : CqhttpMsgRequest
{
    /// <summary>
    /// 发送人信息
    /// </summary>
    public CqhttpPrivateMsgSender? Sender { get; set; }

    /// <summary>
    /// 临时会话来源
    /// </summary>
    public int? Temp_source { get; set; }

}

/// <summary>
/// 群消息请求
/// </summary>
public class CqhttpGroupMsgRequest : CqhttpMsgRequest
{
    /// <summary>
    /// 群号
    /// </summary>
    public int Group_id { get; set; }

    /// <summary>
    /// 发送人信息
    /// </summary>
    public CqhttpGroupMsgSender? Sender { get; set; }
}

/// <summary>
/// 私聊消息发送者信息
/// </summary>
public class CqhttpPrivateMsgSender
{
    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; }
    /// <summary>
    /// 性别
    /// </summary>
    public string? Sex { get; set; }
    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }
    /// <summary>
    /// 发送者id
    /// </summary>
    public long User_id { get; set; }
    /// <summary>
    /// 发送人信息
    /// </summary>
    public CqhttpPrivateMsgSender? Sender { get; set; }
}

/// <summary>
/// 群聊消息发送者信息
/// </summary>
public class CqhttpGroupMsgSender : CqhttpPrivateMsgSender
{
    /// <summary>
    /// 群名片／备注
    /// </summary>
    public string? Card { get; set; }
    /// <summary>
    /// 地区
    /// </summary>
    public string? Area { get; set; }
    /// <summary>
    /// 成员等级
    /// </summary>
    public string? Level { get; set; }
    /// <summary>
    /// 角色
    /// <list>owner 群主</list>
    /// <list>admin 管理员</list>
    /// <list>member 成员</list>
    /// </summary>
    public string? Role { get; set; }
    /// <summary>
    /// 专属头衔
    /// </summary>
    public string? Title { get; set; }
}