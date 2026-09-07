namespace Api.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Api.Data;
using Api.Services;
using Api.Options;
using FluentAssertions;
using Moq;

public class QualityAndSecurityTests
{
    private IFormFile CreateMockFile(string fileName, long length = 100)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.Length).Returns(length);
        mock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        return mock.Object;
    }

    [Theory]
    [InlineData("evil.exe")]
    [InlineData("script.sh")]
    [InlineData("noextension")]
    public async Task FileStorage_ShouldReject_DisallowedExtension(string fileName)
    {
        var storage = new FileStorageService();
        var file = CreateMockFile(fileName);
        Func<Task> act = async () => await storage.SaveFileAsync(file, "evidence");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*not allowed*");
    }

    [Fact]
    public async Task FileStorage_ShouldReject_Oversize()
    {
        var storage = new FileStorageService();
        var file = CreateMockFile("big.pdf", length: 11 * 1024 * 1024);
        Func<Task> act = async () => await storage.SaveFileAsync(file, "evidence");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*exceeds*");
    }

    [Theory]
    [InlineData("../evidence")]
    [InlineData("a/b")]
    [InlineData("a\\b")]
    public async Task FileStorage_ShouldReject_TraversalSubFolder(string subFolder)
    {
        var storage = new FileStorageService();
        var file = CreateMockFile("doc.pdf");
        Func<Task> act = async () => await storage.SaveFileAsync(file, subFolder);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*subFolder*");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567890123456789012345678901")] // 31 chars
    public void JwtService_ShouldThrow_ForShortKey(string shortKey)
    {
        var opts = Options.Create(new JwtOptions { Key = shortKey, Issuer = "iss", Audience = "aud", ExpireMinutes = 60 });
        var svc = new JwtService(opts, TimeProvider.System);
        var user = new Api.Models.ApplicationUser { Id = "u1", UserName = "test@test.com", Email = "test@test.com" };
        Action act = () => svc.GenerateToken(user, new List<string>());
        act.Should().Throw<InvalidOperationException>().WithMessage("*32 bytes*");
    }

    [Fact]
    public void JwtService_ShouldAccept_ValidKey()
    {
        var opts = Options.Create(new JwtOptions { Key = new string('a', 32), Issuer = "iss", Audience = "aud", ExpireMinutes = 60 });
        var svc = new JwtService(opts, TimeProvider.System);
        var user = new Api.Models.ApplicationUser { Id = "u1", UserName = "test@test.com", Email = "test@test.com" };
        var token = svc.GenerateToken(user, new List<string> { "User" });
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AuthResult_ShouldBe_Typed()
    {
        var dto = new Api.DTOs.AuthUserDto { Id = "id1", Email = "e@test.com", FirstName = "F", LastName = "L", Roles = new[] { "User" } };
        var result = new Api.Services.Interfaces.AuthResult(true, User: dto);
        result.User.Should().BeOfType<Api.DTOs.AuthUserDto>();
        result.User!.Email.Should().Be("e@test.com");
    }

    [Fact]
    public async Task UserService_GetAll_ShouldNotLeak_PasswordHash()
    {
        // Ensure UserService returns DTO without PasswordHash
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddLogging();
        services.AddDbContext<Api.Data.ApplicationDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentity<Api.Models.ApplicationUser, Api.Models.ApplicationRole>(o =>
        {
            o.Password.RequireDigit = false; o.Password.RequireLowercase = false; o.Password.RequireUppercase = false; o.Password.RequireNonAlphanumeric = false; o.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<Api.Data.ApplicationDbContext>();
        var provider = services.BuildServiceProvider();
        var userManager = provider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Api.Models.ApplicationUser>>();
        var user = new Api.Models.ApplicationUser { UserName = "t@test.com", Email = "t@test.com", FirstName = "T", LastName = "U" };
        await userManager.CreateAsync(user, "Password123!");
        var userService = new UserService(userManager);
        var all = await userService.GetAllUsersAsync();
        var first = all.First();
        first.Should().BeOfType<Api.DTOs.AuthUserDto>();
        // DTO should not have PasswordHash property
        typeof(Api.DTOs.AuthUserDto).GetProperty("PasswordHash").Should().BeNull();
    }
}
