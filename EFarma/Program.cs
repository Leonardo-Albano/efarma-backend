using EFarma.Config;
using EFarma.Data;
using EFarma.Repositories;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
DependencyInjection.AddBusiness(builder);

// Configuração do Swagger e inclusão de documentação XML
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "EFarma", Version = "v1" });

    // Configuração para incluir o arquivo XML de documentação nos comentários da API
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath); // Certifique-se de que essa linha está aqui no SwaggerGen e não na UI
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddAutoMapper(typeof(Program));

// Configuração do banco de dados
builder.Services.AddDbContext<DataContext>(options =>
{
    //options.UseMySQL(builder.Configuration.GetConnectionString("LocalConnection"));
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "EFarma V1");
        // A configuração IncludeXmlComments não deve estar aqui
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
