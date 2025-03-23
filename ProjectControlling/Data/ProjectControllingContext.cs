using ProjectControlling.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectControlling.Data;

public class ProjectControllingContext : DbContext
{
    public ProjectControllingContext(DbContextOptions<ProjectControllingContext> options) : base(options) { }
    
    public DbSet<Projects> Projects { get; set; } = null!;
    public DbSet<Workers> Workers { get; set; } = null!;
    public DbSet<ProjectWorkers> ProjectWorkers { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectWorkers>()
            .HasKey(pw => new { pw.ProjectId, pw.WorkerId });
        
        modelBuilder.Entity<ProjectWorkers>()
            .HasOne(pw => pw.Project)
            .WithMany(p => p.ProjectWorkers)
            .HasForeignKey(pw => pw.ProjectId);

        modelBuilder.Entity<ProjectWorkers>()
            .HasOne(pw => pw.Worker)
            .WithMany(p => p.ProjectWorkers)
            .HasForeignKey(pw => pw.WorkerId);
    }
}