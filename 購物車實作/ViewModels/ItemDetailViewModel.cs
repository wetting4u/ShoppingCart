using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using 購物車實作.Models;

namespace 購物車實作.ViewModels
{
    public class ItemDetailViewModel
    {
        //新增商品內容
        public Item Data { get; set; }
        //是否在購物車中
        public bool InCart { get; set; }
    }
}