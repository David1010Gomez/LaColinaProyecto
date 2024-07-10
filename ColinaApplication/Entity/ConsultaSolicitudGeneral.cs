using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Entity
{
    public class ConsultaSolicitudGeneral
    {
        public decimal Id { get; set; }
        public System.DateTime? FechaSolicitud { get; set; }
        public decimal? IdMesa { get; set; }
        public decimal? NumeroMesa { get; set; }
        public string NombreMesa { get; set; }
        public decimal? IdMesero { get; set; }
        public string NombreMesero { get; set; }
        public string IdentificacionCliente { get; set; }
        public string NombreCliente { get; set; }
        public string EstadoSolicitud { get; set; }
        public string Observaciones { get; set; }
        public decimal? OtrosCobros { get; set; }
        public decimal? Descuentos { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? PorcentajeIVA { get; set; }
        public decimal? IVATotal { get; set; }
        public decimal? PorcentajeIConsumo { get; set; }
        public decimal? IConsumoTotal { get; set; }
        public decimal? PorcentajeServicio { get; set; }
        public decimal? ServicioTotal { get; set; }
        public decimal? Total { get; set; }
        public List<ProductosSolicitud> ProductosSolicitud { get; set; }
        public List<Impuestos> Impuestos { get; set; }
        public  decimal? IdCliente { get; set; }
        public string FactracionElectronica { get; set; }
        public string EnvioDian { get; set; }
        public ClienteDian cliente { get; set; }
        public string ValoresVouchers { get; set; }
        public string IdFDian { get; set; }

    }
}
