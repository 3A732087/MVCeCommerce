using MVCeCommerce.Models;
using MVCeCommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class MessageViewModel
    {
        public List<Message> DataList { get; set; }

        public ForPaging Paging { get; set; }

        public int A_Id { get; set; }
    }
}