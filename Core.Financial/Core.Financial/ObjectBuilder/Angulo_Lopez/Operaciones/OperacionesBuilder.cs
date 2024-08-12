using Common.Models.Angulo_Lopez;
using Common.Models.Angulo_Lopez.Contabilidad;
using Common.Models.Angulo_Lopez.Egresos;
using Common.Models.Angulo_Lopez.Ingresos;
using Common.Models.Angulo_Lopez.Numeracion;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.Operaciones.Services;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Common.Models.Angulo_Lopez.Pagos;
using Common.Models.Angulo_Lopez.Simadi;
using Common.Models.Angulo_Lopez.Sudeban;
using Common.Models.Angulo_Lopez.Seguridad;
using Common.Models.Angulo_Lopez.TablasMaestras;
using Common.Models.Angulo_Lopez.Tasas;
using Common.Models.Clientes;
using Common.Models.Common;
using Common.Resource;
using DataAccess.Angulo_Lopez.Simadi;
using Common.Util;
using DataAccess.Operaciones;
using IDataAccess.Angulo_Lopez.Operaciones;
using IDataAccess.Angulo_Lopez.Simadi;
using IObjectBuilder.Angulo_Lopez;
using IObjectBuilder.Angulo_Lopez.Contabilidad;
using IObjectBuilder.Angulo_Lopez.Egreso;
using IObjectBuilder.Angulo_Lopez.Ingresos;
using IObjectBuilder.Angulo_Lopez.Numeracion;
using IObjectBuilder.Angulo_Lopez.Pagos;
using IObjectBuilder.Angulo_Lopez.Simadi;
using IObjectBuilder.Angulo_Lopez.Sudeban;
using IObjectBuilder.Angulo_Lopez.TablasMaestras;
using IObjectBuilder.Angulo_Lopez.Tarifas;
using IObjectBuilder.Angulo_Lopez.Tasas;
using IObjectBuilder.Clients;
using IObjectBuilder.Operaciones;
using IObjectBuilder.Services;
using Newtonsoft.Json;
using ObjectBuilder.Angulo_Lopez.Contabilidad;
using ObjectBuilder.Angulo_Lopez.Egreso;
using ObjectBuilder.Angulo_Lopez.Ingresos;
using ObjectBuilder.Angulo_Lopez.Numeracion;
using ObjectBuilder.Angulo_Lopez.Operaciones.ValidationsOperaciones.OnlinePayment;
using ObjectBuilder.Angulo_Lopez.Pagos;
using ObjectBuilder.Angulo_Lopez.Simadi;
using ObjectBuilder.Angulo_Lopez.Sudeban;
using ObjectBuilder.Angulo_Lopez.TablasMaestras;
using ObjectBuilder.Angulo_Lopez.Tarifas;
using ObjectBuilder.Angulo_Lopez.Tasas;
using ObjectBuilder.Clients;
using ObjectBuilder.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Utilities;
using Cashier = Common.Models.Angulo_Lopez.Contabilidad.Cashier;
using Cashiers = Common.Models.Angulo_Lopez.Contabilidad.Cashier;
using CashierSummary = Common.Models.Angulo_Lopez.Contabilidad.CashierSummary;
using CashierSummarys = Common.Models.Angulo_Lopez.Contabilidad.CashierSummary;
using OrderPaymentPendings = Common.Models.Angulo_Lopez.Pagos.OrderPaymentPending;
using REMESAS_ENTRANTESs = Common.Models.Angulo_Lopez.OrdenesEntrantes.REMESAS_ENTRANTES;
using Common.Models.Angulo_Lopez.Tarifas;
using Tarifas_Comiciones = Common.Models.Angulo_Lopez.Operaciones.Tarifa;
using MonedasRequests = Common.Models.Angulo_Lopez.TablasMaestras.MonedasRequest;
using Newtonsoft.Json;
using static Common.Resource.Constant;
using IObjectBuilder.Angulo_Lopez.Oficinas;
using ObjectBuilder.Angulo_Lopez.Oficinas;
using Common.Models.Angulo_Lopez.Oficinas;
using Constants = Common.Resource.Constant;
using ObjectBuilder.Angulo_Lopez.Seguridad;
using IObjectBuilder.Angulo_Lopez.Seguridad;

namespace ObjectBuilder.Angulo_Lopez.Operaciones
{
    public class OperacionesBuilder : Finalize, IOperacionesBuilder
    {
        #region Variables
        private readonly IIngresosBuilder builderIngresos = new IngresosBuilder();
        private readonly IClientsBuilder builderClients = new ClientsBuilder();
        private readonly INumeracionBuilder builderNumeracion = new NumeracionBuilder();
        private readonly ITablasMaestrasBuilder builderTablasMaestras = new TablasMaestrasBuilder();
        private readonly IContabilidadBuilder builderContabilidad = new ContabilidadBuilder();
        private readonly IPagosBuilder builderPagos = new PagosBuilder();
        private readonly ISudebanBuilder builderSudeban = new SudebanBuilder();
        private readonly IEgresoBuilder builderEgreso = new EgresoBuilder();
        private readonly ISimadiBuilder builderSimadi = new SimadiBuilder();
        private readonly ITasasBuilder builderTasas = new TasasBuilder();
        private readonly ITarifasBuilder builderTarifas = new TarifasBuilder();
        private readonly ISeguridadBuilder builderSeguridad = new SeguridadBuilder();

        public static CultureInfo wsCulture { get => new CultureInfo("en-US"); }

        #endregion

        #region Ordenes

        #region SearchOrderPayment

