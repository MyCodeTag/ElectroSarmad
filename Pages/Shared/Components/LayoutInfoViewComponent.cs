using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using Org.BouncyCastle.Asn1.X509;

namespace Sarmad.Pages.Shared.Components
{
    public class LayoutInfoViewComponent : ViewComponent
    {
        private FormSetup FormSet = new FormSetup();
        private DataTable ListYadavar;
        private DataTable ListNewMsg;
        private List<ListForm> ListForms = new List<ListForm>();


        public async Task<IViewComponentResult> InvokeAsync(Int32 FormId, UserInfo userInfo)
        {
            var data = await GetLayoutDataAsync(FormId, userInfo); // تابعی برای دریافت اطلاعات پویا
            return View("HeaderData", data);
        }
        private async Task<LayoutDataModel> GetLayoutDataAsync(Int32 FormId, UserInfo userInfo)
        {

            await FormInfo(FormId, userInfo);
            var parametrYaddash = new Dictionary<string, object>
            {
                { "@Id_User", userInfo.Si }
            };

            ListYadavar = await DBS.GetReportAsync("Yaddasht_Yadavar", parametrYaddash);

            ListNewMsg = await DBS.GetReportAsync("MsgDt_ListNew", parametrYaddash);

            ViewData["Script"] = FormSet.Script;
            ViewData["Titel"] = FormSet.Title;
          
            var model = new LayoutDataModel
            { 
                userInfo = userInfo,
                formSetup = FormSet,
                listForms = ListForms,
                listYadavar = ListYadavar,
                listNewMsg = ListNewMsg,
            };

            HttpContext.Items["LayoutData"] = model;

            return model;
        }

        private async Task FormInfo(Int32 FormId, UserInfo userInfo)
        { 
          // لیست فرمهایی که به کاربر تخصیص داده شده است
          // دیتای ثابت روی فرم
            FormSet.FormId = FormId;
            FormSet.Script = $"~/js/page_{FormId}.js";
            FormSet.UserId = userInfo.Si;
            FormSet.UserName = userInfo.Name;

            FormSet.BaseIcon = "icofont-bubble-down";
            FormSet.BaseArea = "dis-none";
            FormSet.DailyIcon = "icofont-bubble-down";
            FormSet.DailyArea = "dis-none";
            FormSet.ReportIcon = "icofont-bubble-down";
            FormSet.ReportArea = "dis-none";
            FormSet.OtherIcon = "icofont-bubble-down";
            FormSet.OtherArea = "dis-none";

            // لیست دسته بندی منوهایی که به کاربر تخصیص داده شده است؟
            var parametrs = new Dictionary<string, object>
            {
                { "@Id_Karbar", userInfo.Si }
            };

            var listDasteTBL = await DBS.GetReportAsync("[dbo].[KarbaranForms_Daste]", parametrs);
            foreach (DataRow item in listDasteTBL.Rows)
            {
                switch (item["Daste"].ToString())
                {
                    case "D":
                        FormSet.Daily = "OK";
                        break;
                    case "R":
                        FormSet.Report = "OK";
                        break;
                    case "B":
                        FormSet.Base = "OK";
                        break;
                    case "O":
                        FormSet.Other = "OK";
                        break;
                }
            };





            ListForm _Frm;
            // لیست فرمهایی که به کاربر تخصیص داده شده است
            var listFormTBL = await DBS.GetReportAsync("KarbaranForms_Active", parametrs);
            foreach (DataRow item in listFormTBL.Rows)
            {
                _Frm = new ListForm()
                {
                    Daste = item["Daste"].ToString(),
                    Name = item["Onvan"].ToString(),
                    Adres = item["NameForm"].ToString(),
                    Css = "menu-ItemPre",
                };

                if (item.Field<Int32>("Si") == FormId)
                {
                    _Frm.Css = "menu-Item";
                    FormSet.Name = item["NameForm"].ToString();
                    FormSet.Title = item["Onvan"].ToString();
                    FormSet.Header = item["Header"].ToString();
                    FormSet.Footer = item["Footer"].ToString();


                    switch (item["Daste"].ToString())
                    {
                        case "B":
                            FormSet.BaseIcon = "icofont-bubble-up";
                            FormSet.BaseArea = "";
                            break;
                    }

                    switch (item["Daste"].ToString())
                    {
                        case "D":
                            FormSet.DailyIcon = "icofont-bubble-up";
                            FormSet.DailyArea = "";
                            break;
                    }

                    switch (item["Daste"].ToString())
                    {
                        case "R":
                            FormSet.ReportIcon = "icofont-bubble-up";
                            FormSet.ReportArea = "";
                            break;
                    }

                    switch (item["Daste"].ToString())
                    {
                        case "O":
                            FormSet.OtherIcon = "icofont-bubble-up";
                            FormSet.OtherArea = "";
                            break;
                    }
                }

                ListForms.Add(_Frm);
            }


            //// ثبت آخرین فرم ورودی و گرفتن تعداد پیامهای باز کاربر
            //var parametrsLast = new Dictionary<string, object>
            //{
            //    { "@Id_Person", userInfo.Si },
            //    {"@Form",FormId }
            //};

            
           // FormSet.MSG = CNT["Cnt"].ToString();
            FormSet.Today = await DBS.Emruzasync();
        }
    }
}
