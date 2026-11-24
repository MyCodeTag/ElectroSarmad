using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sarmad.Models;
using Sarmad.Services;
using System;
using System.Data;
using System.Reflection;

namespace Sarmad.Pages.Base
{
    public class PersonsModel : PageModel
    {
        private string ProcName = ""; private Dictionary<string, object>? Params;
        private string VU = "./Persons_View";
        public Int32 FormId = 2;

        public UserInfo userInfo = new UserInfo();
        public required FormData formData { get; set; }
        public required DataTable ListPersons { get; set; }
        public required DataTable ListData { get; set; }
        public required DataTable ListForms { get; set; }
        public required DataTable ListGRP { get; set; }
        public required DataRow RowData { get; set; }


        public async Task<IActionResult> OnGet()
        {
            try
            {
                await DBS.CheckToken(HttpContext, userInfo);
                if (userInfo.Active == "N") return RedirectToPage("/Login", new { returnUrl = Url.PageLink()});

                formData = await DBS.GetFormData(userInfo.Si, FormId);

                // لیست گروه های اشخاص
                Params = new Dictionary<string, object>
                {
                    { "@Lg","Y" }
                };
                ProcName = "dbo.PersonsGrp_List";
                ListGRP = await DBS.GetReportAsync(ProcName, Params);

                return Page();
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }

        public async Task<IActionResult> OnPostListPersons(string Act, string Filter, string Sort)
        {// لیست اشخاص
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Act", Act },
                    { "@Filter", Filter },
                    { "@Sort", Sort },
                };
                ProcName = "dbo.Persons_List";

                ListPersons = await DBS.GetReportAsync(ProcName, Params);
                if (ListPersons.Rows.Count == 0)
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = "فردی با مشخصات فوق پیدا نشد. شرایط جستجو را تغییر بدهید و دوباره گزارش بگیرید";

                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }
                ;

                ViewData["Switch"] = "ListPerson";

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

        public async Task<IActionResult> OnGetFormSabtPerson(string ID = "0")
        {// فرم ثبت دیتای اشخاص
            try
            {
                ViewData["ID"] = ID;

                // لیست شهرها
                Params = new Dictionary<string, object>
                {
                    { "@Filter","" }
                };
                ProcName = "dbo.Citys_List";
                ListData = await DBS.GetReportAsync(ProcName, Params);


                // لیست گروه های اشخاص
                Params = new Dictionary<string, object>
                {
                    { "@Lg","Y" }
                };
                ProcName = "dbo.PersonsGrp_List";
                ListGRP = await DBS.GetReportAsync(ProcName, Params);

                if (ID != "0")
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Si" , ID } ,
                    };
                    ProcName = "dbo.Persons_Info";
                    RowData = await DBS.GetReportRowAsync(ProcName, Params);
                }
                else
                {
                    Params = null;
                    ProcName = "dbo.Persons_NewCu";
                    ViewData["NewCu"] = await DBS.GetReportResultAsync(ProcName, Params, "NewCu");

                }
                ViewData["Switch"] = "FormSabtPerson";
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

        public async Task<IActionResult> OnPostUpdatePerson(string ID, string Kind, string Code, string CodeMeli, string Act
                        , string FName, string LName, string Grps, string Responsible, string Mobile, string Email, string Phone, string Post, string City, string Adres, string Sharh)
        {//ثبت شخص
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si } ,
                    { "@Id", ID },
                    { "@Cu_Person", Code },
                    { "@Kind", Kind },
                    { "@isActive", Act },
                    { "@Si_City", City },
                    { "@FName" , FName } ,
                    { "@LName" , LName } ,
                    { "@Groups" , Grps } ,
                    { "@Phone" , Phone } ,
                    { "@CodeMeli" , CodeMeli } ,
                    { "@Mobile" , Mobile } ,
                    { "@Email" , Email } ,
                    { "@Post" , Post } ,
                    { "@Adres" , Adres } ,
                    { "@Responsible" , Responsible } ,
                    { "@Sharh", Sharh },
                };

                ProcName = "dbo.Persons_Update";
                var UpdateRes = await DBS.GetReportRowAsync(ProcName, Params);
              
