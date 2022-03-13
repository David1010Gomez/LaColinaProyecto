﻿using ColinaApplication.Data.Business;
using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using ColinaApplication.Hubs;
using Entity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ColinaApplication.Controllers
{
    [Expiring_Filter]
    public class VentasController : Controller
    {
        VentasBusiness ventas;
        public VentasController()
        {
            ventas = new VentasBusiness();
        }

        [HttpGet]
        public ActionResult Ingresos()
        {
            SuperViewModels model = new SuperViewModels();
            var Text = UltimoCierre().Split(';');
            ViewBag.ultimoCierre = Text[0];
            if (Text.Length > 1)
                ViewBag.ultimaFecha = Text[1];
            return View(model);
        }
        [HttpPost]
        public ActionResult CambiarEstadoCaja(string Cierre)
        {
            List<TBL_MASTER_MESAS> mesas = new List<TBL_MASTER_MESAS>();
            switch (Cierre)
            {
                case "ABRIR CAJA":
                    mesas = ventas.ConsultaMesasCargo(Convert.ToDecimal(Session["IdUsuario"].ToString()));
                    foreach (var item in mesas)
                    {
                        TBL_MASTER_MESAS model = new TBL_MASTER_MESAS();
                        model.ID = item.ID;
                        model.ESTADO = Estados.Libre;
                        ventas.ActualizaEstadoMesa(model);
                    }
                    TBL_CIERRES modelCierres = new TBL_CIERRES();
                    modelCierres.FECHA_HORA_APERTURA = DateTime.Now;
                    modelCierres.ID_USUARIO = Convert.ToDecimal(Session["IdUsuario"].ToString());
                    TempData["CierraCaja"] = ventas.InsertaNuevoCierre(modelCierres);
                    break;

                case "CERRAR CAJA":
                    var solicitudes = ventas.ConsultaSolicitudes(Convert.ToDecimal(Session["IdUsuario"].ToString()), ventas.CierreUsuarioId(Convert.ToDecimal(Session["IdUsuario"].ToString())).FECHA_HORA_APERTURA);
                    if (solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Abierta || x.ESTADO_SOLICITUD == Estados.Llevar).ToList().Count == 0)
                    {
                        mesas = ventas.ConsultaMesasCargo(Convert.ToDecimal(Session["IdUsuario"].ToString()));
                        foreach (var item in mesas)
                        {
                            TBL_MASTER_MESAS model = new TBL_MASTER_MESAS();
                            model.ID = item.ID;
                            model.ESTADO = Estados.Cerrado;
                            ventas.ActualizaEstadoMesa(model);
                        }
                        TBL_CIERRES modelCierres2 = new TBL_CIERRES();
                        var ultimocierre = ventas.CierreUsuarioId(Convert.ToDecimal(Session["IdUsuario"].ToString()));
                        modelCierres2.ID = ultimocierre.ID;
                        modelCierres2.FECHA_HORA_CIERRE = DateTime.Now;
                        //modelCierres2.ID_USUARIO = Convert.ToDecimal(Session["IdUsuario"].ToString());
                        modelCierres2.CANT_MESAS_ATENDIDAS = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido && x.ESTADO_SOLICITUD != Estados.Inhabilitar).Count();
                        modelCierres2.CANT_FINALIZADAS = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Finalizada).Count();
                        modelCierres2.TOTAL_FINALIZADAS = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Finalizada).Sum(a => a.TOTAL);
                        modelCierres2.CANT_LLEVAR = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Llevar).Count();
                        modelCierres2.TOTAL_LLEVAR = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Llevar).Sum(a => a.TOTAL);
                        modelCierres2.CANT_CANCELADAS = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.CancelaPedido).Count();
                        modelCierres2.TOTAL_CANCELADAS = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.CancelaPedido).Sum(a => a.TOTAL);
                        modelCierres2.CANT_CONSUMO_INTERNO = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.ConsumoInterno).Count();
                        modelCierres2.TOTAL_CONSUMO_INTERNO = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.ConsumoInterno).Sum(a => a.TOTAL);
                        modelCierres2.OTROS_COBROS_TOTAL = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.OTROS_COBROS);
                        modelCierres2.DESCUENTOS_TOTAL = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.DESCUENTOS);
                        modelCierres2.IVA_TOTAL = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.IVA_TOTAL);
                        modelCierres2.I_CONSUMO_TOTAL = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.I_CONSUMO_TOTAL);
                        modelCierres2.SERVICIO_TOTAL = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido).Sum(a => a.SERVICIO_TOTAL);
                        modelCierres2.TOTAL_EFECTIVO = solicitudes.Where(x => x.CANT_EFECTIVO > 0).Sum(a => a.CANT_EFECTIVO);
                        var cuentasVouchers = solicitudes.Where(x => x.VOUCHER != "0" && x.VOUCHER != "" && x.VOUCHER != null).Sum(a => a.TOTAL);
                        if (cuentasVouchers != null && cuentasVouchers > 0)
                            modelCierres2.TOTAL_TARJETA = cuentasVouchers - (solicitudes.Where(x => x.VOUCHER != "0" && x.VOUCHER != "" && x.VOUCHER != null).Sum(a => a.CANT_EFECTIVO));
                        else
                            modelCierres2.TOTAL_TARJETA = 0;
                        modelCierres2.VENTA_TOTAL = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido).Sum(a => a.TOTAL);
                        var resultado = ventas.ActualizaCierre(modelCierres2);
                        var imprimir = ventas.ImprimirCierre(solicitudes, Convert.ToDecimal(Session["IdUsuario"].ToString()));
                        if (resultado && imprimir)
                            TempData["CierraCaja"] = true;
                        else
                            TempData["CierraCaja"] = false;
                    }
                    else
                    {
                        TempData["CierraCaja"] = "Mesas Abiertas";
                    }
                    break;

                default:
                    break;
            }
            return RedirectToAction("Ingresos");
        }
        [HttpPost]
        public ActionResult ConsultaFechas(DateTime fechaInicial, DateTime fechaFinal, string Consulta)
        {
            var Text = UltimoCierre().Split(';');
            ViewBag.ultimoCierre = Text[0];
            if (Text.Length > 1)
                ViewBag.ultimaFecha = Text[1];
            SuperViewModels model = new SuperViewModels();
            fechaInicial = fechaInicial.Date.Add(new TimeSpan(0, 0, 0));
            fechaFinal = fechaFinal.Date.Add(new TimeSpan(23, 59, 59));
            if (Consulta == "Consultar")
            {
                model.Solicitudes = ventas.ConsultaSolicitudesXFecha(fechaInicial, fechaFinal);
                return View("Ingresos", model);
            }
            else
            {
                model.Solicitudes = ventas.ConsultaSolicitudesXFecha(fechaInicial, fechaFinal);
                var resultadofinal = GenerarReporte(model.Solicitudes);
                //return File(resultadofinal[0].Value, "application / csv; charset = utf - 8", resultadofinal[0].Key);                
                return File(resultadofinal.Value, "application / csv; charset = utf - 8", resultadofinal.Key);
            }
        }
        [HttpPost]
        public ActionResult ConsultaFactura(string NumeroFactura)
        {
            var Text = UltimoCierre().Split(';');
            ViewBag.ultimoCierre = Text[0];
            if (Text.Length > 1)
                ViewBag.ultimaFecha = Text[1];
            SuperViewModels model = new SuperViewModels();
            model.SolicitudModel = ventas.ConsultaSolicitudXId(Convert.ToDecimal(NumeroFactura));
            if (model.SolicitudModel == null)
            {
                model.SolicitudModel = new ConsultaSolicitud();
                model.SolicitudModel.ProductosSolicitud = new List<ProductosSolicitud>();
            }

            return View("Ingresos", model);

        }
        [HttpGet]
        public ActionResult Gastos()
        {
            SuperViewModels model = new SuperViewModels();
            model.Nomina = ventas.ConsultaNomina();
            return View(model);
        }
        public JsonResult CalcularPago(decimal IdNomina)
        {
            var jsonResult = Json(JsonConvert.SerializeObject(ventas.CalcularPagos(IdNomina)), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult AsignarDiaTrabajo(decimal IdNomina, DateTime FechaTrabajada, decimal SueldoDiario, decimal IdPerfil)
        {
            var jsonResult = Json(JsonConvert.SerializeObject(ventas.AsignaDiaTrabajo(IdNomina, FechaTrabajada, SueldoDiario, IdPerfil)), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult LiquidarUsuario(decimal IdNomina)
        {
            var jsonResult = Json(JsonConvert.SerializeObject(ventas.LiquidarUsuario(IdNomina)), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public string UltimoCierre()
        {
            var ultimocierre = ventas.CierreUsuarioId(Convert.ToDecimal(Session["IdUsuario"].ToString()));
            string respuesta = "";
            if (ultimocierre != null)
            {
                if (ultimocierre.FECHA_HORA_CIERRE == null)
                    respuesta = "CERRAR CAJA";
                else
                    respuesta = "ABRIR CAJA;" + ultimocierre.FECHA_HORA_CIERRE;
            }
            else
            {
                respuesta = "ABRIR CAJA";
            }
            return respuesta;
        }
        public KeyValuePair<string, byte[]> GenerarReporte(List<ConsultaSolicitud> consulta)
        {
            string nombreArchivo = string.Empty;
            IEnumerable<object> registrosConsultados;
            IEnumerable<object> registrosConsultados2;
            registrosConsultados = consulta;
            registrosConsultados2 = consulta[0].ProductosSolicitud;
            nombreArchivo = "ReporteSolicitudes";
            var archivoCsvFinal = GenerarArchivoCsvExport<object>
                                (nombreArchivo, registrosConsultados, registrosConsultados2);

            return archivoCsvFinal;
        }
        public static KeyValuePair<string, byte[]> GenerarArchivoCsvExport<T>(string nombreArchivo, IEnumerable<T> registros, IEnumerable<T> registros2)
        {
            byte[] bytesArchivoCsv;
            var archivoCsv = new Data.Clases.CsvExport();
            var archivoCsv2 = new Data.Clases.CsvExport();
            archivoCsv.AddRows(registros);
            archivoCsv.AddRow();
            archivoCsv2.AddRows(registros2);
            var Hoja1 = archivoCsv.Export();
            Hoja1 += archivoCsv2.Export();

            bytesArchivoCsv = Encoding.ASCII.GetBytes(Hoja1);
            
            var archivoCsvFinal = new KeyValuePair<string, byte[]>(nombreArchivo, bytesArchivoCsv);
            return archivoCsvFinal;
        }

        public JsonResult ReImprimir()
        {
            var solicitudes = ventas.ConsultaSolicitudes(Convert.ToDecimal(Session["IdUsuario"].ToString()), ventas.CierreUsuarioId(Convert.ToDecimal(Session["IdUsuario"].ToString())).FECHA_HORA_APERTURA);
            var jsonResult = Json(JsonConvert.SerializeObject(ventas.ImprimirCierre(solicitudes, Convert.ToDecimal(Session["IdUsuario"].ToString()))), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult ImprimirParcial()
        {
            var solicitudes = ventas.ConsultaSolicitudes(Convert.ToDecimal(Session["IdUsuario"].ToString()), ventas.CierreUsuarioId(Convert.ToDecimal(Session["IdUsuario"].ToString())).FECHA_HORA_APERTURA);
            var jsonResult = Json(JsonConvert.SerializeObject(ventas.ImprimirCierreParcial(solicitudes, Convert.ToDecimal(Session["IdUsuario"].ToString()))), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}
