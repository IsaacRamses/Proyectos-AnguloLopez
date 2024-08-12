using Common.Models.Angulo_Lopez.Contabilidad;
using Common.Models.Angulo_Lopez.Numeracion;
using Common.Models.Angulo_Lopez.Oficinas;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.Simadi;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Common.Models.Angulo_Lopez.RIA;
using Common.Models.Common;
using Common.Resource;
using DataAccess.Angulo_Lopez.Contabilidad;
using DataAccess.Angulo_Lopez.Numeracion;
using DataAccess.Angulo_Lopez.Oficinas;
using DataAccess.Angulo_Lopez.Simadi;
using DataAccess.Conection;
using IDataAccess.Angulo_Lopez.Contabilidad;
using IDataAccess.Angulo_Lopez.Numeracion;
using IDataAccess.Angulo_Lopez.Oficinas;
using IDataAccess.Angulo_Lopez.Operaciones;
using IDataAccess.Angulo_Lopez.Simadi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Xml;
using Utilities;
using CashierDetails = Common.Models.Angulo_Lopez.Contabilidad.CashierDetail;
using Cashiers = Common.Models.Angulo_Lopez.Contabilidad.Cashier;
using CashierSummarys = Common.Models.Angulo_Lopez.Contabilidad.CashierSummary;
using NUMERACIONs = Common.Models.Angulo_Lopez.Numeracion.NUMERACION;
using Tarifas = Common.Models.Common.Tarifa;
using BatchBankOperationDetailOnline = Common.Models.Angulo_Lopez.Operaciones.BatchBankOperationDetailOnline;
using BatchBankOperationOnline = Common.Models.Angulo_Lopez.Operaciones.BatchBankOperationOnline;
using PaymentOnlineBankResponseEntity = Common.Models.Angulo_Lopez.Operaciones.PaymentOnlineBankResponseEntity;
using Common.Models.Angulo_Lopez.Pagos;
using Common.Models.Clientes;

namespace DataAccess.Operaciones
{
    public class OperacionesDal<TEntity> : Finalize, IOperacionesDal<TEntity> where TEntity : new()
    {
        #region Variables
        private readonly AnguloLopezDbContextt<TEntity> _DbContext = new AnguloLopezDbContextt<TEntity>();
        private readonly ALDBEntities db = new ALDBEntities();

        public static CultureInfo wsCulture { get => new CultureInfo("en-US"); }
        #endregion

        #region Ordenes

        #region SearchOrderPayment

        public TEntity SearchOrderPayment(object[] param)
        {
            return _DbContext.Database.SqlQuery<TEntity>("operaciones.SearchOrderPayment @TypeDocument, @ClientDocument, @Amount, @OrderCode", param).FirstOrDefault();
        }

        #endregion

        #region UpdateStatusOrdenes

