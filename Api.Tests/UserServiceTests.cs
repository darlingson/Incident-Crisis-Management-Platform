namespace Api.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api.Data;
using Api.Models;
using Api.Services;
using FluentAssertions;

public class UserServiceTests
{
    private async Task<(UserService service, ApplicationDbContext context, UserManager<ApplicationUser> userManager)> CreateService()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddLogging();
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var service = new UserService(userManager);
        return (service, context, userManager);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers()
    {
        var (service, _, userManager) = await CreateService();
        var user = new ApplicationUser { UserName = "a@test.com", Email = "a@test.com", FirstName = "A", LastName = "B" };
        await userManager.CreateAsync(user, "Password123!");

        var users = await service.GetAllUsersAsync();
        users.Should().Contain(u => u.Email == "a@test.com");
    }

    [Fact]
    public async Task GetProfile_ShouldReturnNotFound_ForInvalidId()
    {
        var (service, _, _) = await CreateService();
        var result = await service.GetProfileAsync("invalid");
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task GetProfile_ShouldReturnData_ForValidUser()
    {
        var (service, _, userManager) = await CreateService();
        var user = new ApplicationUser { UserName = "b@test.com", Email = "b@test.com", FirstName = "B", LastName = "C" };
        await userManager.CreateAsync(user, "Password123!");
        var created = await userManager.FindByEmailAsync("b@test.com");

        var result = await service.GetProfileAsync(created!.Id);
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task DeactivateUser_ShouldSetIsActiveFalse()
    {
        var (service, context, userManager) = await CreateService();
        var user = new ApplicationUser { UserName = "c@test.com", Email = "c@test.com", FirstName = "C", LastName = "D", IsActive = true };
        await userManager.CreateAsync(user, "Password123!");
        var created = await userManager.FindByEmailAsync("c@test.com");

        var result = await service.DeactivateUserAsync(created!.Id);
        result.Succeeded.Should().BeTrue();
        var reloaded = await context.Users.FindAsync(created.Id);
        reloaded!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateUser_ShouldFail_ForNotFound()
    {
        var (service, _, _) = await CreateService();
        var result = await service.DeactivateUserAsync("not-exist");
        result.Succeeded.Should().BeFalse();
    }
}

public class DbSeederTests
{
    private async Task<(DbSeeder seeder, ApplicationDbContext context, IServiceProvider provider)> CreateSeeder()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddLogging();
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
        var roleManager = provider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var seeder = new DbSeeder(roleManager, userManager);
        return (seeder, context, provider);
    }

    [Fact]
    public async Task SeedAsync_ShouldCreateRolesAndAdmin()
    {
        var (seeder, context, _) = await CreateSeeder();
        await seeder.SeedAsync();

        var roles = await context.Roles.ToListAsync();
        roles.Should().Contain(r => r.Name == "Admin");
        roles.Should().Contain(r => r.Name == "User");
        roles.Should().Contain(r => r.Name == "Moderator");

        var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@demoemail.com");
        admin.Should().NotBeNull();
        admin!.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task SeedAsync_ShouldBeIdempotent()
    {
        var (seeder, context, _) = await CreateSeeder();
        await seeder.SeedAsync();
        await seeder.SeedAsync();
        var count = await context.Roles.CountAsync();
        count.Should().Be(3);
    }
}
