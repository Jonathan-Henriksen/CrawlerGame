using CrawlerGame.Logic;
using CrawlerGame.Logic.Factories;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services;
using CrawlerGame.Logic.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<IGameEngine>().Init().Start();
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
            services.AddSingleton<IChatGPTService, ChatGPTService>();
            services.AddSingleton<ICommandFactory, CommandFactory>();
            services.AddSingleton<IGameEngine, GameEngine>();

            services.Configure<OpenAIOptions>(_.Configuration.GetSection(nameof(OpenAIOptions)));
        });
}