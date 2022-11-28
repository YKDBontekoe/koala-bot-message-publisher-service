namespace Koala.MessagePublisherService.Models;

public class Attachment
{
    public ulong Id { get; set; }
    public string Url { get; set; }
    public string ProxyUrl { get; set; }
    public string Filename { get; set; }
    public int Size { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
}