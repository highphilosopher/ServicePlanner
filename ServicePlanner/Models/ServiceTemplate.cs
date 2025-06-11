namespace ServicePlanner.Models
{
    public class ServiceTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<ServiceEvent> Events { get; set; } = new List<ServiceEvent>();
    }
}
