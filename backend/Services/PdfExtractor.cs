namespace backend.Services;

using UglyToad.PdfPig;
using System.Text;
using backend.Models;
public class PdfExtractor
{
    public PdfExtractionResult Extract(string filePath)
    {
        var builder = new StringBuilder();

        using(var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                builder.Append(page.Text);
            }
            
            return new PdfExtractionResult
            {
                Text = builder.ToString(),
                PageCount = document.NumberOfPages
            };
        }
    }
}
