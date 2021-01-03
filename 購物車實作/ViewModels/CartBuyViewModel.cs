using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using 購物車實作.Models;

namespace 購物車實作.ViewModels
{
    public class CartBuyViewModel
    {
        //購物車內商品陣列
        public List<CartBuy> DataList { get; set; }
        //購物車是否已保存
        public bool isCartsave { get; set; }
    }
}