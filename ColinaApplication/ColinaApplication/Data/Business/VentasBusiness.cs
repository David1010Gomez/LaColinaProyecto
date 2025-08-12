using ColinaApplication.Data.Clases;
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
    public class VentasBusiness
    {
        public TBL_CIERRES CierreUsuarioId(decimal? idusuario)
        {
            TBL_CIERRES UltimoCierre = new TBL_CIERRES();
            using (DBLaColina context = new DBLaColina())
            {
                UltimoCierre = context.TBL_CIERRES.Where(x => x.ID_USUARIO == idusuario).ToList().LastOrDefault();
            }
            return UltimoCierre;
        }

        public List<TBL_MASTER_MESAS> ConsultaMesasCargo(decimal? idusuario)
        {
            List<TBL_MASTER_MESAS> mesasACargo = new List<TBL_MASTER_MESAS>();
            using (DBLaColina context = new DBLaColina())
            {
                mesasACargo = context.TBL_MASTER_MESAS.Where(x => x.ID_USUARIO == idusuario).ToList();
            }
            return mesasACargo;
        }
        public bool ActualizaEstadoMesa(TBL_MASTER_MESAS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_MASTER_MESAS actualiza = new TBL_MASTER_MESAS();
                    actualiza = contex.TBL_MASTER_MESAS.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.ESTADO = model.ESTADO;
                        contex.SaveChanges();
                        Respuesta = true;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public bool InsertaNuevoCierre(TBL_CIERRES model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_CIERRES.Add(model);
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
        public bool ActualizaCierre(TBL_CIERRES model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_CIERRES actualiza = new TBL_CIERRES();
                    actualiza = contex.TBL_CIERRES.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.FECHA_HORA_CIERRE = model.FECHA_HORA_CIERRE;
                        actualiza.CANT_MESAS_ATENDIDAS = model.CANT_MESAS_ATENDIDAS;
                        actualiza.CANT_FINALIZADAS = model.CANT_FINALIZADAS;
                        actualiza.TOTAL_FINALIZADAS = model.TOTAL_FINALIZADAS;
                        actualiza.CANT_LLEVAR = model.CANT_LLEVAR;
                        actualiza.TOTAL_LLEVAR = model.TOTAL_LLEVAR;
                        actualiza.CANT_CANCELADAS = model.CANT_CANCELADAS;
                        actualiza.TOTAL_CANCELADAS = model.TOTAL_CANCELADAS;
                        actualiza.CANT_CONSUMO_INTERNO = model.CANT_CONSUMO_INTERNO;
                        actualiza.TOTAL_CONSUMO_INTERNO = model.TOTAL_CONSUMO_INTERNO;
                        actualiza.OTROS_COBROS_TOTAL = model.OTROS_COBROS_TOTAL;
                        actualiza.DESCUENTOS_TOTAL = model.DESCUENTOS_TOTAL;
                        actualiza.IVA_TOTAL = model.IVA_TOTAL;
                        actualiza.I_CONSUMO_TOTAL = model.I_CONSUMO_TOTAL;
                        actualiza.SERVICIO_TOTAL = model.SERVICIO_TOTAL;
                        actualiza.TOTAL_EFECTIVO = model.TOTAL_EFECTIVO;
                        actualiza.TOTAL_TARJETA = model.TOTAL_TARJETA;
                        actualiza.VENTA_TOTAL = model.VENTA_TOTAL;
                        contex.SaveChanges();
                        Respuesta = true;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public List<TBL_SOLICITUD> ConsultaSolicitudes(decimal? idusuario, DateTime? fechaApertura)
        {
            List<TBL_SOLICITUD> solicitudes = new List<TBL_SOLICITUD>();
            using (DBLaColina context = new DBLaColina())
            {
                var mesasACargos = context.TBL_MASTER_MESAS.Where(x => x.ID_USUARIO == idusuario).Select(y => y.ID).ToList();
                solicitudes = context.TBL_SOLICITUD.Where(x => mesasACargos.Any(w => x.ID_MESA == w) && x.FECHA_SOLICITUD >= fechaApertura).ToList();
            }
            return solicitudes;
        }
        public List<ConsultaSolicitud> ConsultaSolicitudesXFecha(DateTime FechaInicial, DateTime FechaFinal)
        {
            List<ConsultaSolicitud> solicitudes = new List<ConsultaSolicitud>();
            using (DBLaColina context = new DBLaColina())
            {
                solicitudes = (from a in context.TBL_SOLICITUD
                               where a.FECHA_SOLICITUD >= FechaInicial && a.FECHA_SOLICITUD <= FechaFinal
                               select new ConsultaSolicitud
                               {
                                   NroFactura = a.ID,
                                   FechaSolicitud = a.FECHA_SOLICITUD,
                                   NumeroMesa = context.TBL_MASTER_MESAS.Where(x => x.ID == a.ID_MESA).FirstOrDefault().NUMERO_MESA,
                                   NombreMesa = context.TBL_MASTER_MESAS.Where(x => x.ID == a.ID_MESA).FirstOrDefault().NOMBRE_MESA,
                                   IdMesero = context.TBL_USUARIOS.Where(x => x.ID == a.ID_MESERO).FirstOrDefault().ID,
                                   NombreMesero = context.TBL_USUARIOS.Where(x => x.ID == a.ID_MESERO).FirstOrDefault().NOMBRE,
                                   IdCliente = a.IDENTIFICACION_CLIENTE,
                                   NombreCliente = a.NOMBRE_CLIENTE,
                                   EstadoSolicitud = a.ESTADO_SOLICITUD,
                                   Observaciones = a.OBSERVACIONES,
                                   OtrosCobros = a.OTROS_COBROS,
                                   Descuentos = a.DESCUENTOS,
                                   Subtotal = a.SUBTOTAL,
                                   PorcentajeIVA = a.PORCENTAJE_IVA,
                                   IVATotal = a.IVA_TOTAL,
                                   PorcentajeIConsumo = a.PORCENTAJE_I_CONSUMO,
                                   IConsumoTotal = a.I_CONSUMO_TOTAL,
                                   PorcentajeServicio = a.PORCENTAJE_SERVICIO,
                                   ServicioTotal = a.SERVICIO_TOTAL,
                                   Total = a.TOTAL,
                                   MetodoPago = a.METODO_PAGO,
                                   Voucher = a.VOUCHER,
                                   Efectivo = a.CANT_EFECTIVO
                               }).ToList();
                var idSolicitudes = solicitudes.Select(x => x.NroFactura).ToList();
                var productosSolicitud = context.TBL_PRODUCTOS_SOLICITUD.Where(x => idSolicitudes.Any(y => x.ID_SOLICITUD == y)).ToList();
                if (productosSolicitud.Count > 0)
                {
                    solicitudes[0].ProductosSolicitud = new List<ProductosSolicitud>();
                    foreach (var item in productosSolicitud)
                    {
                        solicitudes[0].ProductosSolicitud.Add(new ProductosSolicitud
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

                }
                else
                {
                    //solicitudes[0].ProductosSolicitud = new List<ProductosSolicitud>();
                }
            }
            return solicitudes;
        }
        public ConsultaSolicitud ConsultaSolicitudXId(decimal Id)
        {
            ConsultaSolicitud solicitud = new ConsultaSolicitud();
            using (DBLaColina context = new DBLaColina())
            {
                solicitud = (from a in context.TBL_SOLICITUD
                             where a.ID == Id
                             select new ConsultaSolicitud
                             {
                                 NroFactura = a.ID,
                                 FechaSolicitud = a.FECHA_SOLICITUD,
                                 NumeroMesa = context.TBL_MASTER_MESAS.Where(x => x.ID == a.ID_MESA).FirstOrDefault().NUMERO_MESA,
                                 NombreMesa = context.TBL_MASTER_MESAS.Where(x => x.ID == a.ID_MESA).FirstOrDefault().NOMBRE_MESA,
                                 IdMesero = context.TBL_USUARIOS.Where(x => x.ID == a.ID_MESERO).FirstOrDefault().ID,
                                 NombreMesero = context.TBL_USUARIOS.Where(x => x.ID == a.ID_MESERO).FirstOrDefault().NOMBRE,
                                 IdCliente = a.IDENTIFICACION_CLIENTE,
                                 NombreCliente = a.NOMBRE_CLIENTE,
                                 EstadoSolicitud = a.ESTADO_SOLICITUD,
                                 Observaciones = a.OBSERVACIONES,
                                 OtrosCobros = a.OTROS_COBROS,
                                 Descuentos = a.DESCUENTOS,
                                 Subtotal = a.SUBTOTAL,
                                 PorcentajeIVA = a.PORCENTAJE_IVA,
                                 IVATotal = a.IVA_TOTAL,
                                 PorcentajeIConsumo = a.PORCENTAJE_I_CONSUMO,
                                 IConsumoTotal = a.I_CONSUMO_TOTAL,
                                 PorcentajeServicio = a.PORCENTAJE_SERVICIO,
                                 ServicioTotal = a.SERVICIO_TOTAL,
                                 Total = a.TOTAL,
                                 MetodoPago = a.METODO_PAGO,
                                 Voucher = a.VOUCHER
                             }).FirstOrDefault();
                if (solicitud != null)
                {
                    var productosSolicitud = context.TBL_PRODUCTOS_SOLICITUD.Where(x => x.ID_SOLICITUD == solicitud.NroFactura).ToList();
                    if (productosSolicitud.Count > 0)
                    {
                        solicitud.ProductosSolicitud = new List<ProductosSolicitud>();
                        foreach (var item in productosSolicitud)
                        {
                            solicitud.ProductosSolicitud.Add(new ProductosSolicitud
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

                    }
                    else
                    {
                        //solicitud.ProductosSolicitud = new List<ProductosSolicitud>();
                    }
                }
            }
            return solicitud;
        }
        public List<ConsultaNomina> ConsultaNomina()
        {
            List<ConsultaNomina> nomina = new List<ConsultaNomina>();

            using (DBLaColina context = new DBLaColina())
            {
                nomina = (from a in context.TBL_NOMINA
                          where a.ESTADO == "ACTIVO"
                          select new ConsultaNomina
                          {
                              Id = a.ID,
                              IdUsuarioSistema = a.ID_USUARIO_SISTEMA,
                              NombreUsuarioSistema = context.TBL_USUARIOS.Where(x => x.ID == a.ID_USUARIO_SISTEMA).FirstOrDefault().NOMBRE != null ? context.TBL_USUARIOS.Where(x => x.ID == a.ID_USUARIO_SISTEMA).FirstOrDefault().NOMBRE : "N/A",
                              IdPerfil = a.ID_PERFIL,
                              NombrePerfil = context.TBL_PERFIL.Where(x => x.ID == a.ID_PERFIL).FirstOrDefault().NOMBRE_PERFIL,
                              Cedula = a.CEDULA,
                              NombreUsuario = a.NOMBRE,
                              Cargo = a.CARGO,
                              DiasTrabajados = a.DIAS_TRABAJADOS,
                              Propinas = a.PROPINAS,
                              PorcentajeGananciaPropina = context.TBL_PERFIL.Where(x => x.ID == a.ID_PERFIL).FirstOrDefault().PORCENTAJE_PROPINA,
                              FechaPago = a.FECHA_PAGO,
                              FechaNacimmiento = a.FECHA_NACIMIENTO,
                              DireccionResidencia = a.DIRECCION_RESIDENCIA,
                              Telefono = a.TELEFONO,
                              TotalPagar = a.TOTAL_PAGAR,
                              ConsumoInterno = a.CONSUMO_INTERNO
                          }).ToList();
                foreach (var item in nomina)
                {
                    item.FechasAsignadas = context.TBL_DIAS_TRABAJADOS.Where(x => x.ID_USUARIO_NOMINA == item.Id).ToList().Where(x => x.FECHA_TRABAJADO.Value >= item.FechaPago.Value).Select(x => x.FECHA_TRABAJADO.Value.Date).ToList();
                    item.SuledoDiario = context.TBL_DIAS_TRABAJADOS.Where(x => x.ID_USUARIO_NOMINA == item.Id).ToList().Where(x => x.FECHA_TRABAJADO.Value >= item.FechaPago.Value).Select(x => x.SUELDO_DIARIO).ToList();
                    item.PerfilFecha = context.TBL_DIAS_TRABAJADOS.Where(x => x.ID_USUARIO_NOMINA == item.Id).ToList().Where(x => x.FECHA_TRABAJADO.Value >= item.FechaPago.Value).Select(x => x.ID_PERFIL).ToList();
                }
            }
            return nomina;
        }
        public bool AsignaDiaTrabajo(decimal IdUsuarioNomina, DateTime fechaTrabajo, decimal sueldoDiario, decimal IdPerfil)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_NOMINA actualiza = new TBL_NOMINA();
                    actualiza = contex.TBL_NOMINA.Where(a => a.ID == IdUsuarioNomina).FirstOrDefault();
                    if (actualiza != null)
                    {
                        TBL_DIAS_TRABAJADOS validacion = new TBL_DIAS_TRABAJADOS();
                        validacion = contex.TBL_DIAS_TRABAJADOS.Where(a => a.ID_USUARIO_NOMINA == IdUsuarioNomina).ToList().Where(a => a.FECHA_TRABAJADO.Value.Date == fechaTrabajo.Date).FirstOrDefault();
                        if (validacion == null)
                        {
                            TBL_DIAS_TRABAJADOS model = new TBL_DIAS_TRABAJADOS();
                            model.ID_USUARIO_NOMINA = IdUsuarioNomina;
                            model.FECHA_TRABAJADO = fechaTrabajo;
                            model.SUELDO_DIARIO = sueldoDiario;
                            model.ID_PERFIL = IdPerfil;
                            contex.TBL_DIAS_TRABAJADOS.Add(model);
                            actualiza.DIAS_TRABAJADOS += 1;
                            contex.SaveChanges();
                            Respuesta = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public bool CalcularPagos(decimal IdUsuarioNomina)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_NOMINA actualiza = new TBL_NOMINA();
                    TBL_DIAS_TRABAJADOS actualiza2 = new TBL_DIAS_TRABAJADOS();
                    actualiza = contex.TBL_NOMINA.Where(a => a.ID == IdUsuarioNomina).FirstOrDefault();
                    if (actualiza != null)
                    {
                        decimal? propinas = 0;
                        List<TBL_DIAS_TRABAJADOS> listaFechasTrabajadas = new List<TBL_DIAS_TRABAJADOS>();
                        listaFechasTrabajadas = contex.TBL_DIAS_TRABAJADOS.Where(x => x.FECHA_TRABAJADO >= actualiza.FECHA_PAGO && x.ID_USUARIO_NOMINA == actualiza.ID).ToList();

                        foreach (var fecha in listaFechasTrabajadas)
                        {
                            var Ultimocierre = contex.TBL_CIERRES.ToList().Where(x => x.FECHA_HORA_APERTURA.Value.Date == fecha.FECHA_TRABAJADO).LastOrDefault();
                            if (Ultimocierre != null)
                            {
                                DateTime fechaInicioUltimoCierre = Convert.ToDateTime(Ultimocierre.FECHA_HORA_APERTURA);
                                DateTime fechaFinUltimoCierre = Convert.ToDateTime(Ultimocierre.FECHA_HORA_CIERRE);
                                if (fecha.ID_PERFIL == 3)
                                {
                                    var PropinaDia = (contex.TBL_SOLICITUD.Where(x => x.ID_MESERO == actualiza.ID_USUARIO_SISTEMA && x.ESTADO_SOLICITUD != Estados.CancelaPedido && x.ESTADO_SOLICITUD != Estados.Inhabilitar).ToList().Where(x => x.FECHA_SOLICITUD.Value.Date == fecha.FECHA_TRABAJADO.Value.Date && (x.FECHA_SOLICITUD >= fechaInicioUltimoCierre && x.FECHA_SOLICITUD <= fechaFinUltimoCierre)).Sum(a => a.SERVICIO_TOTAL)) * ((contex.TBL_PERFIL.Where(x => x.ID == fecha.ID_PERFIL).FirstOrDefault().PORCENTAJE_PROPINA) / 100);
                                    propinas += PropinaDia;
                                    actualiza2 = contex.TBL_DIAS_TRABAJADOS.Where(x => x.ID == fecha.ID).FirstOrDefault();
                                    if (actualiza2 != null)
                                    {
                                        actualiza2.PROPINAS = PropinaDia;
                                        contex.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (contex.TBL_PERFIL.Where(x => x.ID == fecha.ID_PERFIL).FirstOrDefault().PORCENTAJE_PROPINA > 0)
                                    {
                                        var cantUsuarios = contex.TBL_DIAS_TRABAJADOS.Where(x => x.ID_PERFIL == 4).ToList().Where(x => x.FECHA_TRABAJADO.Value.Date == fecha.FECHA_TRABAJADO).ToList().Count;
                                        var propinaFecha = ((contex.TBL_SOLICITUD.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido && x.ESTADO_SOLICITUD != Estados.Inhabilitar).ToList().Where(x => x.FECHA_SOLICITUD.Value.Date == fecha.FECHA_TRABAJADO.Value.Date && (x.FECHA_SOLICITUD >= fechaInicioUltimoCierre && x.FECHA_SOLICITUD <= fechaFinUltimoCierre)).Sum(a => a.SERVICIO_TOTAL)) * ((contex.TBL_PERFIL.Where(x => x.ID == fecha.ID_PERFIL).FirstOrDefault().PORCENTAJE_PROPINA) / 100) / cantUsuarios);
                                        if (propinaFecha != null)
                                            propinas += propinaFecha;
                                        actualiza2 = contex.TBL_DIAS_TRABAJADOS.Where(x => x.ID == fecha.ID).FirstOrDefault();
                                        if (actualiza2 != null)
                                        {
                                            actualiza2.PROPINAS = propinaFecha;
                                            contex.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        actualiza2 = contex.TBL_DIAS_TRABAJADOS.Where(x => x.ID == fecha.ID).FirstOrDefault();
                                        if (actualiza2 != null)
                                        {
                                            actualiza2.PROPINAS = 0;
                                            contex.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        actualiza.PROPINAS = propinas;
                        actualiza.TOTAL_PAGAR = ((contex.TBL_DIAS_TRABAJADOS.Where(x => x.FECHA_TRABAJADO >= actualiza.FECHA_PAGO && x.ID_USUARIO_NOMINA == actualiza.ID).ToList().Sum(x => x.SUELDO_DIARIO)) + propinas);
                        actualiza.CONSUMO_INTERNO = contex.TBL_SOLICITUD.Where(x => x.ESTADO_SOLICITUD == Estados.ConsumoInterno && x.FECHA_SOLICITUD >= actualiza.FECHA_PAGO).ToList().Where(x => Convert.ToDecimal(x.IDENTIFICACION_CLIENTE) == Convert.ToDecimal(actualiza.CEDULA)).Sum(x => x.TOTAL);
                        contex.SaveChanges();
                        Respuesta = true;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public bool LiquidarUsuario(decimal IdUsuarioNomina)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_NOMINA actualiza = new TBL_NOMINA();
                    TBL_DIAS_TRABAJADOS actualiza2 = new TBL_DIAS_TRABAJADOS();
                    actualiza = contex.TBL_NOMINA.Where(a => a.ID == IdUsuarioNomina).FirstOrDefault();
                    bool impresionNomina = ImprimirNomina(IdUsuarioNomina);
                    if (actualiza != null && impresionNomina)
                    {
                        var fechaPago = DateTime.Now;
                        List<TBL_DIAS_TRABAJADOS> listaFechasTrabajadas = new List<TBL_DIAS_TRABAJADOS>();
                        listaFechasTrabajadas = contex.TBL_DIAS_TRABAJADOS.Where(x => x.ID_USUARIO_NOMINA == actualiza.ID && (x.FECHA_PAGO == null || x.FECHA_PAGO >= actualiza.FECHA_PAGO)).ToList();
                        foreach (var item in listaFechasTrabajadas)
                        {
                            item.FECHA_PAGO = fechaPago;
                            contex.SaveChanges();
                        }

                        actualiza.DIAS_TRABAJADOS = 0;
                        actualiza.PROPINAS = 0;
                        actualiza.FECHA_PAGO = fechaPago.AddDays(1);
                        actualiza.TOTAL_PAGAR = 0;
                        actualiza.CONSUMO_INTERNO = 0;
                        contex.SaveChanges();
                        Respuesta = true;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public bool ImprimirCierre(List<TBL_SOLICITUD> solicitudes, decimal idUsuario)
        {
            bool respuesta;
            PrintDocument printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            printDocument1.PrinterSettings = ps;
            printDocument1.PrinterSettings.PrinterName = "CAJA";
            List<ProductosSolicitud> productosSolicitudes = new List<ProductosSolicitud>();
            TBL_USUARIOS usuario = new TBL_USUARIOS();
            //CONSULTA PRODUCTOS
            var idSolicitudes = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido && x.ESTADO_SOLICITUD != Estados.Inhabilitar).Select(x => x.ID).ToList();
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    //productosSolicitudes = contex.TBL_PRODUCTOS_SOLICITUD.Where(x => idSolicitudes.Any(a => a == x.ID)).ToList();                            var ListAgrupaProductos = AgrupaProductos(productosSolicitudes);
                    productosSolicitudes = (from a in contex.TBL_PRODUCTOS_SOLICITUD
                                            where idSolicitudes.Any(b => a.ID_SOLICITUD == b) && a.ESTADO_PRODUCTO == Estados.Entregado
                                            select new ProductosSolicitud
                                            {
                                                IdProducto = a.ID_PRODUCTO,
                                                NombreProducto = contex.TBL_PRODUCTOS.Where(x => x.ID == a.ID_PRODUCTO).FirstOrDefault().NOMBRE_PRODUCTO,
                                                PrecioProducto = a.PRECIO_PRODUCTO
                                            }).ToList();
                    usuario = contex.TBL_USUARIOS.Where(x => x.ID == idUsuario).FirstOrDefault();
                }
                catch (Exception ex)
                {

                }
            }
            var productosAgrupados = AgrupaProductos(productosSolicitudes);
            var ultimoCierre = CierreUsuarioId(idUsuario);
            //FORMATO FACTURA
            Font Titulo = new Font("MS Mincho", 14, FontStyle.Bold);
            Font body = new Font("MS Mincho", 10);
            Font bodyNegrita = new Font("MS Mincho", 11, FontStyle.Bold);
            Font bodySubrayado = new Font("MS Mincho", 10, FontStyle.Underline);
            int ancho = 280;
            int margenY = 135;
            int YProductos = 0;
            int UltimoPunto = 0;
            var printedLines = 15;
            var hoja = 1;

            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                if (hoja == 1)
                {
                    e.Graphics.DrawString("Cierre - Caja", Titulo, Brushes.Black, new RectangleF(85, 10, ancho, 20));
                    e.Graphics.DrawString("Fecha Apertura: " + ultimoCierre.FECHA_HORA_APERTURA, body, Brushes.Black, new RectangleF(0, 60, ancho, 20));
                    e.Graphics.DrawString("Fecha Cierre: " + ultimoCierre.FECHA_HORA_CIERRE, body, Brushes.Black, new RectangleF(0, 75, ancho, 15));
                    e.Graphics.DrawString("Cajero: " + usuario.CEDULA + " - " + usuario.NOMBRE, body, Brushes.Black, new RectangleF(0, 90, ancho, 15));
                    e.Graphics.DrawString("______________________________________", body, Brushes.Black, new RectangleF(0, 105, ancho, 15));
                    e.Graphics.DrawString("Productos", bodyNegrita, Brushes.Black, new RectangleF(0, 135, ancho, 15));
                }
            };
            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                e.HasMorePages = false;
                float pageHeight = e.PageSettings.PrintableArea.Height;
                for (int i = UltimoPunto; i < productosAgrupados.Count; i++)
                {
                    YProductos += 15;
                    e.Graphics.DrawString("" + productosAgrupados[i].NombreProducto, body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + productosAgrupados[i].Id, body, Brushes.Black, new RectangleF((280 - (productosAgrupados[i].Id.ToString().Length * 9)), margenY + YProductos, ancho, 15));
                    //PRECIO UNITARIO
                    //e.Graphics.DrawString("" + productosAgrupados[i].PrecioProducto, body, Brushes.Black, new RectangleF(160, 215 + YProductos, ancho, 15));
                    //PRECIO TOTAL
                    //e.Graphics.DrawString("" + productosAgrupados[i].IdMesero, body, Brushes.Black, new RectangleF((280 - (item.IdMesero.ToString().Length * 8)), 215 + YProductos, ancho, 15));
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
                var printedLines2 = printedLines + 22;
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
                e.Graphics.DrawString("• Mesas atendidas:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.CANT_MESAS_ATENDIDAS, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.CANT_MESAS_ATENDIDAS.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Finalizadas:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.CANT_FINALIZADAS, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.CANT_FINALIZADAS.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Total: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.TOTAL_FINALIZADAS, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.TOTAL_FINALIZADAS.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Consumo Interno:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.CANT_CONSUMO_INTERNO, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.CANT_CONSUMO_INTERNO.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Total: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.TOTAL_CONSUMO_INTERNO, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.TOTAL_CONSUMO_INTERNO.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Canceladas:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.CANT_CANCELADAS, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.CANT_CANCELADAS.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Total: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.TOTAL_CANCELADAS, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.TOTAL_CANCELADAS.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Otros Cobros:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.OTROS_COBROS_TOTAL, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.OTROS_COBROS_TOTAL.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Descuentos:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.DESCUENTOS_TOTAL, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.DESCUENTOS_TOTAL.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Impuestos:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> I.V.A.:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.IVA_TOTAL, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.IVA_TOTAL.ToString().Length * 9)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Impuesto Consumo: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.I_CONSUMO_TOTAL, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.I_CONSUMO_TOTAL.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Servicio: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.SERVICIO_TOTAL, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.SERVICIO_TOTAL.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Totales:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Efectivo: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.TOTAL_EFECTIVO, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.TOTAL_EFECTIVO.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Tarjeta: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + ultimoCierre.TOTAL_TARJETA, body, Brushes.Black, new RectangleF((280 - (ultimoCierre.TOTAL_TARJETA.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> TOTAL VENTAS: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + Convert.ToInt64(Math.Round(Convert.ToDouble(ultimoCierre.VENTA_TOTAL))), body, Brushes.Black, new RectangleF((280 - ((Convert.ToInt64(Math.Round(Convert.ToDouble(ultimoCierre.VENTA_TOTAL)))).ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 120;
                e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(135, 600 + YProductos, ancho, 15));
            };
            printDocument1.Print();
            respuesta = true;
            return respuesta;
        }
        public bool ImprimirCierreParcial(List<TBL_SOLICITUD> solicitudes, decimal idUsuario)
        {
            bool respuesta;
            PrintDocument printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            printDocument1.PrinterSettings = ps;
            printDocument1.PrinterSettings.PrinterName = "CAJA";
            List<ProductosSolicitud> productosSolicitudes = new List<ProductosSolicitud>();
            TBL_USUARIOS usuario = new TBL_USUARIOS();
            //CONSULTA PRODUCTOS
            var idSolicitudes = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido && x.ESTADO_SOLICITUD != Estados.Inhabilitar).Select(x => x.ID).ToList();
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    //productosSolicitudes = contex.TBL_PRODUCTOS_SOLICITUD.Where(x => idSolicitudes.Any(a => a == x.ID)).ToList();                            var ListAgrupaProductos = AgrupaProductos(productosSolicitudes);
                    productosSolicitudes = (from a in contex.TBL_PRODUCTOS_SOLICITUD
                                            where idSolicitudes.Any(b => a.ID_SOLICITUD == b) && a.ESTADO_PRODUCTO == Estados.Entregado
                                            select new ProductosSolicitud
                                            {
                                                IdProducto = a.ID_PRODUCTO,
                                                NombreProducto = contex.TBL_PRODUCTOS.Where(x => x.ID == a.ID_PRODUCTO).FirstOrDefault().NOMBRE_PRODUCTO,
                                                PrecioProducto = a.PRECIO_PRODUCTO
                                            }).ToList();
                    usuario = contex.TBL_USUARIOS.Where(x => x.ID == idUsuario).FirstOrDefault();
                }
                catch (Exception ex)
                {

                }
            }
            var productosAgrupados = AgrupaProductos(productosSolicitudes);
            var ultimoCierre = CierreUsuarioId(idUsuario);
            //FORMATO FACTURA
            Font Titulo = new Font("MS Mincho", 14, FontStyle.Bold);
            Font body = new Font("MS Mincho", 10);
            Font bodyNegrita = new Font("MS Mincho", 11, FontStyle.Bold);
            Font bodySubrayado = new Font("MS Mincho", 10, FontStyle.Underline);
            int ancho = 280;
            int margenY = 135;
            int YProductos = 0;
            int UltimoPunto = 0;
            var printedLines = 15;
            var hoja = 1;

            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                if (hoja == 1)
                {
                    e.Graphics.DrawString("Cierre - Caja", Titulo, Brushes.Black, new RectangleF(85, 10, ancho, 20));
                    e.Graphics.DrawString("Fecha Apertura: " + ultimoCierre.FECHA_HORA_APERTURA, body, Brushes.Black, new RectangleF(0, 60, ancho, 20));
                    e.Graphics.DrawString("Fecha Cierre: " + ultimoCierre.FECHA_HORA_CIERRE, body, Brushes.Black, new RectangleF(0, 75, ancho, 15));
                    e.Graphics.DrawString("Cajero: " + usuario.CEDULA + " - " + usuario.NOMBRE, body, Brushes.Black, new RectangleF(0, 90, ancho, 15));
                    e.Graphics.DrawString("______________________________________", body, Brushes.Black, new RectangleF(0, 105, ancho, 15));
                    e.Graphics.DrawString("Productos", bodyNegrita, Brushes.Black, new RectangleF(0, 135, ancho, 15));
                }
            };
            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                e.HasMorePages = false;
                float pageHeight = e.PageSettings.PrintableArea.Height;
                for (int i = UltimoPunto; i < productosAgrupados.Count; i++)
                {
                    YProductos += 15;
                    e.Graphics.DrawString("" + productosAgrupados[i].NombreProducto, body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                    e.Graphics.DrawString("" + productosAgrupados[i].Id, body, Brushes.Black, new RectangleF((280 - (productosAgrupados[i].Id.ToString().Length * 9)), margenY + YProductos, ancho, 15));
                    //PRECIO UNITARIO
                    //e.Graphics.DrawString("" + productosAgrupados[i].PrecioProducto, body, Brushes.Black, new RectangleF(160, 215 + YProductos, ancho, 15));
                    //PRECIO TOTAL
                    //e.Graphics.DrawString("" + productosAgrupados[i].IdMesero, body, Brushes.Black, new RectangleF((280 - (item.IdMesero.ToString().Length * 8)), 215 + YProductos, ancho, 15));
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
                var printedLines2 = printedLines + 22;
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
                e.Graphics.DrawString("• Mesas atendidas:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                var MesasAtendidas = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido && x.ESTADO_SOLICITUD != Estados.Inhabilitar).Count();
                e.Graphics.DrawString("" + MesasAtendidas.ToString(), body, Brushes.Black, new RectangleF((280 - (MesasAtendidas.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Finalizadas:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var Finalizadas = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Finalizada).Count();
                e.Graphics.DrawString("" + Finalizadas.ToString(), body, Brushes.Black, new RectangleF((280 - (Finalizadas.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Total: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var TotalFinalizadas = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.Finalizada).Sum(a => a.TOTAL);
                e.Graphics.DrawString("" + TotalFinalizadas.ToString(), body, Brushes.Black, new RectangleF((280 - (TotalFinalizadas.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Consumo Interno:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var ConsumoInterno = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.ConsumoInterno).Count();
                e.Graphics.DrawString("" + ConsumoInterno.ToString(), body, Brushes.Black, new RectangleF((280 - (ConsumoInterno.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Total: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var TotalConsumoInterno = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.ConsumoInterno).Sum(a => a.TOTAL);
                e.Graphics.DrawString("" + TotalConsumoInterno.ToString(), body, Brushes.Black, new RectangleF((280 - (TotalConsumoInterno.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Canceladas:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var Canceladas = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.CancelaPedido).Count();
                e.Graphics.DrawString("" + Canceladas.ToString(), body, Brushes.Black, new RectangleF((280 - (Canceladas.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Total: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var TotalCanceladas = solicitudes.Where(x => x.ESTADO_SOLICITUD == Estados.CancelaPedido).Sum(a => a.TOTAL);
                e.Graphics.DrawString("" + TotalCanceladas.ToString(), body, Brushes.Black, new RectangleF((280 - (TotalCanceladas.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Otros Cobros:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                var OtrosCobros = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.OTROS_COBROS);
                e.Graphics.DrawString("" + OtrosCobros.ToString(), body, Brushes.Black, new RectangleF((280 - (OtrosCobros.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Descuentos:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                var Descuentos = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.DESCUENTOS);
                e.Graphics.DrawString("" + Descuentos.ToString(), body, Brushes.Black, new RectangleF((280 - (Descuentos.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Impuestos:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> I.V.A.:", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var IVA = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.IVA_TOTAL);
                e.Graphics.DrawString("" + IVA.ToString(), body, Brushes.Black, new RectangleF((280 - (IVA.ToString().Length * 9)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Impuesto Consumo: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var IConsumo = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.Cancelado).Sum(a => a.I_CONSUMO_TOTAL);
                e.Graphics.DrawString("" + IConsumo.ToString(), body, Brushes.Black, new RectangleF((280 - (IConsumo.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Servicio: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var Servicio = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido).Sum(a => a.SERVICIO_TOTAL);
                e.Graphics.DrawString("" + Servicio.ToString(), body, Brushes.Black, new RectangleF((280 - (Servicio.ToString().Length * 8)), margenY + YProductos, ancho, 15));

                margenY += 30;
                e.Graphics.DrawString("• Totales:", bodyNegrita, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> Efectivo: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var Efectivo = solicitudes.Where(x => x.CANT_EFECTIVO > 0).Sum(a => a.CANT_EFECTIVO);
                e.Graphics.DrawString("" + Efectivo.ToString(), body, Brushes.Black, new RectangleF((280 - (Efectivo.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                var cuentasVouchers = solicitudes.Where(x => x.VOUCHER != "0" && x.VOUCHER != "" && x.VOUCHER != null).Sum(a => a.TOTAL);
                decimal? totalTarjeta = 0;
                if (cuentasVouchers != null && cuentasVouchers > 0)
                    totalTarjeta = cuentasVouchers - (solicitudes.Where(x => x.VOUCHER != "0" && x.VOUCHER != "" && x.VOUCHER != null).Sum(a => a.CANT_EFECTIVO));
                else
                    totalTarjeta = 0;
                margenY += 15;
                e.Graphics.DrawString("-> Tarjeta: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                e.Graphics.DrawString("" + totalTarjeta.ToString(), body, Brushes.Black, new RectangleF((280 - (totalTarjeta.ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 15;
                e.Graphics.DrawString("-> TOTAL VENTAS: ", body, Brushes.Black, new RectangleF(16, margenY + YProductos, ancho, 15));
                var TotalVentas = solicitudes.Where(x => x.ESTADO_SOLICITUD != Estados.CancelaPedido).Sum(a => a.TOTAL);
                e.Graphics.DrawString("" + (Convert.ToInt64(Math.Round(Convert.ToDouble(TotalVentas)))).ToString(), body, Brushes.Black, new RectangleF((280 - ((Convert.ToInt64(Math.Round(Convert.ToDouble(TotalVentas)))).ToString().Length * 8)), margenY + YProductos, ancho, 15));
                margenY += 120;
                e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(135, 600 + YProductos, ancho, 15));
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
        public bool ImprimirNomina(decimal id)
        {
            bool respuesta;
            PrintDocument printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            printDocument1.PrinterSettings = ps;
            printDocument1.PrinterSettings.PrinterName = "CAJA";
            printDocument1.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                //CONSULTA USUARIO EN NOMINA
                var usuarioNomina = ConsultaUsuarioNomina(id);

                //FORMATO FACTURA
                Font Titulo = new Font("MS Mincho", 14, FontStyle.Bold);
                Font body = new Font("MS Mincho", 10);
                Font bodyNegrita = new Font("MS Mincho", 11, FontStyle.Bold);
                int ancho = 280;
                int Ymargen = 0;

                e.Graphics.DrawString("PAGO - NÓMINA", Titulo, Brushes.Black, new RectangleF(70, 10, ancho, 20));
                e.Graphics.DrawString("Fecha: " + DateTime.Now, body, Brushes.Black, new RectangleF(0, 60, ancho, 20));
                e.Graphics.DrawString("Identificación: " + usuarioNomina.Cedula, body, Brushes.Black, new RectangleF(0, 75, ancho, 15));
                e.Graphics.DrawString("Nombre: " + usuarioNomina.NombreUsuario, body, Brushes.Black, new RectangleF(0, 90, ancho, 20));
                e.Graphics.DrawString("______________________________________", body, Brushes.Black, new RectangleF(0, 105, ancho, 15));

                e.Graphics.DrawString("Dias trabajados: ", bodyNegrita, Brushes.Black, new RectangleF(0, 135, ancho, 15));
                e.Graphics.DrawString("$$$ ", bodyNegrita, Brushes.Black, new RectangleF(256, 135, ancho, 15));
                for (int i = 0; i < usuarioNomina.FechasAsignadas.Count; i++)
                {
                    Ymargen += 15;
                    e.Graphics.DrawString("* " + usuarioNomina.FechasAsignadas[i].ToString("dd-MM-yyyy"), body, Brushes.Black, new RectangleF(0, 135 + Ymargen, ancho, 15));
                    e.Graphics.DrawString("" + usuarioNomina.SuledoDiario[i], body, Brushes.Black, new RectangleF((280 - (usuarioNomina.SuledoDiario[i].ToString().Length * 8)), 135 + Ymargen, ancho, 15));
                }
                e.Graphics.DrawString("______________________________________", body, Brushes.Black, new RectangleF(0, 165 + Ymargen, ancho, 15));

                e.Graphics.DrawString("Propinas: " + usuarioNomina.Propinas, body, Brushes.Black, new RectangleF(0, 195 + Ymargen, ancho, 15));
                e.Graphics.DrawString("Deudas: " + usuarioNomina.ConsumoInterno, body, Brushes.Black, new RectangleF(0, 210 + Ymargen, ancho, 15));

                e.Graphics.DrawString("______________________________________", body, Brushes.Black, new RectangleF(0, 225 + Ymargen, ancho, 15));
                e.Graphics.DrawString("TOTAL: " + usuarioNomina.TotalPagar, bodyNegrita, Brushes.Black, new RectangleF(0, 270 + Ymargen, ancho, 15));
                e.Graphics.DrawString("_", body, Brushes.Black, new RectangleF(0, 355 + Ymargen, ancho, 15));

            };
            printDocument1.Print();
            respuesta = true;
            return respuesta;
        }
        public ConsultaNomina ConsultaUsuarioNomina(decimal id)
        {
            ConsultaNomina usuarioNomina = new ConsultaNomina();

            using (DBLaColina context = new DBLaColina())
            {
                usuarioNomina = (from a in context.TBL_NOMINA
                                 where a.ID == id
                                 select new ConsultaNomina
                                 {
                                     Id = a.ID,
                                     IdUsuarioSistema = a.ID_USUARIO_SISTEMA,
                                     NombreUsuarioSistema = context.TBL_USUARIOS.Where(x => x.ID == a.ID_USUARIO_SISTEMA).FirstOrDefault().NOMBRE != null ? context.TBL_USUARIOS.Where(x => x.ID == a.ID_USUARIO_SISTEMA).FirstOrDefault().NOMBRE : "N/A",
                                     IdPerfil = a.ID_PERFIL,
                                     NombrePerfil = context.TBL_PERFIL.Where(x => x.ID == a.ID_PERFIL).FirstOrDefault().NOMBRE_PERFIL,
                                     Cedula = a.CEDULA,
                                     NombreUsuario = a.NOMBRE,
                                     Cargo = a.CARGO,
                                     DiasTrabajados = a.DIAS_TRABAJADOS,
                                     Propinas = a.PROPINAS,
                                     PorcentajeGananciaPropina = context.TBL_PERFIL.Where(x => x.ID == a.ID_PERFIL).FirstOrDefault().PORCENTAJE_PROPINA,
                                     FechaPago = a.FECHA_PAGO,
                                     FechaNacimmiento = a.FECHA_NACIMIENTO,
                                     DireccionResidencia = a.DIRECCION_RESIDENCIA,
                                     Telefono = a.TELEFONO,
                                     TotalPagar = a.TOTAL_PAGAR,
                                     ConsumoInterno = a.CONSUMO_INTERNO
                                 }).FirstOrDefault();
                if (usuarioNomina != null)
                {
                    usuarioNomina.FechasAsignadas = context.TBL_DIAS_TRABAJADOS.Where(x => x.ID_USUARIO_NOMINA == id).ToList().Where(x => x.FECHA_TRABAJADO.Value.Date >= usuarioNomina.FechaPago.Value.Date).Select(x => x.FECHA_TRABAJADO.Value.Date).ToList();
                    usuarioNomina.SuledoDiario = context.TBL_DIAS_TRABAJADOS.Where(x => x.ID_USUARIO_NOMINA == id).ToList().Where(x => x.FECHA_TRABAJADO.Value.Date >= usuarioNomina.FechaPago.Value.Date).Select(x => x.SUELDO_DIARIO).ToList();
                }
            }
            return usuarioNomina;
        }
        public List<ConsultaSolicitud> ConsultaUltimasFacturas(DateTime fecha1, DateTime fecha2)
        {
            List<ConsultaSolicitud> solicitudes = new List<ConsultaSolicitud>();
            using (DBLaColina context = new DBLaColina())
            {
                solicitudes = (from a in context.TBL_SOLICITUD
                               where a.FECHA_SOLICITUD >= fecha1 && a.FECHA_SOLICITUD <= fecha2
                               orderby a.ID descending
                               select new ConsultaSolicitud
                               {
                                   NroFactura = a.ID,
                                   FechaSolicitud = a.FECHA_SOLICITUD,
                                   NumeroMesa = context.TBL_MASTER_MESAS.Where(x => x.ID == a.ID_MESA).FirstOrDefault().NUMERO_MESA,
                                   NombreMesa = context.TBL_MASTER_MESAS.Where(x => x.ID == a.ID_MESA).FirstOrDefault().NOMBRE_MESA,
                                   IdMesero = context.TBL_USUARIOS.Where(x => x.ID == a.ID_MESERO).FirstOrDefault().ID,
                                   NombreMesero = context.TBL_USUARIOS.Where(x => x.ID == a.ID_MESERO).FirstOrDefault().NOMBRE,
                                   IdCliente = a.IDENTIFICACION_CLIENTE,
                                   NombreCliente = a.NOMBRE_CLIENTE,
                                   EstadoSolicitud = a.ESTADO_SOLICITUD,
                                   Observaciones = a.OBSERVACIONES,
                                   OtrosCobros = a.OTROS_COBROS,
                                   Descuentos = a.DESCUENTOS,
                                   Subtotal = a.SUBTOTAL,
                                   PorcentajeIVA = a.PORCENTAJE_IVA,
                                   IVATotal = a.IVA_TOTAL,
                                   PorcentajeIConsumo = a.PORCENTAJE_I_CONSUMO,
                                   IConsumoTotal = a.I_CONSUMO_TOTAL,
                                   PorcentajeServicio = a.PORCENTAJE_SERVICIO,
                                   ServicioTotal = a.SERVICIO_TOTAL,
                                   Total = a.TOTAL,
                                   MetodoPago = a.METODO_PAGO,
                                   Voucher = a.VOUCHER,
                                   Efectivo = a.CANT_EFECTIVO
                               }).ToList();
            }
            return solicitudes;
        }
        public bool ImprimirFactura(string idFactura)
        {
            bool respuesta;
            PrintDocument printDocument1 = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            printDocument1.PrinterSettings = ps;
            printDocument1.PrinterSettings.PrinterName = "CAJA";

            //CONSULTA SOLICITUD
            var solicitud = ConsultaSolicitudMesa(Convert.ToDecimal(idFactura));
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
                e.Graphics.DrawString("- " + solicitud[0].IdFDian + " -", body, Brushes.Black, new RectangleF(0, margenY + YProductos, ancho, 15));
            };

            printDocument1.Print();
            respuesta = true;
            return respuesta;
        }
        public List<ConsultaSolicitudGeneral> ConsultaSolicitudMesa(decimal IdFactura)
        {
            List<ConsultaSolicitudGeneral> solicitudMesa = new List<ConsultaSolicitudGeneral>();
            using (DBLaColina context = new DBLaColina())
            {
                var ConsultaSolicitud = context.TBL_SOLICITUD.Where(a => a.ID == IdFactura).FirstOrDefault();
                if (ConsultaSolicitud != null)
                {
                    var lista = context.TBL_PRODUCTOS_SOLICITUD.Where(a => a.ID_SOLICITUD == ConsultaSolicitud.ID).ToList();
                    solicitudMesa.Add(new ConsultaSolicitudGeneral
                    {
                        Id = ConsultaSolicitud.ID,
                        FechaSolicitud = ConsultaSolicitud.FECHA_SOLICITUD,
                        IdMesa = ConsultaSolicitud.ID_MESA,
                        NumeroMesa = ConsultaSolicitud.ID_MESA != 999999 ? context.TBL_MASTER_MESAS.Where(z => z.ID == ConsultaSolicitud.ID_MESA).FirstOrDefault().NUMERO_MESA : ConsultaSolicitud.ID_MESA,
                        NombreMesa = ConsultaSolicitud.ID_MESA != 999999 ? context.TBL_MASTER_MESAS.Where(z => z.ID == ConsultaSolicitud.ID_MESA).FirstOrDefault().NOMBRE_MESA : Convert.ToString(ConsultaSolicitud.ID_MESA),
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
                        Total = ConsultaSolicitud.TOTAL,
                        ProductosSolicitud = new List<ProductosSolicitud>(),
                        Impuestos = new List<Impuestos>(),
                        IdCliente = ConsultaSolicitud.ID_CLIENTE,
                        FactracionElectronica = ConsultaSolicitud.FACTURACION_ELECTRONICA,
                        EnvioDian = ConsultaSolicitud.ENVIO_DIAN,
                        ValoresVouchers = ConsultaSolicitud.VALORES_VOUCHERS,
                        IdFDian = ConsultaSolicitud.ID_F_DIAN
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
                                Telefono = consultaClienteDian.TELEFONO
                            });
                        }
                    }
                    else
                    {
                        solicitudMesa[0].cliente = new ClienteDian();
                    }
                }
            }

            return solicitudMesa;
        }
        public bool ActualizaFactura(ConsultaSolicitud model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_SOLICITUD actualiza = new TBL_SOLICITUD();
                    actualiza = contex.TBL_SOLICITUD.Where(a => a.ID == model.NroFactura).FirstOrDefault();
                    if ((actualiza.METODO_PAGO).Trim() == "EFECTIVO")
                        model.Efectivo = Convert.ToDecimal(Math.Round(Convert.ToDouble(model.Total)));
                    if (actualiza != null)
                    {
                        actualiza.OTROS_COBROS = model.OtrosCobros;
                        actualiza.DESCUENTOS = model.Descuentos;
                        actualiza.PORCENTAJE_SERVICIO = model.PorcentajeServicio;
                        actualiza.SERVICIO_TOTAL = model.ServicioTotal;
                        actualiza.TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble(model.Total)));
                        if (model.Efectivo != null)
                            actualiza.CANT_EFECTIVO = model.Efectivo;
                        contex.SaveChanges();
                        Respuesta = true;
                    }
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public List<Propinas> ConsultaPropinasXFecha(DateTime FechaInicial, DateTime FechaFinal)
        {
            List<Propinas> propinas = new List<Propinas>();
            using (DBLaColina context = new DBLaColina())
            {
                propinas = (from a in context.TBL_DIAS_TRABAJADOS
                            join b in context.TBL_NOMINA
                            on a.ID_USUARIO_NOMINA equals b.ID
                            where a.FECHA_TRABAJADO >= FechaInicial && a.FECHA_TRABAJADO <= FechaFinal
                            group a by new { a.ID_USUARIO_NOMINA, b.NOMBRE, b.CEDULA } into TotPropinas
                            select new Propinas
                            {
                                IdSistema = TotPropinas.Key.ID_USUARIO_NOMINA,
                                Cedula = TotPropinas.Key.CEDULA,
                                Nombre = TotPropinas.Key.NOMBRE,
                                ServicioTotal = TotPropinas.Sum(x => x.PROPINAS)
                            }
                            ).ToList();
                if ( propinas.Count > 0 )
                {
                    propinas.LastOrDefault().TotalPropinas = (from a in context.TBL_CIERRES
                                                             where a.FECHA_HORA_CIERRE >= FechaInicial
                                                             && a.FECHA_HORA_CIERRE <= FechaFinal
                                                             //group a by a.VENTA_TOTAL into TotalVentas
                                                             select a.SERVICIO_TOTAL).Sum();
                }                
            }
            return propinas;
        }
    }
}