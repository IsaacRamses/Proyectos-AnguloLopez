using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Financial.Api.Utils
{
    public static class Services
    {
        static Services()
        {
            try
            {
                financiero = new InformacionFinanciera.BV_CCAL_WS();
                catalogos = new TablasMaestras.CONSULTAS_TABLAS_MAESTRAS_WS();
                clientes = new angulolopez.CCALWebService();
                miscelaneos = new otrasFacturas.OTRAS_FACTURAS_CCAL_WS();
                operaciones = new remesas.REMESAS_CCAL_WS();
                utilitarios = new utilidades.UTILIDADES_CCAL_WS();
                users = new usuarios.USUARIOS_CCAL_WS();
                ordenes = new orders.OPERACIONES_CCAL_WS();
                monetarios = new ingresos.INGRESOS_CCAL_WS();
                pagos = new infoPagos.PAGOS_CCAL_WS();
                Egresos = new Egresos.EGRESOS_CCAL_WS();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static InformacionFinanciera.BV_CCAL_WS financiero;
        public static TablasMaestras.CONSULTAS_TABLAS_MAESTRAS_WS catalogos;
        public static otrasFacturas.OTRAS_FACTURAS_CCAL_WS miscelaneos;
        public static angulolopez.CCALWebService clientes;
        public static remesas.REMESAS_CCAL_WS operaciones;
        public static utilidades.UTILIDADES_CCAL_WS utilitarios;
        public static usuarios.USUARIOS_CCAL_WS users;
        public static orders.OPERACIONES_CCAL_WS ordenes;
        public static ingresos.INGRESOS_CCAL_WS monetarios;
        public static infoPagos.PAGOS_CCAL_WS pagos;
        public static Egresos.EGRESOS_CCAL_WS Egresos;

    }
}