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

        public IndexModel(ILogger<IndexModel> logger, IUtilities utilities)
        {
            _logger = logger;
            _utilities = utilities;
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
                var memoryStream = new MemoryStream();
                
                await DocxFile.CopyToAsync(memoryStream);
                Stream modifiedStream = await _utilities.InsertMetadata(memoryStream, MetadataContent);

                modifiedStream.Position = 0;

                FileStreamResult result = new FileStreamResult(modifiedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    FileDownloadName = $"Modified_{DocxFile.FileName}"
                };

                return result;
            }

            OutputText = "AI broke, try again.";

            return RedirectToPage(OutputText);
        }
    }
}