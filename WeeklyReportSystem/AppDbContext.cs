using Microsoft.EntityFrameworkCore;
using WeeklyReportSystem.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Priority> Priorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ticket-Category relationship
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CategoryID);

        // Ticket-User relationship (User who submitted the ticket)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserID);

        // Ticket-Status relationship
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Status)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.StatusID);

        // Ticket-Priority relationship
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Priority)
            .WithMany(p => p.Tickets) // Inverse property
            .HasForeignKey(t => t.PriorityID);

        // Comment-User relationship
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserID);

        // Comment-Ticket relationship
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments) // Assuming you want a one-to-many relationship
            .HasForeignKey(c => c.TicketID);

        // Add the configuration for CommentImage as a byte array
        modelBuilder.Entity<Comment>()
            .Property(c => c.CommentImage)
            .IsRequired(false); // Make it optional; set to true if you want to enforce this

        // Table mappings
        modelBuilder.Entity<Priority>()
            .ToTable("Priorities");

        modelBuilder.Entity<User>()
            .ToTable("Users");

        modelBuilder.Entity<Ticket>()
            .ToTable("Tickets");

        modelBuilder.Entity<Category>()
            .ToTable("Categories");

        modelBuilder.Entity<Status>()
            .ToTable("Statuses");

        modelBuilder.Entity<Ticket>()
         .Property(t => t.SubmissionDate)
         .HasConversion(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc), // Convert to UTC when saving
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // Treat as UTC when loading

        // Add more configurations if needed
    }
}
