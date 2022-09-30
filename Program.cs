using hyjiacan.py4n;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text.Json;

const string ClientName = "go-cqhttp";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
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

app.MapPost("/recv", async (HttpRequest request, IHttpClientFactory factory, IMemoryCache cache) =>
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
            "group" => await GroupMessage(JsonSerializer.Deserialize<CqhttpGroupMsgRequest>(body, serializerOptions)!, factory.CreateClient(ClientName), cache),
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

async Task<string> GroupMessage(CqhttpGroupMsgRequest request, HttpClient client, IMemoryCache cache)
{   
    var msg = request.Message;
    if (string.IsNullOrWhiteSpace(msg)) return string.Empty;
    if (!msg.StartsWith("!!") && !msg.StartsWith("����")) return string.Empty;
    var cmd = msg[2..];
    var cmdType = CommandType.None;
    var isHanzi = PinyinUtil.IsHanzi(cmd[0]);
    if (!isHanzi && cmd == "setu")
    {
        cmdType = CommandType.SexPicture;
    }
    var pinyinOfCmd = Pinyin4Net.GetPinyin(cmd[..], PinyinFormat.LOWERCASE);
    if(pinyinOfCmd.Contains("se4 tu2") || pinyinOfCmd.Contains("bu4 gou4 se4"))//ɬͼ �� ����ɬ
    {
        cmdType = CommandType.SexPicture;
    }

    switch (cmdType)
    {
        case CommandType.SexPicture:
            //����ɬͼ
            var img = GetRandomPic(cache);
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
const string setuCacheKey = "setu";
string GetRandomPic(IMemoryCache cache)
{
    var p = Path.GetFullPath(imagePath);
    var files = cache.GetOrCreate(setuCacheKey, s =>
    {
        var setuPath = Path.Combine(p, "setu");
        if (!Directory.Exists(setuPath))
        {
            Directory.CreateDirectory(setuPath);
        }
        return Directory.GetFiles(setuPath).ToList();
    });
    
    if(files.Count == 0)
    {
        Console.WriteLine($"{Path.Combine(Path.GetFullPath(imagePath), "setu")} ��û���κ�ͼƬŶ");
        cache.Remove(setuCacheKey);
        return string.Empty;
    }
    var index = RandomNumberGenerator.GetInt32(0, files.Count);
    var result = files[index][p.Length..];
    files.RemoveAt(index);
    if(files.Count > 0)
    {
        cache.Set(setuCacheKey, files);
    }
    else
    {
        cache.Remove(setuCacheKey);
    }
    return result;
}

async Task<bool> SendMsg(HttpClient client, string url, object body)
{
    var r = await client.PostAsJsonAsync(url, body);
    if (!r.IsSuccessStatusCode)
    {
        Console.WriteLine($"���ؽ��: {await r.Content.ReadAsStringAsync()}");
        return false;
    }
    return true;
}

/// <summary>
/// ��������
/// </summary>
public enum CommandType
{
    /// <summary>
    /// ��
    /// </summary>
    None,
    /// <summary>
    /// ɬͼ
    /// </summary>
    SexPicture,
}

/// <summary>
/// go-cqhttp����
/// </summary>
public class CqhttpBaseRequest
{
    /// <summary>
    /// ʱ���
    /// </summary>
    public long Time { get; set; }
    /// <summary>
    /// �յ��¼��Ļ����� QQ ��
    /// </summary>
    public long Self_id { get; set; }
    /// <summary>
    /// �ϱ����� 
    /// <list>message	��Ϣ, ����, Ⱥ����Ϣ</list>
    /// <list>request	����, ����, ��������</list>
    /// <list>notice	֪ͨ, ����, Ⱥ��Ա����</list>
    /// <list>meta_event	Ԫ�¼�, ����, go-cqhttp ������</list>
    /// </summary>
    public string? Post_type { get; set; }
}

/// <summary>
/// ������Ϣ����
/// </summary>
public class CqhttpMsgRequest : CqhttpBaseRequest
{
    /// <summary>
    /// ��Ϣ����
    /// <list>private ˽��</list>
    /// <list>group Ⱥ��Ϣ</list>
    /// </summary>
    public string? Message_type { get; set; }
    /// <summary>
    /// ��Ϣ������
    /// <list>friend ����</list>
    /// <list>group Ⱥ��ʱ�Ự</list>
    /// <list>group_self Ⱥ��������</list>
    /// <list>normal ����Ⱥ����Ϣ</list>
    /// <list>anonymous ������Ϣ</list>
    /// <list>notice ϵͳ��ʾ  �硸����Ա�ѽ�ֹȺ���������졹</list>
    /// </summary>
    public string? Sub_type { get; set; }
    /// <summary>
    /// ��Ϣ ID
    /// </summary>
    public int Message_id { get; set; }
    /// <summary>
    /// ������ QQ ��
    /// </summary>
    public long User_id { get; set; }
    /// <summary>
    /// ��Ϣ����
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// ԭʼ��Ϣ����
    /// </summary>
    public string? Raw_message { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public int Font { get; set; }
}

/// <summary>
/// ˽����Ϣ
/// </summary>
public class CqhttpPrivateMsgRequest : CqhttpMsgRequest
{
    /// <summary>
    /// ��������Ϣ
    /// </summary>
    public CqhttpPrivateMsgSender? Sender { get; set; }

    /// <summary>
    /// ��ʱ�Ự��Դ
    /// </summary>
    public int? Temp_source { get; set; }

}

/// <summary>
/// Ⱥ��Ϣ����
/// </summary>
public class CqhttpGroupMsgRequest : CqhttpMsgRequest
{
    /// <summary>
    /// Ⱥ��
    /// </summary>
    public int Group_id { get; set; }

    /// <summary>
    /// ��������Ϣ
    /// </summary>
    public CqhttpGroupMsgSender? Sender { get; set; }
}

/// <summary>
/// ˽����Ϣ��������Ϣ
/// </summary>
public class CqhttpPrivateMsgSender
{
    /// <summary>
    /// ����
    /// </summary>
    public int Age { get; set; }
    /// <summary>
    /// �Ա�
    /// </summary>
    public string? Sex { get; set; }
    /// <summary>
    /// �ǳ�
    /// </summary>
    public string? Nickname { get; set; }
    /// <summary>
    /// ������id
    /// </summary>
    public long User_id { get; set; }
    /// <summary>
    /// ��������Ϣ
    /// </summary>
    public CqhttpPrivateMsgSender? Sender { get; set; }
}

/// <summary>
/// Ⱥ����Ϣ��������Ϣ
/// </summary>
public class CqhttpGroupMsgSender : CqhttpPrivateMsgSender
{
    /// <summary>
    /// Ⱥ��Ƭ����ע
    /// </summary>
    public string? Card { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public string? Area { get; set; }
    /// <summary>
    /// ��Ա�ȼ�
    /// </summary>
    public string? Level { get; set; }
    /// <summary>
    /// ��ɫ
    /// <list>owner Ⱥ��</list>
    /// <list>admin ����Ա</list>
    /// <list>member ��Ա</list>
    /// </summary>
    public string? Role { get; set; }
    /// <summary>
    /// ר��ͷ��
    /// </summary>
    public string? Title { get; set; }
}