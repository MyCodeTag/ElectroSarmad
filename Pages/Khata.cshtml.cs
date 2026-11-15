using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sarmad.Pages
{
    public class KhataModel : PageModel
    {
        
        public readonly int FormId = 29;

        public UserInfo userInfo = new UserInfo();
        public FormData formData;

        public List<ListForm> Form_List;

        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public KhataModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> OnGet(string returnUrl = "Home")
        {
            try
            {
                ViewData["_Page"] = returnUrl;

                // await DBS.CheckToken(HttpContext, userInfo);

               // formData = await DBS.GetFormData(userInfo.Si, FormId);

                return Page();
            }
            catch
            {
                return RedirectToPage("/LoginFast");
            }
        }
        public IActionResult OnPostReturn(string PAGE = "HOME")
        {
            return RedirectToPage(PAGE);
        }
    }
}