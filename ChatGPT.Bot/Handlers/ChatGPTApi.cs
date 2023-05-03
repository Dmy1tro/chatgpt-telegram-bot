using OpenAI_API;
using OpenAI_API.Chat;

namespace ChatGPT.Bot.Handlers
{
    public class ChatGPTApi : IChatGPTApi
    {
        private readonly OpenAIAPI _api;

        public ChatGPTApi(OpenAIAPI api)
        {
            _api = api;
        }

        public ChatEndpoint Chat => _api.Chat;
    }
}
