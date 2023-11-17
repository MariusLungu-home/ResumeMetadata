using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResumeMetadataUI.Models;
using SendGridMailSender.Services;

namespace ResumeMetadataUI.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public ContactFormModel ContactForm { get; set; }
        public IEmailSender _mailSender { get; }

        public ContactModel(IEmailSender mailSender)
        {
            _mailSender = mailSender;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _mailSender.SendContactUsEmail(
                userEmail: ContactForm.Email, 
                userName: ContactForm.Name,
                body: ContactForm.Message
                );

            return RedirectToPage("Success");
        }
    }
}
