using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace JWTAuthToken.DataAccessLayer.EFModels
{
    public partial class JWTContext : DbContext
    {
        public JWTContext()
        {
        }

        public JWTContext(DbContextOptions<JWTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:SqlServerDbCon");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<UserRefreshToken>(entity =>
            {
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RefreshToken).IsRequired();

                entity.Property(e => e.UserName).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRefreshToken)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRefreshToken_UserRefreshToken");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Cognome)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
