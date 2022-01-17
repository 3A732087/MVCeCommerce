using MVCeCommerce.Services;
using MVCeCommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MVCeCommerce.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersDBService membersService = new MembersDBService();
        private readonly MailService mailService = new MailService();
        private readonly CartService cartService = new CartService();

        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        #region 登入
        public ActionResult Login()
        {
            //判斷是否登入
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            //使用Service內的方法驗證
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            if (String.IsNullOrEmpty(ValidateStr))
            {
                //無訊息則登入
                HttpContext.Session.Clear();
                string Cart = cartService.GetCartSave(LoginMember.Account);
                if(Cart != null)
                {
                    HttpContext.Session["Cart"] = Cart;
                }

                //取得登入角色資料
                string RoleData = membersService.GetRole(LoginMember.Account);

                //設定JWT
                JwtService jwtService = new JwtService();

                //從Web.Config撈出資料
                //Cookie名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);

                //產生Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設訂單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", ValidateStr);
                return View(LoginMember);
            }
        }
        #endregion

        #region 註冊
        //註冊一開始顯示畫面
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //傳入註冊資料的action
        [HttpPost]
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            if (ModelState.IsValid)
            {
                if(RegisterMember.MembersImage != null)
                {
                    if (membersService.CheckImage(RegisterMember.MembersImage.ContentType))
                    {
                        string filename = Path.GetFileName(RegisterMember.MembersImage.FileName);
                        string url = Path.Combine(Server.MapPath($@"~/Upload/Members/"), filename);
                        RegisterMember.MembersImage.SaveAs(url);
                        RegisterMember.newMember.Image = filename;
                        RegisterMember.newMember.Password = RegisterMember.Password;
                        string AuthCode = mailService.GetValidateCode();

                        RegisterMember.newMember.AuthCode = AuthCode;
                        //呼叫service註冊會員
                        membersService.Register(RegisterMember.newMember);
                        //取得驗證信範本
                        string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                        //宣告Email驗證用的Url
                        UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                        {
                            Path = Url.Action("EmailValidate", "Members", new
                            {
                                Account = RegisterMember.newMember.Account,
                                AuthCode = AuthCode
                            })
                        };
                        //藉由service將使用者資料填入驗證信範本中
                        string MailBody = mailService.GetRegisterMailBody(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));
                        //呼叫Service寄出驗證信
                        mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);
                        //用TempData儲存註冊訊息
                        TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                        return RedirectToAction("RegisterResult");
                    }
                    else
                    {
                        ModelState.AddModelError("MembersImage", "所上傳檔案不是圖片");
                    }
                }
                else
                {
                    ModelState.AddModelError("MembersImage", "請選擇上傳檔案");
                    return View(RegisterMember);
                }
            }
            //未經驗證清空密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            return View(RegisterMember);
        }
        #endregion

        #region 註冊結果
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷註冊帳號是否已被註冊過Action
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            //呼叫Service來判斷，並回傳結果
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account),
                JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳進來的action
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }

        #endregion

        #region 登出
        [Authorize]
        public ActionResult Logout()
        {
            //使用者登出
            //cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);

            return RedirectToAction("Login");
        }
        #endregion

        #region 修改密碼
        [Authorize]  //需登入
        public ActionResult ChangePassword()
        {
            return View();
        }

        //修改密碼傳入資料
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(MembersChangePassword ChangeData)
        {
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion
    }
}