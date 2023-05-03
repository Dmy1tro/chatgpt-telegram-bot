using ChatGPT.Bot;
using ChatGPT.Bot.Data;
using ChatGPT.Bot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;
using Telegram.Bot;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        using var cts = new CancellationTokenSource();
        var provider = BuildProvider();
        var chatGPTBot = provider.GetRequiredService<ChatGPTBot>();

        chatGPTBot.StartReceiving(cts.Token).GetAwaiter().GetResult();
    }

    private static IServiceProvider BuildProvider()
    {
        var sc = new ServiceCollection();

        sc.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(Configuration.TelegramBotToken));
        sc.AddSingleton<OpenAIAPI>(sp => new OpenAIAPI(Configuration.ChatGPTApiKey));
        sc.AddSingleton<IChatGPTApi, ChatGPTApi>();
        sc.AddSingleton<IConversationStorage, ConversationStorage>();
        sc.AddSingleton<ChatGPTBot>();

        // Register telegram update handlers
        sc.AddSingleton<ITelegramUpdateHandler, MessageToChatGPTHandler>();

        return sc.BuildServiceProvider();
    }
}