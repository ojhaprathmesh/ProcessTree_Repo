using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessTree.Models
{
    public class ProcessItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters")]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, 100000, ErrorMessage = "Weight must be between 0.01 and 100,000")]
        [Display(Name = "Weight (kg)")]
        public decimal Weight { get; set; }

        // If null, this is a root item
        public int? ParentId { get; set; }

        [Display(Name = "Processed?")]
        public bool IsProcessed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties — EF Core uses these to do JOINs automatically
        [ForeignKey("ParentId")]
        public virtual ProcessItem? Parent { get; set; }

        public virtual ICollection<ProcessItem> Children { get; set; } 
            = new List<ProcessItem>();
    }
}