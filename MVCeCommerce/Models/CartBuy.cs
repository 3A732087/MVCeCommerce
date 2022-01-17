using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Models
{
    public class CartBuy
    {
        public string Cart_Id { get; set; }

        public int Item_Id { get; set; }

        public Item Item { get; set; } = new Item();
    }
}