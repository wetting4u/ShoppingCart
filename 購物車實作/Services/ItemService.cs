using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using 購物車實作.Models;

namespace 購物車實作.Services
{
    public class ItemService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["Trolley"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        //藉由編號取得商品資料的方法
        public Item GetDataById(int Id)
        {
            //回傳根據編號所取得的資料
            Item Data = new Item();
            string sql = $@" SELECT * FROM Item WHERE Id = {Id};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Price = Convert.ToInt32(dr["Price"]);
            }
            catch(Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        public List<int> GetIdList(ForPaging Paging)
        {
            //計算所需的總頁面
            SetMaxPaging(Paging);
            //取得資料庫中的Item資料表
            List<int> IdList = new List<int>();
            string sql = $@" SELECT Id FROM (SELECT row_number() OVER(order by Id desc) AS sort,* FROM Item ) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    IdList.Add(Convert.ToInt32(dr["Id"]));
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return IdList;
        }
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            string sql = $@" SELECT * FROM Item; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Row++;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        public void Insert(Item newData)
        {
            string sql = $@" INSERT INTO Item(Name,Price,Image) VALUES ('{newData.Name}',{newData.Price},'{newData.Image}')";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e )
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public void Delete(int Id)
        {
            //根據商品編號刪除資料
            //先將CartBuy的資料刪除才刪Item
            //使用StringBuilder方法一次建立SQL做使用
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($@" DELECT FROM CartBuy WHERE Item_Id = {Id};");
            sql.AppendLine($@" DELETE FROM Item WHERE Id ={Id};");
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
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