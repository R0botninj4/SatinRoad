var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApiDocument();
builder.Services.AddCors();
var app = builder.Build();
app.UseCors(config => config
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod());
app.UseOpenApi();
app.UseSwaggerUi();



app.UseAuthorization();

app.MapControllers();

app.Run();
