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
    
    public partial class TBL_SOLICITUD
    {
        public decimal ID { get; set; }
        public Nullable<System.DateTime> FECHA_SOLICITUD { get; set; }
        public Nullable<decimal> ID_MESA { get; set; }
        public Nullable<decimal> ID_MESERO { get; set; }
        public string IDENTIFICACION_CLIENTE { get; set; }
        public string NOMBRE_CLIENTE { get; set; }
        public string ESTADO_SOLICITUD { get; set; }
        public string OBSERVACIONES { get; set; }
        public Nullable<decimal> OTROS_COBROS { get; set; }
        public Nullable<decimal> DESCUENTOS { get; set; }
        public Nullable<decimal> TOTAL { get; set; }
    }
}
