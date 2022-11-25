using Azure.Messaging.ServiceBus;
using Discord;
using Discord.WebSocket;
using Koala.MessagePublisherService.Models;
using Koala.MessagePublisherService.Options;
using Koala.MessagePublisherService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.MessagePublisherService.Services;

public class MessageService : IMessageService
{
    private readonly DiscordSocketClient _client;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusOptions _serviceBusOptions;
    private readonly DiscordOptions _discordOptions;

    public MessageService(DiscordSocketClient client, ServiceBusClient serviceBusClient, IOptions<DiscordOptions> discordOptions, IOptions<ServiceBusOptions> serviceBusOptions, IConfiguration configuration)
    {
        _client = client;
        _serviceBusClient = serviceBusClient;
        _serviceBusOptions = serviceBusOptions != null ? serviceBusOptions.Value : throw new ArgumentNullException(nameof(serviceBusOptions));
        _discordOptions = discordOptions != null ? discordOptions.Value : throw new ArgumentNullException(nameof(discordOptions));
        
        InitializeDiscordClient();
    }
    
    // Read all incoming messages from the Discord client and send them to the Service Bus
    private async Task Client_MessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot) return;
        if (arg is not SocketUserMessage message) return;
        
        var messageReceived = new Message
        {
            Id = message.Id,
            Content = message.Content,
            Time = message.Timestamp,
            EditedTime = message.EditedTimestamp,
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

        if (arg.Channel is SocketGuildChannel guildChannel)
        {
            messageReceived.Guild = new Guild
            {
                Id = guildChannel.Guild.Id,
                Name = guildChannel.Guild.Name,
            };
        }
        
        var sender = _serviceBusClient.CreateSender(_serviceBusOptions.QueueName);
        await sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(messageReceived)));
    }

    // Initialize the discord message events
    public void Initialize()
    {
        _client.MessageReceived += Client_MessageReceived;
    }

    // Dispose of the client when the service is disposed
    public void Dispose()
    {
        _client.MessageReceived -= Client_MessageReceived;
    }
    
    // Initialize the Discord client and connect to the gateway
    private void InitializeDiscordClient()
    {
        _client.LoginAsync(TokenType.Bot, _discordOptions.Token);
        _client.StartAsync();
    }
}