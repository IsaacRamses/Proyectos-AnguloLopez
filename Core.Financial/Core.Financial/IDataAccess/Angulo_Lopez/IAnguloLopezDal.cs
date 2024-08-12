using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDataAccess.Angulo_Lopez
{
    public interface IAnguloLopezDal<TEntity> : IFinalize
    {
        #region Schema Operaciones
        #region Solicitudes
        HashSet<TEntity> SearchSolicitudes(object[] param);
        TEntity InsertSolicitudes(object[] param);
        TEntity UpdateSolicitudes(object[] param);
        TEntity UpdateStatusSolicitudes(object[] param);
        TEntity UpdateMontoAprobadoSolicitudes(object[] param);
        TEntity ValidateRequestAmount(object[] param);
        TEntity UpdateOrdenReferenciaBCV(object[] param); 
        #endregion

        #region Solicitudes en Backoffice
        HashSet<TEntity> SearchSolicitudesBackoffice(object[] param);
        TEntity UpdateSolicitudBackofficeEnProceso(object[] param);
        TEntity UpdateSolicitudBackofficeAprobada(object[] param);
        TEntity UpdateSolicitudBackofficeRechazada(object[] param);
        TEntity UpdateSolicitudBackofficeConciliada(object[] param);
        #endregion

        #region Tarifas_Aplicadas
        TEntity InsertTarifasAplicadas(object[] param);

        HashSet<TEntity> SearchTarifasAplicadasSolicitud(object[] param);
        HashSet<TEntity> SearchTarifasAplicadasRemesa(object[] param);
        #endregion

        #region Remesas Salientes
        HashSet<TEntity> SearchRemesasSalientes(object[] param);
        TEntity UpdateOrdenesSalientesTransmitidas(object[] param);
        #endregion
        #region Remesas Entrantes
        HashSet<TEntity> SearchRemesasEntrantes(object[] param);
        TEntity UpdateRemesaEntrante(object[] param);
        TEntity UpdateRechazarRemesaEntrante(object[] param);
        TEntity UpdateAprobarRemesaEntrante(object[] param);
        TEntity InsertarRemesaEntrante(object[] param);
        TEntity AnulaRemesaEntranteCorresponsal(object[] param);
        TEntity UpdateStatusRemesasEntrantes(object[] param);
        #endregion

        #endregion

        #region Schema Pagos
        #region ConversionCurrency
        HashSet<TEntity> SearchConversionCurrency(object[] param);
        #endregion
        #endregion

        #region Schema Ingresos
        #region Pagos_Recibidos_Cliente
        TEntity InsertPagos_Recibidos_Cliente(object[] param);
        TEntity RollBackInsertPagos_Recibidos_Cliente(object[] param);
        #endregion

        #region Detalle_Pago_Recibido
        TEntity InsertDetalle_Pago_Recibido(object[] param);
        #endregion

        #region Ordenes_Relacionadas_Pagos
        TEntity InsertOrdenes_Relacionadas_Pagos(object[] param);
        #endregion

        HashSet<TEntity> SearchPagosRecibidosSolicitud(object[] param);
        #endregion

        #region Schema Ciudades
        #region Ciudades
        HashSet<TEntity> SearchCity(object[] param);
        HashSet<TEntity> SearchCityRemitee(object[] param);
        #endregion
        #endregion

        #region Schema Monedas
        #region Monedas
        HashSet<TEntity> SearchMoneda();
        #endregion
        #endregion

        HashSet<TEntity> SearchMotivosAnulacion(object[] param);
        #region Schema Corresponsal
        HashSet<TEntity> SearchCorresponsal(object[] param);
        #endregion

        HashSet<TEntity> SearchMonedasCorresponsal(object[] param);

        HashSet<TEntity> SearchMonedasPagador(object[] param);
        HashSet<TEntity> RemiteeDatosIntegracion(object[] param);
        HashSet<TEntity> UpdateRemiteeDatosIntegracion(object[] param);
        HashSet<TEntity> SearchRemiteeDatosIntegracion(object[] param);
    }
}
