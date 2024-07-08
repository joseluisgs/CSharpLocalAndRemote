using Microsoft.EntityFrameworkCore;

namespace CSharpLocalAndRemote.Database;

public static class DbContextExtensions
{
    // Si quiero que el id por mucho que elimine empiece por 1, npo es obligatori
    public static async Task RemoveAllAsync(this DbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("DELETE FROM TenistaEntity");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name = 'TenistaEntity'");
    }
}