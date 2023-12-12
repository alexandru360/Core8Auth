using AuthAndLogin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var bud = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
bud.Services.AddEndpointsApiExplorer();

// Step 1 --------------------------- Add swagger authentication ---------------------------
bud.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core8Auth", Version = "v1" });

    // Add a Bearer token input box in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
// Step 1 - End ---------------------- Add swagger authentication ---------------------------

// Step 2 --------------------------- Add authentication & authorization support ---------------------------
bud.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();

// bud.Services.Configure<IdentityOptions>(o =>
// {
//     // Password settings.
//     o.Password.RequireDigit = true;
//     o.Password.RequireLowercase = true;
//     o.Password.RequireNonAlphanumeric = true;
//     o.Password.RequireUppercase = true;
//     o.Password.RequiredLength = 6;
//     o.Password.RequiredUniqueChars = 1;
//
//     // Lockout settings.
//     o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//     o.Lockout.MaxFailedAccessAttempts = 5;
//     o.Lockout.AllowedForNewUsers = true;
//
//     // User settings.
//     o.User.AllowedUserNameCharacters =
//         "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//     o.User.RequireUniqueEmail = false;
// });
// Step 2 - End --------------------------- Add authentication & authorization support ---------------------------

// Step 3 --------------------------- Database ---------------------------
// SqlServer
// builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer("name=DefaultConn"));

// SqLite
bud.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("name=sqLite"));
// Step 3 - End --------------------------- Database ---------------------------

bud.Services.AddAuthorization();

var app = bud.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Step 4 --------------------------- Controller ---------------------------
app.MapGroup("/identity").MapIdentityApi<IdentityUser>();
// Step 4 - End --------------------------- Controller ---------------------------

app.MapGet("/weather-forecast-require-auth", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .RequireAuthorization()
    .WithName("GetWeatherForecast")
    .WithOpenApi().RequireAuthorization();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
