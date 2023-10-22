using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResumeMetadataLibrary;

namespace ResumeMetadata.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUtilities _utilities;
        
        [BindProperty(SupportsGet = true)]
        public string OutputText { get; set; }

        [BindProperty]
        public IFormFile DocxFile { get; set; }

        [BindProperty]
        public string MetadataContent{ get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            // Process textarea content
            if (string.IsNullOrWhiteSpace(MetadataContent))
            {
                OutputText = "You added no metadata.";
                return RedirectToPage(OutputText);
            }

            // Process the uploaded .docx file
            if (DocxFile != null && DocxFile.Length > 0)
            {
                var utilities = new Utilities();

                // Assuming InsertMetadata returns a Stream with the modified content
                using (var memoryStream = new MemoryStream())
                {
                    await DocxFile.CopyToAsync(memoryStream);
                    Stream modifiedStream = await utilities.InsertMetadata(memoryStream, MetadataContent);

                    modifiedStream.Position = 0;
                    return new FileStreamResult(modifiedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                    {
                        FileDownloadName = $"Modified_{DocxFile.FileName}"
                    };
                }
            }

            OutputText = "New document created! Warning: A small white (non-visible) object was appended to the end of your document. Please check that this has not caused an additional page to be added to your document.";
            return RedirectToPage(OutputText);
        }
    }
}