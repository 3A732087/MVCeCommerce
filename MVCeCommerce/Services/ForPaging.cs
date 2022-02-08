using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeCommerce.Services
{
    public class ForPaging
    {
        //當前頁數
        public int NowPage { get; set; }

        //最大頁數
        public int MaxPage { get; set; }

        //分頁項目個數，設唯獨
        //之後修改個數只需修改此地方
        public int ItemNum
        {
            get
            {
                return 5;
            }
        }

        //類別建構式
        public ForPaging()
        {
            //當前頁數預設為1
            this.NowPage = 1;
        }

        //此類別建構式傳入頁數
        public ForPaging(int Page)
        {
            this.NowPage = Page;
        }

        //設定正確頁數的方法，以避免傳入不正確的值
        public void SetRightPage()
        {
            //判斷當前頁數是否小於1
            if (this.NowPage < 1)
            {
                this.NowPage = 1;
            }else if(this.NowPage > this.MaxPage){
                this.NowPage = this.MaxPage;
            }

            if (this.MaxPage.Equals(0))
            {
                this.NowPage = 1;
            }

        }

    }
}