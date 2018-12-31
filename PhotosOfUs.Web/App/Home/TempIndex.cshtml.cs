using System.Threading.Tasks;
using MailChimp.Net;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PhotosOfUs.Pages.Home
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {

        }

        public async Task<IActionResult> OnPostAsync(string fromEmail)
        {
            var apiKey = "";
            var mailChimpManager = new MailChimpManager(apiKey);
            var listId = "";
            var email = fromEmail;
            var member = new Member { EmailAddress = email, StatusIfNew = Status.Subscribed };
            //member.MergeFields.Add("FNAME", "FirstName");
            //member.MergeFields.Add("LNAME", "LastName");
            await mailChimpManager.Members.AddOrUpdateAsync(listId, member);

            TempData["IsEmailSent"] = true;

            return Page();
        }
    }
}