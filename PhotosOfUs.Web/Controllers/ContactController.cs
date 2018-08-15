using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using MailChimp.Net;
using MailChimp.Net.Models;

namespace PhotosOfUs.Web.Controllers
{
    public class ContactController : Controller
    {
        public IConfiguration Configuration { get; }
    

    [HttpPost]
        public async Task<IActionResult> SendLaunchEmail(ContactViewModel model)
        {
            //var apiKey = "";
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress(model.FromEmail);
            //var subject = $"Photos Of Us Launch Contact Email";
            //var to = new EmailAddress("marketing@kuviocreative.com");
            //var plainTextContent = model.FromEmail;
            //var htmlContent = $"Add to Contact List: {model.FromEmail}";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //await client.SendEmailAsync(msg);

            //return Redirect("/");

            var apiKey = "";
            var mailChimpManager = new MailChimpManager(apiKey);
            var listId = "";
            var email = model.FromEmail;
            var member = new Member { EmailAddress = email, StatusIfNew = Status.Subscribed };
            //member.MergeFields.Add("FNAME", "FirstName");
            //member.MergeFields.Add("LNAME", "LastName");
            await mailChimpManager.Members.AddOrUpdateAsync(listId, member);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> SendQuoteEmail(ContactViewModel model)
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(model.FromEmail);
            var subject = $"Photos Of Us Requesting Team Quote Contact Email";
            var to = new EmailAddress("marketing@kuviocreative.com");
            var plainTextContent = model.FromEmail;
            var htmlContent = $"Respond to {model.FromEmail} regarting a team quote.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);

            return Redirect("/Home/Pricing");
        }
    }

    public class ContactViewModel
    {
        public string FromEmail { get; set; }
    }
}
