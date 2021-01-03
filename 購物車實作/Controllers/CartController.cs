using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 購物車實作.Services;
using 購物車實作.ViewModels;

namespace 購物車實作.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService cartService = new CartService();
        [Authorize]
        public ActionResult Index()
        {
            //宣告一個新頁面模型
            CartBuyViewModel Data = new CartBuyViewModel();
            //取得Session內購物車資料
            string Cart = (HttpContext.Session["cart"] != null)
                ? HttpContext.Session["Cart"].ToString() : null;
            //藉由Service並根據Session內儲存購物車編號
            //取得已放入的商品資料陣列
            Data.DataList = cartService.GetItemFromCart(Cart);
            //藉由Service來確認購物車是否已保存
            Data.isCartsave = cartService.CheckCartSave(User.Identity.Name, Cart);
            return View(Data);
        }
        [Authorize]
        public ActionResult CartSave()
        {
            //宣告接收購物車Sesion資料物件
            string Cart;
            //判斷Session內是否有值
            if (HttpContext.Session["Cart"] != null)
            {
                //設定購物車值
                Cart = HttpContext.Session["Cart"].ToString();
            }
            else
            {
                //重新定義購物車值
                Cart = DateTime.Now.ToString() + User.Identity.Name;
                //填入Session中
                HttpContext.Session["Cart"] = Cart;
            }
            //藉由Service來儲存購物車資料
            cartService.CartSave(User.Identity.Name, Cart);
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult CartSaveRemove()
        {
            cartService.SaveCartRemove(User.Identity.Name);
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult Pop(int Id, string toPage)
        {
            //取得Session內購物車資料
            string Cart = (HttpContext.Session["cart"] != null)
                ? HttpContext.Session["Cart"].ToString() : null;
            cartService.RemoveForCart(Cart, Id);
            //藉由Service來將商品從購物車取出
            if (toPage == "Item")//判斷傳入的toPage來決定導向
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [Authorize]
        public ActionResult Put(int Id, string toPage)
        {
            //若Session中無購物車資料，以使用者名稱與時間，新增一購物車資料
            if (HttpContext.Session["Cart"] == null)
            {
                HttpContext.Session["Cart"] = DateTime.Now.ToString() + User.Identity.Name;
            }
            cartService.AddToCart(HttpContext.Session["Cart"].ToString(), Id);
            //判斷傳入的toPage來決定導向
            if (toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}