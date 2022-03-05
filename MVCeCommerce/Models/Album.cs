using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Models
{
    public class Album
    {
        //編號
        [DisplayName("編號")]
        public int Alb_Id { get; set; }

        //檔名
        [DisplayName("檔名")]
        public string FileName { get; set; }


        //路徑
        [DisplayName("路徑")]
        public string Url { get; set; }

        [DisplayName("大小（Byte）")]
        public int Size { get; set; }
        public string Type { get; set; }

        [DisplayName("上傳者")]
        public string Account { get; set; }

        [DisplayName("上傳時間")]
        public DateTime CreateTime { get; set; }

        //Members資料表(外來鍵)
        //預設時將Members物件建立完畢
        public Members Member { get; set; } = new Members();
    }
}