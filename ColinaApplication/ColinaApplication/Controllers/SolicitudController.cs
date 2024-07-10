using ColinaApplication.Data.Business;
using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ColinaApplication.Controllers
{
    [Expiring_Filter]
    public class SolicitudController : Controller
    {
        SolicitudBsuiness solicitud;
        Encriptacion encriptacion;
        public SolicitudController()
        {
            solicitud = new SolicitudBsuiness();
            encriptacion = new Encriptacion();
        }

        [HttpGet]
        public ActionResult SeleccionarMesa()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Pedido(string Id)
        {
            if(Id != "")
            {
                byte[] Texto = Convert.FromBase64String(Id);
                var id = encriptacion.DesEncriptar(Texto);
                TBL_SOLICITUD model = new TBL_SOLICITUD();
                model.ID_MESA = Convert.ToDecimal(id);
                return View(model);
            }
            else
            {
                return View();
            }
        }
        public JsonResult ListaCategorias()
        {
            var jsonResult = Json(JsonConvert.SerializeObject(solicitud.ListaCategorias()), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult ListaProductos(string IdProducto)
        {
            var jsonResult = Json(JsonConvert.SerializeObject(solicitud.ListaProductos(Convert.ToDecimal(IdProducto))), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult Encriptar(string Texto)
        {
            var jsonResult = Json(JsonConvert.SerializeObject(encriptacion.Encriptar(Texto)), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult ListaMeseros()
        {
            var jsonResult = Json(JsonConvert.SerializeObject(solicitud.ListaMeseros()), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult ConsultaClienteDian(string Cedula)
        {
            var jsonResult = Json(JsonConvert.SerializeObject(solicitud.ConsultaCedula(Cedula)), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}