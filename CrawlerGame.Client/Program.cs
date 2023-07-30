using CrawlerGame.Logic;
using CrawlerGame.Logic.Factories.Interfaces;
using CrawlerGame.Logic.Options;
using CrawlerGame.Logic.Services;
using CrawlerGame.Logic.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
            services.AddSingleton<IOpenAIService, OpenAIService>();
            services.AddSingleton<ICommandFactory, CommandFactory>();
            services.AddSingleton<IGameEngine, GameEngine>();

            services.AddOptions<OpenAIOptions>()
            .BindConfiguration("OpenAI");

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<OpenAIOptions>>().Value);
        });
}