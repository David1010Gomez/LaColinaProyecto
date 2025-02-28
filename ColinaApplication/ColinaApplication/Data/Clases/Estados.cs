﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColinaApplication.Data.Clases
{
    public static class Estados
    {
        //ESTADO CATEGORIAS E IMPUESTOS
        private static string activo = "ACTIVO";
        private static string inactivo = "INACTIVO";

        public static string Activo { get { return activo; } }
        public static string Inactivo { get { return inactivo; } }

        //ESTADO SOLICITUD
        private static string abierta = "ABIERTA";
        private static string finalizada = "FINALIZADA";
        private static string llevar = "LLEVAR";
        private static string cancelaPedido = "CANCELA PEDIDO";
        private static string consumoInterno = "CONSUMO INTERNO";
        private static string inhabilitar = "INHABILITAR";

        public static string Abierta { get { return abierta; } }
        public static string Finalizada { get { return finalizada; } }
        public static string Llevar { get { return llevar; } }
        public static string CancelaPedido { get { return cancelaPedido; } }
        public static string ConsumoInterno { get { return consumoInterno; } }
        public static string Inhabilitar { get { return inhabilitar; } }

        //ESTADO PRODUCTOS SOLICITUD
        private static string entregado = "ENTREGADO";
        private static string noEntregado = "NO ENTREGADO";
        private static string cancelado = "CANCELADO";

        public static string Entregado { get { return entregado; } }
        public static string NoEntregado { get { return noEntregado; } }
        public static string Cancelado { get { return cancelado; } }

        //ESTADO MESAS
        private static string libre = "LIBRE";
        private static string ocupado = "OCUPADO";
        private static string espera = "ESPERA";
        private static string noDisponible = "NO DISPONIBLE";
        private static string cerrado = "CERRADO";


        public static string Libre { get { return libre; } }
        public static string Ocupado { get { return ocupado; } }
        public static string Espera { get { return espera; } }
        public static string NoDisponible { get { return noDisponible; } }
        public static string Cerrado { get { return cerrado; } }




    }
}