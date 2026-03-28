using ProcessTree.Models;
using ProcessTree.Models.ViewModels;

namespace ProcessTree.Services
{
    public interface IProcessingService
    {
        Task<List<ProcessItem>> GetAllRootItemsAsync();
        Task<ProcessItem?> GetItemWithChildrenAsync(int id);
        Task<ProcessItem> CreateRootItemAsync(ProcessItemViewModel model);
        Task<List<ProcessItem>> ProcessItemAsync(ProcessActionViewModel model);
        Task<TreeNodeViewModel?> BuildTreeAsync(int rootId);
    }
}