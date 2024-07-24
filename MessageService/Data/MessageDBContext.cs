using Microsoft.EntityFrameworkCore;
using MessageService.Entity;

namespace MessageService.Data
{
    public class MessageDBContext:DbContext
    {
        public MessageDBContext(DbContextOptions<MessageDBContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<MessageEntity> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole();
            }));
        }
    }
}
