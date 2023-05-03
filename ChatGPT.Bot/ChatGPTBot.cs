using ChatGPT.Bot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatGPT.Bot
{
    internal class ChatGPTBot
    {
        private readonly ITelegramBotClient _bot;
        private readonly IEnumerable<ITelegramUpdateHandler> _telegramUpdateHandlers;

        public ChatGPTBot(ITelegramBotClient client, IEnumerable<ITelegramUpdateHandler> telegramUpdateHandlers)
        {
            _bot = client;
            _telegramUpdateHandlers = telegramUpdateHandlers;
        }

        public async Task StartReceiving(CancellationToken cancellationToken)
        {
            Console.Title = "ChatGPT bot";

            var tcs = new TaskCompletionSource();

            _bot.StartReceiving(
                updateHandler: HandleUpdate,
                pollingErrorHandler: HandlePollingError,
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates = new UpdateType[] { UpdateType.Message } // receive only text messages.
                },
                cancellationToken: cancellationToken);

            Console.WriteLine("Start Receiving...");

            // Receiving messages until cancellation is requested.
            cancellationToken.Register(() =>
            {
                tcs.SetResult();
            });

            await tcs.Task;
        }

        private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Just for testing purposes.
            var message = update.Message!.Text?.Trim();
            Console.WriteLine($"Message: {message}");

            foreach (var handler in _telegramUpdateHandlers)
            {
                if (handler.SupportUpdate(update))
                {
                    try
                    {
                        await handler.Handle(update, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        await HandleError(update, ex);
                    }
                }
            }
        }

        private Task HandlePollingError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            PrintErrorToConsole(exception);
            return Task.CompletedTask;
        }

        private async Task HandleError(Update update, Exception exception)
        {
            PrintErrorToConsole(exception);

            var chatId = update.Message?.Chat?.Id;

            if (chatId.HasValue)
            {
                await _bot.SendTextMessageAsync(
                    chatId,
                    "Unknown error has occurred while processing your request. " +
                    "Please try submitting your message again later.");
            }
        }

        private void PrintErrorToConsole(Exception exception)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine(exception.ToString());
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
            Console.WriteLine("--------------------------");
        }
    }
}
