using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Jose;
using MVCeCommerce.Security;

namespace MVCeCommerce
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //撰寫權限驗證前執行的動作
        //在此用於設定角色(Role)
        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            //接收請求資料
            HttpRequest httpRequest = HttpContext.Current.Request;
            //設定JWT密鑰
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            //設定cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            if (httpRequest.Cookies[cookieName] != null)
            {
                //將TOKEN還原
                JwtObject JwtObject = JWT.Decode<JwtObject>(Convert.ToString(httpRequest.Cookies[cookieName].Value), Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
                //將使用者角色資料取區，併分割成陣列
                string[] roles = JwtObject.Role.Split(new char[] { ',' });
                //自行建立 Identity 取代 HttpContext.Current.User的Identity
                //將資料塞進Claim內做設計
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, JwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier,JwtObject.Account)
                };
                var claimsIdnetity = new ClaimsIdentity(claims, cookieName);
                //加入identityprovider 這個Claim 使得反仿冒語彙 @Html.AntiForgeryToken()能通過
                claimsIdnetity.AddClaim(
                    new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
"My Identity", @"http://www.w3.org/2001/XMLSchema#string"));
                //指派角色到目前這個HttpContext的User物件去
                HttpContext.Current.User = new GenericPrincipal(claimsIdnetity, roles);
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }

        }
    }
}
