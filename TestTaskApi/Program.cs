using Npgsql;
using System.Data;
using TestTaskApi.DAL.Interfaces;
using TestTaskApi.DAL.Repositories;
using TestTaskApi.DAL;
using TestTaskApi.Domain.Entity;
using TestTaskApi.Service.Implementations;
using TestTaskApi.Service.Interfaces;
using Microsoft.Extensions.FileProviders;
using TestTaskApi.Domain.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", 
    builder =>
    {
        builder.WithOrigins("http://localhost:5001")
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

builder.Services.AddTransient<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new NpgsqlConnection(connectionString);
});

builder.Services.AddTransient<DatabaseInitializer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new DatabaseInitializer(connectionString);
});


builder.Services.AddScoped<IBaseRepository<MessageModel>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MessageRepository(connectionString);
});
builder.Services.AddScoped<IMessageService, MessageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Static", "html")),
    RequestPath = "/html"
});

app.MapHub<MessageHub>("/messageHub");

app.UseAuthorization();

app.MapControllers();

// Initialize the database
var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
dbInitializer.Initialize();

app.Run();
