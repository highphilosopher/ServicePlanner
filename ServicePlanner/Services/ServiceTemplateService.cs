using Microsoft.EntityFrameworkCore;
using ServicePlanner.Data;
using ServicePlanner.Models;

namespace ServicePlanner.Services
{
    public class ServiceTemplateService
    {
        private readonly ServicePlannerContext _context;

        public ServiceTemplateService(ServicePlannerContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceTemplate>> GetAllTemplatesAsync(bool includeDeprecated = false)
        {
            var query = _context.ServiceTemplates.Include(t => t.Events).AsQueryable();
            
            if (!includeDeprecated)
            {
                query = query.Where(t => t.IsActive);
            }
            
            return await query.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<List<ServiceTemplate>> GetActiveTemplatesAsync()
        {
            return await _context.ServiceTemplates
                .Include(t => t.Events)
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<ServiceTemplate?> GetTemplateByIdAsync(int id)
        {
            return await _context.ServiceTemplates
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ServiceTemplate> CreateTemplateAsync(ServiceTemplate template)
        {
            template.CreatedDate = DateTime.Now;
            
            foreach (var evt in template.Events)
            {
                evt.TemplateId = template.Id;
            }
            
            _context.ServiceTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<ServiceTemplate> UpdateTemplateAsync(ServiceTemplate template)
        {
            // Check if template is in use - if so, prevent structural updates
            var isInUse = await IsTemplateInUseAsync(template.Id);
            
            var existingTemplate = await _context.ServiceTemplates
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == template.Id);

            if (existingTemplate != null)
            {
                if (isInUse)
                {
                    // If template is in use, only allow name and description updates
                    existingTemplate.Name = template.Name;
                    existingTemplate.Description = template.Description;
                    // Do not modify events when template is in use
                }
                else
                {
                    // Template not in use, allow full updates
                    existingTemplate.Name = template.Name;
                    existingTemplate.Description = template.Description;
                    
                    // Remove existing events
                    _context.ServiceEvents.RemoveRange(existingTemplate.Events);
                    
                    // Add updated events
                    existingTemplate.Events = template.Events;
                    foreach (var evt in existingTemplate.Events)
                    {
                        evt.TemplateId = template.Id;
                    }
                }
                
                await _context.SaveChangesAsync();
                return existingTemplate;
            }
            
            return template;
        }

        public async Task<bool> DeleteTemplateAsync(int id)
        {
            var template = await _context.ServiceTemplates
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == id);
                
            if (template != null)
            {
                _context.ServiceTemplates.Remove(template);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> IsTemplateInUseAsync(int templateId)
        {
            return await _context.Services.AnyAsync(s => s.TemplateId == templateId);
        }

        public async Task<bool> ToggleTemplateActiveStatusAsync(int templateId)
        {
            var template = await _context.ServiceTemplates.FirstOrDefaultAsync(t => t.Id == templateId);
            if (template != null)
            {
                template.IsActive = !template.IsActive;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
