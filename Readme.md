**Dotnet Core 8 - Authentication/Authorization Template**

In order to get very easily authentication & authorization in a .Net Core 8 project you have to follow a few steps as follows:

1. **Add swagger authentication**
```c#
builder.Services.AddSwaggerGen(c =>
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
```

2. **Add authentication & authorization support**
```c#
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();
```

3. **Add support for your favorite database we'll use for the purpose of this template Sqlite (also define the connection string in appsettings.json)**
```c#
```c#
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("name=sqLite"));
```

4. **Add support for the identity controller**
```c#
app.MapGroup("/identity").MapIdentityApi<IdentityUser>();
```

5. **Configure the repository (notice we are inheriting from _IdentityDbContext_)**
```c#
public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
```

6. Install dotnet ef tool latest version (8.0.0)
```c#
dotnet ef migrations add InitialCreate
dotnet ef database update
    
dotnet build
dotnet run
```
