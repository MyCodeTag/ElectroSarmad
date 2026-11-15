using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sarmad.Pages
{
    //  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public ErrorModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet(string returnUrl, string Khata)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Khata"] = Khata;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }

    }
}
