using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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


            // Configure the User entity relations
            builder.Entity<User>()
                .HasMany(l => l.Parents)
                .WithMany(l => l.Students);


            // Configure the School entity relations

            builder.Entity<School>()
                .HasOne(l => l.Principal)
                .WithOne(l => l.School)
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
