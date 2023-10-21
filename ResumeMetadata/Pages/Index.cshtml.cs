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

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            // TODO: Get data from form

            string sourcePath = Request.Form["sourcePath"];
            string destinationPath = Request.Form["destinationPath"];
            string metaDataToInsert = Request.Form["metaDataToInsert"];

            // TODO: Use dependency injection to get the Utilities class
            new Utilities().InsertMetadata(sourcePath, destinationPath, metaDataToInsert);


            // TODO: Return the data to the page
            return RedirectToPage(new { OutputText });

            // TODO: if error, return error message
        }
    }
}