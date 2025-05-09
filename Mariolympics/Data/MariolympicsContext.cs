using Microsoft.EntityFrameworkCore;

namespace Mariolympics.Data;

public class MariolympicsContext(DbContextOptions<MariolympicsContext> options) : DbContext(options)
{
    public DbSet<Person> Person { get; set; }
    public DbSet<Tournament> Tournament { get; set; }
    public DbSet<Bracket> Bracket { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tournament>(tournament =>
        {
            tournament.HasKey(e => e.Id);
            tournament.Property(e => e.Id).ValueGeneratedOnAdd();
            tournament
                .HasMany(t => t.Brackets)
                .WithOne(b => b.Tournament)
                .HasForeignKey(b => b.TournamentId);
        });

        modelBuilder.Entity<Bracket>(bracket =>
        {
            bracket.HasKey(e => e.Id);
            bracket.Property(e => e.Id).ValueGeneratedOnAdd();
            bracket
                .HasMany(b => b.Rounds)
                .WithOne(r => r.Bracket)
                .HasForeignKey(r => r.BracketId);
        });

        modelBuilder.Entity<Round>(round =>
        {
            round.HasKey(e => e.Id);
            round.Property(e => e.Id).ValueGeneratedOnAdd();
            round
                .HasMany(r => r.Matches)
                .WithOne(m => m.Round)
                .HasForeignKey(m => m.RoundId);
        });

        modelBuilder.Entity<Match>(match =>
        {
            match.HasKey(e => e.Id);
            match.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Person>(person =>
        {
            person.HasKey(e => e.Id);
            person.Property(e => e.Id).ValueGeneratedOnAdd();
        });
    }
}