        public Ordenes SearchOrderPayment(OrderPaymentRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@TypeDocument", SqlDbType.Char, 1){Value = model.TypeDocument??(object)DBNull.Value},
                    new SqlParameter("@ClientDocument", SqlDbType.VarChar, 25){Value = model.ClientDocument??(object)DBNull.Value},
                    new SqlParameter("@Amount", SqlDbType.Decimal){Value = model.Amount??(object)DBNull.Value},
                    new SqlParameter("@OrderCode", SqlDbType.VarChar, 25){Value = model.OrderCode??(object)DBNull.Value}
                };
                return dal.SearchOrderPayment(param);
            }

        }

        #endregion

        #region UpdateStatusOrdenes

        public GenericResponse UpdateStatusOrdenes(Ordenes model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value = model.STATUS_ORDEN},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor},
                    new SqlParameter("@REFERENCIA_PAGO", SqlDbType.VarChar, 50){Value = model.REFERENCIA_PAGO??(object)DBNull.Value},
                    new SqlParameter("@ObservacionesRechazo", SqlDbType.VarChar, 500){Value = model.ObservacionesRechazo??(object)DBNull.Value},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = model.SucursalProcesaId??(object)DBNull.Value},
                };
                return dal.UpdateStatusOrdenes(param);
            }
        }

        #endregion

        #region UpdateStatusOrder

        public GenericResponse UpdateStatusOrder(Ordenes model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value = model.STATUS_ORDEN},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor},
                    new SqlParameter("@REFERENCIA_PAGO", SqlDbType.VarChar, 50){Value = model.REFERENCIA_PAGO??(object)DBNull.Value},
                    new SqlParameter("@ObservacionesRechazo", SqlDbType.VarChar, 500){Value = model.ObservacionesRechazo??(object)DBNull.Value},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = model.SucursalProcesaId??(object)DBNull.Value},
                    new SqlParameter("@OBSERVACIONES", SqlDbType.VarChar, 500){Value = model.OBSERVACIONES??(object)DBNull.Value},
                };
                return dal.UpdateStatusOrder(param);
            }
        }

        #endregion

        #region SearchOrdenesByFilter

        public HashSet<Ordenes> SearchOrdenesByFilter(OrderHistoryRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value = model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@FECHA_OPERACION_INICIAL", SqlDbType.SmallDateTime){Value = model.FECHA_OPERACION_INICIAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_OPERACION_FINAL", SqlDbType.SmallDateTime){Value = model.FECHA_OPERACION_FINAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_PAGO_INICIAL", SqlDbType.SmallDateTime){Value = model.FECHA_PAGO_INICIAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_PAGO_FINAL", SqlDbType.SmallDateTime){Value = model.FECHA_PAGO_FINAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION_INICIAL", SqlDbType.SmallDateTime){Value = model.FECHA_ANULACION_INICIAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION_FINAL", SqlDbType.SmallDateTime){Value = model.FECHA_ANULACION_FINAL??(object)DBNull.Value},
                    new SqlParameter("@AnuladaPor", SqlDbType.VarChar, 15){Value = model.AnuladaPor??(object)DBNull.Value},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = model.SucursalProcesaId??(object)DBNull.Value},
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@ProcessedBy", SqlDbType.VarChar, 15){Value = model.ProcessedBy??(object)DBNull.Value},
                };
                return dal.SearchOrdenesByFilter(param);
            }
        }

        #endregion

        #region SearchReturnFundsOrder

        public HashSet<Ordenes> SearchReturnFundsOrder(OrdenesRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value = model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.SmallDateTime){Value = model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.SmallDateTime){Value = model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.VarChar, 15){Value = model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar, 15){Value = model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                    new SqlParameter("@Moneda", SqlDbType.VarChar, 15){Value = model.Moneda??(object)DBNull.Value},
                };
                return dal.SearchReturnFundsOrder(param);
            }
        }

        #endregion

        #region SearchORDENES

        public HashSet<Ordenes> SearchORDENES(OrdenesRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value= model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_BENEFICIARIO", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@ID_ORDEN_FK", SqlDbType.VarChar,15){Value= model.ID_ORDEN_FK??(object)DBNull.Value},
                };
                return dal.SearchORDENES(param);
            }
        }
        #endregion

        #region SearchOperations

        public HashSet<Operations> SearchOperations(OperationsRequest model)
        {
            using (IOperacionesDal<Operations> dal = new OperacionesDal<Operations>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value= model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_BENEFICIARIO", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@TipoOperaciones", SqlDbType.VarChar,500){Value= model.TipoOperaciones??(object)DBNull.Value},
                    new SqlParameter("@SECUENCIA", SqlDbType.BigInt){Value= model.SECUENCIA??(object)DBNull.Value},
                    new SqlParameter("@REFERENCIA", SqlDbType.VarChar,100){Value= model.REFERENCIA??(object)DBNull.Value},
                };
                return dal.SearchOperations(param);
            }
        }
        #endregion

        #region SearchTurnAlert

        public HashSet<Ordenes> SearchTurnAlert(OrdenesRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value= model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.Char,3){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                };
                return dal.SearchTurnAlert(param);
            }
        }
        #endregion

        #region SearchPaymentOrdersNotCanceled

        public HashSet<SolicitudesBackoffice> SearchPaymentOrdersNotCanceled(OrdenesRequest model)
        {
            using (IOperacionesDal<SolicitudesBackoffice> dal = new OperacionesDal<SolicitudesBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                };
                return dal.SearchPaymentOrdersNotCanceled(param);
            }
        }
        #endregion

        #region SearchPaymentOrdersDivisaTaquilla

        public HashSet<SolicitudesBackoffice> SearchPaymentOrdersDivisaTaquilla(OrdenesRequest model)
        {
            using (IOperacionesDal<SolicitudesBackoffice> dal = new OperacionesDal<SolicitudesBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                };
                return dal.SearchPaymentOrdersDivisaTaquilla(param);
            }
        }
        #endregion

        #region SearchPaymentOrderVentanilla

        public HashSet<SolicitudesBackoffice> SearchPaymentOrderVentanilla(OrdenesRequest model)
        {
            using (IOperacionesDal<SolicitudesBackoffice> dal = new OperacionesDal<SolicitudesBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                };
                return dal.SearchPaymentOrderVentanilla(param);
            }
        }
        #endregion

        #region SearchPaymentOrdersTrasnfer

        public HashSet<SolicitudesBackoffice> SearchPaymentOrdersTrasnfer(OrdenesRequest model)
        {
            using (IOperacionesDal<SolicitudesBackoffice> dal = new OperacionesDal<SolicitudesBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                };
                return dal.SearchPaymentOrdersTrasnfer(param);
            }
        }
        #endregion

        #region SearchOrderPendingCorrespondet

        public HashSet<Ordenes> SearchOrderPendingCorrespondet(OrderHistoryRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value = model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@FECHA_OPERACION_INICIAL", SqlDbType.SmallDateTime){Value = model.FECHA_OPERACION_INICIAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_OPERACION_FINAL", SqlDbType.SmallDateTime){Value = model.FECHA_OPERACION_FINAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_PAGO_INICIAL", SqlDbType.SmallDateTime){Value = model.FECHA_PAGO_INICIAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_PAGO_FINAL", SqlDbType.SmallDateTime){Value = model.FECHA_PAGO_FINAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION_INICIAL", SqlDbType.SmallDateTime){Value = model.FECHA_ANULACION_INICIAL??(object)DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION_FINAL", SqlDbType.SmallDateTime){Value = model.FECHA_ANULACION_FINAL??(object)DBNull.Value},
                    new SqlParameter("@AnuladaPor", SqlDbType.VarChar, 15){Value = model.AnuladaPor??(object)DBNull.Value},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = model.SucursalProcesaId??(object)DBNull.Value},
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@ProcessedBy", SqlDbType.VarChar, 15){Value = model.ProcessedBy??(object)DBNull.Value},
                    new SqlParameter("@StatusCall", SqlDbType.Int){Value = model.StatusCall??(object)DBNull.Value},
                    new SqlParameter("@statusIDsolicitudes", SqlDbType.VarChar, -1){Value = model.statusIDsolicitudes?? (object)DBNull.Value},
                    new SqlParameter("@statusIDordenes", SqlDbType.VarChar, -1){Value = model.statusIDordenes?? (object)DBNull.Value},
                };
                return dal.SearchOrderPendingCorrespondet(param);
            }
        }

        #endregion

        #region SearchPaidOrders

        public HashSet<Ordenes> SearchPaidOrders(PaidOrdersRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DateStat", SqlDbType.SmallDateTime){Value = model.DateStat??(object)DBNull.Value},
                    new SqlParameter("@DateEnd", SqlDbType.SmallDateTime){Value = model.DateEnd??(object)DBNull.Value},
                    new SqlParameter("@BranchId", SqlDbType.Int){Value = model.BranchId??(object)DBNull.Value},
                    new SqlParameter("@OrderId", SqlDbType.Int){Value = model.OrderId??(object)DBNull.Value},
                };
                return dal.SearchPaidOrders(param);
            }
        }

        #endregion

        #region InsertOrdenes

        public GenericResponse InsertOrdenes(Ordenes model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AGENTE", SqlDbType.Char, 3){Value = (object)model.AGENTE??DBNull.Value},
                    new SqlParameter("@CLIENTE", SqlDbType.Int){Value = model.CLIENTE},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char, 3){Value = (object)model.CORRESPONSAL??DBNull.Value},
                    new SqlParameter("@DETALLE_TIPO_OPERACION", SqlDbType.Int){Value = model.DETALLE_TIPO_OPERACION},
                    new SqlParameter("@FECHA_OPERACION", SqlDbType.SmallDateTime){Value = model.FECHA_OPERACION},
                    new SqlParameter("@FECHA_VALOR_TASA", SqlDbType.SmallDateTime){Value = (object)model.FECHA_VALOR_TASA??DBNull.Value},
                    new SqlParameter("@MONEDA", SqlDbType.Int){Value = model.MONEDA},
                    new SqlParameter("@MONTO", SqlDbType.Money){Value = model.MONTO},
                    new SqlParameter("@MONTO_CAMBIO", SqlDbType.Money){Value = (object)model.MONTO_CAMBIO??DBNull.Value},
                    new SqlParameter("@OFICINA", SqlDbType.Int){Value = model.OFICINA??(object)DBNull.Value},
                    new SqlParameter("@PAIS_DESTINO", SqlDbType.Char, 2){Value = model.PAIS_DESTINO??(object)DBNull.Value},
                    new SqlParameter("@PERSONA", SqlDbType.Int){Value = model.PERSONA??(object)DBNull.Value},
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.VarChar, 15){Value = model.REGISTRADOPOR},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value = model.STATUS_ORDEN},
                    new SqlParameter("@SUCURSAL", SqlDbType.Int){Value = model.SUCURSAL},
                    new SqlParameter("@TASA_DESTINO", SqlDbType.Money){Value = model.TASA_DESTINO??(object)DBNull.Value},
                    new SqlParameter("@TIPO_CAMBIO", SqlDbType.Money){Value = model.TIPO_CAMBIO},
                    new SqlParameter("@NUMERO", SqlDbType.Int){Value = model.NUMERO},
                    new SqlParameter("@MOTIVO_OP_BCV", SqlDbType.Int){Value = model.MOTIVO_OP_BCV??(object)DBNull.Value},
                    new SqlParameter("@TIPO_OP_BCV", SqlDbType.VarChar, 10){Value = model.TIPO_OP_BCV??(object)DBNull.Value},
                    new SqlParameter("@NOMBRES_REMITENTE", SqlDbType.VarChar, 100){Value = model.NOMBRES_REMITENTE??(object)DBNull.Value},
                    new SqlParameter("@APELLIDOS_REMITENTE", SqlDbType.VarChar, 100){Value = model.APELLIDOS_REMITENTE??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar, 30){Value = model.IDENTIFICACION_REMITENTE??(object)DBNull.Value},
                    new SqlParameter("@NOMBRES_BENEFICIARIO", SqlDbType.VarChar, 100){Value = model.NOMBRES_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@APELLIDOS_BENEFICIARIO", SqlDbType.VarChar, 100){Value = model.APELLIDOS_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_BENEFICIARIO", SqlDbType.VarChar, 30){Value = model.IDENTIFICACION_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@SECUENCIA", SqlDbType.Int){Value = model.SECUENCIA??(object)DBNull.Value},
                    new SqlParameter("@FECHA_ENVIO", SqlDbType.SmallDateTime){Value = model.FECHA_ENVIO??(object)DBNull.Value},
                    new SqlParameter("@REFERENCIA_PAGO", SqlDbType.VarChar, 50){Value = model.REFERENCIA_PAGO??(object)DBNull.Value},
                    new SqlParameter("@FECHA_PAGO", SqlDbType.SmallDateTime){Value = model.FECHA_PAGO??(object)DBNull.Value},
                    new SqlParameter("@REFERENCIA_ORDEN", SqlDbType.VarChar, 50){Value = model.REFERENCIA_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@BANCO_NACIONAL", SqlDbType.VarChar, 3){Value = model.BANCO_NACIONAL??(object)DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA", SqlDbType.VarChar, 50){Value = model.NUMERO_CUENTA??(object)DBNull.Value},
                    new SqlParameter("@EMAIL_CLIENTE", SqlDbType.VarChar, 50){Value = model.EMAIL_CLIENTE??(object)DBNull.Value},
                    new SqlParameter("@EMAIL_BENEFICIARIO", SqlDbType.VarChar, 50){Value = model.EMAIL_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@BANCO_DESTINO", SqlDbType.VarChar, 50){Value = model.BANCO_DESTINO??(object)DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA_DESTINO", SqlDbType.VarChar, 50){Value = model.NUMERO_CUENTA_DESTINO??(object)DBNull.Value},
                    new SqlParameter("@DIRECCION_BANCO", SqlDbType.VarChar, 2000){Value = model.DIRECCION_BANCO??(object)DBNull.Value},
                    new SqlParameter("@ABA", SqlDbType.VarChar, 50){Value = model.ABA??(object)DBNull.Value},
                    new SqlParameter("@SWIFT", SqlDbType.VarChar, 50){Value = model.SWIFT??(object)DBNull.Value},
                    new SqlParameter("@IBAN", SqlDbType.VarChar, 50){Value = model.IBAN??(object)DBNull.Value},
                    new SqlParameter("@TELEFONO_BENEFICIARIO", SqlDbType.VarChar, 50){Value = model.TELEFONO_BENEFICIARIO??(object)DBNull.Value},
                    new SqlParameter("@TELEFONO_CLIENTE", SqlDbType.VarChar, 50){Value = model.TELEFONO_CLIENTE??(object)DBNull.Value},
                    new SqlParameter("@OBSERVACIONES", SqlDbType.VarChar, 2000){Value = model.OBSERVACIONES??(object)DBNull.Value},
                    new SqlParameter("@BANCO_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = model.BANCO_INTERMEDIARIO??(object)DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = model.NUMERO_CUENTA_INTERMEDIARIO??(object)DBNull.Value},
                    new SqlParameter("@DIRECCION_BANCO_INTERMEDIARIO", SqlDbType.VarChar, 2000){Value = model.DIRECCION_BANCO_INTERMEDIARIO??(object)DBNull.Value},
                    new SqlParameter("@ABA_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = model.ABA_INTERMEDIARIO??(object)DBNull.Value},
                    new SqlParameter("@SWIFT_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = model.SWIFT_INTERMEDIARIO??(object)DBNull.Value},
                    new SqlParameter("@IBAN_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = model.IBAN_INTERMEDIARIO??(object)DBNull.Value},
                    new SqlParameter("@USUARIO_TAQUILLA", SqlDbType.VarChar, 15){Value = model.USUARIO_TAQUILLA??(object)DBNull.Value},
                    new SqlParameter("@pMonedaOperacion", SqlDbType.Int){Value = model.MonedaOperacion??(object)DBNull.Value},
                    new SqlParameter("@pTasaConversion", SqlDbType.Money){Value = model.TasaConversion??(object)DBNull.Value},
                    new SqlParameter("@pMontoConversion", SqlDbType.Money){Value = model.MontoConversion??(object)DBNull.Value},
                    new SqlParameter("@ProcesarTransferencia", SqlDbType.Bit){Value = model.ProcesarTransferencia??(object)DBNull.Value},
                    new SqlParameter("@NumeroCuentaPagoTransferencia", SqlDbType.VarChar, 20){Value = model.NumeroCuentaPagoTransferencia??(object)DBNull.Value},
                    new SqlParameter("@BancoPagoTransferencia", SqlDbType.VarChar, 200){Value = model.BancoPagoTransferencia??(object)DBNull.Value},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = model.SucursalProcesaId??(object)DBNull.Value},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor??(object)DBNull.Value},
                    new SqlParameter("@Modificado", SqlDbType.SmallDateTime){Value = model.Modificado??(object)DBNull.Value},
                    new SqlParameter("@ID_ORDEN_FK", SqlDbType.Int){Value = model.ID_ORDEN_FK??(object)DBNull.Value},
                };
                return dal.InsertOrdenes(param);
            }
        }

        #endregion

        #region SearchOrdenesPayableCorrespondent

        public HashSet<Ordenes> SearchOrdenesPayableCorrespondent(OrdenesRequest model)
        {
            using (IOperacionesDal<Ordenes> dal = new OperacionesDal<Ordenes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value= model.STATUS_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char,3){Value= model.CORRESPONSAL??(object)DBNull.Value},
                };
                return dal.SearchOrdenesPayableCorrespondent(param);
            }
        }
        #endregion

        #region ValidateOperationClientPause

        public GenericResponse ValidateOperationClientPause(int clientId)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.ValidateOperationClientPause(clientId);
            }
        }
        #endregion

        #region RollBackOrden (DELETE)

        public GenericResponse RollBackOrden(int? ID_ORDEN,int ID_TEMPORAL, int? ID_PAGO, int NroFactura, string SUCURSAL)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = (object)ID_ORDEN??DBNull.Value},
                    new SqlParameter("@ID_OPERACION_TEMPORAL", SqlDbType.Int){Value = (object)ID_TEMPORAL??DBNull.Value},
                    new SqlParameter("@ID_PAGO", SqlDbType.Int){Value = (object)ID_PAGO??DBNull.Value},
                    new SqlParameter("@NRO_FACTURA", SqlDbType.Int){Value = (object)NroFactura??DBNull.Value},
                     new SqlParameter("@SUCURSAL", SqlDbType.Char,3){Value = SUCURSAL},
                };
                return dal.RollBackOrden(param);
            }
        }

        #endregion

        #region SearchPaymentOrderInternational

        public HashSet<SolicitudesBackoffice> SearchPaymentOrderInternational(OrdenesRequest model)
        {
            using (IOperacionesDal<SolicitudesBackoffice> dal = new OperacionesDal<SolicitudesBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value= model.STATUS_ORDEN??(object)DBNull.Value}
,
                };
                return dal.SearchPaymentOrderInternational(param);
            }
        }

        #endregion

        #endregion

        #region BatchBankOperations
        public HashSet<BatchBankOperations> SearchBatchBankOperations(BatchBankOperationsRequest model)
        {
            using (IOperacionesDal<BatchBankOperations> dal = new OperacionesDal<BatchBankOperations>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@OperationTypeId", SqlDbType.Int){Value = model.OperationTypeId??(object)DBNull.Value},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = model.CurrencyId??(object)DBNull.Value},
                    new SqlParameter("@BankSourceId", SqlDbType.Int){Value = model.BankSourceId??(object)DBNull.Value},
                    new SqlParameter("@BankDestinationId", SqlDbType.Int){Value = model.BankDestinationId??(object)DBNull.Value},
                    new SqlParameter("@BranchId", SqlDbType.Int){Value = model.BranchId??(object)DBNull.Value},
                    new SqlParameter("@BatchNumber", SqlDbType.Int){Value = model.BatchNumber??(object)DBNull.Value},
                    new SqlParameter("@DateStart", SqlDbType.SmallDateTime){Value = model.DateStart??(object)DBNull.Value},
                    new SqlParameter("@DateEnd", SqlDbType.SmallDateTime){Value = model.DateEnd??(object)DBNull.Value}
                };
                return dal.SearchBatchBankOperations(param);
            }
        }

        public GenericResponse InsertBatchBankOperations(BatchBankOperations model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@OperationTypeId", SqlDbType.Int){Value = model.OperationTypeId},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = model.CurrencyId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar, 15){Value = model.CreationUser},
                    new SqlParameter("@BankSourceId", SqlDbType.Int){Value = (object)model.BankSourceId??DBNull.Value},
                    new SqlParameter("@BankDestinationId", SqlDbType.Int){Value = (object)model.BankDestinationId??DBNull.Value},
                    new SqlParameter("@BranchId", SqlDbType.Int){Value = (object)model.BranchId??DBNull.Value},
                    new SqlParameter("@BatchCountOperations", SqlDbType.Int){Value = model.BatchCountOperations},
                    new SqlParameter("@BatchTotalAmmount", SqlDbType.Money){Value = model.@BatchTotalAmmount},
                    new SqlParameter("@BatchFile", SqlDbType.VarChar, 1000){Value = model.BatchFile},
                    new SqlParameter("@BatchObservation", SqlDbType.VarChar, 150){Value = model.BatchObservation??(object)DBNull.Value},
                };
                return dal.InsertBatchBankOperations(param);
            }
        }

        public GenericResponse InsertBatchBankOperationIntegrated(BatchBankOperations model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@OperationTypeId", SqlDbType.Int){Value = model.OperationTypeId},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = model.CurrencyId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar, 15){Value = model.CreationUser},
                    new SqlParameter("@BankSourceId", SqlDbType.Int){Value = (object)model.BankSourceId??DBNull.Value},
                    new SqlParameter("@BankDestinationId", SqlDbType.Int){Value = (object)model.BankDestinationId??DBNull.Value},
                    new SqlParameter("@BranchId", SqlDbType.Int){Value = (object)model.BranchId??DBNull.Value},
                    new SqlParameter("@Operations", SqlDbType.VarChar, 500){Value = model.Operations},
                    new SqlParameter("@BatchFile", SqlDbType.VarChar, 1000){Value = model.BatchFile},
                    new SqlParameter("@BatchObservation", SqlDbType.VarChar, 150){Value = model.BatchObservation??(object)DBNull.Value},
                    new SqlParameter("@BankAccountsId", SqlDbType.Int){Value = model.BankAccountsId??(object)DBNull.Value},
                };
                return dal.InsertBatchBankOperationIntegrated(param);
            }
        }

        public GenericResponse UpdateStatusBatchBankOperations(BatchBankOperations model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@BatchId", SqlDbType.Int){Value = model.BatchId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar, 15){Value = model.UpdateUser},
                };
                return dal.UpdateStatusBatchBankOperations(param);
            }
        }

        public GenericResponse SearchBatchBankOperationsNumber()
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.SearchBatchBankOperationsNumber();
            }
        }
        #endregion

        #region BatchBankDetail
        public HashSet<BatchBankDetail> SearchBatchBankDetail(BatchBankDetailRequest model)
        {
            using (IOperacionesDal<BatchBankDetail> dal = new OperacionesDal<BatchBankDetail>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@BatchId", SqlDbType.Int){Value = model.BatchId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@RequestId", SqlDbType.Int){Value = model.RequestId??(object)DBNull.Value},
                    new SqlParameter("@OrderId", SqlDbType.Int){Value = model.OrderId??(object)DBNull.Value},
                    new SqlParameter("@OrderIncomingId", SqlDbType.Int){Value = model.OrderIncomingId??(object)DBNull.Value},
                    new SqlParameter("@ReasonRejectedId", SqlDbType.Int){Value = model.ReasonRejectedId??(object)DBNull.Value},
                };
                return dal.SearchBatchBankDetail(param);
            }
        }

        public GenericResponse InsertBatchBankDetail(BatchBankDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@BatchId", SqlDbType.Int){Value = model.BatchId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar, 15){Value = model.CreationUser},
                    new SqlParameter("@RequestId", SqlDbType.Int){Value = (object)model.RequestId??DBNull.Value},
                    new SqlParameter("@OrderId", SqlDbType.Int){Value = (object)model.@OrderId??DBNull.Value},
                    new SqlParameter("@OrderIncomingId", SqlDbType.Int){Value = (object)model.OrderIncomingId??DBNull.Value},
                    new SqlParameter("@DetailObservacion", SqlDbType.VarChar, 500){Value = model.DetailObservacion??(object)DBNull.Value},
                };
                return dal.InsertBatchBankDetail(param);
            }
        }

        public GenericResponse UpdateStatusBatchBankDetail(BatchBankDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value = model.@DetailId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar, 15){Value = model.UpdateUser},
                    new SqlParameter("@ReasonRejectedId", SqlDbType.Int){Value = (object)model.ReasonRejectedId??DBNull.Value},
                    new SqlParameter("@DetailPaymentReference", SqlDbType.VarChar, 50){Value = (object)model.DetailPaymentReference??DBNull.Value},
                    new SqlParameter("@DetailOperationDate", SqlDbType.SmallDateTime){Value = (object)model.DetailOperationDate??DBNull.Value},
                };
                return dal.UpdateStatusBatchBankDetail(param);
            }
        }
        #endregion

        #region Remesas Enrantes

        #region SearchRemesaEntranteFromOrdenes

        public OrdenCompraEfectivo SearchRemesaEntranteFromOrdenes(int id)
        {
            using (IOperacionesDal<OrdenCompraEfectivo> dal = new OperacionesDal<OrdenCompraEfectivo>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Id", SqlDbType.Int){Value = id},
                };
                return dal.SearchRemesaEntranteFromOrdenes(param);
            }
        }

        #endregion

        #region RollbackRemesaPagadas

        public GenericResponse RollbackRemesaPagadas(int id, bool remesaEntrante)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Id", SqlDbType.Int){Value = id},
                    new SqlParameter("@RemesaEntrante", SqlDbType.Bit){Value = remesaEntrante},
                };
                return dal.RollbackRemesaPagadas(param);
            }
        }

        public GenericResponse RollbackMassRemesaPagadas(string id, bool remesaEntrante, string detailId)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@OrderId", SqlDbType.NVarChar,500){Value = id},
                    new SqlParameter("@RemesaEntrante", SqlDbType.Bit){Value = remesaEntrante},
                    new SqlParameter("@DetailId", SqlDbType.NVarChar,500){Value = detailId},
                };
                return dal.RollbackMassRemesaPagadas(param);
            }
        }

        #endregion

        #region SearchRemesasEntrantesFormBatch

        public HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantesFormBatch(OrdenEntranteRequest model)
        {
            using (IOperacionesDal<OrdenEntranteBackoffice> dal = new OperacionesDal<OrdenEntranteBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = model.CurrencyId},
                    new SqlParameter("@BankId", SqlDbType.Int){Value = (object)model.BankId??DBNull.Value},
                    new SqlParameter("@OperationsId", SqlDbType.VarChar, 5000){Value = (object)model.OperationsId??DBNull.Value},
                    new SqlParameter("@BankIds", SqlDbType.VarChar, 5000){Value = (object)model.BankIds??DBNull.Value},
                    new SqlParameter("@BatchId", SqlDbType.Int){Value = (object)model.BatchId??DBNull.Value},
                };
                return dal.SearchRemesasEntrantesFormBatch(param);
            }
        }

        #endregion

        #region SearchRemesasEntrantesFormBatchPorConfirmar

        public HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantesFormBatchPorConfirmar(OrdenEntranteRequest model)
        {
            using (IOperacionesDal<OrdenEntranteBackoffice> dal = new OperacionesDal<OrdenEntranteBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = model.CurrencyId},
                    new SqlParameter("@BankId", SqlDbType.Int){Value = (object)model.BankId??DBNull.Value},
                    new SqlParameter("@OperationsId", SqlDbType.VarChar, 5000){Value = (object)model.OperationsId??DBNull.Value},
                    new SqlParameter("@BankIds", SqlDbType.VarChar, -1){Value = (object)model.BankIds??DBNull.Value},
                };
                return dal.SearchRemesasEntrantesFormBatchPorConfirmar(param);
            }
        }

        #endregion

        #region UpdateStatusRemesasEntrantesIncidencias

        public GenericResponse UpdateStatusRemesasEntrantesIncidencias(REMESAS_ENTRANTESs model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@ID_OPERACION", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@status", SqlDbType.Int){Value = model.STATUS},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor},
                    new SqlParameter("@Modificado", SqlDbType.SmallDateTime){Value = DateTime.Now},
                    new SqlParameter("@OBSERVACIONES", SqlDbType.VarChar, 150){Value = model.OBSERVACIONES},
                };
                return dal.UpdateStatusRemesasEntrantesIncidencias(param);
            }
        }

        #endregion

        #region spuRemesaEntrantePagadaPendientePorConfirmar

        public GenericResponse spuRemesaEntrantePagadaPendientePorConfirmar(RemesaEntrantePagadaPendientePorConfirmar model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = model.id},
                    new SqlParameter("@referencia", SqlDbType.VarChar,50){Value = model.referencia},
                    new SqlParameter("@fecha_pago", SqlDbType.DateTime){Value = model.fecha_pago},
                    new SqlParameter("@observaciones", SqlDbType.VarChar,250){Value = model.observaciones},
                };
                return dal.spuRemesaEntrantePagadaPendientePorConfirmar(param);
            }
        }

        #endregion

        #region UpdateRemesasEntrantesIncidenciaRIA

        public GenericResponse UpdateRemesasEntrantesIncidenciaRIA(REMESAS_ENTRANTESs model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@ID_OPERACION", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@Modificado", SqlDbType.SmallDateTime){Value = DateTime.Now},
                    new SqlParameter("@MENSAJE", SqlDbType.VarChar, 150){Value = model.MENSAJE},
                };
                return dal.UpdateRemesasEntrantesIncidenciaRIA(param);
            }
        }

        #endregion

        #region SearchRemesasEntrantesRejectRIA

        public HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantesRejectRIA(OrdenEntranteRequest request)
        {
            using (IOperacionesDal<OrdenEntranteBackoffice> dal = new OperacionesDal<OrdenEntranteBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ficha", SqlDbType.VarChar, 25){Value = (object)request.ficha??DBNull.Value},
                    new SqlParameter("@identificacion", SqlDbType.VarChar, 25){Value = (object)request.identificacion??DBNull.Value},
                    new SqlParameter("@referencia", SqlDbType.VarChar, 50){Value = (object)request.referencia??DBNull.Value},
                    new SqlParameter("@corresponsal", SqlDbType.VarChar, 3){Value = (object)request.corresponsal??DBNull.Value, IsNullable = true},
                    new SqlParameter("@status", SqlDbType.Int){Value = (object)request.status??DBNull.Value},
                    new SqlParameter("@pais", SqlDbType.VarChar, 3){Value = (object)request.pais??DBNull.Value},
                    new SqlParameter("@id", SqlDbType.Int){Value = (object)request.id??DBNull.Value},
                    new SqlParameter("@fechaCreacionInicio", SqlDbType.SmallDateTime){Value = (object)request.fechaCreacionInicio??DBNull.Value},
                    new SqlParameter("@fechaCreacionFin", SqlDbType.SmallDateTime){Value = (object)request.fechaCreacionFin??DBNull.Value},
                    new SqlParameter("@fechaPagoInicio", SqlDbType.SmallDateTime){Value = (object)request.fechaPagoInicio??DBNull.Value},
                    new SqlParameter("@fechaPagoFin", SqlDbType.SmallDateTime){Value = (object)request.fechaPagoFin??DBNull.Value},
                    new SqlParameter("@secuencia", SqlDbType.Int){Value = (object)request.secuencia??DBNull.Value},
                    new SqlParameter("@GrupoId", SqlDbType.Int){Value = (object)request.GrupoId??DBNull.Value},
                    new SqlParameter("@Modo", SqlDbType.VarChar, 1){Value = (object)request.Modo??DBNull.Value},
                    new SqlParameter("@BankId", SqlDbType.Int){Value = (object)request.BankId??DBNull.Value},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = (object)request.CurrencyId??DBNull.Value},
                    new SqlParameter("@Pending", SqlDbType.Bit){Value = (object)request.Pending??DBNull.Value},
                };
                return dal.SearchRemesasEntrantesRejectRIA(param);
            }
        }

        #endregion

        #region InsertRemesaEntranteWithoutValidationProcess
        public GenericResponse InsertRemesaEntranteWithoutValidationProcess(REMESAS_ENTRANTESs model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@MONEDA", SqlDbType.Char, 3){Value = model.MONEDA},
                    new SqlParameter("@PAIS", SqlDbType.Char, 3){Value = model.PAIS},
                    new SqlParameter("@SUCURSAL", SqlDbType.Char, 3){Value = model.SUCURSAL},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char, 3){Value = model.CORRESPONSAL},
                    new SqlParameter("@REGISTRADO_POR", SqlDbType.Char, 15){Value = model.REGISTRADO_POR},
                    new SqlParameter("@PAGOMANUAL", SqlDbType.Bit){Value = model.PAGOMANUAL},
                    new SqlParameter("@SECUENCIA", SqlDbType.BigInt){Value = model.SECUENCIA},
                    new SqlParameter("@MODO", SqlDbType.Char, 1){Value = model.MODO},
                    new SqlParameter("@CIREM", SqlDbType.Char, 20){Value = model.CIREM},
                    new SqlParameter("@CIDES", SqlDbType.Char, 20){Value = model.CIDES},
                    new SqlParameter("@REFERENCIA", SqlDbType.VarChar, 50){Value = model.REFERENCIA},
                    new SqlParameter("@TELREM", SqlDbType.Char, 20){Value = model.TELREM},
                    new SqlParameter("@TELDES", SqlDbType.Char, 20){Value = model.TELDES},
                    new SqlParameter("@TEL2DES", SqlDbType.Char, 20){Value = model.TEL2DES},
                    new SqlParameter("@NOMREM", SqlDbType.VarChar, 50){Value = model.NOMREM},
                    new SqlParameter("@NOMDES", SqlDbType.VarChar, 50){Value = model.NOMDES},
                    new SqlParameter("@DIRDES", SqlDbType.VarChar, 100){Value = model.DIRDES},
                    new SqlParameter("@USD", SqlDbType.Money){Value = model.USD},
                    new SqlParameter("@TASA", SqlDbType.Money){Value = model.TASA},
                    new SqlParameter("@BOLI", SqlDbType.Money){Value = model.BOLI},
                    new SqlParameter("@COMIUSD", SqlDbType.Money){Value = model.COMIUSD},
                    new SqlParameter("@OTROS", SqlDbType.Money){Value = model.OTROS},
                    new SqlParameter("@IVA", SqlDbType.Char, 3){Value = model.IVA},
                    new SqlParameter("@FECHA", SqlDbType.SmallDateTime){Value = model.FECHA},
                    new SqlParameter("@OBSERVACIONES", SqlDbType.VarChar, 256){Value = model.OBSERVACIONES},
                    new SqlParameter("@MENSAJE", SqlDbType.VarChar, 100){Value = model.MENSAJE},
                    new SqlParameter("@BANCO", SqlDbType.VarChar, 100){Value = model.BancoPago},
                    new SqlParameter("@CUENTA", SqlDbType.VarChar, 100){Value = model.NumeroCuentaPago},
                    new SqlParameter("@TIPO_CUENTA", SqlDbType.VarChar, 100){Value = model.tipoCuenta},
                    new SqlParameter("@EMAIL", SqlDbType.VarChar, 150){Value = model.email},
                    new SqlParameter("@Estatus", SqlDbType.TinyInt){Value = model.STATUS}
                };
                return dal.InsertRemesaEntranteWithoutValidationProcess(param);
            }
        }
        #endregion

        #region Pago de lote
        public GenericResponse UpdateStatusLotPayment(BatchBankDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value = model.@DetailId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@OrderIncomingId", SqlDbType.Int){Value = model.OrderIncomingId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar, 15){Value = model.UpdateUser},
                    new SqlParameter("@Observation", SqlDbType.VarChar,500){Value = model.DetailObservacion},
                    new SqlParameter("@ReasonRejectedId", SqlDbType.Int){Value = (object)model.ReasonRejectedId??DBNull.Value},
                    new SqlParameter("@DetailPaymentReference", SqlDbType.VarChar,50){Value = (object)model.DetailPaymentReference?? DBNull.Value},
                    new SqlParameter("@DetailOperationDate", SqlDbType.SmallDateTime){Value = (object)model.DetailOperationDate?? DBNull.Value},
                };
                return dal.UpdateStatusLotPayment(param);
            }
        }
        #endregion

        #region AccountReceivableCorrespondent

        #region SearchAccountReceivableCorrespondent

        public HashSet<AccountReceivableCorrespondent> SearchAccountReceivableCorrespondent(AccountReceivableCorrespondentRequest model)
        {
            using (IOperacionesDal<AccountReceivableCorrespondent> dal = new OperacionesDal<AccountReceivableCorrespondent>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value= model.AccountId??(object)DBNull.Value},
                    new SqlParameter("@CorrespondentId", SqlDbType.Int){Value= model.CorrespondentId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@AccountNumber", SqlDbType.Int){Value= model.AccountNumber??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},

                };
                return dal.SearchAccountReceivableCorrespondent(param);
            }
        }
        #endregion

        #region InsertAccountReceivableCorrespondent

        public HashSet<GenericResponse> InsertAccountReceivableCorrespondent(AccountReceivableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CorrespondentId", SqlDbType.Int){Value = model.CorrespondentId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar,15){Value = model.CreationUser},
                    new SqlParameter("@AccountTotalOperation", SqlDbType.Int){Value = model.AccountTotalOperation},
                    new SqlParameter("@AccountTotalAmmount", SqlDbType.Money){Value = model.AccountTotalAmmount},
                    new SqlParameter("@CreationDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                    new SqlParameter("@AccountPaymentReference", SqlDbType.VarChar,20){Value = (object)model.AccountPaymentReference??DBNull.Value},
                    new SqlParameter("@AccountObservation", SqlDbType.VarChar,500){Value = (object)model.AccountObservation??DBNull.Value},
                    new SqlParameter("@AccounPaymentObservation", SqlDbType.VarChar,500){Value = (object)model.AccounPaymentObservation??DBNull.Value},
                    new SqlParameter("@AccountPaymentDate", SqlDbType.SmallDateTime){Value = (object)model.AccountPaymentDate??DBNull.Value},
                };
                return dal.InsertAccountReceivableCorrespondent(param);
            }
        }

        #endregion

        #region UpdateAccountReceivableCorrespondent

        public GenericResponse UpdateAccountReceivableCorrespondent(AccountReceivableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@CorrespondentId", SqlDbType.Int){Value = model.CorrespondentId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@AccountNumber", SqlDbType.Int){Value = model.AccountNumber},
                    new SqlParameter("@AccountTotalOperation", SqlDbType.Int){Value = model.AccountTotalOperation},
                    new SqlParameter("@AccountTotalAmmount", SqlDbType.Money){Value = model.AccountTotalAmmount},
                    new SqlParameter("@AccountPaymentReference", SqlDbType.VarChar,20){Value = (object)model.AccountPaymentReference??DBNull.Value},
                    new SqlParameter("@AccountObservation", SqlDbType.VarChar,500){Value = (object)model.AccountObservation??DBNull.Value},
                    new SqlParameter("@AccounPaymentObservation", SqlDbType.VarChar,500){Value = (object)model.AccounPaymentObservation??DBNull.Value},
                    new SqlParameter("@AccountPaymentDate", SqlDbType.SmallDateTime){Value = (object)model.AccountPaymentDate??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountReceivableCorrespondent(param);
            }
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentRemoveDetail

        public GenericResponse UpdateAccountReceivableCorrespondentRemoveDetail(AccountReceivableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@AccountTotalOperation", SqlDbType.Int){Value = model.AccountTotalOperation},
                    new SqlParameter("@AccountTotalAmmount", SqlDbType.Money){Value = model.AccountTotalAmmount},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountReceivableCorrespondentRemoveDetail(param);
            }
        }

        #endregion

        #region UpdateStatusAccountReceivableCorrespondent

        public GenericResponse UpdateStatusAccountReceivableCorrespondent(AccountReceivableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateStatusAccountReceivableCorrespondent(param);
            }
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentPayment

        public GenericResponse UpdateAccountReceivableCorrespondentPayment(AccountReceivableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@AccountPaymentReference", SqlDbType.VarChar,20){Value = model.AccountPaymentReference},
                    new SqlParameter("@AccounPaymentObservation", SqlDbType.VarChar,500){Value = model.AccounPaymentObservation},
                    new SqlParameter("@AccountPaymentDate", SqlDbType.DateTime){Value = model.AccountPaymentDate},
                    new SqlParameter("@AccountPaymentBank", SqlDbType.Int){Value = model.AccountPaymentBank},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountReceivableCorrespondentPayment(param);
            }
        }

        #endregion


        #region SearchDraftsReceivable
        public HashSet<OrdenEntranteBackoffice> SearchDraftsReceivable(OrdenEntranteRequest model)
        {
            using (IOperacionesDal<OrdenEntranteBackoffice> dal = new OperacionesDal<OrdenEntranteBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@corresponsal", SqlDbType.Int){Value = model.CorresponsalId},
                    new SqlParameter("@fechaPagoInicio", SqlDbType.DateTime){Value = (object)model.fechaPagoInicio??DBNull.Value},
                    new SqlParameter("@fechaPagoFin", SqlDbType.DateTime){Value = (object)model.fechaPagoFin??DBNull.Value},
                };
                return dal.SearchDraftsReceivable(param);
            }
        }

        #endregion
        #endregion

        #region AccountReceivableCorrespondentDetail

        #region SearchAccountReceivableCorrespondentDetail

        public HashSet<AccountReceivableCorrespondentDetail> SearchAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetailRequest model)
        {
            using (IOperacionesDal<AccountReceivableCorrespondentDetail> dal = new OperacionesDal<AccountReceivableCorrespondentDetail>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value= model.DetailId??(object)DBNull.Value},
                    new SqlParameter("@OperationId", SqlDbType.Int){Value= model.OperationId??(object)DBNull.Value},
                    new SqlParameter("@AccountId", SqlDbType.Int){Value= model.AccountId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@Corresponsal", SqlDbType.Int){Value= model.Corresponsal??(object)DBNull.Value},
                };
                return dal.SearchAccountReceivableCorrespondentDetail(param);
            }
        }
        #endregion

        #region InsertAccountReceivableCorrespondentDetail

        public GenericResponse InsertAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@OperationId", SqlDbType.Int){Value = (object)model.OperationId},
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar,15){Value = model.CreationUser},
                    new SqlParameter("@CreationDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                    new SqlParameter("@DetailObservation", SqlDbType.VarChar,500){Value = (object)model.DetailObservation??DBNull.Value},
                };
                return dal.InsertAccountReceivableCorrespondentDetail(param);
            }
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentDetail

        public GenericResponse UpdateAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value = (object)model.DetailId},
                    new SqlParameter("@OperationId", SqlDbType.Int){Value = (object)model.OperationId},
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@DetailObservation", SqlDbType.VarChar,500){Value = (object)model.DetailObservation??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountReceivableCorrespondentDetail(param);
            }
        }

        #endregion

        #region UpdateStatusAccountReceivableCorrespondentDetail

        public GenericResponse UpdateStatusAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value = (object)model.DetailId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateStatusAccountReceivableCorrespondentDetail(param);
            }
        }

        #endregion

        #endregion

        #region AccountsPayableCorrespondent

        #region SearchAccountsPayableCorrespondent

        public HashSet<AccountsPayableCorrespondent> SearchAccountsPayableCorrespondent(AccountsPayableCorrespondentRequest model)
        {
            using (IOperacionesDal<AccountsPayableCorrespondent> dal = new OperacionesDal<AccountsPayableCorrespondent>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value= model.AccountId??(object)DBNull.Value},
                    new SqlParameter("@CorrespondentId", SqlDbType.Int){Value= model.CorrespondentId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@AccountNumber", SqlDbType.Int){Value= model.AccountNumber??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@CreacionTo", SqlDbType.DateTime){Value= model.CreacionTo??(object)DBNull.Value},

                };
                return dal.SearchAccountsPayableCorrespondent(param);
            }
        }
        #endregion

        #region InsertAccountsPayableCorrespondent

        public HashSet<GenericResponse> InsertAccountsPayableCorrespondent(AccountsPayableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CorrespondentId", SqlDbType.Int){Value = model.CorrespondentId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar,15){Value = model.CreationUser},
                    new SqlParameter("@AccountTotalOperation", SqlDbType.Int){Value = model.AccountTotalOperation},
                    new SqlParameter("@AccountTotalAmmount", SqlDbType.Money){Value = model.AccountTotalAmmount},
                    new SqlParameter("@CreationDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                    new SqlParameter("@AccountPaymentReference", SqlDbType.VarChar,20){Value = (object)model.AccountPaymentReference??DBNull.Value},
                    new SqlParameter("@AccountObservation", SqlDbType.VarChar,500){Value = (object)model.AccountObservation??DBNull.Value},
                    new SqlParameter("@AccounPaymentObservation", SqlDbType.VarChar,500){Value = (object)model.AccounPaymentObservation??DBNull.Value},
                    new SqlParameter("@AccountPaymentDate", SqlDbType.SmallDateTime){Value = (object)model.AccountPaymentDate??DBNull.Value},
                };
                return dal.InsertAccountsPayableCorrespondent(param);
            }
        }

        #endregion

        #region UpdateAccountsPayableCorrespondent

        public GenericResponse UpdateAccountsPayableCorrespondent(AccountsPayableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@CorrespondentId", SqlDbType.Int){Value = model.CorrespondentId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@AccountNumber", SqlDbType.Int){Value = model.AccountNumber},
                    new SqlParameter("@AccountTotalOperation", SqlDbType.Int){Value = model.AccountTotalOperation},
                    new SqlParameter("@AccountTotalAmmount", SqlDbType.Money){Value = model.AccountTotalAmmount},
                    new SqlParameter("@AccountPaymentReference", SqlDbType.VarChar,20){Value = (object)model.AccountPaymentReference??DBNull.Value},
                    new SqlParameter("@AccountObservation", SqlDbType.VarChar,500){Value = (object)model.AccountObservation??DBNull.Value},
                    new SqlParameter("@AccounPaymentObservation", SqlDbType.VarChar,500){Value = (object)model.AccounPaymentObservation??DBNull.Value},
                    new SqlParameter("@AccountPaymentDate", SqlDbType.SmallDateTime){Value = (object)model.AccountPaymentDate??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountsPayableCorrespondent(param);
            }
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentRemoveDetail

        public GenericResponse UpdateAccountsPayableCorrespondentRemoveDetail(AccountsPayableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@AccountTotalOperation", SqlDbType.Int){Value = model.AccountTotalOperation},
                    new SqlParameter("@AccountTotalAmmount", SqlDbType.Money){Value = model.AccountTotalAmmount},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountsPayableCorrespondentRemoveDetail(param);
            }
        }

        #endregion

        #region UpdateStatusAccountsPayableCorrespondent

        public GenericResponse UpdateStatusAccountsPayableCorrespondent(AccountsPayableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateStatusAccountsPayableCorrespondent(param);
            }
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentPayment

        public GenericResponse UpdateAccountsPayableCorrespondentPayment(AccountsPayableCorrespondent model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@AccountPaymentReference", SqlDbType.VarChar,20){Value = model.AccountPaymentReference},
                    new SqlParameter("@AccounPaymentObservation", SqlDbType.VarChar,500){Value = model.AccounPaymentObservation},
                    new SqlParameter("@AccountPaymentDate", SqlDbType.DateTime){Value = model.AccountPaymentDate},
                    new SqlParameter("@AccountPaymentBank", SqlDbType.Int){Value = model.AccountPaymentBank},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountsPayableCorrespondentPayment(param);
            }
        }

        #endregion

        #endregion

        #region AccountsPayableCorrespondentDetail

        #region SearchAccountsPayableCorrespondentDetail

        public HashSet<AccountsPayableCorrespondentDetail> SearchAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetailRequest model)
        {
            using (IOperacionesDal<AccountsPayableCorrespondentDetail> dal = new OperacionesDal<AccountsPayableCorrespondentDetail>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value= model.DetailId??(object)DBNull.Value},
                    new SqlParameter("@OperationId", SqlDbType.Int){Value= model.OperationId??(object)DBNull.Value},
                    new SqlParameter("@AccountId", SqlDbType.Int){Value= model.AccountId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@Corresponsal", SqlDbType.Int){Value= model.Corresponsal??(object)DBNull.Value},
                };
                return dal.SearchAccountsPayableCorrespondentDetail(param);
            }
        }
        #endregion

        #region InsertAccountsPayableCorrespondentDetail

        public GenericResponse InsertAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@OperationId", SqlDbType.Int){Value = (object)model.OperationId},
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar,15){Value = model.CreationUser},
                    new SqlParameter("@CreationDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                    new SqlParameter("@DetailObservation", SqlDbType.VarChar,500){Value = (object)model.DetailObservation??DBNull.Value},
                };
                return dal.InsertAccountsPayableCorrespondentDetail(param);
            }
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentDetail

        public GenericResponse UpdateAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value = (object)model.DetailId},
                    new SqlParameter("@OperationId", SqlDbType.Int){Value = (object)model.OperationId},
                    new SqlParameter("@AccountId", SqlDbType.Int){Value = model.AccountId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@DetailObservation", SqlDbType.VarChar,500){Value = (object)model.DetailObservation??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateAccountsPayableCorrespondentDetail(param);
            }
        }

        #endregion

        #region UpdateStatusAccountsPayableCorrespondentDetail

        public GenericResponse UpdateStatusAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DetailId", SqlDbType.Int){Value = (object)model.DetailId},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@UpdateUser", SqlDbType.VarChar,15){Value = (object)model.UpdateUser??DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.SmallDateTime){Value = DateTime.Now},
                };
                return dal.UpdateStatusAccountsPayableCorrespondentDetail(param);
            }
        }

        #endregion

        #endregion

        #region CompanyAlliance

        #region SearchCompanyAlliance

        public HashSet<CompanyAlliance> SearchCompanyAlliance(CompanyAllianceRequest model)
        {
            using (IOperacionesDal<CompanyAlliance> dal = new OperacionesDal<CompanyAlliance>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AllianceId", SqlDbType.Int){Value= model.AllianceId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@TypeIdentificationId", SqlDbType.Char,1){Value= model.TypeIdentificationId??(object)DBNull.Value},
                    new SqlParameter("@AllianceIdentificationNumber", SqlDbType.Int){Value= model.AllianceIdentificationNumber??(object)DBNull.Value},
                };
                return dal.SearchCompanyAlliance(param);
            }
        }
        #endregion


        #endregion

        #region CompanyAllianceBank

        #region SearchCompanyAllianceBank

        public HashSet<CompanyAllianceBank> SearchCompanyAllianceBank(CompanyAllianceBankRequest model)
        {
            using (IOperacionesDal<CompanyAllianceBank> dal = new OperacionesDal<CompanyAllianceBank>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@AllianceBankId", SqlDbType.Int){Value= model.AllianceBankId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@AllianceId", SqlDbType.Int){Value= model.AllianceId??(object)DBNull.Value},
                    new SqlParameter("@BankId", SqlDbType.Int){Value= model.BankId??(object)DBNull.Value},
                };
                return dal.SearchCompanyAllianceBank(param);
            }
        }
        #endregion

        #endregion

        #region ShippingAlliance

        #region SearchShippingAlliance

        public HashSet<ShippingAlliance> SearchShippingAlliance(ShippingAllianceRequest model)
        {
            using (IOperacionesDal<ShippingAlliance> dal = new OperacionesDal<ShippingAlliance>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ShippingAllianceId", SqlDbType.Int){Value= model.ShippingAllianceId??(object)DBNull.Value},
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@AllianceId", SqlDbType.Int){Value= model.AllianceId??(object)DBNull.Value},
                    new SqlParameter("@persitentObjectID", SqlDbType.VarChar,14){Value= model.persitentObjectID??(object)DBNull.Value},
                    new SqlParameter("@IdentificationNumber", SqlDbType.Int){Value= model.IdentificationNumber??(object)DBNull.Value},
                    new SqlParameter("@Tracking", SqlDbType.VarChar,50){Value= model.Tracking??(object)DBNull.Value},
                };
                return dal.SearchShippingAlliance(param);
            }
        }
        #endregion

        #region InsertShippingAlliance

        public GenericResponse InsertShippingAlliance(ShippingAlliance model)
        {
            foreach (var item in model.ShippingAllianceList)
            {
                using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
                {

                    SqlParameter[] param =
                    {
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@AllianceId", SqlDbType.Int){Value = model.AllianceId},
                    new SqlParameter("@persitentObjectID", SqlDbType.VarChar,14){Value = model.persitentObjectID},
                    new SqlParameter("@IdentificationNumber", SqlDbType.Int){Value = model.IdentificationNumber},
                    new SqlParameter("@Tracking", SqlDbType.VarChar,50){Value = item.CodigoEncomienda},
                    new SqlParameter("@Weight", SqlDbType.Decimal){Value= model.Weight??(object)DBNull.Value},
                    new SqlParameter("@DestinationCountry", SqlDbType.Char,3){Value= model.DestinationCountry??(object)DBNull.Value},
                    new SqlParameter("@Amount", SqlDbType.Money){Value = item.Monto},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar,15){Value = model.CreationUser},
                    new SqlParameter("@CreationDate", SqlDbType.DateTime){Value = DateTime.Now},
                    };
                    var resultdal = dal.InsertShippingAlliance(param);

                    if (resultdal.Error)
                    {
                        //rolback del for
                        RollbackShippingAlliance(model);
                        return resultdal;
                    }
                }
            }
            return new GenericResponse();
        }

        #endregion

        #region UpdateOrderNumberShippingAlliance

        public GenericResponse UpdateOrderNumberShippingAlliance(ShippingAlliance model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ShippingAllianceId", SqlDbType.Int){Value = model.ShippingAllianceId},
                    new SqlParameter("@Tracking", SqlDbType.VarChar,50){Value = model.Tracking},
                    new SqlParameter("@OrderNumber", SqlDbType.Int){Value = model.OrderNumber},
                };
                return dal.UpdateOrderNumberShippingAlliance(param);
            }
        }

        #endregion

        #region RollbackShippingAlliance

        public GenericResponse RollbackShippingAlliance(ShippingAlliance model)
        {
            foreach (var item in model.ShippingAllianceList)
            {
                using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
                {

                    SqlParameter[] param =
                    {
                        new SqlParameter("@AllianceId", SqlDbType.Int){Value = model.AllianceId},
                        new SqlParameter("@persitentObjectID", SqlDbType.VarChar,14){Value = model.persitentObjectID},
                        new SqlParameter("@Tracking", SqlDbType.VarChar,50){Value = model.Tracking},
                    };
                    dal.RollbackShippingAlliance(param);
                }
            }
            return new GenericResponse();
        }

        #endregion

        #endregion

        #region SearchRemesasGirosPendientes
        public HashSet<OrdenEntranteBackoffice> SearchRemesasGirosPendientes(OrdenEntranteRequest request)
        {
            using (IOperacionesDal<OrdenEntranteBackoffice> dal = new OperacionesDal<OrdenEntranteBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ficha", SqlDbType.VarChar, 25){Value = (object)request.ficha??DBNull.Value},
                    new SqlParameter("@identificacion", SqlDbType.VarChar, 25){Value = (object)request.identificacion??DBNull.Value},
                    new SqlParameter("@referencia", SqlDbType.VarChar, 50){Value = (object)request.referencia??DBNull.Value},
                    new SqlParameter("@corresponsal", SqlDbType.VarChar, 3){Value = (object)request.corresponsal??DBNull.Value, IsNullable = true},
                    new SqlParameter("@status", SqlDbType.Int){Value = (object)request.status??DBNull.Value},
                    new SqlParameter("@pais", SqlDbType.VarChar, 3){Value = (object)request.pais??DBNull.Value},
                    new SqlParameter("@id", SqlDbType.Int){Value = (object)request.id??DBNull.Value},
                    new SqlParameter("@fechaCreacionInicio", SqlDbType.SmallDateTime){Value = (object)request.fechaCreacionInicio??DBNull.Value},
                    new SqlParameter("@fechaCreacionFin", SqlDbType.SmallDateTime){Value = (object)request.fechaCreacionFin??DBNull.Value},
                    new SqlParameter("@fechaPagoInicio", SqlDbType.SmallDateTime){Value = (object)request.fechaPagoInicio??DBNull.Value},
                    new SqlParameter("@fechaPagoFin", SqlDbType.SmallDateTime){Value = (object)request.fechaPagoFin??DBNull.Value},
                    new SqlParameter("@secuencia", SqlDbType.Int){Value = (object)request.secuencia??DBNull.Value},
                    new SqlParameter("@GrupoId", SqlDbType.Int){Value = (object)request.GrupoId??DBNull.Value},
                    new SqlParameter("@Modo", SqlDbType.VarChar, 1){Value = (object)request.Modo??DBNull.Value},
                    new SqlParameter("@BankId", SqlDbType.Int){Value = (object)request.BankId??DBNull.Value},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = (object)request.CurrencyId??DBNull.Value},
                    new SqlParameter("@CorresponsalId", SqlDbType.Int){Value = (object)request.CorresponsalId??DBNull.Value},
                };
                return dal.SearchRemesasGirosPendientes(param);
            }
        }
        #endregion

        #region SearchInternationalMoneyOrderPayment
        public HashSet<OrdenEntranteBackoffice> SearchInternationalMoneyOrderPayment(OrdenEntranteRequest request)
        {
            using (IOperacionesDal<OrdenEntranteBackoffice> dal = new OperacionesDal<OrdenEntranteBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@IDENTIFICACION", SqlDbType.VarChar, 25){Value = (object)request.identificacion??DBNull.Value},
                };
                return dal.SearchInternationalMoneyOrderPayment(param);
            }
        }
        #endregion

        #region OPERACIONES 

        #region SearchOperacionesPorPagar
        public HashSet<OPERACIONES_POR_PAGAR> SearchOperacionesPorPagar(OPERACIONES_POR_PAGARRequest request)
        {
            using (IOperacionesDal<OPERACIONES_POR_PAGAR> dal = new OperacionesDal<OPERACIONES_POR_PAGAR>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Id_OPERACION", SqlDbType.Int){Value = (object)request.Id_OPERACION??DBNull.Value},
                    new SqlParameter("@CIREM", SqlDbType.Char, 15){Value = (object)request.CIREM??DBNull.Value},
                    new SqlParameter("@CIDES", SqlDbType.Char, 15){Value = (object)request.CIDES??DBNull.Value},
                    new SqlParameter("@Sucursal", SqlDbType.Char, 3){Value = (object)request.Sucursal??DBNull.Value},
                    new SqlParameter("@Estatus", SqlDbType.VarChar, 200){Value = (object)request.Estatus??DBNull.Value},
                };
                return dal.SearchOperacionesPorPagar(param);
            }
        }
        #endregion

        #region InsertOperacionesPorPagar
        public GenericResponse InsertOperacionesPorPagar(OPERACIONES_POR_PAGAR model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CiuOrig", SqlDbType.Char, 2){Value = model.CiuOrig},
                    new SqlParameter("@NroRecibo", SqlDbType.Int){Value = model.NroRecibo},
                    new SqlParameter("@SECUENCIA", SqlDbType.BigInt){Value = model.SECUENCIA},
                    new SqlParameter("@Referencia", SqlDbType.Char, 20){Value = model.Referencia},
                    new SqlParameter("@CIREM", SqlDbType.Char, 15){Value = model.CIREM},
                    new SqlParameter("@NOMREM", SqlDbType.Char, 50){Value = model.NOMREM},
                    new SqlParameter("@TELREM", SqlDbType.Char, 20){Value = model.TELREM},
                    new SqlParameter("@NOMDES", SqlDbType.Char, 50){Value = model.NOMDES},
                    new SqlParameter("@CIDES", SqlDbType.Char, 15){Value = model.CIDES},
                    new SqlParameter("@DIRDES", SqlDbType.Char, 100){Value = model.DIRDES},
                    new SqlParameter("@TELDES", SqlDbType.Char, 20){Value = model.TELDES},
                    new SqlParameter("@TEL2DES", SqlDbType.Char, 20){Value = model.TEL2DES},
                    new SqlParameter("@PAIS", SqlDbType.Char, 3){Value = model.PAIS},
                    new SqlParameter("@CIUDAD", SqlDbType.Char, 3){Value = model.CIUDAD},
                    new SqlParameter("@MODO", SqlDbType.Char, 1){Value = model.MODO},
                    new SqlParameter("@USD", SqlDbType.Money){Value = model.USD},
                    new SqlParameter("@TASA", SqlDbType.Money){Value = model.TASA},
                    new SqlParameter("@BOLI", SqlDbType.Money){Value = model.BOLI},
                    new SqlParameter("@COMIUSD", SqlDbType.Money){Value = model.COMIUSD},
                    new SqlParameter("@OTROS", SqlDbType.Money){Value = model.OTROS},
                    new SqlParameter("@IVA", SqlDbType.Char, 3){Value = model.IVA},
                    new SqlParameter("@Observaciones", SqlDbType.Char, 256){Value = (object)model.Observaciones??DBNull.Value},
                    new SqlParameter("@Mensaje", SqlDbType.Char, 80){Value = (object)model.Mensaje??DBNull.Value},
                    new SqlParameter("@Fecha", SqlDbType.SmallDateTime){Value = model.Fecha},
                    new SqlParameter("@FechaPago", SqlDbType.SmallDateTime){Value = (object)model.FechaPago??DBNull.Value},
                    new SqlParameter("@Status", SqlDbType.TinyInt){Value = model.Status},
                    new SqlParameter("@Status_Temp", SqlDbType.TinyInt){Value = model.Status_Temp},
                    new SqlParameter("@Tipo_Operacion", SqlDbType.Int){Value = model.Tipo_Operacion},
                    new SqlParameter("@PagoManual", SqlDbType.TinyInt){Value = model.PagoManual},
                    new SqlParameter("@USUARIO", SqlDbType.Char, 15){Value = model.USUARIO},
                    new SqlParameter("@FICHA", SqlDbType.Int){Value = (object)model.FICHA??DBNull.Value},
                    new SqlParameter("@PERSONA", SqlDbType.Int){Value = (object)model.PERSONA??DBNull.Value},
                    new SqlParameter("@OPERADOR", SqlDbType.VarChar,50){Value = (object)model.OPERADOR??DBNull.Value},
                    new SqlParameter("@ESTACION_TRABAJO", SqlDbType.VarChar,50){Value = (object)model.ESTACION_TRABAJO??DBNull.Value},
                    new SqlParameter("@ACTUALIZADO", SqlDbType.SmallDateTime){Value = (object)model.ACTUALIZADO??DBNull.Value},
                };
                return dal.InsertOperacionesPorPagar(param);
            }
        }
        #endregion

        #region InsertOperacionesPorCobrar

        public GenericResponse InsertOperacionesPorCobrar(OperacionesPorCobrar model)
        {

            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                     new SqlParameter("@CiuOrig", SqlDbType.Char, 2){Value = model.CiuOrig},
                     new SqlParameter("@NroRecibo", SqlDbType.Int){Value = model.NroRecibo},
                     new SqlParameter("@CIRem", SqlDbType.Char, 15){Value = model.Cirem},
                     new SqlParameter("@NomRemA", SqlDbType.Char, 20){Value = model.NomRemA},
                     new SqlParameter("@NomRemB", SqlDbType.Char, 20){Value = (object)model.NomRemB??DBNull.Value},
                     new SqlParameter("@ApeRemA", SqlDbType.Char, 20){Value = model.ApeRemA},
                     new SqlParameter("@ApeRemB", SqlDbType.Char, 20){Value = (object)model.ApeRemB??DBNull.Value },
                     new SqlParameter("@TelRem", SqlDbType.Char, 21){Value = model.TelRem},
                     new SqlParameter("@DirRem", SqlDbType.Char, 100){Value = model.DirRem},
                     new SqlParameter("@NomDesA", SqlDbType.Char, 20){Value = model.NomDesA},
                     new SqlParameter("@NomDesB", SqlDbType.Char, 20){Value = model.NomDesB},
                     new SqlParameter("@ApeDesA", SqlDbType.Char, 20){Value = model.ApeDesA},
                     new SqlParameter("@ApeDesB",SqlDbType.Char, 20){Value = model.ApeDesB},
                     new SqlParameter("@CCDes",SqlDbType.Char, 20){Value = model.Ccdes},
                     new SqlParameter("@DirDes",SqlDbType.Char, 100){Value = model.DirDes},
                     new SqlParameter("@TelDes",SqlDbType.Char, 15){Value = model.TelDes},
                     new SqlParameter("@Tel2Des",SqlDbType.Char, 15){Value = model.Tel2Des},
                     new SqlParameter("@Pais",SqlDbType.Char, 3){Value = model.Pais},
                     new SqlParameter("@Pagador",SqlDbType.Char, 3){Value = model.Pagador},
                     new SqlParameter("@Ciudad",SqlDbType.Int){Value = model.Ciudad},
                     new SqlParameter("@Oficina",SqlDbType.Int){Value = model.Oficina},
                     new SqlParameter("@Bolivares",SqlDbType.Decimal){Value = model.Bolivares},
                     new SqlParameter("@Dolares",SqlDbType.Decimal){Value = model.Dolares},
                     new SqlParameter("@TasaDolar",SqlDbType.Decimal){Value = model.TasaDolar},
                     new SqlParameter("@MonedaDest",SqlDbType.Decimal){Value = model.MonedaDest},
                     new SqlParameter("@PagoTarifaUS",SqlDbType.Decimal){Value = model.PagoTarifaUs},
                     new SqlParameter("@PagoTarifa",SqlDbType.Decimal){Value = model.PagoTarifa},
                     new SqlParameter("@PagoOtros",SqlDbType.Decimal){Value = model.PagoOtros},
                     new SqlParameter("@PagoIsv",SqlDbType.Decimal){Value = model.PagoIsv},
                     new SqlParameter("@Observaciones", SqlDbType.Char, 256){Value = model.Observaciones},
                     new SqlParameter("@Mensaje", SqlDbType.Char, 80){Value = model.Mensaje},
                     new SqlParameter("@Usuario", SqlDbType.Char, 15){Value = model.Usuario},
                     new SqlParameter("@Status", SqlDbType.TinyInt){Value = model.Status},
                     new SqlParameter("@TIPOSOL", SqlDbType.Char, 2){Value = model.TIPOSOL},
                     new SqlParameter("@Cadivi", SqlDbType.Char, 50){Value = model.Cadivi},
                     new SqlParameter("@Status_Temp", SqlDbType.TinyInt){Value = model.Status_Temp},
                     new SqlParameter("@Tipo_Operacion", SqlDbType.Int){Value = model.Tipo_Operacion},
                     new SqlParameter("@MesRemesa", SqlDbType.Char, 25){Value = (object)model.MesRemesa??DBNull.Value},
                     new SqlParameter("@Ficha", SqlDbType.Int){Value = model.Ficha},
                     new SqlParameter("@Persona", SqlDbType.Int){Value = model.Persona},
                     new SqlParameter("@ReTransmite", SqlDbType.TinyInt){Value = model.ReTransmite},
                     new SqlParameter("@ReciboReTransmite", SqlDbType.Int){Value = (object)model.ReciboReTransmite??DBNull.Value},
                     new SqlParameter("@ID_SCD", SqlDbType.Int){Value = (object)model.ID_SCD??DBNull.Value},
                     new SqlParameter("@ID_PROX_SOL", SqlDbType.Int){Value = (object)model.ID_PROX_SOL??DBNull.Value},
                     new SqlParameter("@Concepto", SqlDbType.Int){Value = (object)model.Concepto??DBNull.Value},
                     new SqlParameter("@ES_MATRICULA", SqlDbType.Bit){Value = (object)model.ES_MATRICULA??DBNull.Value},
                     new SqlParameter("@NOMBRE_BENEFICIARIO_MATRICULA", SqlDbType.Char, 100){Value =(object)model.NOMBRE_BENEFICIARIO_MATRICULA??DBNull.Value},
                     new SqlParameter("@CEDULA_BENEFICIARIO_MATRICULA", SqlDbType.Char, 20){Value =(object)model.CEDULA_BENEFICIARIO_MATRICULA??DBNull.Value },
                     new SqlParameter("@tasaAplicada", SqlDbType.Decimal){Value = (object)model.TASA_APLICADA??DBNull.Value},
                     new SqlParameter("@detalleTipoOperacion", SqlDbType.Int){Value = model.DETALLE_TIPO_OPERACION},
                     new SqlParameter("@moneda", SqlDbType.Int){Value = model.MONEDA},
                     new SqlParameter("@sucursalNew", SqlDbType.Int){Value = (object)model.SUCURSAL_NEW??DBNull.Value},
                     new SqlParameter("@corresponsal", SqlDbType.Char, 3){Value = model.CORRESPONSAL},
                     new SqlParameter("@fechaValorTasa", SqlDbType.DateTime){Value = (object)model.FECHA_VALOR_TASA??DBNull.Value},
                     new SqlParameter("@MOTIVO_OP_BCV", SqlDbType.Int){Value = (object)model.MOTIVO_OP_BCV??DBNull.Value},
                     new SqlParameter("@TIPO_OP_BCV", SqlDbType.Char, 10){Value = (object)model.TIPO_OP_BCV??DBNull.Value},
                     new SqlParameter("@BANCO_NACIONAL", SqlDbType.Char, 3){Value = (object)model.BANCO_NACIONAL??DBNull.Value},
                     new SqlParameter("@NUMERO_CUENTA", SqlDbType.Char, 50){Value = (object)model.NUMERO_CUENTA??DBNull.Value},
                     new SqlParameter("@EMAIL_CLIENTE", SqlDbType.Char, 50){Value = model.EMAIL_CLIENTE},
                     new SqlParameter("@EMAIL_BENEFICIARIO", SqlDbType.Char, 50){Value = (object)model.EMAIL_BENEFICIARIO??DBNull.Value},
                     new SqlParameter("@BANCO_DESTINO", SqlDbType.Char, 50){Value =  (object)model.BANCO_DESTINO??DBNull.Value},
                     new SqlParameter("@NUMERO_CUENTA_DESTINO", SqlDbType.Char, 50){Value = (object)model.NUMERO_CUENTA_DESTINO??DBNull.Value},
                     new SqlParameter("@DIRECCION_BANCO", SqlDbType.Char, 2000){Value = (object)model.DIRECCION_BANCO??DBNull.Value},
                     new SqlParameter("@ABA", SqlDbType.Char, 50){Value = (object)model.ABA??DBNull.Value},
                     new SqlParameter("@SWIFT", SqlDbType.Char, 50){Value = (object)model.SWIFT??DBNull.Value },
                     new SqlParameter("@IBAN", SqlDbType.Char, 50){Value = (object)model.IBAN??DBNull.Value},
                     new SqlParameter("@BANCO_INTERMEDIARIO", SqlDbType.Char, 50){Value = (object)model.BANCO_INTERMEDIARIO??DBNull.Value},
                     new SqlParameter("@NUMERO_CUENTA_INTERMEDIARIO", SqlDbType.Char, 50){Value = (object)model.NUMERO_CUENTA_INTERMEDIARIO??DBNull.Value},
                     new SqlParameter("@DIRECCION_BANCO_INTERMEDIARIO", SqlDbType.Char, 2000){Value = (object)model.DIRECCION_BANCO_INTERMEDIARIO??DBNull.Value},
                     new SqlParameter("@ABA_INTERMEDIARIO", SqlDbType.Char, 50){Value = (object)model.ABA_INTERMEDIARIO??DBNull.Value},
                     new SqlParameter("@SWIFT_INTERMEDIARIO", SqlDbType.Char, 50){Value = (object)model.SWIFT_INTERMEDIARIO??DBNull.Value},
                     new SqlParameter("@IBAN_INTERMEDIARIO", SqlDbType.Char, 50){Value = (object)model.IBAN_INTERMEDIARIO??DBNull.Value},
                     new SqlParameter("@pMonedaOperacion", SqlDbType.Int){Value = (object)model.MonedaOperacion??DBNull.Value},
                     new SqlParameter("@pTasaConversion", SqlDbType.Decimal){Value = (object)model.TasaConversion??DBNull.Value},
                     new SqlParameter("@pMontoConversion", SqlDbType.Decimal){Value = (object)model.MontoConversion??DBNull.Value},
                     new SqlParameter("@TypeAccountBank", SqlDbType.Char,10){Value = (object)model.TypeAccountBank??DBNull.Value},
                     new SqlParameter("@NumeroOpTemporal", SqlDbType.BigInt){Value = (object)model.NumeroOpTemporal??DBNull.Value}
                };
                return dal.InsertOperacionesPorCobrar(param);
            };

        }

        #endregion

        #region SearchOperacionesPorCobrar

        public HashSet<OperacionesPorCobrar> SearchOperacionesPorCobrar(OperacionesPorCobrarRequest request)
        {
            using (IOperacionesDal<OperacionesPorCobrar> dal = new OperacionesDal<OperacionesPorCobrar>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Id_OPERACION", SqlDbType.Int){Value = (object)request.Id_OPERACION??DBNull.Value},
                    new SqlParameter("@CIREM", SqlDbType.Char, 15){Value = (object)request.CIREM??DBNull.Value},
                    new SqlParameter("@CIDES", SqlDbType.Char, 15){Value = (object)request.CIDES??DBNull.Value},
                    new SqlParameter("@Sucursal", SqlDbType.Char, 3){Value = (object)request.Sucursal??DBNull.Value},
                    new SqlParameter("@Estatus", SqlDbType.VarChar, 200){Value = (object)request.Estatus??DBNull.Value},
                     new SqlParameter("@OrderId", SqlDbType.Int){Value = (object)request.OrderId??DBNull.Value},
                };
                return dal.SearchOperacionesPorCobrar(param);
            }
        }

        #endregion

        #region DeleteOperacionesPorCobrar

        public GenericResponse DeleteOperacionesPorCobrar(OperacionesPorCobrar request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@IDS_OPERACIONES", SqlDbType.VarChar){Value = (object)request.Id_OPERACION.ToString()??DBNull.Value},
                    new SqlParameter("@NUMERO_OPERACION", SqlDbType.VarChar){Value = (object)request.NUMERO??DBNull.Value},
                    new SqlParameter("@CEDULA_CLIENTE", SqlDbType.VarChar){Value = (object)request.Cirem??DBNull.Value},
                    new SqlParameter("@ANULACION", SqlDbType.Bit){Value = (object)DBNull.Value},
                };
                return dal.DeleteOperacionesPorCobrar(param);
            }
        }

        #endregion

        #region ProcessCashierOperations

        public GenericResponse ProcessCashierOperations(ProcessCashierOperation model)
        {
            var result = new GenericResponse();
            try
            {
                #region Buscar Operacion Temporal

                OperacionesPorCobrarRequest OperaRequest = new OperacionesPorCobrarRequest
                {
                    Id_OPERACION = model.ListOperation.FirstOrDefault().Id_OPERACION,
                    CIREM = model.ListOperation.FirstOrDefault().CIREM
                };

                //Buscar la Operacion Temporal con todo sus Datos
                var OperacionTemporal = SearchOperacionesPorCobrar(OperaRequest).FirstOrDefault();

                if (OperacionTemporal == null)
                {
                    result.Error = true;
                    result.ErrorMessage = "No se ha encontrado la informacion de la Operacion De Negocio a procesar.";
                    return result;
                }

                #endregion

                #region Se Consultan datos del cliente

                SearchClientsRequest clientsRequest = new SearchClientsRequest()
                {
                    clienteTipo = OperacionTemporal.Cirem.Trim().Substring(0, 1),
                    id_cedula = OperacionTemporal.Cirem.Substring(1).Trim(),
                    offSet = 0,
                    limit = 10
                };

                var clients = builderClients.Searchfichas(clientsRequest);

                if (!clients.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "Se presenta error al tratar de consultar ficha del cliente en pago de remesa";
                    return result;
                }

                #endregion

                #region Se Carga en PagoRecibido Sucursal, Usuario y Datos Cliente
                model.ListOperation.FirstOrDefault().PagosRecibidos.SUCURSAL = model.BranchOffice;
                model.ListOperation.FirstOrDefault().PagosRecibidos.REGISTRADOPOR = model.Login.ToUpper();
                model.ListOperation.FirstOrDefault().PagosRecibidos.FICHA = clients.FirstOrDefault().FichaConsecutivo;
                model.ListOperation.FirstOrDefault().PagosRecibidos.NUMEROID = clients.FirstOrDefault().IdCedula;
                model.ListOperation.FirstOrDefault().PagosRecibidos.TIPO_IDENTIDAD = clients.FirstOrDefault().ClienteTipo;
                #endregion

                #region Busco la Lista de Paises para filtrar

                PaisesRequest paisesRequest = new PaisesRequest
                {
                    ACTIVO_ENVIO = null,
                    ID_PAIS = OperacionTemporal.Pais
                };

                var Country = builderTablasMaestras.SearchCountry(paisesRequest);

                if (!Country.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "Se presenta error al tratar de consultar pais de la orden";
                    return result;
                }

                #endregion

                #region Se Consulta de oficina pagadora

                var Sb_46_OficinasRequest = new Sb_46_OficinasRequest
                {
                    STATUS_OFICINA = 1,
                    CODIGO_CCAL = model.BranchOffice
                };

                var Oficina = builderSudeban.SearchSb_46_OficinasEntity(Sb_46_OficinasRequest).ToList();

                if (!Oficina.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "Se presenta error el tratar de consultar los datos de la oficina en pago de remesa";
                    return result;
                }

                #endregion

                #region Listas

                ProccesOrder order = new ProccesOrder()
                {
                    Ficha = clients,
                    Temporal = OperacionTemporal,
                    Country = Country
                };

                #endregion

                using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
                {
                    result = dal.ProcessCashierOperations(model, order);
                }

                result.Message = "La orden ha sido generada exitosamente";
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.Valid = false;
                result.ErrorMessage = "Ha ocurrido un error al guardar la Orden 'ProcessCashierOperations', por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }

        #endregion

        #region InsertOpPorCobrarRemesaEntrante

        public OperacionDeNegocio InsertOpPorCobrarRemesaEntrante(OperacionDeNegocio model)
        {
            GenericResponse resultInsertOpPorCobrar = null;

            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                     new SqlParameter("@Id_RemesaEntrante", SqlDbType.Int){Value = model.Id_RemesaEntrante},
                     new SqlParameter("@User", SqlDbType.Char,15){Value = model.current},
                   
                };
                resultInsertOpPorCobrar =  dal.InsertOpPorCobrarRemesaEntrante(param);
            };

            if (!resultInsertOpPorCobrar.Valid)
            {
                model.error = true;
                model.clientErrorDetail = "Error al generar la operacion: " + resultInsertOpPorCobrar.ErrorMessage;
                model.apiDetail = "InsertOpPorCobrarRemesaEntrante";
                return model;
            }

            model.IdOperacion = resultInsertOpPorCobrar.ReturnId;
            return model;  
        }

        #endregion

        #region CashierOperations

        #region SearchCashierOperations

        public HashSet<SearchCashierOperations> SearchCashierOperations(SearchCashierOperationsRequest request)
        {
            using (IOperacionesDal<SearchCashierOperations> dal = new OperacionesDal<SearchCashierOperations>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CIREM", SqlDbType.Char, 15){Value = (object)request.CIREM??DBNull.Value},
                    new SqlParameter("@CIDES", SqlDbType.Char, 15){Value = (object)request.CIDES??DBNull.Value},
                    new SqlParameter("@IDSUCURSAL", SqlDbType.Char, 15){Value = (object)request.IDSUCURSAL??DBNull.Value},
                };
                return dal.SearchCashierOperations(param);
            }
        }

        #endregion

        #region UpdateStatusCashierOperations

        public GenericResponse UpdateStatusCashierOperations(UpdateStatusCashierOperationsRequest model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Id_OPERACION", SqlDbType.SmallInt){Value= model.Id_OPERACION},
                    new SqlParameter("@Status", SqlDbType.SmallInt){Value= model.Status ??(object)DBNull.Value},
                    new SqlParameter("@Status_Temp", SqlDbType.SmallInt,50){Value= model.Status_Temp??(object)DBNull.Value},
                    new SqlParameter("@Procesado", SqlDbType.Bit){Value= model.Procesado??(object)DBNull.Value},
                    new SqlParameter("@FechaAnulacion", SqlDbType.DateTime){Value= model.FechaAnulacion ??(object)DBNull.Value},
                    new SqlParameter("@UsuarioAnula", SqlDbType.VarChar,15){Value= model.UsuarioAnula??(object)DBNull.Value},
                    new SqlParameter("@ReferenciaAnulacionBCV", SqlDbType.VarChar,50){Value= model.ReferenciaAnulacionBCV??(object)DBNull.Value},
                    new SqlParameter("@MotivoAnulacionId", SqlDbType.Int){Value= model.MotivoAnulacionId??(object)DBNull.Value},
                    new SqlParameter("@ObservacionAnulacion", SqlDbType.VarChar,500){Value= model.ObservacionAnulacion??(object)DBNull.Value},
                    new SqlParameter("@OrderId", SqlDbType.Int){Value = model.OrderId??(object)DBNull.Value},
                };
                return dal.UpdateStatusCashierOperations(param);
            }
        }
        #endregion

        #endregion

        #region InsertMixedOrder

        public GenericResponse InsertMixedOrder(ProcessCashierOperation model)
        {

            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                     new SqlParameter("@ID_OPERACION", SqlDbType.Int){Value = model.ListOperation.FirstOrDefault().Id_OPERACION},
                     new SqlParameter("@IdMetodoPago",  SqlDbType.Int){Value = model.ListPayInternationalMixed.FirstOrDefault().IdMetodoPago },
                     new SqlParameter("@TipoDocumento", SqlDbType.VarChar,1){Value = model.ListPayInternationalMixed.FirstOrDefault().TipoDocumento??(object)DBNull.Value},
                     new SqlParameter("@NroDocumento", SqlDbType.VarChar,15){Value =model.ListPayInternationalMixed.FirstOrDefault().NroDocumento??(object)DBNull.Value},
                     new SqlParameter("@NroTelefono", SqlDbType.VarChar,15){Value = model.ListPayInternationalMixed.FirstOrDefault().NroTelefono??(object)DBNull.Value},
                     new SqlParameter("@IdBanco", SqlDbType.Int){Value = model.ListPayInternationalMixed.FirstOrDefault().IdBanco},
                     new SqlParameter("@NroCuenta", SqlDbType.VarChar,20){Value = model.ListPayInternationalMixed.FirstOrDefault().NroCuenta??(object)DBNull.Value},
                     new SqlParameter("@IdMoneda", SqlDbType.Int){Value = model.ListPayInternationalMixed.FirstOrDefault().IdMoneda??(object)DBNull.Value},
                     new SqlParameter("@CodMoneda", SqlDbType.VarChar,3){Value =model.ListPayInternationalMixed.FirstOrDefault().CodMoneda??(object)DBNull.Value},
                     new SqlParameter("@Tasa", SqlDbType.Money){Value =model.ListPayInternationalMixed.FirstOrDefault().Tasa},
                     new SqlParameter("@MontoDivisa", SqlDbType.Money){Value = model.ListPayInternationalMixed.FirstOrDefault().MontoDivisa},
                     new SqlParameter("@MontoBs", SqlDbType.Money){Value  = model.ListPayInternationalMixed.FirstOrDefault().MontoBs},
                     new SqlParameter("@ModificadoPor", SqlDbType.VarChar,15){Value = model.Login},
                     
                };
                return dal.InsertMixedOrder(param);
            };

        }

        #endregion

        #region UpdateMixedOrderReferenceBCV

        public GenericResponse UpdateMixedOrderReferenceBCV(Ordenes model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN},
                    new SqlParameter("@ReferenciaBCV", SqlDbType.VarChar, 100){Value = model.REFERENCIA_ORDEN},
                };
                return dal.UpdateMixedOrderReferenceBCV(param);
            }
        }

        #endregion

        #region RegistraOperacionBCV

        public static XmlNode RegistraOperacionBCV(Ordenes orden)
        {
            InformacionFinanciera.BV_CCAL_WS ClienteWS = new InformacionFinanciera.BV_CCAL_WS();
            try
            {
                #region Variables
                bool TestMode = true;//En produccion colocar en false
                XmlNode responseBCV;
                var responseBCVInt = new XmlDocument();
                responseBCVInt.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                #endregion
                if (TestMode)
                {
                    if (orden.DETALLE_TIPO_OPERACION == Constant.TipoOperaciones.Compras)
                    {
                        responseBCV = ClienteWS.SetCompraDivisasSIMADITest(orden.TIPO_OP_BCV,
                                      orden.IDENTIFICACION_REMITENTE,
                                    orden.NOMBRES_REMITENTE,
                                    orden.MONTO,
                                    orden.TasaConversion.Value, "840", orden.MONTO,
                                    Convert.ToInt64(orden.MOTIVO_OP_BCV), string.Empty,
                                    string.IsNullOrEmpty(orden.TELEFONO_CLIENTE) ? "04141111111" : orden.TELEFONO_CLIENTE,
                                    string.IsNullOrEmpty(orden.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : orden.EMAIL_CLIENTE);
                    }
                    else //venta
                    {
                        responseBCV = ClienteWS.SetVentaDivisasSIMADITest(orden.TIPO_OP_BCV,
                                       orden.IDENTIFICACION_REMITENTE,
                                    orden.NOMBRES_REMITENTE,
                                    orden.MONTO,
                                    orden.TasaConversion.Value, "840", orden.MONTO,
                                    Convert.ToInt64(orden.MOTIVO_OP_BCV), string.Empty,
                                    string.IsNullOrEmpty(orden.TELEFONO_CLIENTE) ? "04141111111" : orden.TELEFONO_CLIENTE,
                                    string.IsNullOrEmpty(orden.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : orden.EMAIL_CLIENTE);
                    }
                }
                else
                {
                    if (orden.DETALLE_TIPO_OPERACION == Constant.TipoOperaciones.Compras)
                    {
                        responseBCV = ClienteWS.SetCompraDivisasSIMADI(orden.TIPO_OP_BCV,
                                    orden.IDENTIFICACION_REMITENTE,
                                    orden.NOMBRES_REMITENTE,
                                    orden.MONTO,
                                    orden.TasaConversion.Value, "840", orden.MONTO,
                                    Convert.ToInt64(orden.MOTIVO_OP_BCV), string.Empty,
                                    string.IsNullOrEmpty(orden.TELEFONO_CLIENTE) ? "04141111111" : orden.TELEFONO_CLIENTE,
                                    string.IsNullOrEmpty(orden.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : orden.EMAIL_CLIENTE);
                    }
                    else //venta
                    {
                        responseBCV = ClienteWS.SetVentaDivisasSIMADI(orden.TIPO_OP_BCV,
                                    orden.IDENTIFICACION_REMITENTE,
                                    orden.NOMBRES_REMITENTE,
                                    orden.MONTO,
                                    orden.TasaConversion.Value, "840", orden.MONTO,
                                    Convert.ToInt64(orden.MOTIVO_OP_BCV), string.Empty,
                                    string.IsNullOrEmpty(orden.TELEFONO_CLIENTE) ? "04141111111" : orden.TELEFONO_CLIENTE,
                                    string.IsNullOrEmpty(orden.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : orden.EMAIL_CLIENTE);
                    }
                }

                if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                {
                    //responseBCV = new XmlDocument();
                    responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                    return responseBCVInt;
                }
                return responseBCV;
            }
            catch (Exception ex)
            {
                var responseErr = new XmlDocument();
                responseErr.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                return responseErr;
            }
        }

        #endregion

        #region ProcessMixedOrder

        public GenericResponse ProcessMixedOrder(ProcessCashierOperation model)
        {
            GenericResponse _return = new GenericResponse();

            var OperaionTemporal = SearchOperacionesPorCobrar(new OperacionesPorCobrarRequest { Id_OPERACION = model.ListOperation.FirstOrDefault().Id_OPERACION });
            var Oficina = builderSudeban.SearchSb_46_Oficinas(new Sb_46_OficinasRequest { STATUS_OFICINA = 1, CODIGO_CCAL = model.BranchOffice }).FirstOrDefault();
            var OrdenPrimaria = SearchORDENES(new OrdenesRequest { ID_ORDEN = model.Orden.ID_ORDEN }).FirstOrDefault();
            ISimadiDal<List<TipoMovimientosSimadi>> dal = new SimadiDal<List<TipoMovimientosSimadi>>();
            var  TipoMovimiento = dal.SearchTipoMovimiento(new TipoMovimientoRequest { idTipoIdentidad = OrdenPrimaria.IDENTIFICACION_BENEFICIARIO.Substring(0,1), TipoOperacion = Constant.ProcesosAsociadosCajero.CodVentaEfectivo } );
            var NextNumber = new NextNumberRequest
            {
                sucursal = model.BranchOffice,
                usuario = model.Login,
                tipo = Constant.TipoNumeracion.OrdenesDePago,
                consulta = false,
                fecha = DateTime.Now
            };

            var _operationNumber = builderNumeracion.NumberNext(NextNumber);
            if (_operationNumber.Error)
            {
                return _operationNumber;
            }

            Ordenes objOrden = new Ordenes
            {
                ID_ORDEN_FK = model.Orden.ID_ORDEN,
                DETALLE_TIPO_OPERACION = Constant.TipoOperaciones.Ventas,
                CLIENTE = 0,
                SUCURSAL = Convert.ToInt32(model.IdOffice),
                STATUS_ORDEN = Constant.StatusOrden.OrdenPagada,
                MONEDA = model.ListOperation.FirstOrDefault().IdMoneda,
                OFICINA = Oficina.ID_OFIC_EXTERNA,
                PAIS_DESTINO = "VE",
                CORRESPONSAL = "CAL",
                NUMERO = Convert.ToInt32(_operationNumber.NUMERO),
                TIPO_CAMBIO = model.ListOperation.FirstOrDefault().TasaDolar,
                TASA_DESTINO = 1,
                MONTO = OperaionTemporal.FirstOrDefault().Dolares,
                MONTO_CAMBIO = OperaionTemporal.FirstOrDefault().Dolares,
                FECHA_VALOR_TASA = OrdenPrimaria.FECHA_VALOR_TASA,
                TIPO_OP_BCV = TipoMovimiento.FirstOrDefault().ID_BCV,
                NOMBRES_REMITENTE = OrdenPrimaria.NOMBRES_BENEFICIARIO,
                APELLIDOS_REMITENTE = OrdenPrimaria.APELLIDOS_BENEFICIARIO,
                IDENTIFICACION_REMITENTE = OrdenPrimaria.IDENTIFICACION_BENEFICIARIO,
                NOMBRES_BENEFICIARIO = "Casa de Cambio",
                APELLIDOS_BENEFICIARIO = "Angulo López",
                IDENTIFICACION_BENEFICIARIO = "R302075661",
                BANCO_NACIONAL = "",
                EMAIL_CLIENTE = OrdenPrimaria.EMAIL_BENEFICIARIO,
                EMAIL_BENEFICIARIO = "",
                OBSERVACIONES = (string.IsNullOrEmpty(OperaionTemporal.FirstOrDefault().Observaciones) ? string.Empty : OperaionTemporal.FirstOrDefault().Observaciones),       
                DIRECCION_BANCO = "",
                ABA = "",
                SWIFT = "",
                IBAN = "",
                TELEFONO_CLIENTE = OrdenPrimaria.TELEFONO_BENEFICIARIO,
                REGISTRADOPOR = model.Login,
                AGENTE ="CAL",
                MOTIVO_OP_BCV = OrdenPrimaria.MOTIVO_OP_BCV,
                USUARIO_TAQUILLA = model.Login,
                TasaConversion = model.ListOperation.FirstOrDefault().TasaDolar,
                MonedaOperacion = model.ListOperation.FirstOrDefault().IdMoneda,
                MontoConversion = OperaionTemporal.FirstOrDefault().Dolares,
                SucursalProcesaId = Convert.ToInt32(model.IdOffice),
                ModificadoPor = model.Login,
                Modificado = DateTime.Now,
                FECHA_OPERACION = DateTime.Now
            };

            if (model.ListPayInternationalMixed.Count()> 0)
            {
                var Banco = builderPagos.SearchDetalle_Tipos_Pago(new Detalle_Tipo_PagoRequest());
                var Banco_ = Banco.Where(x => x.ID_DETALLE == model.ListPayInternationalMixed.FirstOrDefault().IdBanco && x.TIPO_PAGO == model.ListPayInternationalMixed.FirstOrDefault().IdMetodoPago);

                objOrden.NUMERO_CUENTA = model.ListPayInternationalMixed.FirstOrDefault().NroCuenta;
                objOrden.NUMERO_CUENTA_DESTINO = model.ListPayInternationalMixed.FirstOrDefault().NroCuenta;
                objOrden.BANCO_DESTINO = Banco_.FirstOrDefault().DETALLE;
            }

            if (model.ListOperation.FirstOrDefault().Id_RemesaEntrante != null && model.ListOperation.FirstOrDefault().Id_RemesaEntrante != 0)
            {
                var responseBCV = RegistraOperacionBCV(objOrden);
                var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? String.Empty : responseBCV.SelectSingleNode("//result").InnerText;
                objOrden.REFERENCIA_ORDEN = referenciaBCV;
                var resultInsert = InsertOrdenes(objOrden);
                resultInsert.ReturnId = model.Orden.ID_ORDEN;
                if (!resultInsert.Error)
                {
                    var resultOrderPay = new GenericResponse();
                    if (model.ListPayInternationalMixed.Count() > 0)
                    {

                        OrderPaymentPendings paymentPending = new OrderPaymentPendings
                        {
                            OrderId = model.Orden.ID_ORDEN,
                            CurrencyId = Convert.ToInt32(model.ListPayInternationalMixed.FirstOrDefault().IdMoneda),
                            StatusId = Constant.Status.Activo,
                            TypePayId = Convert.ToInt32(model.ListPayInternationalMixed.FirstOrDefault().IdMetodoPago),
                            Amount = model.ListPayInternationalMixed.FirstOrDefault().MontoBs,
                            AmountCurrency = model.ListPayInternationalMixed.FirstOrDefault().MontoDivisa,
                            AccountNumber = model.ListPayInternationalMixed.FirstOrDefault().NroCuenta,
                            BankId = model.ListPayInternationalMixed.FirstOrDefault().IdBanco,
                            CreationUser = model.UserId,
                            CreationDate = DateTime.Now,
                        };

                        resultOrderPay = builderPagos.InsertOrderPaymentPending(paymentPending);
                    }

                    if (resultOrderPay.Error)
                    {
                        return resultOrderPay;
                    }

                    _return = resultInsert;
                   _return.Valid= true;
                }
            }
            return _return;
        }
        #endregion

        #endregion

        #region SearchOrdenesTransmitirPendientes

        public HashSet<OrdenSalienteExterno> SearchOrdenesTransmitirPendientes(string corresponsal)
        {
            using (IOperacionesDal<OrdenSalienteExterno> dal = new OperacionesDal<OrdenSalienteExterno>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@corresponsal", SqlDbType.Char,3){Value= corresponsal??(object)DBNull.Value},
                };
                return dal.SearchOrdenesTransmitirPendientes(param);
            }
        }
        #endregion

        public GenericResponse UpdateOrdenSalienteTransmitida(int id)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = id},
                };
                return dal.UpdateOrdenSalienteTransmitida(param);
            }
        }

        #region SearchOrdenesTransmitirPendientes

        public HashSet<OrdenSalienteExterno> SearchOrdenesSalientesAnularPendientes(string corresponsal)
        {
            using (IOperacionesDal<OrdenSalienteExterno> dal = new OperacionesDal<OrdenSalienteExterno>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@corresponsal", SqlDbType.Char,3){Value= corresponsal??(object)DBNull.Value},
                };
                return dal.SearchOrdenesSalientesAnularPendientes(param);
            }
        }
        #endregion

        #endregion

        #region Tarifas Aplicadas

        #region SearchTarifasAplicadas

        public HashSet<SearchTarifas_Aplicadas> SearchTarifasAplicadas(Tarifas_Aplicadas model)
        {
            using (IOperacionesDal<SearchTarifas_Aplicadas> dal = new OperacionesDal<SearchTarifas_Aplicadas>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ORDEN", SqlDbType.Int){Value = model.ORDEN??(object)DBNull.Value},
                    new SqlParameter("@TEMPORAL", SqlDbType.Int){Value = model.TEMPORAL??(object)DBNull.Value},
                    new SqlParameter("@idOpNacional", SqlDbType.Int){Value = model.idOpNacional??(object)DBNull.Value},
                    new SqlParameter("@SOLICITUD", SqlDbType.Int){Value = model.SOLICITUD??(object)DBNull.Value}
                };


                return dal.SearchTarifasAplicadas(param);
            }

        }

        #endregion

        #region UpdateOrdenTarifasAplicadas

        public GenericResponse UpdateOrdenTarifasAplicadas(Tarifas_Aplicadas model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@temporal", SqlDbType.Int){Value = model.TEMPORAL},
                    new SqlParameter("@orden", SqlDbType.Int){Value = model.ORDEN??(object)DBNull.Value},

                };
                return dal.UpdateOrdenTarifasAplicadas(param);
            }
        }

        #endregion

        #endregion

        #region Anulaciones

        #region SearchOperationsCashierAnnulment

        public HashSet<OrdenAnulables> SearchOperationsCashierAnnulment(OperationsCashierAnnulmentRequest model)
        {
            using (IOperacionesDal<OrdenAnulables> dal = new OperacionesDal<OrdenAnulables>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = model.ID_ORDEN??(object)DBNull.Value},
                    new SqlParameter("@CIREM", SqlDbType.VarChar,15){Value = model.CIREM??(object)DBNull.Value},
                    new SqlParameter("@SUCURSAL", SqlDbType.Int){Value = model.SUCURSAL??(object)DBNull.Value},
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.VarChar,15){Value = model.REGISTRADOPOR??(object)DBNull.Value}
                };


                return dal.SearchOperationsCashierAnnulment(param);
            }

        }

        #endregion

        #region SearchOperationsTempAnnulment

        public HashSet<SearchCashierOperations> SearchOperationsTempAnnulment(OperationsTempAnnulmentRequest model)
        {
            using (IOperacionesDal<SearchCashierOperations> dal = new OperacionesDal<SearchCashierOperations>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CIREM", SqlDbType.VarChar,15){Value = model.CIREM??(object)DBNull.Value},
                    new SqlParameter("@CIDES", SqlDbType.VarChar,15){Value = model.CIDES??(object)DBNull.Value},
                    new SqlParameter("@IDSUCURSAL", SqlDbType.Int){Value = model.IdSucursal??(object)DBNull.Value},
                    new SqlParameter("@USUARIO", SqlDbType.VarChar,10){Value = model.USUARIO??(object)DBNull.Value}
                    
                };


                return dal.SearchOperationsTempAnnulment(param);
            }

        }

        #endregion

        #region SearchAnnulment

        public HashSet<Annulment> SearchAnnulment(AnnulmentRequest model)
        {
            using (IOperacionesDal<Annulment> dal = new OperacionesDal<Annulment>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId??(object)DBNull.Value},
                    new SqlParameter("@TableId", SqlDbType.Int){Value = model.TableId??(object)DBNull.Value},
                    new SqlParameter("@RowId", SqlDbType.Int){Value = model.RowId??(object)DBNull.Value},
                    new SqlParameter("@AnnulmentId", SqlDbType.Int){Value = model.AnnulmentId??(object)DBNull.Value}

                };


                return dal.SearchAnnulment(param);
            }

        }

        #endregion

        #region SearchAnnulmentPending


        public HashSet<AnnulmentPending> SearchAnnulmentPending(AnnulmentPendingRequest model)
        {
            using (IOperacionesDal<AnnulmentPending> dal = new OperacionesDal<AnnulmentPending>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@BranchOfficeId", SqlDbType.Int){Value = model.BranchOfficeId},
                };
                return dal.SearchAnnulmentPending(param);
            }

        }

        #endregion

        #region InsertAnnulment

        public GenericResponse InsertAnnulment(Annulment model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@StatusId", SqlDbType.Int){Value = model.StatusId},
                    new SqlParameter("@TableId", SqlDbType.Int){Value = model.TableId},
                    new SqlParameter("@RowId", SqlDbType.Int){Value = model.RowId},
                    new SqlParameter("@StatusRowId", SqlDbType.Int){Value = model.StatusRowId??(object)DBNull.Value},
                    new SqlParameter("@CreationUser", SqlDbType.Int){Value = model.CreationUserId},
                    new SqlParameter("@ReasonAnnulmentId", SqlDbType.Int){Value = model.ReasonAnnulmentId},
                    new SqlParameter("@AnnulmentObservation", SqlDbType.VarChar,500){Value = model.AnnulmentObservation},
                  
                };
                return dal.InsertAnnulment(param);
            }
        }

        #endregion

        #region InsertAnnulmentOperationTemp

        public GenericResponse InsertAnnulmentOperationTemp(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            GenericResponse ResultStatusUpdateRemesaEntrante = new GenericResponse();
            GenericResponse ResultUpdateStatus = new GenericResponse();

            try
            {
                //Se busca la operacion temporal en BDD
                var Operacion = SearchOperacionesPorCobrar(new OperacionesPorCobrarRequest { Id_OPERACION = model.RowId });

                //Request para actualizar el estatus de la operacion temporal
                UpdateStatusCashierOperationsRequest UpdateRequest = new UpdateStatusCashierOperationsRequest
                { Id_OPERACION = model.RowId, Status = Constant.StatusOperacionesTemporales.PendienteAnulacion };

                #region Actualizar Status de Operacion

                //Verfico que se encontro la operacion
                if (Operacion.Count > 0)
                {
                    //Verifico si la operacion es una Remesa Entrante
                    if (Operacion.FirstOrDefault().Id_RemesaEntrante != null)
                    {
                          //Actualiza el Estatus de la Remesa Entrante
                    }
                    //Actualiza el Estatus de Operacion Temporal
                    ResultUpdateStatus = UpdateStatusCashierOperations(UpdateRequest);
                }
                else
                {
                    //No se encuentra la operacion
                    genericResponse.ReturnId = 0;
                    genericResponse.Valid = false;
                    genericResponse.Message = "No se pudo encontrar la informacion de la Operacion. No se puede generar la Solicitud de Anulacion";
                    return (genericResponse);
                }

                //Validar La respuesta de Actualizacion de estatus de la operacion
                if (!ResultUpdateStatus.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultUpdateStatus.ErrorMessage + ". No se ha podido actualizar el estatus de la operacion.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }
                #endregion

                #region Insert Anulacion
                genericResponse = InsertAnnulment(model);

                if (!genericResponse.Valid)
                {
                    var ResultRollBackStatus = UpdateStatusCashierOperations(new UpdateStatusCashierOperationsRequest { Id_OPERACION = model.RowId, Status = model.StatusRowId });
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = genericResponse.ErrorMessage + ". No se ha podido generar la solicitud de Anulacion";
                    return genericResponse;
                }


                #endregion

                genericResponse.Valid = true;
                genericResponse.Message = "La Solicitud de Anulacion ha sido generada exitosamente";
                return genericResponse;
            }
            catch (Exception ex)
            {
                var ResultRollBackStatus = UpdateStatusCashierOperations(new UpdateStatusCashierOperationsRequest { Id_OPERACION = model.RowId, Status = model.StatusRowId });
                genericResponse.ErrorMessage = "Se a producido un error al intertar Procesar la solicitud de anulacion";
                genericResponse.Error = true;
            }
            return genericResponse;
        }

        #endregion

        #region InsertAnnulmentOrder

        public GenericResponse InsertAnnulmentOrder(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            GenericResponse ResultStatusUpdateRemesaEntrante = new GenericResponse();
            GenericResponse ResultStatusUpdateOrder = new GenericResponse();
            StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();
            try
            {
                statusAnnulOrderRequest.STATUS_ORDEN = builderTablasMaestras.SearchStatusOrdenes(new StatusOrdenesRequest { entrada = false }).Where(x => x.STATUS_ORDEN == "Pendiente Anulacion").FirstOrDefault().ID_STATUS;
                statusAnnulOrderRequest.ID_ORDEN = model.RowId;
                statusAnnulOrderRequest.ModificadoPor = model.CreationUser;
                statusAnnulOrderRequest.Modificado = DateTime.Now;

                if (model.TableId != Constant.Table.Ordenes)
                {
                    statusAnnulOrderRequest.STATUS_ORDEN = builderTablasMaestras.SearchStatusOrdenes(new StatusOrdenesRequest { entrada = true }).Where(x => x.STATUS_ORDEN == "Pendiente Anulacion").FirstOrDefault().ID_STATUS;
                }

                    ResultStatusUpdateOrder = UpdateStatusAnnulmentOrder(statusAnnulOrderRequest);

                //Validar La respuesta de Actualizacion de estatus de la Orden
                if (!ResultStatusUpdateOrder.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultStatusUpdateOrder.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }

                #region Insert Anulacion
                genericResponse = InsertAnnulment(model);

                if (!genericResponse.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = genericResponse.ErrorMessage + ". No se ha podido generar la solicitud de Anulacion";
                    return genericResponse;
                }
                #endregion

                genericResponse.Valid = true;
                genericResponse.Message = "La Solicitud de Anulacion ha sido generada exitosamente";
                return genericResponse;
            }
            catch (Exception ex)
            {
                genericResponse.ErrorMessage = "Se a producido un error al intertar Procesar la solicitud de anulacion";
                genericResponse.Error = true;
            }
            return genericResponse;
        }

        #endregion

        #region UpdateStatusAnnulmentOrder

        public GenericResponse UpdateStatusAnnulmentOrder(StatusAnnulOrderRequest model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value= model.ID_ORDEN},
                    new SqlParameter("@STATUS_ORDEN", SqlDbType.Int){Value= model.STATUS_ORDEN},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar,15){Value= model.ModificadoPor??(object)DBNull.Value},
                    new SqlParameter("@Modificado", SqlDbType.DateTime){Value= model.Modificado??(object)DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION", SqlDbType.DateTime){Value= model.FECHA_ANULACION ??(object)DBNull.Value},
                    new SqlParameter("@AnuladaPor", SqlDbType.VarChar,15){Value= model.AnuladaPor??(object)DBNull.Value},
                    new SqlParameter("@ReferenciaAnulBcv", SqlDbType.VarChar,50){Value= model.ReferenciaAnulBcv??(object)DBNull.Value},
                    new SqlParameter("@MotivoAnulacionId", SqlDbType.Int){Value= model.MotivoAnulacionId??(object)DBNull.Value},
                    new SqlParameter("@ObservacionesAnulacion", SqlDbType.VarChar,500){Value= model.ObservacionesAnulacion??(object)DBNull.Value},
                };
                return dal.UpdateStatusAnnulmentOrder(param);
            }
        }

        #endregion

        #region UpdateStatusAnnulmentRemesaEntrante

        public GenericResponse UpdateStatusAnnulmentRemesaEntrante(StatusAnnulRemesaEntranteRequest model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Id_OPERACION", SqlDbType.Int){Value= model.ID_OPERACION},
                    new SqlParameter("@STATUS", SqlDbType.Int){Value= model.STATUS},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar,15){Value= model.ModificadoPor??(object)DBNull.Value},
                    new SqlParameter("@Modificado", SqlDbType.DateTime){Value= model.Modificado??(object)DBNull.Value},
                    new SqlParameter("@FechaAnulacion", SqlDbType.DateTime){Value= model.FechaAnulacion ??(object)DBNull.Value},
                    new SqlParameter("@UsuarioAnula", SqlDbType.VarChar,15){Value= model.UsuarioAnula??(object)DBNull.Value},
                    new SqlParameter("@MotivoAnulacionId", SqlDbType.Int){Value= model.MotivoAnulacionId??(object)DBNull.Value},
                    new SqlParameter("@ObservacionAnulacion", SqlDbType.VarChar,500){Value= model.ObservacionAnulacion??(object)DBNull.Value},
                };
                return dal.UpdateStatusAnnulmentRemesaEntrante(param);
            }
        }

        #endregion

        #region UpdateAnnulment

        public GenericResponse UpdateAnnulment(Annulment model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@StatusId", SqlDbType.Int){Value= model.StatusId},
                    new SqlParameter("@TableId", SqlDbType.Int){Value= model.TableId},
                    new SqlParameter("@RowId", SqlDbType.VarChar,15){Value= model.RowId},
                    new SqlParameter("@UpdateUser", SqlDbType.Int){Value= model.UpdateUser??(object)DBNull.Value},
                    new SqlParameter("@UpdateDate", SqlDbType.DateTime){Value= model.UpdateDate ??(object)DBNull.Value},
                    new SqlParameter("@AnnulmentObservation", SqlDbType.VarChar,15){Value= model.AnnulmentObservation??(object)DBNull.Value},
                    new SqlParameter("@AnnulmentId", SqlDbType.Int){Value= model.AnnulmentId},
                };
                return dal.UpdateAnnulment(param);
            }
        }

        #endregion

        #region ValidateAproveAnnulment

        public GenericResponse ValidateAproveAnnulment(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();
            try
            {
                //Verifica si la Solicitud es Aprobada
                if (model.StatusId == Constant.StatusMotivosAnulacion.Aprobada)
                {
                    #region APROBAR ANULACION
                    //Verifico si es Opercion Temporal
                    if (model.TableId == Constant.Table.OperacionesPorCobrar)
                    {
                        var result = AproveAnnulOpTemp(model);
                        if (!result.Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = result.ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                            return genericResponse;
                        }

                    }
                    else if (model.TableId == Constant.Table.Ordenes)
                    {
                        var result = AproveAnnulOrder(model);
                        if (!result.Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = result.ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                            return genericResponse;
                        }
                    }
                    else
                    {
                        var result = AproveAnnulRemesaEntrante(model);
                        if (!result.Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = result.ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                            return genericResponse;
                        }
                    }

                    #endregion
                }
                else
                {
                    #region RECHAZAR ANULACION  

                    if (model.TableId == Constant.Table.OperacionesPorCobrar)
                    {
                        if (!RejectAnnulOpTemp(model).Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = RejectAnnulOpTemp(model).ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                            return genericResponse;
                        }
                    }
                    else if (model.TableId == Constant.Table.Ordenes)
                    {
                        if (!RejectAnnulOrder(model).Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = RejectAnnulOrder(model).ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                            return genericResponse;
                        }
                    }
                    else
                    {
                        if (!RejectAnnulRemensaEntrante(model).Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = RejectAnnulRemensaEntrante(model).ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                            return genericResponse;
                        }
                    }
                    #endregion
                }

                #region Actualizar Status de Solicitud de Anulacion Aprobar/Rechazar

                model.UpdateDate = DateTime.Now;
                //Actualiza la Solicitud de Anulacion 
                genericResponse = UpdateAnnulment(model);

                #endregion

                if (!genericResponse.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = genericResponse.ErrorMessage + ". No se ha podido procesar la solicitud de Anulacion";
                    return genericResponse;
                }
                genericResponse.Valid = true;
                genericResponse.Message = "La Solicitud de Anulacion ha sido procesada exitosamente";
                return genericResponse;
            }
            catch (Exception ex)
            {
                genericResponse.ErrorMessage = "Se a producido un error al intertar Procesar la solicitud de anulacion";
                genericResponse.Error = true;
            }
            return genericResponse;
        }

        #endregion

        #region AproveAnnulOpTemp

        private GenericResponse AproveAnnulOpTemp(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            try
            { 
            var Operacion = SearchOperacionesPorCobrar(new OperacionesPorCobrarRequest { Id_OPERACION = model.RowId }).FirstOrDefault();

            //Verifica si la operacion Temporal esta asociada a una Remesa Entrante
            if (Operacion.Id_RemesaEntrante != null && Operacion.Id_RemesaEntrante != 0)
            {
               
               var Id_Operacion = model.RowId;
                model.RowId = Convert.ToInt32(Operacion.Id_RemesaEntrante);
               //Se habilita nuevamente la Remesa Entrante al Estatus Disponilble para pagos
               var result = AproveEnableRemesaEntrante(model);
                if (result.Error)
                {
                    return result;
                }

                model.RowId = Id_Operacion;
            }

            var TipoAnulacion = Constant.TipoOperacionBCV.AnulacionVentaDivisa;
            string tipoIdentificacion = Operacion.Cirem.Substring(0, 1);
            string numeroIdentificacion = Operacion.Cirem.Replace(tipoIdentificacion, "");
            if (Operacion.Tipo_Operacion != Constant.TipoOperaciones.Ventas)
            {
                TipoAnulacion = Constant.TipoOperacionBCV.AnulacionCompraDivisa;
            }

            //Anulacion de BCV
            var ResultAnnulmentOpBCV = AnnulmentOpBCV(TipoAnulacion, Operacion.Cadivi, tipoIdentificacion, numeroIdentificacion);

            //Request para Actulizar Estatus de Anulacion en Operacion Temporal
            UpdateStatusCashierOperationsRequest UpdateRequest = new UpdateStatusCashierOperationsRequest
            {
                Id_OPERACION = model.RowId,
                Status = Constant.StatusOperacionesTemporales.Anulada,
                MotivoAnulacionId = model.ReasonAnnulmentId,
                UsuarioAnula = model.CreationUser,
                ReferenciaAnulacionBCV = ResultAnnulmentOpBCV.Message,
                FechaAnulacion = DateTime.Now,
                ObservacionAnulacion = model.AnnulmentObservation
            };

            //Actualiza el Estatus de Operacion Temporal
            var ResultUpdateStatusOpTemp = UpdateStatusCashierOperations(UpdateRequest);

            return ResultUpdateStatusOpTemp;
            }
            catch (Exception ex)
            {

                genericResponse.Error = true;
                genericResponse.ErrorMessage = string.Concat("Se ha presentado un error en el proceso de anulacion de la Operacion de Negocio",
                           ex.Message, ". Por favor notificar al administrador del sistema.");

                return genericResponse;
            }

        }
        #endregion

        #region AproveAnnulOrder

        private GenericResponse AproveAnnulOrder(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            try
            {
                StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();
                CashierRequest cashierRequest = new CashierRequest();
                cashierRequest.RowId = model.RowId;
            
                var Orden = SearchORDENES(new OrdenesRequest { ID_ORDEN = model.RowId }).FirstOrDefault();
                var OperacionTemporal = SearchOperacionesPorCobrar(new OperacionesPorCobrarRequest { OrderId = model.RowId }).ToList();
                var BranchOffice = builderSudeban.SearchSb_46_Oficinas(new Sb_46_OficinasRequest()).Where(x=>x.ID_OFICINA == Orden.SUCURSAL).FirstOrDefault();
               var TipoAnulacion = Constant.TipoOperacionBCV.AnulacionVentaDivisa;
                string tipoIdentificacion = Orden.IDENTIFICACION_REMITENTE.Substring(0, 1);
                string numeroIdentificacion = Orden.IDENTIFICACION_REMITENTE.Replace(tipoIdentificacion, "");
                if (Orden.DETALLE_TIPO_OPERACION != Constant.TipoOperaciones.Ventas)
                {
                    TipoAnulacion = Constant.TipoOperacionBCV.AnulacionCompraDivisa;
                }

                //Anulacion de BCV
                var ResultAnnulmentOpBCV = AnnulmentOpBCV(TipoAnulacion, Orden.REFERENCIA_ORDEN, tipoIdentificacion, numeroIdentificacion);   

                statusAnnulOrderRequest.STATUS_ORDEN = 9; //Orden Anulada Taquilla
                statusAnnulOrderRequest.ID_ORDEN = model.RowId;
                statusAnnulOrderRequest.Modificado = DateTime.Now;
                statusAnnulOrderRequest.ModificadoPor = model.CreationUser;
                statusAnnulOrderRequest.FECHA_ANULACION = DateTime.Now;
                statusAnnulOrderRequest.AnuladaPor = model.CreationUser;
                statusAnnulOrderRequest.ReferenciaAnulBcv = ResultAnnulmentOpBCV.Message;
                statusAnnulOrderRequest.MotivoAnulacionId = model.ReasonAnnulmentId;
                statusAnnulOrderRequest.ObservacionesAnulacion = model.AnnulmentObservation;

                var ResultStatusUpdateOrder = UpdateStatusAnnulmentOrder(statusAnnulOrderRequest);

                //Validar La respuesta de Actualizacion de estatus de la Orden
                if (!ResultStatusUpdateOrder.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultStatusUpdateOrder.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }
            
                //Se busca los Pagos Rebidos
                SearchDetallePagoRecibidosRequest searchDetallePagoRecibidosRequest = new SearchDetallePagoRecibidosRequest { BranchId = BranchOffice.CODIGO_CCAL.Trim() ,OrderNumber= Orden.NUMERO.ToString()};
                var pagos = builderIngresos.SearchDetallePagosRecibidos(searchDetallePagoRecibidosRequest);

                if (pagos!= null)
                {
                    if (pagos.Count() > 0)
                    {
                        //Anular Pagos Recibidos de la operacion
                        var ResultPagosAnnulment = builderIngresos.UpdateIngresosOrdenAnulada(new UpdateIngresosOrdenAnnulmentRequest { id_pago = pagos.FirstOrDefault().ID_PAGO, usuario = model.CreationUser });
                        if (!ResultPagosAnnulment.Valid)
                        {
                            genericResponse.Error = true;
                            genericResponse.ErrorMessage = ResultPagosAnnulment.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                            return genericResponse;
                        }
                    }
                }

                //Anular Pagos Realizados de operacion 
                var resultPagosRealizados = builderEgreso.UpdateEgresosOrdenAnulada(new UpdateEgresosOrdenAnnulmentRequest { id_pago = model.RowId, usuario = model.CreationUser });
                if (!resultPagosRealizados.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = resultPagosRealizados.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }


                #region Reversar Movientos de Cajero

                var ListCashier = builderContabilidad.SearchCashier(cashierRequest);

            var datosSummary = (from x in ListCashier.ToList()
                         group x by new
                         {
                             x.Currency,
                             x.DenominationId,
                             x.Amount,
                             x.Quantity,
                             x.BranchOffice,
                             x.UserID,
                             x.OriginOperation,
                             x.CreationUser
                         } into grupo
                         select new CashierSummarys
                         {
                             BranchOffice = grupo.Key.BranchOffice,
                             Currency = grupo.Key.Currency,
                             UserId = grupo.Key.UserID,
                             DenominationId = grupo.Key.DenominationId,
                             Quantity = grupo.Key.Quantity,
                             Amount = grupo.Key.Amount,
                             CreationDate = DateTime.Now,
                             OriginOperation = grupo.Key.OriginOperation,
                             CreationUser = grupo.Key.CreationUser,
                         }).ToList();

                if (OperacionTemporal.Count() > 0)
                {
                    cashierRequest.RowId = OperacionTemporal.FirstOrDefault().Id_OPERACION;
                }

            var ListCashierInClient = builderContabilidad.SearchCashier(cashierRequest).ToList();
            List<Cashiers> Cashier = new List<Cashiers>();
            List<CashierSummarys> CashierSummary = new List<CashierSummarys>();

            Cashier.AddRange(ListCashier);
            CashierSummary.AddRange(datosSummary);
            List<CashierSummarys> datosSummaryInClient = null ;
            if (ListCashierInClient.Count() > 0)
            {
               datosSummaryInClient = (from x in (ListCashierInClient.Where(x => x.OriginOperation == Constant.OperacionesCaja.Entrada && x.TypeOperaction == Constant.OperacionesCaja.IngresoEfecClient).ToList())
                                        group x by new
                                        {
                                            x.Currency,
                                            x.DenominationId,
                                            x.Amount,
                                            x.Quantity,
                                            x.BranchOffice,
                                            x.UserID,
                                            x.OriginOperation,
                                            x.CreationUser
                                        } into grupo
                                        select new CashierSummarys
                                        {
                                            BranchOffice = grupo.Key.BranchOffice,
                                            Currency = grupo.Key.Currency,
                                            UserId = grupo.Key.UserID,
                                            DenominationId = grupo.Key.DenominationId,
                                            Quantity = grupo.Key.Quantity,
                                            Amount = grupo.Key.Amount,
                                            CreationDate = DateTime.Now,
                                            OriginOperation = grupo.Key.OriginOperation,
                                            CreationUser = grupo.Key.CreationUser,
                                        }).ToList();

                    Cashier.AddRange(ListCashierInClient);
                    CashierSummary.AddRange(datosSummaryInClient);
                }



                foreach (var item in Cashier)
                {
                    var resultRollback = builderContabilidad.RollBackCashier(item.CashierId, null);
                }

                foreach (var item in CashierSummary)
                {
                    var resultRollback = builderContabilidad.RollBackCashierSummary(item);
                }

                #endregion


                //Request para Actulizar Estatus de Anulacion en Operacion Temporal
                UpdateStatusCashierOperationsRequest UpdateRequest = new UpdateStatusCashierOperationsRequest
                {
                    Id_OPERACION = model.RowId,
                    Status = Constant.StatusOperacionesTemporales.Anulada,
                    MotivoAnulacionId = model.ReasonAnnulmentId,
                    UsuarioAnula = model.CreationUser,
                    ReferenciaAnulacionBCV = ResultAnnulmentOpBCV.Message,
                    FechaAnulacion = DateTime.Now,
                    ObservacionAnulacion = model.AnnulmentObservation
                };

                //Actualiza el Estatus de Operacion Temporal
                var ResultUpdateStatusOpTemp = UpdateStatusCashierOperations(UpdateRequest);

                return ResultStatusUpdateOrder;
            }
            catch (Exception ex)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = string.Concat("Se ha presentado un error en el proceso de anulacion de la orden",
                           ex.Message, ". Por favor notificar al administrador del sistema.");

                return genericResponse;
            }
        }
        #endregion

        #region AproveAnnulRemesaEntrante

        private GenericResponse AproveAnnulRemesaEntrante(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            try
            {      
                StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();
                StatusAnnulOrderRequest statusAnnulOrderSecundariaRequest = new StatusAnnulOrderRequest();
                StatusAnnulRemesaEntranteRequest statusAnnulRemesaEtrnateRequest = new StatusAnnulRemesaEntranteRequest();
                CashierRequest cashierRequest = new CashierRequest();
                cashierRequest.RowId = model.RowId;         
                var OperacionTemporal = SearchOperacionesPorCobrar(new OperacionesPorCobrarRequest { OrderId = model.RowId }).ToList();
         
                //Busco la Orden Primaria
                    var Orden = SearchORDENES(new OrdenesRequest { ID_ORDEN = model.RowId }).FirstOrDefault();
                //Busco la Orden Secundaria
                var Orden_Secundaria = SearchORDENES(new OrdenesRequest { ID_ORDEN_FK = model.RowId }).FirstOrDefault();

                    var TipoAnulacion = Constant.TipoOperacionBCV.AnulacionVentaDivisa;
                    string tipoIdentificacion = Orden.IDENTIFICACION_REMITENTE.Substring(0, 1);
                    string numeroIdentificacion = Orden.IDENTIFICACION_REMITENTE.Replace(tipoIdentificacion, "");


                    if (Orden.DETALLE_TIPO_OPERACION != Constant.TipoOperaciones.Ventas)
                    {
                        TipoAnulacion = Constant.TipoOperacionBCV.AnulacionCompraDivisa;
                    }
                    //Anulacion de BCV Orden Primaria
                    var ResultAnnulmentOpBCV = AnnulmentOpBCV(TipoAnulacion, Orden.REFERENCIA_ORDEN, tipoIdentificacion, numeroIdentificacion);

                    //Datos para Anular la Orden Primaria
                    statusAnnulOrderRequest.STATUS_ORDEN = 40;
                    statusAnnulOrderRequest.ID_ORDEN = Orden.ID_ORDEN;
                    statusAnnulOrderRequest.Modificado = DateTime.Now;
                    statusAnnulOrderRequest.ModificadoPor = model.CreationUser;
                    statusAnnulOrderRequest.FECHA_ANULACION = DateTime.Now;
                    statusAnnulOrderRequest.AnuladaPor = model.CreationUser;
                    statusAnnulOrderRequest.ReferenciaAnulBcv = ResultAnnulmentOpBCV.Message;
                    statusAnnulOrderRequest.MotivoAnulacionId = model.ReasonAnnulmentId;
                    statusAnnulOrderRequest.ObservacionesAnulacion = model.AnnulmentObservation;

                    if (Orden_Secundaria.DETALLE_TIPO_OPERACION == Constant.TipoOperaciones.Ventas)
                    {
                        TipoAnulacion = Constant.TipoOperacionBCV.AnulacionVentaDivisa;
                    }

                    //Anulacion de BCV Orden Secundaria
                    var ResultAnnulmentOpBCVSEcundaria = AnnulmentOpBCV(TipoAnulacion, Orden_Secundaria.REFERENCIA_ORDEN, tipoIdentificacion, numeroIdentificacion);

                    //Datos para Anular la Orden SEcundaria
                    statusAnnulOrderSecundariaRequest.STATUS_ORDEN = 9;
                    statusAnnulOrderSecundariaRequest.ID_ORDEN = Orden_Secundaria.ID_ORDEN;
                    statusAnnulOrderSecundariaRequest.Modificado = DateTime.Now;
                    statusAnnulOrderSecundariaRequest.ModificadoPor = model.CreationUser;
                    statusAnnulOrderSecundariaRequest.FECHA_ANULACION = DateTime.Now;
                    statusAnnulOrderSecundariaRequest.AnuladaPor = model.CreationUser;
                    statusAnnulOrderSecundariaRequest.ReferenciaAnulBcv = ResultAnnulmentOpBCVSEcundaria.Message;
                    statusAnnulOrderSecundariaRequest.MotivoAnulacionId = model.ReasonAnnulmentId;
                    statusAnnulOrderSecundariaRequest.ObservacionesAnulacion = model.AnnulmentObservation;

                //Actualizacion de Status de La Orden Primaria Anulada
                var ResultStatusUpdateOrder = UpdateStatusAnnulmentOrder(statusAnnulOrderRequest);

                if (!ResultStatusUpdateOrder.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultStatusUpdateOrder.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden Primaria.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }

                //Actualizacion de Status de La Orden Secundaria Anulada
                var ResultStatusUpdateOrderSecond = UpdateStatusAnnulmentOrder(statusAnnulOrderSecundariaRequest);

                if (!ResultStatusUpdateOrderSecond.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultStatusUpdateOrder.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden Secundaria .No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }

                //Anular Pagos Realizados de operacion 
                var resultPagosRealizados = builderEgreso.UpdateEgresosOrdenAnulada(new UpdateEgresosOrdenAnnulmentRequest { id_pago = model.RowId, usuario = model.CreationUser });
                if (!resultPagosRealizados.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = resultPagosRealizados.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }

                //Datos para Anular La Remesa Entrante Asociada
                statusAnnulRemesaEtrnateRequest.STATUS = 40;
                statusAnnulRemesaEtrnateRequest.ID_OPERACION =Convert.ToInt32(OperacionTemporal.FirstOrDefault().Id_RemesaEntrante);
                statusAnnulRemesaEtrnateRequest.ModificadoPor = model.CreationUser;
                statusAnnulRemesaEtrnateRequest.MotivoAnulacionId = 2;
                statusAnnulRemesaEtrnateRequest.Modificado = DateTime.Now;
                statusAnnulRemesaEtrnateRequest.FechaAnulacion = DateTime.Now;
                statusAnnulRemesaEtrnateRequest.UsuarioAnula = model.CreationUser;
                statusAnnulRemesaEtrnateRequest.ObservacionAnulacion = model.AnnulmentObservation;

                //Actualizo el Estatus de la Remesa Entrante
                var ResultStatusUpdateRemesaEntrante = UpdateStatusAnnulmentRemesaEntrante(statusAnnulRemesaEtrnateRequest);

                if (!ResultStatusUpdateRemesaEntrante.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultStatusUpdateRemesaEntrante.ErrorMessage + ". No se ha podido actualizar el estatus de la Remesa Entrante.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }

                #region Reversar Movientos de Cajero

                var ListCashier = builderContabilidad.SearchCashier(cashierRequest);

                var datosSummary = (from x in ListCashier.ToList()
                                    group x by new
                                    {
                                        x.Currency,
                                        x.DenominationId,
                                        x.Amount,
                                        x.Quantity,
                                        x.BranchOffice,
                                        x.UserID,
                                        x.OriginOperation,
                                        x.CreationUser
                                    } into grupo
                                    select new CashierSummary
                                    {
                                        BranchOffice = grupo.Key.BranchOffice,
                                        Currency = grupo.Key.Currency,
                                        UserId = grupo.Key.UserID,
                                        DenominationId = grupo.Key.DenominationId,
                                        Quantity = grupo.Key.Quantity,
                                        Amount = grupo.Key.Amount,
                                        CreationDate = DateTime.Now,
                                        OriginOperation = grupo.Key.OriginOperation,
                                        CreationUser = grupo.Key.CreationUser,
                                    }).ToList();

                if (OperacionTemporal.Count() > 0)
                {
                    cashierRequest.RowId = OperacionTemporal.FirstOrDefault().Id_OPERACION;
                }

                var ListCashierInClient = builderContabilidad.SearchCashier(cashierRequest).ToList();
                List<Cashier> Cashier = new List<Cashier>();
                List<CashierSummary> CashierSummary = new List<CashierSummary>();

                Cashier.AddRange(ListCashier);
                CashierSummary.AddRange(datosSummary);
                List<CashierSummary> datosSummaryInClient = null;
                if (ListCashierInClient.Count() > 0)
                {
                    datosSummaryInClient = (from x in (ListCashierInClient.Where(x => x.OriginOperation == Constant.OperacionesCaja.Entrada && x.TypeOperaction == Constant.OperacionesCaja.IngresoEfecClient).ToList())
                                            group x by new
                                            {
                                                x.Currency,
                                                x.DenominationId,
                                                x.Amount,
                                                x.Quantity,
                                                x.BranchOffice,
                                                x.UserID,
                                                x.OriginOperation,
                                                x.CreationUser
                                            } into grupo
                                            select new CashierSummary
                                            {
                                                BranchOffice = grupo.Key.BranchOffice,
                                                Currency = grupo.Key.Currency,
                                                UserId = grupo.Key.UserID,
                                                DenominationId = grupo.Key.DenominationId,
                                                Quantity = grupo.Key.Quantity,
                                                Amount = grupo.Key.Amount,
                                                CreationDate = DateTime.Now,
                                                OriginOperation = grupo.Key.OriginOperation,
                                                CreationUser = grupo.Key.CreationUser,
                                            }).ToList();

                    Cashier.AddRange(ListCashierInClient);
                    CashierSummary.AddRange(datosSummaryInClient);
                }



                foreach (var item in Cashier)
                {
                    var resultRollback = builderContabilidad.RollBackCashier(item.CashierId, null);
                }

                foreach (var item in CashierSummary)
                {
                    var resultRollback = builderContabilidad.RollBackCashierSummary(item);
                }

                #endregion

                //Request para Actulizar Estatus de Anulacion en Operacion Temporal
                UpdateStatusCashierOperationsRequest UpdateRequest = new UpdateStatusCashierOperationsRequest
                {
                    Id_OPERACION = OperacionTemporal.FirstOrDefault().Id_OPERACION,
                    Status = Constant.StatusOperacionesTemporales.Anulada,
                    MotivoAnulacionId = model.ReasonAnnulmentId,
                    UsuarioAnula = model.CreationUser,
                    ReferenciaAnulacionBCV = statusAnnulOrderRequest.ReferenciaAnulBcv,
                    FechaAnulacion = DateTime.Now,
                    ObservacionAnulacion = model.AnnulmentObservation
                };

                //Actualiza el Estatus de Operacion Temporal
                var ResultUpdateStatusOpTemp = UpdateStatusCashierOperations(UpdateRequest);

                return ResultUpdateStatusOpTemp;
            }
            catch (Exception ex)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = string.Concat("Se ha presentado un error en el proceso de anulacion de la orden aosciada a la Remesa Entrante",
                           ex.Message, ". Por favor notificar al administrador del sistema.");

                return genericResponse;
            }

        }
        #endregion

        #region AproveEnableRemesaEntrante

        private GenericResponse AproveEnableRemesaEntrante(Annulment model)
        {

            GenericResponse genericResponse = new GenericResponse();
            try
            {
                StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();
                StatusAnnulRemesaEntranteRequest statusAnnulRemesaEtrnateRequest = new StatusAnnulRemesaEntranteRequest();

                statusAnnulRemesaEtrnateRequest.STATUS = Constant.StatusGiroInternacional.DisponibleParaPago;
                statusAnnulRemesaEtrnateRequest.ID_OPERACION = model.RowId;
                statusAnnulRemesaEtrnateRequest.ModificadoPor = model.CreationUser;

                //Actualizo el Estatus de la Remesa Entrante
                var ResultStatusUpdateRemesaEntrante = UpdateStatusAnnulmentRemesaEntrante(statusAnnulRemesaEtrnateRequest);

                if (!ResultStatusUpdateRemesaEntrante.Valid)
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = ResultStatusUpdateRemesaEntrante.ErrorMessage + ". No se ha podido actualizar el estatus de la Remesa Entrante.No se puede generar la Solicitud de Anulacion ";
                    return genericResponse;
                }
                return ResultStatusUpdateRemesaEntrante;
            }
            catch (Exception ex)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = string.Concat("Se ha presentado un error al intentar establecer la Remesa Entrante Disponible para Pago",
                           ex.Message, ". Por favor notificar al administrador del sistema.");

                return genericResponse;
            }

        }
            #endregion

        #region RejectAnnulOpTemp

            private GenericResponse RejectAnnulOpTemp(Annulment model)
        {
            var Operacion = SearchOperacionesPorCobrar(new OperacionesPorCobrarRequest { Id_OPERACION = model.RowId });
            //Request para Actulizar Estatus de Anulacion en Operacion Temporal
            UpdateStatusCashierOperationsRequest UpdateRequest = new UpdateStatusCashierOperationsRequest
            {
                Id_OPERACION = model.RowId,
                Status = model.StatusRowId,
            };

            var ResultUpdateStatusOpTemp = UpdateStatusCashierOperations(UpdateRequest);

            return ResultUpdateStatusOpTemp;
        }

        #endregion

        #region RejectAnnulOrder

        private GenericResponse RejectAnnulOrder(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();

            statusAnnulOrderRequest.ID_ORDEN = model.RowId;
            statusAnnulOrderRequest.STATUS_ORDEN = Convert.ToInt32(model.StatusRowId);
            statusAnnulOrderRequest.Modificado = DateTime.Now;
            statusAnnulOrderRequest.ModificadoPor = model.CreationUser;

            var ResultStatusUpdateOrder = UpdateStatusAnnulmentOrder(statusAnnulOrderRequest);

            //Validar La respuesta de Actualizacion de estatus de la Orden
            if (!ResultStatusUpdateOrder.Valid)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = ResultStatusUpdateOrder.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                return genericResponse;
            }
            return ResultStatusUpdateOrder;
        }

        #endregion

        #region RejectAnnulRemensaEntrante

        private GenericResponse RejectAnnulRemensaEntrante(Annulment model)
        {
            GenericResponse genericResponse = new GenericResponse();
            StatusAnnulOrderRequest statusAnnulOrderRequest = new StatusAnnulOrderRequest();

            StatusAnnulRemesaEntranteRequest statusAnnulRemesaEtrnateRequest = new StatusAnnulRemesaEntranteRequest();
            statusAnnulRemesaEtrnateRequest.STATUS = Convert.ToInt32(model.StatusRowId);
            statusAnnulRemesaEtrnateRequest.ID_OPERACION = model.RowId;
            statusAnnulRemesaEtrnateRequest.ModificadoPor = model.CreationUser;
            statusAnnulRemesaEtrnateRequest.Modificado = DateTime.Now;

            var ResultStatusUpdateRemesaEntrante = UpdateStatusAnnulmentRemesaEntrante(statusAnnulRemesaEtrnateRequest);

            if (!ResultStatusUpdateRemesaEntrante.Valid)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = ResultStatusUpdateRemesaEntrante.ErrorMessage + ". No se ha podido actualizar el estatus de la Remesa Entrante.No se puede generar la Solicitud de Anulacion ";
                return genericResponse;
            }
            // Verifico si Tiene Orden Relacionada
            if (ResultStatusUpdateRemesaEntrante.ReturnId != 0)
            {
                statusAnnulOrderRequest.STATUS_ORDEN = Convert.ToInt32(model.StatusRowId);
                statusAnnulOrderRequest.ID_ORDEN = ResultStatusUpdateRemesaEntrante.ReturnId;
                statusAnnulOrderRequest.Modificado = DateTime.Now;
                statusAnnulOrderRequest.ModificadoPor = model.CreationUser;
            }

            var ResultStatusUpdateOrder = UpdateStatusAnnulmentOrder(statusAnnulOrderRequest);

            if (!ResultStatusUpdateOrder.Valid)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = ResultStatusUpdateOrder.ErrorMessage + ". No se ha podido actualizar el estatus de la Orden.No se puede generar la Solicitud de Anulacion ";
                return genericResponse;
            }

            return ResultStatusUpdateOrder;
        }

        #endregion

        #region AnnulmentOpBCV

        private GenericResponse AnnulmentOpBCV (string TipoAnulacion,string referenciaBCV, string  TipoIdentificacionCliente, string NroIdentificacionCliente)
        {
            GenericResponse genericResponse = new GenericResponse();
            InformacionFinanciera.BV_CCAL_WS ClienteWS = new InformacionFinanciera.BV_CCAL_WS();
            try
            {
            bool TestMode = true;
            var BCVint = new XmlDocument();
            BCVint.LoadXml("<root><referencia_anulacion>" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "</referencia_anulacion></root>");
            XmlNode bcv;
            if (TestMode)
            {
                    XmlNode BCVTest = new XmlDocument();
                    for (int i = 0; i < 3; i++)
                    {
                     BCVTest = ClienteWS.SetAnularOperacionSIMADITest(TipoAnulacion, string.Format("{0}{1}", TipoIdentificacionCliente, NroIdentificacionCliente));
                        if (BCVTest.SelectSingleNode("//referencia_anulacion") != null)
                        {
                            if (BCVTest.SelectSingleNode("//referencia_anulacion").InnerText.Trim() != string.Empty)
                            {
                                i = 3;
                            }
                            
                        }
                    }
                    bcv = BCVTest;
            }
            else
            {
                    XmlNode BCVTest = new XmlDocument();
                    for (int i = 0; i < 3; i++)
                    {
                        BCVTest = ClienteWS.SetAnularOperacionSIMADI(TipoAnulacion, string.Format("{0}{1}", TipoIdentificacionCliente, NroIdentificacionCliente));
                        if (BCVTest.SelectSingleNode("//referencia_anulacion") != null)
                        {
                            if (BCVTest.SelectSingleNode("//referencia_anulacion").InnerText.Trim() != string.Empty)
                            {
                                i = 3;
                            }

                        }
                    }
                    bcv = BCVTest;
            }

            if (bcv.FirstChild == bcv.SelectSingleNode("//error"))
            {
                genericResponse.Error = true;
                genericResponse.Valid= true;
                genericResponse.ErrorMessage = "Error del BCV: " + bcv.SelectSingleNode("//error").InnerText;
                genericResponse.Message = BCVint.SelectSingleNode("//referencia_anulacion").InnerText.Trim();
                return genericResponse;
                
            }
            else
            {
                if (bcv.SelectSingleNode("//referencia_anulacion").InnerText.Trim() != string.Empty) { 
                    genericResponse.Valid = true;
                    genericResponse.Message = bcv.SelectSingleNode("//referencia_anulacion").InnerText.Trim();
                }
                else
                {
                    genericResponse.Error = true;
                    genericResponse.ErrorMessage = "Error del BCV: La referencia de anulación del BCV esta en blanco. Por favor verifique ante el BCV si esta operación fue anulada exitosamente.";
                    return genericResponse;


                }
            }

            return genericResponse;
            }
            catch (Exception ex)
            {
                genericResponse.Error = true;
                genericResponse.ErrorMessage = "Error al intentar Anular la operacion ante al BCV: "+ ex.Message;
            }
            return genericResponse;
        }

        #endregion

        #region ConfirmationOrderCashier

        public GenericResponse ConfirmationOrderCashier(ProcessCashierOperation model, List<Cashiers> cashiers)
        {
            GenericResponse result = new GenericResponse();
            //Busco el Detalle Pago Realizado en Efectivo/
            var DetallePagoRealizado = (builderPagos.SearchDetallePagoRealizado(new DetallePagoRealizadoRequest { PAGO_REALIZADO = model.Orden.ID_ORDEN })).Where(x => x.TIPO_PAGO_REALIZADO == Constant.MetodoPago.Efectivo && x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Dolares);

            #region SETEAR E INSERTAR DESGLOSE DE EFECTIVO PAGO REALIZADO

            #region DESGLOSE DE EFECTIVO PAGO REALIZADO DIVISA
            //Crea el Desglose del Efectivo Pagado en la Divisa
            var DesgloseEfectivo = (from x in cashiers.Where(x => x.OriginOperation == Constant.OperacionesCaja.Salida && x.Currency == model.ListOperation.FirstOrDefault().Moneda).ToList()
                                    group x by new
                                    {
                                        x.Currency,
                                        x.DenominationId,
                                        x.BranchOffice,
                                        x.UserID,
                                        x.OriginOperation,
                                        x.CreationUser
                                    } into grupo
                                    select new DesgloseDeEfectivoPagoRealizado
                                    {
                                        DENOMINACION = grupo.Key.DenominationId,
                                        CANTIDAD_ENTREGADA = grupo.Sum(x => x.Quantity),
                                        SUBTOTAL_ENTREGADO = grupo.Sum(x => x.Amount),
                                        REGISTRADO = DateTime.Now,
                                        REGISTRADOPOR = model.Login,
                                    }).ToList();

            //Creael Update del Detalle del Pago Realizado
            var DetalleUpdate = (from x in DesgloseEfectivo
                                 group x by new
                                 {
                                     x.ID_DESGLOSE
                                 } into grupo
                                 select new UpdateDetallePagoRealizado
                                 {
                                     ID_DETALLE = DetallePagoRealizado.FirstOrDefault().ID_DETALLE,
                                     MONTO = grupo.Sum(x => x.SUBTOTAL_ENTREGADO),
                                 }).ToList();

            //Se asigna el Id detalle del Pago Realizado
            if (DesgloseEfectivo.Count() > 0)
            {
                DesgloseEfectivo.Select(S =>
                {
                    S.DETALLE_PAGO_REALIZADO = DetallePagoRealizado.FirstOrDefault().ID_DETALLE;
                    return S;
                }).ToList();
            }

            //Actualiza el Registro
            var ResultUpdateDetalle = builderPagos.UpdateDetallePagoRealizado(DetalleUpdate.FirstOrDefault());

            if (!ResultUpdateDetalle.Error)
            {
                foreach (var item in DesgloseEfectivo.ToList())
                {
                    var resultInsertDesglose = builderPagos.InsertDesgloseDeEfectivoPagoRealizado(item);
                    if (resultInsertDesglose.Error)
                    {
                        result = resultInsertDesglose;
                    }
                }

                if (result.Error)
                {
                    return result;
                }

            }
            #endregion

            #region DESGLOSE DE EFECTIVO PAGO REALIZADO BS

            var detallePagoRealizadoBs = model.ListOperation.FirstOrDefault().DetallePagoRealizado.Where(x => x.TIPO_PAGO_REALIZADO == Constant.MetodoPago.Efectivo && x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Bolivares).FirstOrDefault();
            if (detallePagoRealizadoBs != null)
            {
                detallePagoRealizadoBs.PAGO_REALIZADO = model.Orden.ID_ORDEN;
                detallePagoRealizadoBs.REGISTRADOPOR = model.Login;
                detallePagoRealizadoBs.FECHA_OPERACION = DateTime.Now;
                detallePagoRealizadoBs.FECHA_TRANSACCION = DateTime.Now;

                var resultDetPagoRealBs = builderPagos.InsertDetallePagoRealizado(detallePagoRealizadoBs);
                if (resultDetPagoRealBs.Valid)
                {
                    detallePagoRealizadoBs.ID_DETALLE = resultDetPagoRealBs.ID_OPERACION;
                }
            }
            
            var DesgloseEfectivoBs = (from x in cashiers.Where(x => x.OriginOperation == Constant.OperacionesCaja.Salida && x.Currency == model.ListOperation.FirstOrDefault().MonedaCon).ToList()
                                    group x by new
                                    {
                                        x.Currency,
                                        x.DenominationId,
                                        x.BranchOffice,
                                        x.UserID,
                                        x.OriginOperation,
                                        x.CreationUser
                                    } into grupo
                                    select new DesgloseDeEfectivoPagoRealizado
                                    {
                                        DENOMINACION = grupo.Key.DenominationId,
                                        CANTIDAD_ENTREGADA = grupo.Sum(x => x.Quantity),
                                        SUBTOTAL_ENTREGADO = grupo.Sum(x => x.Amount),
                                        REGISTRADO = DateTime.Now,
                                        REGISTRADOPOR = model.Login,
                                    }).ToList();

            if (DesgloseEfectivoBs.Count() > 0)
            {
                DesgloseEfectivoBs.Select(S =>
                {
                    S.DETALLE_PAGO_REALIZADO = detallePagoRealizadoBs.ID_DETALLE;
                    return S;
                }).ToList();
            }

            foreach (var item in DesgloseEfectivoBs)
            {
                var resultInsertDesgloseVuelto = builderPagos.InsertDesgloseDeEfectivoPagoRealizado(item);
            }

            #endregion

            #endregion

            #region SETEAR E INSERTAR DESGLOSE DE EFECTIVO VUELTO RECIBIDO PAGO


            List<DesgloseDeEfectivoVueltoRecibidoPago> DesgloseEfectivoVueltoRecibido = new List<DesgloseDeEfectivoVueltoRecibidoPago>();
            CashierRequest cashierRequest = new CashierRequest();
            cashierRequest.RowId = model.ListOperation.FirstOrDefault().Id_OPERACION;
            var ListCashierinClient = builderContabilidad.SearchCashier(cashierRequest);

            if (ListCashierinClient.Count() > 0)
            {
                var DesgloseEfectivoVuelto = (from x in (ListCashierinClient.Where(x => x.OriginOperation == Constant.OperacionesCaja.Entrada && x.TypeOperaction == Constant.OperacionesCaja.IngresoEfecClient).ToList())
                                                  group x by new
                                                  {
                                                      x.Currency,
                                                      x.DenominationId,
                                                      x.BranchOffice,
                                                      x.UserID,
                                                      x.OriginOperation,
                                                      x.CreationUser
                                                  } into grupo
                                                  select new DesgloseDeEfectivoVueltoRecibidoPago
                                                  {
                                                      DENOMINACION = grupo.Key.DenominationId,
                                                      CANTIDAD_RECIBIDA = grupo.Sum(x => x.Quantity),
                                                      SUBTOTAL_RECIBIDO = grupo.Sum(x => x.Amount),
                                                      REGISTRADO = DateTime.Now,
                                                      REGISTRADOPOR = model.Login,
                                                  }).ToList();

                DesgloseEfectivoVueltoRecibido.AddRange(DesgloseEfectivoVuelto);
            }

            if (DesgloseEfectivoVueltoRecibido.Count() > 0)
            {
                DesgloseEfectivoVueltoRecibido.Select(S =>
                {
                    S.DETALLE_PAGO_REALIZADO = DetallePagoRealizado.FirstOrDefault().ID_DETALLE;
                    return S;
                }).ToList();
            }

            foreach (var item in DesgloseEfectivoVueltoRecibido)
            {
                var resultInsertDesgloseVuelto = builderEgreso.InsertDesgloseEfectivoVueltoRecibidoPago(item);
            }

            #endregion

            result = builderContabilidad.InsertAllCashier(cashiers);

            if (!result.Error)
            {
                GenericResponse resultprocess = new GenericResponse { ID_OPERACION = 0, ReturnId = model.Orden.ID_ORDEN, Valid = true };

                //if (model.ListPayInternationalMixed.Count() > 0)
                //{
                    //Pago Mixto de la Operacion se Envia a crear la operacion de Complemento
                    resultprocess = ProcessMixedOrder(model);
                //}

                UpdateStatusCashierOperationsRequest updateStatusCashierOperations = new UpdateStatusCashierOperationsRequest
                {
                    Id_OPERACION = model.ListOperation.FirstOrDefault().Id_OPERACION,
                    Status = Constant.StatusOperacionesTemporales.Procesada,
                    OrderId = model.Orden.ID_ORDEN,

                };
                result = UpdateStatusCashierOperations(updateStatusCashierOperations);

                if (result.Error)
                {
                    result.Valid = true;
                    result.ID_OPERACION = resultprocess.ID_OPERACION;
                }

                result = resultprocess;
               
            }
            return result;

        }

        #endregion

        #endregion

        #region SearchOperationsFrontOffice

        public HashSet<OperationsFrontOffice> SearchOperationsFrontOffice(OperationsFrontOfficeRequest model)
        {
            using (IOperacionesDal<OperationsFrontOffice> dal = new OperacionesDal<OperationsFrontOffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Sucursal", SqlDbType.Int){Value= model.Sucursal??(object)DBNull.Value},
                    new SqlParameter("@CreacionFrom", SqlDbType.DateTime){Value= model.CreacionFrom??(object)DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_BENEFICIARIO", SqlDbType.VarChar,15){Value= model.IDENTIFICACION_BENEFICIARIO??(object)DBNull.Value},
                };
                return dal.SearchOperationsFrontOffice(param);
            }
        }
        #endregion

        public ServiceBankError SearchErrorBank(long nroError)
        {
            using(IOperacionesDal<ServiceBankError> dal = new OperacionesDal<ServiceBankError>())
            {
                SqlParameter[] param =
                {
                     new SqlParameter("@CodError", SqlDbType.BigInt){Value = nroError},
                  
                    
                };
                return dal.SearchErrorBank(param);
            }
        }

        #region Entity
        #region ServiceBank
        public List<ServiceBank> SearchServiceBank(ServiceBankRequest request)
        {
            using (IOperacionesDal<List<ServiceBank>> dal = new OperacionesDal<List<ServiceBank>>())
            {
                return dal.SearchServiceBank(request);
            }
        }
        #endregion

        #region ServicesBankType
        public List<ServicesBankType> SearchServicesBankType(ServicesBankTypeRequest request)
        {
            using (IOperacionesDal<List<ServicesBankType>> dal = new OperacionesDal<List<ServicesBankType>>())
            {
                return dal.SearchServicesBankType(request);
            }
        }
        #endregion

        #region OnlinePayment
        public async Task<OperationPaymentOnlineResponse> OnlinePaymentOperation(OperationPaymentOnlineServives request)
        {
            try
            {
                //Consultamos datos necesarios para procesar la remesa
                ModelDataQuery(request);
                //Registramos el Lote
                InsertBatchBankOperationOnLinePrivate(request);
                //Validamos el modelo
                ValidateModelOnlinePaymentOperation(request);
                //Consultamos la remesa
                BuildModelOrder(request);
                //Validamos la disponibilidad de la remesa en CCAL
                ValidateOperationOnline(request);
                //Si el tipo de validación es 2 la operación ya esta marcada como pagada en el lote
                var paymentOnlineResponse = new OperationPaymentOnlineResponse
                {
                    Status = "Pagada",
                    StatusId = Constants.StatusOnlinePayment.Pagada,
                    Reference = request.Reference
                };
                if (request.TypeValidationId != Constants.TypeValidationBankServices.DetalleLote)
                {
                    //Realizamos el pago en la integración del banco
                    paymentOnlineResponse = await InsertPaymentOnlineServices(request);
                    request.Reference = paymentOnlineResponse.Reference;
                    //Registramos el detalle de la operación
                    var statusdetailId = paymentOnlineResponse.StatusId == Constants.StatusOnlinePayment.Rechazada ?
                        Constants.StatusBatchBankDetail.Rechazada : Constants.StatusBatchBankDetail.Pagada;
                    ProcessBatchBankOperationDetailOnline(request, statusdetailId, paymentOnlineResponse.Message);
                }
                if (paymentOnlineResponse.StatusId == Constants.StatusOnlinePayment.Rechazada)
                    throw new Exception(paymentOnlineResponse.Message);
                //Retornamos el modelo para que la remesa sea procesada con la logica existente en el controlador de operaciones
                return BuildModelPayment(request);
            }
            catch (Exception ex)
            {
                return new OperationPaymentOnlineResponse
                {
                    StatusId = Constant.StatusOnlinePayment.Rechazada,
                    Status = "Rechazada",
                    Message = ex.Message,
                    BatchId = request.BatchId
                };
            }            
        }

        public async Task<GenericResponse> OnlinePaymentOperationQuery(OperationPaymentQueryOnlineServives request)
        {
            try
            {
                PaymentInLineServices paymentServices = new PaymentInLineServices(new HttpClientCommonBank()); 
                IOrderQuery estrategiaImplementada = (IOrderQuery)await paymentServices.setEstrategia(request.BankId, Constants.TypeServicesBank.Consulta);
                return estrategiaImplementada.QueryOrderInLine(request);
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Error = true,
                    ErrorMessage = ex.Message
                };
            }
        }
        private void ValidateModelOnlinePaymentOperation(OperationPaymentOnlineServives request)
        {
            var validation = new OnlinePaymentValidations();
            var messageValidation = validation.ValidationsModel(request);
            if (!string.IsNullOrEmpty(messageValidation))
            {
                throw new Exception(messageValidation);
            }
        }

        private void ModelDataQuery(OperationPaymentOnlineServives request)
        {
            var paymentDetail = SearchServicesBankType(new ServicesBankTypeRequest
            {
                ServicesBankId = request.BankId,
                ServicesTypeId = request.ServiceTypeId,
                StatusId = 1
            }).FirstOrDefault();

            if(paymentDetail == null)
            {
                throw new Exception("No se logro consultar el detalle de pago asociado al servicio.");
            }

            request.PaymentDetailTypeId = paymentDetail.BankId;
            request.PaymentDetailTypeParent = paymentDetail.PaymentDetailTypeParent;
            request.BankAccountsId = paymentDetail.BankAccountsId;
            request.ServiceBankTypeId = paymentDetail.ServicesBankTypeId;
            request.CurrencyId = paymentDetail.CurrencyId;
        }

        private OperationPaymentOnlineServives BuildModelOrder(OperationPaymentOnlineServives request)
        {
            int? batchId = null;
            if (request.BatchId != null)
                batchId = request.BatchId;
            var remesa = SearchRemesasEntrantesFormBatch(new OrdenEntranteRequest
            {
                OperationsId = request.OperationId.ToString(),
                CurrencyId = request.CurrencyId,
                BatchId = batchId
            }).FirstOrDefault();
            if (remesa == null)
            {
                throw new Exception("No se logro encontrar información de la remesa a pagar");
            }
            request.AccountNumber = remesa.numeroCuentaPago;
            request.PhoneNumber = remesa.telefonoBeneficiario;
            request.Ammount = remesa.montoPagar.ToString();
            request.BeneficiaryDocument = remesa.CedulaBeneficiario;
            request.BankBeneficiaryCode = remesa.BankBeneficiaryCode;
            request.BeneficiaryName = remesa.nombreBeneficiario;
            request.BankBeneficiaryId = remesa.BankBeneficiaryId??0;
            return request;
        }

        private void InsertBatchBankOperationOnLinePrivate(OperationPaymentOnlineServives request)
        {
            if (request.BatchId != 0)
                return;
            var insertResponsse = InsertBatchBankOperationOnLine(new BatchBankOperationOnline
            {
                StatusId = Constant.StatusBatchBank.Generado,
                CurrencyId = request.CurrencyId,
                ServicesBankTypeId = request.ServiceBankTypeId,
                CreationUser = request.CreationUser,
                BatchOnlineCount = request.OperationCount,
                BatchOnlineTotalAmmount = request.OperationTotalAmmount,
                CreationDate = DateTime.Now,
                BranchId = request.BranchId
            });
            if(insertResponsse.Error)
            {
                throw new Exception(insertResponsse.ErrorMessage);
            }
            request.BatchId = insertResponsse.ReturnId;
        }
        private void ProcessBatchBankOperationDetailOnline(OperationPaymentOnlineServives request, int statusId, string message)
        {
            var processResponse = ProcessBatchBankOperationDetailOnline(new BatchBankOperationDetailOnline
            {
                StatusId = statusId,
                BatchOnlineId = request.BatchId,
                OperationId = request.OperationId,
                CreationUser = request.CreationUser,
                BankDestinationId = request.BankBeneficiaryId,
                BatchDetailReference = request.Reference ?? string.Empty,
                BatchDetailOnlineAmmount = Convert.ToDecimal(request.Ammount),
                CreationDate = DateTime.Now,
                BatchDetailOnlineAccount = request.ServiceTypeId == Constants.TypeServicesBank.PagoMovil ? request.PhoneNumber : request.AccountNumber,
                BatchDetailOnlineObservation = message
            });
            if (processResponse.Error)
            {
                throw new Exception(processResponse.ErrorMessage);
            }
        }
        private async Task<OperationPaymentOnlineResponse> InsertPaymentOnlineServices(OperationPaymentOnlineServives request)
        {
            PaymentInLineServices paymentServices = new PaymentInLineServices(new HttpClientCommonBank()); ;
            IOrderPayment estrategiaImplementada = (IOrderPayment) await paymentServices.setEstrategia(request.BankId, request.ServiceTypeId);
            var paymentOrderResponse = estrategiaImplementada.PaymentOrderInLine(request);
            InsertPaymentOnlineBankResponseEntity(new PaymentOnlineBankResponseEntity { 
                OperationId = request.OperationId,
                EntityId = Constants.Table.RemesasEntrantes,
                CreationUser = request.CreationUser,
                BranchId = request.BranchId,
                StatusId = paymentOrderResponse.StatusId,
                ServiceBankTypeId = request.ServiceBankTypeId,
                OperationAmmount = Convert.ToDecimal(request.Ammount),
                BankResponseJson = JsonConvert.SerializeObject(paymentOrderResponse),
                CreationDate = DateTime.Now
            });
            return paymentOrderResponse;
        }
        private void ValidateOperationOnline(OperationPaymentOnlineServives request)
        {
            var response = ValidateOperationPaymentOnline(request.OperationId);
            if(response.Error)
                throw new Exception(response.Message);
            var validateResponse = (ValidatePaymentOnline)response.Data;

            request.TypeValidationId = validateResponse.TypeValidationId;
            request.Reference = validateResponse.Reference;
            if (validateResponse.Valid || (validateResponse.TypeValidationId == Constants.TypeValidationBankServices.DetalleLote && validateResponse.StatusId == Constants.StatusBatchBankDetail.Pagada))
                return;
            throw new Exception(validateResponse.Message);
        }
        private OperationPaymentOnlineResponse BuildModelPayment(OperationPaymentOnlineServives request)
        {
            var modelPayment = new OperationPaymentOnlineResponse{
                StatusId = Constants.StatusOnlinePayment.Pagada,
                Status = "Pagada",
                Message = "Pagada",
               BranchId = request.BranchId,
               Reference = request.Reference,
               BatchId = request.BatchId
            };
            modelPayment.Payment = new Pago
            {
                Referencia = request.Reference,
                Pais = "VZL",
                Observaciones = "Pago de Remesa",
                FechaRegistro = DateTime.Now,
                Usuario = request.CreationUserLogin,
                Status = 0
            };

            modelPayment.PaymentDetail = new Detalle_Pago_Realizado
            {
                
                TIPO_PAGO_REALIZADO = request.PaymentDetailTypeParent,
                DETALLE_TIPO_PAGO = request.PaymentDetailTypeId,
                REGISTRADOPOR = request.CreationUserLogin,
                REGISTRADO = DateTime.Now,
                FECHA_TRANSACCION = DateTime.Now,
                BankAccountsId = request.BankAccountsId
            };
            return modelPayment;
        }
        #endregion

        #region BatchBankOperationOnLine
        public GenericResponse InsertBatchBankOperationOnLine(BatchBankOperationOnline request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.InsertBatchBankOperationOnLine(request);
            }
        }

        public GenericResponse SearchBatchBankOperationOnline(BatchBankOperationOnlineRequest request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.SearchBatchBankOperationOnline(request);
            }
        }

        public GenericResponse UpdateBatchBankOperationOnline(BatchBankOperationOnline request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.UpdateBatchBankOperationOnline(request);
            }
        }

        public GenericResponse UpdateAnnulmentBatchBankOperationOnline(BatchBankOperationOnline request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.UpdateAnnulmentBatchBankOperationOnline(request);
            }
        }


        #endregion

        #region BatchBankOperationDetailOnLine
        public GenericResponse ProcessBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.ProcessBatchBankOperationDetailOnline(request);
            }
        }

        public GenericResponse SearchBatchBankOperationDetailOnline(BatchBankOperationDetailOnlineRequest request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.SearchBatchBankOperationDetailOnline(request);
            }
        }

        public GenericResponse UpdateBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.UpdateBatchBankOperationDetailOnline(request);
            }
        }
        #endregion

        #region ValidateOperationPaymentOnline
        public GenericResponse ValidateOperationPaymentOnline(int operationId)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.ValidateOperationPaymentOnline(operationId);
            }
        }
        #endregion

        #region PaymentOnlineBankResponseEntity
        public GenericResponse InsertPaymentOnlineBankResponseEntity(PaymentOnlineBankResponseEntity request)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.InsertPaymentOnlineBankResponseEntity(request);
            }
        }
        #endregion

        #endregion

        #region UpdatePagosStatusOrdenes

        public GenericResponse UpdatePagosStatusOrdenes(Ordenes model)
        {
            var result = new GenericResponse();
            try
            {
                Pago modelPago = null;
                var request1 = new PagosPorConfirmarRequest { ProcesoId = model.ID_ORDEN, Entrada = model.RemesaEntrante };
                var pagoPorConfirmar = builderPagos.SearchPagosPorConfirmar(request1);
                var DetallePagoRealizado = (builderPagos.SearchDetallePagoRealizado(new DetallePagoRealizadoRequest { PAGO_REALIZADO = model.ID_ORDEN })).Where(x => x.TIPO_PAGO_REALIZADO == Constant.MetodoPago.Transferencia).FirstOrDefault();

                modelPago = new Pago
                {
                    OrderId = model.ID_ORDEN,
                    Referencia = pagoPorConfirmar.Referencia,
                    Observaciones = pagoPorConfirmar.Observaciones,
                    FechaRegistro = DateTime.Now,
                    Usuario = model.ModificadoPor
                };

                var UpdatePago = builderPagos.UpdatePagos(modelPago);
                if (UpdatePago.Error)
                {
                    return UpdatePago;
                }

                UpdateDetallePagoRealizado updateDetalle = new UpdateDetallePagoRealizado
                {
                    ID_DETALLE = DetallePagoRealizado.ID_DETALLE,
                    REFERENCIA_2 = pagoPorConfirmar.Referencia,
                    REGISTRADOPOR = model.ModificadoPor,
                    FECHA_TRANSACCION = DateTime.Now,
                    BankAccountsId = pagoPorConfirmar.BankAccountsId,
                    MONTO = DetallePagoRealizado.MONTO
                };

                var UpdateDetallePago = builderPagos.UpdateDetallePagoRealizado(updateDetalle);

                if (UpdateDetallePago.Error)
                {
                    return UpdateDetallePago;
                }
                model.CONCILIADO = null;
                result = UpdateStatusOrdenes(model);

                return result;
            }
            catch (Exception ex) 
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion de los Pagos",
                           ex.Message, ". Por favor notificar al administrador del sistema.");

                return result;
            }
       
        }

        #endregion

        #region ProccesCashierRemesa
        public GenericResponse ProccesCashierRemesa(ProcessCashierOperation model)
        {
            var result = new GenericResponse();
            try
            {
                #region Se consulta Remesa Entrante
                var Entrante = SearchRemesaEntranteFromOrdenes(model.Orden.ID_ORDEN);
                if (Entrante == null)
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "No se logro obtener la información de la remesa entrante para su pago. Por favor intente nuevamente y si el error continua comuniquese con el administrador del sistema.";
                    return result;
                }

                #endregion

                #region Convertir Remesa a Orden

                IncomingRemittanceToOrder _incomingRemittanceToOrder = new IncomingRemittanceToOrder(Entrante);
                _incomingRemittanceToOrder.current = String.IsNullOrEmpty(model.Orden.ModificadoPor) ? "AUTOMATICO" : model.Orden.ModificadoPor;
                _incomingRemittanceToOrder.SucursalProcesa = Convert.ToInt32(model.Orden.SucursalProcesaId);
                _incomingRemittanceToOrder.ModificadoPor = model.Orden.ModificadoPor;
                _incomingRemittanceToOrder.ReferenciaPago = model.Orden.REFERENCIA_PAGO;

                #endregion

                #region Se Consultan datos del cliente

                SearchClientsRequest clientsRequest = new SearchClientsRequest()
                {
                    id_cedula = _incomingRemittanceToOrder.numeroIdBeneficiario,
                    clienteTipo = _incomingRemittanceToOrder.tipoIdBeneficiario,
                    offSet = 0,
                    limit = 10
                };

                var clients = builderClients.Searchfichas(clientsRequest);

                if (!clients.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "Se presenta error al tratar de consultar ficha del cliente en pago de remesa";
                    return result;
                }

                #endregion

                #region Se Consulta el tipo de Operacion BCV

                var TipoMovimientoRequest = new TipoMovimientoRequest
                {
                    idTipoIdentidad = _incomingRemittanceToOrder.tipoIdCliente,
                    TipoOperacion = Constant.ProcesosAsociadosCajero.CodCompraEnc
                };

                var TipoMovimiento = builderSimadi.SearchTiposMovimientos(TipoMovimientoRequest);

                if (!TipoMovimiento.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "Se presenta error el tratar de consultar tipo de movimiento en pago de remesa";
                    return result;
                }

                #endregion

                #region Se Consulta de oficina pagadora

                var Sb_46_OficinasRequest = new Sb_46_OficinasRequest
                {
                    STATUS_OFICINA = 1,
                    CODIGO_CCAL = model.BranchOffice
                };

                var Oficina = builderSudeban.SearchSb_46_OficinasEntity(Sb_46_OficinasRequest).ToList();

                if (!Oficina.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "Se presenta error el tratar de consultar los datos de la oficina en pago de remesa";
                    return result;
                }

                #endregion

                #region Se consultas la tasa por fecha

                var tasas = builderTasas.SearchHistorial(new HistorialRequest
                {
                    Date = _incomingRemittanceToOrder.fechaValorTasa
                });

                if (!tasas.Any())
                {
                    result.Error = true;
                    result.Valid = false;
                    result.ErrorMessage = "No se logro consultar las tasas  cambiarias.";
                    return result;
                }

                var objTasa = tasas.OrderByDescending(x => x.fechaRegistro).FirstOrDefault();
                if (_incomingRemittanceToOrder.tasaCambio != 1)
                    _incomingRemittanceToOrder.tasaCambio = objTasa.valorCompra;

                #endregion

                #region Se conultas las tarifas

                var GetComisionesRequest = new GetComisionesRequest()
                {
                    tipo_solicitud = Constant.ProcesosAsociadosCajero.CodCompraEnc,
                    monto_enviar_usd = _incomingRemittanceToOrder.montoOrden,
                    pais = _incomingRemittanceToOrder.PaisId,
                    corresponsal = _incomingRemittanceToOrder.CorresponsalId
                };

                var Comisiones = builderTarifas.SearchGetComisiones(GetComisionesRequest);

                if (Comisiones.Error)
                {
                    Comisiones.Valid = false;
                    return Comisiones;
                }

                List<GetComisiones> _comisiones = (List<GetComisiones>)Comisiones.Data;

                var tarifas = (from r in _comisiones
                               select new Tarifas_Comiciones
                               {
                                   idTarifa = r.ID_COMISION,
                                   concepto = r.COMISION,
                                   moneda = r.MONEDA,
                                   valor = Convert.ToDecimal(r.MONTO, wsCulture)
                               }).ToList();

                #endregion

                #region Se valida si tiene trasnferencia

                if (model.ListPayInternationalMixed.Count() > 0)
                {
                    var Banco = builderPagos.SearchDetalle_Tipos_Pago(new Detalle_Tipo_PagoRequest());
                    var Banco_ = Banco.Where(x => x.ID_DETALLE == model.ListPayInternationalMixed.FirstOrDefault().IdBanco && x.TIPO_PAGO == model.ListPayInternationalMixed.FirstOrDefault().IdMetodoPago);
                    
                    _incomingRemittanceToOrder.nombreBancoDestino = Banco_.FirstOrDefault().DETALLE;

                }

                #endregion

                #region Listas

                ProccesOrder order = new ProccesOrder() 
                {
                    Orden = _incomingRemittanceToOrder,
                    Ficha = clients,
                    TipoMovimientoss = TipoMovimiento,
                    Oficinas = Oficina,
                    Historial = tasas,
                    Tarifas_Comiciones = tarifas
                };

                #endregion

                using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
                {
                    result = dal.ProccesCashierRemesa(model, order);
                }

                //Falta Validacion para trasnmitir pago a BCV

                return result;

            }
            catch (Exception ex)
            {
                result.Error = true;
                result.Valid = false;
                result.ErrorMessage = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }

        #region InsertOrdenesEntity

        public GenericResponse InsertOrdenesEntity(Ordenes model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.InsertOrdenesEntity(model);
            }
        }

        #endregion

        #region SearchRemesaEntranteFromOrdenesEntity

        public GenericResponse SearchRemesaEntranteFromOrdenesEntity(int ID_OPERACION)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.SearchRemesaEntranteFromOrdenesEntity(ID_OPERACION);
            }
        }

        #endregion

        #endregion

        #region SearchOperationsTempPending
        public List<OperationsTempPending> SearchOperationsTempPending()
        {
            using (IOperacionesDal<List<OperationsTempPending>> dal = new OperacionesDal<List<OperationsTempPending>>())
            {
                return dal.SearchOperationsTempPending();
            }
        }
        #endregion

        #region UpdateStatusOperationsTempEntity
        public GenericResponse UpdateStatusOperationsTempEntity(OperacionesPorCobrar model)
        {
            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.UpdateStatusOperationsTempEntity(model);
            }
        }
        #endregion

        #region InsertBusinessOperation

        public OperacionDeNegocio InsertBusinessOperation(OperacionDeNegocio modelOperation)
        {
            try
            {
                #region Variables

                ///Clase del Modelo a Insert 
                OperacionesPorCobrar model = new OperacionesPorCobrar();

                ///Clases Para Buscar Tarifas
                TarifaReq tarifaReq = new TarifaReq();

                #endregion

                #region Se Consultan datos del cliente

                SearchClientsRequest clientsRequest = new SearchClientsRequest()
                {
                    id_cedula = modelOperation.CIREM,
                    clienteTipo = modelOperation.TipoDocumentoREM,
                    offSet = 0,
                    limit = 10
                };

                var clients = builderClients.Searchfichas(clientsRequest);

                if (!clients.Any())
                {
                    modelOperation.error = true;
                    modelOperation.clientErrorDetail = "Se presenta error al tratar de consultar ficha del cliente en pago de remesa";
                    return modelOperation;
                }

                #endregion

                #region Datos de Usuario

                model.Ciudad = modelOperation.IdCiudad;
                model.Oficina = modelOperation.IdOficinaExterna;
                model.CiuOrig = modelOperation.CiuOrig;
                model.Usuario = modelOperation.current;
                model.SUCURSAL_NEW = modelOperation.IdSucursal;
                model.FechaRegistro = DateTime.Now;

                #endregion

                #region Datos del Cliente

                model.NomRemA = clients.FirstOrDefault().PrimerNombre; model.NomRemB = clients.FirstOrDefault().SegundoNombre;
                model.ApeRemA = clients.FirstOrDefault().PrimerApellido; model.ApeRemB = clients.FirstOrDefault().SegundoApellido;
                model.NombresRem = clients.FirstOrDefault().NombreCompleto;
                model.TelRem = clients.FirstOrDefault().TelfMovil1;
                model.DirRem = clients.FirstOrDefault().direccion;
                model.EMAIL_CLIENTE = clients.FirstOrDefault().Email;
                model.Cirem = clients.FirstOrDefault().ClienteTipo.Trim() + clients.FirstOrDefault().id_cedula.Trim();
                model.Observaciones = (string.IsNullOrEmpty(modelOperation.Observacion) ? string.Empty : modelOperation.Observacion);
                model.Mensaje = modelOperation.CodigoTipoOperacion;

                #endregion

                #region Datos del Beneficiario

                model.NomDesA = string.Empty; 
                model.NomDesB = string.Empty; 
                model.ApeDesA = string.Empty; 
                model.ApeDesB = string.Empty;
                model.Ccdes = string.Empty; 
                model.DirDes = string.Empty; 
                model.TelDes = string.Empty; 
                model.Tel2Des = string.Empty;
                model.EMAIL_BENEFICIARIO = string.Empty; 
                model.DirDes = string.Empty; 
                model.Tel2Des = string.Empty; 
                model.TypeAccountBank = modelOperation.TIPOCUENTADES;

                #endregion

                #region Otros Datos

                model.Ficha = 0; 
                model.Persona = 0; 
                model.ReciboReTransmite = 0; 
                model.ReTransmite = 0; 
                model.ID_SCD = 0;
                model.ID_PROX_SOL = 0;
                model.Status = 48;// Status revision Prevencion
                model.Status_Temp = 3;
                model.TIPOSOL = "ve";
                model.PagoOtros = 0;
                model.PagoIsv = 0;

                #endregion

                #region Informacion de la Oficina

                var ofic = new List<DetalleOficina>();
                model.Pagador = "CAL"; //pagador por defecto para este tipo de operaciones
                model.CORRESPONSAL = "CAL"; //pagador por defecto para este tipo de operaciones

                #endregion

                #region Pais de Destino

                if (modelOperation.PAISDES == string.Empty || modelOperation.PAISDES == null)
                {
                    model.Pais = "VZL";
                    modelOperation.PAISDES = "VZL";
                }
                else
                {
                    model.Pais = modelOperation.PAISDES;
                }

                #endregion

                #region Valores para Buscar Monedas

                var MonedasRequest = new MonedasRequests()
                {
                    Activa = true
                };
                var monedas = builderTablasMaestras.SearchMonedasEntity(MonedasRequest).ToList();

                var Serialize = JsonConvert.SerializeObject(monedas);
                var Deserialize = JsonConvert.DeserializeObject<List<Monedas>>(Serialize);

                var MonedaOperacion = Deserialize.Where(x => x.MonedaId == modelOperation.IdMonedaOperacion).FirstOrDefault();
                var MonedaConversion = Deserialize.Where(x => x.MonedaId == modelOperation.IdMonedaConversion).FirstOrDefault();
                model.MONEDA = MonedaOperacion.MonedaId;
                model.MonedaOperacion = MonedaConversion.MonedaId;

                #endregion

                #region Seteo de Variables Segun el Tipo de Operacion

                switch (modelOperation.CodigoTipoOperacion)
                {
                    #region Compra de Divisas Efectivo (compra-simadi-taq)

                    case "compra-simadi-taq"://Compra de Divisa en Efectivo
                        model.Tipo_Operacion = Constant.TipoOperaciones.Compras;
                        model.Pagador = "CAL";
                        model.CORRESPONSAL = "CAL";
                        //Valores de Tarifa
                        tarifaReq.idCorresp = "CAL";
                        tarifaReq.OperationType = OperationType.CompraEfectivo;
                        ////////////////////////
                        //Valores de Beneficiario
                        model.NomDesA = "Casa de Cambio";
                        model.ApeDesA = " Angulo López";
                        model.Ccdes = "R302075661";
                        break;

                    #endregion

                    #region Venta de Divisas en Efectivo (venta-simadi-taq)

                    case "venta-simadi-taq"://Venta de Divisa en Efectivo
                        model.Tipo_Operacion = Constant.TipoOperaciones.Ventas;
                        //Valores de Tarifa
                        tarifaReq.idCorresp = "CAL";
                        tarifaReq.OperationType = OperationType.VentaEfectivo;
                        ////////////////////////
                        //Valores de Beneficiario
                        model.NomDesA = "Casa de Cambio";
                        model.ApeDesA = " Angulo López";
                        model.Ccdes = "R302075661";
                        break;

                    #endregion

                    #region Venta de Divisas por Encomienda (venta-simadi-enc)

                    case "venta-simadi-enc"://Venta de Divisa por Encomienda 
                        model.Tipo_Operacion = Constant.TipoOperaciones.Ventas;

                        ///Datos del Beneficiario
                        model.NomDesA = modelOperation.NOMDES1.ToUpper(); model.NomDesB = (string.IsNullOrEmpty(modelOperation.NOMDES2) ? string.Empty : modelOperation.NOMDES2.ToUpper());
                        model.ApeDesA = modelOperation.APEDES1.ToUpper(); model.ApeDesB = (string.IsNullOrEmpty(modelOperation.APEDES2) ? string.Empty : modelOperation.APEDES2.ToUpper());
                        model.NombresDes = modelOperation.NOMDES.ToUpper();

                        model.Ccdes = modelOperation.TipoDocumentoDES.Trim() + modelOperation.CIDES.Trim();
                        model.TelDes = modelOperation.TELDES;
                        model.EMAIL_BENEFICIARIO = modelOperation.EMAILDES;
                        model.Ciudad = Convert.ToInt32(modelOperation.CIUDADDES);
                        model.Oficina = Convert.ToInt32(modelOperation.OFICIANDES);
                        model.BANCO_DESTINO = modelOperation.BANCODES;
                        model.NUMERO_CUENTA_DESTINO = modelOperation.NUNCUENTADES;
                        //////////////////////////////////////////////////////////
                        ///Informacion de la Oficina Seleccionada
                        IOficinasBuilder BuilderOficinas = new OficinasBuilder();
                        ofic = BuilderOficinas.SearchDetalleOficina(new DetalleOficinaRequest { idOficina = model.Oficina }).ToList();

                        //////////////////////////////////////////
                        //Valores de Tarifa
                        tarifaReq.idCorresp = model.Oficina.ToString();
                        tarifaReq.OperationType = OperationType.Corresponsal;
                        ////////////////////////////  
                        model.CORRESPONSAL = ofic.FirstOrDefault().corresponsal;
                        model.Pagador = ofic.FirstOrDefault().pagador;
                        break;

                    #endregion

                    #region Compra de Divisas por Transferencia (compra-simadi-enc)

                    case "compra-simadi-enc"://Compra de Divisas por Transferencia
                        model.Tipo_Operacion = Constant.TipoOperaciones.Compras;
                        model.Pagador = "CAL";
                        model.CORRESPONSAL = "CAL";
                        model.Ciudad = 0;
                        model.Oficina = 0;
                        //Valores de Transferencia
                        model.BANCO_DESTINO = modelOperation.BANCODES;
                        model.NUMERO_CUENTA_DESTINO = modelOperation.NUNCUENTADES;
                        model.DIRECCION_BANCO = modelOperation.DIRBANCODES;
                        model.ABA = modelOperation.ABA;
                        model.SWIFT = modelOperation.SWIFT;
                        //Valores Banco Intermediario
                        model.BANCO_INTERMEDIARIO = modelOperation.NOMBANCOINTER;
                        model.NUMERO_CUENTA_INTERMEDIARIO = modelOperation.NUMCUENTAINTER;
                        model.ABA_INTERMEDIARIO = modelOperation.ABAINTER;
                        model.DIRECCION_BANCO_INTERMEDIARIO = modelOperation.DIRBANCOINTER;
                        //Valores de Tarifa
                        tarifaReq.idCorresp = "CAL";
                        tarifaReq.OperationType = OperationType.Transferencia;
                        ////////////////////////////
                        break;

                    #endregion

                    #region Venta de Divisas por Transferencia (venta-simadi-trf)

                    case "venta-simadi-trf"://Venta de Divisa por Transferencia 
                        model.Tipo_Operacion = Constant.TipoOperaciones.Ventas;
                        model.Ciudad = 0;
                        model.Oficina = 0;
                        ///Datos del Beneficiario
                        model.NomDesA = modelOperation.NOMDES1.ToUpper(); 
                        model.NomDesB = (string.IsNullOrEmpty(modelOperation.NOMDES2) ? string.Empty : modelOperation.NOMDES2.ToUpper());
                        model.ApeDesA = modelOperation.APEDES1.ToUpper(); 
                        model.ApeDesB = (string.IsNullOrEmpty(modelOperation.APEDES2) ? string.Empty : modelOperation.APEDES2.ToUpper());
                        model.NombresDes = modelOperation.NOMDES.ToUpper();

                        model.Ccdes = modelOperation.TipoDocumentoDES.Trim() + modelOperation.CIDES.Trim();
                        model.TelDes = modelOperation.TELDES;
                        model.EMAIL_BENEFICIARIO = modelOperation.EMAILDES;
                        //Valores de Transferencia
                        model.BANCO_DESTINO = modelOperation.BANCODES;
                        model.NUMERO_CUENTA_DESTINO = modelOperation.NUNCUENTADES;
                        model.DIRECCION_BANCO = modelOperation.DIRBANCODES;
                        model.ABA = modelOperation.ABA;
                        model.SWIFT = modelOperation.SWIFT;
                        //Valores Banco Intermediario
                        model.BANCO_INTERMEDIARIO = modelOperation.NOMBANCOINTER;
                        model.NUMERO_CUENTA_INTERMEDIARIO = modelOperation.NUMCUENTAINTER;
                        model.ABA_INTERMEDIARIO = modelOperation.ABAINTER;
                        model.DIRECCION_BANCO_INTERMEDIARIO = modelOperation.DIRBANCOINTER;
                        //Valores de Tarifa
                        tarifaReq.idCorresp = "CAL";
                        tarifaReq.OperationType = OperationType.Transferencia;
                        ////////////////////////////
                        break;

                    #endregion

                    default:
                        break;

                }

                #endregion

                #region Se Consulta el tipo de Operacion BCV

                var TipoMovimientoRequest = new TipoMovimientoRequest
                {
                    idTipoIdentidad = modelOperation.TipoDocumentoREM,
                    TipoOperacion = modelOperation.CodigoTipoOperacion
                };

                var TipoMovimiento = builderSimadi.SearchTiposMovimientos(TipoMovimientoRequest);

                if (!TipoMovimiento.Any())
                {
                    modelOperation.error = true;
                    modelOperation.clientErrorDetail = "Se presenta error el tratar de consultar tipo de movimiento en pago de remesa";
                    return modelOperation;
                }

                #endregion

                #region Busqueda de Tarifa  

                tarifaReq.moneda = MonedaOperacion.MonedaCodigoInt;
                tarifaReq.idPais = modelOperation.PAISDES;
                tarifaReq.MonedaOperacion = MonedaOperacion.MonedaId;
                tarifaReq.montoEnviar = modelOperation.montoOrden;
                tarifaReq.MonedaConversion = MonedaConversion.MonedaId;
                tarifaReq.tipoId = clients.FirstOrDefault().ClienteTipo;
                tarifaReq.tipoOperacion = TipoMovimiento.FirstOrDefault().ID_TIPO.ToString();
                tarifaReq.tasa = modelOperation.TasaCambio;

                var FinancialSumary = builderTarifas.SearchFinancialSummaryEntity(tarifaReq);

                model.MontoConversion = Math.Round(FinancialSumary.AmmountConversion);
                var tarifas = FinancialSumary.RateOperation.ToList();

                #endregion

                #region Tipo de Movimiento o Detalle Tipo de Operacion/Operacion codigo Homologado del BCV/Motivo de Operacion

                var tipoMovimiento = TipoMovimiento.FirstOrDefault().ID_BCV;
                var idDetalleTipoOperacion = Convert.ToInt32(TipoMovimiento.FirstOrDefault().ID_TIPO);
                model.TIPO_OP_BCV = tipoMovimiento;
                model.MOTIVO_OP_BCV = modelOperation.IdMotivosOperacion;
                model.DETALLE_TIPO_OPERACION = idDetalleTipoOperacion;

                #endregion

                #region Calculo de Montos y Tarifas

                model.TasaConversion = modelOperation.TasaCambio;
                if (modelOperation.CodigoTipoOperacion == "compra-simadi-taq" || modelOperation.CodigoTipoOperacion == "venta-simadi-taq")
                {

                    if (modelOperation.IdMonedaOperacion != 213)
                    {
                        //orden.montoOrden = (orden.montoOrden * decimal.Parse(orden.TasaConversion)) / orden.tasaCambio;
                        model.Dolares = (modelOperation.MontoOperacion * modelOperation.TasaConversion / modelOperation.TasaCambio);
                    }
                    else
                    {
                        model.Dolares = modelOperation.montoOrden;
                        model.TasaConversion = modelOperation.TasaCambio;
                    }
                }

                var Bolivares = Math.Round(modelOperation.montoOrden * modelOperation.TasaCambio, 2);
                decimal comisionUs = 0, comisionBs = 0;

                foreach (var item in tarifas.Where(x => x.moneda != null))
                {
                    if (item.moneda.Equals("USD"))
                    {
                        if (item.valor < 1)
                            comisionUs += Math.Round(item.valor * modelOperation.montoOrden, 2);
                        else
                            comisionUs += Math.Round(item.valor, 2);
                    }
                    else
                    {
                        if (item.valor < 1)
                            comisionBs += Math.Round(item.valor * Math.Round((modelOperation.montoOrden * modelOperation.TasaCambio), 2), 2);
                        else
                            comisionBs += Math.Round(item.valor, 2);
                    }
                }

                ///Monto en Bolivares sin Comision
                model.Bolivares = Bolivares;
                //////////////////////////////////////////
                ///Monto en Dolares sin Comision
                model.Dolares = modelOperation.montoOrden - comisionUs;
                //////////////////////////////////////////
                /// Tarifas de Comision
                model.PagoTarifaUs = comisionUs;
                model.PagoTarifa = comisionBs;
                model.TasaDolar = modelOperation.TasaCambio;
                //////////////////////////////////////////
                model.FECHA_VALOR_TASA = DateTime.Now;

                #region  Oficina Seleccionada para Calcular con su Tasa
                if (modelOperation.CodigoTipoOperacion == "venta-simadi-enc")
                {
                    model.MonedaDest = (modelOperation.montoOrden - comisionUs) * ofic.FirstOrDefault().tasa;
                }
                else
                {
                    model.MonedaDest = modelOperation.montoOrden - comisionUs;
                }
                #endregion

                #endregion

                using (IOperacionesDal<OperacionDeNegocio> dal = new OperacionesDal<OperacionDeNegocio>())
                {
                    modelOperation = dal.InsertBusinessOperation(modelOperation, model, tarifas);
                }

                return modelOperation;
            }
            catch (Exception ex)
            {
                modelOperation.error = true;
                modelOperation.clientErrorDetail = "Ha ocurrido un error al guardar la operación temporal (InsertBusinessOperation Builder), por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return modelOperation;
            }
            

        }

        #endregion

        #region UpdateStatusOrdenesEntity

        public GenericResponse UpdateStatusOrdenesEntity(Ordenes model)
        {
            Pago modelPago = null;
            Detalle_Pago_Realizado modelDetallePago = null;
            var result = new GenericResponse();
            ProccesOrder order = null;

            if (string.IsNullOrEmpty(model.LetraSucursal) || string.IsNullOrEmpty(model.SucursalProcesaName))
            {
                var usuario = builderSeguridad.SearchInformacionUsuario(model.ModificadoPor);

                model.LetraSucursal = usuario.FirstOrDefault().LETRA;
                model.SucursalProcesaName = usuario.FirstOrDefault().SUCURSAL;
            }

            if (model.STATUS_ORDEN == 8)
            {
                if (model.Confirmar == true)
                {
                    var request1 = new PagosPorConfirmarRequest { ProcesoId = model.ID_ORDEN, Entrada = model.RemesaEntrante };
                    var pago = builderPagos.SearchPagosPorConfirmar(request1);

                    model.REFERENCIA_PAGO = pago.Referencia;

                    modelPago = new Pago
                    {
                        Referencia = pago.Referencia,
                        Pais = "VZL",
                        Observaciones = pago.Observaciones,
                        FechaRegistro = DateTime.Now,
                        Usuario = model.ModificadoPor,
                        Status = 0
                    };
                    modelDetallePago = new Detalle_Pago_Realizado
                    {
                        TIPO_PAGO_REALIZADO = pago.TipoPagoRealizadoId,
                        DETALLE_TIPO_PAGO = pago.DetalleTipoPagoId,
                        REGISTRADOPOR = model.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        FECHA_TRANSACCION = pago.FechaOperacion,
                        FECHA_OPERACION = DateTime.Now,
                        BankAccountsId = pago.BankAccountsId,
                    };
                }
                else
                {
                    modelPago = new Pago
                    {
                        Referencia = model.REFERENCIA_PAGO,
                        Pais = "VZL",
                        Observaciones = model.OBSERVACIONES,
                        FechaRegistro = DateTime.Now,
                        Usuario = model.ModificadoPor,
                        Status = 0
                    };
                    modelDetallePago = new Detalle_Pago_Realizado
                    {
                        TIPO_PAGO_REALIZADO = model.Tipo_Pago == 0 ? 1 : model.Tipo_Pago,
                        DETALLE_TIPO_PAGO = model.DetalleTipoPago == 0 ? 951 : model.DetalleTipoPago,
                        REGISTRADOPOR = model.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        FECHA_TRANSACCION = DateTime.Now,
                        BankAccountsId = model.BankAccountsId,
                    };
                }

                if (model.RemesaEntrante)
                {

                    #region Se consulta Remesa Entrante
                    var Entrante = SearchRemesaEntranteFromOrdenes(model.ID_ORDEN);
                    if (Entrante == null)
                    {
                        result.Error = true;
                        result.Valid = false;
                        result.ErrorMessage = "No se logro obtener la información de la remesa entrante para su pago. Por favor intente nuevamente y si el error continua comuniquese con el administrador del sistema.";
                        return result;
                    }

                    #endregion

                    #region Convertir Remesa a Orden

                    IncomingRemittanceToOrder _incomingRemittanceToOrder = new IncomingRemittanceToOrder(Entrante);
                    _incomingRemittanceToOrder.current = String.IsNullOrEmpty(model.ModificadoPor) ? "AUTOMATICO" : model.ModificadoPor;
                    _incomingRemittanceToOrder.SucursalProcesa = Convert.ToInt32(model.SucursalProcesaId);
                    _incomingRemittanceToOrder.ModificadoPor = model.ModificadoPor;
                    _incomingRemittanceToOrder.ReferenciaPago = model.REFERENCIA_PAGO;

                    #endregion

                    #region Se Consultan datos del cliente

                    SearchClientsRequest clientsRequest = new SearchClientsRequest()
                    {
                        id_cedula = _incomingRemittanceToOrder.numeroIdBeneficiario,
                        clienteTipo = _incomingRemittanceToOrder.tipoIdBeneficiario,
                        offSet = 0,
                        limit = 10
                    };

                    var clients = builderClients.Searchfichas(clientsRequest);

                    if (!clients.Any())
                    {
                        result.Error = true;
                        result.Valid = false;
                        result.ErrorMessage = "Se presenta error al tratar de consultar ficha del cliente en pago de remesa";
                        return result;
                    }

                    #endregion

                    #region Se Consulta el tipo de Operacion BCV

                    var TipoMovimientoRequest = new TipoMovimientoRequest
                    {
                        idTipoIdentidad = _incomingRemittanceToOrder.tipoIdCliente,
                        TipoOperacion = Constant.ProcesosAsociadosCajero.CodCompraEnc
                    };

                    var TipoMovimiento = builderSimadi.SearchTiposMovimientos(TipoMovimientoRequest);

                    if (!TipoMovimiento.Any())
                    {
                        result.Error = true;
                        result.Valid = false;
                        result.ErrorMessage = "Se presenta error el tratar de consultar tipo de movimiento en pago de remesa";
                        return result;
                    }

                    #endregion

                    #region Se Consulta de oficina pagadora

                    int? STATUS_OFICINA = null;

                    if (model.SucursalProcesaName != Sucursales.SucursalVirtual)
                    {
                        STATUS_OFICINA = 1;
                    }

                    var Sb_46_OficinasRequest = new Sb_46_OficinasRequest
                    {
                        STATUS_OFICINA = STATUS_OFICINA,
                        CODIGO_CCAL = model.SucursalProcesaName
                    };

                    var Oficina = builderSudeban.SearchSb_46_OficinasEntity(Sb_46_OficinasRequest).ToList();

                    if (!Oficina.Any())
                    {
                        result.Error = true;
                        result.Valid = false;
                        result.ErrorMessage = "Se presenta error el tratar de consultar los datos de la oficina en pago de remesa";
                        return result;
                    }

                    #endregion

                    #region Se consultas la tasa por fecha

                    var tasas = builderTasas.SearchHistorial(new HistorialRequest
                    {
                        Date = _incomingRemittanceToOrder.fechaValorTasa
                    });

                    if (!tasas.Any())
                    {
                        result.Error = true;
                        result.Valid = false;
                        result.ErrorMessage = "No se logro consultar las tasas  cambiarias.";
                        return result;
                    }

                    var objTasa = tasas.OrderByDescending(x => x.fechaRegistro).FirstOrDefault();
                    if (_incomingRemittanceToOrder.tasaCambio != 1)
                        _incomingRemittanceToOrder.tasaCambio = objTasa.valorCompra;

                    #endregion

                    #region Se conultas las tarifas

                    var GetComisionesRequest = new GetComisionesRequest()
                    {
                        tipo_solicitud = Constant.ProcesosAsociadosCajero.CodCompraEnc,
                        monto_enviar_usd = _incomingRemittanceToOrder.montoOrden,
                        pais = _incomingRemittanceToOrder.PaisId,
                        corresponsal = _incomingRemittanceToOrder.CorresponsalId
                    };

                    var Comisiones = builderTarifas.SearchGetComisiones(GetComisionesRequest);

                    if (Comisiones.Error)
                    {
                        Comisiones.Valid = false;
                        return Comisiones;
                    }

                    List<GetComisiones> _comisiones = (List<GetComisiones>)Comisiones.Data;

                    var tarifas = (from r in _comisiones
                                   select new Tarifas_Comiciones
                                   {
                                       idTarifa = r.ID_COMISION,
                                       concepto = r.COMISION,
                                       moneda = r.MONEDA,
                                       valor = Convert.ToDecimal(r.MONTO, wsCulture)
                                   }).ToList();

                    #endregion

                    #region Listas

                    order = new ProccesOrder()
                    {
                        Orden = _incomingRemittanceToOrder,
                        Ficha = clients,
                        TipoMovimientoss = TipoMovimiento,
                        Oficinas = Oficina,
                        Historial = tasas,
                        Tarifas_Comiciones = tarifas
                    };

                    #endregion
                }
            }

            using (IOperacionesDal<GenericResponse> dal = new OperacionesDal<GenericResponse>())
            {
                return dal.UpdateStatusOrdenesEntity(model, modelPago, modelDetallePago, order);
            }
        }

        #endregion

        #region ValidateAccumulatedAmount
        public AccumulatedAmount ValidateAccumulatedAmount(ValidateRequestAmount request)
        {
            using (IOperacionesDal<AccumulatedAmount> dal = new OperacionesDal<AccumulatedAmount>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ClientId", SqlDbType.VarChar, 14){Value = request.ClientId},
                };
                return dal.ValidateAccumulatedAmount(param);
            }
        }
        #endregion

    }

}
