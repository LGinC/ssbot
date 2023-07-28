using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework;

namespace ssbot.Models.Telegram;

public class PicCommandExample : CommandBase<ExampleContext>
{
    private readonly IHttpClientFactory factory;
    const string AiPicClientName = "ai-pic";
    public PicCommandExample(IHttpClientFactory factory)
    {
        this.factory = factory;
    }

    public override async Task HandleAsync(ExampleContext context, UpdateDelegate<ExampleContext> next, string[] args, CancellationToken cancellationToken)
    {
        if (args.Length == 0)
        {
            return;
        }
        Console.WriteLine(args[0]);
        var httpClient = factory.CreateClient(AiPicClientName);
        var r = await httpClient.PostAsJsonAsync("/sdapi/v1/txt2img",
            new
            {
                prompt = string.Join(' ', args[0]), steps = 20, width = 512, height = 512, cfg_scale = 8, batch_size = 3,
                sampler_index = "DPM++ SDE"
            }, cancellationToken);
        var jd = await r.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);
        foreach (var image in jd!.RootElement.GetProperty("images").EnumerateArray())
        {
            await context.Client.SendPhotoAsync(context.ChatId,
                new InputMedia(new MemoryStream(Convert.FromBase64String(image.GetString())), $"{Guid.NewGuid()}.jpg"), cancellationToken: cancellationToken);
        }
    }
}

public class ExampleContext : UpdateContext
{
    
}