using Koala.MessagePublisherService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.MessagePublisherService;

public class MessagePublisherWorker : IHostedService, IDisposable {
    private readonly IMessageService _messageService;

    public MessagePublisherWorker(IMessageService messageService)
    {
        _messageService = messageService;
    }

    // This method is called when the host starts
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messageService.Initialize();
        return Task.CompletedTask;
    }

    // This method is called when the host stops
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    // This method is called when the host is disposed
    public void Dispose()
    {
        _messageService.Dispose();
    }
}