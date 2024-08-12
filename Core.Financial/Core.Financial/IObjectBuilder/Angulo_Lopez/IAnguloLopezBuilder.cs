using Common.Models.Angulo_Lopez;
using Common.Models.Angulo_Lopez.Ingresos;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.Pagos;
using Common.Models.Common;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models.Angulo_Lopez.Monedas;
using Nomeda = Common.Models.Angulo_Lopez.Monedas.Moneda;
using Common.Models.Angulo_Lopez.Ciudades;
using Ciudad = Common.Models.Angulo_Lopez.Ciudades.Ciudad;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Moneda = Common.Models.Common.Moneda;
using Common.Models.Angulo_Lopez.Remitee;

namespace IObjectBuilder.Angulo_Lopez
{
    public interface IAnguloLopezBuilder : IFinalize
    {
        #region Schema Operaciones
        #region Solicitudes
        HashSet<Solicitudes> SearchSolicitudes(SolicitudesRequest param);
        GenericResponse InsertSolicitudes(Solicitudes param);
        GenericResponse UpdateSolicitudes(Solicitudes param);
        GenericResponse UpdateStatusSolicitudes(Solicitudes param);
        GenericResponse UpdateMontoAprobadoSolicitudes(Solicitudes param);

        GenericResponse UpdateOrdenReferenciaBCV(Solicitudes param);
        #endregion
        #region Solicitudes Backoffice
        HashSet<SolicitudesBackoffice> SearchSolicitudesBackoffice(SolicitudesBackofficeRequest param);
        GenericResponse UpdateSolicitudBackofficeEnProceso(SolicitudUpdateEnProceso param);
        GenericResponse UpdateSolicitudBackofficeAprobada(SolicitudUpdateAprobada param);
        GenericResponse UpdateSolicitudBackofficeRechazada(SolicitudUpdateRechazada param);
        GenericResponse UpdateSolicitudBackofficeConciliada(SolicitudUpdateConciliada param);
        GenericResponse ValidateRequestAmount(ValidateRequestAmount param);
        #endregion

        #region Tarifas_Aplicadas
        GenericResponse InsertTarifasAplicadas(Tarifas_Aplicadas param);
        HashSet<ListadoTarifasAplicadasBackoffice> SearchTarifasAplicadasRemesa(int id);
        HashSet<ListadoTarifasAplicadasBackoffice> SearchTarifasAplicadasSolicitud(int id);
        #endregion

        #region Remesas Salientes
        HashSet<OrdenSalienteExterno> SearchRemesasSalientes(string corresponsal);
        UpdateOrdenesSalientesTransmitidasResponse UpdateOrdenesSalientesTransmitidas(UpdateOrdenesSalientesTransmitidasRequest req);
        #endregion
        #region Remesas Entrantes
        HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantes(OrdenEntranteRequest param);
        GenericResponse UpdateRemesaEntrante(REMESAS_ENTRANTES param);
        GenericResponse UpdateRechazarRemesaEntrante(REMESAS_ENTRANTES param);
        GenericResponse UpdateAprobarRemesaEntrante(REMESAS_ENTRANTES param);
        GenericResponse InsertarRemesaEntrante(REMESAS_ENTRANTES param);

        GenericResponse AnulaRemesaEntranteCorresponsal(REMESAS_ENTRANTES param);

        GenericResponse UpdateStatusRemesasEntrantes(REMESAS_ENTRANTES param);
        #endregion
        #endregion

        #region Schame Pagos
        #region ConversionCurrency
        HashSet<Nomeda> SearchConversionCurrency(MonedaRequest param);
        #endregion
        #endregion

        #region Schema Ingresos
        #region Pagos_Recibidos_Cliente
        GenericResponse InsertPagos_Recibidos_Cliente(Pagos_Recibidos_Cliente param);
        GenericResponse RollBackInsertPagos_Recibidos_Cliente(RollBackPayment rollBack);
        HashSet<PagoRecibidosBackoffice> SearchPagosRecibidosSolicitud(int id);
        #endregion

        #region Detalle_Pago_Recibido
        GenericResponse InsertDetalle_Pago_Recibido(Detalle_Pago_Recibido param);
        #endregion

        #region Ordenes_Relacionadas_Pagos
        GenericResponse InsertOrdenes_Relacionadas_Pagos(Ordenes_Relacionadas_Pagos param);
        #endregion
        #endregion

        #region Schema Ciudades
        #region Ciudades
        HashSet<Ciudad> SearchCity(CiudadRequest param);
        HashSet<Ciudad> SearchCityRemitee(CiudadRequest param);
        #endregion
        #endregion

        HashSet<MotivosAnulacionCorresponsal> SearchMotivosAnulacion(MotivosAnulacionCorresponsal param);

        #region Schema Monedas
        #region Monedas
        HashSet<Moneda> SearchMoneda();
        #endregion
        #endregion
        #region Schema Corresponsal
        HashSet<CorresponsalCredenciales> SearchCorresponsal(CorresponsalRequest param);
        #endregion
        HashSet<MonedasCorresponsal> SearchMonedasPagador(string pagador);
        HashSet<RemiteeIntegracion> RemiteeDatosIntegracion(RemiteeIntegracion request);
        HashSet<RemiteeIntegracion> UpdateRemiteeDatosIntegracion(RemiteeIntegracion request);
        HashSet<RemiteeIntegracion> SearchRemiteeDatosIntegracion(RemiteeIntegracion request);
    }
}
