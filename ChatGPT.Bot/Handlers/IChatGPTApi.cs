using OpenAI_API.Chat;

namespace ChatGPT.Bot.Handlers
{
    public interface IChatGPTApi
    {
        // For some reason IOpenAIAPI interface from OpenAI_API package doesn't have 'Chat' property.
        // So put it in our custom interface.
        ChatEndpoint Chat { get; }
    }
}
