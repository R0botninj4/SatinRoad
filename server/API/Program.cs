using Infra;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddCors();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.Cookie.Name = "SatinRoad.Auth");
builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<UserService>();

var connectionString = builder.Configuration
                           .GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Database connection string is missing.");

var options = new DataOptions().UseSQLite(connectionString);
var dataOptions = new DataOptions<MyDatabaseConnection>(options);

builder.Services.AddScoped<MyDatabaseConnection>(
    _ => new MyDatabaseConnection(dataOptions));

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<ListingService>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

app.UseCors(config => config
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowCredentials()
    .AllowAnyMethod());


app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();

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
    db.CreateTable<Listing>(
        tableOptions: TableOptions.CreateIfNotExists);
    db.CreateTable<Order>(
        tableOptions: TableOptions.CreateIfNotExists);

    var items = new List<Item>
    {
        new() { Id = 1, Name = "Blaze rod" },
        new() { Id = 2, Name = "Wheat" },
        new() { Id = 3, Name = "Sugar" },
        new() { Id = 4, Name = "Sugar cane" },
        new() { Id = 5, Name = "TNT" },
        new() { Id = 6, Name = "Seeds" },
        new() { Id = 7, Name = "Iron sword" },
        new() { Id = 8, Name = "Gold helmet" },
        new() { Id = 9, Name = "Stick" }
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
