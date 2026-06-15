using backend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/documents", () =>
{
    Document document1 = new Document();
    document1.Id = 1;
    document1.Title = "Hello World";
    document1.Pages = 100;

    Document document2 = new Document
    {
        Id = 2,
        Title = "Hi",
        Pages = 50
    };

    var documents = new []
    {
        document1,
        document2
    };

    return documents;
});

app.MapPost("/documents",(CreateDocumentRequest request) =>
{
    if(string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title Required");
    }
    if(request.Pages <= 0)
    {
        return Results.BadRequest("Pages cannot be 0");
    }
    return Results.Ok(request);   
});

app.Run();
