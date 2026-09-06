namespace Api.Tests;

using Microsoft.Extensions.Options;
using Api.Services;
using Api.Options;
using Api.Models;
using Api.Data;
using Api.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

public class DIPTests
{
    [Fact]
    public void JwtService_ShouldDependOn_IOptions_Not_IConfiguration()
    {
        var ctor = typeof(JwtService).GetConstructors().First();
        var paramType = ctor.GetParameters().First().ParameterType;
        paramType.Should().Be(typeof(IOptions<JwtOptions>));
    }

    [Fact]
    public void ReportRepository_ShouldDependOn_IApplicationDbContext()
    {
        var ctor = typeof(ReportRepository).GetConstructors().First();
        var paramType = ctor.GetParameters().First().ParameterType.Name;
        paramType.Should().Be("IApplicationDbContext");
    }

    [Fact]
    public void UserRepository_ShouldDependOn_IApplicationDbContext()
    {
        var ctor = typeof(Api.Data.Repository.UserRepository).GetConstructors().First();
        var paramType = ctor.GetParameters().First().ParameterType.Name;
        paramType.Should().Be("IApplicationDbContext");
    }

    [Fact]
    public void JwtService_GenerateToken_ShouldWork_WithOptions()
    {
        var options = Options.Create(new JwtOptions
        {
            Key = "28e9c18d4fe19bf314a73f1111b077cf71efcf1d496ced57c1d6104eccb22328",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpireMinutes = 60
        });
        var jwtService = new JwtService(options);
        var user = new ApplicationUser { Id = "u1", UserName = "test@test.com", Email = "test@test.com", FirstName = "Test", LastName = "User" };
        var token = jwtService.GenerateToken(user, new List<string> { "User" });
        token.Should().NotBeNullOrEmpty();
        var principal = jwtService.GetPrincipalFromExpiredToken(token);
        principal.Should().NotBeNull();
    }

    [Fact]
    public async Task IApplicationDbContext_ShouldResolve_Via_DI()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<Api.Data.Interfaces.IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<Api.Data.Interfaces.IApplicationDbContext>();
        ctx.Should().NotBeNull();
        ctx.Should().BeAssignableTo<ApplicationDbContext>();
    }
}
