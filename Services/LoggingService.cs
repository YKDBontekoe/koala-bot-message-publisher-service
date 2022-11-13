using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Koala.MessagePublisherService.Services.Interfaces;
using Serilog.Core;

namespace Koala.MessagePublisherService.Services;

public class LoggingService : ILoggingService
{
    private readonly DiscordSocketClient _client;

    public LoggingService(DiscordSocketClient client)
    {
        _client = client;
    }

    private static Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            // Write the exception to serilog
            Logger.None.Error(cmdException, "Error executing command {CommandName} in {SourceText}", cmdException.Command.Name, cmdException.Context.Message.Content);
        }
        else 
        {
            // Write the message to serilog
            Logger.None.Information("{Message}", message.Message);
        }

        return Task.CompletedTask;
    }

    public void Initialize()
    {
        _client.Log += LogAsync;
    }
}