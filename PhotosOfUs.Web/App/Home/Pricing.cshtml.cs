using System.Threading.Tasks;
using MailChimp.Net;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PhotosOfUs.Pages.Home
{
    public class PricingModel : PageModel
    {
        public PricingModel()
        {
        }

        public async Task<IActionResult> OnPostAsync(string fromEmail)
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail);
            var subject = $"Photos Of Us Requesting Team Quote Contact Email";
            var to = new EmailAddress("marketing@kuviocreative.com");
            var plainTextContent = fromEmail;
            var htmlContent = $"Respond to {fromEmail} regarding a team quote.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);

            return Page();
        }
    }
}