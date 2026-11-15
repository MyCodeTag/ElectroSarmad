using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using Sarmad.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;


namespace Sarmad.Services
{
    public static class Center
    {
        // کلیه دستوراتی که تغییراتی در دیتابیس میدهد لازم است که دستور ذخیره شود
        // متدهایی که با LogAction همراه هستند، دستور کاربر را ذخیره میکنند
        public static IConfiguration Configuration { get; set; }

        public static string PdfName(string? userCooki)
        {
            string pdfName = "0.pdf";
            if (userCooki == null)
                pdfName = "0.pdf";

            var legacyCookie = userCooki.FromLegacyCookieString();
            if (legacyCookie != null)
                pdfName = legacyCookie["userName"];

            return $"Report_{pdfName}.pdf";
        }


        public static void ChapEmpty(string webRootPath, string Chap, string ext)
        {
            string filePath = Path.Combine(webRootPath, "Uploads");
            File.Copy(Path.Combine(filePath, "Empty" + ext), Path.Combine(filePath, "Temp", Chap + ext), true);
        }

        public static string ExcelName(string? userCooki)
        {
            string excelName = "0.xlsx";
            if (userCooki == null)
                excelName = "0.xlsx";

            var legacyCookie = userCooki.FromLegacyCookieString();
            if (legacyCookie != null)
                excelName = legacyCookie["userName"];

            return $"Report_{excelName}.xlsx";
        }
        public static void DeleteFileIfExists(string FilePath)
        {
            var file = new FileInfo(FilePath);
            if (file.Exists)
                file.Delete();
        }
        public static void CellAddData(IRow row, int col, string Value, CellType Type)
        {

            // ایجاد سلول در این سطر
            ICell cell = row.CreateCell(col, Type);
            // نوشتن در سلول
            if (Type == CellType.Numeric)
                try
                {
                    cell.SetCellValue(Convert.ToDouble(Value));
                }
                catch
                {
                    cell.SetCellValue(Value);
                }
            else
                cell.SetCellValue(Value);
        }
        public static void CellAddDataNew(IRow row, int col, string Value, CellType Type, ICellStyle Style)
        {

            // ایجاد سلول در این سطر
            ICell cell = row.CreateCell(col, Type);
            cell.CellStyle = Style;
            // نوشتن در سلول
            if (Type == CellType.Numeric)
                try
                {
                    cell.SetCellValue(Convert.ToDouble(Value));
                }
                catch
                {
                    cell.SetCellValue(Value);
                }
            else
                cell.SetCellValue(Value);
        }

        public static void CellAddDataHeader(IRow row, int col, string Value, CellType Type, IWorkbook workbook, Int16? Size = 12)
        {

            // ایجاد سلول در این سطر
            ICell cell = row.CreateCell(col, Type);

            IFont font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = (short)(Size ?? 12);
            ICellStyle boldStyle = workbook.CreateCellStyle();
            boldStyle.SetFont(font);

            cell.CellStyle = boldStyle;

            // نوشتن در سلول
            if (Type == CellType.Numeric)
                try
                {
                    cell.SetCellValue(Convert.ToDouble(Value));
                }
                catch
                {
                    cell.SetCellValue(Value);
                }
            else
                cell.SetCellValue(Value);
        }


        public static string CommandText(string procName, Dictionary<string, object> parameters = null)
        { // متن دستور SQL برای اجرا
            //تبدیل پارامترها به جیسون
            string Params = JsonConvert.SerializeObject(parameters);
            // اصلاحات برای ذخیره مناسب
            Params = Params.Replace("\"@", "@").Replace("\":", "=");
            Params = Params.Replace("{", "").Replace("}", "").Replace("\"", "'");
            return $"Exec {procName} {Params} ";
        }


        public static string MonthName(string MN)
        {
            string MNAME = MN.Replace("BB", "بهار").Replace("TT", "تابستان").Replace("PP", "پائیز").Replace("ZZ", "زمستان").Replace("01", "فروردین").Replace("02", "اردیبهشت").Replace("03", "خرداد").Replace("04", "تیر").Replace("05", "مرداد").Replace("06", "شهریور").Replace("07", "مهر").Replace("08", "آبان").Replace("09", "آذر").Replace("10", "دی").Replace("11", "بهمن").Replace("12", "اسفند");
            return MNAME;
        }
        public static string MonthNameFull(string MN)
        {
            string MNAME = MN.Replace("/BB", "/بهار").Replace("/TT", "/تابستان").Replace("/PP", "/پائیز").Replace("/ZZ", "/زمستان").Replace("/01", "/فروردین").Replace("/02", "/اردیبهشت").Replace("/03", "/خرداد").Replace("/04", "/تیر").Replace("/05", "/مرداد").Replace("/06", "/شهریور").Replace("/07", "/مهر").Replace("/08", "/آبان").Replace("/09", "/آذر").Replace("/10", "/دی").Replace("/11", "/بهمن").Replace("/12", "/اسفند");
            return MNAME;
        }
        
