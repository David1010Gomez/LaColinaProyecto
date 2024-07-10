using ColinaApplication.Data.Business;
using ColinaApplication.Data.Conexion;
using ColinaApplication.Dian;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ColinaApplication.Data.Clases
{
    public class Expiring_Filter : ActionFilterAttribute
    {
        BusinessDian businessDian;
        HomeBusiness inicio;

        public Expiring_Filter()
        {
            businessDian = new BusinessDian();
            inicio = new HomeBusiness();
        }

        //Aca igual se puede hacer vencimiento de sesion a 30 min if Session["timeCheck"] -Date.Now() >30 then ctx.Session= null;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            if (HttpContext.Current.Session["IdPerfil"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/LaColinaLogin");
                return;
            }

            if ((HttpContext.Current.Session["IdPerfil"].ToString() == "1" || HttpContext.Current.Session["IdPerfil"].ToString() == "2") && (ConfigurationManager.AppSettings["DIAN_ON"] == "1"))
            {
                if (HttpContext.Current.Session["FechaVencimientoToken"] != null)
                {
                    if (Convert.ToDateTime(HttpContext.Current.Session["FechaVencimientoToken"]) < DateTime.Now)
                    {
                        GeneraToken();
                    }
                }
                else
                {
                    var U_Token = inicio.UltimoToken();
                    if (U_Token.ACCESS_TOKEN != null)
                    {
                        HttpContext.Current.Session["FechaVencimientoToken"] = U_Token.FECHA_VENCIMIENTO;
                        HttpContext.Current.Session["Token"] = U_Token.ACCESS_TOKEN;
                    }
                }                
            }

            base.OnActionExecuting(filterContext);
        }

        public void GeneraToken()
        {
            TBL_TOKENS_DIAN model = new TBL_TOKENS_DIAN();
            var tokenD = businessDian.GeneraToken();
            if (tokenD.access_token != null)
            {
                model.FECHA_TOKEN = DateTime.Now;
                model.FECHA_VENCIMIENTO = Convert.ToDateTime(model.FECHA_TOKEN).AddMilliseconds(tokenD.expires_in);
                model.ACCESS_TOKEN = tokenD.access_token;
                model.EXPIRES_IN = tokenD.expires_in;
                model.TOKEN_TYPE = tokenD.token_type;
                model.SCOPE = tokenD.scope;
                var r = inicio.InsertaToken(model);
                if (r)
                {
                    HttpContext.Current.Session["FechaVencimientoToken"] = model.FECHA_VENCIMIENTO;
                    HttpContext.Current.Session["Token"] = model.ACCESS_TOKEN;
                }
            }
        }
    }
}