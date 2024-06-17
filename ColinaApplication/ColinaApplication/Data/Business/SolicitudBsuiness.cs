﻿using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using Entity;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace ColinaApplication.Data.Business
{
    public class SolicitudBsuiness
    {
        public List<TBL_MASTER_MESAS> ListaMesas()
        {
            List<TBL_MASTER_MESAS> ListMesas = new List<TBL_MASTER_MESAS>();
            using (DBLaColina context = new DBLaColina())
            {
                ListMesas = context.TBL_MASTER_MESAS.ToList();
            }
            return ListMesas;
        }
        public List<ConsultaSolicitudGeneral> ConsultaSolicitudMesa(decimal IdMesa)
        {
            List<ConsultaSolicitudGeneral> solicitudMesa = new List<ConsultaSolicitudGeneral>();
            using (DBLaColina context = new DBLaColina())
            {
                var ConsultaSolicitud = context.TBL_SOLICITUD.Where(a => a.ID_MESA == IdMesa && (a.ESTADO_SOLICITUD == Estados.Abierta || a.ESTADO_SOLICITUD == Estados.Llevar)).ToList().LastOrDefault();
                if (ConsultaSolicitud != null)
                {
                    var lista = context.TBL_PRODUCTOS_SOLICITUD.Where(a => a.ID_SOLICITUD == ConsultaSolicitud.ID).ToList();
                    solicitudMesa.Add(new ConsultaSolicitudGeneral
                    {
                        Id = ConsultaSolicitud.ID,
                        FechaSolicitud = ConsultaSolicitud.FECHA_SOLICITUD,
                        IdMesa = ConsultaSolicitud.ID_MESA,
                        NumeroMesa = context.TBL_MASTER_MESAS.Where(z => z.ID == IdMesa).FirstOrDefault().NUMERO_MESA,
                        NombreMesa = context.TBL_MASTER_MESAS.Where(z => z.ID == IdMesa).FirstOrDefault().NOMBRE_MESA,
                        IdMesero = ConsultaSolicitud.ID_MESERO,
                        NombreMesero = context.TBL_USUARIOS.Where(a => a.ID == ConsultaSolicitud.ID_MESERO).FirstOrDefault().NOMBRE,
                        IdentificacionCliente = ConsultaSolicitud.IDENTIFICACION_CLIENTE,
                        NombreCliente = ConsultaSolicitud.NOMBRE_CLIENTE,
                        EstadoSolicitud = ConsultaSolicitud.ESTADO_SOLICITUD,
                        Observaciones = ConsultaSolicitud.OBSERVACIONES,
                        OtrosCobros = ConsultaSolicitud.OTROS_COBROS,
                        Descuentos = ConsultaSolicitud.DESCUENTOS,
                        Subtotal = ConsultaSolicitud.SUBTOTAL,
                        PorcentajeIVA = ConsultaSolicitud.PORCENTAJE_IVA,
                        IVATotal = ConsultaSolicitud.IVA_TOTAL,
                        PorcentajeIConsumo = ConsultaSolicitud.PORCENTAJE_I_CONSUMO,
                        IConsumoTotal = ConsultaSolicitud.I_CONSUMO_TOTAL,
                        PorcentajeServicio = ConsultaSolicitud.PORCENTAJE_SERVICIO,
                        ServicioTotal = ConsultaSolicitud.SERVICIO_TOTAL,
                        Total = Convert.ToDecimal(Math.Round(Convert.ToDouble(ConsultaSolicitud.TOTAL))),
                        ProductosSolicitud = new List<ProductosSolicitud>(),
                        Impuestos = new List<Impuestos>()

                    });
                    var ConsultaProductosSolicitud = context.TBL_PRODUCTOS_SOLICITUD.Where(b => b.ID_SOLICITUD == ConsultaSolicitud.ID && b.ESTADO_PRODUCTO != Estados.Cancelado).ToList();
                    if (ConsultaProductosSolicitud.Count > 0)
                    {
                        foreach (var item in ConsultaProductosSolicitud)
                        {
                            try
                            {
                                solicitudMesa[0].ProductosSolicitud.Add(new ProductosSolicitud
                                {
                                    Id = item.ID,
                                    FechaRegistro = item.FECHA_REGISTRO,
                                    IdSolicitud = item.ID_SOLICITUD,
                                    IdProducto = item.ID_PRODUCTO,
                                    NombreProducto = context.TBL_PRODUCTOS.Where(a => a.ID == item.ID_PRODUCTO).FirstOrDefault().NOMBRE_PRODUCTO,
                                    IdMesero = item.ID_MESERO,
                                    NombreMesero = context.TBL_USUARIOS.Where(a => a.ID == item.ID_MESERO).FirstOrDefault().NOMBRE,
                                    PrecioProducto = item.PRECIO_PRODUCTO,
                                    EstadoProducto = item.ESTADO_PRODUCTO,
                                    Descripcion = item.DESCRIPCION

                                });
                            }
                            catch (Exception E)
                            {
                                throw E;
                            }
                        }
                    }
                    var ConsultaImpuesto = context.TBL_IMPUESTOS.ToList();
                    if (ConsultaImpuesto.Count > 0)
                    {
                        foreach (var item in ConsultaImpuesto)
                        {
                            solicitudMesa[0].Impuestos.Add(new Impuestos
                            {
                                Id = item.ID,
                                NombreImpuesto = item.NOMBRE_IMPUESTO,
                                Porcentaje = item.PORCENTAJE,
                                Estado = item.ESTADO
                            });
                        }
                    }
                }
            }

            return solicitudMesa;
        }
        public void ActualizaEstadoMesa(decimal Id, string Estado)
        {
            using (DBLaColina contex = new DBLaColina())
            {
                TBL_MASTER_MESAS modelActualizar = new TBL_MASTER_MESAS();
                modelActualizar = contex.TBL_MASTER_MESAS.FirstOrDefault(a => a.ID == Id);

                if (modelActualizar != null)
                {
                    modelActualizar.ESTADO = Estado;
                    contex.SaveChanges();
                }
            }
        }
        public string InsertaSolicitud(TBL_SOLICITUD model)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    model.FECHA_SOLICITUD = DateTime.Now;
                    model.ESTADO_SOLICITUD = Estados.Abierta;
                    model.IDENTIFICACION_CLIENTE = "";
                    model.NOMBRE_CLIENTE = "";
                    model.PORCENTAJE_IVA = contex.TBL_IMPUESTOS.Where(x => x.ID == 1 && x.ESTADO == Estados.Activo).FirstOrDefault() != null ? contex.TBL_IMPUESTOS.Where(x => x.ID == 1).FirstOrDefault().PORCENTAJE : 0;
                    model.PORCENTAJE_I_CONSUMO = contex.TBL_IMPUESTOS.Where(x => x.ID == 2 && x.ESTADO == Estados.Activo).FirstOrDefault() != null ? contex.TBL_IMPUESTOS.Where(x => x.ID == 2).FirstOrDefault().PORCENTAJE : 0;
                    model.PORCENTAJE_SERVICIO = contex.TBL_IMPUESTOS.Where(x => x.ID == 3 && x.ESTADO == Estados.Activo).FirstOrDefault() != null ? contex.TBL_IMPUESTOS.Where(x => x.ID == 3).FirstOrDefault().PORCENTAJE : 0;
                    contex.TBL_SOLICITUD.Add(model);
                    contex.SaveChanges();
                    Respuesta = "Solicitud Insertada exitosamente";
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }

            }
            return Respuesta;
        }
        public List<TBL_CATEGORIAS> ListaCategorias()
        {
            List<TBL_CATEGORIAS> listproductos = new List<TBL_CATEGORIAS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listproductos = contex.TBL_CATEGORIAS.Where(x => x.ESTADO.Equals(Estados.Activo)).ToList();
            }
            return listproductos;
        }
        public List<TBL_PRODUCTOS> ListaProductos(decimal IdProducto)
        {
            List<TBL_PRODUCTOS> listProductos = new List<TBL_PRODUCTOS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listProductos = contex.TBL_PRODUCTOS.Where(a => a.ID_CATEGORIA == IdProducto).ToList();
            }
            return listProductos;
        }
        public decimal ConsultaCantidadProducto(decimal? idProducto)
        {
            decimal? CantidadDisponible;
            using (DBLaColina contex = new DBLaColina())
            {
                var busquedaProducto = contex.TBL_PRODUCTOS.Where(x => x.ID == idProducto).FirstOrDefault();
                CantidadDisponible = busquedaProducto.CANTIDAD;
            }
            return Convert.ToInt32(CantidadDisponible);
        }
        public TBL_PRODUCTOS_SOLICITUD InsertaProductosSolicitud(TBL_PRODUCTOS_SOLICITUD model)
        {
            TBL_PRODUCTOS_SOLICITUD respuesta = new TBL_PRODUCTOS_SOLICITUD();
            using (DBLaColina context = new DBLaColina())
            {
                try
                {
                    TBL_PRODUCTOS_SOLICITUD modelo = new TBL_PRODUCTOS_SOLICITUD();
                    modelo.FECHA_REGISTRO = DateTime.Now;
                    modelo.ID_SOLICITUD = model.ID_SOLICITUD;
                    modelo.ID_PRODUCTO = model.ID_PRODUCTO;
                    modelo.ID_MESERO = model.ID_MESERO;
                    modelo.PRECIO_PRODUCTO = model.PRECIO_PRODUCTO;
                    modelo.ESTADO_PRODUCTO = model.ESTADO_PRODUCTO;
                    modelo.DESCRIPCION = model.DESCRIPCION;

                    context.TBL_PRODUCTOS_SOLICITUD.Add(modelo);
                    var r = context.SaveChanges();
                    respuesta = modelo;

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return respuesta;
        }
        public string ActualizaSolicitud(TBL_SOLICITUD model)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_SOLICITUD actualiza = new TBL_SOLICITUD();
                    actualiza = contex.TBL_SOLICITUD.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.ID_MESA = model.ID_MESA;
                        actualiza.ID_MESERO = model.ID_MESERO;
                        actualiza.IDENTIFICACION_CLIENTE = model.IDENTIFICACION_CLIENTE;
                        actualiza.NOMBRE_CLIENTE = model.NOMBRE_CLIENTE;
                        actualiza.ESTADO_SOLICITUD = model.ESTADO_SOLICITUD;
                        actualiza.OBSERVACIONES = model.OBSERVACIONES;
                        actualiza.OTROS_COBROS = model.OTROS_COBROS;
                        actualiza.DESCUENTOS = model.DESCUENTOS;
                        actualiza.SUBTOTAL = model.SUBTOTAL;
                        actualiza.PORCENTAJE_SERVICIO = Convert.ToDecimal(Math.Round(Convert.ToDouble(model.PORCENTAJE_SERVICIO), 15));
                        actualiza.SERVICIO_TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble((model.SUBTOTAL * model.PORCENTAJE_SERVICIO) / 100), 0));
                        actualiza.TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble(actualiza.SUBTOTAL + actualiza.IVA_TOTAL + actualiza.I_CONSUMO_TOTAL + actualiza.SERVICIO_TOTAL + actualiza.OTROS_COBROS - actualiza.DESCUENTOS), 5));
                        actualiza.METODO_PAGO = model.METODO_PAGO;
                        actualiza.VOUCHER = model.VOUCHER;
                        if (actualiza.METODO_PAGO == "EFECTIVO")
                            actualiza.CANT_EFECTIVO = actualiza.TOTAL;
                        else
                            actualiza.CANT_EFECTIVO = model.CANT_EFECTIVO;

                        contex.SaveChanges();
                        Respuesta = "Solicitud actualizada exitosamente";
                    }
                    else
                    {
                        Respuesta = "No existe la solicitud " + model.ID;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }

            }
            return Respuesta;
        }
        public string ActualizaTotalSolicitud(decimal? Id, decimal? SubTotal)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_SOLICITUD actualiza = new TBL_SOLICITUD();
                    actualiza = contex.TBL_SOLICITUD.Where(a => a.ID == Id).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.SUBTOTAL += SubTotal;
                        actualiza.IVA_TOTAL = (actualiza.SUBTOTAL * actualiza.PORCENTAJE_IVA) / 100;
                        actualiza.I_CONSUMO_TOTAL = (actualiza.SUBTOTAL * actualiza.PORCENTAJE_I_CONSUMO) / 100;
                        actualiza.SERVICIO_TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble(actualiza.SUBTOTAL * actualiza.PORCENTAJE_SERVICIO) / 100, 0));
                        actualiza.TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble(((actualiza.OTROS_COBROS + actualiza.SUBTOTAL) - actualiza.DESCUENTOS) + actualiza.IVA_TOTAL + actualiza.I_CONSUMO_TOTAL + actualiza.SERVICIO_TOTAL), 5));
                        contex.SaveChanges();
                        Respuesta = "Total Actualizado exitosamente";
                    }
                    else
                    {
                        Respuesta = "No existe la solicitud " + Id;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }

            }
            return Respuesta;
        }
        public string ActualizaCantidadProducto(decimal? Id, decimal? Total)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_PRODUCTOS actualiza = new TBL_PRODUCTOS();
                    actualiza = contex.TBL_PRODUCTOS.Where(a => a.ID == Id).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.CANTIDAD = Total;
                        contex.SaveChanges();
                        Respuesta = "Total Actualizado exitosamente";
                    }
                    else
                    {
                        Respuesta = "No existe la solicitud " + Id;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }

            }
            return Respuesta;
        }
        public string CancelaProductosSolicitud(decimal IdSolicitud, bool RetornaInventario)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    List<TBL_PRODUCTOS_SOLICITUD> actualiza = new List<TBL_PRODUCTOS_SOLICITUD>();
                    actualiza = contex.TBL_PRODUCTOS_SOLICITUD.Where(a => a.ID_SOLICITUD == IdSolicitud).ToList();
                    if (actualiza.Count > 0)
                    {
                        foreach (var item in actualiza)
                        {
                            item.ESTADO_PRODUCTO = Estados.Cancelado;
                            contex.SaveChanges();
                            if (RetornaInventario)
                                ActualizaCantidadProducto(item.ID_PRODUCTO, (ConsultaCantidadProducto(item.ID_PRODUCTO) + 1));
                        };
                    }
                    else
                    {
                        Respuesta = "No existe Productos para esta solicitud";
                    }
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }

            }
            return Respuesta;
        }
        public string CancelaProductoSolicitudXId(decimal IdProductoSolicitud, bool RetornaInventario)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_PRODUCTOS_SOLICITUD actualiza = new TBL_PRODUCTOS_SOLICITUD();
                    actualiza = contex.TBL_PRODUCTOS_SOLICITUD.Where(a => a.ID == IdProductoSolicitud).FirstOrDefault();
                    if (actualiza.ID > 0)
                    {
                        actualiza.ESTADO_PRODUCTO = Estados.Cancelado;
                        contex.SaveChanges();
                        if (RetornaInventario)
                            ActualizaCantidadProducto(actualiza.ID_PRODUCTO, (ConsultaCantidadProducto(actualiza.ID_PRODUCTO) + 1));
                        Respuesta = ActualizaTotalSolicitud(actualiza.ID_SOLICITUD, -actualiza.PRECIO_PRODUCTO);

                    }
                    else
                    {
                        Respuesta = "No existe Productos para esta solicitud";
                    }
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }
            }
            return Respuesta;
        }
        public bool ImprimirFactura(string idMesa)
        {
            bool respuesta;
            PrintDocument printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            printDocument1.PrinterSettings = ps;
            printDocument1.PrinterSettings.PrinterName = "CAJA";

            //CONSULTA SOLICITUD
            var solicitud = ConsultaSolicitudMesa(Convert.ToDecimal(idMesa));
            var ListAgrupaProductos = AgrupaProductos(solicitud[0].ProductosSolicitud);
            //FORMATO FACTURA
            Font Titulo = new Font("MS Mincho", 13, FontStyle.Bold);
            Font SubTitulo = new Font("MS Mincho", 12, FontStyle.Bold);
            Font body = new Font("MS Mincho", 10);
            int ancho = 280;
            int margenY = 215;
            int YProductos = 0;
            int UltimoPunto = 0;
            var printedLines = 15;
            var hoja = 1;

            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                if (hoja == 1)
                {
                    e.Graphics.DrawString("La Colina", Titulo, Brushes.Black, new RectangleF(95, 10, ancho, 20));
                    e.Graphics.DrawString("Parilla - Campestre", SubTitulo, Brushes.Black, new RectangleF(60, 30, ancho, 20));
                    e.Graphics.DrawString("NIT " + ConfigurationManager.AppSettings["NIT"].ToString(), body, Brushes.Black, new RectangleF(90, 50, ancho, 15)); ;
                    e.Graphics.DrawString("" + ConfigurationManager.AppSettings["DIRECCION"].ToString(), body, Brushes.Black, new RectangleF(60, 65, ancho, 15));
                    e.Graphics.DrawString("Factura: #" + solicitud[0].Id, body, Brushes.Black, new RectangleF(0, 110, ancho, 15));
                    e.Graphics.DrawString("Fecha: " + solicitud[0].FechaSolicitud, body, Brushes.Black, new RectangleF(0, 125, ancho, 15));
                    e.Graphics.DrawString("Mesero: " + solicitud[0].NombreMesero, body, Brushes.Black, new RectangleF(0, 140, ancho, 15));
                    e.Graphics.DrawString("MESA #" + solicitud[0].NumeroMesa + " - " + solicitud[0].NombreMesa, body, Brushes.Black, new RectangleF(0, 155, ancho, 15));
                    e.Graphics.DrawString("Cliente: " + solicitud[0].IdentificacionCliente + " - " + solicitud[0].NombreCliente, body, Brushes.Black, new RectangleF(0, 170, ancho, 15));

                    e.Graphics.DrawString("PRODUCTOS: ", body, Brushes.Black, new RectangleF(0, 215, ancho, 15));
                }
                
            };

            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                e.HasMorePages = false;
                float pageHeight = e.PageSettings.PrintableArea.Height;
                

                //lista los productos
                for (int i = UltimoPunto; i < ListAgrupaProductos.Count; i++)
                {
                    YProductos += 15;
                    e.Graphics.DrawString("" + ListAgrupaProductos[i].Id, body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + ListAgrupaProductos[i].NombreProducto, body, Brushes.Black, new RectangleF(25, margenY + YProductos, ancho, 15));
                    //PRECIO UNITARIO
                    //e.Graphics.DrawString("" + item.PrecioProducto, body, Brushes.Black, new RectangleF(160, 215 + YProductos, ancho, 15));
                    //PRECIO TOTAL
                    e.Graphics.DrawString("" + ListAgrupaProductos[i].IdMesero, body, Brushes.Black, new RectangleF((280 - (ListAgrupaProductos[i].IdMesero.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                    UltimoPunto++;
                    printedLines++;
                    if ((printedLines * 16) >= pageHeight)
                    {
                        e.HasMorePages = true;
                        printedLines = 0;
                        hoja++;
                        margenY = 0;
                        YProductos = 0;
                        return;
                    }
                }                
                var printedLines2 = printedLines + 11;
                if (printedLines2 > 74)
                {
                    e.HasMorePages = true;
                    printedLines = 0;
                    hoja++;
                    margenY = 0;
                    YProductos = 0;
                    return;
                }
                margenY += 30;
                if (solicitud[0].Descuentos > 0)
                {
                    YProductos += 15;
                    margenY += 15;
                    e.Graphics.DrawString("DESCUENTOS:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + solicitud[0].Descuentos, body, Brushes.Black, new RectangleF((280 - (solicitud[0].Descuentos.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                }
                if (solicitud[0].OtrosCobros > 0)
                {
                    YProductos += 15;
                    margenY += 15;
                    e.Graphics.DrawString("OTROS COBROS:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + solicitud[0].OtrosCobros, body, Brushes.Black, new RectangleF((280 - (solicitud[0].OtrosCobros.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                }
                margenY += 15;
                e.Graphics.DrawString("SUBTOTAL: ", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + solicitud[0].Subtotal, body, Brushes.Black, new RectangleF((280 - (solicitud[0].Subtotal.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                if (solicitud[0].IVATotal > 0 || solicitud[0].IConsumoTotal > 0)
                {
                    YProductos += 15;
                    margenY += 15;
                    var sumaImpuestos = solicitud[0].Subtotal + solicitud[0].IVATotal + solicitud[0].IConsumoTotal;
                    e.Graphics.DrawString("SUBTOTAL CON IMPUESTOS:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + sumaImpuestos, body, Brushes.Black, new RectangleF((280 - (sumaImpuestos.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                }
                margenY += 15;
                e.Graphics.DrawString("PROPINA VOLUNTARIA:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + solicitud[0].ServicioTotal, body, Brushes.Black, new RectangleF((280 - (solicitud[0].ServicioTotal.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("TOTAL:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + Convert.ToInt64(Math.Round(Convert.ToDouble(solicitud[0].Total))), body, Brushes.Black, new RectangleF((280 - ((Convert.ToInt64(Math.Round(Convert.ToDouble(solicitud[0].Total)))).ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 120;
                e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(135, margenY + YProductos, ancho, 15));
            };

            printDocument1.Print();
            respuesta = true;
            return respuesta;
        }
        public List<ProductosSolicitud> AgrupaProductos(List<ProductosSolicitud> productosSolicitud)
        {
            List<ProductosSolicitud> resultado = new List<ProductosSolicitud>();
            var distinctProductos = productosSolicitud.DistinctBy(c => c.IdProducto).ToList();
            foreach (var item in distinctProductos)
            {
                ProductosSolicitud model = new ProductosSolicitud();
                model.Id = productosSolicitud.Where(x => x.IdProducto == item.IdProducto).Count();
                model.NombreProducto = item.NombreProducto;
                //PRECIO UNITARIO
                model.PrecioProducto = item.PrecioProducto;
                //PRECIO TOTAL
                model.IdMesero = productosSolicitud.Where(x => x.IdProducto == item.IdProducto).Sum(x => x.PrecioProducto);
                resultado.Add(model);
            }
            return resultado;
        }
        public bool ImprimirPedido(string cantidad, string idproducto, string descripcion, string idMesa)
        {
            bool respuesta;
            PrintDocument printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            TBL_PRODUCTOS producto = new TBL_PRODUCTOS();
            TBL_IMPRESORAS impresora = new TBL_IMPRESORAS();
            //CONSULTA PRODUCTO
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    var idprod = Convert.ToDecimal(idproducto);
                    producto = contex.TBL_PRODUCTOS.Where(x => x.ID == idprod).FirstOrDefault();
                    if (producto != null)
                        impresora = contex.TBL_IMPRESORAS.Where(x => x.ID == producto.ID_IMPRESORA).FirstOrDefault();
                }
                catch (Exception ex)
                {

                }
            }
            printDocument1.PrinterSettings = ps;
            var consultaE = ConsultaEnergia();
            if (consultaE.VALOR != "1")
            {
                printDocument1.PrinterSettings.PrinterName = "CAJA";
            }
            else
            {
                if (impresora.NOMBRE_IMPRESORA == "PARRILLA. AUX" || impresora.NOMBRE_IMPRESORA == "ENTRADAS")
                    printDocument1.PrinterSettings.PrinterName = "PARRILLA.";
                else
                    printDocument1.PrinterSettings.PrinterName = impresora.NOMBRE_IMPRESORA;
            }
            
            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                //CONSULTA SOLICITUD
                var solicitud = ConsultaSolicitudMesa(Convert.ToDecimal(idMesa));

                //FORMATO FACTURA
                Font body = new Font("MS Mincho", 12);
                Font bodyNegrita = new Font("MS Mincho", 14, FontStyle.Bold);
                int ancho = 280;

                e.Graphics.DrawString("#" + solicitud[0].NumeroMesa + " - MESA => " + solicitud[0].NombreMesa, body, Brushes.Black, new RectangleF(0, 15, ancho, 20));
                e.Graphics.DrawString("MESERO => " + solicitud[0].NombreMesero, body, Brushes.Black, new RectangleF(0, 35, ancho, 20));
                e.Graphics.DrawString("HORA: " + DateTime.Now.ToString("HH:mm:ss"), bodyNegrita, Brushes.Black, new RectangleF(0, 55, ancho, 20));
                e.Graphics.DrawString("" + cantidad, body, Brushes.Black, new RectangleF(0, 95, ancho, 20));
                e.Graphics.DrawString("" + producto.NOMBRE_PRODUCTO, body, Brushes.Black, new RectangleF(30, 95, ancho, 20));

                //DAR FORMATO A DESCRIPCION
                int tamañoDes = 0;
                var descripcionAux = "";
                int Ymargen = 0;
                descripcion = descripcion.Replace("\n", " ");
                while (descripcion.Length > 21)
                {
                    tamañoDes += 21;
                    Ymargen += 20;
                    descripcionAux = descripcion.Substring(0, 21);
                    descripcion = descripcion.Substring(21, descripcion.Length - 21);
                    e.Graphics.DrawString("" + descripcionAux, body, Brushes.Black, new RectangleF(30, 95 + Ymargen, ancho, 20));
                }
                e.Graphics.DrawString("" + descripcion, body, Brushes.Black, new RectangleF(30, 115 + Ymargen, ancho, 20));

                e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(135, 160 + Ymargen, ancho, 20));

            };
            printDocument1.Print();
            respuesta = true;
            return respuesta;
        }
        public bool ImprimirPedidoFactura(List<TBL_PRODUCTOS_SOLICITUD> productos, decimal idMesa)
        {
            bool respuesta;
            List<TBL_PRODUCTOS> producto = new List<TBL_PRODUCTOS>();
            List<TBL_IMPRESORAS> impresoras = new List<TBL_IMPRESORAS>();
            try
            {
                //CONSULTA IMPRESORAS A IMPRIMIR
                var cantProductosDistinct = productos.DistinctBy(c => c.ID_PRODUCTO).ToList();
                foreach (var item in cantProductosDistinct)
                {
                    using (DBLaColina contex = new DBLaColina())
                    {
                        try
                        {
                            producto.Add(contex.TBL_PRODUCTOS.Where(x => x.ID == item.ID_PRODUCTO).FirstOrDefault());
                            if (producto.LastOrDefault() != null)
                            {
                                var idimpresora = producto.LastOrDefault().ID_IMPRESORA;
                                if (!(impresoras.Any(x => x.ID == idimpresora)))
                                    impresoras.Add(contex.TBL_IMPRESORAS.Where(x => x.ID == idimpresora).FirstOrDefault());
                            }

                        }
                        catch (Exception ex)
                        {
                            respuesta = false;
                        }
                    }
                }
                foreach (var item in impresoras)
                {
                    PrinterSettings ps = new PrinterSettings();
                    PrintDocument printDocument1 = new PrintDocument();
                    printDocument1.PrinterSettings = ps;
                    var consultaE = ConsultaEnergia();
                    if (consultaE.VALOR != "1")
                    {
                        printDocument1.PrinterSettings.PrinterName = "CAJA";
                    }
                    else
                    {
                        if (item.NOMBRE_IMPRESORA == "PARRILLA. AUX" || item.NOMBRE_IMPRESORA == "ENTRADAS")
                            printDocument1.PrinterSettings.PrinterName = "PARRILLA.";
                        else
                            printDocument1.PrinterSettings.PrinterName = item.NOMBRE_IMPRESORA;
                    }
                    
                    printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
                    {
                        int Ymargen = 0;
                        //CONSULTA SOLICITUD
                        var solicitud = ConsultaSolicitudMesa(Convert.ToDecimal(idMesa));

                        //FORMATO FACTURA
                        Font body = new Font("MS Mincho", 12);
                        Font bodyNegrita = new Font("MS Mincho", 14, FontStyle.Bold);
                        int ancho = 280;

                        e.Graphics.DrawString("#" + solicitud[0].NumeroMesa + " - MESA => " + solicitud[0].NombreMesa, body, Brushes.Black, new RectangleF(0, 15, ancho, 20));
                        e.Graphics.DrawString("MESERO => " + solicitud[0].NombreMesero, body, Brushes.Black, new RectangleF(0, 35, ancho, 20));
                        e.Graphics.DrawString("HORA: " + DateTime.Now.ToString("HH:mm:ss"), bodyNegrita, Brushes.Black, new RectangleF(0, 55, ancho, 20));

                        foreach (var item2 in producto)
                        {
                            if (item2.ID_IMPRESORA == item.ID)
                            {
                                List<TBL_PRODUCTOS_SOLICITUD> prodImprimir = new List<TBL_PRODUCTOS_SOLICITUD>();
                                prodImprimir = productos.Where(x => x.ID_PRODUCTO == item2.ID).ToList();

                                foreach (var item3 in prodImprimir)
                                {
                                    e.Graphics.DrawString("" + item3.ID, body, Brushes.Black, new RectangleF(0, 95 + Ymargen, ancho, 20));
                                    e.Graphics.DrawString("" + item2.NOMBRE_PRODUCTO, body, Brushes.Black, new RectangleF(30, 95 + Ymargen, ancho, 20));

                                    //DAR FORMATO A DESCRIPCION
                                    int tamañoDes = 0;
                                    var descripcionAux = "";
                                    item3.DESCRIPCION = item3.DESCRIPCION.Replace("\n", " ");
                                    while (item3.DESCRIPCION.Length > 21)
                                    {
                                        tamañoDes += 21;
                                        descripcionAux = item3.DESCRIPCION.Substring(0, 21);
                                        item3.DESCRIPCION = item3.DESCRIPCION.Substring(21, item3.DESCRIPCION.Length - 21);
                                        e.Graphics.DrawString("" + descripcionAux, body, Brushes.Black, new RectangleF(30, 115 + Ymargen, ancho, 20));
                                        Ymargen += 20;
                                    }
                                    //if (descripcionAux == "")
                                    //e.Graphics.DrawString("" + item3.DESCRIPCION, body, Brushes.Black, new RectangleF(30, 115 + Ymargen, ancho, 20));
                                    //else
                                    e.Graphics.DrawString("" + item3.DESCRIPCION, body, Brushes.Black, new RectangleF(30, 115 + Ymargen, ancho, 20));

                                    Ymargen += 40;
                                }
                            }
                        }
                        e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(135, 180 + Ymargen, ancho, 20));
                    };
                    printDocument1.Print();
                }
                respuesta = true;
            }
            catch (Exception e)
            {
                respuesta = false;
            }
            return respuesta;
        }
        public TBL_SISTEMA ConsultaEnergia()
        {
            TBL_SISTEMA Energia = new TBL_SISTEMA();
            using (DBLaColina context = new DBLaColina())
            {
                Energia = context.TBL_SISTEMA.Where(x => x.ID == 1).FirstOrDefault();
            }
            return Energia;
        }
        public List<TBL_USUARIOS> ListaMeseros()
        {
            List<TBL_USUARIOS> ListMeseros = new List<TBL_USUARIOS>();
            using (DBLaColina context = new DBLaColina())
            {
                ListMeseros = context.TBL_USUARIOS.Where(x => x.ID_PERFIL == 3).ToList();
            }
            return ListMeseros;
        }
        public void ActualizaEstadoProducto(TBL_PRODUCTOS_SOLICITUD model)
        {
            TBL_PRODUCTOS_SOLICITUD productoAct = new TBL_PRODUCTOS_SOLICITUD();
            using (DBLaColina context = new DBLaColina())
            {
                productoAct = context.TBL_PRODUCTOS_SOLICITUD.Where(x => x.ID == model.ID).FirstOrDefault();
                if (productoAct != null)
                {
                    productoAct.ESTADO_PRODUCTO = Estados.Entregado;
                    context.SaveChanges();
                }
            }
        }

        public TBL_PRODUCTOS ElementoInventario(decimal Id)
        {
            TBL_PRODUCTOS subrpoducto = new TBL_PRODUCTOS();
            using (DBLaColina context = new DBLaColina())
            {
                subrpoducto = context.TBL_PRODUCTOS.Where(a => a.ID == Id).ToList().LastOrDefault();
            }

            return subrpoducto;
        }
    }
}