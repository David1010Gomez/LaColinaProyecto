using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColinaApplication.Dian.Entity
{
    public class Cliente
    {
        public string id { get; set; }

        public string person_type { get; set; }

        public Id_type id_type { get; set; }

        public string identification { get; set; }

        public string check_digit { get; set; }

        public List<string> name { get; set; }

        public string commercial_name { get; set; }

        public bool vat_responsible { get; set; }

        public List<Fiscal_responsibilities> fiscal_responsibilities { get; set; }

        public Address address { get; set; }

        public List<Contacts> contacts { get; set; }
        public int cantidadClientes { get; set; }
    }

    public class Id_type
    {
        public string code { get; set; }

        public string name { get; set; }
    }
    public class Fiscal_responsibilities
    {
        public string code { get; set; }

        public string name { get; set; }
    }
    public class Address
    {
        public string address { get; set; }

        public City city { get; set; }
        public string postal_code { get; set; }
    }
    public class City
    {
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string state_code { get; set; }
        public string state_name { get; set; }
        public string city_code { get; set; }

        public string city_name { get; set; }
    }
    public class Contacts
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string email { get; set; }

        public Phone phone { get; set; }
    }
    public class Phone
    {
        public string indicative { get; set; }

        public string number { get; set; }
    }
    public class ModelClient
    {
        public Pagination pagination { get; set; }
        public List<Cliente> results { get; set; }
    }
}