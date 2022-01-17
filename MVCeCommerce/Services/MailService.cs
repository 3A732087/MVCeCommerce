using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace MVCeCommerce.Services
{
    public class MailService
    {
        private string gmail_account = "";
        private string gmail_password = "";
        private string gmail_mail = "";

        #region 寄送會員驗證信
        //產生驗證碼
        public string GetValidateCode()
        {
            //設定驗證碼字元陣列
            string[] Code = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","1","2","3","4","5","6","7","8","9","a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"
            };

            //宣告初始為空的驗證碼
            string ValidateCode = string.Empty;
            //宣告可產生隨機述職的物件
            Random rd = new Random();
            //迴圈產出驗證碼
            for (int i = 0; i < 10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
            }
            return ValidateCode;
        }

        //將使用者資料填入驗證範本
        public string GetRegisterMailBody(string TempString, string UserName, string ValidateUrl)
        {
            //使用者資料填入
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            //回傳
            return TempString;
        }

        //寄驗證信
        public void SendRegisterMail(string MailBody, string ToEmail)
        {
            //建立寄信用smtp (gmail)
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            //建立使用者憑據(設定gmail帳戶)
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //open ssl
            SmtpServer.EnableSsl = true;
            //宣告信件內容
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收件者
            mail.To.Add(ToEmail);
            mail.Subject = "會員註冊確認信";
            mail.Body = MailBody;
            //設定信件內容為html格式
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);


        }

        #endregion
    }
}