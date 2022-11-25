namespace Koala.MessagePublisherService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";
    
    public string QueueName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}