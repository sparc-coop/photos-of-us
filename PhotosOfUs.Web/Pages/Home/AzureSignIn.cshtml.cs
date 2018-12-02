using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PhotosOfUs.Pages.Home
{
    [EnableCors(PolicyName = "LoginPolicy")]
    public class AzureSignInModel : PageModel
    {}
}