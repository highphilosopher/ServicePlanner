namespace ServicePlanner.Models
{
    public enum EventType
    {
        Speaker,
        Prayer,
        Communion,
        Baptism,
        Song
    }

    public class ServiceEvent
    {
        public int Id { get; set; }
        public EventType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public int TemplateId { get; set; }
    }
}
