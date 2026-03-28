using System.ComponentModel.DataAnnotations;

namespace ProcessTree.Models
{
    public enum WeightSplitMode
    {
        Equal,
        Custom
    }

    public class ProcessingRule
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Rule Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 20, ErrorMessage = "Output count must be between 1 and 20")]
        [Display(Name = "Number of Outputs")]
        public int OutputCount { get; set; }

        [Display(Name = "Weight Split Mode")]
        public WeightSplitMode SplitMode { get; set; } = WeightSplitMode.Equal;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}