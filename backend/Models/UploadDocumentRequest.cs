namespace backend.Models;

public class UploadDocumentRequest
{
    public string Title { get; set; } = "";
    public IFormFile File { get; set; } = null!;
    
    public ValidationResult Validate()
    {
        if(File.Length == 0)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "File cannot be empty"
            };
        }
        if(File.ContentType != "application/pdf" || Path.GetExtension(File.FileName)!=".pdf")
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "File must be of type PDF"
            };
        }

        return new ValidationResult
        {
            IsValid = true
        };
    }
}