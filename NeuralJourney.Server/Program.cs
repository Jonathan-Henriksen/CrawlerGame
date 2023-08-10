using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Logic.Commands;
using NeuralJourney.Logic.Commands.Admin;
using NeuralJourney.Logic.Commands.Middleware;
using NeuralJourney.Logic.Commands.Players;
using NeuralJourney.Logic.Engines;
using NeuralJourney.Logic.Handlers.Connection;
using NeuralJourney.Logic.Handlers.Input;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using Serilog;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<IEngine>().Run();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger>();
    logger.Error(ex, ex.Message);
}

static IHostBuilder CreateHostBuilder(string[] strings)
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

            services.AddTransient<IInputHandler, ConsoleInputHandler>();
            services.AddTransient<IInputHandler, PlayerInputHandler>();

            services.AddTransient<IMessageService, MessageService>();

            services.AddSingleton<IConnectionHandler, PlayerConnectionHandler>();
            services.AddSingleton<IEngine, ServerEngine>();
            services.AddSingleton<IOpenAIService, OpenAIService>();

            // Register Command Middleware
            services.AddSingleton<ICommandFactory, CommandFactory>();

            services.AddTransient<ICommandDispatcher, CommandDispatcher>();
            services.AddTransient<ICommandStrategyFactory, CommandStrategyFactory>();
            services.AddTransient<ICommandStrategy, AdminCommandStrategy>();
            services.AddTransient<ICommandStrategy, PlayerCommandStrategy>();

            RegisterCommandMiddleware(services);

            // Configure Options
            services.AddOptions<GameOptions>().BindConfiguration("Game");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GameOptions>>().Value);

            services.AddOptions<OpenAIOptions>().BindConfiguration("OpenAI");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OpenAIOptions>>().Value);

            services.AddOptions<ServerOptions>().BindConfiguration("Server");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ServerOptions>>().Value);
        });
}

static IServiceCollection RegisterCommandMiddleware(IServiceCollection services)
{
    return services
        .AddTransient<ICommandMiddleware, InputValidation>()
        .AddTransient<ICommandMiddleware, CompletionTextRequester>()
        .AddTransient<ICommandMiddleware, CompletionTextExtractor>()
        .AddTransient<ICommandMiddleware, CommandInstantiator>()
        .AddTransient<ICommandMiddleware, CommandExecutor>()
        .AddTransient<ICommandMiddleware, ResultProcessor>();
}