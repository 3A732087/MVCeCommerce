using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public string Image { get; set; }
    }
}