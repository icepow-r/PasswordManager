using Microsoft.EntityFrameworkCore;
using PasswordManager.Model;

namespace PasswordManager.Context
{
    public partial class PasswordsContext : DbContext
    {
        private readonly string _connectionString;
        public PasswordsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<Card> Cards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(Manager.GetConnectionString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>(entity =>
            {
                entity.ToTable("Card");

                entity.HasIndex(e => e.Id, "IX_Card_id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Login).HasColumnName("login");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Title).HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
