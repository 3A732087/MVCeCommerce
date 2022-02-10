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
    public class AlbumController : Controller
    {
        private readonly AlbumDBService albumService = new AlbumDBService();

        // GET: Album
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        #region 相片列表
        [Authorize(Roles = "Admin")]
        public ActionResult List(int Page = 1)
        {
            AlbumViewModel Data = new AlbumViewModel();

            Data.Paging = new ForPaging(Page);
            Data.FileList = albumService.GetDataList(Data.Paging);

            return PartialView(Data);
        }
        #endregion

        #region 上傳檔案
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return PartialView();
        }


        //上傳檔案表單內容用的action
        [Authorize]
        [HttpPost]
        public ActionResult Upload([Bind(Include = "upload")]AlbumViewModel File)
        {
            if(File.upload != null)
            {
                int Alb_Id = albumService.LastAlbumFinder();
                //將檔案和伺服器上路徑合併
                string Url = Path.Combine(Server.MapPath("~/Upload/"), Alb_Id.ToString() + "_" + File.upload.FileName);
                File.upload.SaveAs(Url);

                albumService.UploadFile(Alb_Id, Alb_Id.ToString() + "_" + File.upload.FileName, Url, File.upload.ContentLength, File.upload.ContentType, User.Identity.Name);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 顯示圖片
        [Authorize(Roles = "Admin")]
        public ActionResult Show(int Alb_Id)
        {
            AlbumViewModel ToShow = new AlbumViewModel();
            ToShow.File = albumService.GetDataById(Alb_Id);

            if(ToShow.File != null)
            {
                UrlHelper urlHelper = new UrlHelper(Request.RequestContext);
                urlHelper.Content("~/Upload/" + ToShow.File.FileName);

                return Content(urlHelper.Content("~/Upload/" + ToShow.File.FileName));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 下載檔案
        [Authorize(Roles = "Admin")]
        public ActionResult DownloadFile(int Alb_Id)
        {
            AlbumViewModel Download = new AlbumViewModel();
            Download.File = albumService.GetDataById(Alb_Id);

            if (Download.File != null)
            {

                Stream iStream = new FileStream(Download.File.Url, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(iStream, Download.File.Type, Download.File.FileName);
            }
            else
            {
                return JavaScript("alert(\"無此檔案\")");
            }

        }
        #endregion

        #region 刪除檔案
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteFile(int Alb_Id)
        {
            albumService.DeleteArticle(Alb_Id);
            return RedirectToAction("Index");
        }
        #endregion

        #region 相片輪播
        public ActionResult Carousel()
        {
            //宣告一個新頁面模型
            AlbumViewModel Data = new AlbumViewModel();
            //新增頁面模型中的分頁(只顯示第一頁)
            Data.Paging = new ForPaging(1);
            //從Service中取得頁面所需陣列資料
            Data.FileList = albumService.GetDataList(Data.Paging);
            //將頁面資料傳入View中
            return View(Data);
        }
        #endregion
    }
}