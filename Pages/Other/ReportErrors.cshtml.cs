using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sarmad.Pages.Other
{
    public class ReportErrorsModel : PageModel
    {

        private string ProcName = ""; private Dictionary<string, object> Params;
        private string VU = "ReportErrors_View";
        public readonly int FormId = 19;

        public UserInfo userInfo = new UserInfo();
        public FormData formData;

        public DataTable ListData;
        public DataTable ListKarbar;
        public DataTable ListGrpPer;

        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public ReportErrorsModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            { 
                await DBS.CheckToken(HttpContext, userInfo);
                if (userInfo.Active == "N") return RedirectToPage("/Login");

                formData = await DBS.GetFormData(userInfo.Si, FormId);

                // لیست گروه های اشخاص
                Params = new Dictionary<string, object>
                {
                    { "@Lg","Y" }
                };
                ProcName = "dbo.PersonsGrp_List";
                ListGrpPer = await DBS.GetReportAsync(ProcName, Params);

                Params = new Dictionary<string, object>
                {
                    { "@Knd","" }
                };
                ProcName = "dbo.Karbaran_ListActiveKnd";
                ListKarbar = await DBS.GetReportAsync(ProcName, Params);
                return Page();
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }
        public async Task<IActionResult> OnPostFindKarbaran(string KndKarbar = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Knd",KndKarbar }
                };
                ProcName = "dbo.Karbaran_ListActiveKnd";
                ListKarbar = await DBS.GetReportAsync(ProcName, Params);

                ViewData["Switch"] = "SelectKarbar";
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        
        public async Task<IActionResult> OnPostCMDSarmad(string CMD = "")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");
                return Content("فرم غیر فعال است");

                //if (userInfo.Si != 1)
                //{
                //    return Content("شما مجوز اجرا ندارید");
                //}
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }

        public async Task<string> SendMsgAsync(string userId, string msgHd, string sharh, string filePath = null)
        {

            string URLDATA = "https://api.Sarmad.ir/MsgAnswer";
            string UserName = "STORE";
            string Password = "g3oC7M60geLehscK3cQQHIdfQPyGaeh47mI36RgvhzM4MDl0s9335g74OBSnJRz4FALQzK427zK";

            HttpClient _httpClient = new HttpClient();
            var basicAuthenticationValue =
                Convert.ToBase64String(
                       Encoding.ASCII.GetBytes($"{UserName}:{Password}"));

            _httpClient.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Basic", basicAuthenticationValue);

            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(userId), "UserId");
                form.Add(new StringContent(msgHd), "MsgHd");
                form.Add(new StringContent(sharh), "Sharh");

                if (!string.IsNullOrEmpty(filePath))
                {
                    var file = new FileInfo(filePath);
                    if (file.Exists)
                    {
                        var stream = file.OpenRead();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        form.Add(fileContent, "file", Path.GetFileName(filePath));

                    }
                }

                var response = await _httpClient.PostAsync(URLDATA, form);
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<IActionResult> OnPostTest(string UserId, string MsgHd, string Sharh, IFormFile file = null)
        {
            try
            {
                string savedPath = await SaveUploadedFileAsync(file, "wwwroot/uploads");
                
                string RES = await SendMsgAsync(UserId, MsgHd, Sharh, savedPath); ;

                return Content(RES);
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }

            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }

        public async Task<string> SaveUploadedFileAsync(IFormFile file, string uploadFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            // اطمینان از وجود پوشه
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath; // مسیر فیزیکی فایل ذخیره‌شده
        }
    }
}
