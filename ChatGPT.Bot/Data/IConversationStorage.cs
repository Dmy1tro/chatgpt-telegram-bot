using OpenAI_API.Chat;

namespace ChatGPT.Bot.Data
{
    public interface IConversationStorage
    {
        void SetMessages(long key, IEnumerable<ChatMessage> messages);

        List<ChatMessage> GetMessages(long key);
    }
}
