using MVCeCommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Services
{
    public class AlbumDBService
    {
        //連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP_MVCeCommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 查詢一張相片
        public Album GetDataById(int Alb_Id)
        {
            Album Data = new Album();

            string sql = $@"select m.* d.Name from Album m inner join Members d on m.Account = d.Account where m.Alb_Id = {Alb_Id}";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                Data.FileName = dr["FileName"].ToString();
                Data.Size = Convert.ToInt32(dr["Size"]);
                Data.Url = dr["Url"].ToString();
                Data.Type = dr["Type"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
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
            return Data;
        }
        #endregion

        #region 查詢照片陣列資料
        public List<Album> GetDataList(ForPaging Paging)
        {
            SetMaxPaging(Paging);

            List<Album> DataList = new List<Album>();


            string sql = $@" select m.*, d.Name from (select row_number() over(order by CreateTime desc) as sort, * from Album ) m inner join Members d on m.Account = d.Account where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum}";


            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Album Data = new Album();
                    Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                    Data.FileName = dr["FileName"].ToString();
                    Data.Size = Convert.ToInt32(dr["Size"]);
                    Data.Account = dr["Account"].ToString();
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

        //設定最大頁數
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;

            string sql = $@"select * from Album";
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

        #region 上傳檔案
        public void UploadFile(int Alb_Id, string FileName, string Url, int Size, string Type, string Account)
        {
            string sql = $@" insert into Album (Alb_Id, FileName, Url, Size, Type, Account, CreateTime) VALUES ({Alb_Id}, '{FileName}', '{Url}', {Size}, '{Type}', '{Account}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";

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

        #region 計算目前最新一筆Alb_Id
        public int LastAlbumFinder()
        {
            //宣告回傳值
            int Id;

            string sql = $@"select top 1 * from Album order by Alb_Id DESC ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Alb_Id"]);
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

        #region 刪除文章
        public void DeleteArticle(int Alb_Id)
        {
            string sql = $@" delete from Album where Alb_Id = {Alb_Id}";

            try
            {
                Album Data = GetDataById(Alb_Id);
                File.Delete(Data.Url);
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
    }
}