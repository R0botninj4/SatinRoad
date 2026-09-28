using Infra;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Threading.RateLimiting;
using NSwag;
using NSwag.Generation.Processors.Security;
using Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(config =>
{
    config.AddSecurity("Bearer", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Paste the accessToken returned by POST /api/auth/login."
    });
    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});
builder.Services.AddCors();
builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken(options => options.BearerTokenExpiration = TimeSpan.FromHours(1));
builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<UserService>();

var protection = builder.Services.AddDataProtection().SetApplicationName("SatinRoad");
var keyPath = builder.Configuration["DataProtection:KeyPath"];
if (!string.IsNullOrWhiteSpace(keyPath))
    protection.PersistKeysToFileSystem(new DirectoryInfo(keyPath));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

var connectionString = builder.Configuration
                           .GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Database connection string is missing.");

var options = new DataOptions().UseSQLite(connectionString);
var dataOptions = new DataOptions<MyDatabaseConnection>(options);

builder.Services.AddScoped<MyDatabaseConnection>(
    _ => new MyDatabaseConnection(dataOptions));

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ItemService>();

var app = builder.Build();

app.UseCors(config => config
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<MyDatabaseConnection>();

    db.CreateTable<Category>(
        tableOptions: TableOptions.CreateIfNotExists);

    db.CreateTable<User>(tableOptions: TableOptions.CreateIfNotExists);
    db.Execute("CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_NormalizedUsername ON Users (NormalizedUsername)");

    db.CreateTable<Item>(
        tableOptions: TableOptions.CreateIfNotExists);

    var items = new List<Item>
    {
        new() { Id = 1, Name = "Blaze rod" },
        new() { Id = 2, Name = "Wheat" },
        new() { Id = 3, Name = "Sugar" },
        new() { Id = 4, Name = "Sugar cane" },
        new() { Id = 5, Name = "TNT" },
        new() { Id = 6, Name = "Seeds" }
    };

    foreach (var item in items)
    {
        if (!db.Items.Any(existingItem => existingItem.Id == item.Id))
        {
            db.Insert(item);
        }
    }
}

app.Run();
