using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColinaApplication.Dian.Entity
{
    public class Factura
    {
        public string id { get; set; }
        public Document document { get; set; }
        public string date { get; set; }
        public Customer customer { get; set; }
        public int cost_center { get; set; }
        public Currency currency { get; set; }
        public int seller { get; set; }
        public Stamp stamp { get; set; }
        public Mail mail { get; set; }
        public string observations { get; set; }
        public List<Items> items { get; set; }
        public List<Payments> payments { get; set; }
        public List<Globaldiscounts> globaldiscounts { get; set; }


    }
    public class Document
    {
        public int id { get; set; }
    }
    public class Customer
    {
        public string person_type { get; set; }
        public string id_type { get; set; }
        public string identification { get; set; }
        public int branch_office { get; set; }
        public List<string> name { get; set; }
        public Address address { get; set; }
        public List<Phone> phone { get; set; }
        public List<Contacts> contacts { get; set; }
    }
    public class Currency
    {
        public string code { get; set; }
        public int exchange_rate { get; set; }
    }
    public class Stamp
    {
        public bool send { get; set; }
    }
    public class Mail
    {
        public bool send { get; set; }
    }
    public class Items
    {
        public string code { get; set; }
        public string description { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public List<Taxes> taxes { get; set; }

    }
    public class Payments
    {
        public string id { get; set; }
        public string name { get; set; }
        public decimal value { get; set; }
        public string due_date { get; set; }
    }
    public class Globaldiscounts
    {
        public int id { get; set; }
        public decimal percentage { get; set; }
        public int value { get; set; }
    }
    
}