using ColinaApplication.Data.Business;
using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using ColinaApplication.Dian;
using ColinaApplication.Dian.Entity;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using Entity;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ColinaApplication.Hubs
{
    public class Solicitudhub : Hub
    {
        SolicitudBsuiness solicitud;
        BusinessDian businessDian;
        VentasBusiness ventas;

        public Solicitudhub()
        {
            solicitud = new SolicitudBsuiness();
            businessDian = new BusinessDian();
            ventas = new VentasBusiness();
        }
        public void ActualizaMesa(string id, string Estado, string IdUser, string Redirecciona, string Ruta)
        {
            solicitud.ActualizaEstadoMesa(Convert.ToDecimal(id), Estado);
            if (Estado == Estados.Ocupado && Redirecciona == "SI")
            {
                InsertaSolicitud(id, Estados.Ocupado, IdUser);
            }
            ListarEstadoMesas(Redirecciona, Convert.ToDecimal(id), Ruta);
        }
        public void InsertaSolicitud(string IdMesa, string Estado, string IdUser)
        {
            TBL_SOLICITUD model = new TBL_SOLICITUD();
            model.ID_MESA = Convert.ToDecimal(IdMesa);
            model.ID_MESERO = Convert.ToDecimal(IdUser);
            model.ESTADO_SOLICITUD = Estados.Abierta;
            model.OTROS_COBROS = 0;
            model.DESCUENTOS = 0;
            model.SUBTOTAL = 0;
            model.IVA_TOTAL = 0;
            model.I_CONSUMO_TOTAL = 0;
            model.PORCENTAJE_SERVICIO = 10;
            model.SERVICIO_TOTAL = 0;
            model.TOTAL = 0;
            model.ID_SOLICITUD_PRINCIPAL = 0;
            model.MESA_DIVIDIDA = "0";
            solicitud.InsertaSolicitud(model);

        }
        public void ListarEstadoMesas(string Redirecciona, decimal Idmesa, string Ruta)
        {
            List<TBL_MASTER_MESAS> listamesas = new List<TBL_MASTER_MESAS>();
            List<MasterMesas> mesas = new List<MasterMesas>();
            listamesas = solicitud.ListaMesas();
            Clients.All.ListaMesas(listamesas, Redirecciona, Idmesa, Ruta);
        }

        public void ConsultaMesaAbierta(string Id)
        {
            List<ConsultaSolicitudGeneral> ConsultaMesa = new List<ConsultaSolicitudGeneral>();
            ConsultaMesa = solicitud.ConsultaSolicitudMesa(Convert.ToDecimal(Id));
            Clients.All.ListaDetallesMesa(ConsultaMesa);
        }
        public void InsertaProductosSolicitud(List<TBL_PRODUCTOS_SOLICITUD> model, string idMesa)
        {
            bool cantDisponible = true;
            List<bool> resp = new List<bool>();
            List<string> data = new List<string>();
            List<TBL_PRODUCTOS_SOLICITUD> ConteoProductos = new List<TBL_PRODUCTOS_SOLICITUD>();

            ConteoProductos = AgrupaProductos(model);
            // VALIDA SI ALGUN PRODUCTO NO TIENE CANTIDAD EN EXISTENCIA
            foreach (var item in ConteoProductos)
            {
                var cantidaddisponible = solicitud.ConsultaCantidadProducto(item.ID_PRODUCTO);
                if (cantidaddisponible < item.ID)
                {
                    cantDisponible = false;
                    data.Add(item.ESTADO_PRODUCTO);
                }
            }

            if (cantDisponible)
            {
                var cont = 0;
                //IMPRIMIR TICKET
                resp = solicitud.ImprimirPedidoFactura(model, Convert.ToDecimal(idMesa));
                foreach (var item in model)
                {
                    if (resp[cont])
                        item.ESTADO_PRODUCTO = Estados.Entregado;
                    else
                        item.ESTADO_PRODUCTO = Estados.NoEntregado;

                    cont++;
                }

                //ACTUALIZA CANTIDAD PRODUCTO
                foreach (var item in ConteoProductos)
                {
                    var cantidaddisponible = solicitud.ConsultaCantidadProducto(item.ID_PRODUCTO);
                    var ActualizaCantidadProducto = solicitud.ActualizaCantidadProducto(item.ID_PRODUCTO, (cantidaddisponible - item.ID));
                }

                for (int i = 0; i < model.Count; i++)
                {
                    //INSERTA LOS PRODUCTOS EN LA SOLICITUD
                    for (int j = 0; j < model[i].ID; j++)
                    {
                        var InsertaSolicitud = solicitud.InsertaProductosSolicitud(model[i]);
                        if (InsertaSolicitud.ID != 0)
                        {
                            //ACTUALIZA TOTAL SOLICITUD
                            var ActualizaSolicitud = solicitud.ActualizaTotalSolicitud(model[i].ID_SOLICITUD, model[i].PRECIO_PRODUCTO);
                        }
                    }


                }
                Clients.Caller.GuardoProductos("Productos Insertados Exitosamente !");
                ConsultaMesaAbierta(idMesa);
            }
            else
            {
                Clients.Caller.FaltaExistencias(data);
                ConsultaMesaAbierta(idMesa);
            }
        }
        public List<TBL_PRODUCTOS_SOLICITUD> AgrupaProductos(List<TBL_PRODUCTOS_SOLICITUD> productosSolicitud)
        {
            List<TBL_PRODUCTOS_SOLICITUD> resultado = new List<TBL_PRODUCTOS_SOLICITUD>();
            var distinctProductos = productosSolicitud.DistinctBy(c => c.ID_PRODUCTO).ToList();
            foreach (var item in distinctProductos)
            {
                TBL_PRODUCTOS_SOLICITUD model = new TBL_PRODUCTOS_SOLICITUD();
                //CANTIDAD DEL PRODUCTO
                model.ID = productosSolicitud.Where(x => x.ID_PRODUCTO == item.ID_PRODUCTO).Sum(x => x.ID);
                model.ID_PRODUCTO = item.ID_PRODUCTO;
                //NOMBRE PRODUCTO
                model.ESTADO_PRODUCTO = item.ESTADO_PRODUCTO;

                resultado.Add(model);
            }
            return resultado;
        }
        public void GuardaDatosCliente(decimal Id, string Cedula, string NombreCliente, string Observaciones, string OtrosCobros,
            string Descuentos, string SubTotal, string Estado, string IdMesa, decimal porcentajeServicio, string MetodoPago, List<Payments> pagos,
            string CantEfectivo, decimal idMesero, List<string> datosDianCliente, decimal idCliente, string FactElect, bool DianSistema,
            decimal valorTotal, string BorradorDian, string IdMesaPrincipal)
        {
            Cliente cliente = new Cliente();
            TBL_CLIENTES_DIAN clienteDian = new TBL_CLIENTES_DIAN();
            List<ProductosSolicitud> ProductosXSolicitud = new List<ProductosSolicitud>();
            Factura factura = new Factura();
            var vouchers = "";
            var valoresVouchers = "";

            //INSERTA CLIENTE DIAN -- NUNCA ACTUALIZA
            if (datosDianCliente[0] == "true" && idCliente == 0 && datosDianCliente.Count > 2 && ConfigurationManager.AppSettings["DIAN_ON"] == "1")
            {
                cliente.person_type = datosDianCliente[1];
                cliente.id_type = new Id_type();
                cliente.id_type.code = datosDianCliente[2];
                cliente.identification = Cedula.Trim();
                cliente.check_digit = datosDianCliente[16];
                cliente.name = new List<string>();
                if (datosDianCliente[1] == "Person")
                {
                    cliente.name.Add(NombreCliente);
                    cliente.name.Add(datosDianCliente[3]);
                }
                else
                {
                    cliente.name.Add(datosDianCliente[6]);
                }
                cliente.commercial_name = datosDianCliente[4];
                cliente.vat_responsible = Convert.ToBoolean(datosDianCliente[9]);
                cliente.address = new Address();
                cliente.address.address = datosDianCliente[5];
                cliente.contacts = new List<Contacts>();
                //cliente.contacts.phone = new Phone();
                cliente.contacts.Add(new Contacts { email = datosDianCliente[8], phone = new Phone { number = datosDianCliente[7] } });
                cliente.fiscal_responsibilities = new List<Fiscal_responsibilities>();
                if (datosDianCliente[10] == "true")
                    cliente.fiscal_responsibilities.Add(new Fiscal_responsibilities { code = "O-13", name = "Gran Contribuyente" });
                if (datosDianCliente[11] == "true")
                    cliente.fiscal_responsibilities.Add(new Fiscal_responsibilities { code = "O-15", name = "Autorretenedor" });
                if (datosDianCliente[12] == "true")
                    cliente.fiscal_responsibilities.Add(new Fiscal_responsibilities { code = "O-23", name = "Agente de retencion IVA" });
                if (datosDianCliente[13] == "true")
                    cliente.fiscal_responsibilities.Add(new Fiscal_responsibilities { code = "O-47", name = "Regimen simple de tributacion" });
                if (datosDianCliente[14] == "true")
                    cliente.fiscal_responsibilities.Add(new Fiscal_responsibilities { code = "R-99-PN", name = "No aplica - Otros" });
                cliente = businessDian.InsertaCliente(datosDianCliente[15], cliente);
                if (cliente.id != null)
                {
                    clienteDian.TIPO_PERSONA = cliente.person_type;
                    clienteDian.CODIGO_DOCUMENTO = cliente.id_type.code;
                    clienteDian.NOMBRE_DOCUMENTO = cliente.id_type.name;
                    clienteDian.NUMERO_IDENTIFICACION = cliente.identification;
                    clienteDian.DIGITO_VERIFI = cliente.check_digit;
                    clienteDian.NOMBRES = cliente.name[0];
                    clienteDian.APELLIDOS = cliente.name.Count > 1 ? cliente.name[1] : "NA";
                    clienteDian.RAZON_SOCIAL = datosDianCliente[6];
                    clienteDian.NOMBRE_COMERCIAL = cliente.commercial_name;
                    clienteDian.DIRECCION = cliente.address.address;
                    clienteDian.COD_CIUDAD = "0";
                    clienteDian.NOM_CIUDAD = "";
                    clienteDian.EMAIL = cliente.contacts[0].email;
                    clienteDian.RESPONSABLE_IVA = cliente.vat_responsible;
                    string CodRFiscal = string.Empty;
                    string NomRFiscal = string.Empty;
                    int contador = 0;
                    foreach (var cf in cliente.fiscal_responsibilities)
                    {
                        contador++;
                        CodRFiscal += cf.code;
                        NomRFiscal += cf.name;
                        if (contador != cliente.fiscal_responsibilities.Count)
                            CodRFiscal += ";";
                        NomRFiscal += ";";
                    }
                    clienteDian.CODIGO_R_FISCAL = CodRFiscal;
                    clienteDian.NOMBRE_R_FISCAL = NomRFiscal;
                    clienteDian.ID_CODIGO_DIAN = cliente.id;
                    clienteDian.TELEFONO = cliente.contacts[0].phone.number;
                    clienteDian = solicitud.InsertaClientesDian(clienteDian);
                    idCliente = clienteDian.ID;
                    FactElect = "1";
                }
            }

            if (Estado == Estados.Finalizada)
            {
                //CONSULTA CLIENTE DIAN
                if (idCliente != 0)
                    clienteDian = solicitud.ConsultaCedulaId(idCliente);
                else
                    clienteDian = solicitud.ConsultaCedula("222222222222");
                //LLENA MODELO PARA ENVIAR                    
                factura.document = new Document();
                factura.document.id = 31722;
                factura.date = Convert.ToString(DateTime.Now.Year) + "-" + Convert.ToString(DateTime.Now.Month.ToString("D2")) + "-" + Convert.ToString(DateTime.Now.Day.ToString("D2"));
                factura.customer = new Customer();
                factura.customer.person_type = clienteDian.TIPO_PERSONA;
                factura.customer.id_type = clienteDian.CODIGO_DOCUMENTO;
                factura.customer.identification = clienteDian.NUMERO_IDENTIFICACION;
                factura.customer.branch_office = 0;
                factura.customer.name = new List<string>();
                factura.customer.contacts = new List<Contacts>();
                if (clienteDian.TIPO_PERSONA == "Person")
                {
                    factura.customer.name.Add(clienteDian.NOMBRES);
                    factura.customer.name.Add(clienteDian.APELLIDOS);
                    factura.customer.contacts.Add(new Contacts { first_name = clienteDian.NOMBRES, last_name = clienteDian.APELLIDOS, email = clienteDian.EMAIL, phone = new Phone { number = clienteDian.TELEFONO } });
                }
                else
                {
                    factura.customer.name.Add(clienteDian.RAZON_SOCIAL);
                    factura.customer.contacts.Add(new Contacts { first_name = clienteDian.RAZON_SOCIAL, email = clienteDian.EMAIL, phone = new Phone { number = clienteDian.TELEFONO } });
                }
                factura.customer.address = new Address();
                factura.customer.address.address = clienteDian.DIRECCION;
                //factura.customer.address.city = new City();
                //factura.customer.address.city.city_code = "CO";
                //factura.customer.address.city.country_name = "COLOMBIA";
                //factura.customer.address.city.state_code =
                factura.customer.phone = new List<Phone>();
                factura.customer.phone.Add(new Phone { number = clienteDian.TELEFONO });
                factura.seller = 444;
                factura.stamp = new Stamp();
                factura.stamp.send = BorradorDian == "SI" ? false : true;
                factura.mail = new Mail();
                factura.mail.send = BorradorDian == "SI" ? false : datosDianCliente[0] == "true" ? true : false;
                //CONSULTA PRODUCTOS
                ProductosXSolicitud = solicitud.ConsultaProductoSolicitudId(Convert.ToDecimal(Id));
                ProductosXSolicitud = solicitud.AgrupaProductos(ProductosXSolicitud);
                factura.items = new List<Items>();
                foreach (var item in ProductosXSolicitud)
                {
                    Producto producto = new Producto();
                    producto = businessDian.ConsultaProductosDianId(datosDianCliente[1], Convert.ToString(item.IdDian));
                    factura.items.Add(new Items { code = producto.code, quantity = item.Id, description = item.NombreProducto, price = Convert.ToDecimal(item.PrecioProducto), taxes = new List<Taxes> { new Taxes { id = 9286 } } });
                }
                factura.payments = new List<Payments>();
                if (MetodoPago == "EFECTIVO")
                {
                    factura.payments.Add(new Payments { id = "3970", value = Convert.ToInt32(valorTotal) });
                }
                else if (MetodoPago == "AMBAS")
                {
                    factura.payments.Add(new Payments { id = "3970", value = Convert.ToInt32(CantEfectivo) });
                    foreach (var item in pagos)
                    {
                        vouchers += item.id + ";";
                        valoresVouchers += item.value + ";";
                        if (item.name == "Debito")
                            item.id = "3972";
                        if (item.name == "Credito")
                            item.id = "3973";
                        factura.payments.Add(new Payments { id = item.id, value = item.value });
                    }
                }
                else
                {
                    foreach (var item in pagos)
                    {
                        vouchers += item.id + ";";
                        valoresVouchers += item.value + ";";
                        if (item.name == "Debito")
                            item.id = "3972";
                        if (item.name == "Credito")
                            item.id = "3973";
                        factura.payments.Add(new Payments { id = item.id, value = item.value });
                    }
                }
                factura.globaldiscounts = new List<Globaldiscounts>();
                factura.globaldiscounts.Add(new Globaldiscounts { id = 1, value = Convert.ToInt32(Math.Round(Convert.ToDouble((Convert.ToDecimal(SubTotal) * porcentajeServicio) / 100), 0)) });
                if (ConfigurationManager.AppSettings["DIAN_ON"] == "1" && DianSistema && factura.payments[0].value > 0)
                {
                    //LLAMA ENVIAR FACTURA DIAN
                    factura = businessDian.InsertaFactura(datosDianCliente[1], factura, Id);
                }
            }

            //ACTUALIZA SOLICITUD
            TBL_SOLICITUD model = new TBL_SOLICITUD();
            model.ID = Id;
            model.ID_MESERO = idMesero;
            model.ID_MESA = Convert.ToDecimal(IdMesa);
            model.IDENTIFICACION_CLIENTE = Cedula != "0" && Cedula != "null" ? Cedula : "222222222222";
            model.NOMBRE_CLIENTE = NombreCliente != "undefined undefined" ? NombreCliente : "Consumidor Final";
            model.OBSERVACIONES = !string.IsNullOrEmpty(Observaciones) ? model.OBSERVACIONES = Observaciones.ToUpper() : model.OBSERVACIONES = Observaciones;
            model.ESTADO_SOLICITUD = Estado;
            model.OTROS_COBROS = string.IsNullOrEmpty(OtrosCobros) ? 0 : Convert.ToDecimal(OtrosCobros);
            model.DESCUENTOS = string.IsNullOrEmpty(Descuentos) ? 0 : Convert.ToDecimal(Descuentos);
            model.SUBTOTAL = Convert.ToDecimal(SubTotal);
            model.PORCENTAJE_SERVICIO = Convert.ToDecimal(porcentajeServicio);
            model.METODO_PAGO = MetodoPago;
            model.VOUCHER = vouchers;
            model.CANT_EFECTIVO = Convert.ToDecimal(CantEfectivo);
            model.ID_CLIENTE = idCliente;
            model.FACTURACION_ELECTRONICA = FactElect;
            model.ENVIO_DIAN = factura.id != null ? "1" : "0";
            model.VALORES_VOUCHERS = valoresVouchers;
            model.ID_F_DIAN = factura.id != null ? factura.id : "0";
            model.MESA_DIVIDIDA = "0";
            var respuesta = solicitud.ActualizaSolicitud(model);
            if (model.ESTADO_SOLICITUD == Estados.Finalizada)
                ventas.ImprimirFactura(Convert.ToString(model.ID));
            Clients.Caller.GuardoCliente(respuesta);
            if (IdMesa != "999999")
                ConsultaMesaAbierta(IdMesa);
            else
                ConsultaMesaAbierta(IdMesaPrincipal);
        }
        public void CancelaPedido(decimal IdSolicitud, bool RetornaInventario)
        {
            var respuesta = solicitud.CancelaProductosSolicitud(IdSolicitud, RetornaInventario);

        }
        public void CancelaPedidoXId(decimal IdProductoSolicitud, bool RetornaInventario)
        {
            var respuesta = solicitud.CancelaProductoSolicitudXId(IdProductoSolicitud, RetornaInventario);
        }
        public void ImprimirFactura(string IdMesa)
        {
            bool respuesta = solicitud.ImprimirFactura(IdMesa);
            ConsultaMesaAbierta(IdMesa);
        }
        public bool ImprimeProductos(string cantidad, string idproducto, string descripcion, string idMesa, string idprodsolic)
        {
            bool respuesta = solicitud.ImprimirPedido(cantidad, idproducto, descripcion, idMesa, idprodsolic);
            //ACTUALIZA ESTADO DEL PRODUCTO REENVIADO
            return respuesta;
        }
        public void ActualizaIdmesaHTML(string idmesa, string idmesaAnterior)
        {
            Clients.All.CambiaIdMesa(idmesa, idmesaAnterior);
        }
        public void DivideCuenta(string idSolicitudPrincipal, string idUser, List<TBL_PRODUCTOS_SOLICITUD> model, decimal servicio)
        {
            //CALCULOS INICIALES
            decimal? SubTotalDC = 0;
            foreach (var item in model)
                SubTotalDC += item.PRECIO_PRODUCTO;
            decimal? IConsumoTotalDC = ((SubTotalDC * 8) / 100);
            decimal? ServTotalDC = ((SubTotalDC * servicio) / 100);
            decimal? TotalDC = SubTotalDC + IConsumoTotalDC + ServTotalDC;

            //INSERTA NUEVA SOLICITUD
            TBL_SOLICITUD modelI = new TBL_SOLICITUD();
            modelI.ID_MESA = Convert.ToDecimal("999999");
            modelI.ID_MESERO = Convert.ToDecimal(idUser);
            modelI.ESTADO_SOLICITUD = "MESA DIVIDIDA";
            modelI.OTROS_COBROS = 0;
            modelI.DESCUENTOS = 0;
            modelI.SUBTOTAL = SubTotalDC;
            modelI.IVA_TOTAL = 0;
            modelI.I_CONSUMO_TOTAL = IConsumoTotalDC;
            modelI.PORCENTAJE_SERVICIO = servicio;
            modelI.SERVICIO_TOTAL = ServTotalDC;
            modelI.TOTAL = TotalDC;
            modelI.ID_SOLICITUD_PRINCIPAL = Convert.ToDecimal(idSolicitudPrincipal);
            modelI.MESA_DIVIDIDA = "0";
            var IdNuevaSolicitud = solicitud.InsertaSolicitud(modelI);

            //ACTUALIZA PRODUCTOS CON NUEVA SOLICITUD
            var ActuProd = solicitud.ActualizaIdProductosSolicitud(Convert.ToDecimal(IdNuevaSolicitud), model);

            //IMPRIME FACTURA DIVIDIDA
            bool respuesta = solicitud.ImprimirFactura("999999");

            //ACTUALIZA SOLICITUD PRINCIPAL
            var respMesaDivid = solicitud.ActualizaMesaPrincipalDividida(Convert.ToDecimal(idSolicitudPrincipal), modelI.SUBTOTAL);

            var CantidadProdActual = ActuProd - model.Count;
            Clients.All.DividioCuenta(IdNuevaSolicitud, idSolicitudPrincipal, modelI.SUBTOTAL, modelI.PORCENTAJE_SERVICIO, modelI.TOTAL, CantidadProdActual);

        }
        public void ImprimeProductosMasivo(List<ProductosSolicitud>productos, string idMesa)
        {
            List<ProductosSolicitud> result = new List<ProductosSolicitud>();
            result = solicitud.AgrupaProductos(productos);
            foreach (var item in result)
            {
                bool respuesta = solicitud.ImprimirPedido(Convert.ToString(item.Id), Convert.ToString(item.IdProducto), item.Descripcion, idMesa, Convert.ToString(item.IdSolicitud));                
            }
            ConsultaMesaAbierta(idMesa);
        }
    }
}