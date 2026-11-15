using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ZXing;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Mail;
//using MimeKit;

namespace Sarmad.Pages.Dayli
{
    public class MsgCenterModel : PageModel
    {

        private string ProcName = ""; private Dictionary<string, object> Params;
        private string VU = "MsgCenter_View";
        public readonly int FormId = 3;
        public UserInfo userInfo = new UserInfo();
        public FormData formData;

        public DataTable ListNewMsg;
        public DataTable ListPinMsg;

        public DataTable ListData;
        public DataTable ListKarbaran;
        public DataTable ListGroup;
        public DataTable ListMsgDt;
        public DataRow RowData;

        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public MsgCenterModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
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

                formData.Today = await DBS.Emruzasync();

                ViewData["ThisYear"] = formData.Today.Left(4);

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                };
                ProcName = "dbo.MsgDt_ListNew";
                ListNewMsg = await DBS.GetReportAsync(ProcName, Params);

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                };
                ProcName = "dbo.MsgPin_List";
                ListPinMsg = await DBS.GetReportAsync(ProcName, Params);

                // لیست گروه های اشخاص
                Params = new Dictionary<string, object>
                {
                    { "@Lg","Y" }
                };
                ProcName = "dbo.PersonsGrp_List";
                ListGroup = await DBS.GetReportAsync(ProcName, Params);

                // همه کاربران
                Params = new Dictionary<string, object>
                {
                    { "@Grp", ""},
                    { "@Filter", ""},
                };
                ProcName = "dbo.Karbaran_ListActive";
                ListKarbaran = await DBS.GetReportAsync(ProcName, Params);

                ViewData["ToDay"] = await DBS.Emruzasync();
                return Page();
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }
        public async Task<IActionResult> OnGetListMokhatab(string KND, string TAKGRP, string GRP, string FilterName)
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Grp", GRP },
                    { "@Filter", FilterName },
                };  
                ProcName = "dbo.Karbaran_ListActive";
                ListKarbaran = await DBS.GetReportAsync(ProcName, Params);
                if (ListKarbaran.Rows.Count == 0)
                {
                    ViewData["MSG"] = "مخاطبی پیدا نشد. شرایط جستجو را تغییر بدهید";
                    ViewData["Switch"] = "MSG";
                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }

                ViewData["KND"] = KND;
                ViewData["TAKGRP"] = TAKGRP;
                ViewData["Switch"] = "ListMokhatab";
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
        public async Task<IActionResult> OnPostMsgReports(string KndReport, string KND, string OWNER, string WRITER, string ACTIVE, string NEW, string MOKHATAB, string ONVAN, string MATN)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                    { "@Report", KndReport},
                    { "@MOKHATAB", MOKHATAB},
                    { "@KND", KND},
                    { "@Active", ACTIVE},
                    { "@Owner", OWNER},
                    { "@Writer", WRITER},
                    { "@New", NEW},
                    { "@Onvan", ONVAN},
                    { "@Matn", MATN},
                };
                ProcName = "dbo.Msgs_Report";
                ListData = await DBS.GetReportAsync(ProcName, Params);
               // await DBS.TestProcedure(ProcName, Params);
                if (ListData.Rows.Count == 0)
                {
                    ViewData["MSG"] = "پیامی پیدا نشد. شرایط جستجو را تغییر بدهید";
                    ViewData["Switch"] = "MSG";
                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }

                ViewData["UserSi"] = userInfo.Si;
                ViewData["Switch"] = "MsgReports";
                ViewData["KndReport"] = KndReport;

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
        public async Task<IActionResult> OnPostChangAct(string ID, string Active)
        { // تغییر وضعیت یک پیام از باز به بسته و بالعکس 
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_Owner", userInfo.Si},
                    { "@ID", ID},
                };
                ProcName = "dbo.MsgHd_Active";
                var RES = await DBS.GetReportResultAsync(ProcName, Params, "Res");
                return Content(RES);
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Content("OK");
            }
        }
        public async Task<IActionResult> OnPostMsgRead(string Id)
        { // خواندن پیام ها تا تاریخ مشخص 
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    {"@Id_User", userInfo.Si},
                    {"@Id_Dt", Id},
                };
                ProcName = "dbo.MsgDt_Read";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Content("OK");
            }

        }
        public async Task<IActionResult> OnPostChangPin(string Id_Msg)
        { // پین کردن یک پیام و بالعکس
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    {"@Id_User", userInfo.Si},
                    {"@Id_Msg", Id_Msg},
                };
                ProcName = "dbo.MsgPin_Chang";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Content("OK");
            }

        }
        public async Task<IActionResult> OnPostUpdateSortPin(string Id, string Sort)
        { // تفییر مرتب سازی یا اهمیت موضوع پین شده
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    {"@Id_User", userInfo.Si},
                    {"@Id", Id},
                    {"@Sort", Sort},
                };
                ProcName = "dbo.MsgPin_UpdateSort";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Content("OK");
            }

        }

        private async Task Reply_List(string HDID, string Order)
        {
            // 	لیست گفتگوهای یک پیام
            Params = new Dictionary<string, object>
            {
                { "@Id_User", userInfo.Si },
                { "@Id_Hd", HDID },
                { "@Order", Order},
            };
            ProcName = "dbo.MsgDt_ListHd";
            ListMsgDt = await DBS.GetReportAsync(ProcName, Params);
            ViewData["Order"] = Order;

            Params = new Dictionary<string, object>
            {
                { "@Id", HDID},
                { "@Id_User", userInfo.Si },
            };
            ProcName = "dbo.MsgHd_Info";
            RowData = await DBS.GetReportRowAsync(ProcName, Params);

            Params = new Dictionary<string, object>
            {
                { "@Id", HDID},
            };
            ProcName = "dbo.MsgUsers_List";
            ListKarbaran = await DBS.GetReportAsync(ProcName, Params);
            
            ViewData["UserSi"] = userInfo.Si;
        }
        public async Task<IActionResult> OnPostMsgDtList(string HDID, string Order)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                await Reply_List(HDID, Order);

                ViewData["Switch"] = "ReplyList";
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

        public async Task<IActionResult> OnPostNewMsg(IFormFile FileUpload, string Mokhataban, string MsgOnvan, string ChatRoom, string MsgMatn)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                string Res = "توجه:";
                string CodePeyvast = "";
                MsgOnvan = Center.TrimTxt(MsgOnvan);
                MsgMatn = Center.TrimTxt(MsgMatn);

                if (Mokhataban == "0,")
                {
                    Res += " \n مخاطبان را انتخاب کنید";
                }
                if (MsgOnvan == "" && ChatRoom == "Room")
                {
                    Res += " \n موضوع پیام را بنویسید";
                }
                if (MsgMatn == "")
                {
                    Res += " \n متن پیام را بنویسید";
                }

                if (Res == "توجه:")
                {
                    string ext = ".non";
                    string newPath = "";
                    if (FileUpload != null)
                    {
                        string folderName = @"Uploads\MsgCenter";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        newPath = Path.Combine(webRootPath, folderName);
                        ext = Path.GetExtension(FileUpload.FileName);
                        ext = ext.ToLower();
                    }


                    if (ChatRoom == "Chat")
                    { // ارسال پیام بدون موضوع
                        Params = new Dictionary<string, object>
                        {
                            { "@Id_User", userInfo.Si},
                            { "@List_Mokhatan", Mokhataban},
                            { "@Matn", MsgMatn},
                            { "@ext", ext},
                            { "@ReturnMsg", "Y"},
                        };
                        ProcName = "dbo.MsgHd_NewFree";
                        ListData = await DBS.GetReportAsync(ProcName, Params);
                    }
                    else
                    { // ارسال پیام با موضوع
                        Params = new Dictionary<string, object>
                        {
                            { "@Id_Owner", userInfo.Si},
                            { "@Id_Reciver", Mokhataban},
                            { "@Onvan", MsgOnvan},
                            { "@Matn", MsgMatn},
                            { "@ext", ext},
                        };
                        ProcName = "dbo.MsgHd_New";
                        var SabtMsgHd_New = await DBS.GetReportRowAsync(ProcName, Params);
                        CodePeyvast = SabtMsgHd_New.Field<string>("CodePeyvast");

                        await SendEmail(SabtMsgHd_New.Field<int>("HdId"), userInfo.Si, userInfo.Name, MsgOnvan);
                    }


                    if (FileUpload != null)
                    {
                        string fullPath = "";
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".pdf" || ext == ".txt" || ext == ".xls" || ext == ".xlsx" || ext == ".doc" || ext == ".docx" || ext == ".zip" || ext == ".rar")
                        {
                            if (ChatRoom == "Chat")
                            {
                                foreach (DataRow item in ListData.Rows)
                                {
                                    fullPath = Path.Combine(newPath, "P_" + item["Sectp"].ToString());
                                    Center.DeleteFileIfExists(fullPath);

                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        FileUpload.CopyTo(stream);
                                    }
                                }
                            }
                            else
                            {
                                fullPath = Path.Combine(newPath, "P_" + CodePeyvast);
                                Center.DeleteFileIfExists(fullPath);

                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    FileUpload.CopyTo(stream);
                                }
                            }
                            Res = "پیام ارسال شد";
                        }
                        else
                        {
                            Res = "پیام ذخیره شد اما فایل آپلود نشد";
                        }
                    }
                    else
                        Res = "پیام ارسال شد";

                    ViewData["MSG"] = Res;
                    ViewData["Switch"] = "MSG";
                }
                else
                {
                    ViewData["MSGHeader"] = "خطا در ارسال پیام";
                    ViewData["MSG"] = Res;
                    ViewData["MSGFooter"] = "";
                    ViewData["MSGAction"] = "";
                    ViewData["Switch"] = "ResultALarm";
                }

            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error";
            }

            return new PartialViewResult
            {
                ViewName = "_CommonView",
                ViewData = this.ViewData
            };

        }

        public async Task<IActionResult> OnPostReplyMsg(IFormFile FileUpload, string HDID, string MsgMatn)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                if (Center.TrimTxt(MsgMatn) == "")
                {
                    ViewData["Switch"] = "ResultAlarm";
                    ViewData["MSGHeader"] = "خطا در ارسال پیام";
                    ViewData["MSG"] = "توجه: \n\r متن پیام را بنویسید ";
                    ViewData["MSGFooter"] = "";
                    ViewData["MSGAction"] = "MsgDtListBtn";
                    ViewData["MSGActionOnvan"] = "لیست پیامهای این گفتگو";
                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }



                string ext = ".non";
                string newPath = "";
                if (FileUpload != null)
                {
                    string folderName = @"Uploads\MsgCenter";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    newPath = Path.Combine(webRootPath, folderName);
                    ext = Path.GetExtension(FileUpload.FileName);
                    ext = ext.ToLower();
                }


                Params = new Dictionary<string, object>
                {
                    { "@Id_Hd", HDID },
                    { "@Id_User", userInfo.Si },
                    { "@Matn", MsgMatn},
                    { "@ext", ext},
                };
                ProcName = "dbo.MsgDt_New";
                var SabtMsgDt_New = await DBS.GetReportRowAsync(ProcName, Params);
                    
                if(SabtMsgDt_New.Field<string>("success") == "OK")
                {
                    // ارسال ایمیل
                    await SendEmail(SabtMsgDt_New.Field<int>("HdId")
                            , userInfo.Si
                            , userInfo.Name
                            , SabtMsgDt_New.Field<string>("MsgOnvan"));

                    // اگر آیدی ذخیره پیام برگشت داده شد تصویر ذخیره شود
                    if (FileUpload != null)
                    {
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".pdf" || ext == ".txt" || ext == ".xls" || ext == ".xlsx" || ext == ".doc" || ext == ".docx" || ext == ".zip" || ext == ".rar")
                        {
                            string fullPath = Path.Combine(newPath, "P_" + SabtMsgDt_New.Field<string>("CodePeyvast"));
                            Center.DeleteFileIfExists(fullPath);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                FileUpload.CopyTo(stream);
                            }
                        }
                        else
                        {
                            ViewData["Switch"] = "ResultAlarm";
                            ViewData["MSGHeader"] = "خطا در ذخیره پیوست";
                            ViewData["MSG"] = "پیام ذخیره شد اما فایل آپلود نشد";
                            ViewData["MSGAction"] = "MsgDtListBtn";
                            ViewData["MSGActionOnvan"] = "لیست پیامهای این گفتگو";
                            ViewData["MSGFooter"] = "";
                            return new PartialViewResult
                            {
                                ViewName = "_CommonView",
                                ViewData = this.ViewData
                            };
                        }
                    }

                    await Reply_List(HDID, "Desc");

                    ViewData["Switch"] = "ReplyList";

                }
                else
                {
                    ViewData["Switch"] = "ResultAlarm";
                    ViewData["MSGHeader"] = "خطا در ارسال پیام";
                    ViewData["MSG"] = SabtMsgDt_New.Field<string>("message");
                    ViewData["MSGAction"] = "MsgDtListBtn";
                    ViewData["MSGActionOnvan"] = "لیست پیامهای این گفتگو";
                    ViewData["MSGFooter"] = "";
                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
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

        public async Task<IActionResult> OnPostHdDelete(string HDID)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si },
                    { "@Id_Hd", HDID },
                };
                ProcName = "dbo.MsgHd_Delete";
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

        public async Task<IActionResult> OnPostYaddashtUpdate(string ID, string Date, string Time, string Matn)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                string Res = "توجه:";
                Matn = Center.TrimTxt(Matn);
                Time = Center.TrimTxt(Time);

                if (Matn == "")
                {
                    Res += " \n متن یاداشت را بنویسید";
                }
                if (Time == "")
                {
                    Res += " \n ساعت یادآور را انتخاب کنید";
                }

                if (Res == "توجه:")
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Si", ID },
                        { "@Id_User", userInfo.Si },
                        { "@DateYadavar", Date },
                        { "@TimeYadavar", Time },
                        { "@Matn", Matn },
                    };
                    ProcName = "dbo.Yaddasht_Update";
                    await DBS.RunCommandAsync(ProcName, Params);

                    ViewData["MSG"] = "یادداشت یادآور ذخیره شده";
                    ViewData["Switch"] = "MSG";
                }
                else
                {
                    ViewData["MSGHeader"] = "خطا در ثبت یادداشت";
                    ViewData["MSG"] = Res;
                    ViewData["MSGFooter"] = "";
                    ViewData["MSGAction"] = "";
                    ViewData["Switch"] = "ResultALarm";
                }

            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error";
            }

            return new PartialViewResult
            {
                ViewName = "_CommonView",
                ViewData = this.ViewData
            };

        }
        public async Task<IActionResult> OnPostYaddashtReport(string FromDate, string ToDate, string Matn, string Act)
        {
            try
            {
                Center.GetUserData(Center.TrimTxt(Request.Cookies["User.Sarmad.ir"]), userInfo);
                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si },
                    { "@FromDate", FromDate },
                    { "@ToDate", ToDate },
                    { "@Matn", Matn},
                    { "@Act", Act},
                };
                ProcName = "dbo.Yaddasht_List";
                ListData = await DBS.GetReportAsync(ProcName, Params);
                if (ListData.Rows.Count == 0)
                {
                    ViewData["MSG"] = "یادآوری پیدا نشد. شرایط جستجو را تغییر بدهید";
                    ViewData["Switch"] = "MSG";
                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }
                ViewData["Switch"] = "YadList";
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
        public async Task<IActionResult> OnPostChangActYad(string ID)
        { // تغییر وضعیت یک یادآور 
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                    { "@Si", ID},
                };
                ProcName = "dbo.Yaddasht_Activate";
                await DBS.RunCommandAsync(ProcName, Params);
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Content("OK");
        }
        public async Task<IActionResult> OnPostYadDelete(string ID)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                    { "@Si", ID},
                };
                ProcName = "dbo.Yaddasht_Delete";
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
        public async Task<IActionResult> OnGetNewTime(string SEC)
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@SEC", SEC},
                };
                ProcName = "dbo.GetDateTimeFix";

                var ResTimeFix = await DBS.GetReportRowAsync(ProcName, Params);
                return new JsonResult(ResTimeFix.ToJsonObject());
