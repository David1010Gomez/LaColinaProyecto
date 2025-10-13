using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using ColinaApplication.Dian;
using ColinaApplication.Dian.Entity;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ColinaApplication.Data.Business
{
    public class ConfiguracionesBusiness
    {
        BusinessDian businessDian;
        HomeBusiness inicio;
        public ConfiguracionesBusiness()
        {
            businessDian = new BusinessDian();
            inicio = new HomeBusiness();
        }
        public List<TBL_CATEGORIAS> ListaCategorias()
        {
            List<TBL_CATEGORIAS> listCategorias = new List<TBL_CATEGORIAS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listCategorias = contex.TBL_CATEGORIAS.ToList();
            }
            return listCategorias;
        }
        public List<TBL_PRODUCTOS> ListaProductos()
        {
            List<TBL_PRODUCTOS> listProductos = new List<TBL_PRODUCTOS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listProductos = contex.TBL_PRODUCTOS.ToList();
            }
            return listProductos;
        }
        public List<TBL_MASTER_MESAS> ListaMesas()
        {
            List<TBL_MASTER_MESAS> listMesas = new List<TBL_MASTER_MESAS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listMesas = contex.TBL_MASTER_MESAS.ToList();
            }
            return listMesas;
        }
        public List<TBL_PERFIL> ListaPerfiles()
        {
            List<TBL_PERFIL> listPerfiles = new List<TBL_PERFIL>();
            using (DBLaColina contex = new DBLaColina())
            {
                listPerfiles = contex.TBL_PERFIL.ToList();
            }
            return listPerfiles;
        }
        public List<TBL_USUARIOS> ListaUsuarios()
        {
            List<TBL_USUARIOS> listMesas = new List<TBL_USUARIOS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listMesas = contex.TBL_USUARIOS.ToList();
            }
            return listMesas;
        }
        public List<TBL_IMPUESTOS> ListaImpuestos()
        {
            List<TBL_IMPUESTOS> listImpuestos = new List<TBL_IMPUESTOS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listImpuestos = contex.TBL_IMPUESTOS.ToList();
            }
            return listImpuestos;
        }
        public List<TBL_IMPRESORAS> ListaImpresoras()
        {
            List<TBL_IMPRESORAS> listImpresoras = new List<TBL_IMPRESORAS>();
            using (DBLaColina contex = new DBLaColina())
            {
                listImpresoras = contex.TBL_IMPRESORAS.ToList();
            }
            return listImpresoras;
        }
        public List<TBL_NOMINA> ListaNominaEmpleados()
        {
            List<TBL_NOMINA> listImpuestos = new List<TBL_NOMINA>();
            using (DBLaColina contex = new DBLaColina())
            {
                listImpuestos = contex.TBL_NOMINA.ToList();
            }
            return listImpuestos;
        }
        public List<TBL_SISTEMA> ListaSistema()
        {
            List<TBL_SISTEMA> listSistema = new List<TBL_SISTEMA>();
            using (DBLaColina contex = new DBLaColina())
            {
                listSistema = contex.TBL_SISTEMA.ToList();
            }
            return listSistema;
        }
        public bool InsertaCategoria(TBL_CATEGORIAS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_CATEGORIAS.Add(model);
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
        public bool ActualizaCategoria(TBL_CATEGORIAS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_CATEGORIAS actualiza = new TBL_CATEGORIAS();
                    actualiza = contex.TBL_CATEGORIAS.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.CATEGORIA = model.CATEGORIA;
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
        public bool InsertaProducto(TBL_PRODUCTOS model, string token)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    //INSERTA TABLAS LOCALES
                    model.FECHA_INGRESO = DateTime.Now;
                    model.UP_DIAN = 0;
                    contex.TBL_PRODUCTOS.Add(model);
                    contex.SaveChanges();

                    //INSERTA DIAN
                    if (ConfigurationManager.AppSettings["DIAN_ON"] == "1")
                    {
                        Producto modelP = new Producto();
                        modelP.code = Convert.ToString("PROD-" + model.ID);
                        modelP.name = model.NOMBRE_PRODUCTO;
                        modelP.account_groupSend = Convert.ToInt32(model.ACCOUNT_GROUP_DIAN);
                        modelP.taxes = new List<Taxes> { new Taxes() };
                        modelP.taxes[0].id = 9286;
                        modelP.taxes[0].name = "Impoconsumo 8%";
                        modelP.taxes[0].type = "Impoconsumo";
                        modelP.taxes[0].percentage = 8;
                        modelP.prices = new List<Prices> { new Prices() };
                        modelP.prices[0].currency_code = "COP";
                        modelP.prices[0].price_list = new List<PriceList> { new PriceList() };
                        modelP.prices[0].price_list[0].position = 1;
                        modelP.prices[0].price_list[0].value = Convert.ToInt32(model.PRECIO);
                        var resultado = businessDian.InsertaProducto(token, modelP);
                        if (resultado.id != null)
                        {
                            var actualiza = contex.TBL_PRODUCTOS.Where(a => a.ID == model.ID).FirstOrDefault();
                            if (actualiza != null)
                            {
                                actualiza.UP_DIAN = 1;
                                actualiza.ID_DIAN = resultado.id;
                                actualiza.ACCOUNT_GROUP_DIAN = modelP.account_groupSend;
                                contex.SaveChanges();
                            }
                        }
                    }
                    Respuesta = true;
                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public bool ActualizaProducto(TBL_PRODUCTOS model, string token)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    //ACTUALIZA DIAN 
                    Producto producto = new Producto();
                    if (ConfigurationManager.AppSettings["DIAN_ON"] == "1")
                    {
                        var DianActualiza = businessDian.ConsultaProductosDianId(token, model.ID_DIAN);
                        if (DianActualiza.id != null)
                        {
                            producto.id = model.ID_DIAN;
                            producto.account_groupSend = Convert.ToInt32(model.ACCOUNT_GROUP_DIAN); //no puede ser cero
                            producto.code = Convert.ToString(DianActualiza.code);
                            producto.name = model.NOMBRE_PRODUCTO;
                            //producto.description = model.DESCRIPCION;
                            producto.prices = new List<Prices> { new Prices() };
                            producto.prices[0].currency_code = "COP";
                            producto.prices[0].price_list = new List<PriceList> { new PriceList() };
                            producto.prices[0].price_list[0].position = 1;
                            producto.prices[0].price_list[0].value = Convert.ToInt32(model.PRECIO);
                            producto = businessDian.ActualizarProducto(token, producto);
                        }
                        //else
                        //{
                        //    producto.code = Convert.ToString("PROD-" + model.ID);
                        //    producto.name = model.NOMBRE_PRODUCTO;
                        //    producto.account_groupSend = Convert.ToInt32(model.ACCOUNT_GROUP_DIAN);
                        //    producto.taxes = new List<Taxes> { new Taxes() };
                        //    producto.taxes[0].id = 9286;
                        //    producto.prices = new List<Prices> { new Prices() };
                        //    producto.prices[0].currency_code = "COP";
                        //    producto.prices[0].price_list = new List<PriceList> { new PriceList() };
                        //    producto.prices[0].price_list[0].position = 1;
                        //    producto.prices[0].price_list[0].value = model.PRECIO;
                        //    producto = businessDian.InsertaProducto(token, producto);
                        //}
                    }

                    //ACTUALIZA TABLA LOCAL
                    TBL_PRODUCTOS actualiza = new TBL_PRODUCTOS();
                    actualiza = contex.TBL_PRODUCTOS.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.ID_CATEGORIA = model.ID_CATEGORIA;
                        actualiza.NOMBRE_PRODUCTO = model.NOMBRE_PRODUCTO;
                        actualiza.PRECIO = model.PRECIO;
                        actualiza.CANTIDAD = model.CANTIDAD;
                        actualiza.DESCRIPCION = model.DESCRIPCION;
                        actualiza.ID_IMPRESORA = model.ID_IMPRESORA;
                        actualiza.UP_DIAN = producto.id != null ? 1 : 0;
                        actualiza.ID_DIAN = producto.id != null && (actualiza.ID_DIAN == "0" || actualiza.ID_DIAN == null) ? producto.id : actualiza.ID_DIAN;
                        //actualiza.ACCOUNT_GROUP_DIAN = producto.account_group != null ? producto.account_group.id : 0;
                        contex.SaveChanges();
                        
                        List<TBL_SOLICITUD> solicitudesAbiertas = new List<TBL_SOLICITUD>();
                        solicitudesAbiertas = contex.TBL_SOLICITUD.Where(x => x.ESTADO_SOLICITUD == Estados.Abierta).ToList();
                        List<TBL_PRODUCTOS_SOLICITUD> actualizaProdSolicitud = new List<TBL_PRODUCTOS_SOLICITUD>();
                        foreach (var item in solicitudesAbiertas)
                        {
                            actualizaProdSolicitud = contex.TBL_PRODUCTOS_SOLICITUD.Where(x => x.ID_SOLICITUD == item.ID).ToList();
                            foreach (var item2 in actualizaProdSolicitud)
                            {
                                if (item2.ID_PRODUCTO == actualiza.ID)
                                {
                                    var valorAnterior = item2.PRECIO_PRODUCTO;
                                    item2.PRECIO_PRODUCTO = Convert.ToDecimal(model.PRECIO);
                                    item.SUBTOTAL = (item.SUBTOTAL - valorAnterior) + item2.PRECIO_PRODUCTO;
                                    item.IVA_TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble((item.PORCENTAJE_IVA * item.SUBTOTAL) / 100), 15));
                                    item.I_CONSUMO_TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble((item.PORCENTAJE_I_CONSUMO * item.SUBTOTAL) / 100), 15));
                                    item.SERVICIO_TOTAL = Convert.ToDecimal(Math.Round(Convert.ToDouble((item.PORCENTAJE_SERVICIO * item.SUBTOTAL) / 100), 15));
                                    item.TOTAL = item.SUBTOTAL + item.IVA_TOTAL + item.I_CONSUMO_TOTAL + item.SERVICIO_TOTAL;
                                    contex.SaveChanges();
                                }
                            }
                        }
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
        public bool InsertaMesa(TBL_MASTER_MESAS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_MASTER_MESAS.Add(model);
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
        public bool ActualizaMesa(TBL_MASTER_MESAS model)
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
                        actualiza.NOMBRE_MESA = model.NOMBRE_MESA;
                        actualiza.DESCRIPCION = model.DESCRIPCION;
                        actualiza.ESTADO = model.ESTADO;
                        actualiza.ID_USUARIO = model.ID_USUARIO;
                        actualiza.NUMERO_MESA = model.NUMERO_MESA;
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
        public bool InsertaUsuario(TBL_USUARIOS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_USUARIOS buscaUsuario = new TBL_USUARIOS();
                    buscaUsuario = contex.TBL_USUARIOS.Where(a => a.CEDULA == model.CEDULA || a.CONTRASEÑA == model.CONTRASEÑA).FirstOrDefault();
                    if (buscaUsuario == null)
                    {
                        contex.TBL_USUARIOS.Add(model);
                        contex.SaveChanges();
                        Respuesta = true;
                    }
                    else
                    {
                        Respuesta = false;
                    }

                }
                catch (Exception e)
                {
                    Respuesta = false;
                }
            }
            return Respuesta;
        }
        public bool ActualizaUsuario(TBL_USUARIOS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_USUARIOS actualiza = new TBL_USUARIOS();
                    actualiza = contex.TBL_USUARIOS.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.CEDULA = model.CEDULA;
                        actualiza.NOMBRE = model.NOMBRE;
                        actualiza.CONTRASEÑA = model.CONTRASEÑA;
                        actualiza.ID_PERFIL = model.ID_PERFIL;
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
        public bool InsertaImpresora(TBL_IMPRESORAS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_IMPRESORAS.Add(model);
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
        public bool ActualizaImpresora(TBL_IMPRESORAS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_IMPRESORAS actualiza = new TBL_IMPRESORAS();
                    actualiza = contex.TBL_IMPRESORAS.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.NOMBRE_IMPRESORA = model.NOMBRE_IMPRESORA;
                        actualiza.DESCRIPCION = model.DESCRIPCION;
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
        public bool InsertaImpuesto(TBL_IMPUESTOS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_IMPUESTOS.Add(model);
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
        public bool ActualizaImpuesto(TBL_IMPUESTOS model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_IMPUESTOS actualiza = new TBL_IMPUESTOS();
                    actualiza = contex.TBL_IMPUESTOS.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.NOMBRE_IMPUESTO = model.NOMBRE_IMPUESTO;
                        actualiza.PORCENTAJE = model.PORCENTAJE;
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
        public bool InsertaPerfil(TBL_PERFIL model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_PERFIL.Add(model);
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
        public bool ActualizaPerfil(TBL_PERFIL model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_PERFIL actualiza = new TBL_PERFIL();
                    actualiza = contex.TBL_PERFIL.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.NOMBRE_PERFIL = model.NOMBRE_PERFIL;
                        actualiza.PORCENTAJE_PROPINA = model.PORCENTAJE_PROPINA;
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
        public bool InsertaNominaEmpleados(TBL_NOMINA model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    contex.TBL_NOMINA.Add(model);
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
        public bool ActualizaNominaEmpleados(TBL_NOMINA model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_NOMINA actualiza = new TBL_NOMINA();
                    actualiza = contex.TBL_NOMINA.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.ID_USUARIO_SISTEMA = model.ID_USUARIO_SISTEMA;
                        actualiza.ID_PERFIL = model.ID_PERFIL;
                        actualiza.CEDULA = model.CEDULA;
                        actualiza.NOMBRE = model.NOMBRE;
                        actualiza.CARGO = model.CARGO;

                        actualiza.FECHA_NACIMIENTO = model.FECHA_NACIMIENTO;
                        actualiza.DIRECCION_RESIDENCIA = model.DIRECCION_RESIDENCIA;
                        actualiza.TELEFONO = model.TELEFONO;
                        actualiza.ESTADO = model.ESTADO;
                        actualiza.FECHA_PAGO = model.FECHA_PAGO;
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

        public bool ConsultaCedula(decimal Cedula)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_USUARIOS consultaCedula = new TBL_USUARIOS();
                    consultaCedula = contex.TBL_USUARIOS.Where(a => a.CEDULA == Cedula).FirstOrDefault();
                    if (consultaCedula != null)
                    {
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
        public bool ConsultaCodigo(string Contrasena)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_USUARIOS consultaCodigo = new TBL_USUARIOS();
                    consultaCodigo = contex.TBL_USUARIOS.Where(a => a.CONTRASEÑA == Contrasena).FirstOrDefault();
                    if (consultaCodigo != null)
                    {
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
        public bool ActualizaSistema(TBL_SISTEMA model)
        {
            bool Respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    TBL_SISTEMA actualiza = new TBL_SISTEMA();
                    actualiza = contex.TBL_SISTEMA.Where(a => a.ID == model.ID).FirstOrDefault();
                    if (actualiza != null)
                    {
                        actualiza.VALOR = model.VALOR;
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
        public List<Account_group> ListaAccountGroup(string token)
        {
            List<Account_group> listAccountGroup = new List<Account_group>();
            listAccountGroup = businessDian.ConsultaAccountGroupDian(token);
            return listAccountGroup;

        }
        public string ActualizaProductosDianColina(string token)
        {
            string respuesta = string.Empty;
            List<Producto> listProducto = new List<Producto>();
            bool Inicia = true;
            int page = 1;
            int contador = 0;
            int contadorProd = 0;
            while (Inicia)
            {
                var resultado = businessDian.ConsultaProductosDian(token, page);
                var total = Convert.ToInt64(resultado[0].code);
                if (resultado.Count > 0)
                {
                    listProducto.AddRange(resultado);
                    contador += 100;
                    page++;
                    if (total < contador)
                        Inicia = false;
                }
                else
                {
                    Inicia = false;
                }
                
            }

            foreach (var item in listProducto)
            {
                using (DBLaColina contex = new DBLaColina())
                {
                    try
                    {
                        TBL_PRODUCTOS actualiza = new TBL_PRODUCTOS();
                        actualiza = contex.TBL_PRODUCTOS.Where(a => a.NOMBRE_PRODUCTO.Trim() == item.name.Trim()).FirstOrDefault();
                        if (actualiza != null)
                        {
                            actualiza.NOMBRE_PRODUCTO = item.name;
                            actualiza.PRECIO = item.prices != null ? Convert.ToString(Math.Truncate(item.prices[0].price_list[0].value)) : "1000";
                            //CANTIDAD                            
                            actualiza.ID_DIAN = item.id;
                            actualiza.ACCOUNT_GROUP_DIAN = item.account_group.id;
                            contex.SaveChanges();
                            contadorProd++;
                        }
                        else
                        {
                            //INSERTA TABLAS LOCALES
                            TBL_PRODUCTOS model = new TBL_PRODUCTOS();
                            model.ID_CATEGORIA = 2;
                            model.FECHA_INGRESO = DateTime.Now;
                            model.NOMBRE_PRODUCTO = item.name;
                            model.PRECIO = item.prices != null ? Convert.ToString(Math.Truncate(item.prices[0].price_list[0].value)) : "1000";
                            model.CANTIDAD = 0;
                            model.DESCRIPCION = "-";
                            model.ID_IMPRESORA = 1;
                            model.UP_DIAN = 1;
                            model.ID_DIAN = item.id;
                            model.ACCOUNT_GROUP_DIAN = item.account_group.id;
                            contex.TBL_PRODUCTOS.Add(model);
                            contex.SaveChanges();
                            contadorProd++;
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }
            respuesta = "Registros actualizados Dian -> La Colina " + contadorProd.ToString();
            return respuesta;
        }
        public string ActualizaProductosColinaDian(string token)
        {
            string respuesta = string.Empty;
            List<TBL_PRODUCTOS> listProducto = new List<TBL_PRODUCTOS>();
            int contador = 0;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {
                    listProducto = contex.TBL_PRODUCTOS.ToList();
                    foreach (var item in listProducto)
                    {
                        if (item.ID_DIAN == null || item.ID_DIAN == "" || item.ID_DIAN == "0")
                        {
                            Producto modelP = new Producto();
                            modelP.code = Convert.ToString("PROD-" + item.ID);
                            modelP.name = item.NOMBRE_PRODUCTO;
                            modelP.account_groupSend = Convert.ToInt32(item.ACCOUNT_GROUP_DIAN);
                            modelP.taxes = new List<Taxes> { new Taxes() };
                            modelP.taxes[0].id = 9748;
                            modelP.taxes[0].name = "Impoconsumo 8%";
                            modelP.taxes[0].type = "Impoconsumo";
                            modelP.taxes[0].percentage = 8;
                            modelP.prices = new List<Prices> { new Prices() };
                            modelP.prices[0].currency_code = "COP";
                            modelP.prices[0].price_list = new List<PriceList> { new PriceList() };
                            modelP.prices[0].price_list[0].position = 1;
                            modelP.prices[0].price_list[0].value = Convert.ToInt32(item.PRECIO);
                            var insertaDian = businessDian.InsertaProducto(token, modelP);
                            item.ID_DIAN = insertaDian.id != null ? insertaDian.id : "0";
                            contex.SaveChanges();
                            contador++;
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            respuesta = "Registros actualizados La Colina -> Dian " + contador.ToString();
            return respuesta;
        }
    }
}