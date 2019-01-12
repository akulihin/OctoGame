using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OctoGame.WEB.Pages
{
    public class AboutModel : PageModel
    {
       public string OctoMessage {get; set;}
        public void OnGet()
        {
            OctoMessage = "This is Octo Message. Boole!";
        }
    }
}