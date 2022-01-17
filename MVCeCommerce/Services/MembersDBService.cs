using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MVCeCommerce.Services
{
    public class MembersDBService
    {
        //資料庫連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP_MVCeCommerce"].ConnectionString;
        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 註冊
        public void Register(Members newMember)
        {
            //密碼Hash
            newMember.Password = HashPassword(newMember.Password);

            string sql = $@"INSERT INTO Member (Account, Password, Name, Image, Email, AuthCode, IsAdmin) VALUES ('{newMember.Account}','{newMember.Password}','{newMember.Name}','{newMember.Image}','{newMember.Email}','{newMember.AuthCode}','0')";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region Hash密碼
        public string HashPassword(string Password)
        {
            //宣告hash時所添加的無意義亂數值
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            //宣告亂數字串與密碼結合
            string saltAndPassword = String.Concat(Password, saltkey);

            //定義sha256的hash物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得hash後byte資料
            byte[] HashData = sha256Hasher.ComputeHash(PasswordData);
            //hash後資料轉為string 
            string Hashresult = Convert.ToBase64String(HashData);
            return Hashresult;
        }
        #endregion

        #region 查詢一筆資料(private)
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"SELECT * FROM Member WHERE Account = '{Account}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);


            }
            catch (Exception e)
            {
                //throw new Exception(e.Message.ToString());
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 帳號註冊重複確認
        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);

            bool result = (Data == null);

            return result;
        }
        #endregion

        #region 查詢一筆公開性資料(public)
        public Members GetDatabyAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"SELECT * FROM Member WHERE Account = '{Account}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Image = dr["Image"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.Name = dr["Name"].ToString();

            }
            catch (Exception e)
            {
                //throw new Exception(e.Message.ToString());
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 信箱驗證
        public string EmailValidate(string Account, string AuthCode)
        {
            //取得傳入帳號的會員資料
            Members ValidateMeber = GetDataByAccount(Account);

            //宣告驗證後訊息字串
            string ValidateStr = string.Empty;
            if (ValidateMeber != null)
            {
                //判斷傳入驗證碼與資料庫是否相同
                if (ValidateMeber.AuthCode == AuthCode)
                {
                    //驗證碼相同設空
                    string sql = $@"UPDATE Member set AuthCode = '{string.Empty}' where Account = '{Account}'";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message.ToString());
                    }
                    finally
                    {
                        conn.Close();
                    }
                    ValidateStr = "信箱驗證成功，現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再註冊";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或在註冊";
            }
            return ValidateStr;
        }
        #endregion

        #region 登入確認
        public string LoginCheck(string Account, string Password)
        {
            //取此帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);

            if (LoginMember != null)
            {
                //判斷有無信箱驗證
                if (String.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    //密碼確認
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過Email驗證，請到註冊信箱收信";
                }
            }
            else
            {
                return "無此會員帳號，請先註冊";
            }
        }
        #endregion

        #region 密碼確認
        //進行密碼確認
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            //將輸入的密碼hash，判斷是否與資料庫的一樣
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            return result;
        }
        #endregion

        #region 變更密碼
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            //與此會員資料
            Members LoginMember = GetDataByAccount(Account);

            if (PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);

                string sql = $@"UPDATE Member  SET Password = '{LoginMember.Password}' WHERE Account = '{Account}'";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message.ToString());
                }
                finally
                {
                    conn.Close();
                }
                return "密碼修改成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion

        #region 取得角色
        public string GetRole(string Account)
        {
            //初始化身分
            string Role = "User";
            //取得此帳號資料
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";
            }
            return Role;
        }
        #endregion

        #region 檢查圖片類型
        public bool CheckImage(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
            }
            return false;
        }
        #endregion
    }
}
