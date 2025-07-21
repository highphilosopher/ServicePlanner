namespace ServicePlanner.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public int TemplateId { get; set; }
        public ServiceTemplate? Template { get; set; }
        public List<ServiceEventInstance> EventInstances { get; set; } = new List<ServiceEventInstance>();
        public bool IsSeasonal { get; set; } // New property for seasonal services
    }

    public class ServiceEventInstance
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ServiceEventId { get; set; }
        public ServiceEvent? ServiceEvent { get; set; }
        public string? PersonName { get; set; }
        public string? SongTitle { get; set; }
        public string? Notes { get; set; }
    }
}
