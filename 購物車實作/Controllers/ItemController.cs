using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using 購物車實作.Services;
using 購物車實作.ViewModels;

namespace 購物車實作.Controllers
{
    public class ItemController: Controller
    {
        private readonly CartService cartService = new CartService();
        private readonly ItemService itemService = new ItemService();

        public ActionResult Index(int page = 1)
        {
            ItemViewModel Data = new ItemViewModel();
            Data.Paging = new ForPaging(page);
            Data.IdList = itemService.GetIdList(Data.Paging);
            Data.ItemBlock = new List<ItemDetailViewModel>();
            foreach (var Id in Data.IdList)
            {
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                newBlock.Data = itemService.GetDataById(Id);
                //取得Session內購物車資料
                string Cart = (HttpContext.Session["Cart"] != null)
                    ? HttpContext.Session["Cart"].ToString() : null;
                newBlock.InCart = cartService.CheckInCart(Cart, Id);
                Data.ItemBlock.Add(newBlock);
            }
            return View(Data);
        }
        public ActionResult Item(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null)
                    ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return View(ViewData);
        }
        public ActionResult ItemBlock(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null)
                ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return PartialView(ViewData);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Add(ItemCreateViewModel Data)
        {
            if(Data.ItemImage !=null)
            {
                string filename = Path.GetFileName(Data.ItemImage.FileName);
                string Url = Path.Combine(Server.MapPath("~/Upload/"), filename);
                Data.ItemImage.SaveAs(Url);
                Data.NewData.Image = filename;
                //使用Service來新增一筆商品資料
                itemService.Insert(Data.NewData);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("ItemImage", "請選擇上傳檔案");
                return View(Data);
            }
        }
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int Id)
        {
            itemService.Delete(Id);
            return RedirectToAction("Index");

        }
    }
}