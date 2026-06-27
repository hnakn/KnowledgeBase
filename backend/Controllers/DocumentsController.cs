namespace backend.Controllers;

using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.IO;

[ApiController]
[Route("documents")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SearchIndex _searchIndex;
    private readonly PdfExtractor _pdfExtractor;
    
    public DocumentsController(AppDbContext context, SearchIndex searchIndex, PdfExtractor pdfExtractor)
    {
        _context = context;
        _searchIndex = searchIndex;
        _pdfExtractor = pdfExtractor;
    }

    private IActionResult SaveDocument(Document document)
    {
        var result = document.Validate();
        if(!result.IsValid)
        {
            System.IO.File.Delete(document.FilePath);
            return BadRequest(result.ErrorMessage);
        }
        
        _context.Documents.Add(document);
        _context.SaveChanges();

        _searchIndex.AddDocument(document);

        return Ok(document);
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

    [HttpPost("upload")]
    public IActionResult UploadDocument([FromForm] UploadDocumentRequest request)
    {
        var validationResult = request.Validate();
        if(!validationResult.IsValid)
        {
            return BadRequest(validationResult.ErrorMessage);
        }
        var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
        var filePath = Path.Combine("Uploads",fileName);

        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            request.File.CopyTo(stream);
        }
        
        var extractedResult = _pdfExtractor.Extract(filePath);
        var document = new Document
        {
            Title = request.Title,
            Pages = extractedResult.PageCount,
            Content = extractedResult.Text,
            FilePath = filePath
        };
        return SaveDocument(document);
    }

    [HttpDelete("{Id}")]
    public IActionResult DeleteDocumentById(int Id)
    {
        var document = _context.Documents.Find(Id);

        if(System.IO.File.Exists(document.FilePath))
        {
            System.IO.File.Delete(document.FilePath);
        }
        if(document == null)
        {
            return NotFound();
        }

        _searchIndex.RemoveDocument(document);

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

};
