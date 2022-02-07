using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Services
{
    public class CartService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP_MVCeCommerce"].ConnectionString;

        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得購物車內商品陣列
        public List<CartBuy> GetItemFromCart(string Cart)
        {
            //宣告要回傳的搜尋資料為資料庫中的CartBuy資料表
            List<CartBuy> DataList = new List<CartBuy>();

            string sql = $@"select * from CartBuy m inner join Item d on m.Item_Id = d.Id where Cart_id = '{Cart}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CartBuy Data = new CartBuy();
                    Data.Cart_Id = dr["Cart_id"].ToString();
                    Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                    Data.Item.Id = Convert.ToInt32(dr["Id"]);
                    Data.Item.Name = dr["Name"].ToString();
                    Data.Item.Image = dr["Image"].ToString();
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
        #endregion

        #region 確認商品是否於購物車中
        public bool CheckInCart (string Cart, int Item_Id)
        {
            //宣告要回傳的資料為CartBuy資料
            CartBuy Data = new CartBuy();

            //Sql語法
            //根據購物車與商品編號取得CartBuy資料表內的資料
            string sql = $@"select * from CartBuy m inner join Item d on m.Item_id = d.Id where Cart_id = '{Cart}' and Item_Id = '{Item_Id}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                Data.Item.Id = Convert.ToInt32(dr["Id"]);
                Data.Item.Name = dr["Name"].ToString();
                Data.Item.Image = dr["Image"].ToString();
                Data.Item.Price = Convert.ToInt32(dr["Price"]);

            }
            catch(Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data != null);
        }
        #endregion

        #region 放入購物車
        public void AddtoCart (string Cart, int Item_Id)
        {
            string sql = $@" insert into CartBuy (Cart_Id, Item_Id) values ('{Cart}', '{Item_Id}')";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 移除購物車
        public void RemoveForCart (string Cart, int Item_Id)
        {
            string sql = $@"delete from CartBuy where Cart_Id = '{Cart}' and Item_Id = '{Item_Id}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 確認購物車是否有保存
        public bool CheckCartSave(string Account, string Cart)
        {
            CartSave Data = new CartSave();
            string sql = $@" select * from CartSave m inner join Members d on d.Account = m.Account where m.Account = '{Account}' and Cart_Id = '{Cart}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Member.Name = dr["Name"].ToString();
            }catch(Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data != null);
        }
        #endregion

        #region 取得購物車保存
        public string GetCartSave(string Account)
        {
            CartSave Data = new CartSave();

            string sql = $@" select * from CartSave m inner join Members d on m.Account = d.Account where m.Account = '{Account}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Member.Name = dr["Name"].ToString();
            }catch(Exception e)
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
        #endregion

        #region 保存購物車
        public void SaveCart(string Account, string Cart)
        {
            string sql = $@" insert into CartSave (Account, Cart_Id) values ('{Account}','{Cart}')";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 移除保存購物車
        public void SaveCartRemove(string Account)
        {
            string sql = $@" delete from CartSave where Account = '{Account}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion
    }
}