//                return new JsonResult(await DBS.GetReportA_EAsync(ProcName, Params, "DateFix", "TimeFix"));
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
        public async Task<IActionResult> OnGetFormSearchMokhatab(string KND, string TAKGRP)
        {// فرم جستجوی گیرنده
            try
            {
                ViewData["KND"] = KND;
                ViewData["TAKGRP"] = TAKGRP;
                // لیست گروه های اشخاص
                // لیست گروه های اشخاص
                Params = new Dictionary<string, object>
                {
                    { "@Lg","Y" }
                };
                ProcName = "dbo.PersonsGrp_List";
                ListGroup = await DBS.GetReportAsync(ProcName, Params);
                ViewData["Switch"] = "FormSearchMokhatab";
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

        public async Task<IActionResult> OnGetFormAddMokhatab(string HDID, string OWNER)
        {// فرم مدیریت مخاطبان پیام
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                ViewData["KND"] = "ADD";
                //ViewData["OWNER"] = OWNER;

                // لیست کاربران فعلی
                Params = new Dictionary<string, object>
                {
                    { "@Id", HDID},
                };
                ProcName = "dbo.MsgUsers_List";
                ListKarbaran = await DBS.GetReportAsync(ProcName, Params);

                // لیست گروه های اشخاص
                Params = new Dictionary<string, object>
                {
                    { "@Lg","Y" }
                };
                ProcName = "dbo.PersonsGrp_List";
                ListGroup = await DBS.GetReportAsync(ProcName, Params);

                ViewData["UserSi"] = userInfo.Si;
                ViewData["HDID"] = HDID;
                ViewData["Switch"] = "FormAddMokhatab";
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
        public async Task<IActionResult> OnGetFormUpdateMsg(string ID)
        {// فرم آپدیت متن پیام توسط نویسند
            try
            {
                Int32 UserId = Center.GetUserId(Request.Cookies["User.Sarmad.ir"]);
                ViewData["ID"] = ID;

                // متن پیام
                Params = new Dictionary<string, object>
                {
                    { "@User", UserId},
                    { "@Id", ID},
                }; 
                ProcName = "dbo.MsgDt_Info";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);
                ViewData["Switch"] = "FormUpdateMsg";
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
        public async Task<IActionResult> OnGetFormForwardMsg(string ID, string Writer)
        {// فرم فوروارد متن پیام توسط نویسند
            try
            {
                ViewData["IdMatnForward"] = ID;
                // متن پیام فوروارد شده
                Params = new Dictionary<string, object>
                {
                    { "@User", Writer},
                    { "@Id", ID},
                };
                ProcName = "dbo.MsgDt_Info";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);
                ViewData["Switch"] = "FormForwardMsg";
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
        public async Task<IActionResult> OnPostFindMsgHeader(string Onvan, string Member)
        {// پیدا کردن موضوعات بازی که این کاربر عضو است 
            try
            {
                Int32 UserId = Center.GetUserId(Request.Cookies["User.Sarmad.ir"]);
                // متن پیام
                Params = new Dictionary<string, object>
                {
                    { "@Id_User", UserId},
                    { "@Onvan", Onvan},
                    { "@Member", Member},
                };
                ProcName = "dbo.MsgHd_Find";
                ListData = await DBS.GetReportAsync(ProcName, Params);
                ViewData["Switch"] = "ListMsgHd";
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
        public async Task<IActionResult> OnPostForwardMsg(string IDMatnForward, string Mokhataban, string Onvan, string IdMsgHeader, string MsgMatn, string CTRLRoom)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                //ABCDE abc = new ABCDE();
                //abc.A = "توجه:";
                //if (CTRLRoom == "New")
                //{
                //    Onvan = Center.TrimTxt(Onvan);
                //    MsgMatn = Center.TrimTxt(MsgMatn);

                //    if (Mokhataban == "0,")
                //    {
                //        abc.A += " \n مخاطبان را انتخاب کنید";
                //    }
                //    if (Onvan == "")
                //    {
                //        abc.A += " \n موضوع پیام را بنویسید";
                //    }
                //    if (MsgMatn == "")
                //    {
                //        abc.A += " \n متن پیام را بنویسید";
                //    }
                //}
                //else
                //{
                //    if (IdMsgHeader == "0" & CTRLRoom != "New")
                //    {
                //        abc.A += " \n  موضوع یا اتاق گفتگو را انتخاب کنید";
                //    }
                //}


                Params = new Dictionary<string, object>
                {
                    { "@Id_Owner", userInfo.Si},
                    { "@Id_Ref", IDMatnForward},
                    { "@CTRLRoom", CTRLRoom},
                    { "@MsgHdId", IdMsgHeader},
                    { "@Id_Reciver", Mokhataban},
                    { "@Onvan", Onvan},
                    { "@Matn", MsgMatn},
                    { "@ext", ".non"},
                };
                ProcName = "dbo.MsgDt_Forward";
                var ForwardRES = await DBS.GetReportRowAsync(ProcName, Params);
                //// دارای برگشت برای ثبت پیوست
                //abc.A = "OK";
                //abc.B = RES["MsgHdId"].ToString();

                return new JsonResult(ForwardRES.ToJsonObject());
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

        public async Task<IActionResult> OnPostUpdateMsg(IFormFile FileUpload, string MsgMatn, string ID, string EXT)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                string ext = ".non";
                string folderName = @"Uploads\MsgCenter";
                if (FileUpload != null)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    folderName = Path.Combine(webRootPath, folderName);
                    ext = Path.GetExtension(FileUpload.FileName);
                    ext = ext.ToLower();
                }

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                    { "@Id", ID},
                    { "@Matn", MsgMatn},
                    { "@ext", ext},
                };
                ProcName = "dbo.MsgDt_Update";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);

                ViewData["MSG"] = RowData["message"].ToString();


                if (RowData["success"].ToString().ToUpper() == "OK")
                {
                    if (FileUpload != null)
                    {
                        var CodePeyvast = RowData["CodePeyvast"].ToString();

                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".pdf" || ext == ".txt" || ext == ".xls" || ext == ".xlsx" || ext == ".doc" || ext == ".docx" || ext == ".zip" || ext == ".rar")
                        {
                            string fullPath = Path.Combine(folderName, "P_" + ID + "_" + EXT);
                            Center.DeleteFileIfExists(fullPath);
                            fullPath = Path.Combine(folderName, "P_" + CodePeyvast);
                            Center.DeleteFileIfExists(fullPath);
                            // در اینجا تصاویر باید فشرده شوند
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                FileUpload.CopyTo(stream);
                            }
                        }
                        else
                        {
                            ViewData["MSG"] = "پیام اصلاح شد اما فایل آپلود نشد";
                        };
                    };
                };
                    

                ViewData["Switch"] = "MSG";

            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error";
            }

            return new PartialViewResult
            {
                ViewName = "_CommonView",
                ViewData = this.ViewData
            };

        }

        public async Task<IActionResult> OnPostAddMokhatab(string HDID, string Mokhatab)
        {// افزودن مخاطب به گفتگو
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_Owner", userInfo.Si},
                    { "@Id_Hd", HDID},
                    { "@Id_Mokhatab", Mokhatab},
                };
                ProcName = "dbo.MsgUsers_Add";
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

        public async Task<IActionResult> OnPostDeletMokhatab(string HDID, string Mokhatab)
        {// حذف مخاطب از گفتگو
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_Hd", HDID},
                    { "@Id_Mokhatab", Mokhatab},
                    { "@Id_User", userInfo.Si},
                };
                ProcName = "dbo.MsgUsers_Delete";
               // await DBS.TestProcedure(ProcName, Params);
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

        public async Task<IActionResult> OnPostDeleteMsgDt(string ID)
        {// حذف مطلب از گفتگو توسط نویسنده
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_User", userInfo.Si},
                    { "@Id", ID},
                };
                ProcName = "dbo.MsgDt_Delete";
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

        public async Task SendEmail(int msgID, int userId, string Writer, string MsgOnvan)
        {
            // آیدی پیام بررسی شود
            // مخاطبان با آدرس ایمیلشان گرفته شود
            // برای هرکدام یک ایمیل آلارم ارسال شود
            // در ایمیل لینکی برای مشاهده مستقیم پیام بدون ورود به سیستم گذاشته شود
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Id_Msg", msgID},
                    { "@Id_User", userId},
                };
                ProcName = "dbo.MsgUsers_ListEmail";
                ListKarbaran = await DBS.GetReportAsync(ProcName, Params);

                var emailService = new EmailService();
                foreach (DataRow item in ListKarbaran.Rows)
                {
                    try
                    {
                       
                        string email = item.Field<string>("Email");

                        if (MailAddress.TryCreate(email, out _))
                        {
                            string token =
                                Center.Encrypt(
                                    msgID + "_" + item["Si"]);

                            string url =
                                $"https://Sarmad.ir?MSG={token}";

                            await emailService.SendMessageAsync(
                                email,
                                item.Field<string>("FullName"),
                                url,
                                MsgOnvan,
                                Writer
                                );
                        }
                    }
                    catch (Exception ex)
                    {
                        await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        private async Task<string> CheckEtebar()
        {
            string Etebar = "0";
            ProcName = "dbo.SMSCenter";
            var SmsSetUp = await DBS.GetReportRowAsync(ProcName, null);

            string url = $"https://api.kavenegar.com/v1/{SmsSetUp["Api"].ToString()}/account/info.json";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(response);

                    Etebar = json["entries"]?["remaincredit"]?.ToString();
                    Etebar = (Convert.ToInt64(Etebar) / Convert.ToInt64(SmsSetUp["Fee"].ToString())).ToString();
                }
                catch (Exception ex)
                {
                    Etebar = "خطا: " + ex.Message;
                }
            }

            return Etebar;
        }

    }
}
