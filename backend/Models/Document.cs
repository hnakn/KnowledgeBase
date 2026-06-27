namespace backend.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Pages { get; set; }
    public string Content { get; set; } = "";
    public string FilePath { get; set; } = "";

    public ValidationResult Validate()
    {
        if(string.IsNullOrWhiteSpace(this.Title))
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Title is required"
            };
        }
        if(this.Pages<=0)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Pages must be greater than 0"
            };
        }
        if(string.IsNullOrWhiteSpace(this.Content))
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Content cannot be empty"
            };
        }
        return new ValidationResult
        {
            IsValid = true
        };
    }
} 
