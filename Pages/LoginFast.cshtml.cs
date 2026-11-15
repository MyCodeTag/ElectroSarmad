using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sarmad.Pages
{
    public class LoginFastModel : PageModel
    {

        private string ProcName = ""; private Dictionary<string, object>? Params;
        private string URL = "/LoginFast";
        public Int32 FormId = 120;

        public async Task<IActionResult> OnPostLogin(string Password = "", string SaveData = "NO")
        {
            try
            {
                var legacyCookie = Request.Cookies["User.Sarmad.ir"].FromLegacyCookieString();
                string LoginName = legacyCookie["userName"];
                LoginName = (String.IsNullOrEmpty(LoginName)) ? "" : LoginName.Trim();
                Password = (String.IsNullOrEmpty(Password)) ? "" : Password.Trim();

                string key2 = "ziKm87NIqR3etFkLu4pvMwFe54fGnOg6HhjALaBrS0soP7zpQCcbx632yDdElgMO910JKAGHIPQfghj67";
                var r2 = new Random();
                StringBuilder token = new StringBuilder();
                int idx2 = 0;
                for (int i = 0; i < 75; i++)
                {
                    idx2 = r2.Next(0, 79);
                    token.Append(key2.Substring(idx2, 1));
                }
                string IIP = HttpContext.Connection.RemoteIpAddress.ToString();

                Params = new Dictionary<string, object>
                {
                    { "@loginName", LoginName },
                    { "@Password", Password },
                    { "@IIP", IIP },
                    { "@Token", token.ToString()},
                };
                ProcName = "Karbaran_Login";
                var RES = await DBS.GetReportRowAsync(ProcName, Params);
                if (RES["Res"].ToString() == "OK")
                {
                    Response.Cookies.Delete("Sarmad.ir");
                  //  Response.Cookies.Delete("Login.Sarmad.ir");
                    Response.Cookies.Delete("User.Sarmad.ir");

                    //userInfo.Si = RES["Si"].ToString();
                    //userInfo.Name = RES["FullName"].ToString();
                    //userInfo.Kargah = RES["Id_Kargah"].ToString();
                    //userInfo.NameKargah = RES["Kargah"].ToString();
                    //userInfo.Token = token.ToString();

                    CookieOptions option = new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = false,
                        IsEssential = true,
                        Secure = true,
                        Expires = DateTime.Now.AddDays(365),
                    };
                    Response.Cookies.Append("Sarmad.ir", token.ToString(), option);

                    var userCookie = new Dictionary<string, string>()
                    {
                        { "userName", LoginName.Trim() },
                        { "Kargah", RES["Id_Kargah"].ToString() },
                        { "Si", RES["Si"].ToString() },
                    };

                    Response.Cookies.Append("User.Sarmad.ir", userCookie.ToLegacyCookieString(), option);

                    return Content("OK");
                }
                else
                    return Content(RES["ResTp"].ToString());
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = URL });
            }
        }

    }
}
