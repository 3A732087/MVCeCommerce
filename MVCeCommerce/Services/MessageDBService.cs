using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Services
{
    public class MessageDBService
    {
        //連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP_MVCeCommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢留言陣列
        public List<Message> GetDataList(ForPaging Paging, int A_Id)
        {
            List<Message> DataList = new List<Message>();
            SetMaxPaging(Paging, A_Id);
            DataList = GetAllDataList(Paging, A_Id);

            return DataList;
        }
        #endregion

        #region 設定頁數
        //設定最大頁數
        public void SetMaxPaging(ForPaging Paging, int A_Id)
        {
            //計算列數
            int Row = 0;

            string sql = $@"select * from Message where A_Id = {A_Id}";
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
            catch (Exception e)
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

        #region 取得Message資料
        public List<Message> GetAllDataList(ForPaging paging, int A_Id)
        {
            List<Message> DataList = new List<Message>();

            string sql = $@" select m.*, d.Name from (select row_number() over(order by M_Id) as sort, * from Message where A_Id = {A_Id}) m inner join Members d on m.Account = d.Account where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum}";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Message Data = new Message();
                    Data.M_Id = Convert.ToInt32(dr["M_Id"]);
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
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

        #region 新增留言
        public void InsertMessage(Message newData)
        {
            newData.M_Id = LastMessageFinder(newData.A_Id);
            string sql = $@" INSERT INTO Message(A_Id, M_Id,  Account, Content, CreateTime) VALUES ({newData.A_Id}, {newData.M_Id},'{newData.Account}','{newData.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}')";

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
        #endregion

        #region 計算目前最新一筆M_Id
        public int LastMessageFinder(int A_Id)
        {
            //宣告回傳值
            int Id;

            string sql = $@"select top 1 * from Message where A_Id = {A_Id} order by M_Id DESC ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["M_Id"]);
            }
            catch (Exception e)
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

        #region 修改留言
        public void UpdateMessage(Message UpdateData)
        {
            string sql = $@" update Message set Content = '{UpdateData.Content}' where A_Id = {UpdateData.A_Id} and M_Id = {UpdateData.M_Id}";

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
        #endregion

        #region 刪除文章
        public void DeleteMessage(int A_Id, int M_Id)
        {
            string DeleteMessage = $@" delete from Message where A_Id = {A_Id} and M_Id = {M_Id}";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(DeleteMessage, conn);
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
        #endregion

    }
}