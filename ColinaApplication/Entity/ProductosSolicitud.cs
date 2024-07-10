using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ProductosSolicitud
    {
        public decimal Id { get; set; }
        public System.DateTime? FechaRegistro { get; set; }
        public decimal? IdSolicitud { get; set; }
        public decimal? IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal? IdMesero { get; set; }
        public string NombreMesero { get; set; }
        public decimal? PrecioProducto { get; set; }
        public string EstadoProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal? UpDian { get; set; }
        public string IdDian { get; set; }
        public decimal? AccountGroup{ get; set; }

    }
}
