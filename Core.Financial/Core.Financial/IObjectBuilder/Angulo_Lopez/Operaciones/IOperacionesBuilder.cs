using Common.Models.Angulo_Lopez;
using Common.Models.Angulo_Lopez.Contabilidad;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Common.Models.Common;
using Common.Models.Angulo_Lopez.Operaciones.Services;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IObjectBuilder.Operaciones
{
    public interface IOperacionesBuilder : IFinalize
    {
        #region Ordenes
        Ordenes SearchOrderPayment(OrderPaymentRequest param);
        GenericResponse UpdateStatusOrdenes(Ordenes model);
        GenericResponse UpdateStatusOrder(Ordenes model);
        
        HashSet<Ordenes> SearchOrdenesByFilter(OrderHistoryRequest model);
        HashSet<Ordenes> SearchORDENES(OrdenesRequest param);
        HashSet<OrdenSalienteExterno> SearchOrdenesTransmitirPendientes(string corresponsal); 
        HashSet<Ordenes> SearchTurnAlert(OrdenesRequest param);
        HashSet<SolicitudesBackoffice> SearchPaymentOrdersNotCanceled(OrdenesRequest param);
        HashSet<SolicitudesBackoffice> SearchPaymentOrdersDivisaTaquilla(OrdenesRequest param);
        HashSet<SolicitudesBackoffice> SearchPaymentOrderVentanilla(OrdenesRequest param);
        HashSet<SolicitudesBackoffice> SearchPaymentOrdersTrasnfer(OrdenesRequest param);
        HashSet<Ordenes> SearchPaidOrders(PaidOrdersRequest param);
        GenericResponse InsertOrdenes(Ordenes model);
        GenericResponse RollBackOrden(int? ID_ORDEN, int ID_TEMPORAL, int? ID_PAGO, int NroFactura, string SUCURSAL);
        HashSet<Ordenes> SearchOrdenesPayableCorrespondent(OrdenesRequest param);
        HashSet<Ordenes> SearchOrderPendingCorrespondet(OrderHistoryRequest model);
        GenericResponse ValidateOperationClientPause(int clientId);
        HashSet<Ordenes> SearchReturnFundsOrder(OrdenesRequest model);
        GenericResponse UpdateOrdenSalienteTransmitida(int id);
        HashSet<OrdenSalienteExterno> SearchOrdenesSalientesAnularPendientes(string corresponsal);
        HashSet<SolicitudesBackoffice> SearchPaymentOrderInternational(OrdenesRequest param);
        #endregion

        HashSet<Operations> SearchOperations(OperationsRequest param);

        #region BatchBankOperations
        HashSet<BatchBankOperations> SearchBatchBankOperations(BatchBankOperationsRequest param);
        GenericResponse UpdateStatusOrdenesEntity(Ordenes model);
        GenericResponse InsertBatchBankOperations(BatchBankOperations param);
        GenericResponse InsertBatchBankOperationIntegrated(BatchBankOperations model);
        GenericResponse UpdateStatusBatchBankOperations(BatchBankOperations param);
        GenericResponse SearchBatchBankOperationsNumber();
        #endregion

        #region BatchBankDetail
        HashSet<BatchBankDetail> SearchBatchBankDetail(BatchBankDetailRequest param);
        GenericResponse InsertBatchBankDetail(BatchBankDetail param);
        GenericResponse UpdateStatusBatchBankDetail(BatchBankDetail param);
        #endregion

        #region Remesas Enrantes

        #region SearchRemesaEntranteFromOrdenes

        OrdenCompraEfectivo SearchRemesaEntranteFromOrdenes(int id);

        #endregion

        #region RollbackRemesaPagadas

        GenericResponse RollbackRemesaPagadas(int id, bool remesaEntrante);

        #endregion

        #region SearchRemesasEntrantesFormBatch

        HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantesFormBatch(OrdenEntranteRequest model);
        GenericResponse RollbackMassRemesaPagadas(string id, bool remesaEntrante, string detailId);

        #endregion

        #region SearchRemesasEntrantesFormBatchPorConfirmar

        HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantesFormBatchPorConfirmar(OrdenEntranteRequest model);

        #endregion

        #region UpdateStatusRemesasEntrantesIncidencias

        GenericResponse UpdateStatusRemesasEntrantesIncidencias(REMESAS_ENTRANTES param);

        #endregion

        #region RemesaEntrantePagadaPendientePorConfirmar

        GenericResponse spuRemesaEntrantePagadaPendientePorConfirmar(RemesaEntrantePagadaPendientePorConfirmar param);

        #endregion

        #region UpdateRemesasEntrantesIncidenciaRIA

        GenericResponse UpdateRemesasEntrantesIncidenciaRIA(REMESAS_ENTRANTES param);

        #endregion

        #region SearchRemesasEntrantesRejectRIA
        HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantesRejectRIA(OrdenEntranteRequest param);
        #endregion

        #region InsertRemesaEntranteWithoutValidationProcess
        GenericResponse InsertRemesaEntranteWithoutValidationProcess(REMESAS_ENTRANTES param);
        #endregion

        #endregion

        #region Pago de lote
        GenericResponse UpdateStatusLotPayment(BatchBankDetail param);
        #endregion

        #region AccountReceivableCorrespondent

        #region SearchAccountReceivableCorrespondent

        HashSet<AccountReceivableCorrespondent> SearchAccountReceivableCorrespondent(AccountReceivableCorrespondentRequest param);

        #endregion

        #region InsertAccountReceivableCorrespondent
        HashSet<GenericResponse> InsertAccountReceivableCorrespondent(AccountReceivableCorrespondent param);

        #endregion

        #region UpdateAccountReceivableCorrespondent

        GenericResponse UpdateAccountReceivableCorrespondent(AccountReceivableCorrespondent param);

        #endregion

        #region UpdateAccountReceivableCorrespondentRemoveDetail

        GenericResponse UpdateAccountReceivableCorrespondentRemoveDetail(AccountReceivableCorrespondent param);

        #endregion

        #region UpdateStatusAccountReceivableCorrespondent

        GenericResponse UpdateStatusAccountReceivableCorrespondent(AccountReceivableCorrespondent param);

        #endregion

        #region UpdateAccountReceivableCorrespondentPayment

        GenericResponse UpdateAccountReceivableCorrespondentPayment(AccountReceivableCorrespondent param);

        #endregion

        #region SearchDraftsReceivable
        HashSet<OrdenEntranteBackoffice> SearchDraftsReceivable(OrdenEntranteRequest model);
        #endregion
        #endregion

        #region AccountReceivableCorrespondentDetail

        #region SearchAccountReceivableCorrespondentDetail

        HashSet<AccountReceivableCorrespondentDetail> SearchAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetailRequest param);

        #endregion

        #region InsertAccountReceivableCorrespondentDetail

        GenericResponse InsertAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail param);

        #endregion

        #region UpdateAccountReceivableCorrespondentDetail

        GenericResponse UpdateAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail param);

        #endregion

        #region UpdateStatusAccountReceivableCorrespondentDetail

        GenericResponse UpdateStatusAccountReceivableCorrespondentDetail(AccountReceivableCorrespondentDetail param);

        #endregion

        #endregion

        #region AccountsPayableCorrespondent

        #region SearchAccountsPayableCorrespondent

        HashSet<AccountsPayableCorrespondent> SearchAccountsPayableCorrespondent(AccountsPayableCorrespondentRequest param);

        #endregion

        #region InsertAccountsPayableCorrespondent
        HashSet<GenericResponse> InsertAccountsPayableCorrespondent(AccountsPayableCorrespondent param);

        #endregion

        #region UpdateAccountsPayableCorrespondent

        GenericResponse UpdateAccountsPayableCorrespondent(AccountsPayableCorrespondent param);

        #endregion

        #region UpdateAccountsPayableCorrespondentRemoveDetail

        GenericResponse UpdateAccountsPayableCorrespondentRemoveDetail(AccountsPayableCorrespondent param);

        #endregion

        #region UpdateStatusAccountsPayableCorrespondent

        GenericResponse UpdateStatusAccountsPayableCorrespondent(AccountsPayableCorrespondent param);

        #endregion

        #region UpdateAccountsPayableCorrespondentPayment

        GenericResponse UpdateAccountsPayableCorrespondentPayment(AccountsPayableCorrespondent param);

        #endregion

        #endregion

        #region AccountsPayableCorrespondentDetail

        #region SearchAccountsPayableCorrespondentDetail

        HashSet<AccountsPayableCorrespondentDetail> SearchAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetailRequest param);

        #endregion

        #region InsertAccountsPayableCorrespondentDetail

        GenericResponse InsertAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail param);

        #endregion

        #region UpdateAccountsPayableCorrespondentDetail

        GenericResponse UpdateAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail param);

        #endregion

        #region UpdateStatusAccountsPayableCorrespondentDetail

        GenericResponse UpdateStatusAccountsPayableCorrespondentDetail(AccountsPayableCorrespondentDetail param);

        #endregion

        #endregion

        #region CompanyAlliance

        #region SearchCompanyAlliance

        HashSet<CompanyAlliance> SearchCompanyAlliance(CompanyAllianceRequest param);

        #endregion

        #endregion

        #region CompanyAllianceBank

        #region SearchCompanyAllianceBank

        HashSet<CompanyAllianceBank> SearchCompanyAllianceBank(CompanyAllianceBankRequest param);

        #endregion

        #endregion

        #region ShippingAlliance

        #region SearchShippingAlliance

        HashSet<ShippingAlliance> SearchShippingAlliance(ShippingAllianceRequest param);

        #endregion

        #region InsertShippingAlliance

        GenericResponse InsertShippingAlliance(ShippingAlliance param);

        #endregion

        #region UpdateOrderNumberShippingAlliance

        GenericResponse UpdateOrderNumberShippingAlliance(ShippingAlliance param);

        #endregion

        #region RollbackShippingAlliance
        GenericResponse RollbackShippingAlliance(ShippingAlliance param);

        #endregion

        #endregion

        HashSet<OrdenEntranteBackoffice> SearchRemesasGirosPendientes(OrdenEntranteRequest param);

        #region SearchInternationalMoneyOrderPayment
        HashSet<OrdenEntranteBackoffice> SearchInternationalMoneyOrderPayment(OrdenEntranteRequest param);
        #endregion

        #region OPERACIONES 

        #region SearchOperacionesPorPagar
        HashSet<OPERACIONES_POR_PAGAR> SearchOperacionesPorPagar(OPERACIONES_POR_PAGARRequest param);
        #endregion

        #region InsertOperacionesPorPagar
        GenericResponse InsertOperacionesPorPagar(OPERACIONES_POR_PAGAR param);
        #endregion

        #region InsertOperacionesPorCobrar
        GenericResponse InsertOperacionesPorCobrar(OperacionesPorCobrar param);
        #endregion

        #region SearchOperacionesPorCobrar
        HashSet<OperacionesPorCobrar> SearchOperacionesPorCobrar(OperacionesPorCobrarRequest param);
        #endregion

        #region DeleteOperacionesPorCobrar
        GenericResponse DeleteOperacionesPorCobrar(OperacionesPorCobrar param);
        #endregion

        #region ProcessCashierOperations
        GenericResponse ProcessCashierOperations(ProcessCashierOperation param);
        #endregion

        #region InsertOpPorCobrarRemesaEntrante
        OperacionDeNegocio InsertOpPorCobrarRemesaEntrante(OperacionDeNegocio param);
        #endregion

        #region CashierOperations

        #region SearchCashierOperations
        HashSet<SearchCashierOperations> SearchCashierOperations(SearchCashierOperationsRequest param);
        #endregion

        #region UpdateStatusCashierOperations

        GenericResponse UpdateStatusCashierOperations(UpdateStatusCashierOperationsRequest param);

        #endregion

        #endregion

        #region InsertMixedOrder
        GenericResponse InsertMixedOrder(ProcessCashierOperation param);
        #endregion

        #region UpdateMixedOrderReferenceBCV
        GenericResponse UpdateMixedOrderReferenceBCV(Ordenes param);
        #endregion

        #region ProcessMixedOrder

        GenericResponse ProcessMixedOrder(ProcessCashierOperation param);

        #endregion

        #endregion

        #region Tarifas Aplicadas

        #region SearchTarifasAplicadas

        HashSet<SearchTarifas_Aplicadas> SearchTarifasAplicadas(Tarifas_Aplicadas param);

        #endregion

        #region UpdateOrdenTarifasAplicadas

        GenericResponse UpdateOrdenTarifasAplicadas(Tarifas_Aplicadas param);

        #endregion

        #endregion

        #region Anulaciones

        #region SearchOperationsCashierAnnulment

        HashSet<OrdenAnulables> SearchOperationsCashierAnnulment(OperationsCashierAnnulmentRequest param);

        #endregion

        #region SearchOperationsTempAnnulment

        HashSet<SearchCashierOperations> SearchOperationsTempAnnulment(OperationsTempAnnulmentRequest param);

        #endregion

        #region SearchAnnulment
        HashSet<Annulment> SearchAnnulment(AnnulmentRequest param);
        #endregion

        #region SearchAnnulmentPending
        HashSet<AnnulmentPending> SearchAnnulmentPending(AnnulmentPendingRequest param);
        #endregion

        #region InsertAnnulment
        GenericResponse InsertAnnulment(Annulment param);
        #endregion
   
        #region InsertAnnulmentOperationTemp
        GenericResponse InsertAnnulmentOperationTemp(Annulment param);
        #endregion

        #region InsertAnnulmentOrder
        GenericResponse InsertAnnulmentOrder(Annulment param);
        #endregion

        #region UpdateStatusAnnulmentOrder
        GenericResponse UpdateStatusAnnulmentOrder(StatusAnnulOrderRequest param);
        #endregion

        #region UpdateStatusAnnulmentRemesaEntrante
        GenericResponse UpdateStatusAnnulmentRemesaEntrante(StatusAnnulRemesaEntranteRequest param);
        #endregion

        #region UpdateAnnulment
        GenericResponse UpdateAnnulment(Annulment param);
        #endregion

        #region ValidateAproveAnnulment
        GenericResponse ValidateAproveAnnulment(Annulment param);
        #endregion

        #endregion

        #region ConfirmationOrderCashier
        GenericResponse ConfirmationOrderCashier(ProcessCashierOperation param, List<Cashier> cashiers);
        #endregion

        #region SearchOperationsFrontOffice
        HashSet<OperationsFrontOffice> SearchOperationsFrontOffice(OperationsFrontOfficeRequest param);
        #endregion

        #region UpdatePagosStatusOrdenes

        GenericResponse UpdatePagosStatusOrdenes(Ordenes param);

        #endregion


        #region InsertOrdenesEntity
        GenericResponse InsertOrdenesEntity(Ordenes param);
        #endregion

        #region ProccesCashierRemesa

        GenericResponse ProccesCashierRemesa(ProcessCashierOperation param);

        #endregion

        List<OperationsTempPending> SearchOperationsTempPending();
        GenericResponse UpdateStatusOperationsTempEntity(OperacionesPorCobrar model);
        ServiceBankError SearchErrorBank(long nroError);

        #region Entity
        #region ServiceBank

        List<ServiceBank> SearchServiceBank(ServiceBankRequest request);
        #endregion

        #region ServicesBankType
        List<ServicesBankType> SearchServicesBankType(ServicesBankTypeRequest request);
        #endregion

        #region OnlinePayment
        Task<OperationPaymentOnlineResponse> OnlinePaymentOperation(OperationPaymentOnlineServives request);
        Task<GenericResponse> OnlinePaymentOperationQuery(OperationPaymentQueryOnlineServives request);
        #endregion

        #region BatchBankOperationOnLine
        GenericResponse InsertBatchBankOperationOnLine(BatchBankOperationOnline request);
        GenericResponse SearchBatchBankOperationOnline(BatchBankOperationOnlineRequest request);
        GenericResponse UpdateBatchBankOperationOnline(BatchBankOperationOnline request);
        GenericResponse UpdateAnnulmentBatchBankOperationOnline(BatchBankOperationOnline request);
        #endregion

        #region BatchBankOperationDetailOnLine
        GenericResponse ProcessBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request);
        GenericResponse SearchBatchBankOperationDetailOnline(BatchBankOperationDetailOnlineRequest request);
        GenericResponse UpdateBatchBankOperationDetailOnline(BatchBankOperationDetailOnline request);
        #endregion

        #region PaymentOnlineBankResponseEntity
        GenericResponse InsertPaymentOnlineBankResponseEntity(PaymentOnlineBankResponseEntity request);
        #endregion
        #endregion


        OperacionDeNegocio InsertBusinessOperation(OperacionDeNegocio modelOperation);
        AccumulatedAmount ValidateAccumulatedAmount(ValidateRequestAmount param);

    }
}
