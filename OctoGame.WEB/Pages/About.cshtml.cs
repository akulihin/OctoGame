using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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