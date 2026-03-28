namespace ProcessTree.Models.ViewModels
{
    public class TreeNodeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public bool IsProcessed { get; set; }
        public int Depth { get; set; }
        public List<TreeNodeViewModel> Children { get; set; } = new();
    }
}