using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend;

public class BoardDbContext : DbContext
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<Stroke> Strokes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Boards");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>()
            .HasMany(b => b.Strokes)
            .WithOne()
            .HasForeignKey(s => s.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Stroke>()
            .OwnsMany(s => s.Points, p 
                => p.WithOwner().HasForeignKey("StrokeId")
            );
    }
}
