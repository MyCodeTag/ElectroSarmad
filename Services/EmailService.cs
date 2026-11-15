using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Reflection;


//using System.Net.Mail;
using System.Threading.Tasks;

namespace Sarmad.Services
{
    public class EmailService
    {

        private static readonly string _Host;
        private static readonly int _Port;
        private static readonly string _UserName;
        private static readonly string _Password;

        static EmailService()
        {
            _Host = Center.Configuration["MailSettings:Host"];
            _Port = int.Parse(Center.Configuration["MailSettings:Port"]!);
            _UserName = Center.Configuration["MailSettings:UserName"];
            _Password = Center.Configuration["MailSettings:Password"];
        }



        public async Task SendMessageAsync(
                string toEmail,
                string userName,
                string url,
                string MsgOnvan,
                string Writer
            )
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    "بن سافت",
                    "noreply@Sarmad.ir"));

            email.To.Add(
                MailboxAddress.Parse(toEmail));

            email.Subject = "پیام جدید در بن سافت";

            string LOGO = "https://Sarmad.ir/images/logo.svg";

            string body = $"""
                <!DOCTYPE html>
                <html lang="fa" dir="rtl">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>پیام جدید - بن سافت</title>
                </head>
                <body style="margin:0;
                            padding:30px 15px;
                            background:#f4f7fb;
                            font-family:Tahoma,Arial,sans-serif;
                            color:#333333;
                            direction: rtl;">

                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                        <tr>
                            <td align="center">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0"
                                       style="max-width:650px;
                                            background:#ffffff;
                                            border-radius:12px;
                                            overflow:hidden;
                                            border:1px solid #e4e7ec;">

                                    <tr>
                                        <td style="background:#1565c0;
                                                    padding:30px;
                                                    text-align:center;">

                                            <div style="color:white;
                                                        font-size:28px;
                                                        font-weight:bold;
                                                        letter-spacing:1px;">
                                                <a href="https://Sarmad.ir/Dayli/MsgCenter"><img src="{LOGO}" /> </a>
                                            </div>

                                            <div style="margin-top:8px;
                                                        color:#dbe9ff;
                                                        font-size:15px;">
                                                سیستم مدیریت هوشمند کسب‌وکار
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="padding:40px;">

                                            <p style="
                                                margin:0;
                                                font-size:20px;
                                                font-weight:bold;">
                                                سلام {userName} عزیز 🌹
                                            </p>

                                            <p style="
                                                margin-top:25px;
                                                line-height:2;
                                                font-size:15px;">

                                                یک <strong>پیام جدید</strong> در
                                                <strong>مرکز پیام بن سافت</strong>
                                                با موضوع <strong>{MsgOnvan}</strong>
                                                برای شما ثبت شده است.
                                                <hr/>
                                                نویسنده: <strong>{Writer}</strong>
                                            </p>

                                            <div style="
                                                margin:30px 0;
                                                padding:18px;
                                                background:#f6f9ff;
                                                border-right:5px solid #1565c0;
                                                border-radius:6px;
                                                line-height:2;
                                                color:#555555;">

                                                برای مشاهده جزئیات پیام و پاسخ به آن،
                                                روی دکمه زیر کلیک کنید.

                                            </div>

                                            <div style="
                                                text-align:center;
                                                margin:40px 0;">

                                                <a href="{url}"
                                                   style="
                                                    display:inline-block;
                                                    background:#1565c0;
                                                    color:white;
                                                    text-decoration:none;
                                                    padding:16px 40px;
                                                    font-size:16px;
                                                    font-weight:bold;
                                                    border-radius:8px;">

                                                    📩 مشاهده پیام

                                                </a>

                                            </div>

                                            <p style="
                                                font-size:13px;
                                                color:#666666;
                                                line-height:2;">

                                                اگر دکمه بالا عمل نکرد، لینک زیر را در مرورگر خود باز کنید:

                                            </p>

                                            <p style="
                                                word-break:break-all;
                                                font-size:12px;
                                                color:#1565c0;">

                                                {url}

                                            </p>

                                            <hr style="
                                                margin:35px 0;
                                                border:none;
                                                border-top:1px solid #eeeeee;">

                                            <table width="100%">
                                                <tr>
                                                    <td style="
                                                        font-size:13px;
                                                        color:#777777;
                                                        line-height:2;">

                                                        <strong>نکته امنیتی</strong><br>

                                                        این ایمیل صرفاً جهت اطلاع‌رسانی ارسال شده است.
                                                        برای حفظ امنیت اطلاعات، متن کامل پیام در ایمیل نمایش داده نمی‌شود.

                                                    </td>

                                                </tr>
                                            </table>

                                        </td>
                                    </tr>

                                    <tr>

                                        <td style="
                                                background:#fafafa;
                                                padding:25px;
                                                text-align:center;
                                                font-size:12px;
                                                color:#888888;
                                                line-height:2;">

                                            این ایمیل به صورت خودکار توسط
                                            <strong>بن سافت</strong>
                                            ارسال شده است.

                                            <br>

                                            لطفاً به این ایمیل پاسخ ندهید.

                                            <br><br>

                                            <a href="https://Sarmad.ir"
                                               style="
                                                    color:#1565c0;
                                                    text-decoration:none;
                                                    font-weight:bold;">
                                                  www.Sarmad.ir
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """;


            email.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            // ایجاد فایل لاگ SMTP
            //using var smtp = new SmtpClient(
            //    new ProtocolLogger("mail.log"));
            using var smtp = new SmtpClient();

            smtp.Timeout = 30000;

            // فقط برای تست
            //smtp.ServerCertificateValidationCallback =
            //    (sender, certificate, chain, errors) => true;

            try
            {
                //--------------------------------------------------
                // اتصال
                //--------------------------------------------------

                await smtp.ConnectAsync(
                    _Host,
                    _Port,
                    SecureSocketOptions.Auto);

                // برای پورت 465
                //await smtp.ConnectAsync(
                //  _Host,
                //  _Port,
                //  SecureSocketOptions.SslOnConnect);

                // برای پورت 587
                //await smtp.ConnectAsync(
                //    _Host,
                //    _Port,
                //    SecureSocketOptions.StartTls);

                if (!smtp.IsConnected)
                {
                    await DBS.LogErrorSaveAsync("smtp.ConnectAsync", null, 1, 0, MethodBase.GetCurrentMethod().Name, "SMTP Server اتصال را برقرار نکرد.");
                    return;
                }
                //throw new Exception("SMTP Server اتصال را برقرار نکرد.");

                //--------------------------------------------------
                // احراز هویت
                //--------------------------------------------------

                await smtp.AuthenticateAsync(
                    _UserName,
                    _Password);

                if (!smtp.IsAuthenticated)
                {
                    await DBS.LogErrorSaveAsync("smtp.ConnectAsync", null, 1, 0, MethodBase.GetCurrentMethod().Name, "احراز هویت SMTP ناموفق بود.");
                    return;
                }
                //throw new Exception("احراز هویت SMTP ناموفق بود.");

                //--------------------------------------------------
                // ارسال
                //--------------------------------------------------

                await smtp.SendAsync(email);

                //--------------------------------------------------
                // قطع اتصال
                //--------------------------------------------------

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                await DBS.LogErrorSaveAsync("smtp.ConnectAsync", null, 1, 0, MethodBase.GetCurrentMethod().Name, $"خطا در ارسال ایمیل:{Environment.NewLine}{ex}");
                return;

                //throw new Exception(
                //    $"خطا در ارسال ایمیل:{Environment.NewLine}{ex}",
                //    ex);
            }
        }

    }

}
