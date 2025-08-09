using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using ColinaApplication.Dian.Entity;
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
                var ConsultaSolicitud = context.TBL_SOLICITUD.Where(a => a.ID_MESA == IdMesa && (a.ESTADO_SOLICITUD == Estados.Abierta || a.ESTADO_SOLICITUD == Estados.Llevar || a.ESTADO_SOLICITUD == "MESA DIVIDIDA")).ToList().LastOrDefault();
                if (ConsultaSolicitud != null)
                {
                    var lista = context.TBL_PRODUCTOS_SOLICITUD.Where(a => a.ID_SOLICITUD == ConsultaSolicitud.ID).ToList();
                    solicitudMesa.Add(new ConsultaSolicitudGeneral
                    {
                        Id = ConsultaSolicitud.ID,
                        FechaSolicitud = ConsultaSolicitud.FECHA_SOLICITUD,
                        IdMesa = ConsultaSolicitud.ID_MESA,
                        NumeroMesa = IdMesa != 999999 ? context.TBL_MASTER_MESAS.Where(z => z.ID == IdMesa).FirstOrDefault().NUMERO_MESA : IdMesa,
                        NombreMesa = IdMesa != 999999 ? context.TBL_MASTER_MESAS.Where(z => z.ID == IdMesa).FirstOrDefault().NOMBRE_MESA : Convert.ToString(IdMesa),
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
                        Impuestos = new List<Impuestos>(),
                        IdCliente = ConsultaSolicitud.ID_CLIENTE,
                        FactracionElectronica = ConsultaSolicitud.FACTURACION_ELECTRONICA,
                        EnvioDian = ConsultaSolicitud.ENVIO_DIAN,
                        ValoresVouchers = ConsultaSolicitud.VALORES_VOUCHERS,
                        IdFDian = ConsultaSolicitud.ID_F_DIAN,
                        IdSolicitudPrincipal = ConsultaSolicitud.ID_SOLICITUD_PRINCIPAL,
                        MesaDividida = ConsultaSolicitud.MESA_DIVIDIDA
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
                    var idClienteD = Convert.ToInt32(solicitudMesa[0].IdCliente);
                    if (idClienteD > 0)
                    {
                        var consultaClienteDian = context.TBL_CLIENTES_DIAN.Where(x => x.ID == idClienteD).ToList().FirstOrDefault();
                        if (consultaClienteDian != null)
                        {
                            solicitudMesa[0].cliente = (new ClienteDian
                            {
                                Id = consultaClienteDian.ID,
                                TipoPersona = consultaClienteDian.TIPO_PERSONA,
                                CodigoDocumento = consultaClienteDian.CODIGO_DOCUMENTO,
                                NombreDocumento = consultaClienteDian.NOMBRE_DOCUMENTO,
                                NumeroIdentificacion = consultaClienteDian.NUMERO_IDENTIFICACION,
                                Nombres = consultaClienteDian.NOMBRES,
                                Apellidos = consultaClienteDian.APELLIDOS,
                                RazonSocial = consultaClienteDian.RAZON_SOCIAL,
                                NombreComercial = consultaClienteDian.NOMBRE_COMERCIAL,
                                Direccion = consultaClienteDian.DIRECCION,
                                CodCiudad = consultaClienteDian.COD_CIUDAD,
                                NomCiudad = consultaClienteDian.NOM_CIUDAD,
                                Email = consultaClienteDian.EMAIL,
                                ResponsableIva = consultaClienteDian.RESPONSABLE_IVA,
                                CodigoRFiscal = consultaClienteDian.CODIGO_R_FISCAL,
                                NomRFiscal = consultaClienteDian.NOMBRE_R_FISCAL,
                                IdCodigoDian = consultaClienteDian.ID_CODIGO_DIAN,
                                Telefono = consultaClienteDian.TELEFONO,
                                DigitoVerif = consultaClienteDian.DIGITO_VERIFI
                            });
                        }
                    }
                    else {
                        solicitudMesa[0].cliente = new ClienteDian();
                    }
                    var mesaDividida = solicitudMesa[0].MesaDividida;
                    if (mesaDividida == "1")
                    {
                        var solicitudPrincipalID = solicitudMesa[0].Id;
                        var consultaSolicitudDividida = context.TBL_SOLICITUD.Where(a => a.ID_SOLICITUD_PRINCIPAL == solicitudPrincipalID && a.ESTADO_SOLICITUD == "MESA DIVIDIDA").ToList().LastOrDefault();
                        if (consultaSolicitudDividida != null)
                        {
                            solicitudMesa[0].SolicitudDividida = (new ConsultaSolicitudGeneral
                            {
                                Id = consultaSolicitudDividida.ID,
                                EstadoSolicitud = consultaSolicitudDividida.ESTADO_SOLICITUD,
                                IdSolicitudPrincipal = consultaSolicitudDividida.ID_SOLICITUD_PRINCIPAL,
                                ProductosSolicitud = new List<ProductosSolicitud>(),
                                Subtotal = consultaSolicitudDividida.SUBTOTAL,
                                PorcentajeServicio = consultaSolicitudDividida.PORCENTAJE_SERVICIO,
                                Total = consultaSolicitudDividida.TOTAL
                            });
                            var productosSolicitudDividida = context.TBL_PRODUCTOS_SOLICITUD.Where(b => b.ID_SOLICITUD == consultaSolicitudDividida.ID && b.ESTADO_PRODUCTO != Estados.Cancelado).ToList();
                            if (productosSolicitudDividida.Count > 0)
                            {
                                foreach (var item in productosSolicitudDividida)
                                {
                                    try
                                    {
                                        solicitudMesa[0].SolicitudDividida.ProductosSolicitud.Add(new ProductosSolicitud
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
                    model.IDENTIFICACION_CLIENTE = "222222222222";
                    model.NOMBRE_CLIENTE = "Consumidor Final";
                    model.PORCENTAJE_IVA = contex.TBL_IMPUESTOS.Where(x => x.ID == 1 && x.ESTADO == Estados.Activo).FirstOrDefault() != null ? contex.TBL_IMPUESTOS.Where(x => x.ID == 1).FirstOrDefault().PORCENTAJE : 0;
                    model.PORCENTAJE_I_CONSUMO = contex.TBL_IMPUESTOS.Where(x => x.ID == 2 && x.ESTADO == Estados.Activo).FirstOrDefault() != null ? contex.TBL_IMPUESTOS.Where(x => x.ID == 2).FirstOrDefault().PORCENTAJE : 0;                    
                    model.ID_CLIENTE = 0;
                    model.FACTURACION_ELECTRONICA = "0";
                    model.ENVIO_DIAN = "0";
                    var r = contex.TBL_SOLICITUD.Add(model);
                    contex.SaveChanges();
                    Respuesta = Convert.ToString(r.ID);
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
                        actualiza.ID_CLIENTE = model.ID_CLIENTE;
                        actualiza.FACTURACION_ELECTRONICA = model.FACTURACION_ELECTRONICA;
                        actualiza.ENVIO_DIAN = model.ENVIO_DIAN;
                        actualiza.VALORES_VOUCHERS = model.VALORES_VOUCHERS;
                        actualiza.ID_F_DIAN = model.ID_F_DIAN; 
                        actualiza.MESA_DIVIDIDA = model.MESA_DIVIDIDA;
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
            Font Titulo = new Font("MS Mincho", 14, FontStyle.Bold);
            Font SubTitulo = new Font("MS Mincho", 13, FontStyle.Bold);
            Font body = new Font("MS Mincho", 10);
            Font body2 = new Font("MS Mincho", 9, FontStyle.Italic);
            Font body3 = new Font("MS Mincho", 10, FontStyle.Bold);
            int ancho = 280;
            int margenY = 270;
            int YProductos = 0;
            int UltimoPunto = 0;
            var printedLines = 15;
            var hoja = 1;

            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                if (hoja == 1)
                {
                    e.Graphics.DrawString("La Colina", Titulo, Brushes.Black, new RectangleF(95, 10, ancho, 20));
                    e.Graphics.DrawString("Parrilla - Campestre", SubTitulo, Brushes.Black, new RectangleF(50, 30, ancho, 20));
                    e.Graphics.DrawString("NIT " + ConfigurationManager.AppSettings["NIT"].ToString(), body, Brushes.Black, new RectangleF(75, 50, ancho, 15)); ;
                    e.Graphics.DrawString("" + ConfigurationManager.AppSettings["DIRECCION"].ToString(), body, Brushes.Black, new RectangleF(65, 65, ancho, 15));
                    e.Graphics.DrawString("Comprobante Pago: #" + solicitud[0].Id, body, Brushes.Black, new RectangleF(0, 110, ancho, 15));
                    e.Graphics.DrawString("Fecha: " + solicitud[0].FechaSolicitud, body, Brushes.Black, new RectangleF(0, 125, ancho, 15));
                    e.Graphics.DrawString("Mesero: " + solicitud[0].NombreMesero, body, Brushes.Black, new RectangleF(0, 140, ancho, 15));
                    e.Graphics.DrawString("MESA #" + solicitud[0].NumeroMesa + " - " + solicitud[0].NombreMesa, body, Brushes.Black, new RectangleF(0, 155, ancho, 15));
                    e.Graphics.DrawString("Cliente: " + solicitud[0].NombreCliente, body, Brushes.Black, new RectangleF(0, 170, ancho, 15));
                    e.Graphics.DrawString("Documento: " + solicitud[0].IdentificacionCliente, body, Brushes.Black, new RectangleF(0, 185, ancho, 15));
                    e.Graphics.DrawString("Direccion: " + solicitud[0].cliente.Direccion, body, Brushes.Black, new RectangleF(0, 200, ancho, 15));
                    e.Graphics.DrawString("Telefono: " + solicitud[0].cliente.Telefono, body, Brushes.Black, new RectangleF(0, 215, ancho, 15));

                    e.Graphics.DrawString("------------------------------------------------------------------------------------------- ", body, Brushes.Black, new RectangleF(0, 255, ancho, 15));
                    e.Graphics.DrawString("PRODUCTOS: ", body, Brushes.Black, new RectangleF(0, 270, ancho, 15));
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
                margenY += 45;
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
                    //margenY += 15;
                    //var sumaImpuestos = solicitud[0].Subtotal + solicitud[0].IVATotal + solicitud[0].IConsumoTotal;
                    e.Graphics.DrawString("IMP. AL CONSUMO 8%", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + solicitud[0].IConsumoTotal, body, Brushes.Black, new RectangleF((280 - (solicitud[0].IConsumoTotal.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                    margenY += 15;
                }
                e.Graphics.DrawString("PROPINA VOLUNTARIA:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + solicitud[0].ServicioTotal, body, Brushes.Black, new RectangleF((280 - (solicitud[0].ServicioTotal.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 45;
                e.Graphics.DrawString("TOTAL:", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + Convert.ToInt64(Math.Round(Convert.ToDouble(solicitud[0].Total))), body, Brushes.Black, new RectangleF((280 - ((Convert.ToInt64(Math.Round(Convert.ToDouble(solicitud[0].Total)))).ToString().Length * 8)), margenY + YProductos, ancho, 15));
                
                margenY += 45;
                e.Graphics.DrawString("------------------------------------------------------------------------------------------- ", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("Tener en cuenta que este documento es un", body2, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("COMPROBANTE DE PAGO, la factura ", body2, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("electrónica será enviada al correo electrónico", body2, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("registrado", body2, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("------------------------------------------------------------------------------------------- ", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("Recuerda que si necesitas personalizar ", body3, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("tu factura electrónica, únicamente se", body3, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("realizará al momento de solicitar la", body3, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("misma (Según normatividad)", body3, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));

                margenY += 135;
                e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(135, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("- "+ solicitud[0].IdFDian + " -", body, Brushes.Black, new RectangleF(135, margenY + YProductos, ancho, 15));
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
                model.IdDian = item.IdDian;
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
        public TBL_CLIENTES_DIAN InsertaClientesDian(TBL_CLIENTES_DIAN model)
        {
            TBL_CLIENTES_DIAN respuesta = new TBL_CLIENTES_DIAN();
            using (DBLaColina context = new DBLaColina())
            {
                try
                {
                    respuesta = context.TBL_CLIENTES_DIAN.Add(model);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return respuesta;
        }
        public TBL_CLIENTES_DIAN ConsultaCedula(string Cedula)
        {
            TBL_CLIENTES_DIAN ClienteDian = new TBL_CLIENTES_DIAN();
            using (DBLaColina context = new DBLaColina())
            {
                ClienteDian = context.TBL_CLIENTES_DIAN.Where(x => x.NUMERO_IDENTIFICACION == Cedula.Trim()).FirstOrDefault();
            }
            return ClienteDian;
        }
        public TBL_CLIENTES_DIAN ConsultaCedulaId(decimal IdCedula)
        {
            TBL_CLIENTES_DIAN ClienteDian = new TBL_CLIENTES_DIAN();
            using (DBLaColina context = new DBLaColina())
            {
                ClienteDian = context.TBL_CLIENTES_DIAN.Where(x => x.ID == IdCedula).FirstOrDefault();
            }
            return ClienteDian;
        }
        public List<ProductosSolicitud> ConsultaProductoSolicitudId(decimal IdSolicitud)
        {
            List<ProductosSolicitud> productosSolicitud = new List<ProductosSolicitud>();
            using (DBLaColina context = new DBLaColina())
            {
                var productos = context.TBL_PRODUCTOS_SOLICITUD.Where(b => b.ID_SOLICITUD == IdSolicitud && b.ESTADO_PRODUCTO != Estados.Cancelado).ToList();
                foreach (var item in productos)
                {
                    try
                    {
                        productosSolicitud.Add(new ProductosSolicitud
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
                            Descripcion = item.DESCRIPCION,
                            UpDian = context.TBL_PRODUCTOS.Where(a => a.ID == item.ID_PRODUCTO).FirstOrDefault().UP_DIAN,
                            IdDian = context.TBL_PRODUCTOS.Where(a => a.ID == item.ID_PRODUCTO).FirstOrDefault().ID_DIAN,
                            AccountGroup = context.TBL_PRODUCTOS.Where(a => a.ID == item.ID_PRODUCTO).FirstOrDefault().ACCOUNT_GROUP_DIAN
                        });
                    }
                    catch (Exception E)
                    {
                        throw E;
                    }
                }
            }
            return productosSolicitud;
        }
        public int ActualizaIdProductosSolicitud(decimal IdSolicitudNueva, List<TBL_PRODUCTOS_SOLICITUD> model)
        {
            int Respuesta = 0;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    List<TBL_PRODUCTOS_SOLICITUD> actualiza = new List<TBL_PRODUCTOS_SOLICITUD>();
                    List<decimal> idProductos = model.Select(a => a.ID).ToList();                    
                    actualiza = contex.TBL_PRODUCTOS_SOLICITUD.Where(a => idProductos.Contains(a.ID)).ToList();                    
                    if (actualiza.Count > 0)
                    {
                        var idsolicitudPrincipal = actualiza[0].ID_SOLICITUD;
                        var CantProdTotal = contex.TBL_PRODUCTOS_SOLICITUD.Where(a => a.ID_SOLICITUD == idsolicitudPrincipal).ToList().Count;
                        foreach (var item in actualiza)
                        {
                            item.ID_SOLICITUD = IdSolicitudNueva;
                            contex.SaveChanges();
                        }
                        Respuesta = CantProdTotal;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = -1;
                }

            }
            return Respuesta;
        }
        public string ActualizaMesaPrincipalDividida(decimal IdSolicitudPrincipal, decimal? subtotal)
        {
            string Respuesta = "";
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    List<TBL_SOLICITUD> actualiza = new List<TBL_SOLICITUD>();
                    actualiza = contex.TBL_SOLICITUD.Where(a => a.ID == IdSolicitudPrincipal ).ToList();
                    if (actualiza.Count > 0)
                    {
                        foreach (var item in actualiza)
                        {
                            var subTotalF = item.SUBTOTAL - subtotal;
                            var Iconsumo = (subTotalF * item.PORCENTAJE_I_CONSUMO)/100;
                            var servicio = Convert.ToDecimal(Math.Round((Convert.ToDouble(subTotalF * item.PORCENTAJE_SERVICIO) / 100), 5));
                            item.MESA_DIVIDIDA = "1";
                            item.SUBTOTAL = subTotalF;
                            item.I_CONSUMO_TOTAL = Iconsumo;
                            item.SERVICIO_TOTAL = servicio;
                            item.TOTAL = subTotalF + servicio + Iconsumo;
                            contex.SaveChanges();
                        }
                    }
                    else
                    {
                        Respuesta = "No existe la solicitud";
                    }
                }
                catch (Exception e)
                {
                    Respuesta = "Error Servidor: " + e;
                }

            }
            return Respuesta;
        }
    }
}