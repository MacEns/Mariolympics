using Microsoft.EntityFrameworkCore;

namespace Mariolympics.Repositories;

public class TournamentRepository(IDbContextFactory<MariolympicsContext> dbContextFactory)
{
    public async Task<List<Tournament>> GetTournamentsAsync()
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Tournament
            .Include(t => t.Brackets)
            .ThenInclude(b => b.Rounds)
            .ThenInclude(r => r.Matches)
            .ThenInclude(m => m.Player1)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.Rounds)
            .ThenInclude(r => r.Matches)
            .ThenInclude(m => m.Player2)
            .ToListAsync();
    }

    public async Task AddTournamentAsync(Tournament tournament)
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        await context.Tournament.AddAsync(tournament);
        await context.SaveChangesAsync();
    }

    public async Task UpdateTournamentAsync(Tournament tournament)
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        context.Tournament.Update(tournament);
        await context.SaveChangesAsync();
    }

    public async Task RemoveTournamentAsync(Tournament tournament)
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        context.Tournament.Remove(tournament);
        await context.SaveChangesAsync();
    }
}
