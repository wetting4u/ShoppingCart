using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using 購物車實作.Security;
using 購物車實作.Services;
using 購物車實作.ViewModels;

namespace 購物車實作.Controllers
{
    public class MembersController : Controller
    {
        //宣告Members資料的Service物件
        public readonly MembersDBService membersService = new MembersDBService();
        //宣告寄信用的Service物件
        private readonly MailService mailService = new MailService();
        //宣告Cart相關的Service物件
        private readonly CartService cartService = new CartService();

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Item");//已登入則重新導向
            return View();
        }
        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            if (String.IsNullOrEmpty(ValidateStr))
            {
                //先清空Session
                HttpContext.Session.Clear();
                //取得購物車保存
                string Cart = cartService.GetCartSave(LoginMember.Account);
                //判斷是否有保存，若有則存入Session
                if (Cart != null)
                {
                    HttpContext.Session["Cart"] = Cart;
                }
                //先藉由Service取得登入者角色資料
                string RoleData = membersService.GetRole(LoginMember.Account);
                //設定JWT
                JwtService jwtService = new JwtService();
                //從Web.Config撈出資料
                //Coolie名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);
                //產生一個Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設定單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                return RedirectToAction("Index", "item");
            }
            else
            {
                //有驗證錯誤信息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                return View(LoginMember);
            }
        }
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Item");//已登入則重新導向
            return View();
        }
        [HttpPost]
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            //判斷頁面資料中的密碼欄位輸入
            if (ModelState.IsValid)
            {
                //將頁面中的密碼欄位輸入
                RegisterMember.newMember.Password = RegisterMember.Password;
                //取得信箱驗證碼
                string AuthCode = mailService.GetValidateCode();
                //將信箱驗證碼填入
                RegisterMember.newMember.AuthCode = AuthCode;
                //呼叫Service註冊新會員
                membersService.Register(RegisterMember.newMember);
                //取得寫好的驗證信範本內容
                string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                //宣告Email驗證用的Url
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                { Path = Url.Action("EmailValidate", "Members", new { Account = RegisterMember.newMember.Account, AuthCode = AuthCode }) };
                //藉由Service將使用者資料填入驗證信範本中
                string MailBody = mailService.GetRegisterMailBody(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));
                //呼叫Service寄出驗證信
                mailService.SendRegisterMailstring(MailBody, RegisterMember.newMember.Email);
                //TempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                //重新導向頁面
                return RedirectToAction("RegisterResult");
            }
            //未經驗證清空密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            //將資料回填至View中
            return View(RegisterMember);
        }
        public ActionResult RegisterResult()
        {
            return View();
        }
        //判斷註冊帳號是否已被註冊過Action
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳進來的Action
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            //用View儲存，使用Service進行信箱驗正後的結果訊息
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(MembersChangePasswordViewModel ChangeData)
        {
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
                return View();
        }
        [Authorize]
        public ActionResult Logout()
        {
            //Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            //重新導向至登入Action
            return RedirectToAction("Login");
        }
    }
}