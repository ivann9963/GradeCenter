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
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }

        public virtual DbSet<Statistic> Statistics { get; set; }

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

            // Configure a one-to-many relationship
            // between Attendances and AspNetUsers.
            modelBuilder.Entity<AspNetUser>()
                .HasMany(l => l.Attendances)
                .WithOne(l => l.Student)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure a one-to-many relationship
            // between Attendances and Disciplines.
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Discipline)
                .WithMany(a => a.Attendances)
                .OnDelete(DeleteBehavior.Restrict);

            // between Grades and Users.
            modelBuilder.Entity<AspNetUser>()
                .HasMany(l => l.Grades)
                .WithOne(l => l.Student)
                .OnDelete(DeleteBehavior.Restrict);

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

            // Configure a one-to-one relationship
            // between Schools and School Classes.
            modelBuilder.Entity<School>()
                .HasMany(s => s.SchoolClasses)
                .WithOne(s => s.School)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure a one-to-many relationship
            // between Grades and Disciplines
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Discipline)
                .WithMany(g => g.Grades)
                .OnDelete(DeleteBehavior.Restrict);

          

            modelBuilder.Entity<Statistic>()
                .HasOne(s => s.School)
                .WithMany(st => st.Statistics)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Statistic>()
                .HasOne(sc => sc.SchoolClass)
                .WithMany(st => st.Statistics)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Statistic>()
               .HasOne(t => t.Teacher)
               .WithMany(st => st.Statistics)
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