        public static void GetUserData(string userCooki, UserInfo userInfo)
        { // بازگزداندن دیتای کاربر از روی کوکی
            var legacyCookie = userCooki.FromLegacyCookieString();
            if (legacyCookie != null)
            {
                userInfo.UserName = legacyCookie["UserName"];
                userInfo.Knd = legacyCookie["Knd"];
                userInfo.Si = int.Parse(legacyCookie["Si"]);
            }
        }
        public static Int32 GetUserId(string? userCooki)
        { // بازگرداندن آیدی کاربر از روی کوکی
            Int32 UserSi = 0;
           
            if (userCooki == null)
                UserSi = 0;

            var legacyCookie = userCooki.FromLegacyCookieString();
            if (legacyCookie != null)
                UserSi = int.Parse(legacyCookie["Si"]);
            return UserSi;
        }

        public static string TrimTxt(string? text)
        {
            if (text == "" || text == null)
                return "";
            else
                return text.Trim().Replace("'", "^");
        }
        public static string EnglishNumber(string NUMBER = "1234567890")
        {
            if (NUMBER == null)
                NUMBER = "";
            return NUMBER.Replace('۰', '0')
                .Replace('۱', '1')
                .Replace('۲', '2')
                .Replace('۳', '3')
                .Replace('۴', '4')
                .Replace('۵', '5')
                .Replace('۶', '6')
                .Replace('۷', '7')
                .Replace('۸', '8')
                .Replace('۹', '9') // فارسی

                .Replace('٠', '0')
                .Replace('١', '1')
                .Replace('٢', '2')
                .Replace('٣', '3')
                .Replace('٤', '4')
                .Replace('٥', '5')
                .Replace('٦', '6')
                .Replace('٧', '7')
                .Replace('٨', '8')
                .Replace('٩', '9'); // عربی;
        }
        public static string CommaClear(object value)
        {
            if (value == null || value == DBNull.Value)
                return "0";

            string input = value.ToString();

            if (string.IsNullOrWhiteSpace(input))
                return "0";

            input = input
                .Replace('۰', '0')
                .Replace('۱', '1')
                .Replace('۲', '2')
                .Replace('۳', '3')
                .Replace('۴', '4')
                .Replace('۵', '5')
                .Replace('۶', '6')
                .Replace('۷', '7')
                .Replace('۸', '8')
                .Replace('۹', '9') // فارسی

                .Replace('٠', '0')
                .Replace('١', '1')
                .Replace('٢', '2')
                .Replace('٣', '3')
                .Replace('٤', '4')
                .Replace('٥', '5')
                .Replace('٦', '6')
                .Replace('٧', '7')
                .Replace('٨', '8')
                .Replace('٩', '9'); // عربی

            var result = new string(input
                .Where(c => char.IsDigit(c) || c == '.' || c == '-')
                .ToArray());

            return string.IsNullOrWhiteSpace(result) ? "0" : result;
        }

        public static string ClearComma(this object value)
        {
            if (value == null || value == DBNull.Value)
                return "0";

            string input = value.ToString();

            if (string.IsNullOrWhiteSpace(input))
                return "0";

            input = input
                .Replace('۰', '0')
                .Replace('۱', '1')
                .Replace('۲', '2')
                .Replace('۳', '3')
                .Replace('۴', '4')
                .Replace('۵', '5')
                .Replace('۶', '6')
                .Replace('۷', '7')
                .Replace('۸', '8')
                .Replace('۹', '9') // فارسی

                .Replace('٠', '0')
                .Replace('١', '1')
                .Replace('٢', '2')
                .Replace('٣', '3')
                .Replace('٤', '4')
                .Replace('٥', '5')
                .Replace('٦', '6')
                .Replace('٧', '7')
                .Replace('٨', '8')
                .Replace('٩', '9'); // عربی

            var result = new string(input
                .Where(c => char.IsDigit(c) || c == '.' || c == '-')
                .ToArray());
            return string.IsNullOrWhiteSpace(result) ? "0" : result;
        }
        public static string PersianNumbers(this object value)
        {
            if (value == null || value == DBNull.Value)
                return "";

            string input = value.ToString();

            if (string.IsNullOrWhiteSpace(input))
                return "";

            return input
                .Replace('0', '۰')
                .Replace('1', '۱')
                .Replace('2', '۲')
                .Replace('3', '۳')
                .Replace('4', '۴')
                .Replace('5', '۵')
                .Replace('6', '۶')
                .Replace('7', '۷')
                .Replace('8', '۸')
                .Replace('9', '۹')
                .Replace('٠', '۰')
                .Replace('١', '۱')
                .Replace('٢', '۲')
                .Replace('٣', '۳')
                .Replace('٤', '۴')
                .Replace('٥', '۵')
                .Replace('٦', '۶')
                .Replace('٧', '۷')
                .Replace('٨', '۸')
                .Replace('٩', '۹');
        }


