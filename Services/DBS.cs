using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sarmad.Models;
using System.Collections.Concurrent;
using System.Data;

namespace Sarmad.Services
{
    public class DBS
    {

        private static string? _DbsSarmad;


        static DBS()
        {
            string environment =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Production";

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);


            IConfiguration config = builder.Build();

           _DbsSarmad = config.GetConnectionString("DbsSarmad");

            //            _DbsSarmad = config.GetConnectionString("payoncoi_Sarmad");
        }

        private static SqlConnection GetSarmadConnection()
        {
            return new SqlConnection(_DbsSarmad);
        }

        private static readonly ConcurrentDictionary<string, SqlParameter[]> _cache
                                = new ConcurrentDictionary<string, SqlParameter[]>();
        public static SqlParameter[] GetCachedParameters(SqlConnection conn, string storedProcedureName)
        {  // کش کردن پارامترهای پروسجورها برای تشخیص نوع و سایز پارامتر پروسجور
            string cacheKey = $"{conn.ConnectionString}:{storedProcedureName}";

            return _cache.GetOrAdd(cacheKey, _ =>
            {
                // اگر در کش نبود، از دیتابیس می‌خونیم
                using var cmd = new SqlCommand(storedProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;

                SqlCommandBuilder.DeriveParameters(cmd);

                // فقط ساختار پارامترها، بدون ReturnValue
                var discoveredParams = cmd.Parameters
                    .Cast<SqlParameter>()
                    .Where(p => p.Direction != ParameterDirection.ReturnValue)
                    .Select(p =>
                    {
                        var clone = (SqlParameter)((ICloneable)p).Clone();
                        clone.Value = DBNull.Value; // مهم: مقدار خالی
                        return clone;
                    })
                    .ToArray();

                return discoveredParams;
            })
            // ⚠️ مهم: اینجا دوباره Clone می‌کنیم که نسخه کش دستکاری نشه
            .Select(p => (SqlParameter)((ICloneable)p).Clone())
            .ToArray();
        }

        public static async Task RunCommandAsync(string procName, Dictionary<string, object>? inputValues = null)
        {
            using (SqlConnection conn = GetSarmadConnection())
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(); 

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60;

                    var derivedParams = GetCachedParameters(conn, procName);
                    cmd.Parameters.AddRange(derivedParams);

                    if (inputValues != null)
                    {
                        foreach (SqlParameter param in cmd.Parameters)
                        {
                            // فقط برای پارامترهای ورودی مقدار بگذار
                            if (param.Direction == ParameterDirection.Input)
                            {
                                if (inputValues != null && inputValues.TryGetValue(param.ParameterName, out object value))
                                    param.Value = value ?? DBNull.Value;   // ✅ درست
                                else
                                    param.Value = DBNull.Value;            // ✅ درست
                            }
                        }
                    }

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public static async Task<string> GetReportResultAsync(string procName, Dictionary<string, object>? inputValues = null, string Res = "")
        {
            var rowData = await GetReportRowAsync(procName, inputValues);
            return rowData[Res].ToString();
        }


        //public static async Task<DataTable> GetReportDataAsync(string ComText)
        //{
        //    DataTable repTable = new DataTable("RES");

        //    using (SqlConnection conn = GetSarmadConnection())
        //    {
        //        if (conn.State != ConnectionState.Open)
        //            await conn.OpenAsync();

        //        using SqlCommand cmd = new SqlCommand(ComText, conn);

        //        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        //        {
        //            repTable.Load(reader);
        //        }
        //    }
        //    return repTable;
        //}

        public static async Task<DataRow> GetReportRowAsync(string procName, Dictionary<string, object>? inputValues = null)
        {
            var listData = await DBS.GetReportAsync(procName, inputValues);

            return listData.Rows.Count > 0 ? listData.Rows[0] : null;
        }
        public static async Task<DataTable> GetReportAsync(string procName, Dictionary<string, object>? inputValues = null)
        {
            DataTable repTable = new DataTable("RES");

            using (SqlConnection conn = GetSarmadConnection())
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 180;

                    var derivedParams = GetCachedParameters(conn, procName);
                    cmd.Parameters.AddRange(derivedParams);

                    if (inputValues != null)
                    {
                        foreach (SqlParameter param in cmd.Parameters)
                        {
                            // فقط برای پارامترهای ورودی مقدار بگذار
                            if (inputValues.TryGetValue(param.ParameterName, out object value))
                                param.Value = value ?? DBNull.Value;
                            else
                                param.Value = DBNull.Value;
                        }
                    }

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        repTable.Load(reader);
                    }

                }
            }
            return repTable;
        }

        public static async Task<DataTable> GetReportDataAsync(string ComText)
        {
            DataTable repTable = new DataTable("RES");

            using (SqlConnection conn = GetSarmadConnection())
            {
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand(ComText, conn);
                cmd.CommandTimeout = 180;
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    repTable.Load(reader);
                }
            }
            return repTable;
        }

        public static async Task LogErrorSaveCmdAsync(string ComText, int userId, int Frm, string Prc, string Err)
        { // ذخیره خطا
            var mainparameters = new Dictionary<string, object>
            {
                { "@Si_User", userId },
                { "@SI_Frm", Frm },
                { "@Prc", Prc },
                { "@Comm", ComText },
                { "@Err", Err }
            };
            await RunCommandAsync("LogError_Save", mainparameters);
        }
        public static async Task<string> LogErrorSaveAsync(string procName, Dictionary<string, object>? parameters = null, int userId = 0, int Frm = 0, string Prc = "", string Err = "")
        { // ذخیره خطا
            string ComText = Center.CommandText(procName, parameters);
            await LogErrorSaveCmdAsync(ComText, userId, Frm, Prc, Err);

            return ComText;
        }
        public static async Task TestProcedure(string procName, Dictionary<string, object>? parameters = null)
        { // متن دستور SQL برای اجرا
            await LogErrorSaveAsync(procName, parameters, 0, 0, "", "Test Procedure");
        }


        public static async Task<bool> CheckToken(HttpContext httpContext, UserInfo userInfo)
        {
            // default
            userInfo.Active = "N";
            userInfo.Si = 0;

            if (httpContext == null) return false;

            try
            {
                var cookies = httpContext.Request.Cookies;
                if (!cookies.TryGetValue("Sarmad.ir", out var rawToken) || string.IsNullOrWhiteSpace(rawToken))
                    return false;

                // Prevent overly long token (DoS / malformed)
                if (rawToken.Length > 1024) // adjust limit as appropriate
                    return false;


                if (string.IsNullOrWhiteSpace(rawToken) || !Guid.TryParse(rawToken, out Guid authToken))
                    return false;

                Dictionary<string, object> Params = new Dictionary<string, object>
                {
                    {"@Token", authToken}
                };
                var ProcName = "dbo.CheckToken";


                DataRow chektoken;
                try
                {
                    chektoken = await GetReportRowAsync(ProcName, Params);
                }
                catch (Exception ex)
                {
                    // لاگ خطا بهتر است اینجا ثبت شود (LogErrorSaveAsync یا ILogger)
                    // await LogErrorSaveAsync(...);
                    return false;
                }
                if (chektoken == null) return false;

                // بررسی وضعیت فعال بودن توکن
                var active = chektoken["IsActive"]?.ToString();
                if (!active.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    userInfo.Si = 0;
                    userInfo.Active = "N";
                    return false;
                }

                userInfo.Active = "Y";
                userInfo.Si = int.Parse(chektoken["Si"].ToString());
                userInfo.Name = chektoken["FullName"].ToString();
                userInfo.UserName = chektoken["UserName"].ToString();
                userInfo.Knd = chektoken["Kind"].ToString();
                return true;
            }
            catch
            {
                userInfo.Active = "N";
                userInfo.Si = 0;
                return false;
            }
        }

        //public static async Task<bool> CheckToken(string Cooki, UserInfo userInfo)
        //{ // کنترل اینکه آیا کاربر لاگین کرده است؟
        //    try
        //    {

        //        userInfo.Token = Cooki.ToString();
        //        Dictionary<string, object> Param = new Dictionary<string, object>
        //        {
        //            { "@Token",userInfo.Token},
        //        };
        //        var chektoken = await GetReportRowAsync("CheckToken", Param);
        //        if (chektoken != null) {
        //            if (chektoken["IsActive"].ToString() == "Y")
        //            {
        //                userInfo.Active = chektoken["IsActive"].ToString();
        //                userInfo.Si = int.Parse(chektoken["Si"].ToString());
        //                userInfo.Name = chektoken["FullName"].ToString();
        //                userInfo.UserName = chektoken["UserName"].ToString();
        //                userInfo.Knd = chektoken["Kind"].ToString();
        //                return  true;
        //            }
        //            else
        //            {
        //                userInfo.Active = "N";
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            userInfo.Si = 0;
        //            userInfo.Active = "N";
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        userInfo.Si = 0;
        //        userInfo.Active = "N";
        //        return false;
        //    };

        //}
        public static async Task<string> Emruzasync()
        {
            return await GetReportResultAsync("GetToday", null, "Today");
        }

        public static async Task<string> DateSegmantAsync(string _Date)
        { // بازگرداندن اجزای تاریخ
            return await GetReportResultAsync("GetDate", null, _Date);
        }

        public static async Task<DataRow> DateSegmantAsync()
        { // بازگرداندن اجزای تاریخ
            return await GetReportRowAsync("GetDate", null);
        }
        public static async Task<FormData> GetFormData(int UserSi, int FormId)
        {
            // ثبت آخرین فرم ورودی و گرفتن تعداد پیامهای باز کاربر
            var parametrsLast = new Dictionary<string, object>
            {
                { "@Person", UserSi },
                { "@Form",FormId }
            };
            await DBS.GetReportRowAsync("KarbaranForms_UpdateTime", parametrsLast);

            FormData formData = new();

            var paramlastForms = new Dictionary<string, object>
            {
                { "@Person", UserSi }
            };
            formData.lastForms = await GetReportAsync("KarbaranForms_LastForms", paramlastForms);

            var paramPages = new Dictionary<string, object>
            {
                { "@Si_Form", FormId }
            };
            formData.Pages = await GetReportAsync("FormPages_List", paramPages);

            return formData;
        }
        public static async Task<string> FindValueAsync(string Table, Int32 SiValue, string FiledResult)
        {
            string Result = "";
            using (SqlConnection conn = GetSarmadConnection())
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("FindValue", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Table", Table);
                    cmd.Parameters.AddWithValue("@Id", SiValue);
                    cmd.Parameters.AddWithValue("@Filed", FiledResult);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Result = reader["Res"].ToString();
                        }
                    }
                }
            }

            return Result;
        }
    }
}
