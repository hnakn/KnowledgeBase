namespace backend.Controllers;

using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using backend.Services;

[ApiController]
[Route("documents")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SearchIndex _searchIndex;
    
    public DocumentsController(AppDbContext context, SearchIndex searchIndex)
    {
        _context = context;
        _searchIndex = searchIndex;
    }

    [HttpGet]
    public IActionResult GetDocuments()
    {
        var documents = _context.Documents.ToList();
        return Ok(documents);
    }

    [HttpGet("{Id}")]
    public IActionResult GetDocumentsById(int Id)
    {
        var document = _context.Documents.Find(Id);
        
        if(document == null)
        {
            return NotFound();
        }
        return Ok(document);
    }

    [HttpPost]
    public IActionResult CreateDocument(CreateDocumentRequest request)
    {
        var result = request.ValidateDocument();
        if(!result.IsValid)
        {
            return BadRequest(result.ErrorMessage);
        }
        
        var document = new Document
        {
            Title = request.Title,
            Pages = request.Pages,
            Content = request.Content
        };
        
        _context.Documents.Add(document);
        _context.SaveChanges();

        _searchIndex.AddDocument(document);

        return Ok(document);
    }

    [HttpDelete("{Id}")]
    public IActionResult DeleteDocumentById(int Id)
    {
        var document = _context.Documents.Find(Id);

        if(document == null)
        {
            return NotFound();
        }

        _context.Documents.Remove(document);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPut("{Id}")]
    public IActionResult UpdateDocument(CreateDocumentRequest request, int Id)
    {
        var document = _context.Documents.Find(Id);
        document.Title = request.Title;
        document.Pages = request.Pages;
        document.Content = request.Content;
        _context.SaveChanges();
        return Ok(document);
    }

    [HttpGet("search")]
    public IActionResult SearchDocuments(string q)
    {
        var searchResult = _searchIndex.SearchDocuments(q);

        var result = new List<Document>();
        
        foreach(int i in searchResult) result.Add(_context.Documents.Find(i));

        return Ok(result);
    }

}
