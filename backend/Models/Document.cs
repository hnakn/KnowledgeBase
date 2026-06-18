namespace backend.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Pages { get; set; }
    public string Content { get; set; } = "";
} 
