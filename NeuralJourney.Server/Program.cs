using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Logic.Commands;
using NeuralJourney.Logic.Commands.Admin;
using NeuralJourney.Logic.Commands.Players;
using NeuralJourney.Logic.Engines;
using NeuralJourney.Logic.Handlers;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using Serilog;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<IGameEngine>().Run();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger>();
    logger.Error(ex, ex.Message);
}

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", false);
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddTransient<IClockService, ClockService>();
            services.AddTransient<ICommandDispatcher, CommandDispatcher>();
            services.AddTransient<IInputHandler, InputHandler>();
            services.AddTransient<IMessageService, MessageService>();

            services.AddTransient<IAdminCommandStrategy, AdminCommandStrategy>();
            services.AddTransient<IPlayerCommandStrategy, PlayerCommandStrategy>();

            services.AddSingleton<ICommandFactory, CommandFactory>();
            services.AddSingleton<IConnectionHandler, ConnectionHandler>();
            services.AddSingleton<IGameEngine, GameEngine>();

            services.AddSingleton<IOpenAIService, OpenAIService>();

            services.AddOptions<GameOptions>().BindConfiguration("Game");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GameOptions>>().Value);

            services.AddOptions<OpenAIOptions>().BindConfiguration("OpenAI");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OpenAIOptions>>().Value);

            services.AddOptions<ServerOptions>().BindConfiguration("Server");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ServerOptions>>().Value);
        });
}