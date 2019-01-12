using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OctoGame.WEB.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; }
        public void OnGet()
        {
            Message = "boooooooole~";
        }
    }
}
