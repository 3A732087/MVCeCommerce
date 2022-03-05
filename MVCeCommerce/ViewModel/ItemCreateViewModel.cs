﻿using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class ItemCreateViewModel
    {
        [DisplayName("商品圖片")]
        [FileExtensions(ErrorMessage = "您所上傳的檔案不是圖片")]
        //HttpPostedFileBase為檔案上傳的操作類別
        public HttpPostedFileBase ItemImage { get; set; }

        //新增商品內容
        public Item NewData { get; set; }

    }
}