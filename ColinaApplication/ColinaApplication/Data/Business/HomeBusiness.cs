using ColinaApplication.Data.Clases;
using ColinaApplication.Data.Conexion;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColinaApplication.Data.Business
{
    public class HomeBusiness
    {
        public TBL_USUARIOS Login (string Codigo)
        {
            TBL_USUARIOS user = new TBL_USUARIOS();
            using (DBLaColina context = new DBLaColina())
            {
                user = context.TBL_USUARIOS.FirstOrDefault(a=>a.CONTRASEÑA == Codigo);
                if (user != null)
                {
                    
                }
                else
                {
                    user = new TBL_USUARIOS();
                    user.ID = -1;
                }
                context.TBL_MASTER_MESAS.ToList();
            }
            return user;
        }

        public TBL_TOKENS_DIAN UltimoToken()
        {
            TBL_TOKENS_DIAN token = new TBL_TOKENS_DIAN();
            using (DBLaColina context = new DBLaColina())
            {
                token = context.TBL_TOKENS_DIAN.ToList().LastOrDefault();
            }
            return token;
        }

        public bool InsertaToken(TBL_TOKENS_DIAN model)
        {
            bool respuesta = false;
            using (DBLaColina contex = new DBLaColina())
            {
                try
                {                    
                    contex.TBL_TOKENS_DIAN.Add(model);
                    contex.SaveChanges();
                    respuesta = true;
                }
                catch (Exception ex)
                {

                }
            }
            return respuesta;
        }
    }
}