                return new JsonResult(UpdateRes.ToJsonObject());

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
        public async Task<IActionResult> OnPostDeletePerson(string ID)
        {//حذف شخص
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");


                Params = new Dictionary<string, object>
                {
                    { "@ID", ID },
                };

                ProcName = "dbo.Persons_Delete";

                return Content(await DBS.GetReportResultAsync(ProcName, Params, "Res"));

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

        public async Task<IActionResult> OnGetFormSabtNewCity()
        {// فرم ثبت شهر جدید
            try
            {
                // لیست استانها
                Params = new Dictionary<string, object>
                {
                    { "@Filter","" }
                };
                ProcName = "dbo.Ostan_List";
                ListData = await DBS.GetReportAsync(ProcName, Params);

                ViewData["Switch"] = "FormSabtNewCity";
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

        public async Task<IActionResult> OnPostAddNewCity(string StateId, string CityName)
        {
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id", 0},
                    { "@Si_Ostan", @StateId},
                    { "@Name", @CityName},
                    { "@Act", "Y"},
                };
                ProcName = "dbo.Citys_Update";
                await DBS.RunCommandAsync(ProcName, Params);

                // لیست شهرها
                Params = new Dictionary<string, object>
                {
                    { "@Filter", CityName }
                };
                ProcName = "dbo.Citys_List";
                ListData = await DBS.GetReportAsync(ProcName, Params);

            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            ViewData["Switch"] = "Select";

            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }



        public async Task<IActionResult> OnPostListKarbaran(string Act, string Group, string Filter, string Sort)
        {// لیست اشخاص
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Act", Act },
                    { "@Group", Group },
                    { "@Filter", Filter },
                    { "@Sort", Sort },
                };
                ProcName = "dbo.Karbarn_List";

                ListPersons = await DBS.GetReportAsync(ProcName, Params);
                if (ListPersons.Rows.Count == 0)
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = "کاربری با مشخصات فوق پیدا نشد. شرایط جستجو را تغییر بدهید و دوباره گزارش بگیرید";

                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }
                ;

                ViewData["Switch"] = "ListKarbaran";

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

        public async Task<IActionResult> OnGetFormSabtKarbaran(string ID)
        {// فرم ثبت دیتای کاربر
            try
            {
                ViewData["ID"] = ID;

                Params = new Dictionary<string, object>
                {
                    { "@Si_Person" , ID },
                };
                ProcName = "dbo.Karbaran_InfoPer";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);

                ViewData["Switch"] = "FormSabtKarbaran";
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

        public async Task<IActionResult> OnGetFormFindPerson()
        {// فرم پیدا کردن کاربر
            try
            {
                ViewData["Switch"] = "FormFindPerson";
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
        public async Task<IActionResult> OnGetFindPerson(string Filter)
        {// لیست اشخاص فیلتر شده
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Filter", Filter },
                };
                ProcName = "dbo.Persons_ListNotUser";

                ListPersons = await DBS.GetReportAsync(ProcName, Params);
                if (ListPersons.Rows.Count == 0)
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = "فردی با مشخصات فوق پیدا نشد. شرایط جستجو را تغییر بدهید و دوباره گزارش بگیرید";

                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }
                ;

                ViewData["Switch"] = "ListPersonFiltered";

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

        public async Task<IActionResult> OnPostUpdateKarbar(string ID, string PersonId, string UserName, string KarbarAct, string Knd, string Pass)
        {//ثبت کاربر
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                Pass = Pass?.Trim() ?? "";

                var passwordHash = PasswordService.HashPassword(Pass);

                Params = new Dictionary<string, object>
                {
                    { "@Si_Editor", userInfo.Si } ,
                    { "@Si", ID },
                    { "@Si_Person", PersonId },
                    { "@UserName", UserName },
                    { "@isActive", KarbarAct },
                    { "@Kind", Knd },
                    { "@PassHash" , passwordHash } ,
                };

                ProcName = "dbo.Karbaran_Update";
                var UpdateRes = await DBS.GetReportRowAsync(ProcName, Params);

                return new JsonResult(UpdateRes.ToJsonObject());

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

        public async Task<IActionResult> OnPostUpdatePass(string ID, string PASS)
        {//تغییر رمز کاربر
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                var passwordHash = PASS?.Trim() ?? "";
                if (passwordHash != "")
                {
                    passwordHash = PasswordService.HashPassword(passwordHash);
                }

                Params = new Dictionary<string, object>
                {
                    { "@Si_Editor", userInfo.Si } ,
                    { "@Si", ID },
                    { "@PassHash", passwordHash },
                };

                ProcName = "dbo.Karbaran_UpdatePass";

                var UpdatePas = await DBS.GetReportRowAsync(ProcName, Params);

                return Content(UpdatePas["message"].ToString());

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

        public async Task<IActionResult> OnPostDeleteKarbar(string ID, string IDPerson)
        {//ثبت کاربر
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                if (IDPerson == userInfo.Si.ToString())
                {
                    return Content("شما نمی توانید خودتان را حذف کنید");
                }
                Params = new Dictionary<string, object>
                {
                    { "@Si", ID } ,
                    { "@Si_Person", IDPerson } ,
                };

                ProcName = "dbo.Karbaran_Delete";
                return Content(await DBS.GetReportResultAsync(ProcName, Params, "Res"));

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

        public async Task<IActionResult> OnPostGoToUserSystem(string UserId = "")
        {
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                Response.Cookies.Delete("User.Sarmad.ir");
                var userCookie = new Dictionary<string, string>()
                {
                    { "Si", UserId  },
                };
                Response.Cookies.Append("User.Sarmad.ir", userCookie.ToLegacyCookieString());

                Params = new Dictionary<string, object>
                {
                    { "@Mng", userInfo.Si },
                    { "@Client", UserId },
                };
                ProcName = "dbo.UserChangeToken";
                await DBS.RunCommandAsync(ProcName, Params);
                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
                return new PartialViewResult
                {
                    ViewName = VU,
                    ViewData = this.ViewData
                };
            }
        }

        public async Task<IActionResult> OnGetFormSabtAccess(string IDPerson = "0")
        {// فرم ثبت دیتای اشخاص
            try
            {
                ViewData["ID"] = IDPerson;

                // تخصیص داده نشده لیست فرمها
                Params = new Dictionary<string, object>
                {
                    { "@Person", IDPerson },
                };
                ProcName = "dbo.KarbaranForms_NotActive";
                ListForms = await DBS.GetReportAsync(ProcName, Params);

                // فرمهای تخصیص داده شده
                Params = new Dictionary<string, object>
                {
                    { "@Person", IDPerson },
                };
                ProcName = "dbo.KarbaranForms_Active";
                ListData = await DBS.GetReportAsync(ProcName, Params);

                ViewData["Switch"] = "FormSabtAccess";
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

        public async Task<IActionResult> OnGetAccessAreaInfo(string IDPerson, string Act)
        {// فرم تخصیصها به کاربر
            try
            {
                ViewData["ID"] = IDPerson;

                if (Act == "A")
                {
                    // تخصیص داده نشده لیست فرمها
                    Params = new Dictionary<string, object>
                    {
                        { "@Id_Karbar", IDPerson },
                    };
                    ProcName = "dbo.KarbaranForms_NotActive";
                    ListForms = await DBS.GetReportAsync(ProcName, Params);

                    ViewData["Switch"] = "Access_NotForms";
                }

                if (Act == "B")
                {
                    // تخصیص داده نشده لیست فرمها
                    Params = new Dictionary<string, object>
                    {
                        { "@Id_Karbar", IDPerson },
                    };
                    ProcName = "dbo.KarbaranForms_Active";
                    ListForms = await DBS.GetReportAsync(ProcName, Params);

                    ViewData["Switch"] = "Access_ActiveForms";
                }

                if (Act == "C")
                {
                    // جانشین ها
                    Params = new Dictionary<string, object>
                    {
                        { "@Person", IDPerson },
                    };
                    ProcName = "dbo.KarbaranForms_Active";
                    ListForms = await DBS.GetReportAsync(ProcName, Params);

                    ViewData["Switch"] = "Access_Standby";
                }

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


        public async Task<IActionResult> OnPostUpdateAccessForm(string IdForm, string IdPerson)
        {//تخصیص فرم به کاربر
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo)) return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Karbar", IdPerson } ,
                    { "@Form", IdForm },
                };

                ProcName = "dbo.KarbaranForms_Update";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
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

        public async Task<IActionResult> OnGetLastVisit_IP(string IDPerson)
        {// سوابق ورود به سیستم
            try
            {

                Params = new Dictionary<string, object>
                {
                    { "@Si_Person", IDPerson},
                };
                ProcName = "dbo.LastVisit_IP";
                ListData = await DBS.GetReportAsync(ProcName, Params);


                ViewData["Switch"] = "LastVisit_IP";
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

    }
}
