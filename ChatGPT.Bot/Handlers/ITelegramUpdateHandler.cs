using Telegram.Bot.Types;

namespace ChatGPT.Bot.Handlers
{
    public interface ITelegramUpdateHandler
    {
        bool SupportUpdate(Update update);

        Task Handle(Update update, CancellationToken cancellation = default);
    }
}
