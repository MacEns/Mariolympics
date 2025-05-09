using Microsoft.EntityFrameworkCore;

namespace Mariolympics.Data;

public class MariolympicsContext(DbContextOptions<MariolympicsContext> options) : DbContext(options)
{
    public DbSet<Person> Person { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(person =>
        {
            person.HasKey(e => e.Id);
            person.Property(e => e.Id).ValueGeneratedOnAdd();
        });
    }
}
