using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Client;
using NeuralJourney.Logic.Options;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    var gameClient = await services.GetRequiredService<GameClient>().Init();

    await gameClient.Run();
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
            services.AddSingleton<GameClient>();

            services.AddOptions<ClientOptions>()
            .BindConfiguration("Client");

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ClientOptions>>().Value);
        });
}