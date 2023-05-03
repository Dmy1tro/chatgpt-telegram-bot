using ChatGPT.Bot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatGPT.Bot.Handlers
{
    public class MessageToChatGPTHandler : ITelegramUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IChatGPTApi _chatGPTApi;
        private readonly IConversationStorage _conversationStorage;

        public MessageToChatGPTHandler(
            ITelegramBotClient botClient,
            IChatGPTApi chatGPTApi,
            IConversationStorage conversationStorage)
        {
            _botClient = botClient;
            _chatGPTApi = chatGPTApi;
            _conversationStorage = conversationStorage;
        }

        public bool SupportUpdate(Update update)
        {
            return update.Type == UpdateType.Message &&
                   !string.IsNullOrEmpty(update.Message?.Text?.Trim());
        }

        public async Task Handle(Update update, CancellationToken cancellation = default)
        {
            var chatId = update.Message!.Chat.Id;
            var userMessage = update.Message!.Text?.Trim();
            var conversation = _chatGPTApi.Chat.CreateConversation();
            var messages = _conversationStorage.GetMessages(chatId);

            // Append previous messages to conversation in order to put ChatGPT into context.
            foreach (var message in messages)
            {
                conversation.AppendMessage(message);
            }

            // Append new message from user
            conversation.AppendUserInput(userMessage);

            try
            {
                // Getting response might take a long time so give it a 1 min timeout.
                var response = await ExecuteWithTimeout(
                    () => conversation.GetResponseFromChatbot(),
                    TimeSpan.FromMinutes(1));

                _conversationStorage.SetMessages(chatId, conversation.Messages);

                await _botClient.SendTextMessageAsync(chatId, response, cancellationToken: cancellation);
            }
            catch (TimeoutException)
            {
                var errorMessage = "Sorry, we encountered a timeout exception while processing your request. " +
                                   "This means that the system didn't receive a response within the expected time frame. " +
                                   "Please don't worry, you can try submitting your request again. " +
                                   "Thank you for your patience and understanding.";

                await _botClient.SendTextMessageAsync(chatId, errorMessage, cancellationToken: cancellation);
            }
        }

        private async Task<T> ExecuteWithTimeout<T>(Func<Task<T>> taskToExecute, TimeSpan timeout)
        {
            var task = taskToExecute();
            var timeoutTask = Task.Delay(timeout);
            var completedTask = await Task.WhenAny(task, timeoutTask);

            // If timeout task is completed first then throw exception.
            if (timeoutTask == completedTask)
            {
                throw new TimeoutException();
            }

            return task.Result;
        }
    }
}
