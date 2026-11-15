using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Reflection;

namespace Sarmad.Pages
{
    public class ProfileModel : PageModel
    {
        private string ProcName = ""; private Dictionary<string, object>? Params;
        private string VU = "Profile_View";
        public Int32 FormId = 5;

        public UserInfo userInfo = new UserInfo();
        public required FormData formData;

        public required DataRow UserData { get; set; }
        public required DataTable ListLogin { get; set; }

        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public ProfileModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }


        public async Task<IActionResult> OnGet()
        {
            try
            {
                if (userInfo.Active == "N") return RedirectToPage("/Login", new { returnUrl = Url.PageLink() });

                formData = await DBS.GetFormData(userInfo.Si, FormId);


                Params = new Dictionary<string, object>
                {
                    { "@Si_Person", userInfo.Si},
                };
                ProcName = "dbo.Karbaran_InfoPer";
                UserData = await DBS.GetReportRowAsync(ProcName, Params);

                Params = new Dictionary<string, object>
                {
                    { "@Si_Person", userInfo.Si},
                };
                ProcName = "dbo.LastVisit_IP";
                ListLogin = await DBS.GetReportAsync(ProcName, Params);

                ViewData["ToDay"] = await DBS.Emruzasync();
                return Page();
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }

        // ✅ به‌روزرسانی دیتای کاربر
        public async Task<IActionResult> OnPostUpdateUserInfo(string FName, string LName, string EnName, string Responsible, string Phone, string NationalCode, string Mobile, string Email, string Post, string Adres)
        {
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si } ,
                    { "@FName" , FName } ,
                    { "@LName" , LName } ,
                    { "@EnName" , EnName } ,
                    { "@Phone" , Phone } ,
                    { "@CodeMeli" , NationalCode } ,
                    { "@Mobile" , Mobile } ,
                    { "@Email" , Email } ,
                    { "@Post" , Post } ,
                    { "@Adres" , Adres } ,
                    { "@Responsible" , Responsible } ,
                };

                ProcName = "dbo.Persons_UpdateInfo";
                return Content(await DBS.GetReportResultAsync(ProcName, Params, "Res"));
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error";
                return new PartialViewResult { ViewName = "_CommonView", ViewData = ViewData };
            }
        }

        // ✅ تغییر رمز عبور
        public async Task<IActionResult> OnPostUpdatePass(string UserName, string PassNow, string PassNew)
        {
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                UserName = Center.TrimTxt(UserName);
                PassNow = Center.TrimTxt(PassNow);
                PassNew = Center.TrimTxt(PassNew);

                string Res = "توجه: ";
                if (UserName == "") Res += "\n نام کاربری خود را بنویسید";
                if (PassNow == "") Res += "\n رمز فعلی را بنویسید";
                if (PassNew == "") Res += "\n رمز جدید را بنویسید";

                if (Res == "توجه: ")
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Si_Person", userInfo.Si },
                        { "@UserName", UserName },
                        { "@NowPass", PassNow },
                        { "@NewPass", PassNew},
                    };
                    ProcName = "dbo.Karbaran_Update_Per";
                    var ResRow = await DBS.GetReportRowAsync(ProcName, Params);

                    Res = ResRow["ResTp"].ToString();

                }

                return Content(Res);
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error";

                return new PartialViewResult { ViewName = "_CommonView", ViewData = ViewData };
            }
        }

        // ✅ آپلود تصویر پروفایل
        public async Task<IActionResult> OnPostUpLoadPic(IFormFile PicUpload, string OldPic = "")
        {
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");


                string folder = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", "Persons");

                string ext = Path.GetExtension(PicUpload.FileName).ToUpperInvariant();
                Random random = new Random();
                int Scr = random.Next(1000, 9999);

                if (ext is ".JPG" or ".JPEG" or ".PNG" or ".BMP")
                {
                    ext = ".webp";

                    string fileName = OldPic;
                    string path = Path.Combine(folder, fileName);
                    if (fileName != "K_0.jpg")
                    {
                        Center.DeleteFileIfExists(path);
                    }

                    fileName = $"K_{userInfo.Si}_{Scr}{ext}";
                    path = Path.Combine(folder, fileName);

                    // ذخیره موقت
                    string Temp = Path.Combine(folder, "temp");
                    Center.DeleteFileIfExists(Temp);
                    using (var stream = new FileStream(Temp, FileMode.Create))
                    {
                        PicUpload.CopyTo(stream);
                    }

                    // تغییر سایز تصویر
                    ImageResizer _Img = new ImageResizer(300, 300);
                    await _Img.ResizeAsync(Temp, path, "webp");

                    // اصلاح نام تصویر برای کاربر
                    string ScrExt = $"{Scr}{ext}";
                    Params = new Dictionary<string, object>
                    {
                        { "@Id_Person", userInfo.Si },
                        { "@Pic", ScrExt},
                    };
                    ProcName = "dbo.Karbaran_UpdatePic";
                    await DBS.RunCommandAsync(ProcName, Params);

                    return Content("تصویر با موفقیت ذخیره شد");
                }

                return Content("فقط فرمت‌های تصویری مجازند");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error";

                return new PartialViewResult { ViewName = "_CommonView", ViewData = ViewData };
            }
        }


    }
}
