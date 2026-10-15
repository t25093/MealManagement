using AutoMapper;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using MealManagement.Core.Settings;
using MealManagement.Data;
using MealManagement.Data.Repositories;
using MealManagement.Service.Mapping;
using MealManagement.Service.Security;
using MealManagement.Service.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// הגדרות עם ולידציה בעלייה
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<PaginationSettings>()
    .Bind(builder.Configuration.GetSection(PaginationSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// ה-connection string נבדק כבר עכשיו, וישמש ב-DbContext בשלב 4
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Connection string 'Default' is missing. Add it to User Secrets.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IDishService, DishService>();

builder.Services.AddScoped<IMealService, MealService>();

builder.Services.AddScoped<IMealDishService, MealDishService>();

builder.Services.AddScoped<IUserDishAssignmentService, UserDishAssignmentService>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var mapper = app.Services.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

// בפיתוח בלבד: יצירה ועדכון של בסיס הנתונים, כולל Seed, בעליית האפליקציה
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
