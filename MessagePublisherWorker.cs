using Infrastructure.Messaging.Handlers.Interfaces;
using Koala.MessagePublisherService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.MessagePublisherService;

public class MessagePublisherWorker : IHostedService, IMessageHandlerCallback
{
    private readonly IMessageHandler _messageHandler;
    private readonly IMessageService _messageService;
    private readonly ILoggingService _loggingService;

    public MessagePublisherWorker(IMessageHandler messageHandler, IMessageService messageService, ILoggingService loggingService)
    {
        _messageHandler = messageHandler;
        _messageService = messageService;
        _loggingService = loggingService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messageHandler.Start(this);
        _messageService.Initialize();
        _loggingService.Initialize();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messageHandler.Stop();
        return Task.CompletedTask;
    }

    public Task<bool> HandleMessageAsync(string messageType, string message)
    {
        return Task.FromResult(true);
    }
}