using System;
using System.ComponentModel.DataAnnotations;

namespace ServicePlanner.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Song Name
        
        [Required]
        [StringLength(100)]
        public string SongName { get; set; } // Alternative Song Name property
        
        [StringLength(10)]
        public string? Key { get; set; } // Musical Key (e.g., C, G, Am, etc.)
        
        [StringLength(50)]
        public string? Category { get; set; } // Song Category
        
        [StringLength(500)]
        public string? Notes { get; set; } // Song Notes
        
        public int? SongSelectId { get; set; } // SongSelect ID (Can be null)
        
        public bool Seasonal { get; set; } // Seasonal (Boolean)
        
        [Required]
        [StringLength(50)]
        public string Speed { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Publisher { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Artist { get; set; }
        
        public bool Disabled { get; set; } // Disabled (Boolean)
        
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Created Date
    }
}
