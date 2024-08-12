using Common.Models.Angulo_Lopez;
using Common.Models.Angulo_Lopez.Ciudades;
using Common.Models.Angulo_Lopez.Ingresos;
using Common.Models.Angulo_Lopez.Monedas;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Common.Models.Angulo_Lopez.Pagos;
using Common.Models.Common;
using DataAccess.Angulo_Lopez;
using IDataAccess.Angulo_Lopez;
using IObjectBuilder.Angulo_Lopez;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Ciudad = Common.Models.Angulo_Lopez.Ciudades.Ciudad;
using Nomeda = Common.Models.Angulo_Lopez.Monedas.Moneda;
using Moneda = Common.Models.Common.Moneda;
using Common.Models.Angulo_Lopez.Remitee;

namespace ObjectBuilder.Angulo_Lopez
{
    public class AnguloLopezBuilder : Finalize, IAnguloLopezBuilder
    {
        #region Schema Operaciones
        #region Solicitudes
        public HashSet<Solicitudes> SearchSolicitudes(SolicitudesRequest request)
        {
            object responseData;
            using (IAnguloLopezDal<Solicitudes> dal = new AnguloLopezDal<Solicitudes>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CLIENTE", SqlDbType.VarChar, 14){Value = request.CLIENTE?? (object)DBNull.Value},
                    new SqlParameter("@SUCURSAL", SqlDbType.Int){Value = request.SUCURSAL?? (object)DBNull.Value},
                    new SqlParameter("@DETALLE_TIPO_OPERACION", SqlDbType.Int){Value = request.DETALLE_TIPO_OPERACION?? (object)DBNull.Value},
                    new SqlParameter("@STATUS_SOLICITUD", SqlDbType.Int){Value = request.STATUS_SOLICITUD?? (object)DBNull.Value},
                    new SqlParameter("@SolicitudIds", SqlDbType.VarChar, 200){Value = request.SolicitudIds?? (object)DBNull.Value}
                };
                return dal.SearchSolicitudes(param);
            }
            //return (GenericResponse)responseData;
        }

