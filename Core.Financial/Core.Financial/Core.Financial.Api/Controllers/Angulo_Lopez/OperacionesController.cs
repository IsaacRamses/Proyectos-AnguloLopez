
using Common.Filters;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.Operaciones.Services;
using Common.Models.Angulo_Lopez.Operaciones.Services.Bancaribe;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Common.Models.Angulo_Lopez.Pagos;
using Common.Models.Common;
using Common.Resource;
using Core.Financial.Api.Utils;
using IObjectBuilder.Angulo_Lopez;
using IObjectBuilder.Angulo_Lopez.Contabilidad;
using IObjectBuilder.Angulo_Lopez.Pagos;
using IObjectBuilder.Clients;
using IObjectBuilder.Operaciones;
using ObjectBuilder.Angulo_Lopez;
using ObjectBuilder.Angulo_Lopez.Contabilidad;
using ObjectBuilder.Angulo_Lopez.Operaciones;
using ObjectBuilder.Angulo_Lopez.Pagos;
using ObjectBuilder.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using OrdenCompraEfectivo = Core.Financial.Api.Utils.OrdenCompraEfectivo;
using Common.Models.Angulo_Lopez.Numeracion;
using IObjectBuilder.Angulo_Lopez.Numeracion;
using ObjectBuilder.Angulo_Lopez.Numeracion;
using System.Web.Http.Results;

