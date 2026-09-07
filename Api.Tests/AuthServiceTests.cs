namespace Api.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api.Data;
using Api.Models;
using Api.Services;
using Api.DTOs;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;

public class AuthServiceTests
{
    private async Task<(AuthService service, ApplicationDbContext context, Mock<IJwtService> jwtMock)> CreateService()
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

        // Seed roles
        var roleManager = provider.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var r in new[] { "Admin", "User", "Moderator" })
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = r });
        }

        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleMgr = provider.GetRequiredService<RoleManager<ApplicationRole>>();
        var jwtMock = new Mock<IJwtService>();
        jwtMock.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>())).Returns("jwt-token");
        jwtMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");
        jwtMock.Setup(j => j.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "test-id") })
        ));
        var logger = new Mock<ILogger<AuthService>>();

        var service = new AuthService(userManager, roleMgr, jwtMock.Object, TimeProvider.System, logger.Object);
        return (service, context, jwtMock);
    }

    [Fact]
    public async Task SignUp_ShouldCreateUser_AndReturnToken()
    {
        var (service, _, jwtMock) = await CreateService();
        var dto = new SignUpDto { Email = "new@test.com", Password = "Password123!", FirstName = "New", LastName = "User" };

        var result = await service.SignUpAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Token.Should().Be("jwt-token");
        result.User.Should().NotBeNull();
        jwtMock.Verify(j => j.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()), Times.Once);
    }

    [Fact]
    public async Task SignUp_ShouldFail_ForDuplicateEmail()
    {
        var (service, _, _) = await CreateService();
        var dto = new SignUpDto { Email = "dup@test.com", Password = "Password123!", FirstName = "Dup", LastName = "User" };
        await service.SignUpAsync(dto);
        var result2 = await service.SignUpAsync(dto);
        result2.Succeeded.Should().BeFalse();
        result2.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterUser_ShouldFail_WhenRoleNotExist()
    {
        var (service, _, _) = await CreateService();
        var dto = new RegisterUserDto { Email = "admin2@test.com", Password = "Password123!", FirstName = "A", LastName = "B", Role = "NonExist" };
        var result = await service.RegisterUserAsync(dto);
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Contain("does not exist");
    }

    [Fact]
    public async Task SignIn_ShouldFail_ForInvalidPassword()
    {
        var (service, _, _) = await CreateService();
        var signUp = new SignUpDto { Email = "user@test.com", Password = "Password123!", FirstName = "U", LastName = "V" };
        await service.SignUpAsync(signUp);

        var result = await service.SignInAsync(new SignInDto { Email = "user@test.com", Password = "Wrong!", AllowUnconfirmedEmail = true });
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }

    [Fact]
    public async Task SignIn_ShouldSucceed_AfterSignUp()
    {
        var (service, _, _) = await CreateService();
        var dto = new SignUpDto { Email = "login@test.com", Password = "Password123!", FirstName = "L", LastName = "I" };
        await service.SignUpAsync(dto);
        var result = await service.SignInAsync(new SignInDto { Email = "login@test.com", Password = "Password123!", AllowUnconfirmedEmail = true });
        result.Succeeded.Should().BeTrue();
        result.Token.Should().NotBeNull();
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task SignIn_ShouldFail_WhenDeactivated()
    {
        var (service, context, _) = await CreateService();
        var dto = new SignUpDto { Email = "deact@test.com", Password = "Password123!", FirstName = "D", LastName = "E" };
        await service.SignUpAsync(dto);
        var user = await context.Users.FirstAsync(u => u.Email == "deact@test.com");
        user.IsActive = false;
        await context.SaveChangesAsync();

        var result = await service.SignInAsync(new SignInDto { Email = "deact@test.com", Password = "Password123!", AllowUnconfirmedEmail = true });
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Contain("deactivated");
    }

    [Fact]
    public async Task Logout_ShouldClearRefreshToken()
    {
        var (service, context, _) = await CreateService();
        var dto = new SignUpDto { Email = "logout@test.com", Password = "Password123!", FirstName = "L", LastName = "O" };
        await service.SignUpAsync(dto);
        await service.SignInAsync(new SignInDto { Email = "logout@test.com", Password = "Password123!", AllowUnconfirmedEmail = true });
        var user = await context.Users.FirstAsync(u => u.Email == "logout@test.com");
        user.RefreshToken.Should().NotBeNull();

        await service.LogoutAsync(user.Id);
        var after = await context.Users.FindAsync(user.Id);
        after!.RefreshToken.Should().BeNull();
    }
}
