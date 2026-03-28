using Microsoft.EntityFrameworkCore;
using ProcessTree.Data;
using ProcessTree.Models;
using ProcessTree.Models.ViewModels;

namespace ProcessTree.Services
{
    public class ProcessingService : IProcessingService
    {
        private readonly AppDbContext _db;
        private const int MaxDepth = 10;

        public ProcessingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProcessItem>> GetAllRootItemsAsync()
        {
            return await _db.ProcessItems
                .Where(x => x.ParentId == null)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProcessItem?> GetItemWithChildrenAsync(int id)
        {
            return await _db.ProcessItems
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProcessItem> CreateRootItemAsync(ProcessItemViewModel model)
        {
            var item = new ProcessItem
            {
                Name = model.Name.Trim(),
                Weight = model.Weight,
                Notes = model.Notes?.Trim(),
                IsProcessed = false,
                ParentId = null
            };

            _db.ProcessItems.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<List<ProcessItem>> ProcessItemAsync(ProcessActionViewModel model)
        {
            var parent = await _db.ProcessItems
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == model.ItemId);

            if (parent == null)
                throw new InvalidOperationException("Item not found.");

            if (parent.IsProcessed)
                throw new InvalidOperationException("This item has already been processed.");

            // Depth guard — prevent infinite recursive chains
            int currentDepth = await GetItemDepthAsync(parent.Id);
            if (currentDepth >= MaxDepth)
                throw new InvalidOperationException(
                    $"Maximum depth of {MaxDepth} levels reached. Cannot process further.");

            var weights = CalculateOutputWeights(model, parent.Weight);
            var children = new List<ProcessItem>();

            for (int i = 0; i < model.OutputCount; i++)
            {
                var child = new ProcessItem
                {
                    Name = $"{parent.Name} — Output {i + 1}",
                    Weight = weights[i],
                    ParentId = parent.Id,
                    IsProcessed = false,
                    Notes = $"Generated from '{parent.Name}'"
                };
                children.Add(child);
            }

            _db.ProcessItems.AddRange(children);

            // Mark parent as done
            parent.IsProcessed = true;
            _db.ProcessItems.Update(parent);

            await _db.SaveChangesAsync();
            return children;
        }

        private List<decimal> CalculateOutputWeights(
            ProcessActionViewModel model, decimal totalWeight)
        {
            var weights = new List<decimal>();

            if (model.SplitMode == WeightSplitMode.Equal)
            {
                decimal baseWeight = Math.Round(totalWeight / model.OutputCount, 2);
                decimal assigned = 0;

                for (int i = 0; i < model.OutputCount; i++)
                {
                    if (i == model.OutputCount - 1)
                        weights.Add(Math.Round(totalWeight - assigned, 2));
                    else
                    {
                        weights.Add(baseWeight);
                        assigned += baseWeight;
                    }
                }
            }
            else // Custom
            {
                if (string.IsNullOrWhiteSpace(model.CustomWeights))
                    throw new ArgumentException("Custom weights must be provided.");

                var parts = model.CustomWeights.Split(',');
                decimal total = 0;

                foreach (var part in parts)
                {
                    if (!decimal.TryParse(part.Trim(), out decimal w) || w <= 0)
                        throw new ArgumentException(
                            $"Invalid weight value: '{part.Trim()}'");

                    weights.Add(Math.Round(w, 2));
                    total += w;
                }

                if (weights.Count != model.OutputCount)
                    throw new ArgumentException(
                        $"Expected {model.OutputCount} weights, got {weights.Count}.");

                if (total > totalWeight + 0.01m)
                    throw new ArgumentException(
                        $"Total custom weight ({total}) exceeds item weight ({totalWeight}).");
            }

            return weights;
        }

        private async Task<int> GetItemDepthAsync(int itemId)
        {
            int depth = 0;
            var current = await _db.ProcessItems.FindAsync(itemId);

            while (current?.ParentId != null)
            {
                current = await _db.ProcessItems.FindAsync(current.ParentId);
                depth++;
                if (depth > MaxDepth + 1) break; // safety net
            }

            return depth;
        }

        public async Task<TreeNodeViewModel?> BuildTreeAsync(int rootId)
        {
            // Load all items once — avoids N+1 query problem
            var allItems = await _db.ProcessItems.ToListAsync();
            var root = allItems.FirstOrDefault(x => x.Id == rootId);

            if (root == null) return null;

            return BuildNode(root, allItems, 0);
        }

        private TreeNodeViewModel BuildNode(
            ProcessItem item, List<ProcessItem> all, int depth)
        {
            var node = new TreeNodeViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Weight = item.Weight,
                IsProcessed = item.IsProcessed,
                Depth = depth,
                Children = new List<TreeNodeViewModel>()
            };

            var children = all.Where(x => x.ParentId == item.Id).ToList();

            foreach (var child in children)
            {
                if (depth < MaxDepth)
                    node.Children.Add(BuildNode(child, all, depth + 1));
            }

            return node;
        }
    }
}