namespace Core.Financial.Api.Controllers.Angulo_Lopez
{
    [AuthorizeApiMe]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OperacionesController : ApiController
    {
        #region Variables
        private readonly IOperacionesBuilder builder = new OperacionesBuilder();
        private readonly IPagosBuilder builderPagos = new PagosBuilder();
        private readonly IAnguloLopezBuilder builderAnguloLopez = new AnguloLopezBuilder();
        public static utilidades.UTILIDADES_CCAL_WS utilitarios;
        private readonly IClientsBuilder builderclients = new ClientsBuilder();
        private readonly IContabilidadBuilder builderContabilidad = new ContabilidadBuilder();
        private readonly INumeracionBuilder builderNumeracion = new NumeracionBuilder();

        #endregion

        #region Ordenes
        [HttpPost]
        [Route("api/Operaciones/SearchOrderPayment")]
        public IHttpActionResult SearchOrderPayment(OrderPaymentRequest model)
        {
            var data = builder.SearchOrderPayment(model);
            return Ok(data);
        }

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusOrdenes")]
        public IHttpActionResult UpdateStatusOrdenes(Ordenes model)
        {
            var data = builder.UpdateStatusOrdenesEntity(model);
            return Ok(data);

        }

        private GenericResponse UpdateStatusRemesaEntrante(int status, string userlogin, int id_operacion, int sucursal, int? orderId, string observacionesAnulacion, string referencia, int? motivoAnulacionId)
        {
            IAnguloLopezBuilder anguloLopezBuilder = new AnguloLopezBuilder();
            var result = anguloLopezBuilder.UpdateStatusRemesasEntrantes(new REMESAS_ENTRANTES
            {
                STATUS = status,
                ModificadoPor = userlogin,
                ID_OPERACION = id_operacion,
                SucursalProcesaId = sucursal,
                ID_ORDEN = status == 8 ? orderId : null,
                ObservacionAnulacion = status == 4 || status == 7 ? observacionesAnulacion : null,
                REFERENCIA_BCV = status == 8 ? referencia : null,
                MotivoAnulacionId = status == 7 ? motivoAnulacionId : null
            });

            if (result.Error && status == 8)
            {
                builder.RollbackRemesaPagadas(orderId ?? 0, true);
            }
            result.ID_OPERACION = id_operacion;
            result.ReturnId = Convert.ToInt32(orderId);
            return result;
        }
        private GenericResponse ProcesarRemesaEntrante(int id, Pago modelPago, Detalle_Pago_Realizado modelDetallePago, int sucursalProcesa)
        {
            var result = new GenericResponse();
            var modelEntrante = builder.SearchRemesaEntranteFromOrdenes(id);
            if (modelEntrante == null)
            {
                result.Error = true;
                result.ErrorMessage = "No se logro obtener la información de la remesa entrante para su pago. Por favor intente nuevamente y si el error continua comuniquese con el administrador del sistema.";
                return result;
            }

            //1. Registro en Ordenes
            /*Faltan las tarifas y el detalle del pago*/
            OrdenCompraEfectivo ordenCompraEfectivo = new Utils.OrdenCompraEfectivo(modelEntrante);
            ordenCompraEfectivo.current = String.IsNullOrEmpty(modelPago.Usuario) ? "AUTOMATICO" : modelPago.Usuario;
            ordenCompraEfectivo.SucursalProcesa = sucursalProcesa;
            ordenCompraEfectivo.ModificadoPor = modelPago.Usuario;
            ordenCompraEfectivo.ReferenciaPago = modelPago.Referencia;
            var order = Utils.Functions.InsertPaymentRemesa(ordenCompraEfectivo);
            if (order.error)
            {
                result.Error = true;
                result.ErrorMessage = order.clientErrorDetail;
                return result;
            }
            result.ReturnId = int.Parse(ordenCompraEfectivo.id.ToString());
            modelPago.Usd = modelEntrante.montoOrden;
            modelPago.Bolivares = modelEntrante.MontoBolivares;
            modelDetallePago.MONTO = modelEntrante.MontoBolivares;
            var pago = InsertRemesaPagadaPagos(modelPago, int.Parse(ordenCompraEfectivo.id.ToString()),
                ordenCompraEfectivo.CiuOrig, int.Parse(ordenCompraEfectivo.OrderNumber), order.tasaCambio);
            if (pago.Error)
            {
                builder.RollbackRemesaPagadas(modelPago.OrderId ?? 0, true);
                return pago;
            }
            modelDetallePago.PAGO_REALIZADO = modelPago.OrderId ?? 0;
            var detallePago = InsertDetallePagoRealizado(modelDetallePago);
            if (detallePago.Error)
            {
                builder.RollbackRemesaPagadas(modelPago.OrderId ?? 0, true);
                return detallePago;
            }
            return result;
        }

        private GenericResponse ProcesarRemesa(int id, Pago modelPago, Detalle_Pago_Realizado modelDetallePago)
        {
            var result = new GenericResponse();
            var modelOrden = builder.SearchOrdenesByFilter(new OrderHistoryRequest { ID_ORDEN = id }).FirstOrDefault();
            if (modelOrden == null)
            {
                result.Error = true;
                result.ErrorMessage = "No se logro obtener la información de la orden para su pago. Por favor intente nuevamente y si el error continua comuniquese con el administrador del sistema.";
                return result;
            }
            modelPago.Usd = modelOrden.MONTO_CAMBIO ?? 0;
            modelDetallePago.MONTO = modelPago.Usd;
            var pago = InsertRemesaPagadaPagos(modelPago, modelOrden.ID_ORDEN,
                modelOrden.LetraSucursal, modelOrden.NUMERO, modelOrden.TASA_DESTINO ?? 0);
            if (pago.Error)
            {
                builder.RollbackRemesaPagadas(modelPago.OrderId ?? 0, false);
                return pago;
            }

            modelDetallePago.PAGO_REALIZADO = modelOrden.ID_ORDEN;
            var detallePago = InsertDetallePagoRealizado(modelDetallePago);
            if (detallePago.Error)
            {
                builder.RollbackRemesaPagadas(modelPago.OrderId ?? 0, false);
                return detallePago;
            }
            return result;
        }

        private GenericResponse InsertRemesaPagadaPagos(Pago pago, int orderId, string ciuOrig, int nroRecibo, decimal tasa)
        {
            pago.OrderId = orderId;
            pago.CiuOrig = ciuOrig;
            pago.NroRecibo = nroRecibo;
            pago.Tasa = tasa;
            //pago.Bolivares = (pago.Usd * tasa);
            IPagosBuilder pagos = new PagosBuilder();
            var resultpago = pagos.InsertRemesaPagadaPagos(pago);
            return resultpago;
        }
        private GenericResponse InsertDetallePagoRealizado(Detalle_Pago_Realizado detalle)
        {
            IPagosBuilder pagos = new PagosBuilder();
            detalle.FECHA_OPERACION = DateTime.Now;
            var resultDetallepago = pagos.InsertDetallePagoRealizado(detalle);
            return resultDetallepago;
        }

        [HttpPost]
        [Route("api/Operaciones/SearchOrdenesByFilter")]
        public IHttpActionResult SearchOrdenesByFilter(OrderHistoryRequest model)
        {
            try
            {
                var data = builder.SearchOrdenesByFilter(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchReturnFundsOrder")]
        public IHttpActionResult SearchReturnFundsOrder(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchReturnFundsOrder(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchORDENES")]
        public IHttpActionResult SearchORDENES(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchORDENES(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusOrder")]
        public IHttpActionResult UpdateStatusOrder(Ordenes model)
        {
            try
            {
                var data = builder.UpdateStatusOrder(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchTurnAlert")]
        public IHttpActionResult SearchTurnAlert(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchTurnAlert(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchPaymentOrdersNotCanceled")]
        public IHttpActionResult SearchPaymentOrdersNotCanceled(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchPaymentOrdersNotCanceled(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchPaymentOrdersDivisaTaquilla")]
        public IHttpActionResult SearchPaymentOrdersDivisaTaquilla(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchPaymentOrdersDivisaTaquilla(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchPaymentOrderVentanilla")]
        public IHttpActionResult SearchPaymentOrderVentanilla(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchPaymentOrderVentanilla(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchPaymentOrdersTrasnfer")]
        public IHttpActionResult SearchPaymentOrdersTrasnfer(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchPaymentOrdersTrasnfer(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchOrderPendingCorrespondet")]
        public IHttpActionResult SearchOrderPendingCorrespondet(OrderHistoryRequest model)
        {
            try
            {
                var data = builder.SearchOrderPendingCorrespondet(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/SearchPaidOrders")]
        public IHttpActionResult SearchPaidOrders(PaidOrdersRequest model)
        {
            try
            {
                var data = builder.SearchPaidOrders(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/InsertOrdenes")]
        public IHttpActionResult InsertOrdenes(Ordenes model)
        {
            try
            {
                var data = builder.InsertOrdenes(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region SearchOrdenesPayableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/SearchOrdenesPayableCorrespondent")]
        public IHttpActionResult SearchOrdenesPayableCorrespondent(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchOrdenesPayableCorrespondent(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #region ValidateOperationClientPause
        [HttpPost]
        [Route("api/Operaciones/ValidateOperationClientPause")]
        public IHttpActionResult ValidateOperationClientPause(int clientId)
        {
            return Ok(builder.ValidateOperationClientPause(clientId));
        }
        #endregion

        #region SearchPaymentOrderInternational

        [HttpPost]
        [Route("api/Operaciones/SearchPaymentOrderInternational")]
        public IHttpActionResult SearchPaymentOrderInternational(OrdenesRequest model)
        {
            try
            {
                var data = builder.SearchPaymentOrderInternational(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #endregion

        [HttpPost]
        [Route("api/Operaciones/SearchOperations")]
        public IHttpActionResult SearchOperations(OperationsRequest model)
        {
            try
            {
                var data = builder.SearchOperations(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region BatchBankOperations
        [HttpPost]
        [Route("api/Operaciones/SearchBatchBankOperations")]
        public IHttpActionResult SearchBatchBankOperations(BatchBankOperationsRequest model)
        {
            try
            {
                var data = builder.SearchBatchBankOperations(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/InsertBatchBankOperations")]
        public IHttpActionResult InsertBatchBankOperations(BatchBankOperations model)
        {
            try
            {
                var data = builder.InsertBatchBankOperations(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/InsertBatchBankOperationIntegrated")]
        public IHttpActionResult InsertBatchBankOperationIntegrated(BatchBankOperations model)
        {
            try
            {
                return Ok(builder.InsertBatchBankOperationIntegrated(model));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusBatchBankOperations")]
        public IHttpActionResult UpdateStatusBatchBankOperations(BatchBankOperations model)
        {
            try
            {
                var data = builder.UpdateStatusBatchBankOperations(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/Operaciones/SearchBatchBankOperationsNumber")]
        public IHttpActionResult SearchBatchBankOperationsNumber()
        {
            try
            {
                return Ok(builder.SearchBatchBankOperationsNumber());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region BatchBankDetail
        [HttpPost]
        [Route("api/Operaciones/SearchBatchBankDetail")]
        public IHttpActionResult SearchBatchBankDetail(BatchBankDetailRequest model)
        {
            try
            {
                var data = builder.SearchBatchBankDetail(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/InsertBatchBankDetail")]
        public IHttpActionResult InsertBatchBankDetail(BatchBankDetail model)
        {
            try
            {
                var data = builder.InsertBatchBankDetail(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("api/Operaciones/UpdateStatusBatchBankDetail")]
        public IHttpActionResult UpdateStatusBatchBankDetail(BatchBankDetail model)
        {
            try
            {
                var data = builder.UpdateStatusBatchBankDetail(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Remesas Entrantes

        #region SearchRemesasEntrantesFormBatch

        [HttpPost]
        [Route("api/Operaciones/SearchRemesasEntrantesFormBatch")]
        public IHttpActionResult SearchRemesasEntrantesFormBatch(OrdenEntranteRequest model)
        {
            return Ok(builder.SearchRemesasEntrantesFormBatch(model));
        }

        [HttpPost]
        [Route("api/Operaciones/InsertarRemesaEntranteOperation")]
        public IHttpActionResult InsertarRemesaEntranteOperation(REMESAS_ENTRANTES param)
        {
            var user = Core.Financial.Api.Utils.Functions.getUsuario(param.REGISTRADO_POR);
            var num = Core.Financial.Api.Utils.Services.utilitarios.GetProximoNumero(24, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");

            if (num.FirstChild != num.SelectSingleNode("ERROR"))
            {
                if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                    param.SECUENCIA = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                else
                    param.SECUENCIA = 0;
            }

            var ret = builderAnguloLopez.InsertarRemesaEntrante(param);
            return Ok(ret);
        }

        #endregion

        #region SearchRemesasEntrantesFormBatchPorConfirmar

        [HttpPost]
        [Route("api/Operaciones/SearchRemesasEntrantesFormBatchPorConfirmar")]
        public IHttpActionResult SearchRemesasEntrantesFormBatchPorConfirmar(OrdenEntranteRequest model)
        {
            return Ok(builder.SearchRemesasEntrantesFormBatchPorConfirmar(model));
        }

        #endregion

        #region UpdateStatusRemesasEntrantesIncidencias

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusRemesasEntrantesIncidencias")]
        public IHttpActionResult UpdateStatusRemesasEntrantesIncidencias(REMESAS_ENTRANTES param)
        {
            return Ok(builder.UpdateStatusRemesasEntrantesIncidencias(param));
        }

        #endregion

        #region UpdateRemesasEntrantesIncidenciaRIA

        [HttpPost]
        [Route("api/Operaciones/UpdateRemesasEntrantesIncidenciaRIA")]
        public IHttpActionResult UpdateRemesasEntrantesIncidenciaRIA(REMESAS_ENTRANTES param)
        {
            return Ok(builder.UpdateRemesasEntrantesIncidenciaRIA(param));
        }

        #endregion

        #region SearchRemesasEntrantesRejectRIA
        [HttpPost]
        [Route("api/Operaciones/SearchRemesasEntrantesRejectRIA")]
        public IHttpActionResult SearchRemesasEntrantesRejectRIA(OrdenEntranteRequest model)
        {
            return Ok(builder.SearchRemesasEntrantesRejectRIA(model));
        }

        #endregion

        #region spuRemesaEntrantePagadaPendientePorConfirmar

        [HttpPost]
        [Route("api/Operaciones/spuRemesaEntrantePagadaPendientePorConfirmar")]
        public IHttpActionResult spuRemesaEntrantePagadaPendientePorConfirmar(RemesaEntrantePagadaPendientePorConfirmar model)
        {
            try
            {
                var data = builder.spuRemesaEntrantePagadaPendientePorConfirmar(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #region InsertRemesaEntranteWithoutValidationProcess
        [HttpPost]
        [Route("api/Operaciones/InsertRemesaEntranteWithoutValidationProcess")]
        public IHttpActionResult InsertRemesaEntranteWithoutValidationProcess(REMESAS_ENTRANTES param)
        {
            return Ok(builder.InsertRemesaEntranteWithoutValidationProcess(param));
        }
        #endregion

        #endregion

        #region Pago de lote
        [HttpPost]
        [Route("api/Operaciones/UpdateStatusLotPayment")]
        public IHttpActionResult UpdateStatusLotPayment(BatchBankDetail model)
        {
            try
            {

                var data = builder.UpdateStatusLotPayment(model);
                if (data.Error && model.OrderId != null)
                    builder.RollbackRemesaPagadas(model.OrderId.Value, false);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Operaciones/MassPaymentLots")]
        public IHttpActionResult MassPaymentLots(ListBatchBankDetail model)
        {
            string orderIds = string.Empty;
            string detailIds = string.Empty;
            try
            {
                var request = new GenericResponse();
                foreach (var m in model.ListaBatchBankDetail)
                {
                    model.Order.STATUS_ORDEN = 8;
                    model.Order.REFERENCIA_PAGO = m.DetailPaymentReference;
                    model.Order.OBSERVACIONES = m.DetailObservacion;
                    model.Order.ModificadoPor = m.UpdateUser;
                    model.Order.RemesaEntrante = true;
                    model.Order.ID_ORDEN = m.OrderIncomingId.Value;
                    model.Order.Tipo_Pago = m.Tipo_Pago;
                    model.Order.BankDestinationId = m.BankDestinationId;
                    model.Order.BankAccountsId = m.BankAccountsId;
                 
                    var ordenes = builder.UpdateStatusOrdenesEntity(model.Order);
                    if (!ordenes.Error)
                    {
                        m.OrderId = ordenes.ReturnId;
                        var data = builder.UpdateStatusLotPayment(m);
                        //if (data.Error && m.OrderId != null)
                        //{
                        //    request = data;
                        //    break;
                        //}
                    }
 
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                builder.RollbackMassRemesaPagadas(orderIds, false, detailIds);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Funciones
        private GenericResponse InsertStatusOrdenes(Ordenes model)
        {
            var result = new GenericResponse();
            Pago modelPago = null;
            Detalle_Pago_Realizado modelDetallePago = null;
            if (model.RemesaEntrante)
            {
                if (model.STATUS_ORDEN == 8)
                {
                    /*Operaciónpor confirmar
                     * if es operacion por confirmar se realizar
                     */

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
                        TIPO_PAGO_REALIZADO = model.Tipo_Pago,
                        DETALLE_TIPO_PAGO = model.BankDestinationId,
                        REGISTRADOPOR = model.ModificadoPor,
                        REGISTRADO = DateTime.Now,
                        FECHA_TRANSACCION = DateTime.Now,
                        BankAccountsId = model.BankAccountsId
                    };
                    var resultPago = ProcesarRemesaEntrante(model.ID_ORDEN, modelPago, modelDetallePago, model.SucursalProcesaId ?? 0);
                    if (resultPago.Error)
                        return resultPago;
                    modelPago.OrderId = resultPago.ReturnId;
                }
                IAnguloLopezBuilder anguloLopezBuilder = new AnguloLopezBuilder();
                result = anguloLopezBuilder.UpdateStatusRemesasEntrantes(new REMESAS_ENTRANTES
                {
                    STATUS = model.STATUS_ORDEN,
                    ModificadoPor = model.ModificadoPor,
                    ID_OPERACION = model.ID_ORDEN,
                    SucursalProcesaId = model.SucursalProcesaId,
                    ID_ORDEN = model.STATUS_ORDEN == 8 ? modelPago.OrderId : null,
                    ObservacionAnulacion = model.STATUS_ORDEN == 4 ? model.ObservacionesRechazo : null

                });

                if (result.Error && model.STATUS_ORDEN == 8)
                {
                    builder.RollbackRemesaPagadas(modelPago.OrderId ?? 0, true);
                }
                result.ID_OPERACION = model.ID_ORDEN;
                return result;
            }

            if (model.STATUS_ORDEN == 8)
            {
                result = ProcesarRemesa(model.ID_ORDEN, modelPago, modelDetallePago);
                if (result.Error)
                    return result;
            }
            result = builder.UpdateStatusOrdenes(model);
            if (result.Error && model.STATUS_ORDEN == 8)
            {
                builder.RollbackRemesaPagadas(model.ID_ORDEN, false);
            }
            return result;
        }
        #endregion

        #region AccountReceivableCorrespondent

        #region SearchAccountReceivableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/SearchAccountReceivableCorrespondent")]
        public IHttpActionResult SearchAccountReceivableCorrespondent(AccountReceivableCorrespondentRequest model)
        {
            var data = builder.SearchAccountReceivableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region InsertAccountReceivableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/InsertAccountReceivableCorrespondent")]
        public IHttpActionResult InsertAccountReceivableCorrespondent(AccountReceivableCorrespondent model)
        {
            var data = builder.InsertAccountReceivableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountReceivableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountReceivableCorrespondent")]
        public IHttpActionResult UpdateAccountReceivableCorrespondent(AccountReceivableCorrespondent model)
        {
            var data = builder.UpdateAccountReceivableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentRemoveDetail

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountReceivableCorrespondentRemoveDetail")]
        public IHttpActionResult UpdateAccountReceivableCorrespondentRemoveDetail(AccountReceivableCorrespondent model)
        {
            var data = builder.UpdateAccountReceivableCorrespondentRemoveDetail(model);
            return Ok(data);
        }

        #endregion

        #region UpdateStatusAccountReceivableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusAccountReceivableCorrespondent")]
        public IHttpActionResult UpdateStatusAccountReceivableCorrespondent(AccountReceivableCorrespondent model)
        {
            var data = builder.UpdateStatusAccountReceivableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentPayment

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountReceivableCorrespondentPayment")]
        public IHttpActionResult UpdateAccountReceivableCorrespondentPayment(AccountReceivableCorrespondent model)
        {
            var data = builder.UpdateAccountReceivableCorrespondentPayment(model);
            return Ok(data);
        }

        #endregion

        #region SearchDraftsReceivable

        [HttpPost]
        [Route("api/Operaciones/SearchDraftsReceivable")]
        public IHttpActionResult SearchDraftsReceivable(OrdenEntranteRequest model)
        {
            var data = builder.SearchDraftsReceivable(model);
            return Ok(data);
        }

        #endregion
        #endregion

        #region AccountReceivableCorrespondentDetail

        #region SearchAccountReceivableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/SearchAccountReceivableCorrespondentDetail")]
        public IHttpActionResult SearchAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetailRequest model)
        {
            var data = builder.SearchAccountReceivableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #region InsertAccountReceivableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/InsertAccountReceivableCorrespondentDetail")]
        public IHttpActionResult InsertAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail model)
        {
            var data = builder.InsertAccountReceivableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountReceivableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountReceivableCorrespondentDetail")]
        public IHttpActionResult UpdateAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail model)
        {
            var data = builder.UpdateAccountReceivableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #region UpdateStatusAccountReceivableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusAccountReceivableCorrespondentDetail")]
        public IHttpActionResult UpdateStatusAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail model)
        {
            var data = builder.UpdateStatusAccountReceivableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #endregion

        #region AccountsPayableCorrespondent

        #region SearchAccountsPayableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/SearchAccountsPayableCorrespondent")]
        public IHttpActionResult SearchAccountsPayableCorrespondent(AccountsPayableCorrespondentRequest model)
        {
            var data = builder.SearchAccountsPayableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region InsertAccountsPayableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/InsertAccountsPayableCorrespondent")]
        public IHttpActionResult InsertAccountsPayableCorrespondent(AccountsPayableCorrespondent model)
        {
            var data = builder.InsertAccountsPayableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountsPayableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountsPayableCorrespondent")]
        public IHttpActionResult UpdateAccountsPayableCorrespondent(AccountsPayableCorrespondent model)
        {
            var data = builder.UpdateAccountsPayableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentRemoveDetail

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountsPayableCorrespondentRemoveDetail")]
        public IHttpActionResult UpdateAccountsPayableCorrespondentRemoveDetail(AccountsPayableCorrespondent model)
        {
            var data = builder.UpdateAccountsPayableCorrespondentRemoveDetail(model);
            return Ok(data);
        }

        #endregion

        #region UpdateStatusAccountsPayableCorrespondent

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusAccountsPayableCorrespondent")]
        public IHttpActionResult UpdateStatusAccountsPayableCorrespondent(AccountsPayableCorrespondent model)
        {
            var data = builder.UpdateStatusAccountsPayableCorrespondent(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentPayment

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountsPayableCorrespondentPayment")]
        public IHttpActionResult UpdateAccountsPayableCorrespondentPayment(AccountsPayableCorrespondent model)
        {
            var data = builder.UpdateAccountsPayableCorrespondentPayment(model);
            return Ok(data);
        }

        #endregion

        #endregion

        #region AccountsPayableCorrespondentDetail

        #region SearchAccountsPayableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/SearchAccountsPayableCorrespondentDetail")]
        public IHttpActionResult SearchAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetailRequest model)
        {
            var data = builder.SearchAccountsPayableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #region InsertAccountsPayableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/InsertAccountsPayableCorrespondentDetail")]
        public IHttpActionResult InsertAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail model)
        {
            var data = builder.InsertAccountsPayableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #region UpdateAccountsPayableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/UpdateAccountsPayableCorrespondentDetail")]
        public IHttpActionResult UpdateAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail model)
        {
            var data = builder.UpdateAccountsPayableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #region UpdateStatusAccountsPayableCorrespondentDetail

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusAccountsPayableCorrespondentDetail")]
        public IHttpActionResult UpdateStatusAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail model)
        {
            var data = builder.UpdateStatusAccountsPayableCorrespondentDetail(model);
            return Ok(data);
        }

        #endregion

        #endregion

        #region CompanyAlliance

        #region SearchCompanyAlliance

        [HttpPost]
        [Route("api/Operaciones/SearchCompanyAlliance")]
        public IHttpActionResult SearchCompanyAlliance(CompanyAllianceRequest model)
        {
            var data = builder.SearchCompanyAlliance(model);
            return Ok(data);
        }

        #endregion

        #endregion

        #region CompanyAllianceBank

        #region SearchCompanyAllianceBank

        [HttpPost]
        [Route("api/Operaciones/SearchCompanyAllianceBank")]
        public IHttpActionResult SearchCompanyAllianceBank(CompanyAllianceBankRequest model)
        {
            var data = builder.SearchCompanyAllianceBank(model);
            return Ok(data);
        }

        #endregion

        #endregion

        #region ShippingAlliance

        #region SearchShippingAlliance

        [HttpPost]
        [Route("api/Operaciones/SearchShippingAlliance")]
        public IHttpActionResult SearchShippingAlliance(ShippingAllianceRequest model)
        {
            var data = builder.SearchShippingAlliance(model);
            return Ok(data);
        }

        #endregion

        #region InsertShippingAlliance

        [HttpPost]
        [Route("api/Operaciones/InsertShippingAlliance")]
        public IHttpActionResult InsertShippingAlliance(ShippingAlliance model)
        {
            var data = builder.InsertShippingAlliance(model);
            return Ok(data);
        }

        #endregion

        #region UpdateOrderNumberShippingAlliance

        [HttpPost]
        [Route("api/Operaciones/UpdateOrderNumberShippingAlliance")]
        public IHttpActionResult UpdateOrderNumberShippingAlliance(ShippingAlliance model)
        {
            var data = builder.UpdateOrderNumberShippingAlliance(model);
            return Ok(data);
        }

        #endregion

        #endregion

        #region SearchRemesasGirosPendientes
        [HttpPost]
        [Route("api/Operaciones/SearchRemesasGirosPendientes")]
        public IHttpActionResult SearchRemesasGirosPendientes(OrdenEntranteRequest param)
        {
            var result = builder.SearchRemesasGirosPendientes(param);
            return Ok(result);
        }
        #endregion
        
        #region SearchInternationalMoneyOrderPayment
        [HttpPost]
        [Route("api/Operaciones/SearchInternationalMoneyOrderPayment")]
        public IHttpActionResult SearchInternationalMoneyOrderPayment(OrdenEntranteRequest param)
        {
            var result = builder.SearchInternationalMoneyOrderPayment(param);
            return Ok(result);
        }
        #endregion

        #region OPERACIONES

        #region SearchOperacionesPorPagar
        [HttpPost]
        [Route("api/Operaciones/SearchOperacionesPorPagar")]
        public IHttpActionResult SearchOperacionesPorPagar(OPERACIONES_POR_PAGARRequest param)
        {
            var result = builder.SearchOperacionesPorPagar(param);
            return Ok(result);
        }
        #endregion

        #region InsertOperacionesPorPagar
        [HttpPost]
        [Route("api/Operaciones/InsertOperacionesPorPagar")]
        public IHttpActionResult InsertOperacionesPorPagar(OPERACIONES_POR_PAGAR param)
        {
            var result = builder.InsertOperacionesPorPagar(param);
            return Ok(result);
        }
        #endregion

        #region InsertOperacionesPorCobrar

        [HttpPost]
        [Route("api/Operaciones/InsertOperacionesPorCobrar")]
        public IHttpActionResult InsertOperacionesPorCobrar(OperacionDeNegocio modelOperation)
        {
            var result = builder.InsertBusinessOperation(modelOperation);
            return Ok(result);
        }
        #endregion

        #region SearchOperacionesPorCobrar

        [HttpPost]
        [Route("api/Operaciones/SearchOperacionesPorCobrar")]
        public IHttpActionResult SearchOperacionesPorCobrar(OperacionesPorCobrarRequest param)
        {
            var result = builder.SearchOperacionesPorCobrar(param);
            return Ok(result);
        }

        #endregion

        #region ProcessCashierOperations
        [HttpPost]
        [Route("api/Operaciones/ProcessCashierOperations")]
        public IHttpActionResult ProcessCashierOperations(ProcessCashierOperation model)
        {
            var result = builder.ProcessCashierOperations(model);
            return Ok(result);

        }

        #endregion

        #region InsertOpPorCobrarRemesaEntrante

        [HttpPost]
        [Route("api/Operaciones/InsertOpPorCobrarRemesaEntrante")]
        public IHttpActionResult InsertOpPorCobrarRemesaEntrante(OperacionDeNegocio modelOperation)
        {
            var result = builder.InsertOpPorCobrarRemesaEntrante(modelOperation);
            return Ok(result);
        }

        #endregion

        #region ConfirmationOrderCashier

        [HttpPost]
        [Route("api/Operaciones/ConfirmationOrderCashier")]
        public IHttpActionResult ConfirmationOrderCashier(ProcessCashierOperation model)
        {
            var _operationNumber = Functions.GetProximoNumero(Constant.TipoNumeracion.OperacionDeCaja , model.BranchOffice, model.Login, false);
            var cashiers = builderContabilidad.ProcessOperationCashier(model,_operationNumber);
            cashiers.Select(S => { S.RowId = model.Orden.ID_ORDEN; return S; }).ToList();
            var result = builder.ConfirmationOrderCashier(model, cashiers);
            return Ok(result);
        }

        #endregion

        #region CashierOperations 

        #region SearchCashierOperations
        [HttpPost]
        [Route("api/Operaciones/SearchCashierOperations")]
        public IHttpActionResult SearchCashierOperations(SearchCashierOperationsRequest model)
        {

            MonedasRequest moneda = new MonedasRequest { MonedaActiva = true, TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33 };
            var resp = Functions.SearchMonedas(moneda);
            var result = builder.SearchCashierOperations(model);
            ///Se recorre la Lista de Operaciones para Cargarle los codigos y Simbolos a las Monedas de cada operacion
            foreach (var item in result)
            {
                item.Moneda = resp.Where(x => x.MonedaId == item.IdMoneda).FirstOrDefault().MonedaCodigoInt;
                item.MonedaSymb = resp.Where(x => x.MonedaId == item.IdMoneda).FirstOrDefault().MonedaSimbolo;
                item.MonedaCon = resp.Where(x => x.MonedaId == item.IdMonedaCon).FirstOrDefault().MonedaCodigoInt;
                item.MonedaConSymb = resp.Where(x => x.MonedaId == item.IdMonedaCon).FirstOrDefault().MonedaSimbolo;
            }

            return Ok(result);
        }

        #endregion

        #region UpdateStatusCashierOperations

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusCashierOperations")]
        public IHttpActionResult UpdateStatusCashierOperations(UpdateStatusCashierOperationsRequest model)
        {
            var data = builder.UpdateStatusCashierOperations(model);
            return Ok(data);
        }

        #endregion     

        #endregion

        #endregion

        #region Tarifas Aplicadas

        #region SearchTarifasAplicadas

        [HttpPost]
        [Route("api/Operaciones/SearchTarifasAplicadas")]
        public IHttpActionResult SearchTarifasAplicadas(Tarifas_Aplicadas model)
        {
            var result = builder.SearchTarifasAplicadas(model);
            return Ok(result);
        }

        #endregion

        #region UpdateOrdenTarifasAplicadas

        [HttpPost]
        [Route("api/Operaciones/UpdateOrdenTarifasAplicadas")]
        public IHttpActionResult UpdateOrdenTarifasAplicadas(Tarifas_Aplicadas model)
        {
            var result = builder.UpdateOrdenTarifasAplicadas(model);
            return Ok(result);
        }

        #endregion

        #endregion

        #region Anulaciones

        #region SearchOperationsCashierAnnulment

        [HttpPost]
        [Route("api/Operaciones/SearchOperationsCashierAnnulment")]
        public IHttpActionResult SearchOperationsCashierAnnulment(OperationsCashierAnnulmentRequest param)
        {
            var result = builder.SearchOperationsCashierAnnulment(param);
            return Ok(result);
        }

        #endregion

        #region SearchOperationsTempAnnulment

        [HttpPost]
        [Route("api/Operaciones/SearchOperationsTempAnnulment")]
        public IHttpActionResult SearchOperationsTempAnnulment(OperationsTempAnnulmentRequest param)
        {
            var result = builder.SearchOperationsTempAnnulment(param);
            return Ok(result);
        }

        #endregion

        #region InsertAnnulmentOperationTemp

        [HttpPost]
        [Route("api/Operaciones/InsertAnnulmentOperationTemp")]
        public IHttpActionResult InsertAnnulmentOperationTemp(Annulment param)
        {
            var result = builder.InsertAnnulmentOperationTemp(param);
            return Ok(result);
        }

        #endregion

        #region InsertAnnulmentOrder

        [HttpPost]
        [Route("api/Operaciones/InsertAnnulmentOrder")]
        public IHttpActionResult InsertAnnulmentOrder(Annulment param)
        {
            var result = builder.InsertAnnulmentOrder(param);
            return Ok(result);
        }

        #endregion

        #region SearchAnnulmentPending

        [HttpPost]
        [Route("api/Operaciones/SearchAnnulmentPending")]
        public IHttpActionResult SearchAnnulmentPending(AnnulmentPendingRequest param)
        {
            var result = builder.SearchAnnulmentPending(param);
            return Ok(result);
        }

        #endregion

        #region ValidateAproveAnnulment

        [HttpPost]
        [Route("api/Operaciones/ValidateAproveAnnulment")]
        public IHttpActionResult ValidateAproveAnnulment(Annulment param)
        {
            var result = builder.ValidateAproveAnnulment(param);
            return Ok(result);
        }

        #endregion

        #endregion

        #region RejectIPaymentInter
        [HttpPost]
        [Route("api/Operaciones/RejectIPaymentInter")]
        public IHttpActionResult RejectIPaymentInter(Ordenes model)
        {
            var resultDelete = builderPagos.DeletePayForConfirm(model);
            if (resultDelete.Error)
            {
                return Ok(resultDelete);
            }
            var result = builder.UpdateStatusOrdenes(model);
            return Ok(result);
        }
        #endregion

        #region SearchOperationsFrontOffice
        [HttpPost]
        [Route("api/Operaciones/SearchOperationsFrontOffice")]
        public IHttpActionResult SearchOperationsFrontOffice(OperationsFrontOfficeRequest param)
        {
            var result = builder.SearchOperationsFrontOffice(param);
            return Ok(result);
        }
        #endregion

        #region Entity
        #region ServiceBank
        [HttpPost]
        [Route("api/Operaciones/SearchServiceBank")]
        public IHttpActionResult SearchServiceBank(ServiceBankRequest request)
        {
            var result = builder.SearchServiceBank(request);
            return Ok(result);
        }
        #endregion

        #region ServicesBankType
        [HttpPost]
        [Route("api/Operaciones/SearchServicesBankType")]
        public IHttpActionResult SearchServicesBankType(ServicesBankTypeRequest request)
        {
            var result = builder.SearchServicesBankType(request);
            return Ok(result);
        }
        #endregion

        #region OnlinePayment
        [HttpPost]
        [Route("api/Operaciones/OnlinePaymentOperation")]
        public async Task<IHttpActionResult> OnlinePaymentOperation(OperationPaymentOnlineServives request)
        {
            var result = await builder.OnlinePaymentOperation(request);
            if(result.StatusId == Constant.StatusOnlinePayment.Rechazada)
            {
                return Ok(result);
            }

            var resultOperation = ProcesarRemesaEntrante(request.OperationId, result.Payment, result.PaymentDetail, request.BranchId);
            if(resultOperation.Error)
            {
                result.StatusId = Constant.StatusOnlinePayment.Rechazada;
                result.Message = "Se ha realizado el pago de la operación en el banco, pero no se logro realizar el registro en CCAL: referencia=" + result.Reference + ". " + resultOperation.ErrorMessage;
                result.Status = "Rechazada CCAL";
                return Ok(result); 
            }
            var updateResponse = UpdateStatusRemesaEntrante(8, request.CreationUserLogin, request.OperationId, request.BranchId, result.Payment.OrderId, string.Empty, result.Reference, null);

            if (updateResponse.Error)
            {
                result.StatusId = Constant.StatusOnlinePayment.Rechazada;
                result.Message = "Se ha realizado el pago de la operación en el banco, pero no se logro realizar el registro en CCAL: referncia=" + result.Reference + ". " + updateResponse.ErrorMessage;
                result.Status = "Rechazada CCAL";
            }
            return Ok(result);
        }

        [Route("api/Operaciones/OnlinePaymentOperationQuery")]
        [HttpPost]
        public async Task<IHttpActionResult> OnlinePaymentOperationQuery(OperationPaymentQueryOnlineServives request)
        {
            try
            {
                return Ok(await builder.OnlinePaymentOperationQuery(request));
            }
            catch (Exception ex)
            {
                return Ok(new GenericResponse { Error = true, ErrorMessage = ex.Message });
            }
        }
        #endregion

        #region BatchBankOperationOnLine
        [HttpPost]
        [Route("api/Operaciones/InsertBatchBankOperationOnLine")]
        public IHttpActionResult InsertBatchBankOperationOnLine(BatchBankOperationOnline request)
        {
            return Ok(builder.InsertBatchBankOperationOnLine(request));
        }

        [HttpPost]
        [Route("api/Operaciones/SearchBatchBankOperationOnline")]
        public IHttpActionResult SearchBatchBankOperationOnline(BatchBankOperationOnlineRequest request)
        {
            return Ok(builder.SearchBatchBankOperationOnline(request));
        }
        [HttpPost]
        [Route("api/Operaciones/UpdateBatchBankOperationOnline")]
        public IHttpActionResult UpdateBatchBankOperationOnline(BatchBankOperationOnline request)
        {
            return Ok(builder.UpdateBatchBankOperationOnline(request));
        }
        #endregion

        #region UpdatePagosStatusOrdenes
        [HttpPost]
        [Route("api/Operaciones/UpdatePagosStatusOrdenes")]
        public IHttpActionResult UpdatePagosStatusOrdenes(Ordenes model)
        {
            var result = builder.UpdatePagosStatusOrdenes(model);
            return Ok(result);

        }
        #endregion

        #region ProccesCashierRemesa

        [HttpPost]
        [Route("api/Operaciones/ProccesCashierRemesa")]
        public IHttpActionResult ProccesCashierRemesa(ProcessCashierOperation model)
        {
            var result = builder.ProccesCashierRemesa(model);
            return Ok(result);
        }

        #endregion

        #region SearchOperationsTempPending

        [HttpPost]
        [Route("api/Operaciones/SearchOperationsTempPending")]
        public IHttpActionResult SearchOperationsTempPending()
        {
            var result = builder.SearchOperationsTempPending();
            return Ok(result);
        }

        #endregion

        #region UpdateStatusOperationsTempEntity

        [HttpPost]
        [Route("api/Operaciones/UpdateStatusOperationsTempEntity")]
        public IHttpActionResult UpdateStatusOperationsTempEntity(OperacionesPorCobrar model)
        {
            var result = builder.UpdateStatusOperationsTempEntity(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/Operaciones/UpdateAnnulmentBatchBankOperationOnline")]
        public IHttpActionResult UpdateAnnulmentBatchBankOperationOnline(BatchBankOperationOnline request)
        {
            return Ok(builder.UpdateAnnulmentBatchBankOperationOnline(request));
        }
        #endregion

        #region BatchBankOperationDetailOnLine
        [HttpPost]
        [Route("api/Operaciones/ProcessBatchBankOperationDetailOnline")]
        public IHttpActionResult ProcessBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request)
        {
            return Ok(builder.ProcessBatchBankOperationDetailOnline(request));
        }

        [HttpPost]
        [Route("api/Operaciones/SearchBatchBankOperationDetailOnline")]
        public IHttpActionResult SearchBatchBankOperationDetailOnline(BatchBankOperationDetailOnlineRequest request)
        {
            return Ok(builder.SearchBatchBankOperationDetailOnline(request));
        }

        [HttpPost]
        [Route("api/Operaciones/UpdateBatchBankOperationDetailOnline")]
        public IHttpActionResult UpdateBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request)
        {
            return Ok(builder.UpdateBatchBankOperationDetailOnline(request));
        }
        #endregion

        #region PaymentOnlineBankResponseEntity
        [HttpPost]
        [Route("api/Operaciones/InsertPaymentOnlineBankResponseEntity")]
        public IHttpActionResult InsertPaymentOnlineBankResponseEntity(PaymentOnlineBankResponseEntity request)
        {
            return Ok(builder.InsertPaymentOnlineBankResponseEntity(request));
        }
        #endregion
        #endregion
    }
}
