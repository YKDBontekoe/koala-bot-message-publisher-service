namespace Koala.MessagePublisherService.Models;

public class Message
{
    public ulong Id { get; set; }
    public User User { get; set; }
    public string Content { get; set; } = string.Empty;
    public Channel? Channel { get; set; }

    public Guild? Guild { get; set; }
    public DateTimeOffset Time { get; set; }
    public DateTimeOffset? EditedTime { get; set; }
    public List<Attachment> Attachments { get; set; } = new ();
}