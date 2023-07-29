using CrawlerGame.Library.Models.ChatGPT;
using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace CrawlerGame.Logic.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly Conversation CommandMapperConversation;
        private readonly Conversation RoomGeneratorConversation;

        public OpenAIService(IOptions<OpenAIOptions> options)
        {
            var api = new OpenAIAPI(new APIAuthentication(options.Value.ApiKey));

            var model = !string.IsNullOrEmpty(options.Value.Model) ? new Model(options.Value.Model) : Model.DavinciText;

            CommandMapperConversation = api.Chat.CreateConversation(new ChatRequest() { Model = model });
            RoomGeneratorConversation = api.Chat.CreateConversation(new ChatRequest() { Model = model });

            InitConversations();
        }

        public async Task<CommandMapperResponse?> GetCommandFromPlayerInput(string userinput, IEnumerable<string> availableCommands)
        {
            CommandMapperConversation.AppendUserInput($"{string.Join(',', availableCommands)}\n\n{userinput}");

            var response = await CommandMapperConversation.GetResponseFromChatbotAsync();

            return JsonConvert.DeserializeObject<CommandMapperResponse>(response);
        }

        private void InitConversations()
        {
            InitCommandMapperConversation();
            InitFoodItemGeneratorConversation();
        }

        private void InitCommandMapperConversation()
        {
            var commands = new List<string>() { "Use|key", "Eat|apple", "Move North" };

            // Instructions
            CommandMapperConversation.AppendSystemMessage("You will act as a phrase-to-command mapper in a text based dungeaon crawler game");
            CommandMapperConversation.AppendSystemMessage("I will send you messages that begins with a comma separated list of available commands, followed by the user phrase that needs to be mapped to one of them.");
            CommandMapperConversation.AppendSystemMessage("You should return the command that fits the phrase most accurately, but ONLY if it is a semantically close match");
            CommandMapperConversation.AppendSystemMessage("Commands with parameters will contain a pipe seperator with the command on the left side and the comma separated parameters on the right side");


            // Training

            var unknownCommandResponse = $"Sorry I do not understand. Try another command such as:\n{string.Join('\n', commands.Select(c => $"- {c.Replace('|', ' ')}"))}";

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nUse the key");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nUse my key");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nMake use of key");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nUtilize the key");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nUse the key on the door");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nUse key to open door");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nUse the key to open the door");
            CommandMapperConversation.AppendExampleChatbotOutput("Use|key");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nJump");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nSwallow key");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nEat key");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nWalk key");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nNourish key");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nThrow the key north");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nKey");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nEat apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nEat the apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nConsume apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nEat my apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nSwallow an apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nGobble down apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nIngest apple");
            CommandMapperConversation.AppendExampleChatbotOutput("Eat|apple");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nThrow apple");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nBlow apple");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nForget apple");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nMove towards apple");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nHead north");
            CommandMapperConversation.AppendExampleChatbotOutput("Move North");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nGo north");
            CommandMapperConversation.AppendExampleChatbotOutput("Move Northt");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nMove towards north");
            CommandMapperConversation.AppendExampleChatbotOutput("Move North");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nKeep moving north");
            CommandMapperConversation.AppendExampleChatbotOutput("Move North");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nWalk north");
            CommandMapperConversation.AppendExampleChatbotOutput("Move North");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nTravel north");
            CommandMapperConversation.AppendExampleChatbotOutput("Move North");

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nSpool north");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nEat north");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nApple north");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nLook north");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);

            CommandMapperConversation.AppendUserInput($"{string.Join(',', commands)}\n\nStart north");
            CommandMapperConversation.AppendExampleChatbotOutput(unknownCommandResponse);
        }

        private void InitFoodItemGeneratorConversation()
        {
            RoomGeneratorConversation.AppendSystemMessage("Your job will be to return a list of 5 to 15 json objects, based on a scheme that I will provide, that represents various food items you can find in a dungean crawler game.");
            RoomGeneratorConversation.AppendSystemMessage($"The items should be related to the theme and have varying property values, such as how much health they regenerate, that are as realistic as possible in context of the item.");
            RoomGeneratorConversation.AppendSystemMessage("The players health spans between 0 and 100, and decreases by 2 every turn in the game.");
            RoomGeneratorConversation.AppendSystemMessage("You should only create as many items as you can accurately create from the theme, so if there are only 6 food items that are related to the theme, then only create 6. In short, quality over quantity.");
        }
    }
}