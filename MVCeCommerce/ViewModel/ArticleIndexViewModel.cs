using MVCeCommerce.Models;
using MVCeCommerce.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class ArticleIndexViewModel
    {
        [DisplayName("搜尋")]
        public string Search { get; set; }

        public List<Article> DataList { get; set; }

        //分頁
        public ForPaging Paging { get; set; }

        //文章列表帳號
        public string Account { get; set; }
    }
}