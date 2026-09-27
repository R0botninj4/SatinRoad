using Infra;
using LinqToDB;
using LinqToDB.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddCors();

var connectionString = builder.Configuration
                           .GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Database connection string is missing.");

var options = new DataOptions().UseSQLite(connectionString);
var dataOptions = new DataOptions<MyDatabaseConnection>(options);

builder.Services.AddScoped<MyDatabaseConnection>(
    _ => new MyDatabaseConnection(dataOptions));

var app = builder.Build();

app.UseCors(config => config
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthorization();

app.MapControllers();



if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider
        .GetRequiredService<MyDatabaseConnection>();

    var result = db.Execute<int>("SELECT 1");

    app.Logger.LogInformation("Database connection test: {Result}", result);
}

app.Run();