        public GenericResponse InsertSolicitudes(Solicitudes request)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@DETALLE_TIPO_OPERACION", SqlDbType.Int){Value = request.DETALLE_TIPO_OPERACION},
                    new SqlParameter("@CLIENTE", SqlDbType.VarChar, 14){Value = request.CLIENTE},
                    new SqlParameter("@SUCURSAL", SqlDbType.Int){Value = request.SUCURSAL},
                    new SqlParameter("@STATUS_SOLICITUD", SqlDbType.Int){Value = request.STATUS_SOLICITUD},
                    new SqlParameter("@MONEDA", SqlDbType.Int){Value = request.MONEDA},
                    new SqlParameter("@OFICINA", SqlDbType.Int){Value = (object)request.OFICINA??DBNull.Value},
                    new SqlParameter("@PERSONA", SqlDbType.Int){Value = (object)request.PERSONA??DBNull.Value},
                    new SqlParameter("@PAIS_DESTINO", SqlDbType.Char, 2){Value = (object)request.PAIS_DESTINO??DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char, 3){Value = (object)request.CORRESPONSAL??DBNull.Value},
                    new SqlParameter("@AGENTE", SqlDbType.Char, 3) { Value = (object)request.AGENTE ?? DBNull.Value },
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.VarChar, 15) { Value = request.REGISTRADOPOR},
                    new SqlParameter("@NUMERO", SqlDbType.Int){Value = request.NUMERO},
                    new SqlParameter("@TIPO_CAMBIO", SqlDbType.Money){Value = request.TIPO_CAMBIO},
                    new SqlParameter("@MONTO", SqlDbType.Money){Value = request.MONTO},
                    new SqlParameter("@TASA_DESTINO", SqlDbType.Money){Value = (object)request.TASA_DESTINO??DBNull.Value},
                    new SqlParameter("@MONTO_CAMBIO", SqlDbType.Money){Value = (object)request.MONTO_CAMBIO??DBNull.Value},
                    new SqlParameter("@FECHA_VALOR_TASA", SqlDbType.SmallDateTime){Value = (object)request.FECHA_VALOR_TASA??DBNull.Value},
                    new SqlParameter("@MOTIVO_OP_BCV", SqlDbType.Int){Value = (object)request.MOTIVO_OP_BCV??DBNull.Value},
                    new SqlParameter("@TIPO_OP_BCV", SqlDbType.VarChar, 10){Value = (object)request.TIPO_OP_BCV??DBNull.Value},
                    new SqlParameter("@NOMBRES_REMITENTE", SqlDbType.VarChar, 100){Value = (object)request.NOMBRES_REMITENTE??DBNull.Value},
                    new SqlParameter("@APELLIDOS_REMITENTE", SqlDbType.VarChar, 100){Value = (object)request.APELLIDOS_REMITENTE??DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar, 30){Value = (object)request.IDENTIFICACION_REMITENTE??DBNull.Value},
                    new SqlParameter("@NOMBRES_BENEFICIARIO", SqlDbType.VarChar, 30){Value = (object)request.NOMBRES_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@APELLIDOS_BENEFICIARIO", SqlDbType.VarChar, 100){Value = (object)request.APELLIDOS_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_BENEFICIARIO", SqlDbType.VarChar, 30){Value = (object)request.IDENTIFICACION_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@SECUENCIA", SqlDbType.Int){Value = (object)request.SECUENCIA??DBNull.Value},
                    new SqlParameter("@FECHA_ENVIO", SqlDbType.SmallDateTime){Value = (object)request.FECHA_ENVIO??DBNull.Value},
                    new SqlParameter("@REFERENCIA_PAGO", SqlDbType.VarChar, 50){Value = (object)request.REFERENCIA_PAGO??DBNull.Value},
                    new SqlParameter("@FECHA_PAGO", SqlDbType.SmallDateTime){Value = (object)request.FECHA_PAGO??DBNull.Value},
                    new SqlParameter("@REFERENCIA_ANULACION", SqlDbType.VarChar, 50){Value = (object)request.REFERENCIA_ANULACION??DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION", SqlDbType.SmallDateTime){Value = (object)request.FECHA_ANULACION??DBNull.Value},
                    new SqlParameter("@REFERENCIA_ORDEN", SqlDbType.VarChar, 50){Value = (object)request.REFERENCIA_ORDEN??DBNull.Value},
                    new SqlParameter("@BANCO_NACIONAL", SqlDbType.VarChar, 3){Value = (object)request.BANCO_NACIONAL??DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA", SqlDbType.VarChar, 50){Value = (object)request.NUMERO_CUENTA??DBNull.Value},
                    new SqlParameter("@EMAIL_CLIENTE", SqlDbType.VarChar, 50){Value = (object)request.EMAIL_CLIENTE??DBNull.Value},
                    new SqlParameter("@EMAIL_BENEFICIARIO", SqlDbType.VarChar, 50){Value = (object)request.EMAIL_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@OBSERVACIONES", SqlDbType.Text){Value = (object)request.OBSERVACIONES??DBNull.Value},
                    new SqlParameter("@BANCO_DESTINO", SqlDbType.VarChar, 150){Value = (object)request.BANCO_DESTINO??DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA_DESTINO", SqlDbType.VarChar, 50){Value = (object)request.NUMERO_CUENTA_DESTINO??DBNull.Value},
                    new SqlParameter("@DIRECCION_BANCO", SqlDbType.VarChar){Value = (object)request.DIRECCION_BANCO??DBNull.Value},
                    new SqlParameter("@ABA", SqlDbType.VarChar, 50){Value = (object)request.ABA??DBNull.Value},
                    new SqlParameter("@SWIFT", SqlDbType.VarChar, 50){Value = (object)request.SWIFT??DBNull.Value},
                    new SqlParameter("@IBAN", SqlDbType.VarChar, 50){Value = (object)request.IBAN??DBNull.Value},
                    new SqlParameter("@TELEFONO_BENEFICIARIO", SqlDbType.VarChar, 50){Value = (object)request.TELEFONO_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@TELEFONO_CLIENTE", SqlDbType.VarChar, 50){Value = (object)request.TELEFONO_CLIENTE??DBNull.Value},
                    new SqlParameter("@BANCO_INTERMEDIARIO", SqlDbType.VarChar, 150){Value = (object)request.BANCO_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.NUMERO_CUENTA_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@DIRECCION_BANCO_INTERMEDIARIO", SqlDbType.VarChar){Value = (object)request.DIRECCION_BANCO_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@ABA_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.ABA_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@SWIFT_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.SWIFT_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@IBAN_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.IBAN_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@USUARIO_TAQUILLA", SqlDbType.VarChar, 15){Value = (object)request.USUARIO_TAQUILLA??DBNull.Value},
                    new SqlParameter("@AnulAutorizadaPor", SqlDbType.VarChar, 15){Value = (object)request.AnulAutorizadaPor??DBNull.Value},
                    new SqlParameter("@AnulProcesada", SqlDbType.Bit){Value = request.AnulProcesada},
                    new SqlParameter("@ReferenciaAnulBcv", SqlDbType.VarChar, 50){Value = (object)request.ReferenciaAnulBcv??DBNull.Value},
                    new SqlParameter("@AnuladaPor", SqlDbType.VarChar, 15){Value = (object)request.AnuladaPor??DBNull.Value},
                    new SqlParameter("@CONCILIADO", SqlDbType.Bit){Value = (object)request.CONCILIADO??DBNull.Value},
                    new SqlParameter("@FECHA_CONCILIACION", SqlDbType.SmallDateTime){Value = (object)request.FECHA_CONCILIACION??DBNull.Value},
                    new SqlParameter("@CONCILIADO_POR", SqlDbType.VarChar, 15){Value = (object)request.CONCILIADO_POR??DBNull.Value},
                    new SqlParameter("@OBSERVACIONES_CONCILIACION", SqlDbType.VarChar, 1000){Value = (object)request.OBSERVACIONES_CONCILIACION??DBNull.Value},
                    new SqlParameter("@TasaConversion", SqlDbType.Money){Value = (object)request.TasaConversion??DBNull.Value},
                    new SqlParameter("@MonedaOperacion", SqlDbType.Int){Value = (object)request.MonedaOperacion??DBNull.Value},
                    new SqlParameter("@MontoConversion", SqlDbType.Money){Value = (object)request.MontoConversion??DBNull.Value},
                    new SqlParameter("@BancoPagoTransferencia", SqlDbType.VarChar, 200){Value = (object)request.BancoPagoTransferencia??DBNull.Value},
                    new SqlParameter("@NumeroCuentaPagoTransferencia", SqlDbType.VarChar, 20){Value = (object)request.NumeroCuentaPagoTransferencia??DBNull.Value},
                    new SqlParameter("@TransferenciaPagada", SqlDbType.Bit){Value = (object)request.TransferenciaPagada??DBNull.Value},
                    new SqlParameter("@FechaPagoTransferencia", SqlDbType.SmallDateTime){Value = (object)request.FechaPagoTransferencia??DBNull.Value},
                    new SqlParameter("@UsuarioPagoTransferencia", SqlDbType.VarChar, 15){Value = (object)request.UsuarioPagoTransferencia??DBNull.Value},
                    new SqlParameter("@ProcesarTransferencia", SqlDbType.Bit){Value = (object)request.ProcesarTransferencia??DBNull.Value},
                    new SqlParameter("@CommissionUsd", SqlDbType.Money){Value = (object)request.CommissionUsd??DBNull.Value},
                    new SqlParameter("@CommissionBs", SqlDbType.Money){Value = (object)request.CommissionBs??DBNull.Value},
                    new SqlParameter("@ExchangeDifferential", SqlDbType.Money){Value = (object)request.ExchangeDifferential??DBNull.Value},
                    new SqlParameter("@TypeAccountBank", SqlDbType.Char,10){Value = (object)request.TypeAccountBank??DBNull.Value},
                };
                return dal.InsertSolicitudes(param);
            }
        }

        public GenericResponse UpdateSolicitudes(Solicitudes request)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_SOLICITUD", SqlDbType.Int){Value = request.ID_SOLICITUD},
                    new SqlParameter("@DETALLE_TIPO_OPERACION", SqlDbType.Int){Value = request.DETALLE_TIPO_OPERACION},
                    new SqlParameter("@SUCURSAL", SqlDbType.Int){Value = request.SUCURSAL},
                    new SqlParameter("@STATUS_SOLICITUD", SqlDbType.Int){Value = request.STATUS_SOLICITUD},
                    new SqlParameter("@MONEDA", SqlDbType.Int){Value = request.MONEDA},
                    new SqlParameter("@OFICINA", SqlDbType.Int){Value = (object)request.OFICINA??DBNull.Value},
                    new SqlParameter("@PERSONA", SqlDbType.Int){Value = (object)request.PERSONA??DBNull.Value},
                    new SqlParameter("@PAIS_DESTINO", SqlDbType.Char, 2){Value = (object)request.PAIS_DESTINO??DBNull.Value},
                    new SqlParameter("@CORRESPONSAL", SqlDbType.Char, 3){Value = (object)request.CORRESPONSAL??DBNull.Value},
                    new SqlParameter("@AGENTE", SqlDbType.Char, 3) { Value = (object)request.AGENTE ?? DBNull.Value },
                    new SqlParameter("@NUMERO", SqlDbType.Int){Value = request.NUMERO},
                    new SqlParameter("@TIPO_CAMBIO", SqlDbType.Money){Value = request.TIPO_CAMBIO},
                    new SqlParameter("@MONTO", SqlDbType.Money){Value = request.MONTO},
                    new SqlParameter("@TASA_DESTINO", SqlDbType.Money){Value = (object)request.TASA_DESTINO??DBNull.Value},
                    new SqlParameter("@MONTO_CAMBIO", SqlDbType.Money){Value = (object)request.MONTO_CAMBIO??DBNull.Value},
                    new SqlParameter("@FECHA_VALOR_TASA", SqlDbType.SmallDateTime){Value = (object)request.FECHA_VALOR_TASA??DBNull.Value},
                    new SqlParameter("@MOTIVO_OP_BCV", SqlDbType.Int){Value = (object)request.MOTIVO_OP_BCV??DBNull.Value},
                    new SqlParameter("@TIPO_OP_BCV", SqlDbType.VarChar, 10){Value = (object)request.TIPO_OP_BCV??DBNull.Value},
                    new SqlParameter("@NOMBRES_REMITENTE", SqlDbType.VarChar, 100){Value = (object)request.NOMBRES_REMITENTE??DBNull.Value},
                    new SqlParameter("@APELLIDOS_REMITENTE", SqlDbType.VarChar, 100){Value = (object)request.APELLIDOS_REMITENTE??DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_REMITENTE", SqlDbType.VarChar, 30){Value = (object)request.IDENTIFICACION_REMITENTE??DBNull.Value},
                    new SqlParameter("@NOMBRES_BENEFICIARIO", SqlDbType.VarChar, 30){Value = (object)request.NOMBRES_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@APELLIDOS_BENEFICIARIO", SqlDbType.VarChar, 100){Value = (object)request.APELLIDOS_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@IDENTIFICACION_BENEFICIARIO", SqlDbType.VarChar, 30){Value = (object)request.IDENTIFICACION_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@SECUENCIA", SqlDbType.Int){Value = (object)request.SECUENCIA??DBNull.Value},
                    new SqlParameter("@FECHA_ENVIO", SqlDbType.SmallDateTime){Value = (object)request.FECHA_ENVIO??DBNull.Value},
                    new SqlParameter("@REFERENCIA_PAGO", SqlDbType.VarChar, 50){Value = (object)request.REFERENCIA_PAGO??DBNull.Value},
                    new SqlParameter("@FECHA_PAGO", SqlDbType.SmallDateTime){Value = (object)request.FECHA_PAGO??DBNull.Value},
                    new SqlParameter("@REFERENCIA_ANULACION", SqlDbType.VarChar, 50){Value = (object)request.REFERENCIA_ANULACION??DBNull.Value},
                    new SqlParameter("@FECHA_ANULACION", SqlDbType.SmallDateTime){Value = (object)request.FECHA_ANULACION??DBNull.Value},
                    new SqlParameter("@REFERENCIA_ORDEN", SqlDbType.VarChar, 50){Value = (object)request.REFERENCIA_ORDEN??DBNull.Value},
                    new SqlParameter("@BANCO_NACIONAL", SqlDbType.VarChar, 3){Value = (object)request.BANCO_NACIONAL??DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA", SqlDbType.VarChar, 50){Value = (object)request.NUMERO_CUENTA??DBNull.Value},
                    new SqlParameter("@EMAIL_CLIENTE", SqlDbType.VarChar, 50){Value = (object)request.EMAIL_CLIENTE??DBNull.Value},
                    new SqlParameter("@EMAIL_BENEFICIARIO", SqlDbType.VarChar, 50){Value = (object)request.EMAIL_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@OBSERVACIONES", SqlDbType.Text){Value = (object)request.OBSERVACIONES??DBNull.Value},
                    new SqlParameter("@BANCO_DESTINO", SqlDbType.VarChar, 150){Value = (object)request.BANCO_DESTINO??DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA_DESTINO", SqlDbType.VarChar, 50){Value = (object)request.NUMERO_CUENTA_DESTINO??DBNull.Value},
                    new SqlParameter("@DIRECCION_BANCO", SqlDbType.VarChar){Value = (object)request.DIRECCION_BANCO??DBNull.Value},
                    new SqlParameter("@ABA", SqlDbType.VarChar, 50){Value = (object)request.ABA??DBNull.Value},
                    new SqlParameter("@SWIFT", SqlDbType.VarChar, 50){Value = (object)request.SWIFT??DBNull.Value},
                    new SqlParameter("@IBAN", SqlDbType.VarChar, 50){Value = (object)request.IBAN??DBNull.Value},
                    new SqlParameter("@TELEFONO_BENEFICIARIO", SqlDbType.VarChar, 50){Value = (object)request.TELEFONO_BENEFICIARIO??DBNull.Value},
                    new SqlParameter("@TELEFONO_CLIENTE", SqlDbType.VarChar, 50){Value = (object)request.TELEFONO_CLIENTE??DBNull.Value},
                    new SqlParameter("@BANCO_INTERMEDIARIO", SqlDbType.VarChar, 150){Value = (object)request.BANCO_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@NUMERO_CUENTA_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.NUMERO_CUENTA_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@DIRECCION_BANCO_INTERMEDIARIO", SqlDbType.VarChar){Value = (object)request.DIRECCION_BANCO_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@ABA_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.ABA_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@SWIFT_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.SWIFT_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@IBAN_INTERMEDIARIO", SqlDbType.VarChar, 50){Value = (object)request.IBAN_INTERMEDIARIO??DBNull.Value},
                    new SqlParameter("@USUARIO_TAQUILLA", SqlDbType.VarChar, 15){Value = (object)request.USUARIO_TAQUILLA??DBNull.Value},
                    new SqlParameter("@AnulAutorizadaPor", SqlDbType.VarChar, 15){Value = (object)request.AnulAutorizadaPor??DBNull.Value},
                    new SqlParameter("@AnulProcesada", SqlDbType.Bit){Value = request.AnulProcesada},
                    new SqlParameter("@ReferenciaAnulBcv", SqlDbType.VarChar, 50){Value = (object)request.ReferenciaAnulBcv??DBNull.Value},
                    new SqlParameter("@AnuladaPor", SqlDbType.VarChar, 15){Value = (object)request.AnuladaPor??DBNull.Value},
                    new SqlParameter("@CONCILIADO", SqlDbType.Bit){Value = (object)request.CONCILIADO??DBNull.Value},
                    new SqlParameter("@FECHA_CONCILIACION", SqlDbType.SmallDateTime){Value = (object)request.FECHA_CONCILIACION??DBNull.Value},
                    new SqlParameter("@CONCILIADO_POR", SqlDbType.VarChar, 15){Value = (object)request.CONCILIADO_POR??DBNull.Value},
                    new SqlParameter("@OBSERVACIONES_CONCILIACION", SqlDbType.VarChar, 1000){Value = (object)request.OBSERVACIONES_CONCILIACION??DBNull.Value},
                    new SqlParameter("@TasaConversion", SqlDbType.Money){Value = (object)request.TasaConversion??DBNull.Value},
                    new SqlParameter("@MonedaOperacion", SqlDbType.Int){Value = (object)request.MonedaOperacion??DBNull.Value},
                    new SqlParameter("@MontoConversion", SqlDbType.Money){Value = (object)request.MontoConversion??DBNull.Value},
                    new SqlParameter("@BancoPagoTransferencia", SqlDbType.VarChar, 200){Value = (object)request.BancoPagoTransferencia??DBNull.Value},
                    new SqlParameter("@NumeroCuentaPagoTransferencia", SqlDbType.VarChar, 20){Value = (object)request.NumeroCuentaPagoTransferencia??DBNull.Value},
                    new SqlParameter("@TransferenciaPagada", SqlDbType.Bit){Value = (object)request.TransferenciaPagada??DBNull.Value},
                    new SqlParameter("@FechaPagoTransferencia", SqlDbType.SmallDateTime){Value = (object)request.FechaPagoTransferencia??DBNull.Value},
                    new SqlParameter("@UsuarioPagoTransferencia", SqlDbType.VarChar, 15){Value = (object)request.UsuarioPagoTransferencia??DBNull.Value},
                    new SqlParameter("@ProcesarTransferencia", SqlDbType.Bit){Value = (object)request.ProcesarTransferencia??DBNull.Value},
                };
                return dal.InsertSolicitudes(param);
            }
        }

        public GenericResponse UpdateStatusSolicitudes(Solicitudes model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_SOLICITUD", SqlDbType.Int){Value = model.ID_SOLICITUD},
                    new SqlParameter("@STATUS_SOLICITUD", SqlDbType.Int){Value = model.STATUS_SOLICITUD},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor},
                    new SqlParameter("@ObservacionesAprobacion", SqlDbType.VarChar, 500){Value = (object)model.ObservacionesAprobacion??DBNull.Value},
                    new SqlParameter("@ObservacionesAnulacion", SqlDbType.VarChar, 500){Value = (object)model.ObservacionesAnulacion??DBNull.Value},
                    new SqlParameter("@ObservacionesConciliacion", SqlDbType.VarChar, 500){Value = (object)model.OBSERVACIONES_CONCILIACION??DBNull.Value},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = (object)model.SucursalProcesaId??DBNull.Value},
                };
                return dal.UpdateStatusSolicitudes(param);
            }
        }

        public GenericResponse UpdateMontoAprobadoSolicitudes(Solicitudes model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_SOLICITUD", SqlDbType.Int){Value = model.ID_SOLICITUD},
                    new SqlParameter("@MontoAprobado", SqlDbType.Decimal){Value = model.MontoAprobado},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor},
                };
                return dal.UpdateMontoAprobadoSolicitudes(param);
            }
        }

        public GenericResponse ValidateRequestAmount(ValidateRequestAmount request)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ClientId", SqlDbType.VarChar, 14){Value = request.ClientId},
                    new SqlParameter("@IdentificationType", SqlDbType.Char, 1){Value = request.IdentificationType},
                    new SqlParameter("@Ammount", SqlDbType.Decimal){Value = request.Ammount},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = request.CurrencyId},
                    new SqlParameter("@OperationTypeId", SqlDbType.Int){Value = request.OperationTypeId},
                };
                return dal.ValidateRequestAmount(param);
            }
        }
        public GenericResponse UpdateOrdenReferenciaBCV(Solicitudes request)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@idSolicitud", SqlDbType.Int){Value = request.ID_SOLICITUD},
                    new SqlParameter("@referencia", SqlDbType.VarChar, 100){Value = request.REFERENCIA_ORDEN},
                    new SqlParameter("@TIPO_OP_BCV", SqlDbType.VarChar, 10){Value = request.TIPO_OP_BCV},
                };
                return dal.UpdateOrdenReferenciaBCV(param);
            }
        }

        #endregion

        #region Solicitudes Backoffice
        public HashSet<SolicitudesBackoffice> SearchSolicitudesBackoffice(SolicitudesBackofficeRequest request)
        {
            using (IAnguloLopezDal<SolicitudesBackoffice> dal = new AnguloLopezDal<SolicitudesBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@numero", SqlDbType.Int){Value = request.numero?? (object)DBNull.Value},
                    new SqlParameter("@sucursal", SqlDbType.VarChar, 3){Value = request.sucursal?? (object)DBNull.Value},
                    new SqlParameter("@fechaInicio", SqlDbType.SmallDateTime){Value = request.fechaInicio?? (object)DBNull.Value},
                    new SqlParameter("@fechaFin", SqlDbType.SmallDateTime){Value = request.fechaFin?? (object)DBNull.Value},
                    new SqlParameter("@key", SqlDbType.VarChar, 50){Value = request.tipoOperacion?? (object)DBNull.Value},
                    new SqlParameter("@status", SqlDbType.Int){Value = request.status?? (object)DBNull.Value},
                    new SqlParameter("@id", SqlDbType.Int){Value = request.id?? (object)DBNull.Value},
                    new SqlParameter("@idMoneda", SqlDbType.Int){Value = request.idMoneda?? (object)DBNull.Value},
                    new SqlParameter("@idMonedaOperacion", SqlDbType.Int){Value = request.idMonedaOperacion?? (object)DBNull.Value},
                    new SqlParameter("@statusIDs", SqlDbType.VarChar, -1){Value = request.StatusRange?? (object)DBNull.Value},
                };
                var result = dal.SearchSolicitudesBackoffice(param);
                return result;
            }

        }
        public GenericResponse UpdateSolicitudBackofficeEnProceso(SolicitudUpdateEnProceso request)
        {
            object responseData;
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = request.id},
                    new SqlParameter("@usuario", SqlDbType.VarChar, 15){Value = request.usuario?? (object)DBNull.Value},
                };
                responseData = dal.UpdateSolicitudBackofficeEnProceso(param);
            }
            return (GenericResponse)responseData;
        }
        public GenericResponse UpdateSolicitudBackofficeAprobada(SolicitudUpdateAprobada request)
        {
            GenericResponse responseData;
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = request.id},
                    new SqlParameter("@usuario", SqlDbType.VarChar, 15){Value = request.usuario?? (object)DBNull.Value},
                    new SqlParameter("@observaciones", SqlDbType.VarChar, 500){Value = request.observaciones?? (object)DBNull.Value},
                    new SqlParameter("@monto", SqlDbType.Money){Value = request.monto},
                };
                responseData = (GenericResponse)dal.UpdateSolicitudBackofficeAprobada(param);
            }
            return responseData;
        }
        public GenericResponse UpdateSolicitudBackofficeRechazada(SolicitudUpdateRechazada request)
        {
            object responseData;
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = request.id},
                    new SqlParameter("@usuario", SqlDbType.VarChar, 15){Value = request.usuario?? (object)DBNull.Value},
                    new SqlParameter("@observaciones", SqlDbType.VarChar, 500){Value = request.observaciones?? (object)DBNull.Value},
                };
                responseData = dal.UpdateSolicitudBackofficeRechazada(param);
            }
            return (GenericResponse)responseData;
        }
        public GenericResponse UpdateSolicitudBackofficeConciliada(SolicitudUpdateConciliada request)
        {
            object responseData;
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = request.id},
                    new SqlParameter("@usuario", SqlDbType.VarChar, 15){Value = request.usuario?? (object)DBNull.Value},
                    new SqlParameter("@observaciones", SqlDbType.VarChar, 500){Value = request.observaciones?? (object)DBNull.Value},
                };
                responseData = dal.UpdateSolicitudBackofficeConciliada(param);
            }
            return (GenericResponse)responseData;
        }

        #endregion

        #region Tarifas_Aplicadas
        public GenericResponse InsertTarifasAplicadas(Tarifas_Aplicadas request)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.Char, 15){Value = request.REGISTRADOPOR},
                    new SqlParameter("@ORDEN", SqlDbType.Int){Value = (object)request.ORDEN??DBNull.Value},
                    new SqlParameter("@TARIFA", SqlDbType.Int){Value = request.TARIFA},
                    new SqlParameter("@MONTO", SqlDbType.Money){Value = request.MONTO},
                    new SqlParameter("@SOLICITUD", SqlDbType.Int){Value = (object)request.SOLICITUD??DBNull.Value}
                };
                return dal.InsertTarifasAplicadas(param);
            }
        }
        public HashSet<ListadoTarifasAplicadasBackoffice> SearchTarifasAplicadasSolicitud(int id)
        {
            using (IAnguloLopezDal<ListadoTarifasAplicadasBackoffice> dal = new AnguloLopezDal<ListadoTarifasAplicadasBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID", SqlDbType.Int){Value = id },
                };
                return dal.SearchTarifasAplicadasSolicitud(param);
            }

        }
        public HashSet<ListadoTarifasAplicadasBackoffice> SearchTarifasAplicadasRemesa(int id)
        {
            using (IAnguloLopezDal<ListadoTarifasAplicadasBackoffice> dal = new AnguloLopezDal<ListadoTarifasAplicadasBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_REMESA", SqlDbType.Int){Value = id },
                };
                return dal.SearchTarifasAplicadasRemesa(param);
            }

        }
        #endregion
        #region Remesas Salientes
        public UpdateOrdenesSalientesTransmitidasResponse UpdateOrdenesSalientesTransmitidas(UpdateOrdenesSalientesTransmitidasRequest req)
        {
            using (IAnguloLopezDal<UpdateOrdenesSalientesTransmitidasResponse> dal = new AnguloLopezDal<UpdateOrdenesSalientesTransmitidasResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = (object)req.id??DBNull.Value, IsNullable = true},
                };
                return dal.UpdateOrdenesSalientesTransmitidas(param);
            }
        }
        public HashSet<OrdenSalienteExterno> SearchRemesasSalientes(string corresponsal)
        {
            using (IAnguloLopezDal<OrdenSalienteExterno> dal = new AnguloLopezDal<OrdenSalienteExterno>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@corresponsal", SqlDbType.VarChar, 3){Value = (object)corresponsal??DBNull.Value, IsNullable = true},
                };
                return dal.SearchRemesasSalientes(param);
            }
        }
        #endregion
        #region Remesas Entrantes
        public HashSet<OrdenEntranteBackoffice> SearchRemesasEntrantes(OrdenEntranteRequest request)
        {
            using (IAnguloLopezDal<OrdenEntranteBackoffice> dal = new AnguloLopezDal<OrdenEntranteBackoffice>())
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
                    new SqlParameter("@secuencia", SqlDbType.BigInt){Value = (object)request.secuencia??DBNull.Value},
                    new SqlParameter("@GrupoId", SqlDbType.Int){Value = (object)request.GrupoId??DBNull.Value},
                    new SqlParameter("@Modo", SqlDbType.VarChar, 1){Value = (object)request.Modo??DBNull.Value},
                    new SqlParameter("@BankId", SqlDbType.Int){Value = (object)request.BankId??DBNull.Value},
                    new SqlParameter("@CurrencyId", SqlDbType.Int){Value = (object)request.CurrencyId??DBNull.Value},
                    new SqlParameter("@Pending", SqlDbType.Bit){Value = (object)request.Pending??DBNull.Value},
                    new SqlParameter("@StatusGroupId", SqlDbType.Int){Value = (object)request.StatusGroupId??DBNull.Value},
                };
                return dal.SearchRemesasEntrantes(param);
            }
        }
        public GenericResponse UpdateRemesaEntrante(REMESAS_ENTRANTES model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@idRemesa", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@status", SqlDbType.Int){Value = model.STATUS},
                    new SqlParameter("@modo", SqlDbType.VarChar, 1){Value = model.MODO},
                    new SqlParameter("@bancoPago", SqlDbType.VarChar, 50){Value = (object)model.BancoPago??DBNull.Value},
                    new SqlParameter("@numeroCuentaPago", SqlDbType.VarChar, 50){Value = (object)model.NumeroCuentaPago??DBNull.Value},
                };
                return dal.UpdateRemesaEntrante(param);
            }
        }
        public GenericResponse UpdateRechazarRemesaEntrante(REMESAS_ENTRANTES model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@observaciones", SqlDbType.VarChar, 256){Value = model.ObservacionAnulacion},
                    new SqlParameter("@rechazadoPor", SqlDbType.VarChar, 15){Value = model.UsuarioAnula},
                };
                return dal.UpdateRechazarRemesaEntrante(param);
            }
        }
        public GenericResponse UpdateAprobarRemesaEntrante(REMESAS_ENTRANTES model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@observaciones", SqlDbType.VarChar, 256){Value = model.OBSERVACIONES},
                    new SqlParameter("@aprobadoPor", SqlDbType.VarChar, 15){Value = model.CoordinadorAutoriza},
                };
                return dal.UpdateAprobarRemesaEntrante(param);
            }
        }
        public GenericResponse InsertarRemesaEntrante(REMESAS_ENTRANTES model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
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
                    new SqlParameter("@EMAIL", SqlDbType.VarChar, 150){Value = model.email}
                };
                return dal.InsertarRemesaEntrante(param);
            }
        }
        public GenericResponse AnulaRemesaEntranteCorresponsal(REMESAS_ENTRANTES model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@observacion", SqlDbType.VarChar, 150){Value = model.ObservacionAnulacion},
                    new SqlParameter("@usuarioAnula", SqlDbType.VarChar, 15){Value = model.UsuarioAnula},
                    new SqlParameter("@motivoAnulacion", SqlDbType.Int){Value = model.MotivoAnulacionId },
                    new SqlParameter("@usuarioAutoriza", SqlDbType.VarChar, 15){Value = (object)model.CoordinadorAutoriza??DBNull.Value},
                };
                return dal.AnulaRemesaEntranteCorresponsal(param);
            }
        }

        public GenericResponse UpdateStatusRemesasEntrantes(REMESAS_ENTRANTES model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {

                SqlParameter[] param =
                {
                    new SqlParameter("@ID_OPERACION", SqlDbType.Int){Value = model.ID_OPERACION},
                    new SqlParameter("@STATUS", SqlDbType.Int){Value = model.STATUS},
                    new SqlParameter("@ModificadoPor", SqlDbType.VarChar, 15){Value = model.ModificadoPor},
                    new SqlParameter("@SucursalProcesaId", SqlDbType.Int){Value = (object)model.SucursalProcesaId??DBNull.Value},
                    new SqlParameter("@ID_ORDEN", SqlDbType.Int){Value = (object)model.ID_ORDEN??DBNull.Value},
                    new SqlParameter("@Observation", SqlDbType.VarChar, 150){Value = (object)model.ObservacionAnulacion??DBNull.Value},
                    new SqlParameter("@REFERENCIA_BCV", SqlDbType.VarChar, 50){Value = (object)model.REFERENCIA_BCV??DBNull.Value},
                    new SqlParameter("@MotivoAnulacionId", SqlDbType.Int){Value = (object)model.MotivoAnulacionId??DBNull.Value}


                };
                return dal.UpdateStatusRemesasEntrantes(param);
            }
        }
        #endregion
        #endregion

        #region Schema Pagos
        #region ConversionCurrency
        public HashSet<Nomeda> SearchConversionCurrency(MonedaRequest request)
        {
            using (IAnguloLopezDal<Nomeda> dal = new AnguloLopezDal<Nomeda>())
            {
                SqlParameter[] param =
               {
                    new SqlParameter("@ExcludeCurrency", SqlDbType.Int){Value = request.ExcludeCurrency?? (object)DBNull.Value},
                    new SqlParameter("@Multicurrency", SqlDbType.Bit){Value = request.Multicurrency},
                };
                return dal.SearchConversionCurrency(param);
            }
        }
        #endregion
        #endregion

        #region Schema Ingresos
        #region Pagos_Recibidos_Cliente
        public GenericResponse InsertPagos_Recibidos_Cliente(Pagos_Recibidos_Cliente model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.Char, 15){Value = model.REGISTRADOPOR},
                    new SqlParameter("@MONTO_TOTAL", SqlDbType.Money){Value = model.MONTO_TOTAL},
                    new SqlParameter("@FICHA", SqlDbType.Int){Value = (object)model.FICHA??DBNull.Value},
                    new SqlParameter("@NUMEROID", SqlDbType.VarChar, 50){Value = (object)model.NUMEROID??DBNull.Value},
                    new SqlParameter("@TIPO_IDENTIDAD", SqlDbType.Char, 1){Value = (object)model.TIPO_IDENTIDAD??DBNull.Value},
                    new SqlParameter("@SUCURSAL", SqlDbType.Char, 3){Value = model.SUCURSAL},
                    new SqlParameter("@TIPO_CANJE", SqlDbType.Int){Value = (object)model.TIPO_CANJE??DBNull.Value}
                };
                var resultInsert = dal.InsertPagos_Recibidos_Cliente(param);
                if (resultInsert.Error)
                    return resultInsert;

                var rollback = new RollBackPayment
                {
                    PaymentId = resultInsert.ID_OPERACION,
                    RequestId = model.MixedOperation ? model.ListOrdenesRelacionadasPago.FirstOrDefault().SolicitudId : null
                };
                if (model.ListDetallePago != null)
                {
                    foreach (var item in model.ListDetallePago)
                    {
                        item.REGISTRADOPOR = model.REGISTRADOPOR;
                        item.PAGO_CLIENTE = resultInsert.ID_OPERACION;
                        item.STATUS_PAGO = 1;
                        var resultDetalle = InsertDetalle_Pago_Recibido(item);
                        if (resultDetalle.Error)
                        {
                            var resultRollback = RollBackInsertPagos_Recibidos_Cliente(rollback);
                            if (resultRollback.Error)
                                return resultRollback;
                            return resultDetalle;
                        }
                    }
                }

                if (model.ListOrdenesRelacionadasPago != null)
                {
                    foreach (var item in model.ListOrdenesRelacionadasPago)
                    {
                        item.REGISTRADOPOR = model.REGISTRADOPOR;
                        item.PAGO_CLIENTE = resultInsert.ID_OPERACION;
                        var resultRelacion = InsertOrdenes_Relacionadas_Pagos(item);
                        if (resultRelacion.Error)
                        {
                            var resultRollback = RollBackInsertPagos_Recibidos_Cliente(rollback);
                            if (resultRollback.Error)
                                return resultRollback;
                            return resultRelacion;
                        }

                        var objSolicitud = new Solicitudes
                        {
                            ID_SOLICITUD = item.SolicitudId ?? 0,
                            ModificadoPor = model.REGISTRADOPOR,
                            STATUS_SOLICITUD = 6
                        };
                        var resulUpdate = UpdateStatusSolicitudes(objSolicitud);
                        if (resulUpdate.Error)
                        {
                            var resultRollback = RollBackInsertPagos_Recibidos_Cliente(rollback);
                            if (resultRollback.Error)
                                return resultRollback;
                            return resulUpdate;
                        }

                    }
                }
                return resultInsert;
            }
        }

        public GenericResponse RollBackInsertPagos_Recibidos_Cliente(RollBackPayment rollBack)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@ID_PAGO", SqlDbType.Int){Value = rollBack.PaymentId},
                    new SqlParameter("@ID_SOLICITUD", SqlDbType.Int){Value = (object)rollBack.RequestId??DBNull.Value},

                };
                return dal.RollBackInsertPagos_Recibidos_Cliente(param);
            }
        }
        #endregion

        #region Detalle_Pago_Recibido
        public GenericResponse InsertDetalle_Pago_Recibido(Detalle_Pago_Recibido model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@PAGO_CLIENTE", SqlDbType.Int){Value = model.PAGO_CLIENTE},
                    new SqlParameter("@TIPO_PAGO_RECIBIDO", SqlDbType.Int){Value = model.TIPO_PAGO_RECIBIDO},
                    new SqlParameter("@DETALLE_TIPO_PAGO", SqlDbType.Int){Value = model.DETALLE_TIPO_PAGO},
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.Char, 15){Value = model.REGISTRADOPOR},
                    new SqlParameter("@MONTO", SqlDbType.Money){Value = model.MONTO},
                    new SqlParameter("@REFERENCIA_1", SqlDbType.VarChar, 100){Value = (object)model.REFERENCIA_1??DBNull.Value},
                    new SqlParameter("@REFERENCIA_2", SqlDbType.VarChar, 100){Value = (object)model.REFERENCIA_2??DBNull.Value},
                    new SqlParameter("@REFERENCIA_3", SqlDbType.VarChar, 100){Value = (object)model.REFERENCIA_3??DBNull.Value},
                    new SqlParameter("@FECHA_TRANSACCION", SqlDbType.SmallDateTime){Value = model.FECHA_TRANSACCION == null ? (object)DBNull.Value: (model.FECHA_TRANSACCION??DateTime.Now).ToString("yyyy-MM-dd")},
                    new SqlParameter("@PaymentSupportPath", SqlDbType.VarChar, 200){Value = (object)model.PaymentSupportPath??DBNull.Value},
                    new SqlParameter("@OriginBank", SqlDbType.VarChar, 50){Value = (object)model.OriginBank??DBNull.Value},
                    new SqlParameter("@OriginBankAccount", SqlDbType.VarChar, 50){Value = (object)model.OriginBankAccount??DBNull.Value},
                };
                return dal.InsertDetalle_Pago_Recibido(param);
            }
        }
        #endregion

        #region Ordenes_Relacionadas_Pagos
        public GenericResponse InsertOrdenes_Relacionadas_Pagos(Ordenes_Relacionadas_Pagos model)
        {
            using (IAnguloLopezDal<GenericResponse> dal = new AnguloLopezDal<GenericResponse>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@PAGO_CLIENTE", SqlDbType.Int){Value = model.PAGO_CLIENTE},
                    new SqlParameter("@REGISTRADOPOR", SqlDbType.Char, 15){Value = model.REGISTRADOPOR},
                    new SqlParameter("@NUMERO", SqlDbType.VarChar, 15){Value = model.NUMERO},
                    new SqlParameter("@ID_GNC", SqlDbType.Int){Value = (object)model.ID_GNC??DBNull.Value},
                    new SqlParameter("@ID_GIR", SqlDbType.Int){Value = (object)model.ID_GIR??DBNull.Value},
                    new SqlParameter("@ID_FAC", SqlDbType.Int){Value = (object)model.ID_FAC??DBNull.Value},
                    new SqlParameter("@ID_GPR", SqlDbType.Int){Value = (object)model.ID_GPR??DBNull.Value},
                    new SqlParameter("@SolicitudId", SqlDbType.Int){Value = (object)model.SolicitudId??DBNull.Value}
                };
                return dal.InsertOrdenes_Relacionadas_Pagos(param);
            }
        }
        #endregion

        #endregion

        public HashSet<PagoRecibidosBackoffice> SearchPagosRecibidosSolicitud(int id)
        {
            using (IAnguloLopezDal<PagoRecibidosBackoffice> dal = new AnguloLopezDal<PagoRecibidosBackoffice>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@idSolicitud", SqlDbType.Int){Value = id},
                };
                return dal.SearchPagosRecibidosSolicitud(param);
            }
        }


        #region Schema Ciudades
        #region Ciudades
        public HashSet<Ciudad> SearchCity(CiudadRequest ciudad)
        {
            using (IAnguloLopezDal<Ciudad> dal = new AnguloLopezDal<Ciudad>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@CountryId", SqlDbType.VarChar, 3){Value = (object)ciudad.CountryId??DBNull.Value},
                    new SqlParameter("@ForShipping", SqlDbType.Bit){Value = ciudad.ForShipping},
                };
                return dal.SearchCity(param);
            }
        }
        public HashSet<Ciudad> SearchCityRemitee(CiudadRequest ciudad)
        {
            using (IAnguloLopezDal<Ciudad> dal = new AnguloLopezDal<Ciudad>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.Int){Value = (object)ciudad.id??DBNull.Value}
                };
                return dal.SearchCityRemitee(param);
            }
        }
        #endregion
        #endregion
        public HashSet<MotivosAnulacionCorresponsal> SearchMotivosAnulacion(MotivosAnulacionCorresponsal req)
        {
            using (IAnguloLopezDal<MotivosAnulacionCorresponsal> dal = new AnguloLopezDal<MotivosAnulacionCorresponsal>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@corresponsal", SqlDbType.Char, 3){Value = (object)req.corresponsal??DBNull.Value},
                };
                return dal.SearchMotivosAnulacion(param);
            }

        }

        #region Schema Monedas
        #region Monedas
        public HashSet<Moneda> SearchMoneda()
        {
            using (IAnguloLopezDal<Moneda> dal = new AnguloLopezDal<Moneda>())
            {
                return dal.SearchMoneda();
            }
        }

        #endregion
        #endregion

        #region Schema Corresponsal
        public HashSet<CorresponsalCredenciales> SearchCorresponsal(CorresponsalRequest req)
        {
            using (IAnguloLopezDal<CorresponsalCredenciales> dal = new AnguloLopezDal<CorresponsalCredenciales>())
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@id", SqlDbType.VarChar, 3){Value = (object)req.codigo??DBNull.Value},
                    new SqlParameter("@novalida", SqlDbType.Bit){Value = (object)req.validaTransmision??DBNull.Value},
                };
                return dal.SearchCorresponsal(param);
            }

        }
        #endregion

        public HashSet<MonedasCorresponsal> SearchMonedasPagador(string pagador)
        {
            using (IAnguloLopezDal<MonedasCorresponsal> dal = new AnguloLopezDal<MonedasCorresponsal>())
            {
                SqlParameter[] param =
                  {
                    new SqlParameter("@pagador", SqlDbType.Char, 3){Value = (object)pagador??DBNull.Value},
                };
                return dal.SearchMonedasPagador(param);
            }
        }
        public HashSet<RemiteeIntegracion> RemiteeDatosIntegracion(RemiteeIntegracion request)
        {
            using (IAnguloLopezDal<RemiteeIntegracion> dal = new AnguloLopezDal<RemiteeIntegracion>())
            {
                SqlParameter[] param =
                  {
                    new SqlParameter("@OrdenId", SqlDbType.Int){Value = (object)request.OrdenId??DBNull.Value},
                    new SqlParameter("@TemporalId", SqlDbType.Int){Value = (object)request.TemporalId??DBNull.Value},
                    new SqlParameter("@TipoIntegracionRemiteeId", SqlDbType.Int){Value = (object)request.TipoIntegracionRemiteeId},
                    new SqlParameter("@DataJson", SqlDbType.VarChar, 8000){Value = (object)request.DataJson},
                    new SqlParameter("@CreationUser", SqlDbType.VarChar, 15){Value = (object)request.CreationUser},
                };
                return dal.RemiteeDatosIntegracion(param);
            }
        }

        public HashSet<RemiteeIntegracion> UpdateRemiteeDatosIntegracion(RemiteeIntegracion request)
        {
            using (IAnguloLopezDal<RemiteeIntegracion> dal = new AnguloLopezDal<RemiteeIntegracion>())
            {
                SqlParameter[] param =
                  {
                    new SqlParameter("@OrdenId", SqlDbType.Int){Value = (object)request.OrdenId??DBNull.Value},
                    new SqlParameter("@TemporalId", SqlDbType.Int){Value = (object)request.TemporalId??DBNull.Value},
                };
                return dal.UpdateRemiteeDatosIntegracion(param);
            }
        }

        public HashSet<RemiteeIntegracion> SearchRemiteeDatosIntegracion(RemiteeIntegracion request)
        {
            using (IAnguloLopezDal<RemiteeIntegracion> dal = new AnguloLopezDal<RemiteeIntegracion>())
            {
                SqlParameter[] param =
                  {
                    new SqlParameter("@OrdenId", SqlDbType.Int){Value = (object)request.OrdenId??DBNull.Value},
                    new SqlParameter("@TemporalId", SqlDbType.Int){Value = (object)request.TemporalId??DBNull.Value},
                    new SqlParameter("@TipoIntegracionRemiteeId", SqlDbType.Int){Value = (object)request.TipoIntegracionRemiteeId??DBNull.Value},
                };
                return dal.SearchRemiteeDatosIntegracion(param);
            }
        }
    }
}
