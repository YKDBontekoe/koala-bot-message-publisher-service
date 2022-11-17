using Discord.WebSocket;
using Koala.Infrastructure.Messaging.Handlers.Interfaces;
using Koala.MessagePublisherService.Models;
using Koala.MessagePublisherService.Services.Interfaces;

namespace Koala.MessagePublisherService.Services;

public class MessageService : IMessageService
{
    private readonly BaseSocketClient _client;
    private readonly IMessagePublisher _publisher;
    
    public MessageService(BaseSocketClient client, IMessagePublisher publisher)
    {
        _client = client;
        _publisher = publisher;
    }

    // Read all incoming messages and log them
    private async Task Client_MessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;
        if (arg is not SocketUserMessage message) return;
        
        var messageReceived = new Message()
        {
            Content = message.Content,
            Channel = new Channel()
            {
                Id = message.Channel.Id,
                Name = message.Channel.Name
            },
            User = new User
            {
                Id = message.Author.Id,
                Username = message.Author.Username
            }
        };
        
        await _publisher.PublishMessageAsync("MESSAGE_RECEIVED", messageReceived, string.Empty);
    }

    public void Initialize()
    {
        _client.MessageReceived += Client_MessageReceived;
    }
}