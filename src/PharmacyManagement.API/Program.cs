using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql;
using PharmacyManagement.Infrastructure.Data;
using PharmacyManagement.Infrastructure.Repositories;
using PharmacyManagement.Application.Services;
using PharmacyManagement.API.Data;
using PharmacyManagement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Pharmacy Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "VotreCleSecreteTresLongueEtSecuriseePourJWT123456789";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PharmacyManagement";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PharmacyManagement";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrateur"));
    options.AddPolicy("PharmacienOrAdmin", policy => policy.RequireRole("Administrateur", "Pharmacien"));
    options.AddPolicy("CaissierOrAbove", policy => policy.RequireRole("Administrateur", "Pharmacien", "Caissier"));
    options.AddPolicy("CaissierOnly", policy => policy.RequireRole("Caissier"));
    options.AddPolicy("ReadOnly", policy => policy.RequireRole("Administrateur", "Pharmacien", "Caissier")); // Pour lecture seule
});

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Port=3306;Database=PharmacyManagementDb;User=root;Password=;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IMedicamentService, MedicamentService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IVenteService, VenteService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddScoped<IFournisseurService, FournisseurService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICaissierService, CaissierService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Gestion globale des erreurs
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Activer Swagger en développement et en production pour faciliter les tests
app.UseSwagger();
app.UseSwaggerUI();

// Désactiver HTTPS redirection en développement pour éviter les problèmes de connexion
// Note: En production, HTTPS devrait être activé
// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// IMPORTANT: Mapper les contrôleurs API EN PREMIER
// Cela garantit que les routes /api/* sont enregistrées avant tout
app.MapControllers();

// Servir les fichiers statiques (interface web) - après les routes API
app.UseStaticFiles();

// Route par défaut pour l'interface web - en dernier
// MapFallbackToFile ne devrait pas intercepter les routes /api/* car elles sont déjà mappées
app.MapFallbackToFile("index.html");

// Initialiser les données de test
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        SeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données.");
    }
}

app.Run();

