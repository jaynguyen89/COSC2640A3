using COSC2640A3.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace COSC2640A3.DbContexts
{
    public partial class MainDbContext : DbContext
    {
        public MainDbContext()
        {
        }

        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountRole> AccountRoles { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Id, "UQ__Account__3214EC067682C7CC")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NormalizedUsername)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.PreferredName).HasMaxLength(100);

                entity.Property(e => e.RecoveryToken).HasMaxLength(100);

                entity.Property(e => e.TwoFaSecretKey).HasMaxLength(20);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AccountRole>(entity =>
            {
                entity.ToTable("AccountRole");

                entity.HasIndex(e => e.Id, "UQ__AccountR__3214EC0656316ECA")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountRoles)
                    .HasForeignKey(d => d.AccountId);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.HasIndex(e => e.Id, "UQ__Student__3214EC06751F50E0")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Faculty).HasMaxLength(50);

                entity.Property(e => e.PersonalUrl).HasMaxLength(100);

                entity.Property(e => e.SchoolName).HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.AccountId);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.HasIndex(e => e.Id, "UQ__Teacher__3214EC06F70D544D")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Company).HasMaxLength(50);

                entity.Property(e => e.JobTitle).HasMaxLength(20);

                entity.Property(e => e.PersonalWebsite).HasMaxLength(100);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.AccountId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
