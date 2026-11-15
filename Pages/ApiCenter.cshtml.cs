using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http.Json;

namespace Sarmad.Pages
{
    public class ApiCenterModel : PageModel
    {


        private string ProcName = ""; private Dictionary<string, object> Params;
        public readonly int FormId = 118;
        private string VU = "ApiCenter_View";
        public UserInfo userInfo = new UserInfo();

        public DataTable ListData;
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;

        public ApiCenterModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnGetHelpShow(string ID = "0")
        {
            try
            {
                int RJ = 0;
                //******** راهنمای کلی اولیه
                Params = new Dictionary<string, object>
                {
                    { "@Si", ID},
                };
                ProcName = "dbo.Forms_Help";
                var formInfo = await DBS.GetReportRowAsync(ProcName, Params);
                //abcde = new ABCDE
                //{
                //    RJ = formInfo["Si"].ToString(),
                //    A = formInfo["Onvan"].ToString(),
                //    B = formInfo["Help"].ToString(),
                //    E = RJ.ToString()
                //};

                //AE_List.Add(abcde);

                //********  راهنمای صفحات
                Params = new Dictionary<string, object>
                {
                    { "@Si", ID},
                };
                ProcName = "dbo.Forms_HelpPages";
                var listPage = await DBS.GetReportAsync(ProcName, Params);
                foreach (DataRow DRow in listPage.Rows)
                {
                    RJ += 1;
                    //abcde = new ABCDE()
                    //{
                    //    RJ = DRow["Si"].ToString(),
                    //    A = DRow["OnvanPage"].ToString(),
                    //    B = DRow["HelpPage"].ToString(),
                    //    E = RJ.ToString()
                    //};

                    //AE_List.Add(abcde);
                }



                ViewData["Switch"] = "Help";
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
        public async Task<IActionResult> OnGetLimitDate(string LimitDateKnd = "0")
        {
            LimitDate _LimitDate;
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@LimitDate", LimitDateKnd},
                };
                ProcName = "dbo.LimitDateFind";
                var DlimitDate = await DBS.GetReportRowAsync(ProcName, Params);
                {
                    _LimitDate = new LimitDate()
                    {
                        FromDate = DlimitDate["FromDate"].ToString(),
                        ToDate = DlimitDate["ToDate"].ToString(),
                    };
                };
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
                _LimitDate = new LimitDate()
                {
                    FromDate = "خطا",
                    ToDate = "خطا",
                };
            }
            return new JsonResult(_LimitDate);
        }
        public async Task<IActionResult> OnGetCheckNewMsg(string UserId = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Id_User", UserId},
                };
               ProcName = "dbo.MsgDt_CountNew";
                var res = await DBS.GetReportRowAsync(ProcName,Params);
                return Content(res["Cnt"].ToString());
            }
            catch
            {
                return Content("0");
            }
        }

        public async Task<IActionResult> OnPostChangMsgLg(string Id = "0", string UserID="0")
        { // تغییر وضعیت یک پیام از خوانده شده به نشده و بالعکس 
            try
            {
                Params = new Dictionary<string, object>
                {
                    {"@Id_User", UserID},
                    {"@Id_Dt", Id},
                };
                ProcName = "dbo.MsgDt_Read";
                await DBS.RunCommandAsync(ProcName,Params);
                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return Content("OK");
            }

        }
    }
}
