using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<SearchIndex>();
builder.Services.AddSingleton<PdfExtractor>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    var searchIndex = scope.ServiceProvider.GetRequiredService<SearchIndex>();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var documents = context.Documents.ToList();

    foreach(var document in documents)
    {
        searchIndex.AddDocument(document);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

Directory.CreateDirectory("Uploads");

app.MapControllers();
app.Run();
