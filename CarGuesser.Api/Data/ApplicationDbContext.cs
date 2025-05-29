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
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<CarQuestionAnswer> CarQuestionAnswers => Set<CarQuestionAnswer>();

        // метод для настройки моделей и связи между ними
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarQuestionAnswer>().HasKey(cq => new { cq.CarId, cq.QuestionId });
            base.OnModelCreating(modelBuilder);
        }
    }
}
