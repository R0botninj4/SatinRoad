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
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

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

    // Upgrade databases created before orders stored their product.
    var hasItemId = db.Execute<int>(
        "SELECT COUNT(*) FROM pragma_table_info('Order') WHERE name = 'ItemId'");
    if (hasItemId == 0)
    {
        db.Execute("ALTER TABLE [Order] ADD COLUMN ItemId INTEGER NOT NULL DEFAULT 0");
    }

    // Preserve product information for older orders while their listings exist.
    db.Execute("""
        UPDATE [Order]
        SET ItemId = (SELECT ItemId FROM Listing WHERE Listing.Id = [Order].ListingId)
        WHERE ItemId = 0 AND EXISTS (SELECT 1 FROM Listing WHERE Listing.Id = [Order].ListingId)
        """);

    // Upgrade databases created before items had a category.
    var hasCategoryId = db.Execute<int>(
        "SELECT COUNT(*) FROM pragma_table_info('Item') WHERE name = 'CategoryId'");
    if (hasCategoryId == 0)
    {
        db.Execute("ALTER TABLE Item ADD COLUMN CategoryId TEXT");
    }

    // Fixed categories, so items can refer to them by a stable id.
    var categories = new List<Category>
    {
        new() { Id = "drug", Name = "Drug" },
        new() { Id = "weapon", Name = "Weapon" },
        new() { Id = "artifact", Name = "Stolen artifact" },
        new() { Id = "farming", Name = "Farming" }
    };

    foreach (var category in categories)
    {
        if (!db.Categories.Any(existingCategory => existingCategory.Id == category.Id))
        {
            db.Insert(category);
        }
    }

    // every item now has a CategoryId.
    var items = new List<Item>
    {
        new() { Id = 1, Name = "Blaze rod", CategoryId = "drug" },
        new() { Id = 2, Name = "Wheat", CategoryId = "drug" },
        new() { Id = 3, Name = "Sugar", CategoryId = "drug" },
        new() { Id = 4, Name = "Sugar cane", CategoryId = "farming" },
        new() { Id = 5, Name = "TNT", CategoryId = "weapon" },
        new() { Id = 6, Name = "Seeds", CategoryId = "farming" },
        new() { Id = 7, Name = "Iron sword", CategoryId = "weapon" },
        new() { Id = 8, Name = "Gold helmet", CategoryId = "artifact" },
        new() { Id = 9, Name = "Stick", CategoryId = "artifact" }
    };

    // insert new items, and update existing ones so they get their category.
    foreach (var item in items)
    {
        if (db.Items.Any(existingItem => existingItem.Id == item.Id))
        {
            db.Update(item);
        }
        else
        {
            db.Insert(item);
        }
    }
    var hasSellerId = db.Execute<int>(
        "SELECT COUNT(*) FROM pragma_table_info('Order') WHERE name = 'SellerId'");
    if (hasSellerId == 0)
    {
        db.Execute("ALTER TABLE [Order] ADD COLUMN SellerId TEXT NOT NULL DEFAULT ''");
    }

    db.Execute("""
               UPDATE [Order]
               SET SellerId = (SELECT UserId FROM Listing WHERE Listing.Id = [Order].ListingId)
               WHERE SellerId = '' AND EXISTS (SELECT 1 FROM Listing WHERE Listing.Id = [Order].ListingId)
               """);
}

app.Run();