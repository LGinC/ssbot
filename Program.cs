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
    if (!msg.StartsWith("!!") && !msg.StartsWith("����")) return string.Empty;
    var cmd = msg[2..];
    var isHanzi = PinyinUtil.IsHanzi(cmd[0]);
    if (!isHanzi && cmd == "setu")
    {
        //����ɬͼ
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
            //����ɬͼ
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
        Console.WriteLine($"{setuPath} ��û������ͼƬŶ");
        return string.Empty;
    }
    return files[RandomNumberGenerator.GetInt32(0, files.Length)][p.Length..];
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