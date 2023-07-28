using CrawlerGame.Logic.Services.Interfaces;
using OpenAI_API;
using OpenAI_API.Chat;

namespace CrawlerGame.Logic.Services
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly Conversation CommandMapperConversation;
        private readonly Conversation RoomGeneratorConversation;

        public ChatGPTService()
        {
            OpenAIAPI api = new OpenAIAPI(new APIAuthentication("YOUR_API_KEY", "org-yourOrgHere"));

            CommandMapperConversation = api.Chat.CreateConversation();
            RoomGeneratorConversation = api.Chat.CreateConversation();

            InitConversations();
        }

        public async Task<string> GetCommandFromPlayerInput(string userinput)
        {
            CommandMapperConversation.AppendUserInput(userinput);

            return await CommandMapperConversation.GetResponseFromChatbotAsync();
        }

        private void InitConversations()
        {
            InitCommandMapperConversation();
            InitRoomGeneratorConversation();
        }

        private void InitCommandMapperConversation()
        {
            var commands = new List<string>() { "Walk", "Turn left", "Turn right" };

            // Instructions
            CommandMapperConversation.AppendSystemMessage("You will act as a phrase-to-command mapper in a text based dungeaon crawler game");
            CommandMapperConversation.AppendSystemMessage($"When you receive a message, you must respond with one of the commands in the following commaseparated list:\n{commands}");
            CommandMapperConversation.AppendSystemMessage("You should return the command that resembles the phrase the most, but ONLY if it is a good match.");

            // Examples
            foreach (var command in commands)
            {
                CommandMapperConversation.AppendUserInput(command.Replace(" ", ""));
                CommandMapperConversation.AppendExampleChatbotOutput(command);

                CommandMapperConversation.AppendUserInput(command + command.Last());
                CommandMapperConversation.AppendExampleChatbotOutput(command);

                CommandMapperConversation.AppendUserInput("dadawd12ead");

                var exampleCommands = commands.Aggregate((current, next) => $"- {current}\n- {next}") ?? string.Empty;
                CommandMapperConversation.AppendExampleChatbotOutput($"Sorry I do not understand. Try another command such as:\n{exampleCommands}");
            }
        }

        private void InitRoomGeneratorConversation()
        {
            RoomGeneratorConversation.AppendSystemMessage("You will be used to generate short responses to the user input from a text based dungeon crawler game. The responses can never be in the form of a question");
        }
    }
}