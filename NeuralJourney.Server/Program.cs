using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Logic;
using NeuralJourney.Logic.Factories;
using NeuralJourney.Logic.Factories.Interfaces;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using NeuralJourney.Logic.Services.Interfaces;
using NeuralJourney.Server;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<GameServer>().Init().Run();
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
            services.AddSingleton<GameServer>();

            services.AddTransient<ICommandFactory, CommandFactory>();
            services.AddTransient<IClockService, ClockService>();

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