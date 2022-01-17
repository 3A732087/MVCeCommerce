using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Models
{
    public class CartSave
    {
        public string Account { get; set; }

        public string Cart_Id { get; set; }

        public Members Member { get; set; } = new Members();
    }
}