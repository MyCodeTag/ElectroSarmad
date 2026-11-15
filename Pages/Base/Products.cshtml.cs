using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sarmad.Models;
using Sarmad.Services;
using System;
using System.Data;
using System.Reflection;

namespace Sarmad.Pages.Base
{
    public class ProductsModel : PageModel
    {
        private string ProcName = ""; private Dictionary<string, object>? Params;
        private string VU = "./Products_View";
        public Int32 FormId = 6;

        public UserInfo userInfo = new UserInfo();
        public required FormData formData { get; set; }
        public required DataTable ListProducts { get; set; }
        public required DataTable ListData { get; set; }
        public required DataTable ListForms { get; set; }
        public required DataTable ListCategoris { get; set; }
        public required DataTable ListUnits { get; set; }
        public required DataRow RowData { get; set; }

        private IWebHostEnvironment _hostingEnvironment;
        public ProductsModel(IWebHostEnvironment hostingEnvironment)
        {// مسیر روت برنامه
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                await DBS.CheckToken(HttpContext, userInfo);
                if (userInfo.Active == "N") return RedirectToPage("/Login", new { returnUrl = Url.PageLink()});

                formData = await DBS.GetFormData(userInfo.Si, FormId);

                return Page();
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }

        public async Task<IActionResult> OnGetListProducts(string Act, string Filter, string Grp, string Vahed, string Sort)
        {// لیست محصولات
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Act", Act },
                    { "@Category", Grp },
                    { "@Vahed", Vahed },
                    { "@Filter", Filter },
                    { "@Sort", Sort },
                };
                ProcName = "dbo.Products_List";
                ListProducts = await DBS.GetReportAsync(ProcName, Params);

                if (ListProducts.Rows.Count == 0)
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = "کالایی با مشخصات فوق پیدا نشد. شرایط جستجو را تغییر بدهید و دوباره گزارش بگیرید";

                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                };

                ViewData["Switch"] = "ListProduct";

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

        public async Task<IActionResult> OnGetFormSabtProduct(string ID)
        {// فرم ثبت دیتای محصول
            try
            {
                ViewData["ID"] = ID;

                // لیست طبقه ها
                Params = new Dictionary<string, object>
                {
                    { "@Filter","" }
                };
                ProcName = "dbo.Categories_List";
                ListCategoris = await DBS.GetReportAsync(ProcName, Params);

                // لیست واحدها
                ProcName = "dbo.ComDt_ProdVahed_List";
                ListUnits = await DBS.GetReportAsync(ProcName, null);

                if (ID != "0")
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Si" , ID } ,
                    };
                    ProcName = "dbo.Products_Info";
                    RowData = await DBS.GetReportRowAsync(ProcName, Params);
                }
                else
                {
                    Params = null;
                    ProcName = "dbo.Products_NewCu";
                    ViewData["NewCu"] = await DBS.GetReportResultAsync(ProcName, Params, "NewCu");

                }

                ViewData["Switch"] = "FormSabtProduct";
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

        public async Task<IActionResult> OnPostUpdateProduct(string ID, string Cu, string Tp, string Sharh, string Act
                        , string Info, string Category, string Vahed)
        {//ثبت کالا
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si_Editor", userInfo.Si } ,
                    { "@Id", ID },
                    { "@Cu_Product", Cu },
                    { "@Tp_Product", Tp },
                    { "@Sharh_Product", Sharh },
                    { "@Info_Product", Info },
                    { "@Si_Category" , Category } ,
                    { "@Si_Vahed" , Vahed } ,
                    { "@IsActive" , Act } ,
                };

                ProcName = "dbo.Products_Update";
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
        public async Task<IActionResult> OnPostDeleteProduct(string ID)
        {//حذف کالا
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@ID", ID },
                };

                ProcName = "dbo.Products_Delete";

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

        public async Task<IActionResult> OnGetFormSabtBox(string ID)
        {// فرم بسته بندی های یک کالا
            try
            {
                ViewData["ID"] = ID;

                Params = new Dictionary<string, object>
                {
                    { "@Si_Product" , ID }
                };
                ProcName = "dbo.ProductUnits_List";
                ListUnits = await DBS.GetReportAsync(ProcName, Params);


                ViewData["Switch"] = "FormSabtBox";
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

        public async Task<IActionResult> OnPostSabtBox(string ProdID, string Title, string Zarib)
        {// ثبت یک بسته بندی
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si_Editor" , userInfo.Si },
                    { "@ProductId" , ProdID },
                    { "@Title" , Title },
                    { "@Zarib" , Zarib },
                };
                ProcName = "dbo.ProductUnits_Add";
                var AddRes = await DBS.GetReportRowAsync(ProcName, Params);
                if (AddRes["success"].ToString().ToUpper() == "OK")
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Si_Product" , ProdID  }
                    };
                    ProcName = "dbo.ProductUnits_List";
                    ListUnits = await DBS.GetReportAsync(ProcName, Params);
                    ViewData["Switch"] = "ListSabtBox";
                }
                else
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = AddRes["message"].ToString();

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

