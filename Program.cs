using Discord;
using Discord.WebSocket;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Handlers.Interfaces;
using Koala.MessagePublisherService.Services;
using Koala.MessagePublisherService.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Koala.MessagePublisherService;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.UseRabbitMQMessagePublisher(hostContext.Configuration);
                services.UseRabbitMQMessageHandler(hostContext.Configuration);
                services.AddHostedService<MessagePublisherWorker>();
                
                var config = new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    GatewayIntents = GatewayIntents.All
                };
                var client = new DiscordSocketClient(config);

                client.LoginAsync(TokenType.Bot, hostContext.Configuration["Discord:Token"]);
                client.StartAsync();
                
                services.AddTransient<ILoggingService>(_ => 
                    new LoggingService(client));
                
                services.AddTransient<IMessageService>(_ => new MessageService(client,
                    services.BuildServiceProvider().GetService<IMessagePublisher>() ??
                    throw new InvalidOperationException()));
            })
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .UseConsoleLifetime()
            .Build();

        await host.RunAsync();
    }
}