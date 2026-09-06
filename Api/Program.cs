using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Api.Data.Interfaces;
using Api.Data.Repository;
using Api.Data;
using Api.Models;
using Api.Services;
using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
builder.Services.Configure<Api.Options.JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Moderator", "Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Moderator", "Admin"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<Api.Services.Interfaces.IAuthService, Api.Services.AuthService>();
builder.Services.AddScoped<Api.Services.Interfaces.IUserService, Api.Services.UserService>();
builder.Services.AddScoped<Api.Services.Interfaces.IDbSeeder, Api.Services.DbSeeder>();
builder.Services.AddScoped<Api.Services.Interfaces.ICurrentUserService, Api.Services.CurrentUserService>();
builder.Services.AddScoped<Api.Services.Interfaces.ICategorySuggestionService, Api.Services.CategorySuggestionService>();
builder.Services.AddScoped<Api.Services.Interfaces.IFileStorageService, Api.Services.FileStorageService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<Api.Mappers.IReportMapper, Api.Mappers.ReportMapperImpl>();
builder.Services.AddScoped<Api.Services.Interfaces.IReportService, Api.Services.ReportService>();
builder.Services.AddScoped<Api.Services.Interfaces.IReportEvidenceService, Api.Services.ReportEvidenceService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<IReportRepository>(sp => sp.GetRequiredService<ReportRepository>());
builder.Services.AddScoped<IReportReadRepository>(sp => sp.GetRequiredService<ReportRepository>());
builder.Services.AddScoped<IReportWriteRepository>(sp => sp.GetRequiredService<ReportRepository>());
builder.Services.AddScoped<ReportEvidenceRepository>();
builder.Services.AddScoped<IReportEvidenceRepository>(sp => sp.GetRequiredService<ReportEvidenceRepository>());
builder.Services.AddScoped<IReportEvidenceReadRepository>(sp => sp.GetRequiredService<ReportEvidenceRepository>());
builder.Services.AddScoped<IReportEvidenceWriteRepository>(sp => sp.GetRequiredService<ReportEvidenceRepository>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
// Segregated service interfaces (ISP) - same implementation, forward via factory to share instance per scope
builder.Services.AddScoped<Api.Services.Interfaces.IReportQueryService>(sp => sp.GetRequiredService<Api.Services.Interfaces.IReportService>());
builder.Services.AddScoped<Api.Services.Interfaces.IReportCommandService>(sp => sp.GetRequiredService<Api.Services.Interfaces.IReportService>());
builder.Services.AddScoped<Api.Services.Interfaces.IReportWorkflowService>(sp => sp.GetRequiredService<Api.Services.Interfaces.IReportService>());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Api.Services.Interfaces.IDbSeeder>();
    await seeder.SeedAsync();
}

app.Run();
