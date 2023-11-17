using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResumeMetadataLibrary.Services;
using ResumeMetadataUI;
using SendGridMailSender.Services;

namespace ResumeMetadata.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUtilities _utilities;
        private readonly IEmailSender _emailsender;

        [BindProperty(SupportsGet = true)]
        public string OutputText { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public string MetadataContent{ get; set; }
        public MessageType MessageStatus { get; set; } = MessageType.None;

        private Stream _modifiedStream;

        public IndexModel(ILogger<IndexModel> logger, IUtilities utilities, IEmailSender mailsender)
        {

            _logger = logger;
            _utilities = utilities;
            _emailsender = mailsender;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            // Check if the user uploaded a file
            if (!FileIsValid())
            {
                return Page();
            }

            // Check if the user added metadata to insert
            if (!MetadataIsValid())
            {
                return Page();
            }

            MemoryStream memoryStream = await ReadTheFileToStream();
            await InjectMetadata(memoryStream);

            if (!InjectedFileIsValid())
            {
                return Page();
            }

            FileStreamResult result = GenerateFile();
            MessageStatus = MessageType.Information;

            await Info(InputFile.FileName);

            return result;
        }

        private async Task Info(string text)
        {
            await _emailsender.SendAdminNotification(text);
        }

        private FileStreamResult GenerateFile()
        {
            _modifiedStream.Position = 0;
            FileStreamResult result = new FileStreamResult(_modifiedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                FileDownloadName = $"Modified_{InputFile.FileName}"
            };

            OutputText = "File generated successfully!";

            return result;
        }

        private async Task<MemoryStream> ReadTheFileToStream()
        {
            var memoryStream = new MemoryStream();
            await InputFile.CopyToAsync(memoryStream);

            return memoryStream;
        }

        private async Task InjectMetadata(MemoryStream memoryStream)
        {
            if (InputFile.FileName.EndsWith("pdf"))
            {
                _modifiedStream = await _utilities.InsertMetadataPdf(memoryStream, MetadataContent);
            }
            else if (InputFile.FileName.EndsWith("docx"))
            {
                _modifiedStream = await _utilities.InsertMetadataDocx(memoryStream, MetadataContent);
            }
            else
                _modifiedStream = null;
        }

        private bool MetadataIsValid()
        {
            if (string.IsNullOrWhiteSpace(MetadataContent))
            {
                OutputText = "You added no metadata. Please add relevant metadata before proceeding.";
                MessageStatus = MessageType.Warning;
                return false;
            }
            return true;
        }

        private bool FileIsValid()
        {
            if (InputFile == null || InputFile.Length == 0)
            {
                OutputText = "Please upload a valid .pdf or .docx file.";
                MessageStatus = MessageType.Warning;
                return false;
            }
            return true;
        }

        private bool InjectedFileIsValid()
        {
            if (_modifiedStream == null)
            {
                OutputText = "There was an issue processing your file. Please try again.";
                MessageStatus = MessageType.Warning;
                return false;
            }

            return true;
        }
    }
}