using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using ColinaApplication.Dian.Entity;
using Newtonsoft.Json;
using ColinaApplication.Data.Conexion;
using DocumentFormat.OpenXml.Wordprocessing;
using ColinaApplication.Data.Clases;

namespace ColinaApplication.Dian
{
    public class BusinessDian
    {
        public Token GeneraToken()
        {   
            Token token = new Token();
            try
            {
                Task<string> strObj = null;
                using (var httpClient = new HttpClient())
                {
                    string url = "https://api.siigo.com/auth";
                    var body = "{ \"username\": \""+ ConfigurationManager.AppSettings["USUARIOSIIGO"] + "\", \"access_key\": \""+ ConfigurationManager.AppSettings["CONTRASIIGO"] +"\"}";
                    body = body.Replace("\n", " ");
                    var data = new StringContent(body, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = httpClient.PostAsync(url, data).Result;

                    if (resp.IsSuccessStatusCode)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<Token>(strObj.Result);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return token;
        }

        public List<Producto> ConsultaProductosDian(string token)
        {
            List<Producto> respuesta = new List<Producto>();
            Task<string> strObj = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "products";
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;

                    HttpResponseMessage resp = httpClient.GetAsync(url).Result;

                    if (resp != null && resp.Content != null)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<ModelProduct>(strObj.Result).results;
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return respuesta;
        }
        public List<Account_group> ConsultaAccountGroupDian(string token)
        {
            List<Account_group> respuesta = new List<Account_group>();
            Task<string> strObj = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "account-groups";
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;

                    HttpResponseMessage resp = httpClient.GetAsync(url).Result;

                    if (resp != null && resp.Content != null)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<List<Account_group>>(strObj.Result);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return respuesta;
        }
        public Producto InsertaProducto(string token, Producto model)
        {
            Producto producto = new Producto();
            try
            {
                Task<string> strObj = null;
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "products";
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;
                    var body = "{ \"code\": \""+ model.code + "\", "+
                                 "\"name\":\"" + model.name + "\", "+
                                 "\"account_group\": "+ model.account_groupSend +","+
                                 "\"taxes\": [{ \"id\": " + model.taxes[0].id +", \"name\": \""+ model.taxes[0].name+ "\", \"type\": \""+ model.taxes[0].type+ "\", \"percentage\": "+ model.taxes[0].percentage + " }],"+
                                 "\"prices\": [{ \"currency_code\": \"" + model.prices[0].currency_code + "\", \"price_list\": [{ \"position\": " + model.prices[0].price_list[0].position +", \"value\": " + model.prices[0].price_list[0].value + "}] }] " + 
                               "}";
                    body = body.Replace("\n", " ");
                    var data = new StringContent(body, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = httpClient.PostAsync(url, data).Result;

                    if (resp.IsSuccessStatusCode)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        producto = JsonConvert.DeserializeObject<Producto>(strObj.Result);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return producto;
        }
        public Producto ConsultaProductosDianId(string token, string idProducto)
        {
            Producto respuesta = new Producto();
            Task<string> strObj = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "products/"+idProducto;
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;

                    HttpResponseMessage resp = httpClient.GetAsync(url).Result;

                    if (resp != null && resp.Content != null)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<Producto>(strObj.Result);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return respuesta;
        }
        public Producto ActualizarProducto(string token, Producto model)
        {
            Producto producto = new Producto();
            try
            {
                Task<string> strObj = null;
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "products/" + model.id;
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;
                    var body = "{ \"code\": \"" + model.code + "\", " +
                                 "\"name\":\"" + model.name + "\", " +
                                 "\"account_group\": " + model.account_groupSend + "," +
                                 "\"prices\": [{ \"currency_code\": \"" + model.prices[0].currency_code + "\", \"price_list\": [{ \"position\": " + model.prices[0].price_list[0].position + ", \"value\": " + model.prices[0].price_list[0].value + "}] }] " +
                               "}";
                    body = body.Replace("\n", " ");
                    var data = new StringContent(body, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = httpClient.PutAsync(url, data).Result;

                    if (resp.IsSuccessStatusCode)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        producto = JsonConvert.DeserializeObject<Producto>(strObj.Result);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return producto;
        }
        public Cliente InsertaCliente(string token, Cliente model)
        {
            Cliente cliente = new Cliente();
            try
            {
                Task<string> strObj = null;
                string fResponsibilities = string.Empty;
                var contador = 0;
                foreach (var item in model.fiscal_responsibilities)
                {
                    contador++;
                    fResponsibilities += "{ \"code\": \""+ item.code +"\" }";
                    if (contador != model.fiscal_responsibilities.Count)
                        fResponsibilities += ",";
                }
                string names = string.Empty;
                contador = 0;
                foreach (var item in model.name)
                {
                    contador++;
                    names += "\"" + item + "\"";
                    if (contador != model.name.Count)
                        names += ",";
                }
                if (model.name.Count == 1)
                {
                    model.name.Add("NA");
                }
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "customers";
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;
                   var body = "{ \"person_type\": \"" + model.person_type + "\", " +
                                 "\"id_type\":\"" + model.id_type.code + "\", " +
                                 "\"identification\": \"" + model.identification + "\"," +
                                 "\"check_digit\": \""+model.check_digit+"\"," +
                                 "\"name\": [ "+names+" ]," +                                 
                                 "\"commercial_name\":\"" + model.commercial_name + "\"," +
                                 "\"vat_responsible\": " + model.vat_responsible.ToString().ToLower() + ", " +
                                 "\"address\": { \"address\":\"" + model.address.address + "\"}, " +
                                 "\"contacts\": [{ \"first_name\": \"" + model.name[0] + "\", \"last_name\":\"" + model.name[1] + "\", \"email\": \""+ model.contacts[0].email + "\", \"phone\": { \"number\": \"" + model.contacts[0].phone.number +"\" }}]," +
                                 "\"fiscal_responsibilities\": ["+ fResponsibilities + "]" +
                               "}";
                    body = body.Replace("\n", " ");
                    var data = new StringContent(body, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = httpClient.PostAsync(url, data).Result;

                    if (resp.IsSuccessStatusCode)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        cliente = JsonConvert.DeserializeObject<Cliente>(strObj.Result);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
            return cliente;
        }
        public Factura InsertaFactura(string token, Factura model, decimal IdSolicitudMesa)
        {
            Factura factura = new Factura();
            TBL_FACTURAS_FALLIDAS_DIAN facturasFallidas = new TBL_FACTURAS_FALLIDAS_DIAN();
            try
            {
                Task<string> strObj = null;
                string items = string.Empty;
                var contador = 0;
                foreach (var item in model.items)
                {
                    contador++;
                    items += "{ \"code\": \"" + item.code + "\", \"description\":\"" + item.description + "\", \"taxes\": [{\"id\": " + item.taxes[0].id + " }], \"quantity\": \"" + item.quantity + "\", \"price\": " + item.price + "}";
                    if (contador != model.items.Count)
                        items += ",";
                }
                string payments = string.Empty;
                contador = 0;
                foreach (var item in model.payments)
                {
                    contador++;
                    payments += "{ \"id\": \"" + item.id + "\", \"value\": " + item.value + " }";
                    if (contador != model.payments.Count)
                        payments += ",";
                }
                string names = string.Empty;
                contador = 0;
                foreach (var item in model.customer.name)
                {
                    contador++;
                    names += "\"" + item + "\"";
                    if (contador != model.customer.name.Count)
                        names += ",";
                }
                if (model.customer.name.Count == 1)
                {
                    model.customer.contacts[0].last_name = "NA";
                }
                using (var httpClient = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["URLSIIGO"] + "invoices";
                    httpClient.DefaultRequestHeaders.Add("Partner-Id", "LaColinaPOS");
                    var header = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Authorization = header;
                    var body = "{ \"document\": {\"id\": " + model.document.id + "}, " +
                                 "\"date\":\"" + model.date + "\", " +
                                 "\"customer\": { \"person_type\": \""+ model.customer.person_type + "\", \"id_type\": \"" + model.customer.id_type +"\", \"identification\": \"" + model.customer.identification + "\", \"name\": ["+names+"], \"address\": { \"address\": \""+model.customer.address.address+"\" }, "+
                                                "\"phones\": [{ \"number\": \"" + model.customer.phone[0].number +"\" }], \"contacts\": [ { \"first_name\": \""+ model.customer.contacts[0].first_name + "\", \"last_name\": \""+ model.customer.contacts[0].last_name+ "\", \"email\": \""+ model.customer.contacts[0].email + "\"}] }," +
                                 "\"seller\": " + model.seller + ", " +
                                 "\"stamp\": {\"send\": "+model.stamp.send.ToString().ToLower()+" }," +
                                 "\"mail\": {\"send\": "+model.mail.send.ToString().ToLower() + "}," +
                                 "\"items\": ["+ items + "]," +
                                 "\"payments\": [" +payments+ "]," +
                                 "\"global_charges\": [{\"id\": 1, \"value\":" + model.globaldiscounts[0].value +" }]" +
                               "}";
                    body = body.Replace("\n", " ");
                    var data = new StringContent(body, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = httpClient.PostAsync(url, data).Result;

                    if (resp.IsSuccessStatusCode)
                    {
                        strObj = resp.Content.ReadAsStringAsync();
                        factura = JsonConvert.DeserializeObject<Factura>(strObj.Result);
                    }
                    else {
                        strObj = resp.Content.ReadAsStringAsync();                        
                        facturasFallidas.FECHA = DateTime.Now;
                        facturasFallidas.ID_SOLICITUD = IdSolicitudMesa;
                        facturasFallidas.BODY_JSON = body.ToString();
                        facturasFallidas.ERROR = strObj.Result;
                        bool r = InsertaFacturasFallidas(facturasFallidas);
                    }
                }
            }
            catch (Exception e)
            {
                facturasFallidas.FECHA = DateTime.Now;
                facturasFallidas.ID_SOLICITUD = IdSolicitudMesa;
                facturasFallidas.ERROR = e.Message.ToString() + " --- " + e.InnerException.ToString() + " --- " + e.Source + " **** " + e.ToString();
                bool r = InsertaFacturasFallidas(facturasFallidas);
            }
            finally
            {

            }
            return factura;
        }
        public bool InsertaFacturasFallidas(TBL_FACTURAS_FALLIDAS_DIAN model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_FACTURAS_FALLIDAS_DIAN.Add(model);
                    contex.SaveChanges();
                    Respuesta = true;
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }

            }
            return Respuesta;
        }
    }
}