using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GradeCenter.Data
{
    public class GradeCenterContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public GradeCenterContext() { }

        public GradeCenterContext(DbContextOptions<GradeCenterContext> options) : base(options)
        {
        }

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<School>? Schools { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the primary key for IdentityUserLogin<string>
            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // Configure a one-to-many relationship
            // between Users and School.
            builder.Entity<User>()
                .HasOne(l => l.School)
                .WithMany(l => l.Users)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=GradeCenter;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True");
        }
    }
}
