using Common.Models.Common;
using DataAccess.Conection;
using IDataAccess.Angulo_Lopez;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace DataAccess.Angulo_Lopez
{
    public class AnguloLopezDal<TEntity> : Finalize, IAnguloLopezDal<TEntity> where TEntity : new()
    {
        #region Variables
        private readonly AnguloLopezDbContextt<TEntity> _DbContext = new AnguloLopezDbContextt<TEntity>();
        #endregion

        #region Schema Operaciones
        #region Solicitudes
        public HashSet<TEntity> SearchSolicitudes(object[] param)
        {
            //try
            //{
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchSolicitudes @CLIENTE, @SUCURSAL, @DETALLE_TIPO_OPERACION, @STATUS_SOLICITUD, @SolicitudIds", param));
            //return (TEntity)(object)new GenericResponse
            //{
            //    Data = (object)new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchSolicitudes @CLIENTE, @SUCURSAL, @DETALLE_TIPO_OPERACION, @STATUS_SOLICITUD", param))
            //};
            //}
            //catch (Exception ex)
            //{
            //    var error = "";
            //    //return (TEntity)(object)new GenericResponse
            //    //{
            //    //    Error = true,
            //    //    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar consultar la solicitud: ",
            //    //    ex.Message, ". Por favor notificar al administrador del sistema.")
            //    //};
            //}
        }

        public TEntity InsertSolicitudes(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>(string.Concat("[operaciones].[InsertSolicitudes] ",
                    "@DETALLE_TIPO_OPERACION, ",
                    "@CLIENTE,",
                    "@SUCURSAL,",
                    "@STATUS_SOLICITUD,",
                    "@MONEDA,",
                    "@OFICINA,",
                    "@PERSONA,",
                    "@PAIS_DESTINO,",
                    "@CORRESPONSAL,",
                    "@AGENTE,",
                    "@REGISTRADOPOR,",
                    "@NUMERO,",
                    "@TIPO_CAMBIO,",
                    "@MONTO,",
                    "@TASA_DESTINO,",
                    "@MONTO_CAMBIO,",
                    "@FECHA_VALOR_TASA,",
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
                    "@REFERENCIA_ANULACION,",
                    "@FECHA_ANULACION,",
                    "@REFERENCIA_ORDEN,",
                    "@BANCO_NACIONAL,",
                    "@NUMERO_CUENTA,",
                    "@EMAIL_CLIENTE,",
                    "@EMAIL_BENEFICIARIO,",
                    "@OBSERVACIONES,",
                    "@BANCO_DESTINO,",
                    "@NUMERO_CUENTA_DESTINO,",
                    "@DIRECCION_BANCO,",
                    "@ABA,",
                    "@SWIFT,",
                    "@IBAN,",
                    "@TELEFONO_BENEFICIARIO,",
                    "@TELEFONO_CLIENTE,",
                    "@BANCO_INTERMEDIARIO,",
                    "@NUMERO_CUENTA_INTERMEDIARIO,",
                    "@DIRECCION_BANCO_INTERMEDIARIO,",
                    "@ABA_INTERMEDIARIO,",
                    "@SWIFT_INTERMEDIARIO,",
                    "@IBAN_INTERMEDIARIO,",
                    "@USUARIO_TAQUILLA,",
                    "@AnulAutorizadaPor,",
                    "@AnulProcesada,",
                    "@ReferenciaAnulBcv,",
                    "@AnuladaPor,",
                    "@CONCILIADO,",
                    "@FECHA_CONCILIACION,",
                    "@CONCILIADO_POR,",
                    "@OBSERVACIONES_CONCILIACION,",
                    "@TasaConversion,",
                    "@MonedaOperacion,",
                    "@MontoConversion,",
                    "@BancoPagoTransferencia,",
                    "@NumeroCuentaPagoTransferencia,",
                    "@TransferenciaPagada,",
                    "@FechaPagoTransferencia,",
                    "@UsuarioPagoTransferencia,",
                    "@ProcesarTransferencia,",
                    "@CommissionUsd,",
                    "@CommissionBs, ",
                    "@ExchangeDifferential, ",
                    "@TypeAccountBank"), param).First();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar registrar la solicitud: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }

        public TEntity UpdateSolicitudes(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.UpdateSolicitudes ",
                    "@ID_SOLICITUD, ",
                    "@DETALLE_TIPO_OPERACION, ",
                    "@SUCURSAL,",
                    "@STATUS_SOLICITUD,",
                    "@MONEDA,",
                    "@OFICINA,",
                    "@PERSONA,",
                    "@PAIS_DESTINO,",
                    "@CORRESPONSAL,",
                    "@AGENTE,",
                    "@NUMERO,",
                    "@TIPO_CAMBIO,",
                    "@MONTO,",
                    "@TASA_DESTINO,",
                    "@MONTO_CAMBIO,",
                    "@FECHA_VALOR_TASA,",
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
                    "@REFERENCIA_ANULACION,",
                    "@FECHA_ANULACION,",
                    "@REFERENCIA_ORDEN,",
                    "@BANCO_NACIONAL,",
                    "@NUMERO_CUENTA,",
                    "@EMAIL_CLIENTE,",
                    "@EMAIL_BENEFICIARIO,",
                    "@OBSERVACIONES,",
                    "@BANCO_DESTINO,",
                    "@NUMERO_CUENTA_DESTINO,",
                    "@DIRECCION_BANCO,",
                    "@ABA,",
                    "@SWIFT,",
                    "@IBAN,",
                    "@TELEFONO_BENEFICIARIO,",
                    "@TELEFONO_CLIENTE,",
                    "@BANCO_INTERMEDIARIO,",
                    "@NUMERO_CUENTA_INTERMEDIARIO,",
                    "@DIRECCION_BANCO_INTERMEDIARIO,",
                    "@ABA_INTERMEDIARIO,",
                    "@SWIFT_INTERMEDIARIO,",
                    "@IBAN_INTERMEDIARIO,",
                    "@USUARIO_TAQUILLA,",
                    "@AnulAutorizadaPor,",
                    "@AnulProcesada,",
                    "@ReferenciaAnulBcv,",
                    "@AnuladaPor,",
                    "@CONCILIADO,",
                    "@FECHA_CONCILIACION,",
                    "@CONCILIADO_POR,",
                    "@OBSERVACIONES_CONCILIACION,",
                    "@TasaConversion,",
                    "@MonedaOperacion,",
                    "@MontoConversion,",
                    "@BancoPagoTransferencia,",
                    "@NumeroCuentaPagoTransferencia,",
                    "@TransferenciaPagada,",
                    "@FechaPagoTransferencia,",
                    "@UsuarioPagoTransferencia,",
                    "@ProcesarTransferencia"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la solicitud: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }

        public TEntity UpdateStatusSolicitudes(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.UpdateStatusSolicitudes  ",
                    "@ID_SOLICITUD, ",
                    "@STATUS_SOLICITUD, ",
                    "@ModificadoPor, ",
                    "@ObservacionesAprobacion, ",
                    "@ObservacionesAnulacion, ",
                    "@ObservacionesConciliacion"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar el estatus de la solicitud: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }

        public TEntity UpdateMontoAprobadoSolicitudes(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("operaciones.UpdateMontoAprobadoSolicitudes  ",
                    "@ID_SOLICITUD, ",
                    "@MontoAprobado, ",
                    "@ModificadoPor"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar el monto aprobado de la solicitud: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }

        public TEntity ValidateRequestAmount(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>("operaciones.ValidateRequestAmount @ClientId, @IdentificationType, @Ammount, @CurrencyId, @OperationTypeId", param).First();
            }
            catch (Exception ex)
            {

                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar validar el monto de la solicitud: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }

        }

        public TEntity UpdateOrdenReferenciaBCV(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateOrdenReferenciaBCV @idSolicitud, @referencia, @TIPO_OP_BCV", param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la referencia: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        #endregion

        #region Solicitudes en Backoffice
        public HashSet<TEntity> SearchSolicitudesBackoffice(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchOrderRequestBackoffice @numero, @sucursal, @fechaInicio, @fechaFin, @key, @status, @id, @idMoneda, @idMonedaOperacion, @statusIDs", param));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TEntity UpdateSolicitudBackofficeEnProceso(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateSolicitudProceso @id, @usuario", param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la solicitud: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        public TEntity UpdateSolicitudBackofficeAprobada(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateSolicitudAprobada @id, @usuario, @observaciones, @monto", param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la solicitud: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        public TEntity UpdateSolicitudBackofficeRechazada(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateSolicitudRechazada @id, @usuario, @observaciones", param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la solicitud: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        public TEntity UpdateSolicitudBackofficeConciliada(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateSolicitudConciliada @id, @usuario, @observaciones", param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar la solicitud: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        #endregion

        #region Tarifas_Aplicadas
        public TEntity InsertTarifasAplicadas(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("[operaciones].[spiTarifasAplicadas] ",
                   "@REGISTRADOPOR, ",
                   "@ORDEN,",
                   "@TARIFA,",
                   "@MONTO,",
                   "@SOLICITUD"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar registrar la tarifa: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        public HashSet<TEntity> SearchTarifasAplicadasSolicitud(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[operaciones].[spsTarifasAplicadasSolicitud]  @ID", param));
        }

        public HashSet<TEntity> SearchTarifasAplicadasRemesa(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[operaciones].[spsTarifasAplicadasRemesa]  @ID_REMESA", param));
        }
        #endregion

        #region Remesas Salientes
        public TEntity UpdateOrdenesSalientesTransmitidas(object[] param)
        {
            try
            {
                var r = new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.[UpdateOrdenesSalientesTransmitidas] @id ", param));
                return r.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HashSet<TEntity> SearchRemesasSalientes(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.[spsGetOrdenesTransmitirExternos] @corresponsal ", param));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Remesas Entrantes
        public HashSet<TEntity> SearchRemesasEntrantes(object[] param)
        {
            try
            {
                return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("operaciones.SearchRemesasEntrantes @ficha, @identificacion, " +
                    "@referencia, @corresponsal, @status, @pais, @id, @fechaCreacionInicio, @fechaCreacionFin, @fechaPagoInicio, @fechaPagoFin, @secuencia, @GrupoId, @Modo, @BankId, @CurrencyId,@Pending,@StatusGroupId", param));
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TEntity UpdateRemesaEntrante(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateRemesaEntrante @idRemesa, @status, @modo, @bancoPago, @numeroCuentaPago", param);
                return (TEntity)(object)new GenericResponse();
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
        public TEntity UpdateStatusRemesasEntrantes(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateStatusRemesasEntrantes @ID_OPERACION, @STATUS, @ModificadoPor, @SucursalProcesaId, @ID_ORDEN, @Observation, @REFERENCIA_BCV, @MotivoAnulacionId", param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar actualizar el estatus de la remesa: ",
                                ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        public TEntity UpdateRechazarRemesaEntrante(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateRechazarRemesaEntrante @id, @observaciones, @rechazadoPor", param);
                return (TEntity)(object)new GenericResponse();
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
        public TEntity UpdateAprobarRemesaEntrante(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("operaciones.UpdateAprobarRemesaEntrante @id, @observaciones, @aprobadoPor", param);
                return (TEntity)(object)new GenericResponse();
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
        public TEntity InsertarRemesaEntrante(object[] param)
        {
            try
            {
                var id = _DbContext.Database.SqlQuery<int>(
                    string.Concat("[operaciones].[InsertRemesaEntrante] @MONEDA, @PAIS, @SUCURSAL, @CORRESPONSAL, " +
                    "@REGISTRADO_POR, @PAGOMANUAL, @SECUENCIA, @MODO, @CIREM, @CIDES, @REFERENCIA, @TELREM, @TELDES, @TEL2DES, " +
                    "@NOMREM, @NOMDES, @DIRDES, @USD, @TASA, @BOLI, @COMIUSD, @OTROS, @IVA, @FECHA, @OBSERVACIONES, @MENSAJE, @BANCO, @CUENTA, @TIPO_CUENTA, @EMAIL "), param).First();
                return (TEntity)(object)new GenericResponse { 
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
        public TEntity AnulaRemesaEntranteCorresponsal(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand("[operaciones].[AnularRemesasEntrante] @id, @observacion, @usuarioAnula, @motivoAnulacion, @usuarioAutoriza", param);
                return (TEntity)(object)new GenericResponse();
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

        #region Schema Pagos
        #region ConversionCurrency
        public HashSet<TEntity> SearchConversionCurrency(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[pagos].[SearchConversionCurrency] @ExcludeCurrency, @Multicurrency", param));
        }
        #endregion
        #endregion

        #region Schema Ingresos
        #region Pagos_Recibidos_Cliente
        public TEntity InsertPagos_Recibidos_Cliente(object[] param)
        {
            try
            {
                return _DbContext.Database.SqlQuery<TEntity>(string.Concat("[ingresos].[spiPagoRecibido] ",
                    "@REGISTRADOPOR, ",
                    "@MONTO_TOTAL,",
                    "@FICHA,",
                    "@NUMEROID,",
                    "@TIPO_IDENTIDAD,",
                    "@SUCURSAL,",
                    "@TIPO_CANJE"), param).First();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar registrar el pago: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }

        public TEntity RollBackInsertPagos_Recibidos_Cliente(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("ingresos.RollBackPagos_Recibido_Clientes ",
                    "@ID_PAGO, ",
                    "@ID_SOLICITUD"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar realizar el roolback del pago: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        #endregion

        #region Detalle_Pago_Recibido
        public TEntity InsertDetalle_Pago_Recibido(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("[ingresos].[spiDetallePagoRecibido] ",
                    "@PAGO_CLIENTE, ",
                    "@TIPO_PAGO_RECIBIDO,",
                    "@DETALLE_TIPO_PAGO,",
                    "@REGISTRADOPOR,",
                    "@MONTO,",
                    "@REFERENCIA_1,",
                    "@REFERENCIA_2,",
                    "@REFERENCIA_3,",
                    "@FECHA_TRANSACCION,",
                    "@PaymentSupportPath,",
                    "@OriginBank,",
                    "@OriginBankAccount"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar registrar el detalle de pago: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }


        public HashSet<TEntity> SearchPagosRecibidosSolicitud(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[operaciones].[SearchPagosRelacionadosSolicitud]  @idSolicitud", param));
        }
        #endregion

        #region Ordenes_Relacionadas_Pagos
        public TEntity InsertOrdenes_Relacionadas_Pagos(object[] param)
        {
            try
            {
                _DbContext.Database.ExecuteSqlCommand(string.Concat("[ingresos].[spiFacturasRelacionadas] ",
                    "@PAGO_CLIENTE, ",
                    "@REGISTRADOPOR,",
                    "@NUMERO,",
                    "@ID_GNC,",
                    "@ID_GIR,",
                    "@ID_FAC,",
                    "@ID_GPR,",
                    "@SolicitudId"), param);
                return (TEntity)(object)new GenericResponse();
            }
            catch (Exception ex)
            {
                return (TEntity)(object)new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar registrar la solicitud asociada al pago: ",
                    ex.Message, ". Por favor notificar al administrador del sistema.")
                };
            }
        }
        #endregion
        #endregion

        #region Schema Ciudades
        #region Ciudades
        public HashSet<TEntity> SearchCity(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("ciudades.SearchCity @CountryId, @ForShipping", param));
        }
        public HashSet<TEntity> SearchCityRemitee(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[ciudades].[SearchCityRemitee] @id", param));
        }
        #endregion
        #endregion

        public HashSet<TEntity> SearchMotivosAnulacion(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[corresponsal].[SearchMotivosAnulacion] @corresponsal", param));
        }
        #region Schema Corresponsal
        public HashSet<TEntity> SearchCorresponsal(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[corresponsal].[SearchCorresponsalById] @id, @novalida", param));
        }
        public HashSet<TEntity> SearchMonedasCorresponsal(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[pagadores].[SearchMonedasPagador] @corresponsal", param));
        }

        public HashSet<TEntity> RemiteeDatosIntegracion(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[operaciones].[InsertRemiteeDatosIntegracion] @OrdenId, @TemporalId, @TipoIntegracionRemiteeId, @DataJson, @CreationUser", param));
        }
        public HashSet<TEntity> UpdateRemiteeDatosIntegracion(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[operaciones].[UpdateRemiteeDatosIntegracion] @OrdenId, @TemporalId", param));
        }
        public HashSet<TEntity> SearchRemiteeDatosIntegracion(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[operaciones].[SearchRemiteeDatosIntegracion] @OrdenId, @TemporalId, @TipoIntegracionRemiteeId", param));
        }

        public HashSet<TEntity> SearchMonedasPagador(object[] param)
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("[pagadores].[SearchMonedasPagador] @pagador", param));
        }
        #endregion


        #region Schema Monedas
        #region Monedas
        public HashSet<TEntity> SearchMoneda()
        {
            return new HashSet<TEntity>(_DbContext.Database.SqlQuery<TEntity>("monedas.spsMonedasBackoffice"));
        }
        #endregion
        #endregion
    }
}
