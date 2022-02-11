using MVCeCommerce.Models;
using MVCeCommerce.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class HomeViewModel
    {
        [DisplayName("搜尋")]
        public string Search { get; set; }

        public List<Members> DataList { get; set; }

        public ForPaging Paging { get; set; }
    }
}