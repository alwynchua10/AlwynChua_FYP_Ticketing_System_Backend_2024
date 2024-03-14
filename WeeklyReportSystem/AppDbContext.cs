using Microsoft.EntityFrameworkCore;
using WeeklyReportSystem.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Report> reports { get; set; }
    public DbSet<Category> categories { get; set; }
    public DbSet<Tasks> tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>()
            .HasMany(r => r.Tasks)
            .WithOne()
            .HasForeignKey(t => t.ReportID);


        modelBuilder.Entity<Tasks>()
            .HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryID);
    }


}
