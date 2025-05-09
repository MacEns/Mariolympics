using Microsoft.EntityFrameworkCore;

namespace Mariolympics.Repositories;

public class PersonRepository(IDbContextFactory<MariolympicsContext> dbContextFactory)
{
    public async Task<List<Person>> GetPersonsAsync()
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Person.ToListAsync();
    }

    public async Task AddPersonAsync(Person person)
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        await context.Person.AddAsync(person);
        await context.SaveChangesAsync();
    }
}
