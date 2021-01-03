using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using 購物車實作.Models;

namespace 購物車實作.Services
{
    public class CartService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["Trolley"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        //取得於購物車內的商品陣列
        public List<CartBuy> GetItemFromCart(string Cart)
        {
            List<CartBuy> DataList = new List<CartBuy>();
            string sql = $@" SELECT * FROM CartBuy m INNER JOIN Item d ON m.Item_Id = d.Id WHERE Cart_Id = '{Cart}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CartBuy Data = new CartBuy();
                    Data.Cart_Id = dr["Cart_Id"].ToString();
                    Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                    Data.Item.Id = Convert.ToInt32(dr["Id"]);
                    Data.Item.Image = dr["Image"].ToString();
                    Data.Item.Name = dr["Name"].ToString();
                    Data.Item.Price = Convert.ToInt32(dr["Price"]);
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        public bool CheckInCart(string Cart, int Item_Id)
        {
            //選告要回傳的搜尋資料為資料庫中的CartBuy資料
            CartBuy Data = new CartBuy();
            string sql = $@" SELECT * FROM CartBuy m INNER JOIN Item d ON m.Item_Id = d.Id WHERE Cart_Id = '{Cart}' AND Item_Id = '{Item_Id}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Cart_Id = dr["Cart_id"].ToString();
                Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                Data.Item.Id = Convert.ToInt32(dr["Id"]);
                Data.Item.Image = dr["Image"].ToString();
                Data.Item.Name = dr["Name"].ToString();
                Data.Item.Price = Convert.ToInt32(dr["Price"]);
            }
            catch
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data != null);
        }
        public void AddToCart(string Cart, int Item_Id)
        {
            string sql = $@" INSERT INTO CartBuy(Cart_Id,Item_Id) VALUES ('{Cart}',{Item_Id};)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public void RemoveForCart(string Cart, int Item_Id)
        {
            string sql = $@" DELETE FROM CartBuy WHERE Cart_Id = '{Cart}' AND Item_Id = {Item_Id};)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public bool CheckCartSave(string Account, string Cart)
        {
            CartSave Data = new CartSave();
            string sql = $@" SELECT * FROM CartSave m INNER JOIN Members d ON m.Account = d.Account WHERE m.Account = '{Account}' AND Cart_Id = '{Cart}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Member.Name = dr["Name"].ToString();
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data != null);
        }
        public string GetCartSave(string Account)
        {
            CartSave Data = new CartSave();
            string sql = $@" SELECT * FROM CartSave m INNER JOIN Members d ON m.Account = d.Account WHERE m.Account = '{Account}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Member.Name = dr["Name"].ToString();
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            if (Data != null)
            {
                return Data.Cart_Id;
            }
            else
            {
                return null;
            }
        }
        public void CartSave(string Account, string Cart)
        {
            string sql = $@" INSERT INTO CartSave(Account,Cart_Id) VALUES ('{Account}','{Cart}');";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public void SaveCartRemove(string Account)
        {
            string sql = $@" DELETE FROM CartSave WHERE Account = '{Account}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
    }
}