using MVCeCommerce.Services;
using MVCeCommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCeCommerce.Controllers
{
    public class ItemController : Controller
    {
        private readonly CartService cartService = new CartService();
        private readonly ItemService itemService = new ItemService();


        // GET: Item
        public ActionResult Index(int Page = 1)
        {
            ItemViewModel Data = new ItemViewModel();
            Data.Paging = new ForPaging(Page);
            Data.IdList = itemService.GetIdList(Data.Paging);
            Data.ItemBlock = new List<ItemDetailViewModel>();

            foreach(var Id in Data.IdList)
            {
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                newBlock.Data = itemService.GetDataById(Id);
                string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;

                newBlock.InCart = cartService.CheckInCart(Cart, Id);
                Data.ItemBlock.Add(newBlock);
            }
            return View(Data);
        }

        #region 商品頁面
        public ActionResult Item(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;

            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return View(ViewData);
        }
        #endregion

        #region 商品列表區塊
        public ActionResult ItemBlock(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);

            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return PartialView(ViewData);
        }
        #endregion

        #region 新增商品
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Add(ItemCreateViewModel Data)
        {
            if(Data.ItemImage != null)
            {
                string fileName = Path.GetFileName(Data.ItemImage.FileName);
                string Url = Path.Combine(Server.MapPath("~/Upload/"), fileName);
                //將檔案存在伺服器
                Data.ItemImage.SaveAs(Url);
                //設定路徑
                Data.NewDate.Image = fileName;
                itemService.Insert(Data.NewDate);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("ItemImage", "請選擇上傳檔案");
                return View(Data);

            }
        }
        #endregion

        #region 商品刪除
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int Id)
        {
            itemService.Delete(Id);
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}