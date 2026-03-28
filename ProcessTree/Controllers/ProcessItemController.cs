using Microsoft.AspNetCore.Mvc;
using ProcessTree.Models.ViewModels;
using ProcessTree.Services;

namespace ProcessTree.Controllers
{
    public class ProcessItemController(
        IProcessingService service,
        ILogger<ProcessItemController> logger) : Controller
    {
        private readonly IProcessingService _service = service;
        private readonly ILogger<ProcessItemController> _logger = logger;

        // GET: /ProcessItem
        public async Task<IActionResult> Index()
        {
            var items = await _service.GetAllRootItemsAsync();
            return View(items);
        }

        // GET: /ProcessItem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ProcessItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProcessItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var item = await _service.CreateRootItemAsync(model);
                TempData["Success"] = $"Item '{item.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                ModelState.AddModelError("", "Something went wrong. Please try again.");
                return View(model);
            }
        }

        // GET: /ProcessItem/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetItemWithChildrenAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ProcessItem/Process/5
        public async Task<IActionResult> Process(int id)
        {
            var item = await _service.GetItemWithChildrenAsync(id);

            if (item == null) return NotFound();

            if (item.IsProcessed)
            {
                TempData["Warning"] = "This item has already been processed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = new ProcessActionViewModel
            {
                ItemId = item.Id,
                ItemName = item.Name,
                ItemWeight = item.Weight,
                OutputCount = 2,
                SplitMode = Models.WeightSplitMode.Equal
            };

            return View(vm);
        }

        // POST: /ProcessItem/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(ProcessActionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var outputs = await _service.ProcessItemAsync(model);
                TempData["Success"] = $"Done! {outputs.Count} output item(s) created.";
                return RedirectToAction(nameof(Tree), new { id = model.ItemId });
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violations — show directly to user
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Process), new { id = model.ItemId });
            }
            catch (ArgumentException ex)
            {
                // Validation errors — show on form
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during processing");
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Details), new { id = model.ItemId });
            }
        }

        // GET: /ProcessItem/Tree/5
        public async Task<IActionResult> Tree(int id)
        {
            var tree = await _service.BuildTreeAsync(id);
            if (tree == null) return NotFound();
            return View(tree);
        }
    }
}