using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace Sarmad.Pages
{
    public class HomeModel : PageModel
    {
        private string ProcName = ""; private Dictionary<string, object> Params;
        private string VU = "Home_View";
        public readonly int FormId = 1;

        public UserInfo userInfo = new UserInfo();
        public FormData formData;

        public DataTable ListLastForms;

        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public HomeModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> OnGet(string update = "", string ID = "")
        {
            try
            {
                await DBS.CheckToken(HttpContext, userInfo);
                if (userInfo.Active == "N") return RedirectToPage("/Login", new { returnUrl = Url.PageLink() });


                formData = await DBS.GetFormData(userInfo.Si, FormId);

                //                ViewData["UpdateSatrap"] = update;
                //                ViewData["SatrapId"] = ID;

                // کمتر از ده فرم - جستحو را نشان ندهد
                Params = new Dictionary<string, object>
                {
                    { "@Id_Karbar", userInfo.Si },
                };
                ProcName = "dbo.KarbaranForms_Cnt";
                var cnt = await DBS.GetReportResultAsync(ProcName, Params, "CNT");
                if (int.Parse(cnt) < 11)
                    ViewData["HiddenSearch"] = "hidden";
                else
                    ViewData["HiddenSearch"] = "";

                return Page();
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }

        public async Task<IActionResult> OnPostListForm(string FilterForm = "", string FilterPage = "")
        {
            try
            {
                Center.GetUserData(Center.TrimTxt(Request.Cookies["User.Sarmad.ir"]), userInfo);

                Params = new Dictionary<string, object>
                {
                    { "@Karbar", userInfo.Si },
                    { "@FRM", FilterForm},
                    { "@Page", FilterPage},
                };
                ProcName = "dbo.Forms_ListKarbarAll";
                
                ListLastForms = await DBS.GetReportAsync(ProcName,Params);

                ViewData["Switch"] = "Forms";
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


        private void Report()
        {
            /*
            ComText = "Exec [dbo].[Report_Home] '" + userInfo.Si + "', '" + userInfo.Knd + "'";
            var DS = await DBS.GetReportAsync(ProcName, Params);
            {
                DataRow DRow = DS.Tables["RES"].Rows[0];

                _Report = new Rep()
                {
                    Rep_10 = DRow["Rep_10"].ToString(),
                    Rep_11 = DRow["Rep_11"].ToString(),
                    Rep_12 = DRow["Rep_12"].ToString(),
                    Rep_13 = DRow["Rep_13"].ToString(),
                    Rep_14 = DRow["Rep_14"].ToString(),
                    Rep_15 = DRow["Rep_15"].ToString(),
                    Rep_16 = DRow["Rep_16"].ToString(),
                    Rep_17 = DRow["Rep_17"].ToString(),
                    Rep_18 = DRow["Rep_18"].ToString(),
                    Rep_19 = DRow["Rep_19"].ToString(),
                    Rep_20 = DRow["Rep_20"].ToString(),
                    Rep_21 = DRow["Rep_21"].ToString(),
                    Rep_22 = DRow["Rep_22"].ToString(),
                    Rep_23 = DRow["Rep_23"].ToString(),
                    Rep_24 = DRow["Rep_24"].ToString(),
                    Rep_25 = DRow["Rep_25"].ToString(),
                    Rep_26 = DRow["Rep_26"].ToString(),
                    Rep_27 = DRow["Rep_27"].ToString(),
                    Rep_28 = DRow["Rep_28"].ToString(),
                    Rep_29 = DRow["Rep_29"].ToString(),
                    Rep_30 = DRow["Rep_30"].ToString(),
                    Rep_Tp = DRow["Rep_Tp"].ToString(),
                };
            
            }
            ViewData["Switch"] = "KND_" + userInfo.Knd;
            if (userInfo.Knd == "2" || userInfo.Knd == "6" || userInfo.Knd == "7")
            {
                ViewData["Switch"] = "KND_267";
            }
    */
            ViewData["Switch"] = "KND";
        }

    }
}