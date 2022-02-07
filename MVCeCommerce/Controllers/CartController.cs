using MVCeCommerce.Services;
using MVCeCommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCeCommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService cartService = new CartService();

        [Authorize]
        // GET: Cart
        public ActionResult Index()
        {
            CartBuyViewModel Data = new CartBuyViewModel();
            //取得session內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;

            Data.DataList = cartService.GetItemFromCart(Cart);
            Data.isCartsave = cartService.CheckCartSave(User.Identity.Name,Cart);

            return View(Data);
        }

        #region 保存購物車
        [Authorize]
        public ActionResult CartSave()
        {
            string Cart;

            if(HttpContext.Session["Cart"] != null)
            {
                Cart = HttpContext.Session["Cart"].ToString();
            }
            else
            {
                Cart = DateTime.Now.ToString() + User.Identity.Name;
                HttpContext.Session["Cart"] = Cart;
            }

            //藉由service儲存購物車資料
            cartService.SaveCart(User.Identity.Name, Cart);
            return RedirectToAction("Index");
        }
        #endregion

        [Authorize]
        public ActionResult CartSaveRemove()
        {
            cartService.SaveCartRemove(User.Identity.Name);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Pop(int Id, string toPage)
        {
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            cartService.RemoveForCart(Cart, Id);
            if(toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if(toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Put(int Id, string toPage)
        {

            if(HttpContext.Session["Cart"] == null)
            {
                HttpContext.Session["Cart"] = DateTime.Now.ToString() + User.Identity.Name;
            }
            cartService.AddtoCart(HttpContext.Session["Cart"].ToString(), Id);

            if(toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }else if(toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
    }
}