<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ReportEvidence> ReportEvidences => Set<ReportEvidence>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ReportCategories> ReportCategories => Set<ReportCategories>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            builder.Entity<ReportCategories>()
            .HasOne(rc => rc.Report)
            .WithMany(r => r.ReportCategories)
            .HasForeignKey(rc => rc.ReportId);

            builder.Entity<ReportCategories>()
                .HasOne(rc => rc.Category)
                .WithMany()
                .HasForeignKey(rc => rc.CategoryId);


            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "reputational" },
                new Category { Id = 2, Name = "facilities" },
                new Category { Id = 3, Name = "security" },
                new Category { Id = 4, Name = "HR" },
                new Category { Id = 5, Name = "safety" },
                new Category { Id = 6, Name = "compliance" },
                new Category { Id = 7, Name = "other" }
            );
        }
    }
=======
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, 
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, 
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ReportEvidence> ReportEvidences => Set<ReportEvidence>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }
    }
>>>>>>> 681d14d (missed files)
}