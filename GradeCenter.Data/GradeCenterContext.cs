using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GradeCenter.Data
{
    public class GradeCenterContext : IdentityDbContext<AspNetUser, IdentityRole<Guid>, Guid>
    {
        public GradeCenterContext() { }

        public GradeCenterContext(DbContextOptions<GradeCenterContext> options) : base(options)
        {
        }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<School>? Schools { get; set; }
        public virtual DbSet<SchoolClass> SchoolClasses { get; set; }
        public virtual DbSet<Discipline> Disciplines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the primary key for IdentityUserLogin<string>
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // Configure a one-to-many relationship
            // between Users and School.
            modelBuilder.Entity<AspNetUser>()
                .HasOne(l => l.School)
                .WithMany(l => l.People)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRelation>()
                .HasKey(ur => new { ur.ParentId, ur.ChildId });

            modelBuilder.Entity<UserRelation>()
                .HasOne(ur => ur.Parent)
                .WithMany(u => u.ChildrenRelations)
                .HasForeignKey(ur => ur.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRelation>()
                .HasOne(ur => ur.Child)
                .WithMany(u => u.ParentRelations)
                .HasForeignKey(ur => ur.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SchoolClass>()
                .HasMany(s => s.Students)
                .WithOne(c => c.SchoolClass)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SchoolClass>()
                .HasMany(s => s.Curriculum)
                .WithOne(c => c.SchoolClass)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=GradeCenter;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True");
        }
    }
}
