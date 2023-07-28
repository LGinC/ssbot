using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotFramework;

namespace ssbot.Models.Telegram;

public class MyBot : BaseBot
{
    public MyBot(IOptions<BotSettings> options) : this(new TelegramBotClient(new TelegramBotClientOptions(options.Value.ApiToken, options.Value.BaseUrl, false)))
    {
    }
    

    public MyBot(string token, string username = null) : base(token, username)
    {
    }

    public MyBot(TelegramBotClient client) : base(client)
    {
    }
}