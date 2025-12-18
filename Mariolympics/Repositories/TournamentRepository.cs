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
            .ThenInclude(p => p.Person)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.Rounds)
            .ThenInclude(r => r.Matches)
            .ThenInclude(m => m.Player2)
            .ThenInclude(p => p.Person)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.BronzeMedalMatch)
            .ThenInclude(m => m.Player1)
            .ThenInclude(p => p.Person)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.BronzeMedalMatch)
            .ThenInclude(m => m.Player2)
            .ThenInclude(p => p.Person)
            .ToListAsync();
    }

    public async Task<Tournament> GetTournamentByIdAsync(int id)
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Tournament
            .Include(t => t.Brackets)
            .ThenInclude(b => b.Rounds)
            .ThenInclude(r => r.Matches)
            .ThenInclude(m => m.Player1)
            .ThenInclude(p => p.Person)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.Rounds)
            .ThenInclude(r => r.Matches)
            .ThenInclude(m => m.Player2)
            .ThenInclude(p => p.Person)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.BronzeMedalMatch)
            .ThenInclude(m => m.Player1)
            .ThenInclude(p => p.Person)
            .Include(t => t.Brackets)
            .ThenInclude(b => b.BronzeMedalMatch)
            .ThenInclude(m => m.Player2)
            .ThenInclude(p => p.Person)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddTournamentAsync(Tournament tournament)
    {
        var context = await dbContextFactory.CreateDbContextAsync();

        // Attach existing Person entities to prevent duplicate insertion
        if (tournament.Brackets != null)
        {
            foreach (var bracket in tournament.Brackets)
            {
                if (bracket.Rounds != null)
                {
                    foreach (var round in bracket.Rounds)
                    {
                        if (round.Matches != null)
                        {
                            foreach (var match in round.Matches)
                            {
                                if (match.Player1?.Person != null)
                                {
                                    context.Attach(match.Player1.Person);
                                }
                                if (match.Player2?.Person != null)
                                {
                                    context.Attach(match.Player2.Person);
                                }
                            }
                        }
                    }
                }

                // Handle bronze medal match
                if (bracket.BronzeMedalMatch != null)
                {
                    if (bracket.BronzeMedalMatch.Player1?.Person != null)
                    {
                        context.Attach(bracket.BronzeMedalMatch.Player1.Person);
                    }
                    if (bracket.BronzeMedalMatch.Player2?.Person != null)
                    {
                        context.Attach(bracket.BronzeMedalMatch.Player2.Person);
                    }
                }
            }
        }

        await context.Tournament.AddAsync(tournament);
        await context.SaveChangesAsync();
    }

    public async Task UpdateTournamentAsync(Tournament tournament)
    {
        var context = await dbContextFactory.CreateDbContextAsync();

        // Attach existing Person entities to prevent duplicate insertion
        if (tournament.Brackets != null)
        {
            foreach (var bracket in tournament.Brackets)
            {
                if (bracket.Rounds != null)
                {
                    foreach (var round in bracket.Rounds)
                    {
                        if (round.Matches != null)
                        {
                            foreach (var match in round.Matches)
                            {
                                if (match.Player1?.Person != null && context.Entry(match.Player1.Person).State == EntityState.Detached)
                                {
                                    context.Attach(match.Player1.Person);
                                }
                                if (match.Player2?.Person != null && context.Entry(match.Player2.Person).State == EntityState.Detached)
                                {
                                    context.Attach(match.Player2.Person);
                                }
                            }
                        }
                    }
                }

                // Handle bronze medal match
                if (bracket.BronzeMedalMatch != null)
                {
                    if (bracket.BronzeMedalMatch.Player1?.Person != null && context.Entry(bracket.BronzeMedalMatch.Player1.Person).State == EntityState.Detached)
                    {
                        context.Attach(bracket.BronzeMedalMatch.Player1.Person);
                    }
                    if (bracket.BronzeMedalMatch.Player2?.Person != null && context.Entry(bracket.BronzeMedalMatch.Player2.Person).State == EntityState.Detached)
                    {
                        context.Attach(bracket.BronzeMedalMatch.Player2.Person);
                    }
                }
            }
        }

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
