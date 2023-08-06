using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Library.Enums;
using NeuralJourney.Logic.Commands.Admin.Base;
using NeuralJourney.Logic.Commands.Players.Base;
using NeuralJourney.Logic.Dispatchers;
using NeuralJourney.Logic.Dispatchers.Interfaces;
using NeuralJourney.Logic.Engines;
using NeuralJourney.Logic.Engines.Interfaces;
using NeuralJourney.Logic.Factories;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Handlers;
using NeuralJourney.Logic.Handlers.Interfaces;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using NeuralJourney.Logic.Services.Interfaces;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<IGameEngine>().Run();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", false);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddTransient<IClockService, ClockService>();
            services.AddTransient<ICommandDispatcher, CommandDispatcher>();

            services.AddSingleton(typeof(ICommandFactory<AdminCommand, AdminCommandEnum>), typeof(CommandFactory<AdminCommand, AdminCommandEnum>));
            services.AddSingleton(typeof(ICommandFactory<PlayerCommand, PlayerCommandEnum>), typeof(CommandFactory<PlayerCommand, PlayerCommandEnum>));

            services.AddSingleton<IConnectionHandler, ConnectionHandler>();
            services.AddSingleton<IGameEngine, GameEngine>();
            services.AddSingleton<IInputHandler, InputHandler>();
            services.AddSingleton<IOpenAIService, OpenAIService>();

            services.AddOptions<GameOptions>().BindConfiguration("Game");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GameOptions>>().Value);

            services.AddOptions<OpenAIOptions>().BindConfiguration("OpenAI");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OpenAIOptions>>().Value);

            services.AddOptions<ServerOptions>().BindConfiguration("Server");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ServerOptions>>().Value);
        });
}