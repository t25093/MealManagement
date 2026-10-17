using AutoMapper;
using MealManagement.API.Middleware;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using MealManagement.Core.Settings;
using MealManagement.Data;
using MealManagement.Data.Repositories;
using MealManagement.Service.Mapping;
using MealManagement.Service.Security;
using MealManagement.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===== Logging: NLog =====
builder.Logging.ClearProviders();
builder.Host.UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false });

// ===== Options עם ולידציה בעלייה =====
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<PaginationSettings>()
    .Bind(builder.Configuration.GetSection(PaginationSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// ===== Data =====
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing. Add it to User Secrets.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<IMealRepository, MealRepository>();
builder.Services.AddScoped<IMealDishRepository, MealDishRepository>();
builder.Services.AddScoped<IUserDishAssignmentRepository, UserDishAssignmentRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();

// ===== Service =====
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IMealDishService, MealDishService>();
builder.Services.AddScoped<IUserDishAssignmentService, UserDishAssignmentService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// ===== Authentication: JWT =====
var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
          ?? throw new InvalidOperationException("Missing 'Jwt' configuration section");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),

            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = "role"
        };
    });

// ===== Authorization =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ===== Controllers =====
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MealManagement API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "יש להדביק את ה-token בלבד, בלי המילה Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ===== CORS =====
var clientUrl = builder.Configuration["Cors:ClientUrl"]
                ?? throw new InvalidOperationException("Missing 'Cors:ClientUrl' configuration");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy => policy
        .WithOrigins(clientUrl)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("X-Correlation-ID", "Location"));
});

var app = builder.Build();

// בדיקת AutoMapper בעלייה
app.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

// Migrate אוטומטי ב-Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// ===== Pipeline: הסדר קובע =====
 app.UseMiddleware<ExceptionHandlingMiddleware>();
 app.UseMiddleware<CorrelationIdMiddleware>();  

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
