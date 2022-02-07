using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.ViewModel
{
    public class CartBuyViewModel
    {
        //購物車內商品陣列
        public List<CartBuy> DataList { get; set; }
        //購物車是否以保存
        public bool isCartsave { get; set; }
    }
}