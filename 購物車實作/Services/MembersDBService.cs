using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using 購物車實作.Models;

namespace 購物車實作.Services
{
    public class MembersDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["Trolley"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        //註冊新會員方法
        public void Register(Members newMember)
        {
            //將密碼Hash過
            newMember.Password = HashPassword(newMember.Password);
            string sql = $@" INSERT INTO Members (Account,Password,Name,Email,AuthCode,IsAdmin) VALUES ('{newMember.Account}','{newMember.Password}','{newMember.Name}','{newMember.Email}','{newMember.AuthCode}','0')";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public string HashPassword(string Password)
        {
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            //將剛剛宣告的字串與密碼結合
            string saltAndPassword = String.Concat(Password, saltkey);
            //定義SHA256的HASH物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte資料
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得Hash後byte資料
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
            //將Hash後byte資料轉換成string
            string Hashresult = Convert.ToBase64String(HashDate);
            //回傳hash後結果
            return Hashresult;
        }
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@" select * from Members where Account = '{Account}' ";
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
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            bool result = (Data == null);
            return result;
        }
        public string EmailValidate(string Account, string AuthCode)
        {
            //取得傳入帳號的會員資料
            Members ValidateMember = GetDataByAccount(Account);
            //宣告驗證後訊息字串
            string ValidateStr = string.Empty;
            if (ValidateMember != null)
            {
                if (ValidateMember.AuthCode == AuthCode)
                {
                    //將資料庫的驗證碼設為空
                    //sql更新語法
                    string sql = $@" update Members set AuthCode = '{string.Empty}' where Account = '{Account}'";
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
                    ValidateStr = "帳號信箱驗證成功，現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再註冊";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或再註冊";
            }
            return ValidateStr;
        }
        public string LoginCheck(string Account, string Password)
        {
            //取得傳入帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);
            //判斷是否有此會員
            if(LoginMember != null)
            {
                //判斷是否有經過信箱驗正，有經驗證碼欄位會被清空
                if(String.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    //進行帳號密碼確認
                    if(PasswordCheck(LoginMember, Password))
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
                    return "此帳號尚未經過Email驗證，請去收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            //判斷資料庫裡的密碼資料與傳入密碼資料Hash後是否一樣
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            return result;
        }
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);
            if(PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);
                string sql = $@" update Members set Password = '{LoginMember.Password}' where Account = '{Account}'";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch(Exception e)
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
        public string GetRole(string Account)
        {
            //宣告初始角色字串
            string Role = "User";
            //取得傳入帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);
            //判斷資料庫欄位是否為Admin
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";//添加Admin
            }
            return Role;
        }
    }
}