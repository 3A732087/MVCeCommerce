using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class ArticleViewModel
    {
        public Article article { get; set; }

        public List<Message> DataList { get; set; }
    }
}