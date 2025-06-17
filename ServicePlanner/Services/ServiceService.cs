using Microsoft.EntityFrameworkCore;
using ServicePlanner.Data;
using ServicePlanner.Models;

namespace ServicePlanner.Services
{
    public class ServiceService
    {
        private readonly ServicePlannerContext _context;

        public ServiceService(ServicePlannerContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _context.Services
                .Include(s => s.Template)
                .Include(s => s.EventInstances)
                    .ThenInclude(ei => ei.ServiceEvent)
                .OrderByDescending(s => s.ServiceDate)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Template)
                .Include(s => s.EventInstances)
                    .ThenInclude(ei => ei.ServiceEvent)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            // Get the template to ensure it exists
            var template = await _context.ServiceTemplates
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == service.TemplateId);
                
            if (template != null)
            {
                service.Template = template;
                
                // If EventInstances are already populated (from UI), preserve them
                // Otherwise, create new ones from template
                if (!service.EventInstances.Any())
                {
                    service.EventInstances = template.Events.Select(evt => new ServiceEventInstance
                    {
                        ServiceId = service.Id,
                        ServiceEventId = evt.Id,
                        ServiceEvent = evt,
                        PersonName = "",
                        SongTitle = "",
                        Notes = ""
                    }).ToList();
                }
                else
                {
                    // EventInstances already exist from UI, just ensure ServiceId is set
                    foreach (var instance in service.EventInstances)
                    {
                        instance.ServiceId = service.Id;
                    }
                }
            }
            
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<Service> UpdateServiceAsync(Service service)
        {
            var existingService = await _context.Services
                .Include(s => s.EventInstances)
                .FirstOrDefaultAsync(s => s.Id == service.Id);

            if (existingService != null)
            {
                existingService.Name = service.Name;
                existingService.ServiceDate = service.ServiceDate;
                
                // Update event instances
                foreach (var instance in service.EventInstances)
                {
                    var existingInstance = existingService.EventInstances
                        .FirstOrDefault(ei => ei.Id == instance.Id);
                        
                    if (existingInstance != null)
                    {
                        existingInstance.PersonName = instance.PersonName;
                        existingInstance.SongTitle = instance.SongTitle;
                        existingInstance.Notes = instance.Notes;
                    }
                }
                
                await _context.SaveChangesAsync();
                return existingService;
            }
            
            return service;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Services
                .Include(s => s.EventInstances)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Service>> GetServicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Services
                .Include(s => s.Template)
                .Include(s => s.EventInstances)
                    .ThenInclude(ei => ei.ServiceEvent)
                .Where(s => s.ServiceDate >= startDate && s.ServiceDate <= endDate)
                .OrderBy(s => s.ServiceDate)
                .ToListAsync();
        }
    }
}
