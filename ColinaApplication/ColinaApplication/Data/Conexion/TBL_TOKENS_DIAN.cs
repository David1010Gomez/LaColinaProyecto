//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ColinaApplication.Data.Conexion
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_TOKENS_DIAN
    {
        public decimal ID { get; set; }
        public Nullable<System.DateTime> FECHA_TOKEN { get; set; }
        public Nullable<System.DateTime> FECHA_VENCIMIENTO { get; set; }
        public string ACCESS_TOKEN { get; set; }
        public Nullable<int> EXPIRES_IN { get; set; }
        public string TOKEN_TYPE { get; set; }
        public string SCOPE { get; set; }
    }
}
