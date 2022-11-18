using Azure.Messaging.ServiceBus;
using Discord.WebSocket;
using Koala.MessagePublisherService.Models;
using Koala.MessagePublisherService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Koala.MessagePublisherService.Services;

public class MessageService : IMessageService
{
    private readonly BaseSocketClient _client;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IConfiguration _configuration;

    public MessageService(BaseSocketClient client, ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _client = client;
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _client.MessageReceived += Client_MessageReceived;
    }

    // Read all incoming messages and log them
    private async Task Client_MessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;
        if (arg is not SocketUserMessage message) return;
        
        var messageReceived = new Message
        {
            Id = message.Id,
            Content = message.Content,
            Channel = new Channel
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
        
        var sender = _serviceBusClient.CreateSender(_configuration["ServiceBus:QueueName"]);
        await sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(messageReceived)));
    }

    public void Initialize()
    {
        _client.MessageReceived += Client_MessageReceived;
    }

    public void Dispose()
    {
        _client.MessageReceived -= Client_MessageReceived;
    }
}