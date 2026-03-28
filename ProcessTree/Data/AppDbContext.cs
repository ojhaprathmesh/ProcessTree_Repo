using Microsoft.EntityFrameworkCore;
using ProcessTree.Models;

namespace ProcessTree.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ProcessItem> ProcessItems { get; set; }
        public DbSet<ProcessingRule> ProcessingRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Self-referencing relationship — parent has many children
            // Restrict delete so you can't accidentally delete a parent while children still exist
            modelBuilder.Entity<ProcessItem>()
                .HasOne(p => p.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision for Weight column
            modelBuilder.Entity<ProcessItem>()
                .Property(p => p.Weight)
                .HasPrecision(10, 2);

            // Seed two default processing rules so app isn't empty on first run
            modelBuilder.Entity<ProcessingRule>().HasData(
                new ProcessingRule
                {
                    Id = 1,
                    Name = "Equal Split — 2 Outputs",
                    OutputCount = 2,
                    SplitMode = WeightSplitMode.Equal,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new ProcessingRule
                {
                    Id = 2,
                    Name = "Equal Split — 3 Outputs",
                    OutputCount = 3,
                    SplitMode = WeightSplitMode.Equal,
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        }
    }
}