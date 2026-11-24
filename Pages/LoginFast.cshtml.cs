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
        public Int32 FormId = 7;

        public async Task<IActionResult> OnPostLogin(string Password)
        {
            var legacyCookie = Request.Cookies["User.Sarmad.ir"].FromLegacyCookieString();
            string UserName = legacyCookie["userName"];
            UserName = (String.IsNullOrEmpty(UserName)) ? "" : UserName.Trim();
            Password = (String.IsNullOrEmpty(Password)) ? "" : Password.Trim();

            //  یوزر را میفرستم و هش پسورد را میگیرم
            Params = new Dictionary<string, object>
            {
                { "@UserName", UserName }
            };
            ProcName = "dbo.GetForLogin";
            var user = await DBS.GetReportRowAsync(ProcName, Params);

            if (user == null)
            {
                return new JsonResult(new JResult
                {
                    Status = "notOK",
                    Message = "نام کاربری یا رمز عبور اشتباه است"
                });

            }

            string clientIp = GetClientIp(HttpContext);

            var passwordHash = user["PasswordHash"].ToString();
            // هش پسورد را با پسوردی که هم اکنون نوشته صحت سنجی میکنم
            if (!PasswordService.VerifyPassword(passwordHash, Password))
            {
                Params = new Dictionary<string, object>
                {
                    { "@PersonId", user["Si_Person"] },
                    { "@ClientIp", clientIp },
                    { "@Sharh", "رمز ورود اشتباه وارد شده است" },
                };
                ProcName = "dbo.SaveErrorLogin";
                await DBS.RunCommandAsync(ProcName, Params);

                return new JsonResult(new JResult
                {
                    Status = "notOK",
                    Message = "نام کاربری یا رمز عبور اشتباه است"
                });
            }

            //پسورد را درست وارد کرده است
            if (user["IsActive"].ToString() == "0")
            {
                Params = new Dictionary<string, object>
                {
                    { "@PersonId", user["Si_Person"] },
                    { "@ClientIp", clientIp },
                    { "@Sharh", "وضعیت شما فعال نیست با مدیریت تماس بگیرید" },
                };
                ProcName = "dbo.SaveErrorLogin";
                await DBS.RunCommandAsync(ProcName, Params);

                return new JsonResult(new JResult
                {
                    Status = "notOK",
                    Message = "وضعیت شما فعال نیست با مدیریت تماس بگیرید"
                });
            }

            Guid token = Guid.NewGuid();

            //   صدازدن تابع ذخیره توکن
            Params = new Dictionary<string, object>
                {
                    { "@PersonId", user["Si_Person"] },
                    { "@AuthToken",  token },
                    { "@ClientIp", clientIp },
                };
            ProcName = "dbo.SabtLogin";
            await DBS.RunCommandAsync(ProcName, Params);

            // ✔ رمز درست است → توکن از SQL
            SetLoginCookies(UserName, token.ToString(), user["Si_Person"].ToString());

            return new JsonResult(new JResult
            {
                Status = "OK",
               // Message = ReturnPage
            });
        }
        private void SetLoginCookies(string UserName, string Token, string ueserSi)
        {
            var option = new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = DateTime.Now.AddDays(365)
            };

            Response.Cookies.Append("Sarmad.ir", Token, option);

            Response.Cookies.Append(
                "User.Sarmad.ir",
                new Dictionary<string, string>
                {
                    { "UserName", UserName },
                    { "Si", ueserSi },
//                    { "Kind", kind }
                }.ToLegacyCookieString(),
                option
            );
        }

        private string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
                return cfIp;

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff))
                return xff.ToString().Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString();
        }


    }
}
