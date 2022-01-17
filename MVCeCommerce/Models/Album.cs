using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Models
{
    public class Album
    {
        //編號
        public int Alb_Id { get; set; }
        //檔名
        public string FileName { get; set; }
        //路徑
        public string Url { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public string Account { get; set; }
        public DateTime CreateTime { get; set; }

        //Members資料表(外來鍵)
        //預設時將Members物件建立完畢
        public Members Member {get;set;}
    }
}