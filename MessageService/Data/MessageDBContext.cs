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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Text).IsRequired(false);
                entity.Property(e => e.Author).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Photo).IsRequired(false);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
