using Microsoft.EntityFrameworkCore;
using ClientService.Entity;

namespace ClientService.Data
{
    public class ClientDbContext:DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<ClientEntity> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole();
            }));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.userName).IsRequired();
                entity.Property(e => e.password).IsRequired();
                entity.Property(e => e.email).IsRequired();
                entity.Property(e=> e.friends).IsRequired();
                entity.Property(e=>e.chats).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
