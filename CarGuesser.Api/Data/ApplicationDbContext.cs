using Microsoft.EntityFrameworkCore;
using CarGuesser.Api.Models;

namespace CarGuesser.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<GameSession> GameSessions => Set<GameSession>();
        public DbSet<SessionAnswer> SessionAnswers => Set<SessionAnswer>();
    }
}
