using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using 購物車實作.Services;

namespace 購物車實作.ViewModels
{
    public class ItemViewModel
    {
        public List<int> IdList { get; set; }
        //商品區塊
        public List<ItemDetailViewModel> ItemBlock { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
    }
}