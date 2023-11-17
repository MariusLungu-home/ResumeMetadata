using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResumeMetadataLibrary.Services;
using ResumeMetadataUI;

namespace ResumeMetadata.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUtilities _utilities;
        
        [BindProperty(SupportsGet = true)]
        public string OutputText { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public string MetadataContent{ get; set; }
        public MessageType MessageStatus { get; set; } = MessageType.None;


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
            // Check if the user did not upload a file
            if (InputFile == null || InputFile.Length == 0)
            {
                OutputText = "Please upload a valid .docx file.";
                MessageStatus = MessageType.Warning;
                return Page();
            }


            // Check if the user did not add metadata
            if (string.IsNullOrWhiteSpace(MetadataContent))
            {
                OutputText = "You added no metadata. Please add relevant metadata before proceeding.";
                MessageStatus = MessageType.Warning;
                return Page();
            }

            var memoryStream = new MemoryStream();
            await InputFile.CopyToAsync(memoryStream);
            Stream modifiedStream;

            if (InputFile.FileName.EndsWith("pdf"))
            {
                modifiedStream = await _utilities.InsertMetadataPdf(memoryStream, MetadataContent);
            }
            else
            {
                modifiedStream = await _utilities.InsertMetadataDocx(memoryStream, MetadataContent);
            }
            
            
            // Info that a file was uploaded
            _logger.LogInformation($"File uploaded.");

            if (modifiedStream == null)
            {
                OutputText = "There was an issue processing your file. Please try again.";
                MessageStatus = MessageType.Warning;
                return Page();
            }

            modifiedStream.Position = 0;
            FileStreamResult result = new FileStreamResult(modifiedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                FileDownloadName = $"Modified_{InputFile.FileName}"
            };

            OutputText = "File generated successfully!";
            MessageStatus = MessageType.Information;

            return result;
        }

    }
}