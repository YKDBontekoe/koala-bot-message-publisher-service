using Koala.MessagePublisherService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.MessagePublisherService;

public class MessagePublisherWorker : IHostedService, IDisposable {
    private readonly IMessageService _messageService;

    public MessagePublisherWorker(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messageService.Initialize();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _messageService.Dispose();
    }
}