using Sarmad.Models;
using Sarmad.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Sarmad.Pages.Other
{
    public class HelpModel : PageModel
    {

        private string ProcName = ""; private Dictionary<string, object> Params;
        private string VU = "Help_View";
        public readonly int FormId = 8;

        public UserInfo userInfo = new UserInfo();
        public FormData formData;

        public DataTable ListForms;
        public DataTable ListPages;
        public DataTable ListDevs;
        public DataTable ListDoucs;
        public DataTable ListClumns;
        public DataTable ListPics;
        public DataRow RowData;


        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public HelpModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
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

                Params = new Dictionary<string, object>
                {
                    { "@Daste", "" },
                    { "@FRM", ""},
                    { "@Page", ""},
                };
                ProcName = "dbo.Forms_List";
                ListForms = await DBS.GetReportAsync(ProcName, Params);

                return Page();
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), FormId, MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToPage("/Khata", new { returnUrl = Url.PageLink() });
            }
        }

        public async Task<IActionResult> OnGetListForm(string FilterForm = "", string FilterPage = "", string Daste = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Daste", Daste },
                    { "@FRM", FilterForm},
                    { "@Page", FilterPage},
                };
                ProcName = "dbo.Forms_List";
                ListForms = await DBS.GetReportAsync(ProcName, Params);

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
        public async Task<IActionResult> OnGetFindMainHelp(string FormId)
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Si", FormId},
                };
                ProcName = "dbo.Forms_Show";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);

                ViewData["Switch"] = "PageSub";
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        public async Task<IActionResult> OnGetFindPage(string FormId, string FormName)
        {

            try
            {
                ViewData["FormName"] = FormName;
                Params = new Dictionary<string, object>
                {
                    { "@Id_Form", FormId},
                };
                ProcName = "dbo.FormsPage_List";
                ListPages = await DBS.GetReportAsync(ProcName, Params);

                Params = new Dictionary<string, object>
                {
                    { "@Id_Form", FormId},
                };
                ProcName = "dbo.FormsDev_List";
                ListDevs = await DBS.GetReportAsync(ProcName, Params);

                Params = new Dictionary<string, object>
                {
                    { "@Form", FormId},
                };
                ProcName = "dbo.DoctInfo_ListDev";
                ListDoucs = await DBS.GetReportAsync(ProcName, Params);

                ViewData["FormId"] = FormId;
                Params = new Dictionary<string, object>
                {
                    { "@Si", FormId},
                };
                ProcName = "dbo.Forms_Show";
                ViewData["FormAdres"] = await DBS.GetReportResultAsync(ProcName, Params, "NameForm");
                ViewData["Switch"] = "Pages";
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        public async Task<IActionResult> OnGetFindPageHelp(string ID = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Si", ID},
                };
                ProcName = "dbo.FormsPage_Show";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);

                ViewData["Switch"] = "PageDetail";
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
        public async Task<IActionResult> OnGetFindDev(string ID = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Si", ID},
                };
                ProcName = "dbo.FormsDev_Show";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);
                ViewData["Switch"] = "DevDetail";
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
        public async Task<IActionResult> OnGetFindDoc(string ID = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Si", ID},
                };
                ProcName = "dbo.DoctInfo_Info";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);

                if (RowData["Knd"].ToString() != "U" & RowData["Knd"].ToString() != "V")
                {
                    ViewData["Switch"] = "DocInfo";
                }
                else
                {
                    Params = new Dictionary<string, object>
                    {
                        { "@Table", RowData["Onvan"]},
                    };
                    ProcName = "dbo.TableColumns_Info";
                    ListClumns = await DBS.GetReportAsync(ProcName, Params);
                    if (ListClumns.Rows.Count == 0)
                    {
                        ViewData["Switch"] = "MSG";
                        ViewData["MSG"] = "جدول/ویو پیدا نشد";
                        return new PartialViewResult
                        {
                            ViewName = "_CommonView",
                            ViewData = this.ViewData
                        };
                    }

                    if(RowData["Knd"].ToString() == "U")
                        ViewData["Switch"] = "TableInfo";
                    else
                        ViewData["Switch"] = "ViewInfo";

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
        public async Task<IActionResult> OnPostColumnUpdate(string SI = "0", string TP = "")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", SI },
                    { "@Tp", TP },
                };
                ProcName = "dbo.TableColumns_Update";
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

        public async Task<IActionResult> OnPostHelpSaveHeader(string FormId, string FormHeader)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                if (Center.TrimTxt(FormHeader).Length > 1000)
                {
                    return Content("متن راهنما خیلی طولانی میباشد. حداکثر 1000 کاراکتر مجاز میباشد");
                }
                Params = new Dictionary<string, object>
                {
                    { "@Si", FormId },
                    { "@Header", FormHeader },
                    { "@Editor", userInfo.Si },
                };
                ProcName = "dbo.Forms_UpdateHeader";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        public async Task<IActionResult> OnPostHelpSaveTp(string FormId, string FormTp)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", FormId },
                    { "@Help", FormTp },
                    { "@Editor", userInfo.Si },
                };
                ProcName = "dbo.Forms_UpdateTp";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        public async Task<IActionResult> OnPostHelpSaveFooter(string FormId, string FormFooter)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                if (Center.TrimTxt(FormFooter).Length > 1000)
                {
                    return Content("متن راهنما خیلی طولانی میباشد. حداکثر 1000 کاراکتر مجاز میباشد");
                }

                Params = new Dictionary<string, object>
                {
                    { "@Si", FormId },
                    { "@Footer", FormFooter },
                    { "@Editor", userInfo.Si },
                };
                ProcName = "dbo.Forms_UpdateFooter";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }

        public async Task<IActionResult> OnGetListPics(string PageId = "0")
        {
            try
            {
                Params = new Dictionary<string, object>
                {
                    { "@Id_Page", PageId},
                };
                ProcName = "dbo.FormsPics_List";
                ListPics = await DBS.GetReportAsync(ProcName, Params);

                //Uri Urladdress = new Uri(Request.Host.ToString());
                ViewData["FilePath"] = "https://Sarmad.ir/Uploads/Help/Pic";
                ViewData["Page"] = PageId;
                ViewData["Switch"] = "Pictures";
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

        public async Task<IActionResult> OnPostSavePictur(IFormFile PhotoUpload, string PageId = "0", string Tp = "")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                if (PhotoUpload != null)
                {
                    string Temp = @"Upload\Help\Temp";
                    string folderName = @"Upload\Help";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    Temp = Path.Combine(webRootPath, Temp);
                    folderName = Path.Combine(webRootPath, folderName);

                    string ext = Path.GetExtension(PhotoUpload.FileName);
                    ext = ext.ToUpper();
                    if (ext == ".JPG" || ext == ".JPEG" || ext == ".PNG" || ext == ".BMP")
                    {
                        ext = ".webp";

                        Params = new Dictionary<string, object>
                        {
                            { "@Id_Page", PageId },
                            { "@Ext", ext },
                            { "@Tp", Tp},
                        };  
                        ProcName = "dbo.FormsPics_Add";
                        var SI = await DBS.GetReportResultAsync(ProcName, Params,"Si");

                        string fullPath = Path.Combine(folderName, "Pic_" + SI + ext);
                        Center.DeleteFileIfExists(fullPath);
                        Center.DeleteFileIfExists(Temp);

                        using (var stream = new FileStream(Temp, FileMode.Create))
                        {
                            PhotoUpload.CopyTo(stream);
                        }
                        ImageResizer _Img = new ImageResizer(500, 500);
                        await _Img.ResizeAsync(Temp, fullPath, "webp");

                    }
                    else
                    {
                        ViewData["Switch"] = "MSG";
                        ViewData["MSG"] = "تصویر ذخیره نشد. لطفا از تصاویر با فرمت JPG استفاده کنید";
                        return new PartialViewResult
                        {
                            ViewName = "_CommonView",
                            ViewData = this.ViewData
                        };
                    }
                }
                else
                {
                    ViewData["Switch"] = "MSG";
                    ViewData["MSG"] = "فایل تصویری را انتخاب کنید";
                    return new PartialViewResult
                    {
                        ViewName = "_CommonView",
                        ViewData = this.ViewData
                    };
                }

                Params = new Dictionary<string, object>
                {
                    { "@Id_Page", PageId},
                };
                ProcName = "dbo.FormsPics_List";
                ListPics = await DBS.GetReportAsync(ProcName, Params);

                ViewData["Page"] = PageId;
                ViewData["Switch"] = "ListPictures";
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
        public async Task<IActionResult> OnPostPhotoDelete(string Id = "0", string PhotoExt = ".jpg")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");
                // حذف از دیتابیس
                Params = new Dictionary<string, object>
                {
                    { "@Si", Id},
                    { "@Id_User", userInfo.Si},
                };
                ProcName = "dbo.FormsPics_Delete";
                await DBS.RunCommandAsync(ProcName, Params);

                // حذف فیزیکی فایل تصویر
                string folderName = @"Upload\Help\Pic_" + Id + PhotoExt;
                string webRootPath = _hostingEnvironment.WebRootPath;
                string PicPath = Path.Combine(webRootPath, folderName);
                Center.DeleteFileIfExists(PicPath);
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
        public async Task<IActionResult> OnPostHelpNameSave(string FormId, string Reg, string Onvan, string Tp)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", FormId },
                    { "@Reg", Reg },
                    { "@Onvan", Onvan},
                    { "@Tp", Tp},
                    { "@Editor", userInfo.Si},
                };
                ProcName = "dbo.Forms_UpdateName";
                await DBS.RunCommandAsync(ProcName, Params);
                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
                return new PartialViewResult
                {
                    ViewName = VU,
                    ViewData = this.ViewData
                };
            }
        }

        public async Task<IActionResult> OnPostPageSaveHeader(string PageId = "0", string HeaderTp = "")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");
                if (Center.TrimTxt(HeaderTp).Length > 2000)
                {
                    return Content("متن راهنمای هدر خیلی طولانی میباشد. حداکثر 2000 کاراکتر مجاز میباشد");
                }

                Params = new Dictionary<string, object>
                {
                    { "@Si", PageId },
                    { "@Header", HeaderTp},
                    { "@Editor", userInfo.Si},
                };  

                ProcName = "dbo.FormsPage_UpdateHeader";
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
        public async Task<IActionResult> OnPostPageSaveTp(string PageId = "0", string FormTp = "")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", PageId },
                    { "@Help", FormTp},
                    { "@Editor", userInfo.Si},
                };
                ProcName = "dbo.FormsPage_UpdateTp";
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
        public async Task<IActionResult> OnPostPageSaveDev(string ID = "0", string TP = "")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", ID },
                    { "@Editor", userInfo.Name },
                    { "@Dev", TP },
                };
                ProcName = "dbo.FormsDev_Update";
                await DBS.RunCommandAsync(ProcName, Params);

                Params = new Dictionary<string, object>
                {
                    { "@Si", ID},
                };
                ProcName = "dbo.FormsDev_Show";
                RowData = await DBS.GetReportRowAsync(ProcName, Params);

                ViewData["Switch"] = "DevDetail";
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

        public async Task<IActionResult> OnPostUpdateOnvanRj(string PageId, string PageOnvan, string PageRj, string Active)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", PageId },
                    { "@RJ", PageRj },
                    { "@Active", Active },
                    { "@Onvan", PageOnvan},
                    { "@Editor", userInfo.Si},
                };
                ProcName = "dbo.FormsPage_UpdateOnvanRj";
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
        public async Task<IActionResult> OnPostPageDelete(string PageId = "0")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Si", PageId},
                    { "@Id_User", userInfo.Si },
                };
                ProcName = "dbo.FormsPage_Delete";
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
        public async Task<IActionResult> OnPostAddNewPage(string FormId, string PageName)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_Form", FormId },
                    { "@Name", PageName},
                    { "@Editor", userInfo.Si},
                };

                ProcName = "dbo.FormsPage_Add";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        public async Task<IActionResult> OnPostAddNewOnvanDev(string FormId, string Onvan)
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "@Id_Form", FormId },
                    { "@Onvan", Onvan},
                };
                ProcName = "dbo.FormsDev_Add";
                await DBS.RunCommandAsync(ProcName, Params);

                return Content("OK");
            }
            catch (Exception ex)
            {
                ViewData["ERRComm"] = await DBS.LogErrorSaveAsync(ProcName, Params, Center.GetUserId(Request.Cookies["User.Sarmad.ir"]), int.Parse(FormId), MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewData["ERRRes"] = ex.Message;
                ViewData["Switch"] = "Error"; VU = "_CommonView";
            }
            return new PartialViewResult
            {
                ViewName = VU,
                ViewData = this.ViewData
            };
        }
        public async Task<IActionResult> OnPostDocSave(string SI = "0", string TP = "0")
        {
            try
            {
                if (! await DBS.CheckToken(HttpContext, userInfo))
                    return RedirectToPage("/LoginFast");

                Params = new Dictionary<string, object>
                {
                    { "Si", SI},
                    { "Tp", TP},
                };
                ProcName = "dbo.DoctInfo_Update";
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

    }
}
