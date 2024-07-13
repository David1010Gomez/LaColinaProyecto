using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColinaApplication.Dian.Entity
{
    public class Producto
    {
        public string id { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public int account_groupSend { get; set; }

        public Account_group account_group { get; set; }

        //public string unit_label { get; set; }

        //public string description { get; set; }

        public List<Taxes> taxes { get; set; }

        public List<Prices> prices { get; set; }

    }
    public class Account_group
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool active{ get; set; }
    }
    public class Taxes
    {
        public int id { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public decimal percentage { get; set; }
    }
    public class Prices
    {
        public string currency_code { get; set; }

        public List<PriceList> price_list { get; set; }
    }
    public class PriceList
    {
        public int position { get; set; }
        public string name { get; set; }

        public int value { get; set; }
    }
    public class ModelProduct
    {
        public List<Producto> results { get; set; }
    }
}