        public static string ToComma(this object value, int decimals = -3, string nullText = "0")
        { // نمایش اعداد با کاما  نمایش الزامی اعشار در صورتی که decimals مقدار دهی شده باشد
          // decimals > 0 : fixed decimal digits
          // decimals = 0 : integer format
          // decimals < 0 : optional decimal digits (max = Abs(decimals))

            if (value == null || value == DBNull.Value)
                return nullText;

            // تبدیل به decimal
            if (!decimal.TryParse(value.ToString(), out decimal result))
                return nullText;

            //// اگر عدد صحیح بود اعشار حذف شود
            if (result == Math.Truncate(result))
                return result.ToString("N0");

            string format = decimals < 0
                ? "#,##0." + new string('#', -decimals)
                : $"N{decimals}";

            return result.ToString(format);

        }

        public static string ToCommaAshar(this object value, int decimals = 0, string nullText = "0")
        { // نمایش اعداد با کاما و جداسازی قسمت اعشاری با استایل مشخص ، اعشار درصورتی نمایش داده میشود که decimals مقداردهی شده باشد
            if (value == null || value == DBNull.Value)
                return nullText;

            if (!decimal.TryParse(value.ToString(), out decimal result))
                return nullText;

            string formatted = result.ToString($"N{decimals}"); // 123,456.78
            int dotIndex = formatted.LastIndexOf('.');
            int Ashar = dotIndex + 1;

            string intPart = dotIndex > -1 ? formatted[..dotIndex] : formatted;

            string decPart = dotIndex > -1 ? formatted[Ashar..] : "0";
            decPart = (int.Parse(decPart) == 0) ? "" : "." + decPart;
            // سمبل داخل span برای استایل‌دهی
            if (result < 0)
            //            if (value.ToString().Contains("-"))
            {
                return $"<int class='text-danger'>{intPart}</int><ashar>{decPart}</ashar>";
            }
            else
            {
                return $"<int>{intPart}</int><ashar>{decPart}</ashar>";
            }
        }


        public static string ToMony(this object value, string nullText = "0")
        { // نمایش اعداد مبلغ با فرمت پول  
            if (value == null || value == DBNull.Value)
                return nullText;

            if (!decimal.TryParse(value.ToString(), out decimal result))
                return nullText;

            var culture = new CultureInfo("fa-IR");

            //culture.NumberFormat.CurrencySymbol = "ریال";
            culture.NumberFormat.CurrencyPositivePattern = 3;
            culture.NumberFormat.CurrencyNegativePattern = 8;

            return result.ToString("N0", culture); // بدون اعشار
        }

        public static string ToMonySymbol(this object value, string nullText = "0")
        { // نمایش اعداد مبلغ با فرمت پول و نمایش آیکون ریال در انتها
            if (value == null || value == DBNull.Value)
                return nullText;

            if (!decimal.TryParse(value.ToString(), out decimal result))
                return nullText;

            var culture = new CultureInfo("fa-IR");
            culture.NumberFormat.CurrencyPositivePattern = 3;
            culture.NumberFormat.CurrencyNegativePattern = 8;

            // فقط عدد، بدون CurrencySymbol
            string number = result.ToString("N0", culture);

            // سمبل داخل span برای استایل‌دهی
            if (result < 0)
            //            if (value.ToString().Contains("-"))
            {
                return $"<span class='text-danger'>{number}</span> <span class='rial-symbol icofont-riyal'></span>";
            }
            else
            {
                return $"{number} <span class='rial-symbol icofont-riyal'></span>";
            }
        }

        public static string ToPercent(this object value, string nullText = "0")
        {
            if (value == null || value == DBNull.Value)
                return nullText;

            if (decimal.TryParse(value.ToString(), out decimal result))
                return result.ToString("P");

            return nullText;
        }

        public static string FormatShabaRegex(string Shaba)
        {
            return Regex.Replace(
                Shaba.Replace(" ", "").Replace("-", "").ToUpper(),
                ".{4}",
                "$0-").TrimEnd('-');
        }
        public static string FormatCardRegex(string card)
        {
            return Regex.Replace(
                card.Replace(" ", "").Replace("-", ""),
                ".{4}",
                "$0-").TrimEnd('-');
        }

        public static string HashToken(string token, string secret)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (secret == null) throw new ArgumentNullException(nameof(secret));

            var key = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = Configuration["Auth:TokenSecret"];
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = Configuration["Auth:TokenSecret"];
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static Dictionary<string, object?> ToJsonObject(this DataRow row)
        {
            return row.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(
                    c => c.ColumnName.ToLowerInvariant(),
                    c => row[c]
                );
        }
        public static string Left(this string number, int maxLength)
        { // کاراکترهای سمت چپ یک عبارت
            if (string.IsNullOrEmpty(number)) return number;
            maxLength = Math.Abs(maxLength);

            return (number.Length <= maxLength
                    ? number
                    : number.Substring(0, maxLength)
                    );
        }

    }

    public class PasswordService
    {
        private static readonly PasswordHasher<string> _hasher = new();

        public static string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}

