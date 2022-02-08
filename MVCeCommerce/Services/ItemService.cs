using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace MVCeCommerce.Services
{
    public class ItemService
    {
        //連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP_MVCeCommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得單一商品資料
        public Item GetDataById(int Id)
        {
            Item Data = new Item();

            string sql = $@" select * from Item where Id = '{Id}'";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql , conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Name = dr["Name"].ToString();
                Data.Price = Convert.ToInt32(dr["Price"]);
                Data.Image = dr["Image"].ToString();
            }catch(Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 取得商品編號陣列
        public List<int> GetIdList(ForPaging Paging)
        {
            //計算所需總頁面
            SetMaxPaging(Paging);

            List<int> IdList = new List<int>();

            string sql = $@" select Id from (select row_number() over(order by Id desc) as sort, * from Item) m where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1 } and {Paging.NowPage * Paging.ItemNum}";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    IdList.Add(Convert.ToInt32(dr["Id"]));
                }
            }catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return IdList;
        }
        

        #region 設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;

            string sql = $@"select * from Item";
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
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion
        #endregion

        #region 新增商品
        public void Insert(Item newData)
        {
            newData.Id = LastItemFinder();

            string sql = $@"insert into Item(Id, Name, Price, Image) VALUES ({newData.Id},'{newData.Name}', '{newData.Price}','{newData.Image}'";
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

        #region 計算目前商品最新一筆的ID
        public int LastItemFinder()
        {
            //宣告回傳值
            int Id;

            string sql = $@"select top 1 * from Item order by Id DESC ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Id"]);
            }catch(Exception e)
            {
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion

        #region 刪除商品
        public void Delete(int Id)
        {
            //刪除CartBuy
            //根據商品編號刪除資料
            //先刪CartBuy再刪Item
            StringBuilder sql = new StringBuilder();

            sql.Append($@"Delete from CartBuy where Item_Id = {Id}");
            sql.Append($@"Delete from Item where Id = {Id}");
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
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
    }
}