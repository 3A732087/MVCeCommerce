using MVCeCommerce.Models;
using MVCeCommerce.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class AlbumViewModel
    {
        [DisplayName("上傳圖片")]
        [FileExtensions(ErrorMessage = "所上傳檔案不是圖片")]
        public HttpPostedFileBase upload { get; set; }

        public List<Album> FileList { get; set; }

        public ForPaging Paging { get; set; }

        public Album File { get; set; }
    }
}