        public async Task<IActionResult> OnPostDeleteBox(string ID)
        {// حذف یک بسته بندی
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si_Editor" , userInfo.Si },
                    { "@ID" , ID },
                };
                ProcName = "dbo.ProductUnits_Delete";
                var DeletRes = await DBS.GetReportRowAsync(ProcName, Params);

                return Content(DeletRes["message"].ToString());

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



        public async Task<IActionResult> OnGetListCategories(string Filter, string Sort)
        {// لیست طبقات
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Filter", Filter },
                    { "@Sort", Sort },
                };
                ProcName = "dbo.Categories_List";

                ListCategoris = await DBS.GetReportAsync(ProcName, Params);
                if (ListCategoris.Rows.Count == 0)
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = "طبقه کالا با مشخصات فوق پیدا نشد. شرایط جستجو را تغییر بدهید و دوباره گزارش بگیرید";

                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }
                ;

                ViewData["Switch"] = "ListCategory";

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

        public async Task<IActionResult> OnGetFormSabtCategory(string ID = "0")
        {// فرم ثبت دیتای طبقه کالا
            try
            {
                ViewData["ID"] = ID;

                if (ID != "0")
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Si" , ID } ,
                    };
                    ProcName = "dbo.Categories_Info";
                    RowData = await DBS.GetReportRowAsync(ProcName, Params);
                }
                else
                {
                    Params = null;
                    ProcName = "dbo.Categories_NewCu";
                    ViewData["NewCu"] = await DBS.GetReportResultAsync(ProcName, Params, "NewCu");

                }

                Params = new Dictionary<string, object>
                {
                    { "@Filter" , "" } ,
                };
                ProcName = "dbo.Categories_List";
                ListCategoris = await DBS.GetReportAsync(ProcName, Params);


                ViewData["Switch"] = "FormSabtCategory";
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

        public async Task<IActionResult> OnPostUpdateCategory(string ID, string Cu, string Tp, string Father)
        {//ثبت طبقه
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si_Editor", userInfo.Si } ,
                    { "@Id", ID },
                    { "@Cu", Cu },
                    { "@Tp", Tp },
                    { "@Father", Father },
                };

                ProcName = "dbo.Categories_Update";
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
        public async Task<IActionResult> OnPostDeleteCategory(string ID)
        {//حذف طبقه
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@ID_User", userInfo.Si },
                    { "@ID", ID },
                };

                ProcName = "dbo.Categories_Delete";
                var DeleteRes = await DBS.GetReportRowAsync(ProcName, Params);
                return Content(DeleteRes["message"].ToString());

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

        public async Task<IActionResult> OnGetFormSabtPic(string ID, string PicOld)
        {// فرم ثبت دیتای تصویر کالا
            try
            {
                ViewData["ID"] = ID;

                ViewData["PicOld"] = (Center.TrimTxt(PicOld) == "") ? "P_0.jpg" : $"P_{ID}_{PicOld}";

                ViewData["Switch"] = "FormSabtPic";
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

        public async Task<IActionResult> OnPostUpLoadPic(IFormFile PicUpload, string OldPic, string ID)
        {
            try
            {
                if (!await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                string folder = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads", "Products");

                string ext = Path.GetExtension(PicUpload.FileName).ToUpperInvariant();
                Random random = new Random();
                int Scr = random.Next(1000, 9999);

                if (ext is ".JPG" or ".JPEG" or ".PNG" or ".BMP")
                {
                    string fileName = Center.TrimTxt(OldPic);
                    string path = Path.Combine(folder, fileName);
                    if (fileName != "P_0.jpg")
                    {
                        Center.DeleteFileIfExists(path);
                    }

                    fileName = $"P_{ID}_{Scr}{ext}";
                    path = Path.Combine(folder, fileName);

                    // ذخیره موقت
                    string Temp = Path.Combine(folder, "temp");
                    Center.DeleteFileIfExists(Temp);
                    using (var stream = new FileStream(Temp, FileMode.Create))
                    {
                        PicUpload.CopyTo(stream);
                    }

                    // تغییر سایز تصویر
                    ImageResizer _Img = new ImageResizer(800, 800);
                    await _Img.ResizeAsync(Temp, path, "webp");

                    // اصلاح نام تصویر برای کالا
                    string ScrExt = $"{Scr}{ext}";
                    Params = new Dictionary<string, object>
                    {
                        { "@Si_Editor", userInfo.Si },
                        { "@Id", ID},
                        { "@Pic", ScrExt},
                    };
                    ProcName = "dbo.Products_UpdatePic";
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