        public TEntity UpdateStatusOrdenes(object[] param)
        {
            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateStatusOrdenes @ID_ORDEN, @STATUS_ORDEN, @ModificadoPor, @REFERENCIA_PAGO, @ObservacionesRechazo, @SucursalProcesaId", param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de cambiar el estatus de la orden: ", ex.Message);
            }
            return (TEntity)(object)result;
        }

        #endregion

        #region UpdateStatusOrder

        public TEntity UpdateStatusOrder(object[] param)
        {
            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateStatusOrder @ID_ORDEN, @STATUS_ORDEN, @ModificadoPor, @REFERENCIA_PAGO, @ObservacionesRechazo, @SucursalProcesaId, @OBSERVACIONES", param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de cambiar el estatus de la orden: ", ex.Message);
            }
            return (TEntity)(object)result;
        }

        #endregion

        #region SearchOrdenesByFilter

        public HashSet<TEntity> SearchOrdenesByFilter(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchOrdenesByFilter @STATUS_ORDEN, @FECHA_OPERACION_INICIAL, @FECHA_OPERACION_FINAL, @FECHA_PAGO_INICIAL, @FECHA_PAGO_FINAL, @FECHA_ANULACION_INICIAL, @FECHA_ANULACION_FINAL, @AnuladaPor, @SucursalProcesaId, @ID_ORDEN, @ProcessedBy", param));
            }
            catch (Exception ex)
            {

                throw new Exception(string.Concat("Se ha presentado el siguiente error al tratar de consultar las ordenes: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message));
            }

        }

        #endregion

        #region SearchReturnFundsOrder
        public HashSet<TEntity> SearchReturnFundsOrder(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchReturnFundsOrder ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE, ",
                                "@Moneda "), param)
                );
        }

        #endregion

        #region SearchORDENES
        public HashSet<TEntity> SearchORDENES(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchORDENES ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE, ",
                                "@IDENTIFICACION_BENEFICIARIO, ",
                                "@ID_ORDEN_FK "), param)
                );
        }

        #endregion

        #region SearchTurnAlert
        public HashSet<TEntity> SearchTurnAlert(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchTurnAlert ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE "), param)
                );
        }

        #endregion

        #region SearchPaymentOrdersNotCanceled
        public HashSet<TEntity> SearchPaymentOrdersNotCanceled(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchPaymentOrdersNotCanceled ",
                                "@ID_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE "), param)
                );
        }

        #endregion

        #region SearchPaymentOrdersDivisaTaquilla
        public HashSet<TEntity> SearchPaymentOrdersDivisaTaquilla(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchPaymentOrdersDivisaTaquilla ",
                                "@ID_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE "), param)
                );
        }

        #endregion

        #region SearchPaymentOrderVentanilla
        public HashSet<TEntity> SearchPaymentOrderVentanilla(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchPaymentOrderVentanilla ",
                                "@ID_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE "), param)
                );
        }

        #endregion

        #region SearchPaymentOrdersTrasnfer
        public HashSet<TEntity> SearchPaymentOrdersTrasnfer(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchPaymentOrdersTrasnfer ",
                                "@ID_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE "), param)
                );
        }

        #endregion

        #region SearchPaidOrders

        public HashSet<TEntity> SearchPaidOrders(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchPaidOrders @DateStat, @DateEnd, @BranchId, @OrderId", param));
        }

        #endregion

        #region InsertOrdenes

        public TEntity InsertOrdenes(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>(string.Concat("operaciones.spiOrdenes ",
                "@AGENTE,",
                "@CLIENTE,",
                "@CORRESPONSAL,",
                "@DETALLE_TIPO_OPERACION,",
                "@FECHA_OPERACION,",
                "@FECHA_VALOR_TASA,",
                "@MONEDA,",
                "@MONTO,",
                "@MONTO_CAMBIO,",
                "@OFICINA,",
                "@PAIS_DESTINO,",
                "@PERSONA,",
                "@REGISTRADOPOR,",
                "@STATUS_ORDEN,",
                "@SUCURSAL,",
                "@TASA_DESTINO,",
                "@TIPO_CAMBIO,",
                "@NUMERO,",
                "@MOTIVO_OP_BCV,",
                "@TIPO_OP_BCV,",
                "@NOMBRES_REMITENTE,",
                "@APELLIDOS_REMITENTE,",
                "@IDENTIFICACION_REMITENTE,",
                "@NOMBRES_BENEFICIARIO,",
                "@APELLIDOS_BENEFICIARIO,",
                "@IDENTIFICACION_BENEFICIARIO,",
                "@SECUENCIA,",
                "@FECHA_ENVIO,",
                "@REFERENCIA_PAGO,",
                "@FECHA_PAGO,",
                "@REFERENCIA_ORDEN,",
                "@BANCO_NACIONAL,",
                "@NUMERO_CUENTA,",
                "@EMAIL_CLIENTE,",
                "@EMAIL_BENEFICIARIO,",
                "@BANCO_DESTINO,",
                "@NUMERO_CUENTA_DESTINO,",
                "@DIRECCION_BANCO,",
                "@ABA,",
                "@SWIFT,",
                "@IBAN,",
                "@TELEFONO_BENEFICIARIO,",
                "@TELEFONO_CLIENTE,",
                "@OBSERVACIONES,",
                "@BANCO_INTERMEDIARIO,",
                "@NUMERO_CUENTA_INTERMEDIARIO,",
                "@DIRECCION_BANCO_INTERMEDIARIO,",
                "@ABA_INTERMEDIARIO,",
                "@SWIFT_INTERMEDIARIO, ",
                "@IBAN_INTERMEDIARIO, ",
                "@USUARIO_TAQUILLA, ",
                "@pMonedaOperacion, ",
                "@pTasaConversion, ",
                "@pMontoConversion, ",
                "@ProcesarTransferencia, ",
                "@NumeroCuentaPagoTransferencia, ",
                "@BancoPagoTransferencia,",
                "@SucursalProcesaId,",
                "@ModificadoPor,",
                "@Modificado,",
                "@ID_ORDEN_FK"), param).FirstOrDefault();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de registrar la orden: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message)
                };
            }


        }

        #endregion

        #region SearchOrdenesPayableCorrespondent
        public HashSet<TEntity> SearchOrdenesPayableCorrespondent(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchOrdenesPayableCorrespondent ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL "), param)
                );
        }

        #endregion

        #region SearchOrderPendingCorrespondet

        public HashSet<TEntity> SearchOrderPendingCorrespondet(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchOrderPendingCorrespondet @STATUS_ORDEN, @FECHA_OPERACION_INICIAL, @FECHA_OPERACION_FINAL, @FECHA_PAGO_INICIAL, @FECHA_PAGO_FINAL, @FECHA_ANULACION_INICIAL, @FECHA_ANULACION_FINAL, @AnuladaPor, @SucursalProcesaId, @ID_ORDEN, @ProcessedBy, @StatusCall, @statusIDsolicitudes, @statusIDordenes", param));
            }
            catch (Exception ex)
            {

                throw new Exception(string.Concat("Se ha presentado el siguiente error al tratar de consultar las ordenes: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message));
            }

        }

        #endregion

        #region ValidateOperationClientPause
        public TEntity ValidateOperationClientPause(int clientId)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>("operaciones.ValidateOperationClientPause @ClientId", clientId).First();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar validar las operaciones con clientes en estatus pausado: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }

        }

        #endregion

        #region RollBackOrden

        public GenericResponse RollBackOrden(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ID_OPERACION = _DbContext.Database.SqlQuery<int>(
               string.Concat("operaciones.RollBackOrden ",
                               "@ID_ORDEN, ",
                               "@ID_OPERACION_TEMPORAL, ",
                               "@ID_PAGO, ",
                               "@NRO_FACTURA, ",
                               "@SUCURSAL "), param).First();
                _return.Valid = true;

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de eliminar DeleteOperacionesPorCobrar:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region SearchPaymentOrderInternational
        public HashSet<TEntity> SearchPaymentOrderInternational(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchOrderPayInternacionalMixed ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN "), param)
                );
        }

        #endregion

        #endregion

        #region SearchOperations
        public HashSet<TEntity> SearchOperations(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchOperations ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN, ",
                                "@CreacionFrom, ",
                                "@CreacionTo, ",
                                "@CORRESPONSAL, ",
                                "@IDENTIFICACION_REMITENTE, ",
                                "@IDENTIFICACION_BENEFICIARIO, ",
                                "@TipoOperaciones, ",
                                "@SECUENCIA, ",
                                "@REFERENCIA "), param)
                );
        }

        #endregion

        #region BatchBankOperations
        public HashSet<TEntity> SearchBatchBankOperations(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(string.Concat("operaciones.SearchBatchBankOperations ",
                "@StatusId, ",
                "@OperationTypeId,",
                "@CurrencyId,",
                "@BankSourceId,",
                "@BankDestinationId,",
                "@BranchId,",
                "@BatchNumber,",
                "@DateStart,",
                "@DateEnd"), param));
        }

        public TEntity InsertBatchBankOperations(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>(string.Concat("operaciones.InsertBatchBankOperations ",
                "@StatusId,",
                "@OperationTypeId,",
                "@CurrencyId,",
                "@CreationUser,",
                "@BankSourceId,",
                "@BankDestinationId,",
                "@BranchId,",
                "@BatchCountOperations,",
                "@BatchTotalAmmount,",
                "@BatchFile,",
                "@BatchObservation"), param).FirstOrDefault();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de registrar el lote: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message)
                };
            }

        }

        public TEntity InsertBatchBankOperationIntegrated(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>(string.Concat("operaciones.InsertBatchBankOperationIntegrated ",
                "@OperationTypeId,",
                "@CurrencyId,",
                "@CreationUser,",
                "@BankSourceId,",
                "@BankDestinationId,",
                "@BranchId,",
                "@Operations,",
                "@BatchFile,",
                "@BatchObservation,",
                "@BankAccountsId"), param).FirstOrDefault();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de registrar el lote: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message)
                };
            }

        }

        public TEntity UpdateStatusBatchBankOperations(object[] param)
        {
            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.UpdateStatusBatchBankOperations ",
                    "@BatchId,",
                    "@StatusId,",
                    "@UpdateUser"), param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de cambiar el estatus del lote: ", ex.Message);
            }
            return (TEntity)(object)result;
        }

        public TEntity SearchBatchBankOperationsNumber()
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>("[operaciones].[SearchBatchBankOperationsNumber]").FirstOrDefault();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de obtener el número el lote: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message)
                };
            }

        }
        #endregion

        #region BatchBankDetail
        public HashSet<TEntity> SearchBatchBankDetail(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(string.Concat("operaciones.SearchBatchBankDetail ",
                "@BatchId, ",
                "@StatusId,",
                "@RequestId,",
                "@OrderId,",
                "@OrderIncomingId,",
                "@ReasonRejectedId"), param));
        }

        public TEntity InsertBatchBankDetail(object[] param)
        {
            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.InsertBatchBankDetail ",
                    "@BatchId,",
                    "@StatusId,",
                    "@CreationUser,",
                    "@RequestId,",
                    "@OrderId,",
                    "@OrderIncomingId,",
                    "@DetailObservacion"), param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de registrar el detalle de lote: ", ex.Message);
            }
            return (TEntity)(object)result;
        }

        public TEntity UpdateStatusBatchBankDetail(object[] param)
        {
            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.UpdateStatusBatchBankDetail ",
                    "@DetailId,",
                    "@StatusId,",
                    "@UpdateUser,",
                    "@ReasonRejectedId,",
                    "@DetailPaymentReference,",
                    "@DetailOperationDate"), param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de cambiar el estatus del detalle lote: ", ex.Message);
            }
            return (TEntity)(object)result;
        }
        #endregion

        #region Remesas Enrantes

        #region SearchRemesaEntranteFromOrdenes

        public TEntity SearchRemesaEntranteFromOrdenes(object[] param)
        {
            return _DbContext.Database.SqlQuery<TEntity>("operaciones.SearchRemesaEntranteFromOrdenes @Id", param).FirstOrDefault();
        }

        #endregion

        #region RollbackRemesaPagadas

        public TEntity RollbackRemesaPagadas(object[] param)
        {

            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.RollbackRemesaPagadas @Id, @RemesaEntrante", param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de realizar el Rollback del pago de remesa: ", ex.Message);
            }
            return (TEntity)(object)result;
        }
        public TEntity RollbackMassRemesaPagadas(object[] param)
        {

            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.RollbackMassRemesaPagadas @OrderId, @RemesaEntrante, @DetailId", param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de realizar el Rollback del pago de remesa: ", ex.Message);
            }
            return (TEntity)(object)result;
        }
        public HashSet<TEntity> SearchRemesasEntrantesFormBatch(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchRemesasEntrantesFormBatch @CurrencyId, @BankId, @OperationsId, @BankIds, @BatchId", param));
        }

        public HashSet<TEntity> SearchRemesasEntrantesFormBatchPorConfirmar(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchRemesasEntrantesFormBatchPorConfirmar @CurrencyId, @BankId, @OperationsId, @BankIds", param));
        }
        

        #endregion

        #region UpdateStatusRemesasEntrantesIncidencias

        public TEntity UpdateStatusRemesasEntrantesIncidencias(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.UpdateStatusRemesasEntrantesIncidencias ",
                    "@ID_OPERACION, ",
                    "@status, ",
                    "@ModificadoPor, ",
                    "@Modificado, ",
                    "@OBSERVACIONES "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar los datos: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return (TEntity)(object)_return;
        }

        #endregion

        #region spuRemesaEntrantePagadaPendientePorConfirmar

        public TEntity spuRemesaEntrantePagadaPendientePorConfirmar(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.spuRemesaEntrantePagadaPendientePorConfirmar ",
                    "@id, ",
                    "@referencia, ",
                    "@fecha_pago, ",
                    "@observaciones "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al actualizar los datos: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return (TEntity)(object)_return;
        }

        #endregion

        #region UpdateRemesasEntrantesIncidenciaRIA
        public TEntity UpdateRemesasEntrantesIncidenciaRIA(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.UpdateRemesasEntrantesIncidenciaRIA ",
                    "@ID_OPERACION, ",
                    "@Modificado, ",
                    "@MENSAJE "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar los datos: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return (TEntity)(object)_return;
        }

        #endregion

        #region SearchRemesasEntrantesRejectRIA
        public HashSet<TEntity> SearchRemesasEntrantesRejectRIA(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchRemesasEntrantesRejectRIA @ficha, @identificacion, " +
                    "@referencia, @corresponsal, @status, @pais, @id, @fechaCreacionInicio, @fechaCreacionFin, @fechaPagoInicio, @fechaPagoFin, @secuencia, @GrupoId, @Modo, @BankId, @CurrencyId,@Pending", param));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region InsertRemesaEntranteWithoutValidationProcess
        public TEntity InsertRemesaEntranteWithoutValidationProcess(object[] param)
        {
            try
            {
                var id = _DbContext.Database.SqlQuery<int>(
                    string.Concat("[operaciones].[InsertRemesaEntranteWithoutValidationProcess] @MONEDA, @PAIS, @SUCURSAL, @CORRESPONSAL, " +
                    "@REGISTRADO_POR, @PAGOMANUAL, @SECUENCIA, @MODO, @CIREM, @CIDES, @REFERENCIA, @TELREM, @TELDES, @TEL2DES, " +
                    "@NOMREM, @NOMDES, @DIRDES, @USD, @TASA, @BOLI, @COMIUSD, @OTROS, @IVA, @FECHA, @OBSERVACIONES, @MENSAJE, @BANCO, @CUENTA, @TIPO_CUENTA, @EMAIL, @Estatus "), param).First();
                return (TEntity)(object)new GenericResponse
                {
                    ReturnId = id
                };
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar los datos: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        #endregion
        #endregion

        #region Pago de lote
        public TEntity UpdateStatusLotPayment(object[] param)
        {
            var result = new GenericResponse();
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.UpdateStatusLotPayment ",
                    "@DetailId,",
                    "@StatusId,",
                    "@OrderIncomingId,",
                    "@UpdateUser,",
                    "@Observation,",
                    "@ReasonRejectedId,",
                    "@DetailPaymentReference, ",
                    "@DetailOperationDate"), param);
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de cambiar el estatus del pago de lote: ", ex.Message);
            }
            return (TEntity)(object)result;
        }
        #endregion

        #region AccountReceivableCorrespondent

        #region SearchAccountReceivableCorrespondent
        public HashSet<TEntity> SearchAccountReceivableCorrespondent(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchAccountReceivableCorrespondent ",
                                "@AccountId, ",
                                "@CorrespondentId, ",
                                "@StatusId, ",
                                "@AccountNumber, ",
                                "@CreacionFrom, ",
                                "@CreacionTo "), param)
                );
        }

        #endregion

        #region InsertAccountReceivableCorrespondent

        public HashSet<TEntity> InsertAccountReceivableCorrespondent(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.InsertAccountReceivableCorrespondent ",
                "@CorrespondentId, ",
                "@StatusId, ",
                "@CreationUser, ",
                "@AccountTotalOperation, ",
                "@AccountTotalAmmount, ",
                "@CreationDate, ",
                "@AccountPaymentReference, ",
                "@AccountObservation, ",
                "@AccounPaymentObservation, ",
                "@AccountPaymentDate "), param));
        }

        #endregion

        #region UpdateAccountReceivableCorrespondent

        public GenericResponse UpdateAccountReceivableCorrespondent(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountReceivableCorrespondent ",
                    "@AccountId, ",
                    "@CorrespondentId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@AccountNumber, ",
                    "@AccountTotalOperation, ",
                    "@AccountTotalAmmount, ",
                    "@AccountPaymentReference, ",
                    "@AccountObservation, ",
                    "@AccounPaymentObservation, ",
                    "@AccountPaymentDate, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentRemoveDetail

        public GenericResponse UpdateAccountReceivableCorrespondentRemoveDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountReceivableCorrespondentRemoveDetail ",
                    "@AccountId, ",
                    "@UpdateUser, ",
                    "@AccountTotalOperation, ",
                    "@AccountTotalAmmount, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateStatusAccountReceivableCorrespondent

        public GenericResponse UpdateStatusAccountReceivableCorrespondent(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateStatusAccountReceivableCorrespondent ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentPayment

        public GenericResponse UpdateAccountReceivableCorrespondentPayment(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountReceivableCorrespondentPayment ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@AccountPaymentReference, ",
                    "@AccounPaymentObservation, ",
                    "@AccountPaymentDate, ",
                    "@AccountPaymentBank, ",
                    "@UpdateUser, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        #region AccountReceivableCorrespondentDetail

        #region SearchAccountReceivableCorrespondentDetail
        public HashSet<TEntity> SearchAccountReceivableCorrespondentDetail(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchAccountReceivableCorrespondentDetail ",
                                "@DetailId, ",
                                "@OperationId, ",
                                "@AccountId, ",
                                "@StatusId, ",
                                "@Corresponsal "), param)

                );
        }

        #endregion

        #region InsertAccountReceivableCorrespondentDetail

        public GenericResponse InsertAccountReceivableCorrespondentDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.InsertAccountReceivableCorrespondentDetail ",
                    "@OperationId, ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@CreationUser, ",
                    "@CreationDate, ",
                    "@DetailObservation "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar AccountReceivableCorrespondentDetail:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentDetail

        public GenericResponse UpdateAccountReceivableCorrespondentDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountReceivableCorrespondentDetail ",
                    "@DetailId, ",
                    "@OperationId, ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@DetailObservation, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar AccountReceivableCorrespondentDetail:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateStatusAccountReceivableCorrespondentDetail

        public GenericResponse UpdateStatusAccountReceivableCorrespondentDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateStatusAccountReceivableCorrespondentDetail ",
                    "@DetailId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar AccountReceivableCorrespondentDetail:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region SearchDraftsReceivable
        public HashSet<TEntity> SearchDraftsReceivable(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchDraftsReceivable ",
                                "@corresponsal, ",
                                "@fechaPagoInicio, ",
                                "@fechaPagoFin "), param)

                );
        }

        #endregion
        #endregion

        #region AccountsPayableCorrespondent

        #region SearchAccountsPayableCorrespondent
        public HashSet<TEntity> SearchAccountsPayableCorrespondent(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchAccountsPayableCorrespondent ",
                                "@AccountId, ",
                                "@CorrespondentId, ",
                                "@StatusId, ",
                                "@AccountNumber, ",
                                "@CreacionFrom, ",
                                "@CreacionTo "), param)
                );
        }

        #endregion

        #region InsertAccountsPayableCorrespondent

        public HashSet<TEntity> InsertAccountsPayableCorrespondent(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.InsertAccountsPayableCorrespondent ",
                "@CorrespondentId, ",
                "@StatusId, ",
                "@CreationUser, ",
                "@AccountTotalOperation, ",
                "@AccountTotalAmmount, ",
                "@CreationDate, ",
                "@AccountPaymentReference, ",
                "@AccountObservation, ",
                "@AccounPaymentObservation, ",
                "@AccountPaymentDate "), param));
        }

        #endregion

        #region UpdateAccountsPayableCorrespondent

        public GenericResponse UpdateAccountsPayableCorrespondent(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountsPayableCorrespondent ",
                    "@AccountId, ",
                    "@CorrespondentId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@AccountNumber, ",
                    "@AccountTotalOperation, ",
                    "@AccountTotalAmmount, ",
                    "@AccountPaymentReference, ",
                    "@AccountObservation, ",
                    "@AccounPaymentObservation, ",
                    "@AccountPaymentDate, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentRemoveDetail

        public GenericResponse UpdateAccountsPayableCorrespondentRemoveDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountsPayableCorrespondentRemoveDetail ",
                    "@AccountId, ",
                    "@UpdateUser, ",
                    "@AccountTotalOperation, ",
                    "@AccountTotalAmmount, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateStatusAccountsPayableCorrespondent

        public GenericResponse UpdateStatusAccountsPayableCorrespondent(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateStatusAccountsPayableCorrespondent ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentPayment

        public GenericResponse UpdateAccountsPayableCorrespondentPayment(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountsPayableCorrespondentPayment ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@AccountPaymentReference, ",
                    "@AccounPaymentObservation, ",
                    "@AccountPaymentDate, ",
                    "@AccountPaymentBank, ",
                    "@UpdateUser, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de editar AccountReceivableCorrespondent:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        #region AccountsPayableCorrespondentDetail

        #region SearchAccountsPayableCorrespondentDetail
        public HashSet<TEntity> SearchAccountsPayableCorrespondentDetail(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchAccountsPayableCorrespondentDetail ",
                                "@DetailId, ",
                                "@OperationId, ",
                                "@AccountId, ",
                                "@StatusId, ",
                                "@Corresponsal "), param)

                );
        }

        #endregion

        #region InsertAccountsPayableCorrespondentDetail

        public GenericResponse InsertAccountsPayableCorrespondentDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.InsertAccountsPayableCorrespondentDetail ",
                    "@OperationId, ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@CreationUser, ",
                    "@CreationDate, ",
                    "@DetailObservation "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar AccountReceivableCorrespondentDetail:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentDetail

        public GenericResponse UpdateAccountsPayableCorrespondentDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateAccountsPayableCorrespondentDetail ",
                    "@DetailId, ",
                    "@OperationId, ",
                    "@AccountId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@DetailObservation, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar AccountReceivableCorrespondentDetail:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateStatusAccountsPayableCorrespondentDetail

        public GenericResponse UpdateStatusAccountsPayableCorrespondentDetail(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateStatusAccountsPayableCorrespondentDetail ",
                    "@DetailId, ",
                    "@StatusId, ",
                    "@UpdateUser, ",
                    "@UpdateDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar AccountReceivableCorrespondentDetail:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        #region CompanyAlliance

        #region SearchCompanyAlliance
        public HashSet<TEntity> SearchCompanyAlliance(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchCompanyAlliance ",
                                "@AllianceId, ",
                                "@StatusId, ",
                                "@TypeIdentificationId, ",
                                "@AllianceIdentificationNumber "), param)
                );
        }

        #endregion

        #endregion

        #region CompanyAllianceBank

        #region SearchCompanyAllianceBank
        public HashSet<TEntity> SearchCompanyAllianceBank(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchCompanyAllianceBank ",
                                "@AllianceBankId, ",
                                "@StatusId, ",
                                "@AllianceId, ",
                                "@BankId "), param)
                );
        }

        #endregion

        #endregion

        #region ShippingAlliance

        #region SearchShippingAlliance
        public HashSet<TEntity> SearchShippingAlliance(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.SearchShippingAlliance ",
                                "@ShippingAllianceId, ",
                                "@StatusId, ",
                                "@AllianceId, ",
                                "@persitentObjectID, ",
                                "@IdentificationNumber, ",
                                "@Tracking "), param)
                );
        }
        #endregion

        #region InsertShippingAlliance

        public GenericResponse InsertShippingAlliance(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.InsertShippingAlliance ",
                    "@StatusId, ",
                    "@AllianceId, ",
                    "@persitentObjectID, ",
                    "@IdentificationNumber, ",
                    "@Tracking, ",
                    "@Weight, ",
                    "@DestinationCountry, ",
                    "@Amount, ",
                    "@CreationUser, ",
                    "@CreationDate "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar InsertShippingAlliance:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateOrderNumberShippingAlliance

        public GenericResponse UpdateOrderNumberShippingAlliance(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.UpdateOrderNumberShippingAlliance ",
                    "@ShippingAllianceId, ",
                    "@Tracking, ",
                    "@OrderNumber "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar UpdateOrderNumberShippingAlliance:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region RollbackShippingAlliance

        public GenericResponse RollbackShippingAlliance(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("Operaciones.RollbackShippingAlliance ",
                    "@AllianceId, ",
                    "@persitentObjectID, ",
                    "@Tracking "), param).First();
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de RollbackShippingAlliance:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        public HashSet<TEntity> SearchRemesasGirosPendientes(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchRemesasGirosPendientes @ficha, @identificacion, " +
                    "@referencia, @corresponsal, @status, @pais, @id, @fechaCreacionInicio, @fechaCreacionFin, @fechaPagoInicio, @fechaPagoFin, @secuencia, @GrupoId, @Modo, @BankId, @CurrencyId, @CorresponsalId", param));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region SearchInternationalMoneyOrderPayment
        public HashSet<TEntity> SearchInternationalMoneyOrderPayment(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchInternationalMoneyOrderPayment @identificacion", param));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OPERACIONES

        #region SearchOperacionesPorPagar
        public HashSet<TEntity> SearchOperacionesPorPagar(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchOperacionesPorPagar ",
                                "@Id_OPERACION, ",
                                "@CIREM, ",
                                "@CIREM, ",
                                "@Sucursal, ",
                                "@Estatus "), param)
                );
        }
        #endregion

        #region InsertOperacionesPorPagar

        public GenericResponse InsertOperacionesPorPagar(object[] param)
        {
            var _return = new GenericResponse();

            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
       string.Concat("operaciones.InsertOperacionesPorPagar ",
               "@CiuOrig, ",
               "@NroRecibo, ",
               "@SECUENCIA, ",
               "@Referencia, ",
               "@CIREM, ",
               "@NOMREM, ",
               "@TELREM, ",
               "@NOMDES, ",
               "@CIDES, ",
               "@DIRDES, ",
               "@TELDES, ",
               "@TEL2DES, ",
               "@PAIS, ",
               "@CIUDAD, ",
               "@MODO, ",
               "@USD, ",
               "@TASA, ",
               "@BOLI, ",
               "@COMIUSD, ",
               "@OTROS, ",
               "@IVA, ",
               "@Observaciones, ",
               "@Mensaje, ",
               "@Fecha, ",
               "@FechaPago, ",
               "@Status, ",
               "@Status_Temp, ",
               "@Tipo_Operacion, ",
               "@PagoManual, ",
               "@USUARIO, ",
               "@FICHA, ",
               "@PERSONA, ",
               "@OPERADOR, ",
               "@ESTACION_TRABAJO, ",
               "@ACTUALIZADO "), param).First();

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar InsertOperacionesPorPagar:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region InsertOperacionesPorCobrar

        public GenericResponse InsertOperacionesPorCobrar(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                  string.Concat("operaciones.InsertOperacionesPorCobrar ",
                 "@CiuOrig, ",
                 "@NroRecibo, ",
                 "@CIRem, ",
                 "@NomRemA, ",
                 "@NomRemB, ",
                 "@ApeRemA, ",
                 "@ApeRemB, ",
                 "@TelRem, ",
                 "@DirRem, ",
                 "@NomDesA, ",
                 "@NomDesB, ",
                 "@ApeDesA, ",
                 "@ApeDesB, ",
                 "@CCDes, ",
                 "@DirDes, ",
                 "@TelDes, ",
                 "@Tel2Des, ",
                 "@Pais, ",
                 "@Pagador, ",
                 "@Ciudad, ",
                 "@Oficina, ",
                 "@Bolivares, ",
                 "@Dolares, ",
                 "@TasaDolar, ",
                 "@MonedaDest, ",
                 "@PagoTarifaUS, ",
                 "@PagoTarifa, ",
                 "@PagoOtros, ",
                 "@PagoIsv, ",
                 "@Observaciones, ",
                 "@Mensaje, ",
                 "@Usuario, ",
                 "@Status, ",
                 "@TIPOSOL, ",
                 "@Cadivi, ",
                 "@Status_Temp, ",
                 "@Tipo_Operacion, ",
                 "@MesRemesa, ",
                 "@Ficha, ",
                 "@Persona, ",
                 "@ReTransmite, ",
                 "@ReciboReTransmite, ",
                 "@ID_SCD, ",
                 "@ID_PROX_SOL, ",
                 "@Concepto, ",
                 "@ES_MATRICULA, ",
                 "@NOMBRE_BENEFICIARIO_MATRICULA, ",
                 "@CEDULA_BENEFICIARIO_MATRICULA, ",
                 "@tasaAplicada, ",
                 "@detalleTipoOperacion, ",
                 "@moneda, ",
                 "@sucursalNew, ",
                 "@corresponsal, ",
                 "@fechaValorTasa, ",
                 "@MOTIVO_OP_BCV, ",
                 "@TIPO_OP_BCV, ",
                 "@BANCO_NACIONAL, ",
                 "@NUMERO_CUENTA, ",
                 "@EMAIL_CLIENTE, ",
                 "@EMAIL_BENEFICIARIO, ",
                 "@BANCO_DESTINO, ",
                 "@NUMERO_CUENTA_DESTINO, ",
                 "@DIRECCION_BANCO, ",
                 "@ABA, ",
                 "@SWIFT, ",
                 "@IBAN, ",
                 "@BANCO_INTERMEDIARIO, ",
                 "@NUMERO_CUENTA_INTERMEDIARIO, ",
                 "@DIRECCION_BANCO_INTERMEDIARIO, ",
                 "@ABA_INTERMEDIARIO, ",
                 "@SWIFT_INTERMEDIARIO, ",
                 "@IBAN_INTERMEDIARIO, ",
                 "@pMonedaOperacion, ",
                 "@pTasaConversion, ",
                 "@pMontoConversion, ",
                 "@TypeAccountBank ,",
                "@NumeroOpTemporal "), param).First();

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar InsertOperacionesPorPagar:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        public GenericResponse InsertOpPorCobrarRemesaEntrante(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                  string.Concat("operaciones.InsertOperacionesPorCobrarRemesaEntrante ",
                 "@Id_RemesaEntrante, ",
                 "@User "), param).First();
                _return.Valid = true;

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar  InsertOperacionesPorCobrarRemesaEntrante:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region SearchOperacionesPorCobrar
        public HashSet<TEntity> SearchOperacionesPorCobrar(object[] param)
        {

            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchOperacionesPorCobrar ",
                                "@Id_OPERACION, ",
                                "@CIREM, ",
                                "@CIDES, ",
                                "@Sucursal, ",
                                "@Estatus, ",
                                "@OrderId "), param)
                );
        }
        #endregion

        #region DeleteOperacionesPorCobrar

        public GenericResponse DeleteOperacionesPorCobrar(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                var result = new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
               string.Concat("operaciones.DeleteOperacionPorCobrar ",
                               "@IDS_OPERACIONES, ",
                               "@NUMERO_OPERACION, ",
                               "@CEDULA_CLIENTE, ",
                               "@ANULACION "), param)
               );

                _return.Valid = true;
                
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de eliminar DeleteOperacionesPorCobrar:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region CashierOperations

        #region SearchCashierOperations

        public HashSet<TEntity> SearchCashierOperations(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchCashierOperations ",
                                "@CIREM, ",
                                "@CIDES, ",
                                "@IDSUCURSAL "), param)
                );
        }
        #endregion

        #region UpdateStatusCashierOperations

        public GenericResponse UpdateStatusCashierOperations(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.UpdateStatusCashierOperations ",
                                "@Id_OPERACION, ",
                                "@Status, ",
                                "@Status_Temp, ",
                                "@Procesado, ",
                                "@FechaAnulacion, ",
                                "@UsuarioAnula, ",
                                " @ReferenciaAnulacionBCV, ",
                                "@MotivoAnulacionId, ",
                                "@ObservacionAnulacion, ",
                                "@OrderId "), param).First();
                                 

                _return.Valid = true;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de UpdateStatusCashierOperations:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        #region InsertMixedOrder

        public GenericResponse InsertMixedOrder(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ID_OPERACION = _DbContext.Database.SqlQuery<int>(
                  string.Concat("operaciones.InsertMixedOrder ",
                 "@ID_OPERACION, ",
                 "@IdMetodoPago, ",
                 "@TipoDocumento, ",
                 "@NroDocumento, ",
                 "@NroTelefono, ",
                 "@IdBanco, ",
                 "@NroCuenta, ",
                 "@IdMoneda, ",
                 "@CodMoneda, ",
                 "@Tasa, ",
                 "@MontoDivisa, ",
                 "@MontoBs, ",
                 "@ModificadoPor "), param).First();
                _return.Valid = true;

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar InsertMixedOrder:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateMixedOrderReferenceBCV

        public GenericResponse UpdateMixedOrderReferenceBCV(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ID_OPERACION = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.UpdateMixedOrderReferenceBCV ",
                                "@ID_ORDEN, ",
                                "@ReferenciaBCV  "), param).First();


                _return.Valid = true;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de UpdateMixedOrderReferenceBCV:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }
            #endregion

        #endregion

        #region SearchORDENES
            public HashSet<TEntity> SearchOrdenesTransmitirPendientes(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.spsGetOrdenesTransmitirExternos ",
                                "@corresponsal "), param)
                );
        }
        public GenericResponse UpdateOrdenSalienteTransmitida(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("[operaciones].[UpdateOrdenesSalientesTransmitidas] ",
                            "@id "), param).First();

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de actualizar UpdateOrdenSalienteTransmitida:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }
        public HashSet<TEntity> SearchOrdenesSalientesAnularPendientes(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("Operaciones.spsGetOrdenesSalientesAnularPendientes ",
                                "@corresponsal "), param)
                );
        }

        #endregion

        #region Tarifas Aplicadas

        #region SearchTarifasAplicadas

        public HashSet<TEntity> SearchTarifasAplicadas(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.spsTarifasAplicadas ",
                                "@ORDEN, ",
                                "@TEMPORAL, ",
                                "@idOpNacional, ",
                                "@SOLICITUD "), param)
                );
        }

        #endregion

        #region UpdateOrdenTarifasAplicadas

        public TEntity UpdateOrdenTarifasAplicadas(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.spuTarifasAplicadas ",
                    "@temporal, ",
                    "@orden "), param).First();
                _return.Valid = true;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la orden en tarifas aplicadas: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return (TEntity)(object)_return;
        }

        #endregion

        #endregion

        #region Anulaciones

        #region SearchOperationsCashierAnnulment

        public HashSet<TEntity> SearchOperationsCashierAnnulment(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchOperationsCashierAnnulment ",
                                "@ID_ORDEN, ",
                                "@CIREM, ",
                                "@SUCURSAL, ",
                                "@REGISTRADOPOR "), param)
                );
        }

        #endregion

        #region SearchOperationsTempAnnulment

        public HashSet<TEntity> SearchOperationsTempAnnulment(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchOperationsTempAnnulment ",
                                "@CIREM, ",
                                "@CIDES, ",
                                "@IDSUCURSAL, ",
                                "@USUARIO "), param)
                );
        }

        #endregion

        #region SearchAnnulment

        public HashSet<TEntity> SearchAnnulment(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchAnnulment ",
                                "@StatusId, ",
                                "@TableId, ",
                                "@RowId, ",
                                "@AnnulmentId "), param)
                );
        }


        #endregion

        #region SearchAnnulmentPending

        public HashSet<TEntity> SearchAnnulmentPending(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchAnnulmentPending ",
                                "@BranchOfficeId "), param)
                );
        }
        #endregion

        #region InsertAnnulment
        public GenericResponse InsertAnnulment(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                  string.Concat("operaciones.InsertAnnulment ",
                 "@StatusId, ",
                 "@TableId, ",
                 "@RowId, ",
                 "@StatusRowId, ",
                 "@CreationUser, ",
                 "@ReasonAnnulmentId, ",
                 "@AnnulmentObservation "), param).First();
                _return.Valid = true;

            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar  InsertAnnulment:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }
        #endregion

        #region UpdateStatusAnnulmentOrder

        public GenericResponse UpdateStatusAnnulmentOrder(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.UpdateStatusAnnulmentOrder ",
                                "@ID_ORDEN, ",
                                "@STATUS_ORDEN, ",
                                "@ModificadoPor, ",
                                "@Modificado, ",
                                "@FECHA_ANULACION, ",
                                "@AnuladaPor, ",
                                "@ReferenciaAnulBcv, ",
                                "@MotivoAnulacionId, ",
                                "@ObservacionesAnulacion "), param).First();


                _return.Valid = true;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de UpdateStatusCashierOperations:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateStatusAnnulmentRemesaEntrante

        public GenericResponse UpdateStatusAnnulmentRemesaEntrante(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                GenericResponse result = new HashSet<GenericResponse>(_DbContext.Database.SqlQuery<GenericResponse>(
             string.Concat("operaciones.UpdateStatusAnnulmentRemesaEntrante ",
                                "@Id_OPERACION, ",
                                "@STATUS, ",
                                "@ModificadoPor, ",
                                "@Modificado, ",
                                "@FechaAnulacion, ",
                                "@UsuarioAnula, ",
                                "@MotivoAnulacionId, ",
                                "@ObservacionAnulacion "), param)
             ).ToList().FirstOrDefault();

                _return.ReturnId= result.ReturnId;
                _return.ID_OPERACION =result.ID_OPERACION;
                _return.Valid = true;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de UpdateStatusCashierOperations:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region UpdateAnnulment

        public GenericResponse UpdateAnnulment(object[] param)
        {
            var _return = new GenericResponse();
            try
            {
                _return.ReturnId = _DbContext.Database.SqlQuery<int>(
                    string.Concat("operaciones.UpdateAnnulment ",
                                "@StatusId, ",
                                "@TableId, ",
                                "@RowId, ",
                                "@UpdateUser, ",
                                "@UpdateDate, ",
                                "@AnnulmentObservation, ",
                                "@AnnulmentId "), param).First();


                _return.Valid = true;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de UpdateAnnulment:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        #region SearchOperationsFrontOffice
        public HashSet<TEntity> SearchOperationsFrontOffice(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>(
                string.Concat("operaciones.SearchOperationsFrontOffice ",
                                "@Sucursal, ",
                                "@CreacionFrom, ",
                                "@IDENTIFICACION_BENEFICIARIO "), param)
                );
        }
        #endregion

        #region InsertOrdenesEntity
        public GenericResponse InsertOrdenesEntity(Ordenes model)
        {
            var _return = new GenericResponse();
            try
            {
                var Serialize = JsonConvert.SerializeObject(model);
                var Deserialize = JsonConvert.DeserializeObject<DataAccess.Conection.ORDENES>(Serialize);

                db.ORDENES.Add(Deserialize);
                db.SaveChanges();

                _return.Error = false;
                _return.Data = Deserialize;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("Se ha presentado el siguiente error al tratar de insertar  InsertOrdenesEntity:",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }
        #endregion

        #region SearchRemesaEntranteFromOrdenesEntity
        public GenericResponse SearchRemesaEntranteFromOrdenesEntity(int id)
        {
            var _return = new GenericResponse();
            try
            {

                var remesa = db.SearchRemesaEntranteFromOrdenes(id).ToList();
                var Serialize = JsonConvert.SerializeObject(remesa);
                var Deserialize = JsonConvert.DeserializeObject<OrdenCompraEfectivo>(Serialize);
                
                _return.Error = false;
                _return.Data = Deserialize;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("No se logro obtener la información de la remesa entrante: ",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #region ProccesCashierRemesa

        public GenericResponse ProccesCashierRemesa(ProcessCashierOperation model, ProccesOrder ListOrder)
        {
            var result = new GenericResponse();
            var transaction = db.Database.BeginTransaction();
            try
            {
                #region Orden Primaria

                var OrdenPrimary = CreateOrderForInboundRemittance(ListOrder, model);

                if (OrdenPrimary.Error)
                {
                    transaction.Rollback();
                    return OrdenPrimary;
                }

                ORDENES OrderSecond = (ORDENES)OrdenPrimary.Data;
                #endregion

                #region Se Actualiza Operacion Temporal
                var ID_OperationTemp = model.ListOperation.FirstOrDefault().Id_OPERACION;

                var OperationTemp = (from p in db.OPERACIONES_POR_COBRAR
                                     where p.Id_OPERACION == ID_OperationTemp
                                     select p).FirstOrDefault();

                OperationTemp.Status = Convert.ToByte(Constant.StatusOperacionesTemporales.Procesada);
                OperationTemp.OrderId = OrderSecond.ID_ORDEN;
                db.Entry(OperationTemp).State = EntityState.Modified;
                db.SaveChanges();

                #endregion

                #region Se carga resumen y detalle de entrada y salida de Cashier

                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = model.BranchOffice,
                    usuario = model.Login,
                    tipo = Constant.TipoNumeracion.OperacionDeCaja,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    return _operationNumber;
                }

                #endregion

                var cashiers = ProcessOperationCashier(model, _operationNumber.NUMERO);

                #endregion

                #region Pagos de la Orden

                var ResultPaymentOrder = CreatePaymentInboundRemittance(model, ListOrder.Orden, OrderSecond, cashiers);

                if (ResultPaymentOrder.Error)
                {
                    transaction.Rollback();
                    return ResultPaymentOrder;
                }

                #endregion

                #region Orden Secundaria

                if (cashiers.Where(x => x.OriginOperation == Constant.OperacionesCaja.Salida && x.Currency == model.ListOperation.FirstOrDefault().Moneda).Count() > 0)
                {
                    var ResultOrderSecond = CreateSecondOrderForInboundRemittance(ListOrder, OrderSecond, model.BranchOffice, model);

                    if (ResultOrderSecond.Error)
                    {
                        transaction.Rollback();
                        return OrdenPrimary;
                    }

                }

                #endregion

                #region Se Actualiza Remesa Entrante

                var RemesaOrder = (from p in db.REMESAS_ENTRANTES
                                   where p.ID_OPERACION == model.Orden.ID_ORDEN
                                   select p).FirstOrDefault();

                RemesaOrder.STATUS = Convert.ToByte(Constant.StatusOrden.OrdenPagada);
                RemesaOrder.ModificadoPor = model.Login;
                RemesaOrder.ID_ORDEN = OrderSecond.ID_ORDEN;
                RemesaOrder.ObservacionAnulacion = null;
                RemesaOrder.REFERENCIA_BCV = OrderSecond.REFERENCIA_PAGO;
                RemesaOrder.FECHAPAGO = OrderSecond.FECHA_PAGO;
                RemesaOrder.MotivoAnulacionId = null;

                db.Entry(RemesaOrder).State = EntityState.Modified;
                db.SaveChanges();

                #endregion

                #region Tarifas Aplicadas

                if (ListOrder.Orden.tasaCambio == 1)
                {
                    ListOrder.Tarifas_Comiciones = ListOrder.Tarifas_Comiciones.Where(x => x.idTarifa != 39).ToList();
                }

                List<TARIFAS_APLICADAS> _tarifas_Aplicadas = new List<TARIFAS_APLICADAS>();

                //Tarifas aplicadas en USD
                foreach (var item in ListOrder.Tarifas_Comiciones.Where(x => x.moneda.Equals("USD")).ToList())
                {
                    decimal _comisionUs = 0;
                    if (item.valor < 1)
                        _comisionUs = Math.Round(item.valor * ListOrder.Orden.montoOrden, 2);
                    else
                        _comisionUs = Math.Round(item.valor, 2);

                    _tarifas_Aplicadas.Add(new TARIFAS_APLICADAS
                    {
                        TARIFA = item.idTarifa,
                        MONTO = _comisionUs,
                        REGISTRADOPOR = ListOrder.Orden.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        ORDEN = OrderSecond.ID_ORDEN
                    });
                }

                //Tarifas aplicadas en Bolivares
                foreach (var item in ListOrder.Tarifas_Comiciones.Where(x => x.moneda.Equals("VEB")).ToList())
                {
                    decimal _comisionBs = 0;
                    var objTasa = ListOrder.Historial.OrderByDescending(x => x.fechaRegistro).FirstOrDefault();
                    if (item.valor < 1)
                        //Se utiliza valorCompra del bcv porque la tasa venta tiene error en el WS el mismo valor suministrado por
                        //el WS en la tasa de compra pertenece a la tasa de venta BCV
                        _comisionBs = Math.Round(item.valor * Math.Round((ListOrder.Orden.montoOrden * objTasa.valorCompra), 2), 2);
                    else
                        _comisionBs = Math.Round(item.valor, 2);

                    _tarifas_Aplicadas.Add(new TARIFAS_APLICADAS
                    {
                        TARIFA = item.idTarifa,
                        MONTO = _comisionBs,
                        REGISTRADOPOR = ListOrder.Orden.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        ORDEN = OrderSecond.ID_ORDEN

                    });
                }

                db.TARIFAS_APLICADAS.AddRange(_tarifas_Aplicadas);
                db.SaveChanges();

                #endregion

                transaction.Commit();
                result.Error = false;
                result.Valid = true;
                result.ReturnId = OrderSecond.ID_ORDEN;
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion de los Pagos",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }

        }

        #region CreateOrderForInboundRemittance

        private GenericResponse CreateOrderForInboundRemittance(ProccesOrder ListOrder, ProcessCashierOperation model)
        {
            var result = new GenericResponse();
            try
            {
                #region Variables

                var idTipoOperacion = 10; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idMonedaOperacion = 213; //el codigo de la moneda interna
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idPaisDestino = "VE";
                var n = string.Empty;

                #endregion

                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = model.BranchOffice,
                    usuario = ListOrder.Orden.ModificadoPor,
                    tipo = Constant.TipoNumeracion.FacturaDeOperacion,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    return _operationNumber;
                }

                #endregion

                #region Mapeo de Nombres de Beneficiario

                NameBeneficiary NameBeneficiary = SplitName.NameSeparator(ListOrder.Orden.NombresRemitente);

                #endregion

                #region Orden

                Ordenes objOrden = new Ordenes
                {
                    DETALLE_TIPO_OPERACION = idTipoOperacion,
                    CLIENTE = ListOrder.Orden.idCliente,
                    SUCURSAL = ListOrder.Orden.SucursalProcesa,
                    STATUS_ORDEN = Constant.StatusOrden.OrdenPagada,
                    MONEDA = ListOrder.Orden.MonedaConversion,
                    OFICINA = ListOrder.Oficinas.FirstOrDefault().ID_OFIC_EXTERNA,
                    PAIS_DESTINO = idPaisDestino,
                    CORRESPONSAL = "CAL",
                    NUMERO = int.Parse(_operationNumber.NUMERO.ToString()),
                    TIPO_CAMBIO = ListOrder.Orden.tasaCambio,
                    TASA_DESTINO = ListOrder.Orden.tasaCambio,
                    MONTO = ListOrder.Orden.montoOrden,
                    MONTO_CAMBIO = ListOrder.Orden.MontoBolivares,
                    FECHA_VALOR_TASA = ListOrder.Orden.fechaValorTasa,
                    TIPO_OP_BCV = ListOrder.TipoMovimientoss.FirstOrDefault().ID_BCV,
                    NOMBRES_REMITENTE = NameBeneficiary.Names,
                    APELLIDOS_REMITENTE = NameBeneficiary.Surnames,
                    IDENTIFICACION_REMITENTE = ListOrder.Orden.DocumentoRemitente,
                    NOMBRES_BENEFICIARIO = ListOrder.Ficha.FirstOrDefault().PrimerNombre.Trim() + " " + (string.IsNullOrEmpty(ListOrder.Ficha.FirstOrDefault().SegundoNombre) ? string.Empty : ListOrder.Ficha.FirstOrDefault().SegundoNombre.Trim()),
                    APELLIDOS_BENEFICIARIO = ListOrder.Ficha.FirstOrDefault().PrimerApellido.Trim() + " " + (string.IsNullOrEmpty(ListOrder.Ficha.FirstOrDefault().SegundoApellido) ? string.Empty : ListOrder.Ficha.FirstOrDefault().SegundoApellido.Trim()),
                    IDENTIFICACION_BENEFICIARIO = ListOrder.Orden.tipoIdBeneficiario + ListOrder.Orden.numeroIdBeneficiario,
                    //BANCO_NACIONAL = orden.bancoCliente,
                    //NUMERO_CUENTA = orden.numeroCuentaCliente,
                    EMAIL_CLIENTE = "",
                    EMAIL_BENEFICIARIO = ListOrder.Orden.emailCliente,
                    OBSERVACIONES = ListOrder.Orden.observaciones,
                    BANCO_DESTINO = ListOrder.Orden.nombreBancoDestino,
                    NUMERO_CUENTA_DESTINO = ListOrder.Orden.numeroCuentaDestino.ToString(),
                    DIRECCION_BANCO = ListOrder.Orden.direccionBancoDestino,
                    ABA = ListOrder.Orden.aba,
                    SWIFT = ListOrder.Orden.swift,
                    IBAN = ListOrder.Orden.iban,
                    TELEFONO_CLIENTE = ListOrder.Orden.telefonoCliente,
                    REGISTRADOPOR = ListOrder.Orden.ModificadoPor,
                    AGENTE = idPagador,
                    MOTIVO_OP_BCV = ListOrder.Orden.idMotivoOferta,
                    USUARIO_TAQUILLA = ListOrder.Orden.ModificadoPor,
                    TasaConversion = ListOrder.Orden.tasaCambio,
                    MonedaOperacion = idMonedaOperacion,
                    MontoConversion = ListOrder.Orden.montoOrden,
                    //CommissionUsd = comisionUs,
                    BancoPagoTransferencia = ListOrder.Orden.bancoCliente,
                    NumeroCuentaPagoTransferencia = ListOrder.Orden.numeroCuentaCliente,
                    SucursalProcesaId = ListOrder.Orden.SucursalProcesa,
                    ModificadoPor = ListOrder.Orden.ModificadoPor,
                    Modificado = DateTime.Now,
                    FECHA_OPERACION = DateTime.Now,
                    FECHA_PAGO = DateTime.Now,
                    REFERENCIA_PAGO = model.Letra + "-" + int.Parse(_operationNumber.NUMERO.ToString()),
                    //CommissionBs = comisionBs
                    AnulProcesada = false
                };

                #endregion

                #region Se registra operacion BCV

                var responseBCVInt = new XmlDocument();
                responseBCVInt.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                var referenciaBCV = responseBCVInt.SelectSingleNode("//result").InnerText;
                objOrden.REFERENCIA_ORDEN = referenciaBCV;

                #endregion

                #region Se valida si tiene trasnferencia

                if (model.ListPayInternationalMixed.Count() > 0)
                {
                    objOrden.NUMERO_CUENTA_DESTINO = model.ListPayInternationalMixed.FirstOrDefault().NroCuenta;
                }

                #endregion

                #region Insert Orden

                var Serialize = JsonConvert.SerializeObject(objOrden);
                var Deserialize = JsonConvert.DeserializeObject<DataAccess.Conection.ORDENES>(Serialize);

                db.ORDENES.Add(Deserialize);
                db.SaveChanges();

                #endregion

                result.Error = false;
                result.Data = Deserialize;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }

        #endregion

        #region CreatePaymentInboundRemittance
        private GenericResponse CreatePaymentInboundRemittance(ProcessCashierOperation model, IncomingRemittanceToOrder modelEntrante, ORDENES OrdenPrimary, List<Cashiers> cashiers)
        {
            var result = new GenericResponse();
            try
            {
                #region Se crea PAGO

                DataAccess.Conection.PAGOS modelPago = new DataAccess.Conection.PAGOS
                {
                    OrderId = OrdenPrimary.ID_ORDEN,
                    CiuOrig = model.Letra,
                    NroRecibo = OrdenPrimary.NUMERO,
                    Secuencia = 0,
                    Referencia = model.Orden.REFERENCIA_PAGO,
                    Pais = "VZL",
                    Usd = modelEntrante.montoOrden,
                    Tasa = Convert.ToDecimal(OrdenPrimary.TASA_DESTINO),
                    Bolivares = modelEntrante.MontoBolivares,
                    Observaciones = model.Orden.OBSERVACIONES,
                    FechaRegistro = DateTime.Now,
                    Usuario = model.Orden.ModificadoPor,
                    Status = 0
                };

                db.PAGOS.Add(modelPago);
                db.SaveChanges();

                #endregion

                #region DESGLOSE DE EFECTIVO PAGO REALIZADO DIVISA

                #region Crea el Desglose del Efectivo Pagado en la Divisa
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
                                        select new DESGLOSE_PAGO_REALIZADO
                                        {
                                            DENOMINACION = grupo.Key.DenominationId,
                                            CANTIDAD_ENTREGADA = grupo.Sum(x => x.Quantity),
                                            SUBTOTAL_ENTREGADO = grupo.Sum(x => x.Amount),
                                            REGISTRADO = DateTime.Now,
                                            REGISTRADOPOR = model.Login,
                                        }).ToList();
                #endregion

                if (DesgloseEfectivo.Count() > 0)
                {

                    DETALLE_PAGO_REALIZADO modelDetallePagoEfctUsd = new DETALLE_PAGO_REALIZADO
                    {
                        PAGO_REALIZADO = OrdenPrimary.ID_ORDEN,
                        TIPO_PAGO_REALIZADO = model.Orden.Tipo_Pago == 0 ? 1 : model.Orden.Tipo_Pago,
                        DETALLE_TIPO_PAGO = model.Orden.DetalleTipoPago == 0 ? 951 : model.Orden.DetalleTipoPago,
                        REGISTRADOPOR = model.Orden.ModificadoPor,
                        MONTO = DesgloseEfectivo.Sum(x => x.SUBTOTAL_ENTREGADO), //validar
                        REGISTRADO = DateTime.Now,
                        FECHA_TRANSACCION = DateTime.Now,
                        BankAccountsId = model.Orden.BankAccountsId
                    };

                    db.DETALLE_PAGO_REALIZADO.Add(modelDetallePagoEfctUsd);
                    db.SaveChanges();

                    DesgloseEfectivo.Select(S =>
                    {
                        S.DETALLE_PAGO_REALIZADO = modelDetallePagoEfctUsd.ID_DETALLE;
                        return S;
                    }).ToList();

                    db.DESGLOSE_PAGO_REALIZADO.AddRange(DesgloseEfectivo);
                    db.SaveChanges();

                    #region DESGLOSE DE EFECTIVO VUELTO RECIBIDO PAGO

                    #region Se consulta ingreso de billetes del cliente

                    CashierRequest cashierRequest = new CashierRequest();
                    cashierRequest.RowId = model.ListOperation.FirstOrDefault().Id_OPERACION;

                    IContabilidadDal<List<Cashiers>> dalContabilidad = new ContabilidadDal<List<Cashiers>>();
                    var ListCashierinClient = dalContabilidad.SearchCashierEntity(cashierRequest);

                    #endregion

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
                                                      select new DESGLOSE_VUELTO_RECIBIDO_PAGO
                                                      {
                                                          DENOMINACION = grupo.Key.DenominationId,
                                                          CANTIDAD_RECIBIDA = grupo.Sum(x => x.Quantity),
                                                          SUBTOTAL_RECIBIDO = grupo.Sum(x => x.Amount),
                                                          REGISTRADO = DateTime.Now,
                                                          REGISTRADOPOR = model.Login,
                                                      }).ToList();

                        if (DesgloseEfectivoVuelto.Count() > 0)
                        {
                            DesgloseEfectivoVuelto.Select(S =>
                            {
                                S.DETALLE_PAGO_REALIZADO = modelDetallePagoEfctUsd.ID_DETALLE;
                                return S;
                            }).ToList();

                            db.DESGLOSE_VUELTO_RECIBIDO_PAGO.AddRange(DesgloseEfectivoVuelto);
                            db.SaveChanges();
                        }
                    }

                    #endregion
                }

                #endregion

                #region DESGLOSE DE EFECTIVO PAGO REALIZADO BS

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
                                          select new DESGLOSE_PAGO_REALIZADO
                                          {
                                              DENOMINACION = grupo.Key.DenominationId,
                                              CANTIDAD_ENTREGADA = grupo.Sum(x => x.Quantity),
                                              SUBTOTAL_ENTREGADO = grupo.Sum(x => x.Amount),
                                              REGISTRADO = DateTime.Now,
                                              REGISTRADOPOR = model.Login,
                                          }).ToList();

                if (DesgloseEfectivoBs.Count() > 0)
                {
                    var detallePagoRealizadoBs = model.ListOperation.FirstOrDefault().DetallePagoRealizado.Where(x => x.TIPO_PAGO_REALIZADO == Constant.MetodoPago.Efectivo && x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Bolivares).FirstOrDefault();

                    DETALLE_PAGO_REALIZADO modelDetallePagoBs = new DETALLE_PAGO_REALIZADO
                    {
                        PAGO_REALIZADO = OrdenPrimary.ID_ORDEN,
                        TIPO_PAGO_REALIZADO = detallePagoRealizadoBs.TIPO_PAGO_REALIZADO,
                        DETALLE_TIPO_PAGO = detallePagoRealizadoBs.DETALLE_TIPO_PAGO,
                        REGISTRADOPOR = model.Orden.ModificadoPor,
                        MONTO = detallePagoRealizadoBs.MONTO, //validar
                        REGISTRADO = DateTime.Now,
                        FECHA_TRANSACCION = DateTime.Now,
                        BankAccountsId = detallePagoRealizadoBs.BankAccountsId
                    };

                    db.DETALLE_PAGO_REALIZADO.Add(modelDetallePagoBs);
                    db.SaveChanges();

                    DesgloseEfectivo.Select(S =>
                    {
                        S.DETALLE_PAGO_REALIZADO = modelDetallePagoBs.ID_DETALLE;
                        return S;
                    }).ToList();

                    db.DESGLOSE_PAGO_REALIZADO.AddRange(DesgloseEfectivoBs);
                    db.SaveChanges();
                }


                #endregion

                #region Pago de PICO

                if (model.ListPayInternationalMixed.Count() > 0)
                {

                    DataAccess.Conection.OrderPaymentPending paymentPending = new DataAccess.Conection.OrderPaymentPending
                    {
                        OrderId = OrdenPrimary.ID_ORDEN,
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

                    db.OrderPaymentPending.Add(paymentPending);
                    db.SaveChanges();
                }

                #endregion

                #region Se actualiza Cashier

                List<DataAccess.Conection.Cashier> Cashier = new List<DataAccess.Conection.Cashier>();

                foreach (var item in cashiers)
                {
                    List<DataAccess.Conection.CashierDetail> casD = new List<DataAccess.Conection.CashierDetail>();

                    DataAccess.Conection.Cashier cas = new DataAccess.Conection.Cashier()
                    {
                        ProcessId = item.ProcessId,
                        RowId = OrdenPrimary.ID_ORDEN,
                        PassesId = item.PassesId,
                        BranchOffice = item.BranchOffice,
                        OriginOperation = item.OriginOperation,
                        TypeOperaction = item.TypeOperaction,
                        UserId = item.UserID,
                        Currency = item.Currency,
                        DenominationId = item.DenominationId,
                        Quantity = item.Quantity,
                        Amount = item.Amount,
                        ControlNumber = item.ControlNumber,
                        StatusId = item.StatusId,
                        CreationUser = item.CreationUser,
                        UpdateUser = item.UpdateUser,
                        CreationDate = item.CreationDate,
                        UpdateDate = item.UpdateDate,
                    };

                    //Insert del Cashier que se esta Recorriendo
                    db.Cashier.Add(cas);
                    db.SaveChanges();

                    //Se verifica que el Cashier tiene detalle de Billetes 
                    if (item.ListCashierDetail.Count() > 0)
                    {
                        var SerializeCashierDetail = JsonConvert.SerializeObject(item.ListCashierDetail);
                        var DeserializeCashierDetail = JsonConvert.DeserializeObject<List<DataAccess.Conection.CashierDetail>>(SerializeCashierDetail);

                        foreach (var itemDetail in DeserializeCashierDetail)
                        {
                            //Busca el Detalle del Billete Activo
                            var CashDetail = (from p in db.CashierDetail
                                              where p.CurrencyCode == itemDetail.CurrencyCode && p.StatusId == Constant.Status.Activo && p.DenominationId == itemDetail.DenominationId
                                              select p).ToList().FirstOrDefault();

                            // Verifico si existe  el registro
                            if (CashDetail != null)
                            {
                                //Actualiza el Estatus del Billete 
                                CashDetail.StatusId = 2;
                                CashDetail.CashierIdOut = cas.CashierId;
                                CashDetail.UpdateUser = item.CreationUser;
                                CashDetail.UpdateDate = item.CreationDate;
                                db.Entry(CashDetail).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                #region CashierSummary

                var datos = (from x in cashiers.ToList()
                             group x by new
                             {
                                 x.Currency,
                                 x.DenominationId,
                                 x.BranchOffice,
                                 x.UserID,
                                 x.OriginOperation,
                                 x.CreationUser
                             } into grupo
                             select new DataAccess.Conection.CashierSummary
                             {
                                 BranchOffice = grupo.Key.BranchOffice,
                                 Currency = grupo.Key.Currency,
                                 UserId = grupo.Key.UserID,
                                 DenominationId = grupo.Key.DenominationId,
                                 Quantity = grupo.Sum(x => x.Quantity),
                                 Amount = grupo.Sum(x => x.Amount),
                                 CreationDate = DateTime.Now,
                                 CreationUser = grupo.Key.CreationUser,
                             }).ToList();

                foreach (var datosCashiers in datos.ToList())
                {
                    var QuantityNew = 0;
                    decimal AmountNew = 0;

                    var Summary = (from vs in db.CashierSummary
                                   where vs.BranchOffice == datosCashiers.BranchOffice &&
                                   vs.Currency == datosCashiers.Currency &&
                                   vs.DenominationId == datosCashiers.DenominationId &&
                                   vs.UserId == datosCashiers.UserId
                                   select vs).OrderByDescending(x => x.CreationDate).FirstOrDefault();

                    if (Summary != null)
                    {
                        QuantityNew = Summary.Quantity - datosCashiers.Quantity;
                        AmountNew = Summary.Amount - datosCashiers.Amount;

                        if (Summary.CreationDate.Date == DateTime.Now.Date)
                        {
                            Summary.Quantity = QuantityNew;
                            Summary.Amount = AmountNew;
                            Summary.UpdateUser = datosCashiers.CreationUser;
                            Summary.UpdateDate = DateTime.Now;
                            db.Entry(Summary).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Summary.StatusId = 2;
                            Summary.UpdateUser = datosCashiers.CreationUser;
                            Summary.UpdateDate = DateTime.Now;
                            db.Entry(Summary).State = EntityState.Modified;
                            db.SaveChanges();

                            DataAccess.Conection.CashierSummary SaveSummary = new DataAccess.Conection.CashierSummary()
                            {
                                BranchOffice = datosCashiers.BranchOffice,
                                Currency = datosCashiers.Currency,
                                DenominationId = datosCashiers.DenominationId,
                                Quantity = QuantityNew,
                                Amount = AmountNew,
                                CreationUser = datosCashiers.CreationUser,
                                CreationDate = datosCashiers.CreationDate,
                                StatusId = Constant.Status.Activo,
                                UserId = datosCashiers.UserId
                            };

                            db.CashierSummary.Add(SaveSummary);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        DataAccess.Conection.CashierSummary SaveSummary = new DataAccess.Conection.CashierSummary()
                        {
                            BranchOffice = datosCashiers.BranchOffice,
                            Currency = datosCashiers.Currency,
                            DenominationId = datosCashiers.DenominationId,
                            Quantity = datosCashiers.Quantity,
                            Amount = datosCashiers.Amount,
                            CreationUser = datosCashiers.CreationUser,
                            CreationDate = datosCashiers.CreationDate,
                            StatusId = Constant.Status.Activo,
                            UserId = datosCashiers.UserId
                        };

                        db.CashierSummary.Add(SaveSummary);
                        db.SaveChanges();
                    }
                }
                #endregion

                #endregion

                result.Error = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = "Ha ocurrido un error al procesar los pagos de la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }
        #endregion

        #region CreateSecondOrderForInboundRemittance

        private GenericResponse CreateSecondOrderForInboundRemittance(ProccesOrder ListOrder, ORDENES OrdenPrimary, string BranchOffice, ProcessCashierOperation model)
        {
            var result = new GenericResponse();
            try
            {
                #region Variables
                OrdenPrimary.TARIFAS_APLICADAS = null;
                var Serialize = JsonConvert.SerializeObject(OrdenPrimary);
                var Deserialize = JsonConvert.DeserializeObject<Ordenes>(Serialize);
                Deserialize.CORRESPONSAL = "CAL";
                Deserialize.PAIS_DESTINO = "VE";
                Deserialize.DETALLE_TIPO_OPERACION = Constant.TipoOperaciones.Ventas;
                Deserialize.NOMBRES_BENEFICIARIO = "Casa de Cambio";
                Deserialize.APELLIDOS_BENEFICIARIO = "Angulo López";
                Deserialize.IDENTIFICACION_BENEFICIARIO = "R302075661";
                Deserialize.AGENTE = "CAL";
                Deserialize.ID_ORDEN_FK = OrdenPrimary.ID_ORDEN;
                Deserialize.NOMBRES_REMITENTE = OrdenPrimary.NOMBRES_BENEFICIARIO;
                Deserialize.APELLIDOS_REMITENTE = OrdenPrimary.APELLIDOS_BENEFICIARIO;
                Deserialize.IDENTIFICACION_REMITENTE = OrdenPrimary.IDENTIFICACION_BENEFICIARIO;
                #endregion

                #region Se Consulta el tipo de Operacion BCV

                var TipoMovimientoRequest = new TipoMovimientoRequest
                {
                    idTipoIdentidad = ListOrder.Orden.tipoIdCliente,
                    TipoOperacion = Constant.ProcesosAsociadosCajero.CodVentaEfectivo
                };

                ISimadiDal<List<TipoMovimientosSimadi>> dalSimadi = new SimadiDal<List<TipoMovimientosSimadi>>();
                var TipoMovimiento = dalSimadi.SearchTipoMovimiento(TipoMovimientoRequest);

                if (!TipoMovimiento.Any())
                {
                    result.Error = true;
                    result.ErrorMessage = "Se presenta error el tratar de consultar tipo de movimiento en pago de remesa";
                    return result;
                }

                Deserialize.TIPO_OP_BCV = TipoMovimiento.FirstOrDefault().ID_BCV;
                #endregion

                // OJO VALIDAR CON JOHAN el idico que deberian registrarse con 1
                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = BranchOffice,
                    usuario = ListOrder.Orden.ModificadoPor,
                    tipo = Constant.TipoNumeracion.Solicitudes,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    return _operationNumber;
                }

                Deserialize.NUMERO = int.Parse(_operationNumber.NUMERO.ToString());
                #endregion

                #region Se registra operacion BCV

                var responseBCVInt = new XmlDocument();
                responseBCVInt.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                var referenciaBCV = responseBCVInt.SelectSingleNode("//result").InnerText;
                Deserialize.REFERENCIA_ORDEN = referenciaBCV;

                #endregion

                #region Insert Orden
                var SerializeOrden = JsonConvert.SerializeObject(Deserialize);
                var DeserializeOrden = JsonConvert.DeserializeObject<DataAccess.Conection.ORDENES>(SerializeOrden);

                db.ORDENES.Add(DeserializeOrden);
                db.SaveChanges();

                result.Error = false;

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }

        #endregion

        #region ProcessOperationCashier

        public List<Cashiers> ProcessOperationCashier(ProcessCashierOperation model, long operationNumber)
        {
            List<CashierMethodPay> ResumenInPay = null; List<CashierMethodPay> ResumenOutPay = null;
            List<CashierSummarys> ResumenOut = null; List<CashierSummarys> ResumenIn = null;
            List<CashierDetails> DetailIn = null; List<CashierDetails> DetailOut = null;
            var TypeOperation = 0;
            var Fecha = DateTime.Now;
            model.ListCashierDetail.Select(S =>
            {
                S.CreationUser = model.Login.ToUpper();
                S.StatusId = Constant.Status.Activo;
                S.CreationDate = Fecha;
                return S;
            }).ToList();
            if (model.ListOperation.FirstOrDefault().Tipo_Operacion == Constant.TipoOperaciones.Ventas)
            {
                // Resumen y Detalle de Entrada
                ResumenInPay = model.ListCashierMethodPay.Where(x => x.In == true && x.Out == false && x.IdMetodoPago == 1).ToList();
                DetailIn = model.ListCashierDetail.Where(x => x.In == true && x.Out == false).ToList();
                // Resumen y Detalle de Salida
                ResumenOutPay = model.ListCashierMethodPay.Where(x => x.In == false && x.Out == true && x.IdMetodoPago == 0).ToList();
                ResumenOut = model.ListCashierSumamry.Where(x => x.QuantitySelect != null).ToList();
                DetailOut = model.ListCashierDetail.Where(x => x.In == false && x.Out == true).ToList();

                switch (model.ListOperation.FirstOrDefault().Mensaje.Trim())
                {
                    case "venta-simadi-taq":
                        TypeOperation = Constant.ProcesosAsociadosCajero.VentaDivisaEfectivo;
                        break;
                    case "venta-simadi-enc":
                        TypeOperation = Constant.ProcesosAsociadosCajero.VentaDivisaEncomienda;
                        break;
                    default:
                        break;
                }

            }
            else if (model.ListOperation.FirstOrDefault().Tipo_Operacion == Constant.TipoOperaciones.Compras)
            {
                // Resumen y Detalle de Entrada
                ResumenIn = model.ListCashierSumamry.Where(x => x.OriginOperation == Constant.OperacionesCaja.Entrada).ToList();
                DetailIn = model.ListCashierDetail.Where(x => x.In == true && x.Out == false).ToList();
                ResumenOut = model.ListCashierSumamry.Where(x => x.OriginOperation == Constant.OperacionesCaja.Salida).ToList();
                //Resumen y Detalle de Salida
                ResumenOutPay = model.ListCashierMethodPay.Where(x => x.In == false && x.Out == true && x.IdMetodoPago == 1).ToList();
                DetailOut = model.ListCashierDetail.Where(x => x.In == false && x.Out == true).ToList();
                switch (model.ListOperation.FirstOrDefault().Mensaje.Trim())
                {
                    case "compra-simadi-taq":
                        TypeOperation = Constant.ProcesosAsociadosCajero.CompraDivisaEfectivo;
                        break;

                    default:
                        break;
                }
            }
            else
            {

                // Resumen y Detalle de Salida
                ResumenOut = model.ListCashierSumamry.Where(x => x.QuantitySelect != null).ToList();
                DetailOut = model.ListCashierDetail.Where(x => x.In == false && x.Out == true).ToList();
                TypeOperation = Constant.ProcesosAsociadosCajero.PagoGiroInter;

            }

            #region Cashier

            //Se arma Toda la Clase Cashier con sus Detalles 
            List<Cashiers> cashiers = new List<Cashiers>();
            if (ResumenIn != null)
            {
                foreach (var item in ResumenIn)
                {
                    Cashiers cashier = new Cashiers();
                    cashier.Currency = item.Currency; cashier.Amount = item.Amount; cashier.Quantity = item.Quantity; cashier.OriginOperation = Convert.ToInt32(item.OriginOperation);
                    cashier.DenominationId = Convert.ToInt32(item.DenominationId); cashier.StatusId = 1; cashier.ProcessId = item.ProcessId;
                    cashier.ListCashierDetail = DetailIn.Where(x => x.Currency == item.Currency && x.DenominationId == item.DenominationId).ToList();
                    cashiers.Add(cashier);
                }
            }

            if (ResumenOut != null)
            {
                foreach (var item in ResumenOut)
                {
                    Cashiers cashier = new Cashiers();
                    cashier.Currency = item.Currency; cashier.Amount = (Convert.ToDecimal(item.DenominacionName) * Convert.ToDecimal(item.QuantitySelect));
                    cashier.Quantity = Convert.ToInt32(item.QuantitySelect); cashier.OriginOperation = Convert.ToInt32(item.OriginOperation); cashier.ProcessId = item.ProcessId;
                    cashier.DenominationId = Convert.ToInt32(item.DenominationId);
                    cashier.ListCashierDetail = DetailOut.Where(x => x.Currency == item.Currency && Convert.ToDecimal(x.DenominacionName) == item.DenominacionName).ToList();
                    cashiers.Add(cashier);
                }
            }

            if (ResumenInPay != null)
            {
                foreach (var item in ResumenInPay)
                {
                    Cashiers cashier = new Cashiers();
                    cashier.Currency = item.MonedaTemp; cashier.Amount = item.Totales; cashier.Quantity = item.Quantity; cashier.OriginOperation = item.OriginOperation;
                    cashier.DenominationId = Convert.ToInt32(item.denominations); cashier.StatusId = 1; cashier.ProcessId = item.ProcessId;
                    cashier.ListCashierDetail = DetailIn.Where(x => x.Currency == item.MonedaTemp && Convert.ToDecimal(x.DenominacionName) == item.DenominacionTemp).ToList();
                    cashiers.Add(cashier);
                }
            }

            if (ResumenOutPay != null)
            {
                foreach (var item in ResumenOutPay)
                {
                    Cashiers cashier = new Cashiers();
                    cashier.Currency = item.MonedaTemp; cashier.Amount = ((Convert.ToDecimal(item.Totales) < 0) ? Convert.ToDecimal(item.Totales) * (-1) : Convert.ToDecimal(item.Totales)); cashier.Quantity = item.Quantity; cashier.OriginOperation = item.OriginOperation;
                    cashier.DenominationId = Convert.ToInt32(item.denominations); cashier.ProcessId = item.ProcessId;
                    cashier.ListCashierDetail = DetailOut.Where(x => x.Currency == item.MonedaTemp && Convert.ToDecimal(x.DenominacionName) == item.DenominacionTemp).ToList();
                    cashiers.Add(cashier);
                }
            }

            cashiers.Select(S =>

            {
                S.BranchOffice = model.BranchOffice;
                S.UserID = model.UserId;
                S.ControlNumber = operationNumber;
                S.StatusId = Constant.Status.Activo;
                S.ProcessId = Constant.ProcesosAsociadosCajero.ProcessOperacion;
                S.CreationUser = model.Login;
                S.CreationDate = Fecha;
                S.TypeOperaction = TypeOperation;
                return S;
            }).ToList();
            //Para realizar Pruebas desde el Controller
            //var xxx = InsertAllCashier(cashiers); 
            #endregion
            return cashiers;

        }
        #endregion

        #endregion

        #region ProcessCashierOperations

        public GenericResponse ProcessCashierOperations(ProcessCashierOperation model, ProccesOrder ListOrder)
        {
            var result = new GenericResponse();
            var transaction = db.Database.BeginTransaction();
            try
            {
                #region Orden

                var OrdenPrimary = CreateOrderForOutboundOperations(ListOrder, model);

                if (OrdenPrimary.Error)
                {
                    transaction.Rollback();
                    return OrdenPrimary;
                }

                ORDENES Order = (ORDENES)OrdenPrimary.Data;
                #endregion

                #region ACTULIZAR TARIFAS APLICADAS
                var OperationTemp = ListOrder.Temporal.NumeroOpTemporal;

                var TariApli = (from p in db.TARIFAS_APLICADAS
                                where p.TEMPORAL == OperationTemp
                                select p).ToList();

                TariApli.Select(S =>
                {
                    S.ORDEN = Order.ID_ORDEN;
                    return S;
                }).ToList();

                TariApli.ForEach(type => db.Entry(type).State = EntityState.Modified);
                db.SaveChanges();

                #endregion

                #region Se Actualiza Operacion Temporal

                var ID_OperationTemp = ListOrder.Temporal.Id_OPERACION;

                var OperatioTemp = (from p in db.OPERACIONES_POR_COBRAR
                                     where p.Id_OPERACION == ID_OperationTemp
                                     select p).FirstOrDefault();

                OperatioTemp.Status = Convert.ToByte(Constant.StatusOperacionesTemporales.Procesada);
                OperatioTemp.OrderId = Order.ID_ORDEN;
                db.Entry(OperatioTemp).State = EntityState.Modified;
                db.SaveChanges();

                #endregion

                #region Pagos de la Orden

                var ResultPaymentOrder = CreatePaymentForOutboundOperations(model, ListOrder, Order);

                if (ResultPaymentOrder.Error)
                {
                    transaction.Rollback();
                    return ResultPaymentOrder;
                }

                #endregion

                transaction.Commit();
                result.Error = false;
                result.Valid = true;
                result.ID_OPERACION = Order.ID_ORDEN;

                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion de los Pagos",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }

        }

        #region CreateOrderForOutboundOperations

        private GenericResponse CreateOrderForOutboundOperations(ProccesOrder ListOrder, ProcessCashierOperation model)
        {
            var result = new GenericResponse();
            try
            {
                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = model.BranchOffice,
                    usuario = model.Login.ToUpper(),
                    tipo = Constant.TipoNumeracion.FacturaDeOperacion,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    return _operationNumber;
                }

                #endregion

                #region Orden

                Ordenes objOrden = new Ordenes
                {
                    CLIENTE = 0,
                    NUMERO = _operationNumber.NUMERO,
                    DETALLE_TIPO_OPERACION = ListOrder.Temporal.Tipo_Operacion,
                    FECHA_OPERACION = DateTime.Now,
                    FECHA_VALOR_TASA = ListOrder.Temporal.FECHA_VALOR_TASA,
                    MOTIVO_OP_BCV = ListOrder.Temporal.MOTIVO_OP_BCV,
                    TIPO_OP_BCV = ListOrder.Temporal.TIPO_OP_BCV,
                    MONTO = ListOrder.Temporal.Dolares,
                    TIPO_CAMBIO = ListOrder.Temporal.TasaDolar,
                    AGENTE = ListOrder.Temporal.Pagador,
                    CORRESPONSAL = ListOrder.Temporal.CORRESPONSAL,
                    OFICINA = ListOrder.Temporal.Oficina,
                    NOMBRES_REMITENTE = ListOrder.Temporal.NomRemA.Trim() + " " + (string.IsNullOrEmpty(ListOrder.Temporal.NomRemB) ? string.Empty : ListOrder.Temporal.NomRemB.Trim()),
                    APELLIDOS_REMITENTE = ListOrder.Temporal.ApeRemA.Trim() + " " + (string.IsNullOrEmpty(ListOrder.Temporal.ApeRemB) ? string.Empty : ListOrder.Temporal.ApeRemB.Trim()),
                    IDENTIFICACION_REMITENTE = ListOrder.Temporal.Cirem,
                    NOMBRES_BENEFICIARIO = ListOrder.Temporal.NomDesA.Trim() + " " + (string.IsNullOrEmpty(ListOrder.Temporal.NomDesB) ? string.Empty : ListOrder.Temporal.NomDesB.Trim()),
                    APELLIDOS_BENEFICIARIO = ListOrder.Temporal.ApeDesA.Trim() + " " + (string.IsNullOrEmpty(ListOrder.Temporal.ApeDesB) ? string.Empty : ListOrder.Temporal.ApeDesB.Trim()),
                    IDENTIFICACION_BENEFICIARIO = ListOrder.Temporal.Ccdes,
                    REFERENCIA_PAGO = ListOrder.Temporal.CiuOrig + "-" + _operationNumber.NUMERO,
                    REFERENCIA_ORDEN = ListOrder.Temporal.Cadivi,
                    BANCO_NACIONAL = ListOrder.Temporal.BANCO_NACIONAL,
                    NUMERO_CUENTA = ListOrder.Temporal.NUMERO_CUENTA,
                    EMAIL_CLIENTE = ListOrder.Temporal.EMAIL_CLIENTE,
                    EMAIL_BENEFICIARIO = ListOrder.Temporal.EMAIL_BENEFICIARIO,
                    BANCO_DESTINO = ListOrder.Temporal.BANCO_DESTINO,
                    NUMERO_CUENTA_DESTINO = ListOrder.Temporal.NUMERO_CUENTA_DESTINO,
                    DIRECCION_BANCO = ListOrder.Temporal.DIRECCION_BANCO,
                    ABA = ListOrder.Temporal.ABA,
                    SWIFT = ListOrder.Temporal.SWIFT,
                    IBAN = ListOrder.Temporal.IBAN,
                    TELEFONO_BENEFICIARIO = ListOrder.Temporal.TelDes,
                    TELEFONO_CLIENTE = ListOrder.Temporal.TelRem,
                    OBSERVACIONES = ListOrder.Temporal.Observaciones,
                    BANCO_INTERMEDIARIO = ListOrder.Temporal.BANCO_INTERMEDIARIO,
                    NUMERO_CUENTA_INTERMEDIARIO = ListOrder.Temporal.NUMERO_CUENTA_INTERMEDIARIO,
                    DIRECCION_BANCO_INTERMEDIARIO = ListOrder.Temporal.DIRECCION_BANCO_INTERMEDIARIO,
                    ABA_INTERMEDIARIO = ListOrder.Temporal.ABA_INTERMEDIARIO,
                    SWIFT_INTERMEDIARIO = ListOrder.Temporal.SWIFT_INTERMEDIARIO,
                    IBAN_INTERMEDIARIO = ListOrder.Temporal.IBAN_INTERMEDIARIO,
                    MonedaOperacion = ListOrder.Temporal.MonedaOperacion,
                    TasaConversion = ListOrder.Temporal.TasaConversion,
                    MontoConversion = ListOrder.Temporal.MontoConversion,
                    MONEDA = Convert.ToInt32(ListOrder.Temporal.MONEDA),
                    AnulProcesada = false,
                    TASA_DESTINO = ListOrder.Temporal.TasaDolar,
                    REGISTRADOPOR = model.Login.ToUpper(),
                    STATUS_ORDEN = Constant.StatusOrden.OrdenCobrada,
                    SUCURSAL = Convert.ToInt32(model.IdOffice),
                    USUARIO_TAQUILLA = model.Login.ToUpper(),
                    PAIS_DESTINO = ListOrder.Country.FirstOrDefault().ISO_CODE
                };

                //Validacion Tipo de Operacion a Procesar
                if (ListOrder.Temporal.Tipo_Operacion == Constant.TipoOperaciones.Ventas)
                {
                    objOrden.MONTO_CAMBIO = ListOrder.Temporal.Total_Cobrar;
                }
                else
                {
                    objOrden.MONTO_CAMBIO = ListOrder.Temporal.MonedaDest;
                }

                #region Detalle Oficina

                if (ListOrder.Temporal.TIPO_OP_BCV.Trim() == "CCVENV" || ListOrder.Temporal.TIPO_OP_BCV.Trim() == "CCEENV" || ListOrder.Temporal.TIPO_OP_BCV.Trim() == "CCPENV")
                {
                    IOficinasDal<List<DetalleOficina>> dalOficinas = new OficinasDal<List<DetalleOficina>>();
                    var DetalleOficina = dalOficinas.SearchDetalleOficina(new DetalleOficinaRequest { idOficina = ListOrder.Temporal.Oficina }).ToList();

                    objOrden.TASA_DESTINO = DetalleOficina.FirstOrDefault().tasa;
                    objOrden.MONTO_CAMBIO = ListOrder.Temporal.MonedaDest;
                    objOrden.MontoConversion = null;
                    objOrden.TasaConversion = null;
                    objOrden.MonedaOperacion = 213;
                }

                #endregion

                if (ListOrder.Temporal.TIPO_OP_BCV != null && ListOrder.Temporal.TIPO_OP_BCV.Trim() == Constant.TipoOperacionBCV.CompraDivisaEfectivo)
                {
                    if (model.ListOperation.FirstOrDefault().DetallePagoRealizado.ToList().Where(x => x.TIPO_PAGO_REALIZADO == Constant.MetodoPago.Transferencia).Count() > 0)
                    {
                        objOrden.STATUS_ORDEN = Constant.StatusOrden.OrdenEnProcesoPago;
                    }
                }

                #endregion

                #region Insert Orden

                var Serialize = JsonConvert.SerializeObject(objOrden);
                var Deserialize = JsonConvert.DeserializeObject<DataAccess.Conection.ORDENES>(Serialize);

                db.ORDENES.Add(Deserialize);
                db.SaveChanges();

                #endregion

                result.Error = false;
                result.Data = Deserialize;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }

        #endregion

        #region CreatePaymentForOutboundOperations

        private GenericResponse CreatePaymentForOutboundOperations(ProcessCashierOperation model, ProccesOrder ListOrder, ORDENES Order)
        {
            var result = new GenericResponse();
            try
            {
                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = model.BranchOffice,
                    usuario = model.Login,
                    tipo = Constant.TipoNumeracion.OperacionDeCaja,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    return _operationNumber;
                }

                #endregion

                #region Se carga resumen y detalle de entrada y salida de Cashier

                var cashiers = ProcessOperationCashier(model, _operationNumber.NUMERO);

                #endregion

                #region Se actualiza Cashier

                List<DataAccess.Conection.Cashier> Cashier = new List<DataAccess.Conection.Cashier>();

                foreach (var item in cashiers)
                {
                    List<DataAccess.Conection.CashierDetail> casD = new List<DataAccess.Conection.CashierDetail>();

                    DataAccess.Conection.Cashier cas = new DataAccess.Conection.Cashier()
                    {
                        ProcessId = item.ProcessId,
                        RowId = Order.ID_ORDEN,
                        PassesId = item.PassesId,
                        BranchOffice = item.BranchOffice,
                        OriginOperation = item.OriginOperation,
                        TypeOperaction = item.TypeOperaction,
                        UserId = item.UserID,
                        Currency = item.Currency,
                        DenominationId = item.DenominationId,
                        Quantity = item.Quantity,
                        Amount = item.Amount,
                        ControlNumber = item.ControlNumber,
                        StatusId = item.StatusId,
                        CreationUser = item.CreationUser,
                        UpdateUser = item.UpdateUser,
                        CreationDate = item.CreationDate,
                        UpdateDate = item.UpdateDate,
                    };

                    //Insert del Cashier que se esta Recorriendo
                    db.Cashier.Add(cas);
                    db.SaveChanges();

                    //Se verifica que el Cashier tiene detalle de Billetes 
                    if (item.ListCashierDetail.Count() > 0)
                    {
                        var ListCashier = item.ListCashierDetail.Where(x => x.Currency != Constant.CodigoMoneda.Bolivares);
                        var SerializeCashierDetail = JsonConvert.SerializeObject(ListCashier);
                        var DeserializeCashierDetail = JsonConvert.DeserializeObject<List<DataAccess.Conection.CashierDetail>>(SerializeCashierDetail);

                        foreach (var itemDetail in DeserializeCashierDetail)
                        {
                            //Busca el Detalle del Billete Activo
                            var CashDetail = (from p in db.CashierDetail
                                              where p.CurrencyCode == itemDetail.CurrencyCode && 
                                              p.StatusId == Constant.Status.Activo && 
                                              p.DenominationId == itemDetail.DenominationId
                                              select p).ToList().FirstOrDefault();

                            // Verifico si existe  el registro
                            if (CashDetail != null)
                            {
                                //Actualiza el Estatus del Billete 
                                CashDetail.StatusId = 2;
                                CashDetail.CashierIdOut = cas.CashierId;
                                CashDetail.UpdateUser = item.CreationUser;
                                CashDetail.UpdateDate = item.CreationDate;
                                db.Entry(CashDetail).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                DataAccess.Conection.CashierDetail SaveCashierDetail = new DataAccess.Conection.CashierDetail()
                                {
                                    CashierId = cas.CashierId,
                                    DenominationId = itemDetail.DenominationId,
                                    CurrencyCode = itemDetail.CurrencyCode,
                                    StatusId = Constant.Status.Activo,
                                    CreationUser = item.CreationUser,
                                    CreationDate = item.CreationDate,
                                };

                                db.CashierDetail.Add(SaveCashierDetail);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                #region CashierSummary

                var datos = (from x in cashiers.ToList()
                             group x by new
                             {
                                 x.Currency,
                                 x.DenominationId,
                                 x.BranchOffice,
                                 x.UserID,
                                 x.OriginOperation,
                                 x.CreationUser
                             } into grupo
                             select new DataAccess.Conection.CashierSummary
                             {
                                 BranchOffice = grupo.Key.BranchOffice,
                                 Currency = grupo.Key.Currency,
                                 UserId = grupo.Key.UserID,
                                 DenominationId = grupo.Key.DenominationId,
                                 Quantity = grupo.Sum(x => x.Quantity),
                                 Amount = grupo.Sum(x => x.Amount),
                                 CreationDate = DateTime.Now,
                                 CreationUser = grupo.Key.CreationUser,
                             }).ToList();

                foreach (var datosCashiers in datos.ToList())
                {
                    var QuantityNew = 0;
                    decimal AmountNew = 0;

                    var Summary = (from vs in db.CashierSummary
                                   where vs.BranchOffice == datosCashiers.BranchOffice &&
                                   vs.Currency == datosCashiers.Currency &&
                                   vs.DenominationId == datosCashiers.DenominationId &&
                                   vs.UserId == datosCashiers.UserId
                                   select vs).OrderByDescending(x => x.CreationDate).FirstOrDefault();

                    if (Summary != null)
                    {
                        var origin = (from vs in cashiers.ToList()
                                     where vs.BranchOffice == datosCashiers.BranchOffice &&
                                     vs.Currency == datosCashiers.Currency &&
                                     vs.DenominationId == datosCashiers.DenominationId &&
                                     vs.UserID == datosCashiers.UserId
                                     select vs).OrderByDescending(x => x.CreationDate).FirstOrDefault();

                        if (origin.OriginOperation == 70)
                        {
                            QuantityNew = Summary.Quantity + datosCashiers.Quantity;
                            AmountNew = Summary.Amount + datosCashiers.Amount;
                        }
                        else
                        {
                            QuantityNew = Summary.Quantity - datosCashiers.Quantity;
                            AmountNew = Summary.Amount - datosCashiers.Amount;
                        }

                        if (Summary.CreationDate.Date == DateTime.Now.Date)
                        {
                            Summary.Quantity = QuantityNew;
                            Summary.Amount = AmountNew;
                            Summary.UpdateUser = datosCashiers.CreationUser;
                            Summary.UpdateDate = DateTime.Now;
                            db.Entry(Summary).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Summary.StatusId = 2;
                            Summary.UpdateUser = datosCashiers.CreationUser;
                            Summary.UpdateDate = DateTime.Now;
                            db.Entry(Summary).State = EntityState.Modified;
                            db.SaveChanges();

                            DataAccess.Conection.CashierSummary SaveSummary = new DataAccess.Conection.CashierSummary()
                            {
                                BranchOffice = datosCashiers.BranchOffice,
                                Currency = datosCashiers.Currency,
                                DenominationId = datosCashiers.DenominationId,
                                Quantity = QuantityNew,
                                Amount = AmountNew,
                                CreationUser = datosCashiers.CreationUser,
                                CreationDate = datosCashiers.CreationDate,
                                StatusId = Constant.Status.Activo,
                                UserId = datosCashiers.UserId
                            };

                            db.CashierSummary.Add(SaveSummary);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        DataAccess.Conection.CashierSummary SaveSummary = new DataAccess.Conection.CashierSummary()
                        {
                            BranchOffice = datosCashiers.BranchOffice,
                            Currency = datosCashiers.Currency,
                            DenominationId = datosCashiers.DenominationId,
                            Quantity = datosCashiers.Quantity,
                            Amount = datosCashiers.Amount,
                            CreationUser = datosCashiers.CreationUser,
                            CreationDate = datosCashiers.CreationDate,
                            StatusId = Constant.Status.Activo,
                            UserId = datosCashiers.UserId
                        };

                        db.CashierSummary.Add(SaveSummary);
                        db.SaveChanges();
                    }
                }
                #endregion

                #endregion

                #region Se Inserta el registro del Pago Recibido

                PAGOS_RECIBIDOS_CLIENTE pAGOS_RECIBIDOS_CLIENTE = new PAGOS_RECIBIDOS_CLIENTE()
                {
                    SUCURSAL = model.BranchOffice,
                    REGISTRADOPOR = model.Login.ToUpper(),
                    REGISTRADO = DateTime.Now,
                    FICHA = ListOrder.Ficha.FirstOrDefault().FichaConsecutivo,
                    NUMEROID = ListOrder.Ficha.FirstOrDefault().id_cedula,
                    TIPO_IDENTIDAD = ListOrder.Ficha.FirstOrDefault().ClienteTipo,
                    MONTO_TOTAL = model.ListOperation.FirstOrDefault().PagosRecibidos.MONTO_TOTAL
                };

                db.PAGOS_RECIBIDOS_CLIENTE.Add(pAGOS_RECIBIDOS_CLIENTE);
                db.SaveChanges();

                #endregion

                #region Se Inserta el registro de Orden Relacionada Pago

                ORDENES_RELACIONADAS_PAGOS oRDENES_RELACIONADAS_PAGOS = new ORDENES_RELACIONADAS_PAGOS() 
                { 
                    PAGO_CLIENTE = pAGOS_RECIBIDOS_CLIENTE.ID_PAGO,
                    REGISTRADOPOR = model.Login.ToUpper(),
                    REGISTRADO = DateTime.Now,
                    NUMERO = _operationNumber.NUMERO.ToString(),
                    ID_GNC = 0,
                    ID_GIR = 0,
                    ID_FAC = 0,
                    ID_GPR = 0,
                };

                switch (Order.DETALLE_TIPO_OPERACION)
                {
                    case 1:
                        oRDENES_RELACIONADAS_PAGOS.ID_GNC = Order.ID_ORDEN;
                        break;
                    case 2:
                        oRDENES_RELACIONADAS_PAGOS.ID_GIR = Order.ID_ORDEN;
                        break;
                    case 3:
                        oRDENES_RELACIONADAS_PAGOS.ID_FAC = Order.ID_ORDEN;
                        break;
                    case 4:
                        oRDENES_RELACIONADAS_PAGOS.ID_GPR = Order.ID_ORDEN;
                        break;
                }

                db.ORDENES_RELACIONADAS_PAGOS.Add(oRDENES_RELACIONADAS_PAGOS);
                db.SaveChanges();

                #endregion

                #region Se inserta el detalle de pago recibido 

                foreach (var item in model.ListOperation.FirstOrDefault().PagosRecibidos.ListDetallePago)
                {
                    DETALLE_PAGO_RECIBIDO dETALLE_PAGO_RECIBIDO = new DETALLE_PAGO_RECIBIDO()
                    {
                        PAGO_CLIENTE = pAGOS_RECIBIDOS_CLIENTE.ID_PAGO,
                        TIPO_PAGO_RECIBIDO = item.TIPO_PAGO_RECIBIDO,
                        DETALLE_TIPO_PAGO = item.DETALLE_TIPO_PAGO,
                        REGISTRADOPOR = model.Login.ToUpper(),
                        MONTO = item.MONTO,
                        REGISTRADO = DateTime.Now,
                        REFERENCIA_1 = item.REFERENCIA_1,
                        REFERENCIA_2 = item.REFERENCIA_2,
                        REFERENCIA_3 = item.REFERENCIA_3,
                        FECHA_TRANSACCION = item.FECHA_TRANSACCION,
                        STATUS_PAGO = item.STATUS_PAGO,
                        FECHA_TOPE_RECUPERACION_CHEQUE = item.FECHA_TOPE_RECUPERACION_CHEQUE,
                        DIAS_TOPE_RECUPERACION = item.DIAS_TOPE_RECUPERACION,
                        PaymentSupportPath = item.PaymentSupportPath,
                        OriginBank = item.OriginBank,
                        OriginBankAccount = item.OriginBankAccount,
                        BankAccountsId = item.BankAccountsId
                    };

                    db.DETALLE_PAGO_RECIBIDO.Add(dETALLE_PAGO_RECIBIDO);
                    db.SaveChanges();

                    #region Verifa si tiene un desglose de Efectivo

                    if (item.ListDesgloseEfectivoRecibido != null)
                    {
                        if (item.ListDesgloseEfectivoRecibido.Count() > 0)
                        {
                            item.ListDesgloseEfectivoRecibido.Select(S =>
                            {
                                S.DETALLE_PAGO_RECIBIDO = dETALLE_PAGO_RECIBIDO.ID_DETALLE;
                                S.REGISTRADOPOR = model.Login;
                                return S;
                            }).ToList();
                        }
                    }
                    #endregion

                    #region Verifa si tiene un desglose de Efectivo

                    if (item.ListDesgloseEfectivoVueltoEntregado != null)
                    {
                        if (item.ListDesgloseEfectivoVueltoEntregado.Count() > 0)
                        {
                            item.ListDesgloseEfectivoVueltoEntregado.Select(S =>
                            {
                                S.DETALLE_PAGO_RECIBIDO = dETALLE_PAGO_RECIBIDO.ID_DETALLE;
                                S.REGISTRADOPOR = model.Login;
                                return S;
                            }).ToList();
                        }
                    }

                    #endregion
                }

                #endregion

                #region Se inserta Desglose de pago recibido

                var ListDesgloseEfectivo = model.ListOperation.FirstOrDefault().PagosRecibidos.ListDetallePago.Where(x => x.ListDesgloseEfectivoRecibido != null).FirstOrDefault();
                if (ListDesgloseEfectivo != null)
                {
                    List<DESGLOSE_PAGO_RECIBIDO> dDESGLOSE_PAGO_RECIBIDO = new List<DESGLOSE_PAGO_RECIBIDO>();
                    foreach (var item in ListDesgloseEfectivo.ListDesgloseEfectivoRecibido)
                    {
                        var SerializeDESGLOSE_PAGO_RECIBIDO = JsonConvert.SerializeObject(item);
                        var DeserializeDESGLOSE_PAGO_RECIBIDO = JsonConvert.DeserializeObject<DESGLOSE_PAGO_RECIBIDO>(SerializeDESGLOSE_PAGO_RECIBIDO);
                        DeserializeDESGLOSE_PAGO_RECIBIDO.MODIFICADO = null;
                        DeserializeDESGLOSE_PAGO_RECIBIDO.REGISTRADO = DateTime.Now;
                        dDESGLOSE_PAGO_RECIBIDO.Add(DeserializeDESGLOSE_PAGO_RECIBIDO);
                    }
                    db.DESGLOSE_PAGO_RECIBIDO.AddRange(dDESGLOSE_PAGO_RECIBIDO);
                    db.SaveChanges();
                }

                #endregion

                #region Se inserta Desglose de vuelto entregado

                var ListDesgloseEfectivoDevuelto = model.ListOperation.FirstOrDefault().PagosRecibidos.ListDetallePago.Where(x => x.ListDesgloseEfectivoVueltoEntregado != null).FirstOrDefault();
                if (ListDesgloseEfectivoDevuelto != null)
                {
                    List<DESGLOSE_VUELTO_ENTREGADO> dDESGLOSE_VUELTO_ENTREGADO = new List<DESGLOSE_VUELTO_ENTREGADO>();
                    foreach (var item in ListDesgloseEfectivoDevuelto.ListDesgloseEfectivoVueltoEntregado)
                    {
                        var SerializeDESGLOSE_VUELTO_ENTREGADO = JsonConvert.SerializeObject(item);
                        var DeserializeDESGLOSE_VUELTO_ENTREGADO = JsonConvert.DeserializeObject<DESGLOSE_VUELTO_ENTREGADO>(SerializeDESGLOSE_VUELTO_ENTREGADO);
                        DeserializeDESGLOSE_VUELTO_ENTREGADO.MODIFICADO = null;
                        DeserializeDESGLOSE_VUELTO_ENTREGADO.REGISTRADO = DateTime.Now;
                        dDESGLOSE_VUELTO_ENTREGADO.Add(DeserializeDESGLOSE_VUELTO_ENTREGADO);
                    }
                    db.DESGLOSE_VUELTO_ENTREGADO.AddRange(dDESGLOSE_VUELTO_ENTREGADO);
                    db.SaveChanges();
                }

                #endregion

                #region Se crea PAGO

                DataAccess.Conection.PAGOS modelPago = new DataAccess.Conection.PAGOS
                {
                    OrderId = Order.ID_ORDEN,
                    CiuOrig = model.ListOperation.FirstOrDefault().CiuOrig,
                    NroRecibo = _operationNumber.NUMERO,
                    Secuencia = 0,
                    Referencia = model.ListOperation.FirstOrDefault().PagoRealizado.Referencia,
                    Pais = "VZL",
                    Usd = model.ListOperation.FirstOrDefault().PagoRealizado.Usd,
                    Tasa = model.ListOperation.FirstOrDefault().PagoRealizado.Tasa,
                    Bolivares = model.ListOperation.FirstOrDefault().PagoRealizado.Bolivares,
                    Observaciones = model.ListOperation.FirstOrDefault().PagoRealizado.Observaciones,
                    FechaRegistro = Order.FECHA_OPERACION,
                    Usuario = model.Login.ToUpper(),
                    Status = 0
                };

                db.PAGOS.Add(modelPago);
                db.SaveChanges();

                #endregion

                #region Se inserta detalle de pago realizado

                List<DETALLE_PAGO_REALIZADO> dDETALLE_PAGO_REALIZADO = new List<DETALLE_PAGO_REALIZADO>();

                foreach (var item in model.ListOperation.FirstOrDefault().DetallePagoRealizado.ToList())
                {
                    dDETALLE_PAGO_REALIZADO.Add(new DETALLE_PAGO_REALIZADO()
                    {
                        PAGO_REALIZADO = Order.ID_ORDEN,
                        TIPO_PAGO_REALIZADO = item.TIPO_PAGO_REALIZADO,
                        DETALLE_TIPO_PAGO = item.DETALLE_TIPO_PAGO,
                        REGISTRADOPOR = model.Login,
                        MONTO = item.MONTO,
                        REGISTRADO = DateTime.Now,
                        FECHA_TRANSACCION = DateTime.Now,
                        BankAccountsId = item.BankAccountsId,
                        REFERENCIA_1 = model.ListCashierMethodPay.FirstOrDefault().Referencia
                    });
                }

                db.DETALLE_PAGO_REALIZADO.AddRange(dDETALLE_PAGO_REALIZADO);
                db.SaveChanges();

                #endregion

                #region SETEAR DESGLOSE DE EFECTIVO DE PAGO REALIZADO

                var DetalleEfectivoPagoRealizado = dDETALLE_PAGO_REALIZADO.Where(x => x.TIPO_PAGO_REALIZADO == Constant.MetodoPago.Efectivo).ToList();

                List<DESGLOSE_PAGO_REALIZADO> ListDesgloseEfectivoPagoRealizado = new List<DESGLOSE_PAGO_REALIZADO>();

                var ListDesgloseMonedaOp = (from x in (cashiers.Where(x => x.OriginOperation == Constant.OperacionesCaja.Salida && x.Currency == model.ListOperation.FirstOrDefault().Moneda))
                                            group x by new
                                            {
                                                x.DenominationId,
                                                x.BranchOffice,
                                                x.UserID,
                                                x.OriginOperation,
                                                x.CreationUser
                                            } into grupo
                                            select new DESGLOSE_PAGO_REALIZADO
                                            {
                                                DENOMINACION = grupo.Key.DenominationId,
                                                REGISTRADOPOR = grupo.Key.CreationUser,
                                                CANTIDAD_ENTREGADA = grupo.Sum(x => x.Quantity),
                                                SUBTOTAL_ENTREGADO = grupo.Sum(x => x.Amount),
                                                REGISTRADO = DateTime.Now
                                            }).ToList();

                if (ListDesgloseMonedaOp.Count > 0)
                {
                    if (DetalleEfectivoPagoRealizado.Where(x => x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Dolares).Count() > 0)
                    {
                        ListDesgloseMonedaOp.Select(S =>
                        {
                            S.DETALLE_PAGO_REALIZADO = DetalleEfectivoPagoRealizado.Where(x => x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Dolares).FirstOrDefault().ID_DETALLE;
                            return S;
                        }).ToList();

                        ListDesgloseEfectivoPagoRealizado.AddRange(ListDesgloseMonedaOp);
                    }
                }

                var ListDesgloseMonedaBol = (from x in (cashiers.Where(x => x.OriginOperation == Constant.OperacionesCaja.Salida && x.Currency == model.ListOperation.FirstOrDefault().MonedaCon))
                                             group x by new
                                             {
                                                 x.DenominationId,
                                                 x.BranchOffice,
                                                 x.UserID,
                                                 x.OriginOperation,
                                                 x.CreationUser
                                             } into grupo
                                             select new DESGLOSE_PAGO_REALIZADO
                                             {
                                                 DENOMINACION = grupo.Key.DenominationId,
                                                 REGISTRADOPOR = grupo.Key.CreationUser,
                                                 CANTIDAD_ENTREGADA = grupo.Sum(x => x.Quantity),
                                                 SUBTOTAL_ENTREGADO = grupo.Sum(x => x.Amount),
                                                 REGISTRADO = DateTime.Now
                                             }).ToList();

                if (ListDesgloseMonedaBol.Count > 0)
                {
                    if (DetalleEfectivoPagoRealizado.Where(x => x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Bolivares).Count() > 0)
                    {
                        ListDesgloseMonedaBol.Select(S =>
                        {
                            S.DETALLE_PAGO_REALIZADO = DetalleEfectivoPagoRealizado.Where(x => x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Bolivares).FirstOrDefault().ID_DETALLE;
                            return S;
                        }).ToList();
                        
                        ListDesgloseEfectivoPagoRealizado.AddRange(ListDesgloseMonedaBol);
                    }
                }

                db.DESGLOSE_PAGO_REALIZADO.AddRange(ListDesgloseEfectivoPagoRealizado);
                db.SaveChanges();

                #endregion

                #region SETEAR DESGLOSE DE EFECTIVO DE VUELTO DE PAGO REALIZADO

                var RowId = model.ListOperation.FirstOrDefault().Id_OPERACION;

                var ListCashierinClient = from c in db.Cashier
                                          where c.RowId == RowId
                                          select c;

                List<DESGLOSE_VUELTO_RECIBIDO_PAGO> DesgloseEfectivoVueltoRecibido = new List<DESGLOSE_VUELTO_RECIBIDO_PAGO>();

                if (ListCashierinClient.Count() > 0)
                {
                    var DesgloseEfectivoVuelto = (from x in (ListCashierinClient.Where(x => x.OriginOperation == Constant.OperacionesCaja.Entrada && x.TypeOperaction == Constant.OperacionesCaja.IngresoEfecClient).ToList())
                                                  group x by new
                                                  {
                                                      x.Currency,
                                                      x.DenominationId,
                                                      x.Amount,
                                                      x.Quantity,
                                                      x.BranchOffice,
                                                      x.UserId,
                                                      x.OriginOperation,
                                                      x.CreationUser
                                                  } into grupo
                                                  select new DESGLOSE_VUELTO_RECIBIDO_PAGO
                                                  {
                                                      DENOMINACION = grupo.Key.DenominationId,
                                                      CANTIDAD_RECIBIDA = grupo.Key.Quantity,
                                                      SUBTOTAL_RECIBIDO = grupo.Key.Amount,
                                                      REGISTRADO = DateTime.Now,
                                                      REGISTRADOPOR = model.Login,
                                                  }).ToList();

                    DesgloseEfectivoVueltoRecibido.AddRange(DesgloseEfectivoVuelto);
                }

                if (DesgloseEfectivoVueltoRecibido.Count() > 0)
                {
                    DesgloseEfectivoVueltoRecibido.Select(S =>
                    {
                        S.DETALLE_PAGO_REALIZADO = DetalleEfectivoPagoRealizado.Where(x => x.DETALLE_TIPO_PAGO == Constant.DetalleTipoPago.Dolares).FirstOrDefault().ID_DETALLE;
                        return S;
                    }).ToList();
                }

                db.DESGLOSE_VUELTO_RECIBIDO_PAGO.AddRange(DesgloseEfectivoVueltoRecibido);
                db.SaveChanges();

                #endregion

                result.Error = false;
                result.Valid = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.";
                return result;
            }
        }

        #endregion

        #endregion

        #region Operaciones Pendientes

        #region SearchOperationsTempPending
        public List<OperationsTempPending> SearchOperationsTempPending()
        {
            var StatusPendiente = Convert.ToByte(Constant.StatusOperacionesTemporales.PendienteOperation);

            var OperationTemp = (from p in db.OPERACIONES_POR_COBRAR
                                 join d in db.DETALLE_TIPO_OPERACION on p.Tipo_Operacion equals d.ID_DETALLE
                                 where p.Status == StatusPendiente
                                 select new OperationsTempPending
                                 {
                                     Id_OPERACION = p.Id_OPERACION,
                                     CICliente = (p.Tipo_Operacion == 10 ? p.CCDes : p.CIRem),
                                     Cliente = (p.Tipo_Operacion == 10 ? p.NOMDESTINATARIO : p.NOMREMITENTE),
                                     Pais = p.Pais,
                                     Tipo_Operacion = d.DETALLE_OPERACION,
                                     Monto = p.Dolares.ToString(),
                                     FechaRegistro = p.FechaRegistro,
                                     Status = p.Status
                                 }).ToList();

            return OperationTemp;
        }

        #endregion

        #region UpdateStatusOperationsTempEntity
        public GenericResponse UpdateStatusOperationsTempEntity(OperacionesPorCobrar model)
        {
            var _return = new GenericResponse();
            try
            {
                var OperationTemp = (from p in db.OPERACIONES_POR_COBRAR
                                     where p.Id_OPERACION == model.Id_OPERACION
                                     select p).FirstOrDefault();

                OperationTemp.Status = model.Status;
                OperationTemp.UpdateDate = DateTime.Now;
                db.Entry(OperationTemp).State = EntityState.Modified;
                db.SaveChanges();

                _return.Error = false;
            }
            catch (Exception ex)
            {
                _return.Error = true;
                _return.ErrorMessage = string.Concat("No se logro actualizar la operacion temporal: ",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
            }
            return _return;
        }

        #endregion

        #endregion

        #region InsertBusinessOperation
        public OperacionDeNegocio InsertBusinessOperation(OperacionDeNegocio modelOperation, OperacionesPorCobrar model, List<Tarifas> tarifas)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = modelOperation.BranchOffice,
                    usuario = model.Usuario,
                    tipo = Constant.TipoNumeracion.OPERACIONES_TEMPORALES,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    transaction.Rollback();
                    modelOperation.error = _operationNumber.Error;
                    modelOperation.clientErrorDetail = _operationNumber.ErrorMessage;
                    return modelOperation;
                }

                model.NroRecibo = Convert.ToInt32(_operationNumber.NUMERO);
                model.NumeroOpTemporal = Convert.ToInt64(_operationNumber.NUMERO);
                modelOperation.Numero = Convert.ToInt64(_operationNumber.NUMERO);

                #endregion

                #region Se registra operacion BCV

                var responseBCVInt = new XmlDocument();
                responseBCVInt.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                var referenciaBCV = responseBCVInt.SelectSingleNode("//result").InnerText;
                model.Cadivi = referenciaBCV;

                #endregion

                #region Se Inserta Operacion Temporal

                var SerializeOT = JsonConvert.SerializeObject(model);
                var DeserializeOT = JsonConvert.DeserializeObject<DataAccess.Conection.OPERACIONES_POR_COBRAR>(SerializeOT);

                db.OPERACIONES_POR_COBRAR.Add(DeserializeOT);
                db.SaveChanges();

                modelOperation.IdOperacion = DeserializeOT.Id_OPERACION;

                #endregion

                #region Tarifas Aplicadas

                List<TARIFAS_APLICADAS> _tarifas_Aplicadas = new List<TARIFAS_APLICADAS>();

                var Tarifa = tarifas.Where(x => x.moneda != null);
                foreach (var item in Tarifa.Where(x => x.moneda.Equals("USD")))
                {
                    decimal _comisionUs = 0;
                    if (item.valor < 1)
                        _comisionUs = Math.Round(item.valor * modelOperation.montoOrden, 2);
                    else
                        _comisionUs = Math.Round(item.valor, 2);

                    _tarifas_Aplicadas.Add(new TARIFAS_APLICADAS
                    {
                        TARIFA = item.idTarifa,
                        MONTO = _comisionUs,
                        REGISTRADOPOR = model.Usuario,
                        REGISTRADO = DateTime.Now,
                        TEMPORAL = Convert.ToInt32(modelOperation.Numero)
                    });
                }

                foreach (var item in Tarifa.Where(x => x.moneda.Equals("VEB")))
                {
                    decimal _comisionBs = 0;

                    if (item.valor < 1)
                        _comisionBs = Math.Round(item.valor * Math.Round((modelOperation.montoOrden * modelOperation.TasaCambio), 2), 2);
                    else
                        _comisionBs = Math.Round(item.valor, 2);

                    _tarifas_Aplicadas.Add(new TARIFAS_APLICADAS
                    {
                        TARIFA = item.idTarifa,
                        MONTO = _comisionBs,
                        REGISTRADOPOR = model.Usuario,
                        REGISTRADO = DateTime.Now,
                        TEMPORAL = Convert.ToInt32(modelOperation.Numero)
                    });
                }

                db.TARIFAS_APLICADAS.AddRange(_tarifas_Aplicadas);
                db.SaveChanges();

                #endregion

                transaction.Commit();
                modelOperation.error = false;
                modelOperation.clientErrorDetail = _operationNumber.ErrorMessage;
                return modelOperation;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                modelOperation.error = true;
                modelOperation.clientErrorDetail = string.Concat("No se logro insertar la operacion temporal: ",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return modelOperation;
            }
            
        }


        #endregion

        #region Buscar Errores Servicio Bancos

        public TEntity SearchErrorBank(object[] param)
        {
            return _DbContext.Database.SqlQuery<TEntity>("operaciones.SearchErrorBank @CodError", param).FirstOrDefault();
        }


        #endregion

        #region Entity
        #region ServiceBank

        public List<ServiceBank> SearchServiceBank(ServiceBankRequest request)
        {
            var response = db.SearchServiceBank(request.StatusId).ToList();
            var responseSerializer = JsonConvert.SerializeObject(response);
            var responseDeserializer = JsonConvert.DeserializeObject<List<ServiceBank>>(responseSerializer);
            return responseDeserializer;
        }

        #endregion

        #region ServicesBankType

        public List<ServicesBankType> SearchServicesBankType(ServicesBankTypeRequest request)
        {
            var response = db.SearchServicesBankType(request.ServicesBankId, request.StatusId, request.ServicesTypeId).ToList();
            var responseSerializer = JsonConvert.SerializeObject(response);
            var responseDeserializer = JsonConvert.DeserializeObject<List<ServicesBankType>>(responseSerializer);
            return responseDeserializer;
        }

        #endregion

        #region BatchBankOperationOnLine
        public GenericResponse InsertBatchBankOperationOnLine(BatchBankOperationOnline request)
        {
            try
            {
                var requestSerializer = JsonConvert.SerializeObject(request);
                var entityDeserializer = JsonConvert.DeserializeObject<Conection.BatchBankOperationOnline>(requestSerializer);
                entityDeserializer.BatchOnlineNumber = db.SearchNextNumberBatchOperationOnline().FirstOrDefault().Value;
                db.BatchBankOperationOnline.Add(entityDeserializer);
                db.SaveChanges();
                return new GenericResponse { 
                    ReturnId = entityDeserializer.BatchOnlineId
                };
            }
            catch (Exception ex)
            {

                return new GenericResponse
                {
                    Error =true,
                    ErrorMessage = ex.Message
                };
            }
            
        }

        public GenericResponse SearchBatchBankOperationOnline(BatchBankOperationOnlineRequest request)
        {
            try
            {
                var response =  db.SearchBatchBankOperationOnline(request.StatusId, request.ServiceBankId, request.BatchOnlineNumber);
                var responseSerializer = JsonConvert.SerializeObject(response, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver()});
                var entityDeserializer = JsonConvert.DeserializeObject<List<BatchBankOperationOnline>>(responseSerializer);
                return new GenericResponse
                {
                    Data = entityDeserializer
                };
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

        public GenericResponse UpdateBatchBankOperationOnline(BatchBankOperationOnline request)
        {
            try
            {
                var updateResponse = db.UpdateBatchBankOperationOnline(request.BatchOnlineId, request.StatusId, request.UpdateUser).FirstOrDefault();

                return new GenericResponse
                {
                    Error = !updateResponse.Valid ?? false,
                    ErrorMessage = updateResponse.Message
                };
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

        public GenericResponse UpdateAnnulmentBatchBankOperationOnline(BatchBankOperationOnline request)
        {
            try
            {
                var annulmentResponse = db.UpdateAnnulmentBatchBankOperationOnline(request.BatchOnlineId, request.UpdateUser).FirstOrDefault();

                return new GenericResponse
                {
                    Error = !annulmentResponse.Valid??false,
                    ErrorMessage = annulmentResponse.Message
                };
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
        #endregion

        #region BatchBankOperationDetailOnLine
        public GenericResponse ProcessBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request)
        {
            try
            {
                var responseDetail = db.BatchBankOperationDetailOnline.FirstOrDefault(x => x.BatchOnlineId == request.BatchOnlineId && x.OperationId == request.OperationId);
                if (responseDetail == null) 
                {
                    var requestSerializer = JsonConvert.SerializeObject(request);
                    var entityDeserializer = JsonConvert.DeserializeObject<Conection.BatchBankOperationDetailOnline>(requestSerializer);
                    db.BatchBankOperationDetailOnline.Add(entityDeserializer);
                }
                else
                {
                    responseDetail.UpdateDate = DateTime.Now;
                    responseDetail.UpdateUser = request.CreationUser;
                    responseDetail.BatchDetailReference = request.BatchDetailReference;
                    responseDetail.StatusId = request.StatusId;
                    responseDetail.BatchDetailOnlineObservation = request.BatchDetailOnlineObservation;
                    db.Entry(responseDetail).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return new GenericResponse();
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

        public GenericResponse SearchBatchBankOperationDetailOnline(BatchBankOperationDetailOnlineRequest request)
        {
            try
            {
                var response = db.SearchBatchBankOperationDetailOnline(request.StatusId, request.BatchOnlineId, request.OperationId, request.BankDestinationId);
                var responseSerializer = JsonConvert.SerializeObject(response);
                var entityDeserializer = JsonConvert.DeserializeObject<List<BatchBankOperationDetailOnline>>(responseSerializer);
                return new GenericResponse
                {
                    Data = entityDeserializer
                };
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

        public GenericResponse UpdateBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request)
        {
            try
            {
                db.UpdateBatchBankOperationDetailOnline(request.BatchDetailOnlineId, request.StatusId, request.UpdateUser);
                return new GenericResponse
                {
                    Message = "Operación procesada satisfactoriamente"
                };
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

        #endregion

        #region ValidateOperationPaymentOnline
        public GenericResponse ValidateOperationPaymentOnline(int operationId)
        {
            try
            {
                var response = new GenericResponse();
                var validateResponse = db.ValidatedOperationPaymentOnline(operationId).FirstOrDefault();
                var responseSerializer = JsonConvert.SerializeObject(validateResponse);
                response.Data = JsonConvert.DeserializeObject<ValidatePaymentOnline>(responseSerializer);
                return response;
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
        #endregion

        #region PaymentOnlineBankResponseEntity
        public GenericResponse InsertPaymentOnlineBankResponseEntity(PaymentOnlineBankResponseEntity request)
        {
            try
            {
                var requestSerializer = JsonConvert.SerializeObject(request);
                var entityDeserializer = JsonConvert.DeserializeObject<Conection.PaymentOnlineBankResponseEntity>(requestSerializer);
                db.PaymentOnlineBankResponseEntity.Add(entityDeserializer);
                db.SaveChanges();
                return new GenericResponse();
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
        #endregion
        #endregion

        #region UpdateStatusOrdenesEntity

        public GenericResponse UpdateStatusOrdenesEntity(Ordenes model, Pago modelPago, Detalle_Pago_Realizado modelDetallePago, ProccesOrder order)
        {
            var result = new GenericResponse();
            var transaction = db.Database.BeginTransaction();
            try
            {
                if (model.RemesaEntrante)
                {
                    if (model.STATUS_ORDEN == 8)
                    {
                        result = ProcesarRemesaEntranteEntity(modelPago, modelDetallePago, model, order);

                        if (result.Error)
                        {
                            transaction.Rollback();
                            return result;
                        }

                        modelPago.OrderId = result.ReturnId;
                    }

                    result = UpdateStatusRemesaEntranteEntity(model, modelPago);
                    if (result.Error)
                    {
                        transaction.Rollback();
                        return result;
                    }

                    transaction.Commit();
                    result.ReturnId = result.ReturnId;
                    return result;
                }

                if (model.STATUS_ORDEN == 8)
                {
                    result = ProcesarRemesaEntity(model.ID_ORDEN, modelPago, modelDetallePago);
                    if (result.Error)
                    {
                        transaction.Rollback();
                        return result;
                    }
                }

                result = UpdateStatusOrdenesEntity(model);
                if (result.Error)
                {
                    transaction.Rollback();
                    return result;
                }

                transaction.Commit();
                result.Error = false;
                result.Valid = true;
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion el estatus de la orden",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }
        }

        #endregion

        #region ProcesarRemesaEntity

        private GenericResponse ProcesarRemesaEntity(int id, Pago modelPago, Detalle_Pago_Realizado modelDetallePago)
        {
            var result = new GenericResponse();

            try
            {
                var modelOrden = db.SearchOrdenesByFilter(null, null, null, null, null, null, null, null, null, id, null).ToList();

                if (modelOrden == null)
                {
                    result.Error = true;
                    result.ErrorMessage = "No se logro obtener la información de la orden para su pago. Por favor intente nuevamente y si el error continua comuniquese con el administrador del sistema.";
                    return result;
                }

                PAGOS pAGOS = new PAGOS()
                {
                    Referencia = modelPago.Referencia,
                    Pais = modelPago.Pais,
                    Observaciones = modelPago.Observaciones,
                    FechaRegistro = modelPago.FechaRegistro,
                    Usuario = modelPago.Usuario,
                    OrderId = modelOrden.FirstOrDefault().ID_ORDEN,
                    CiuOrig = modelOrden.FirstOrDefault().LetraSucursal,
                    NroRecibo = modelOrden.FirstOrDefault().NUMERO,
                    Tasa = Convert.ToDecimal(modelOrden.FirstOrDefault().TASA_DESTINO),
                    Status = 0,
                    Usd = modelOrden.FirstOrDefault().MONTO_CAMBIO ?? 0
                };

                db.PAGOS.Add(pAGOS);
                db.SaveChanges();

                DETALLE_PAGO_REALIZADO dETALLE_PAGO_REALIZADO = new DETALLE_PAGO_REALIZADO()
                {
                    PAGO_REALIZADO = modelOrden.FirstOrDefault().ID_ORDEN,
                    TIPO_PAGO_REALIZADO = modelDetallePago.TIPO_PAGO_REALIZADO,
                    DETALLE_TIPO_PAGO = modelDetallePago.DETALLE_TIPO_PAGO,
                    REGISTRADOPOR = modelDetallePago.REGISTRADOPOR,
                    REGISTRADO = modelDetallePago.REGISTRADO,
                    FECHA_TRANSACCION = modelDetallePago.FECHA_TRANSACCION,
                    BankAccountsId = modelDetallePago.BankAccountsId,
                    MONTO = modelOrden.FirstOrDefault().MONTO_CAMBIO ?? 0
                };

                db.DETALLE_PAGO_REALIZADO.Add(dETALLE_PAGO_REALIZADO);
                db.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la carga de los pagos efectuados ProcesarRemesaEntity",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }

        }

        #endregion

        #region UpdateStatusRemesaEntranteEntity

        private GenericResponse UpdateStatusRemesaEntranteEntity(Ordenes model, Pago modelPago)
        {
            var result = new GenericResponse();

            try
            {
                var statusOld = 0;

                var UpdateStatusRemesaEntrante = (from r in db.REMESAS_ENTRANTES
                                                  where r.ID_OPERACION == model.ID_ORDEN
                                                  select r);

                if (UpdateStatusRemesaEntrante.Count() > 0)
                {
                    var rRemesaEntrante = UpdateStatusRemesaEntrante.FirstOrDefault();

                    if (rRemesaEntrante.STATUS == 7 || rRemesaEntrante.STATUS == 8)
                    {
                        result.Error = true;
                        result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion del estatus de una remesa que no puede ser actualizada UpdateStatusRemesaEntranteEntity. Por favor notificar al administrador del sistema.");
                        return result;
                    }
                    else
                    {
                        statusOld = rRemesaEntrante.STATUS;

                        rRemesaEntrante.STATUS = Convert.ToByte(model.STATUS_ORDEN);
                        rRemesaEntrante.ModificadoPor = model.ModificadoPor;
                        rRemesaEntrante.Modificado = DateTime.Now;
                        rRemesaEntrante.FECHAPAGO = model.STATUS_ORDEN == 8 ? DateTime.Now : rRemesaEntrante.FECHAPAGO;
                        rRemesaEntrante.REFERENCIA_BCV = model.STATUS_ORDEN == 8 ? modelPago.Referencia : rRemesaEntrante.REFERENCIA_BCV;
                        rRemesaEntrante.ID_ORDEN = model.STATUS_ORDEN == 8 ? modelPago.OrderId : rRemesaEntrante.ID_ORDEN;
                        rRemesaEntrante.FechaAnulacion = model.STATUS_ORDEN == 7 ? DateTime.Now : rRemesaEntrante.FechaAnulacion;
                        rRemesaEntrante.UsuarioAnula = model.STATUS_ORDEN == 7 ? model.ModificadoPor : rRemesaEntrante.UsuarioAnula;
                        rRemesaEntrante.MotivoAnulacionId = model.STATUS_ORDEN == 7 ? model.MotivoAnulacionId : rRemesaEntrante.MotivoAnulacionId;
                        rRemesaEntrante.ObservacionAnulacion = model.STATUS_ORDEN == 7 || model.STATUS_ORDEN == 16 ? model.ObservacionesRechazo : rRemesaEntrante.ObservacionAnulacion;
                        rRemesaEntrante.OBSERVACIONES = model.STATUS_ORDEN != 7 || model.STATUS_ORDEN != 16 ? model.ObservacionesRechazo : rRemesaEntrante.OBSERVACIONES;

                        db.Entry(rRemesaEntrante).State = EntityState.Modified;
                        db.SaveChanges();

                        int? tipoHistoria = null;

                        if (model.STATUS_ORDEN == 18 || model.STATUS_ORDEN == 19 || model.STATUS_ORDEN == 20 || model.STATUS_ORDEN == 21)
                        {
                            tipoHistoria = 16;
                        }
                        else
                        {
                            tipoHistoria = (from th in db.TIPOS_HISTORIA
                                            where th.ID_STATUS_ORDEN == model.STATUS_ORDEN
                                            select th.ID_TIPO).FirstOrDefault();
                        }

                        if (tipoHistoria != null)
                        {
                            var HISTORIA = "Cambio de Estatus " + @statusOld + " a " + model.STATUS_ORDEN;

                            HISTORIAL_REMESAS_ENTRANTES hISTORIAL_REMESAS_ENTRANTES = new HISTORIAL_REMESAS_ENTRANTES()
                            {
                                ID_OPERACION = UpdateStatusRemesaEntrante.FirstOrDefault().ID_OPERACION,
                                TIPO_HISTORIA = Convert.ToInt32(tipoHistoria),
                                FECHA = DateTime.Now,
                                HISTORIA = HISTORIA,
                                UserCreatiId = model.ModificadoPor,
                                StatusId = model.STATUS_ORDEN,
                                SucursalProcesaId = model.SucursalProcesaId,
                            };

                            db.HISTORIAL_REMESAS_ENTRANTES.Add(hISTORIAL_REMESAS_ENTRANTES);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    result.Error = true;
                    result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion del estatus de la remesa UpdateStatusRemesaEntranteEntity. Por favor notificar al administrador del sistema.");
                    return result;
                }

                result.Error = false;
                result.ReturnId = model.STATUS_ORDEN == 7 ? model.ID_ORDEN : Convert.ToInt32(modelPago.OrderId);
                result.Valid = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion del estatus de la remesa UpdateStatusRemesaEntranteEntity",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }
        }

        #endregion

        #region UpdateStatusOrdenesEntity

        private GenericResponse UpdateStatusOrdenesEntity(Ordenes model)
        {
            var result = new GenericResponse();

            try
            {
                var statusOld = 0;

                var UpdateStatusOrdenes = (from r in db.ORDENES
                                           where r.ID_ORDEN == model.ID_ORDEN
                                           select r);

                if (UpdateStatusOrdenes.Count() > 0)
                {
                    var rOrdenes = UpdateStatusOrdenes.FirstOrDefault();

                    if (rOrdenes.STATUS_ORDEN == 7 || rOrdenes.STATUS_ORDEN == 8 || rOrdenes.STATUS_ORDEN == 3)
                    {
                        result.Error = true;
                        result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion del estatus de una orden que no puede ser actualizada UpdateStatusOrdenesEntity. Por favor notificar al administrador del sistema.");
                        return result;
                    }
                    else
                    {
                        statusOld = rOrdenes.STATUS_ORDEN;

                        rOrdenes.STATUS_ORDEN = model.STATUS_ORDEN;
                        rOrdenes.ModificadoPor = model.ModificadoPor;
                        rOrdenes.Modificado = DateTime.Now;
                        rOrdenes.REFERENCIA_PAGO = model.REFERENCIA_PAGO == null ? rOrdenes.REFERENCIA_PAGO : model.REFERENCIA_PAGO;
                        rOrdenes.FECHA_PAGO = model.STATUS_ORDEN == 8 ? DateTime.Now : rOrdenes.FECHA_PAGO;
                        rOrdenes.ObservacionesRechazo = model.STATUS_ORDEN == 4 || model.STATUS_ORDEN == 14 ? model.ObservacionesRechazo : rOrdenes.ObservacionesRechazo;
                        rOrdenes.FechaRechazo = model.STATUS_ORDEN == 4 || model.STATUS_ORDEN == 14 ? DateTime.Now : rOrdenes.FechaRechazo;
                        rOrdenes.SucursalProcesaId = model.STATUS_ORDEN == 4 || model.STATUS_ORDEN == 8 || model.STATUS_ORDEN == 14 || model.STATUS_ORDEN == 15 ? model.SucursalProcesaId : rOrdenes.SucursalProcesaId;

                        db.Entry(rOrdenes).State = EntityState.Modified;
                        db.SaveChanges();

                        int? tipoHistoria = null;

                        if (model.STATUS_ORDEN == 14 || model.STATUS_ORDEN == 15)
                        {
                            tipoHistoria = 18;
                        }
                        else
                        {
                            tipoHistoria = (from th in db.TIPOS_HISTORIA
                                            where th.ID_STATUS_ORDEN == model.STATUS_ORDEN
                                            select th.ID_TIPO).FirstOrDefault();
                        }

                        if (tipoHistoria != null)
                        {
                            var HISTORIA = "Cambio de Estatus " + @statusOld + " a " + model.STATUS_ORDEN;

                            HISTORIAL_ORDENES hISTORIAL_ORDENES = new HISTORIAL_ORDENES()
                            {
                                ORDEN = model.ID_ORDEN,
                                TIPO_HISTORIA = Convert.ToInt32(tipoHistoria),
                                FECHA = DateTime.Now,
                                HISTORIA = HISTORIA,
                                UserCreatiId = model.ModificadoPor,
                                StatusId = model.STATUS_ORDEN,
                                SucursalProcesaId = model.SucursalProcesaId,
                            };

                            db.HISTORIAL_ORDENES.Add(hISTORIAL_ORDENES);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    result.Error = true;
                    result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion del estatus de la orden UpdateStatusOrdenesEntity. Por favor notificar al administrador del sistema.");
                    return result;
                }

                result.Error = false;
                result.Valid = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error en la actualizacion del estatus de la orden UpdateStatusOrdenesEntity",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }
        }

        #endregion

        #region ProcesarRemesaEntranteEntity

        private GenericResponse ProcesarRemesaEntranteEntity(Pago modelPago, Detalle_Pago_Realizado modelDetallePago, Ordenes model, ProccesOrder order)
        {
            var result = new GenericResponse();
            try
            {
                result = InsertPaymentRemesaEntity(order, model);

                if (result.Error)
                {
                    return result;
                }

                #region Tarifas Aplicadas

                if (order.Orden.tasaCambio == 1)
                {
                    order.Tarifas_Comiciones = order.Tarifas_Comiciones.Where(x => x.idTarifa != 39).ToList();
                }

                List<TARIFAS_APLICADAS> _tarifas_Aplicadas = new List<TARIFAS_APLICADAS>();

                //Tarifas aplicadas en USD
                foreach (var item in order.Tarifas_Comiciones.Where(x => x.moneda.Equals("USD")).ToList())
                {
                    decimal _comisionUs = 0;
                    if (item.valor < 1)
                        _comisionUs = Math.Round(item.valor * order.Orden.montoOrden, 2);
                    else
                        _comisionUs = Math.Round(item.valor, 2);

                    _tarifas_Aplicadas.Add(new TARIFAS_APLICADAS
                    {
                        TARIFA = item.idTarifa,
                        MONTO = _comisionUs,
                        REGISTRADOPOR = order.Orden.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        ORDEN = result.ID_OPERACION
                    });
                }

                //Tarifas aplicadas en Bolivares
                foreach (var item in order.Tarifas_Comiciones.Where(x => x.moneda.Equals("VEB")).ToList())
                {
                    decimal _comisionBs = 0;
                    var objTasa = order.Historial.OrderByDescending(x => x.fechaRegistro).FirstOrDefault();
                    if (item.valor < 1)
                        //Se utiliza valorCompra del bcv porque la tasa venta tiene error en el WS el mismo valor suministrado por
                        //el WS en la tasa de compra pertenece a la tasa de venta BCV
                        _comisionBs = Math.Round(item.valor * Math.Round((order.Orden.montoOrden * objTasa.valorCompra), 2), 2);
                    else
                        _comisionBs = Math.Round(item.valor, 2);

                    _tarifas_Aplicadas.Add(new TARIFAS_APLICADAS
                    {
                        TARIFA = item.idTarifa,
                        MONTO = _comisionBs,
                        REGISTRADOPOR = order.Orden.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        ORDEN = result.ID_OPERACION

                    });
                }

                db.TARIFAS_APLICADAS.AddRange(_tarifas_Aplicadas);
                db.SaveChanges();

                #endregion

                PAGOS pAGOS = new PAGOS()
                {
                    Referencia = model.REFERENCIA_PAGO,
                    Pais = modelPago.Pais,
                    Observaciones = modelPago.Observaciones,
                    FechaRegistro = modelPago.FechaRegistro,
                    Usuario = modelPago.Usuario,
                    OrderId = result.ID_OPERACION,
                    CiuOrig = model.LetraSucursal,
                    NroRecibo = result.ReturnId,
                    Tasa = Convert.ToDecimal(order.Orden.tasaCambio),
                    Status = 0,
                    Usd = order.Orden.montoOrden,
                    Bolivares = order.Orden.MontoBolivares
                };

                db.PAGOS.Add(pAGOS);
                db.SaveChanges();

                DETALLE_PAGO_REALIZADO dETALLE_PAGO_REALIZADO = new DETALLE_PAGO_REALIZADO()
                {
                    PAGO_REALIZADO = result.ID_OPERACION,
                    TIPO_PAGO_REALIZADO = modelDetallePago.TIPO_PAGO_REALIZADO,
                    DETALLE_TIPO_PAGO = modelDetallePago.DETALLE_TIPO_PAGO,
                    REGISTRADOPOR = modelDetallePago.REGISTRADOPOR,
                    REGISTRADO = modelDetallePago.REGISTRADO,
                    FECHA_TRANSACCION = modelDetallePago.FECHA_TRANSACCION,
                    BankAccountsId = modelDetallePago.BankAccountsId,
                    MONTO = order.Orden.MontoBolivares
                };

                db.DETALLE_PAGO_REALIZADO.Add(dETALLE_PAGO_REALIZADO);
                db.SaveChanges();

                result.ReturnId = result.ID_OPERACION;
                result.Error = false;
                result.Valid = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error al procesas pagos de la remesa ProcesarRemesaEntranteEntity",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }
        }

        #endregion

        #region InsertPaymentRemesaEntity

        public GenericResponse InsertPaymentRemesaEntity(ProccesOrder order, Ordenes model)
        {
            var result = new GenericResponse();
            try
            {
                #region Variables

                var idTipoOperacion = 10; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idMonedaOperacion = 213; //el codigo de la moneda interna
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idPaisDestino = "VE";
                var n = string.Empty;

                #endregion

                #region Se consulta proximo numero

                var NextNumber = new NextNumberRequest
                {
                    sucursal = model.SucursalProcesaName,
                    usuario = order.Orden.ModificadoPor,
                    tipo = Constant.TipoNumeracion.FacturaDeOperacion,
                    consulta = false,
                    fecha = DateTime.Now
                };

                INumeracionDal<NUMERACIONs> dalNumber = new NumeracionDal<NUMERACIONs>();
                var _operationNumber = dalNumber.NumberNext(NextNumber);
                if (_operationNumber.Error)
                {
                    return _operationNumber;
                }

                #endregion

                #region Mapeo de Nombres de Beneficiario

                NameBeneficiary NameBeneficiary = SplitName.NameSeparator(order.Orden.NombresRemitente);

                #endregion

                #region Orden

                Ordenes objOrden = new Ordenes
                {
                    DETALLE_TIPO_OPERACION = idTipoOperacion,
                    CLIENTE = order.Orden.idCliente,
                    SUCURSAL = order.Orden.SucursalProcesa,
                    STATUS_ORDEN = Constant.StatusOrden.OrdenPagada,
                    MONEDA = order.Orden.MonedaConversion,
                    OFICINA = order.Oficinas.FirstOrDefault().ID_OFIC_EXTERNA,
                    PAIS_DESTINO = idPaisDestino,
                    CORRESPONSAL = "CAL",
                    NUMERO = int.Parse(_operationNumber.NUMERO.ToString()),
                    TIPO_CAMBIO = order.Orden.tasaCambio,
                    TASA_DESTINO = order.Orden.tasaCambio,
                    MONTO = order.Orden.montoOrden,
                    MONTO_CAMBIO = order.Orden.MontoBolivares,
                    FECHA_VALOR_TASA = order.Orden.fechaValorTasa,
                    TIPO_OP_BCV = order.TipoMovimientoss.FirstOrDefault().ID_BCV,
                    NOMBRES_REMITENTE = NameBeneficiary.Names,
                    APELLIDOS_REMITENTE = NameBeneficiary.Surnames,
                    IDENTIFICACION_REMITENTE = order.Orden.DocumentoRemitente,
                    NOMBRES_BENEFICIARIO = order.Ficha.FirstOrDefault().PrimerNombre.Trim() + " " + (string.IsNullOrEmpty(order.Ficha.FirstOrDefault().SegundoNombre) ? string.Empty : order.Ficha.FirstOrDefault().SegundoNombre.Trim()),
                    APELLIDOS_BENEFICIARIO = order.Ficha.FirstOrDefault().PrimerApellido.Trim() + " " + (string.IsNullOrEmpty(order.Ficha.FirstOrDefault().SegundoApellido) ? string.Empty : order.Ficha.FirstOrDefault().SegundoApellido.Trim()),
                    IDENTIFICACION_BENEFICIARIO = order.Orden.tipoIdBeneficiario + order.Orden.numeroIdBeneficiario,
                    //BANCO_NACIONAL = orden.bancoCliente,
                    //NUMERO_CUENTA = orden.numeroCuentaCliente,
                    EMAIL_CLIENTE = "",
                    EMAIL_BENEFICIARIO = order.Orden.emailCliente,
                    OBSERVACIONES = order.Orden.observaciones,
                    BANCO_DESTINO = order.Orden.nombreBancoDestino,
                    NUMERO_CUENTA_DESTINO = order.Orden.numeroCuentaDestino.ToString(),
                    DIRECCION_BANCO = order.Orden.direccionBancoDestino,
                    ABA = order.Orden.aba,
                    SWIFT = order.Orden.swift,
                    IBAN = order.Orden.iban,
                    TELEFONO_CLIENTE = order.Orden.telefonoCliente,
                    REGISTRADOPOR = order.Orden.ModificadoPor,
                    AGENTE = idPagador,
                    MOTIVO_OP_BCV = order.Orden.idMotivoOferta,
                    USUARIO_TAQUILLA = order.Orden.ModificadoPor,
                    TasaConversion = order.Orden.tasaCambio,
                    MonedaOperacion = idMonedaOperacion,
                    MontoConversion = order.Orden.montoOrden,
                    //CommissionUsd = comisionUs,
                    BancoPagoTransferencia = order.Orden.bancoCliente,
                    NumeroCuentaPagoTransferencia = order.Orden.numeroCuentaCliente,
                    SucursalProcesaId = order.Orden.SucursalProcesa,
                    ModificadoPor = order.Orden.ModificadoPor,
                    Modificado = DateTime.Now,
                    FECHA_OPERACION = DateTime.Now,
                    FECHA_PAGO = DateTime.Now,
                    REFERENCIA_PAGO = model.REFERENCIA_PAGO,
                    //CommissionBs = comisionBs
                    AnulProcesada = false
                };

                #endregion

                #region Se registra operacion BCV

                var responseBCVInt = new XmlDocument();
                responseBCVInt.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                var referenciaBCV = responseBCVInt.SelectSingleNode("//result").InnerText;
                objOrden.REFERENCIA_ORDEN = referenciaBCV;

                #endregion

                #region Insert Orden

                var Serialize = JsonConvert.SerializeObject(objOrden);
                var Deserialize = JsonConvert.DeserializeObject<DataAccess.Conection.ORDENES>(Serialize);

                db.ORDENES.Add(Deserialize);
                db.SaveChanges();

                #endregion

                result.Error = false;
                result.ReturnId = int.Parse(_operationNumber.NUMERO.ToString());
                result.ID_OPERACION = Deserialize.ID_ORDEN;
                result.Message = objOrden.REFERENCIA_ORDEN;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = string.Concat("Se ha presentado un error al guardar la orden InsertPaymentRemesaEntity",
                           ex.Message, ". Por favor notificar al administrador del sistema.");
                return result;
            }
        }

        #endregion

        #region ValidateAccumulatedAmount

        public TEntity ValidateAccumulatedAmount(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>("operaciones.ValidateAccumulatedAmount @ClientId", param).First();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new AccumulatedAmount
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar consultar monto acumulado del cliente: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }

        }

        #endregion
    }
}

