using System.Collections.Concurrent;
using OpenAI_API.Chat;

namespace ChatGPT.Bot.Data
{
    internal class ConversationStorage : IConversationStorage
    {
        // TODO: use some persist and reliable storage...
        private static readonly ConcurrentDictionary<long, List<ChatMessage>> _inMemStorage = new();

        public void SetMessages(long key, IEnumerable<ChatMessage> messages)
        {
            _inMemStorage[key] = messages.ToList();
        }

        public List<ChatMessage> GetMessages(long key)
        {
            if (_inMemStorage.ContainsKey(key))
            {
                return _inMemStorage[key];
            }

            return new List<ChatMessage>();
        }
    }
}
