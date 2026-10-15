using MealManagement.Core.Settings;
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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
