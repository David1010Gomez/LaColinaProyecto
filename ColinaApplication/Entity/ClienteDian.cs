using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ClienteDian
    {
        public decimal Id { get; set; }
        public string TipoPersona { get; set; }
        public string CodigoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string Direccion { get; set; }
        public string CodCiudad { get; set; }
        public string NomCiudad { get; set; }
        public string Email { get; set; }
        public bool? ResponsableIva{ get; set; }
        public string CodigoRFiscal { get; set; }
        public string NomRFiscal { get; set; }
        public string IdCodigoDian { get; set; }
        public string Telefono { get; set; }
        public string DigitoVerif { get; set; }
    }
}
