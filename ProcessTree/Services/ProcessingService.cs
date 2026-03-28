using ProcessTree.Data;

namespace ProcessTree.Services
{
    public class ProcessingService(AppDbContext db) : IProcessingService
    {
        private readonly AppDbContext _db = db;
    }
}