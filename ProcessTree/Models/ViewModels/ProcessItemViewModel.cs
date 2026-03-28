using System.ComponentModel.DataAnnotations;
using ProcessTree.Models;

namespace ProcessTree.Models.ViewModels
{
    public class ProcessItemViewModel
    {
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100)]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Weight must be between 0.01 and 100,000 kg")]
        [Display(Name = "Weight (kg)")]
        public decimal Weight { get; set; }

        public string? Notes { get; set; }
    }

    public class ProcessActionViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal ItemWeight { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Must produce 1 to 20 outputs")]
        [Display(Name = "Number of Output Items")]
        public int OutputCount { get; set; }

        [Display(Name = "Weight Split Mode")]
        public WeightSplitMode SplitMode { get; set; } = WeightSplitMode.Equal;

        [Display(Name = "Custom Weights (comma-separated)")]
        public string? CustomWeights { get; set; }
    }
}