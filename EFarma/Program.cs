using EFarma.Config;
using EFarma.Data;
using EFarma.Repositories;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddHttpClient();
DependencyInjection.AddBusiness(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "EFarma", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://157.230.224.194:3000", "http://157.230.224.194:", "http://localhost:5174")
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddAutoMapper(typeof(Program));

// Configure Entity Framework DbContext with logging settings
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore())
           .EnableSensitiveDataLogging(false)
           .LogTo(Console.WriteLine, LogLevel.Warning); // Suppresses logs below Warning level
});

// Configure logging to suppress EF Core command logs but allow application logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // Default log level
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning); // Suppresses EF Core command logs unless they are warnings or errors
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Information); // Allows general EF logs at Information level

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "EFarma V1");
});

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
