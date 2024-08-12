
using Common.Models.Angulo_Lopez;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.Tarifas;
using IObjectBuilder.Angulo_Lopez;
using Newtonsoft.Json;
using IObjectBuilder.Operaciones;
using ObjectBuilder.Angulo_Lopez;
using ObjectBuilder.Angulo_Lopez.Operaciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Net.Http.Headers;
using ObjectBuilder.Clients;
using Common.Models.Common;
using ObjectBuilder.Angulo_Lopez.Tasas;
using IObjectBuilder.Angulo_Lopez.Tasas;
using Common.Resource;
using IObjectBuilder.Clients;
using Common.Models.Angulo_Lopez.Contabilidad;
using IObjectBuilder.Angulo_Lopez.Passes;
using ObjectBuilder.Angulo_Lopez.Passes;
using IObjectBuilder.Angulo_Lopez.Contabilidad;
using ObjectBuilder.Angulo_Lopez.Contabilidad;
using Common.Models.Angulo_Lopez.Pases;
using Common.Models.Angulo_Lopez.Seguridad;
using IObjectBuilder.Angulo_Lopez.TablasMaestras;
using ObjectBuilder.Angulo_Lopez.TablasMaestras;
using MonedasRequests = Common.Models.Angulo_Lopez.TablasMaestras.MonedasRequest;
using Common.Models.Angulo_Lopez.Oficinas;
using IObjectBuilder.Angulo_Lopez.Oficinas;
using ObjectBuilder.Angulo_Lopez.Oficinas;
using IObjectBuilder.Angulo_Lopez.Simadi;
using ObjectBuilder.Angulo_Lopez.Simadi;
using ObjectBuilder.Angulo_Lopez.Tarifas;
using IObjectBuilder.Angulo_Lopez.Tarifas;

namespace Core.Financial.Api.Utils
{

    public static class Functions
    {
        public static CultureInfo wsCulture { get => new CultureInfo("en-US"); }

        public static string getCiudadCliente(int? c)
        {
            var ret = string.Empty;

            var key = Services.clientes.GetUserSessionTokenId("FURBANO", "");
            var access = Services.clientes.GetDataAccessPassport(key);
            var ciudad = Services.clientes.CiudadesSelectOne((int)c, key);
            if (ciudad.Rows.Count > 0)
            {
                ret = ciudad.Rows[0]["ciudad"].ToString();
            }
            return ret;
        }

        public static Fichas getObtenerNombres(string NombreCompl)
        {
            Fichas Nombres = new Fichas();
            if (NombreCompl != null && NombreCompl != String.Empty)
            {
                Nombres.NombreCompleto = NombreCompl.Replace("  ", " ");
                var NomBenefic = NombreCompl.Replace("  ", " ").Split(' ');
                switch (NomBenefic.Length)
                {
                    case 5:
                        Nombres.PrimerNombre = string.Format("{0} {1}", NomBenefic[0], NomBenefic[1]);
                        Nombres.SegundoNombre = NomBenefic[2];
                        Nombres.PrimerApellido = NomBenefic[3];
                        Nombres.SegundoApellido = NomBenefic[4];
                        break;
                    case 4:
                        Nombres.PrimerNombre = NomBenefic[0];
                        Nombres.SegundoNombre = NomBenefic[1];
                        Nombres.PrimerApellido = NomBenefic[2];
                        Nombres.SegundoApellido = NomBenefic[3];
                        break;
                    case 3:
                        Nombres.PrimerNombre = NomBenefic[0];
                        Nombres.SegundoNombre = string.Empty;
                        Nombres.PrimerApellido = NomBenefic[1];
                        Nombres.SegundoApellido = NomBenefic[2];
                        break;
                    case 2:
                        Nombres.PrimerNombre = NomBenefic[0];
                        Nombres.SegundoNombre = string.Empty;
                        Nombres.PrimerApellido = NomBenefic[1];
                        Nombres.SegundoApellido = string.Empty;
                        break;
                    default:
                        break;
                }
                return Nombres;
            }
            else
            {
                return Nombres;
            }
        }

        public static List<TipoIdentidad> getTiposIdentidades(bool nacional)
        {
            try
            {
                XDocument xd = new XDocument();


                xd = XDocument.Parse(Services.catalogos.GetTiposIdentidades(nacional, "").OuterXml);

                var error = (from r in xd.Descendants("ROOT")
                             select new Base
                             {
                                 error = string.IsNullOrEmpty(r.TryGetElementValue("ERROR")) ? false : true,
                                 clientErrorDetail = r.TryGetElementValue("ERROR")
                             }).FirstOrDefault();
                if (!error.error)
                {
                    var result = (from r in xd.Descendants("RESULTADO")
                                  select new TipoIdentidad
                                  {
                                      idTipo = r.Element("ID_TIPO_IDENTIDAD").Value,
                                      tipoIdentidad = r.Element("TIPO_IDENTIDAD").Value
                                  }).ToList();

                    return result;
                }
                else
                {
                    var ret = new List<TipoIdentidad>() { new TipoIdentidad { clientErrorDetail = error.clientErrorDetail, error = true, apiDetail = "getTiposIdentidades", errorDetail = error.errorDetail } };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new List<TipoIdentidad>() { new TipoIdentidad { clientErrorDetail = "Error al Cargar los Tipos de Identidades", error = true, apiDetail = "getTiposIdentidades", errorDetail = ex } };
                return ret;
            }
        }

        public static TasaCambio getTasaCambio()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.financiero.GetTasaCambioSIMADI().OuterXml);
                if (xd.Root.Name.LocalName != "TASAS")
                {
                    XmlNode xdInterno;
                    xdInterno = Services.financiero.SearchTasaInternal(null, true);
                    if (xdInterno.FirstChild != xdInterno.SelectSingleNode("ERROR"))
                    {
                        if (xdInterno.SelectSingleNode("ENCONTRADO").InnerText == "true")
                        {
                            var result = new TasaCambio
                            {
                                compra = Convert.ToDecimal(xdInterno.SelectSingleNode("//VALORCOMPRA").InnerText, wsCulture),
                                venta = Convert.ToDecimal(xdInterno.SelectSingleNode("//VALORVENTA").InnerText, wsCulture),
                                fechaValor = xdInterno.SelectSingleNode("//FECHAVALOR").InnerText,
                                Internaa = true
                            };
                            return result;
                        }
                        else
                        {
                            var ret = new TasaCambio { clientErrorDetail = "Ha ocurrido un error al obtener la Tasa de Cambio en el banco central de venezuela y no hay tasa asignada para el día.", error = true, apiDetail = "getTasaCambio" };
                            return ret;
                        }
                    }
                    else
                    {
                        var ret = new TasaCambio { clientErrorDetail = "Ha ocurrido un error al obtener la Tasa de Cambio en el banco central de venezuela y no hay tasa asignada para el día:" + xdInterno.SelectSingleNode("ERROR").InnerText, error = true, apiDetail = "getTasaCambio" };
                        return ret;
                    }
                }
                else
                {
                    //where(string)r.Attribute("CODIGO") == "USD"
                    var fecha = (from r in xd.Descendants("DICOM")
                                 select r.Attribute("FECHA_VALOR").Value);

                    var monedas = (from r in xd.Descendants("MONEDA")
                                   select new TasaCambio
                                   {
                                       codigo = r.Attribute("CODIGO").Value,
                                       compra = Convert.ToDecimal(r.Element("COMPRA").Value, wsCulture),
                                       venta = Convert.ToDecimal(r.Element("VENTA").Value, wsCulture),
                                       fechaValor = fecha.FirstOrDefault(),
                                       Internaa = false
                                   }).ToList();
                    var result = monedas.Where(x => x.codigo == "USD").FirstOrDefault();
                    //var result = (from r in xd.Descendants("MONEDA")
                    //              select new TasaCambio
                    //              {
                    //                  compra = Convert.ToDecimal(r.Element("COMPRA").Value.Replace(",", "").Replace(".", ",")),
                    //                  venta = Convert.ToDecimal(r.Element("VENTA").Value.Replace(",", "").Replace(".", ",")),
                    //                  fechaValor = r.Attribute("FECHA_VALOR").Value,
                    //                  Internaa = false
                    //              }).FirstOrDefault();
                    return result;
                }

                //return new TasaCambio
                //{
                //    compra = 1,
                //    venta=2,
                //    fechaValor= DateTime.Now.ToString()
                //};            
            }
            catch (Exception ex)
            {
                var ret = new TasaCambio { clientErrorDetail = "Ha ocurrido un error al obtener la Tasa de Cambio.", error = true, apiDetail = "getTasaCambio", errorDetail = ex };
                return ret;
            }
        }

        public static InfoOficinaDeposito getInfoDepositoByCorresponsal(string pais, bool deposito, string tipo_remesa, string corresponsal)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetInfoDepositoByCorresponsal(pais, deposito, tipo_remesa, corresponsal, string.Empty).OuterXml);
                var result = (from r in xd.Descendants("CORRESPONSALES")
                              select new InfoOficinaDeposito
                              {
                                  idCorresponsal = r.Element("ID_CORRESPONSAL").Value,
                                  idOficina = Convert.ToInt32(r.Element("ID_OFICINA").Value),
                                  idCiudad = Convert.ToInt32(r.Element("ID_CIUDAD").Value),
                                  idPagador = r.Element("ID_PAGADOR").Value,
                                  tasa = Convert.ToDecimal(r.Element("TASA").Value, wsCulture)
                              }).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new InfoOficinaDeposito { clientErrorDetail = "Error al Cargar los Tipos de Movimientos", error = true, apiDetail = "getTiposMovimientos", errorDetail = ex };
                return ret;
            }
        }

        public static List<TipoMovimientosSimadi> getTiposMovimientos(string tipoIdentidad, string keyword)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.financiero.GetTiposMovimientosSimadiInternos(tipoIdentidad, keyword).OuterXml);
                var result = (from r in xd.Descendants("MOVIMIENTO")
                              select new TipoMovimientosSimadi
                              {
                                  id = r.Element("ID_TIPO").Value,
                                  tipoMovimiento = r.Element("TIPO_MOVIMIENTO").Value,
                                  idSudeban = r.Element("ID_SUDEBAN").Value,
                                  requestTarifa = r.Element("REQUEST_TARIFA").Value,
                                  idBcv = r.Element("ID_BCV").Value
                              }).ToList();


                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<TipoMovimientosSimadi>() { new TipoMovimientosSimadi { clientErrorDetail = "Error al Cargar los Tipos de Movimientos", error = true, apiDetail = "getTiposMovimientos", errorDetail = ex } };
                return ret;
            }
        }

        public static List<MotivoOperacion> getMotivosOperacion()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.financiero.GetMotivosSIMADI().OuterXml);
                if (xd.Descendants("error") != null)
                {
                    XmlNode xdInterno;
                    xdInterno = Services.financiero.SearchReasonOrderInternal(null);
                    if (xdInterno.FirstChild != xdInterno.SelectSingleNode("ERROR"))
                    {
                        if (xdInterno.SelectSingleNode("ENCONTRADO").InnerText == "true")
                        {
                            xd = XDocument.Parse(xdInterno.OuterXml);
                            var result = (from r in xd.Descendants("MOTIVOS")
                                          select new MotivoOperacion
                                          {
                                              id = r.Element("REASONCODEBCV").Value,
                                              motivo = r.Element("REASONNAME").Value,
                                              Interna = true
                                          }).Where(x => x.id == "1" || x.id == "2" || x.id == "3" || x.id == "4").ToList();
                            return result;
                        }
                        else
                        {
                            var ret = new List<MotivoOperacion> { new MotivoOperacion { clientErrorDetail = "Ha ocurrido un error al obtener los motivos de operacion en el banco central de venezuela y no hay motivos en la tabla Interna.", error = true, apiDetail = "getMotivosOperacion" } };
                            return ret;
                        }
                    }
                    else
                    {
                        var ret = new List<MotivoOperacion> { new MotivoOperacion { clientErrorDetail = "Ha ocurrido un error al obtener los motivos de operación en el banco central de venezuela y error interanamente:" + xdInterno.SelectSingleNode("ERROR").InnerText, error = true, apiDetail = "getMotivosOperacion" } };
                        return ret;
                    }
                }
                else
                {
                    var result = (from r in xd.Descendants("MOTIVO")
                                  select new MotivoOperacion
                                  {
                                      id = r.Element("CO_MOTIVO_OPERACION").Value,
                                      motivo = r.Element("TX_MOTIVO_OPERACION").Value
                                  }).Where(x => x.id == "1" || x.id == "2" || x.id == "3" || x.id == "4").ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                var ret = new List<MotivoOperacion>() { new MotivoOperacion { clientErrorDetail = "Error al Cargar los Motivos de la Orden", error = true, apiDetail = "getMotivosOperacion", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Banco> getBancosNacionales()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetBancos(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("BANCO")
                              select new Banco
                              {
                                  codBanco = r.Element("CODBANCO").Value,
                                  nombreBanco = r.Element("NOMBREBANCO").Value,
                                  activo = r.TryGetElementValue("ACTIVO", "0"),
                                  ccal = r.TryGetElementValue("CCAL", "0"),
                                  idPais = r.Element("CODPAIS").Value,
                                  depositoActivo = r.TryGetElementValue("DEPOSITO_ACTIVO", "0"),
                              }).Where(x => x.activo == "1" && x.idPais == "VZL").OrderBy(x => x.nombreBanco).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Banco>() { new Banco { clientErrorDetail = "Error al Cargar los Bancos Nacionales", error = true, apiDetail = "getBancosNacionales", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Pais> getPaises()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetPaisesTodos(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("PAIS")
                              select new Pais
                              {
                                  nombrePais = r.Element("NOMBRE_PAIS").Value,
                                  idPais = r.Element("ID_PAIS").Value,
                                  activo_envio = Convert.ToBoolean(r.Element("ACTIVO_ENVIO").Value),
                                  codeIso = r.Element("ISO_CODE").Value
                              }).Where(x => x.activo_envio).OrderBy(x => x.nombrePais).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Pais>() { new Pais { clientErrorDetail = "Error al Cargar los Bancos Nacionales", error = true, apiDetail = "getBancosNacionales", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Pais> getPaisesEnvios()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetPaisesEnvios(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("PAISES")
                              select new Pais
                              {
                                  nombrePais = r.Element("PAIS").Value,
                                  idPais = r.Element("ID_PAIS").Value,
                                  codeIso = r.Element("ISO_CODE").Value
                                  //activo_envio = Convert.ToBoolean(r.Element("ACTIVO_ENVIO").Value)
                              }).OrderBy(x => x.nombrePais).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Pais>() { new Pais { clientErrorDetail = "Error al Cargar los paises para envios ", error = true, apiDetail = "getPaisesEnvios", errorDetail = ex } };
                return ret;
            }
        }

        public static long GetProximoNumero(int tipo, string BranchOffice, string user, bool SoloConsulta)
        {
            var num = Core.Financial.Api.Utils.Services.utilitarios.GetProximoNumero(tipo, BranchOffice, user, SoloConsulta, DateTime.Now.ToString(), "");
            long _operationNumber = 0;
            if (num.FirstChild != num.SelectSingleNode("ERROR"))
            {
                if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                    _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                else
                    _operationNumber = 0;
            }
            return _operationNumber;
        }

        public static bool ValidateTokenExternal(string tk, string cli)
        {
            return true;
        }

        public static string getNumeroIdFormato(string tipoId, string numeroId)
        {
            var ret = "";
            switch (tipoId.ToUpper())
            {
                case "V":
                case "E":
                    ret = numeroId.ToString().PadLeft(8, '0');
                    break;
                default:
                    ret = numeroId.ToString();
                    break;
            }

            return ret;
        }

        public static OrdenCompraEfectivo setCompraDivisasEfectivo(OrdenCompraEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "compra-simadi-taq";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 6; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = user.idCiudadExterna;
                var idOficina = user.idOficinaExterna;
                var idDetalleTipoOperacion = 8; //para las ventas
                var idMoneda = 210; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;

                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion

                if (orden.MonedaOperacion != 213)
                {
                    orden.montoOrden = (orden.montoOrden * decimal.Parse(orden.TasaConversion)) / orden.tasaCambio;

                }
                else
                {
                    orden.MontoConversion = orden.montoOrden.ToString();
                    orden.TasaConversion = orden.tasaCambio.ToString();
                }
                //Primer paso, validar si el BCV aprueba la operación
                //var responseBCV = Services.financiero.SetCompraDivisasSIMADITest(tipoMovimiento,
                //               string.Format("{0}{1}", orden.tipoIdCliente, numeroId),
                //                orden.nombresCliente,
                //                orden.montoOrden,
                //                orden.tasaCambio, "840", orden.montoOrden,
                //                Convert.ToInt64(orden.motivoOferta),
                //                string.IsNullOrEmpty(orden.numeroCuentaCliente) ? "01020000000000000000" : orden.numeroCuentaCliente.ToString(),
                //                string.IsNullOrEmpty(orden.telefonoCliente) ? "04141111111" : orden.telefonoCliente,
                //                string.IsNullOrEmpty(orden.emailCliente) ? "sincorreo@gmail.com" : orden.emailCliente);
                //if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                //{
                //    //responseBCV = new XmlDocument();
                //    responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //}
                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenCompraEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                        orden.tipoIdBeneficiario = "R";
                        orden.numeroIdBeneficiario = "302075661";

                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        //var referenciaBCV = "jkhkjhkhklhkhk";
                        var num = Services.utilitarios.GetProximoNumero(4, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }

                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        orden.idPaisDestino = "VZL";
                        #region Order Insert
                        var resultOrder = Services.miscelaneos.SetRegistrarOperacionTemporal(
                         user.letraSucursal, _operationNumber, string.Format("{0}{1}", orden.tipoIdCliente, orden.numeroIdCliente),
                         nc1, nc2, nc3, nc4, n,
                         n, nb1, nb2, nb3, nb4,
                         string.Format("{0}{1}", orden.tipoIdBeneficiario, orden.numeroIdBeneficiario),
                         n, n, n, orden.idPaisDestino, idPagador, idCiudad, idOficina,
                         montoBolivares, orden.montoOrden - comisionUs,
                         1, orden.montoOrden - comisionUs, comisionUs,
                         comisionBs, 0, 0, n, n, user.login,
                         codigoTipoOperacion, referenciaBCV,
                         1, idTipoOperacion, 0, 0, 0, _idDatosDeposito, 0, "",
                         orden.idCliente, 0, true, false, false, 0, n, n,
                         orden.tasaCambio, idDetalleTipoOperacion, idMoneda,
                         user.idOficinaNew, "TRF",
                         orden.fechaValorTasa, Convert.ToInt32(orden.motivoOferta), tipoMovimiento,
                         orden.bancoCliente, orden.numeroCuentaCliente, orden.emailCliente, n,
                         n, "0", n, n, n, n, n, n, n, n, n, n, orden.MonedaOperacion, decimal.Parse(orden.TasaConversion), decimal.Parse(orden.MontoConversion), "");
                        /*
                           string BANCO_INTERMEDIARIO, 
                           string NUMERO_CUENTA_INTERMEDIARIO,
                           string DIRECCION_BANCO_INTERMEDIARIO,
                           string ABA_INTERMEDIARIO,
                           string SWIFT_INTERMEDIARIO,
                           string IBAN_INTERMEDIARIO
                        */
                        if (resultOrder.FirstChild != resultOrder.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(resultOrder.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                            {
                                var _idOrden = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                                if (orden.tarifas == null)
                                {
                                    var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                    return ret;
                                }
                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                                {
                                    decimal _comisionUs = 0;
                                    if (item.valor < 1)
                                        _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                    else
                                        _comisionUs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                                }

                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                                {
                                    decimal _comisionBs = 0;

                                    if (item.valor < 1)
                                        _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                    else
                                        _comisionBs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                                }
                            }
                            else
                            {
                                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultOrder.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultOrder.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                        orden.CiuOrig = user.letraSucursal;
                        orden.OrderNumber = _operationNumber.ToString();
                    }
                }
                else
                {
                    var ret = new OrdenCompraEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenCompraEfectivo InsertSolicitudDivisasEfectivo(OrdenCompraEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "compra-simadi-taq";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 8; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = user.idCiudadExterna;
                var idOficina = user.idOficinaExterna;
                var idDetalleTipoOperacion = 8; //para las ventas
                var idMoneda = 213; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;

                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                if (orden.MonedaConversion == 0)
                    orden.MonedaConversion = 221;
                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                var operationCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaOperacion).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                if (operationCoins == null)
                {
                    return new OrdenCompraEfectivo { clientErrorDetail = "No se logro consultar las tasas de la operación.", error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new OrdenCompraEfectivo { clientErrorDetail = "No se logro consultar las tasas de conversión.", error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorCompra ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorVenta.ToString();
                }


                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenCompraEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                        orden.tipoIdBeneficiario = "R";
                        orden.numeroIdBeneficiario = "302075661";

                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * decimal.Parse(orden.TasaConversion), 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }

                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * decimal.Parse(orden.TasaConversion)), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        orden.idPaisDestino = "VE";
                        orden.ClientSurnames = string.Concat(nc1, " ", nc2);
                        orden.ClientNames = string.Concat(nc3, " ", nc4);
                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;

                        #region Order Insert
                        IAnguloLopezBuilder builder = new AnguloLopezBuilder();
                        Solicitudes objSolicitud = new Solicitudes
                        {
                            DETALLE_TIPO_OPERACION = idTipoOperacion,
                            CLIENTE = orden.idClienteUnique,
                            SUCURSAL = 16,
                            STATUS_SOLICITUD = 1,
                            MONEDA = orden.MonedaConversion,
                            OFICINA = idOficina,
                            PAIS_DESTINO = orden.idPaisDestino,
                            CORRESPONSAL = "TRF",
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            MONTO_CAMBIO = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            NOMBRES_REMITENTE = nc3 + " " + nc4,
                            APELLIDOS_REMITENTE = nc1 + " " + nc2,
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = nb1 + " " + nb2,
                            APELLIDOS_BENEFICIARIO = nb3 + " " + nb4,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            //BANCO_NACIONAL = orden.bancoCliente,
                            //NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            EMAIL_BENEFICIARIO = orden.emailCliente,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.nombreBancoDestino,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaDestino.ToString(),
                            DIRECCION_BANCO = orden.direccionBancoDestino,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = idPagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            USUARIO_TAQUILLA = user.login,
                            TasaConversion = decimal.Parse(orden.TasaConversion),
                            MonedaOperacion = orden.MonedaOperacion,
                            MontoConversion = changeAmount,
                            REFERENCIA_ORDEN = referenciaBCV,
                            CommissionUsd = comisionUs,
                            BancoPagoTransferencia = orden.bancoCliente,
                            NumeroCuentaPagoTransferencia = orden.numeroCuentaCliente,
                            CommissionBs = comisionBs
                        };
                        var resultInsert = builder.InsertSolicitudes(objSolicitud);
                        if (!resultInsert.Error)
                        {
                            if (orden.tarifas == null)
                            {
                                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                            foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);

                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });

                                //Services.ordenes.setTarifasAplicadasOrden(resultInsert.ReturnId, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * decimal.Parse(orden.TasaConversion)), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);

                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(resultInsert.ReturnId, item.idTarifa, _comisionBs, user.login);
                            }
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultInsert.ErrorMessage) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = resultInsert.ReturnId;
                    }
                }
                else
                {
                    var ret = new OrdenCompraEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenCompraEfectivo InsertPagoRemesa(OrdenCompraEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "compra-simadi-enc";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.FirstOrDefault().idBcv;
                var idTipoOperacion = 10; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = user.idCiudadExterna;
                var idOficina = user.idOficinaExterna;
                var idDetalleTipoOperacion = 10;
                var idMoneda = 213; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;

                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion

                if (orden.MonedaConversion == 0)
                    orden.MonedaConversion = 221;
                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                var operationCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaOperacion).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                if (operationCoins == null)
                {
                    return new OrdenCompraEfectivo { clientErrorDetail = "No se logro consultar las tasas de la operación.", error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new OrdenCompraEfectivo { clientErrorDetail = "No se logro consultar las tasas de conversión.", error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorCompra ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorVenta.ToString();
                }

                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenCompraEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                        orden.tipoIdBeneficiario = "R";
                        orden.numeroIdBeneficiario = "302075661";

                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * decimal.Parse(orden.TasaConversion), 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }

                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * decimal.Parse(orden.TasaConversion)), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        orden.idPaisDestino = "VE";
                        orden.ClientSurnames = string.Concat(nc1, " ", nc2);
                        orden.ClientNames = string.Concat(nc3, " ", nc4);
                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;

                        #region Order Insert
                        IOperacionesBuilder builder = new OperacionesBuilder();
                        Ordenes objOrden = new Ordenes
                        {
                            DETALLE_TIPO_OPERACION = idTipoOperacion,
                            CLIENTE = orden.idCliente,
                            SUCURSAL = 16,
                            STATUS_ORDEN = 8,
                            MONEDA = orden.MonedaConversion,
                            OFICINA = idOficina,
                            PAIS_DESTINO = orden.idPaisDestino,
                            CORRESPONSAL = "TRF",
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            MONTO_CAMBIO = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            NOMBRES_REMITENTE = nc3 + " " + nc4,
                            APELLIDOS_REMITENTE = nc1 + " " + nc2,
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = nb1 + " " + nb2,
                            APELLIDOS_BENEFICIARIO = nb3 + " " + nb4,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            //BANCO_NACIONAL = orden.bancoCliente,
                            //NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            EMAIL_BENEFICIARIO = orden.emailCliente,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.nombreBancoDestino,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaDestino.ToString(),
                            DIRECCION_BANCO = orden.direccionBancoDestino,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = idPagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            USUARIO_TAQUILLA = user.login,
                            TasaConversion = decimal.Parse(orden.TasaConversion),
                            MonedaOperacion = orden.MonedaOperacion,
                            MontoConversion = changeAmount,
                            REFERENCIA_ORDEN = referenciaBCV,
                            //CommissionUsd = comisionUs,
                            BancoPagoTransferencia = orden.bancoCliente,
                            NumeroCuentaPagoTransferencia = orden.numeroCuentaCliente,
                            SucursalProcesaId = orden.SucursalProcesa,
                            ModificadoPor = orden.ModificadoPor,
                            Modificado = DateTime.Now,
                            FECHA_OPERACION = DateTime.Now,
                            FECHA_PAGO = DateTime.Now,
                            REFERENCIA_PAGO = orden.ReferenciaPago,
                            //CommissionBs = comisionBs
                        };
                        var resultInsert = builder.InsertOrdenes(objOrden);
                        if (!resultInsert.Error)
                        {
                            if (orden.tarifas == null)
                            {
                                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                            IAnguloLopezBuilder builderAngulo = new AnguloLopezBuilder();
                            foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);

                                builderAngulo.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });

                                //Services.ordenes.setTarifasAplicadasOrden(resultInsert.ReturnId, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * decimal.Parse(orden.TasaConversion)), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);

                                builderAngulo.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(resultInsert.ReturnId, item.idTarifa, _comisionBs, user.login);
                            }
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultInsert.ErrorMessage) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.OrderNumber = _operationNumber.ToString();
                        orden.CiuOrig = user.letraSucursal;
                        orden.id = resultInsert.ID_OPERACION;
                    }
                }
                else
                {
                    var ret = new OrdenCompraEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenCompraEfectivo InsertPaymentRemesa(OrdenCompraEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "compra-simadi-enc";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.FirstOrDefault().idBcv;
                var idTipoOperacion = 10; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = user.idCiudadExterna;
                var idOficina = user.idOficinaExterna;
                var idDetalleTipoOperacion = 10;
                var idMonedaOperacion = 213; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                decimal amountParameter = 0;

                Common.Models.Clientes.SearchClientsRequest clientsRequest = new Common.Models.Clientes.SearchClientsRequest()
                {
                    id_cedula = orden.numeroIdBeneficiario,
                    clienteTipo = orden.tipoIdBeneficiario,
                    offSet = 0,
                    limit = 10
                };

                IClientsBuilder builderclients = new ClientsBuilder();
                var clients = builderclients.Searchfichas(clientsRequest).FirstOrDefault();

                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                ITasasBuilder tasaBuilder = new TasasBuilder();
                var tasas = tasaBuilder.SearchHistorial(new Common.Models.Angulo_Lopez.Tasas.HistorialRequest
                {
                    Date = orden.fechaValorTasa
                });

                if (!tasas.Any())
                {
                    return new OrdenCompraEfectivo { clientErrorDetail = "No se logro consultar las tasas de la operación.", error = true };
                }
                var objTasa = tasas.OrderByDescending(x => x.fechaRegistro).FirstOrDefault();
                if (orden.tasaCambio != 1)
                    orden.tasaCambio = objTasa.valorCompra;

                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(codigoTipoOperacion, orden.montoOrden, "USD", orden.PaisId, "FURBANO", orden.CorresponsalId, string.Empty).OuterXml);
                if (xd.Root.Descendants("error").Any())
                {
                    return new OrdenCompraEfectivo { clientErrorDetail = xd.Root.Descendants("error").First().Value, error = true };
                }
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();


                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenCompraEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables

                        Common.Resource.NameBeneficiary NameBeneficiary = Common.Resource.SplitName.NameSeparator(orden.NombresRemitente);

                        #endregion
                        decimal comisionUs = 0, comisionBs = 0, montoOrden = orden.montoOrden, montoCoversion = orden.MontoBolivares;
                        orden.idPaisDestino = "VE";
                        orden.ClientSurnames = string.Concat(nb1, " ", nb2);
                        orden.ClientNames = string.Concat(nb3, " ", nb4);
                        decimal changeAmount = orden.montoOrden;
                        #region Order Insert
                        IOperacionesBuilder builder = new OperacionesBuilder();
                        Ordenes objOrden = new Ordenes
                        {
                            DETALLE_TIPO_OPERACION = idTipoOperacion,
                            CLIENTE = orden.idCliente,
                            SUCURSAL = orden.SucursalProcesa,
                            STATUS_ORDEN = 8,
                            MONEDA = orden.MonedaConversion,
                            OFICINA = idOficina,
                            PAIS_DESTINO = orden.idPaisDestino,
                            CORRESPONSAL = "CAL",
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            TASA_DESTINO= orden.tasaCambio,
                            MONTO = montoOrden,
                            MONTO_CAMBIO = montoCoversion,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            NOMBRES_REMITENTE = NameBeneficiary.Names,
                            APELLIDOS_REMITENTE = NameBeneficiary.Surnames,
                            IDENTIFICACION_REMITENTE = orden.DocumentoRemitente,
                            NOMBRES_BENEFICIARIO = clients.PrimerNombre.Trim() + " " + (string.IsNullOrEmpty(clients.SegundoNombre) ? string.Empty : clients.SegundoNombre.Trim()), 
                            APELLIDOS_BENEFICIARIO = clients.PrimerApellido.Trim() + " " + (string.IsNullOrEmpty(clients.SegundoApellido) ? string.Empty : clients.SegundoApellido.Trim()),
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            //BANCO_NACIONAL = orden.bancoCliente,
                            //NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = "",
                            EMAIL_BENEFICIARIO = orden.emailCliente,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.nombreBancoDestino,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaDestino.ToString(),
                            DIRECCION_BANCO = orden.direccionBancoDestino,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = idPagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            USUARIO_TAQUILLA = user.login,
                            TasaConversion = orden.tasaCambio,
                            MonedaOperacion = idMonedaOperacion,
                            MontoConversion = changeAmount,
                            REFERENCIA_ORDEN = referenciaBCV,
                            //CommissionUsd = comisionUs,
                            BancoPagoTransferencia = orden.bancoCliente,
                            NumeroCuentaPagoTransferencia = orden.numeroCuentaCliente,
                            SucursalProcesaId = orden.SucursalProcesa,
                            ModificadoPor = orden.ModificadoPor,
                            Modificado = DateTime.Now,
                            FECHA_OPERACION = DateTime.Now,
                            FECHA_PAGO = DateTime.Now,
                            REFERENCIA_PAGO = orden.ReferenciaPago,
                            //CommissionBs = comisionBs
                        };
                        var resultInsert = builder.InsertOrdenes(objOrden);
                        if (!resultInsert.Error)
                        {
                            if (orden.tasaCambio == 1)
                            {
                                tarifas = tarifas.Where(x => x.idTarifa != 39).ToList();
                            }
                            IAnguloLopezBuilder builderAngulo = new AnguloLopezBuilder();
                            foreach (var item in tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);

                                builderAngulo.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login,
                                    ORDEN = resultInsert.ID_OPERACION
                                });

                                //Services.ordenes.setTarifasAplicadasOrden(resultInsert.ReturnId, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                            {
                                decimal _comisionBs = 0;
                                if (item.valor < 1)
                                    //Se utiliza valorCompra del bcv porque la tasa venta tiene error en el WS el mismo valor suministrado por
                                    //el WS en la tasa de compra pertenece a la tasa de venta BCV
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * objTasa.valorCompra), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);

                                builderAngulo.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login,
                                    ORDEN= resultInsert.ID_OPERACION
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(resultInsert.ReturnId, item.idTarifa, _comisionBs, user.login);
                            }
                        }
                        else
                        {
                            var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultInsert.ErrorMessage) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.OrderNumber = _operationNumber.ToString();
                        orden.CiuOrig = user.letraSucursal;
                        orden.id = resultInsert.ID_OPERACION;
                        orden.tasaCambio = objTasa.valorCompra;
                    }
                }
                else
                {
                    var ret = new OrdenCompraEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenCompraEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }



        public static OrdenCompraCorresponsal setCompraDivisasCorresponsal(OrdenCompraCorresponsal orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "compra-simadi-enc";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 10; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = user.idCiudadExterna;
                var idOficina = user.idOficinaExterna;
                var idDetalleTipoOperacion = 8; //para las compras
                var idMoneda = 210; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;

                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                //Primer paso, validar si el BCV aprueba la operación

                //Comentado por Johan Cortes 21_07_2016 Proceso que se realiza cuando en el metodo de entrada de la remesa
                //var responseBCV = Services.financiero.SetCompraDivisasSIMADI(tipoMovimiento,
                //               string.Format("{0}{1}", orden.tipoIdCliente, numeroId),
                //                orden.nombresCliente,
                //                orden.montoOrden,
                //                orden.tasaCambio, "840", orden.montoOrden,
                //                Convert.ToInt64(orden.motivoOferta),
                //                n, //string.IsNullOrEmpty(string.Empty) ? "01020000000000000000" : string.Empty,
                //                string.IsNullOrEmpty(orden.telefonoCliente) ? "04141111111" : orden.telefonoCliente,
                //                string.IsNullOrEmpty(orden.emailCliente) ? "sincorreo@gmail.com" : orden.emailCliente);

                //if (responseBCV != null)
                //{
                //    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                //    {
                //        var ret = new OrdenCompraCorresponsal { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                //        return ret;
                //    }
                //    else
                //    {
                //orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                //orden.tipoIdBeneficiario = "R";
                //orden.numeroIdBeneficiario = "302075661";

                #region Asignación Numero de Orden
                //var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                var num = Services.utilitarios.GetProximoNumero(4, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                long _operationNumber = 0;
                if (num.FirstChild != num.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                        _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                    else
                        _operationNumber = 0;
                }
                else
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                    return ret;

                }
                if (_operationNumber == 0)
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                    return ret;
                }
                #endregion
                #region Agregamos la información a la tabla temporal
                #region Mapeo de Nombres en Variables
                var nc = orden.nombreOrdenante.Split(' ');
                switch (nc.Length)
                {
                    case 5:
                        nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                        nc2 = nc[2];
                        nc3 = nc[3];
                        nc4 = nc[4];
                        break;
                    case 4:
                        nc1 = nc[0];
                        nc2 = nc[1];
                        nc3 = nc[2];
                        nc4 = nc[3];
                        break;
                    case 3:
                        nc1 = nc[0];
                        nc2 = string.Empty;
                        nc3 = nc[1];
                        nc4 = nc[2];
                        break;
                    case 2:
                        nc1 = nc[0];
                        nc2 = string.Empty;
                        nc3 = nc[1];
                        nc4 = string.Empty;
                        break;
                    default:
                        break;
                }
                var nb = orden.nombreBeneficiario.Split(' ');
                switch (nb.Length)
                {
                    case 5:
                        nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                        nb2 = nb[2];
                        nb3 = nb[3];
                        nb4 = nb[4];
                        break;
                    case 4:
                        nb1 = nb[0];
                        nb2 = nb[1];
                        nb3 = nb[2];
                        nb4 = nb[3];
                        break;
                    case 3:
                        nb1 = nb[0];
                        nb2 = string.Empty;
                        nb3 = nb[1];
                        nb4 = nb[2];
                        break;
                    case 2:
                        nb1 = nb[0];
                        nb2 = string.Empty;
                        nb3 = nb[1];
                        nb4 = string.Empty;
                        break;
                    default:
                        break;
                }
                #endregion
                var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                decimal comisionUs = 0, comisionBs = 0;

                if (orden.tarifas == null)
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                    return ret;
                }
                foreach (var item in orden.tarifas)
                {
                    if (item.moneda.Equals("USD"))
                    {
                        if (item.valor < 1)
                            comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                        else
                            comisionUs += Math.Round(item.valor, 2);
                    }
                    else
                    {
                        if (item.valor < 1)
                            comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                        else
                            comisionBs += Math.Round(item.valor, 2);
                    }
                }

                orden.idPaisDestino = "VZL";
                #region Order Insert

                var resultOrder = Services.miscelaneos.SetRegistrarOperacionTemporal(
                 user.letraSucursal, _operationNumber,
                 string.Format("{0}{1}", orden.identificacionOrdenante.Substring(0, 1), orden.identificacionOrdenante.Replace(orden.identificacionOrdenante.Substring(0, 1), "")),
                 nc1, nc2, nc3, nc4, n,
                 n, nb1, nb2, nb3, nb4,
                 string.Format("{0}{1}", orden.tipoIdBeneficiario, orden.numeroIdBeneficiario),
                 orden.telefonoBeneficiario, n, n, orden.idPaisDestino, idPagador, idCiudad, idOficina,
                 montoBolivares - comisionBs, orden.montoOrden - comisionUs,
                 1, orden.montoOrden - comisionUs, comisionUs,
                 comisionBs, 0, 0, n, n, user.login,
                 codigoTipoOperacion, orden.referenciaBCV,
                 1, idTipoOperacion, 0, 0, 0, _idDatosDeposito, 0, "",
                 orden.idCliente, 0, true, false, false, 0, n, n,
                 orden.tasaCambio, idDetalleTipoOperacion, idMoneda,
                 user.idOficinaNew, "TRF",
                 orden.fechaValorTasa, Convert.ToInt32(orden.motivoOferta), tipoMovimiento,
                 n, "0", orden.emailCliente, n,
                 n, "0", n, n, n, n, n, n, n, n, n, n, null, null, null, "");
                /*
                    n, n, n, n, n, n
                  string BANCO_INTERMEDIARIO, 
                  string NUMERO_CUENTA_INTERMEDIARIO,
                  string DIRECCION_BANCO_INTERMEDIARIO,
                  string ABA_INTERMEDIARIO,
                  string SWIFT_INTERMEDIARIO,
                  string IBAN_INTERMEDIARIO
               */
                if (resultOrder.FirstChild != resultOrder.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(resultOrder.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                    {
                        #region Tarifas Aplicadas
                        var _idOrden = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }
                        foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                        {
                            decimal _comisionUs = 0;
                            if (item.valor < 1)
                                _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                            else
                                _comisionUs = Math.Round(item.valor, 2);

                            Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                        }

                        foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                        {
                            decimal _comisionBs = 0;

                            if (item.valor < 1)
                                _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                            else
                                _comisionBs = Math.Round(item.valor, 2);

                            Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                        }
                        #endregion
                        #region Actualización Parcial del registro de pago enviado por el corresponsal
                        var resultUpdate = Services.pagos.SetInfoRemesaEntrante(0, orden.referenciaBCV, Convert.ToInt32(orden.id), DateTime.Now, Convert.ToInt32(_idOrden), n);
                        if (resultUpdate.FirstChild != resultUpdate.SelectSingleNode("ERROR"))
                        {
                            if (!Convert.ToBoolean(resultUpdate.SelectSingleNode("//ACTUALIZADO").InnerText.Trim()))
                            {
                                var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultUpdate.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultUpdate.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        #endregion
                    }
                    else
                    {
                        var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                        return ret;
                    }
                }
                else
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultOrder.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultOrder.SelectSingleNode("ERROR").InnerText) };
                    return ret;
                }
                #endregion
                #endregion
                //orden.idResult = referenciaBCV;
                orden.temporal = 1;
                orden.id = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                //}
                //}
                //else
                //{
                //    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                //    return ret;
                //}
            }
            catch (Exception ex)
            {
                var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static Usuario getUsuario(string login)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.users.GetInformacionUsuario(login, string.Empty).OuterXml);
                var result = (from r in xd.Descendants("RESULTADO")
                              select new Usuario
                              {
                                  login = r.TryGetElementValue("LOGIN"),
                                  apellidos = r.TryGetElementValue("APELLIDOS"),
                                  nombreCompleto = r.TryGetElementValue("USERNAME"),
                                  nivelSeguridad = Convert.ToInt32(r.Element("NIVELSEG").Value),
                                  status = r.Element("STATUS").Value,
                                  idSucursal = r.Element("SUCURSAL").Value,
                                  idSucursal2 = Convert.ToInt32(r.Element("ID_SUCURSAL2").Value),
                                  letraSucursal = r.Element("LETRA").Value,
                                  cajaIndependiente = Convert.ToBoolean(r.Element("CAJA_INDEPENDIENTE").Value),
                                  fechaCambioClave = r.TryGetElementValue("CAMBIOPWD"),
                                  nombreSucursal = r.TryGetElementValue("NOMBRE_SUCURSAL"),
                                  fechaCreacion = r.TryGetElementValue("FECHA_CREACION"),
                                  esAgencia = Convert.ToBoolean(r.Element("ES_AGENCIA").Value),
                                  formaLibre = Convert.ToBoolean(r.Element("FORMA_LIBRE").Value),
                                  direccionSucursal = r.TryGetElementValue("DIRECCION_SUCURSAL"),
                                  tipoIdentidad = r.TryGetElementValue("TIPO_IDENTIDAD"),
                                  numeroIdentidad = r.TryGetElementValue("NUMERO_IDENTIDAD"),
                                  cargo = r.TryGetElementValue("CARGO"),
                                  codigoCargo = r.TryGetElementValue("CODIGO_CARGO"),
                                  codigoEmpleado = r.TryGetElementValue("CODIGO_EMPLEADO"),
                                  idOficinaNew = Convert.ToInt32(r.Element("ID_OFICINA_NEW").Value),
                                  idOficinaExterna = Convert.ToInt32(r.Element("ID_OFIC_EXTERNA").Value),
                                  idCiudadExterna = Convert.ToInt32(r.Element("ID_CIUDAD_EXTERNA").Value)
                              }).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new Usuario { clientErrorDetail = "Ha ocurrido un error al obtener información del usuario.", error = true, apiDetail = "getTasaCambio", errorDetail = ex };
                return ret;
            }

        }

        public static OrdenVentaTransferencia setVentaDivisasTransferencia(OrdenVentaTransferencia orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "venta-simadi-trf";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 8; //tipo de operacion tabla temporal, para las ventas de divisas por transferencias.

                var oficInfo = getInfoDepositoByCorresponsal(orden.idPaisDestino, true, codigoTipoOperacion, orden.idCorresponsal);

                if (oficInfo == null)
                {
                    oficInfo = new InfoOficinaDeposito
                    {
                        idPagador = "MUL",
                        idCiudad = 368,
                        idOficina = 448,
                        tasa = 1
                    };
                }

                var idPagador = oficInfo.idPagador; //"MUL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = oficInfo.idCiudad; //368;
                var idOficina = oficInfo.idOficina; //448;
                var idDetalleTipoOperacion = 9; //para las ventas
                var idMoneda = 210; //el codigo de la moneda interna

                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                //Primer paso, validar si el BCV aprueba la operación
                //var responseBCV = Services.financiero.SetVentaDivisasSIMADI(tipoMovimiento,
                //               string.Format("{0}{1}", orden.tipoIdCliente, numeroId),
                //                orden.nombresCliente,
                //                orden.montoOrden,
                //                orden.tasaCambio, "840", orden.montoOrden,
                //                Convert.ToInt64(orden.motivoOferta),
                //                n,
                //                string.IsNullOrEmpty(orden.telefonoCliente) ? "04141111111" : orden.telefonoCliente,
                //                string.IsNullOrEmpty(orden.emailCliente) ? "sincorreo@gmail.com" : orden.emailCliente);
                //if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                //{
                //    responseBCV = new XmlDocument();
                //    responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //}
                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenVentaTransferencia { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(4, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion
                        #region Información de Depósito
                        var _resultadoFirstInsertXml = Services.operaciones.SetRegistrarCambioDatos(user.letraSucursal,
                                                _operationNumber, orden.nombresBeneficiario,
                                                orden.numeroIdBeneficiario, "A", orden.numeroCuentaDestino.ToString(), orden.nombreBancoDestino, orden.direccionBancoDestino,
                                                string.Format("ABA: {0}, SWIFT: {1}, IBAN: {2}", orden.aba, orden.swift, orden.iban),
                                                user.login, 1, 0, user.login, "APROBADO AUTOMATICAMENTE SEGUN INFORMACION SUMINISTRADA POR EL CLIENTE", 1, 0, "");
                        if (_resultadoFirstInsertXml.FirstChild == _resultadoFirstInsertXml.SelectSingleNode("ERROR"))
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de registrar la información de la transferencia \n [" + _resultadoFirstInsertXml.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(_resultadoFirstInsertXml.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        else
                        {
                            if (Convert.ToBoolean(_resultadoFirstInsertXml.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                                _idDatosDeposito = Convert.ToInt64(_resultadoFirstInsertXml.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                            else
                            {
                                var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de registrar la información de la transferencia.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("Sin Id de Insert") };
                                return ret;
                            }
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }
                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        #region Order Insert
                        var resultOrder = Services.miscelaneos.SetRegistrarOperacionTemporal(
                         user.letraSucursal, _operationNumber, string.Format("{0}{1}", orden.tipoIdCliente, orden.numeroIdCliente),
                         nc1, nc2, nc3, nc4, n,
                         n, nb1, nb2, nb3, nb4,
                         string.Format("{0}{1}", orden.tipoIdBeneficiario, orden.numeroIdBeneficiario),
                         n, n, n, orden.idPaisDestino, idPagador, idCiudad, idOficina,
                         montoBolivares, orden.montoOrden - comisionUs,
                         oficInfo.tasa, (orden.montoOrden - comisionUs) * oficInfo.tasa, comisionUs,
                         comisionBs, 0, 0, orden.observaciones, n, user.login,
                         codigoTipoOperacion, referenciaBCV,
                         1, idTipoOperacion, 0, 0, 0, _idDatosDeposito, 0, "",
                         orden.idCliente, 0, true, false, false, 0, n, n,
                         orden.tasaCambio, idDetalleTipoOperacion, idMoneda,
                         user.idOficinaNew, orden.idCorresponsal,
                         orden.fechaValorTasa, Convert.ToInt32(orden.motivoOferta), tipoMovimiento,
                         orden.bancoCliente, orden.numeroCuentaCliente, orden.emailCliente, n,
                         orden.nombreBancoDestino, orden.numeroCuentaDestino, orden.direccionBancoDestino,
                         orden.aba, orden.swift, orden.iban, orden.bancoIntermediario, orden.numeroCuentaIntermediario,
                         orden.direccionBancoIntermediario, orden.abaIntermediario, orden.swiftIntermediario,
                         orden.ibanIntermediario, null, null, null, "");
                        /*
                            n, n, n, n, n, n
                          string BANCO_INTERMEDIARIO, 
                          string NUMERO_CUENTA_INTERMEDIARIO,
                          string DIRECCION_BANCO_INTERMEDIARIO,
                          string ABA_INTERMEDIARIO,
                          string SWIFT_INTERMEDIARIO,
                          string IBAN_INTERMEDIARIO
                       */

                        if (resultOrder.FirstChild != resultOrder.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(resultOrder.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                            {
                                var _idOrden = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                                if (orden.tarifas == null)
                                {
                                    var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                    return ret;
                                }
                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                                {
                                    decimal _comisionUs = 0;
                                    if (item.valor < 1)
                                        _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                    else
                                        _comisionUs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                                }

                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                                {
                                    decimal _comisionBs = 0;

                                    if (item.valor < 1)
                                        _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                    else
                                        _comisionBs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                                }
                            }

                            else
                            {
                                var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultOrder.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultOrder.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                    }
                }
                else
                {
                    var ret = new OrdenVentaTransferencia { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenVentaTransferencia InsertSaleTransfer(OrdenVentaTransferencia orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "venta-simadi-trf";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 8; //tipo de operacion tabla temporal, para las ventas de divisas por transferencias.

                var oficInfo = getInfoDepositoByCorresponsal(orden.idPaisDestino, true, codigoTipoOperacion, orden.idCorresponsal);
                if (orden.MonedaConversion == 0)
                    orden.MonedaConversion = 221;
                if (oficInfo == null)
                {
                    oficInfo = new InfoOficinaDeposito
                    {
                        idPagador = "MUL",
                        idCiudad = 368,
                        idOficina = 448,
                        tasa = 1
                    };
                }

                var idPagador = oficInfo.idPagador; //"MUL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = oficInfo.idCiudad; //368;
                var idOficina = oficInfo.idOficina; //448;
                var idDetalleTipoOperacion = 9; //para las ventas
                var idMoneda = 213; //el codigo de la moneda interna

                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion

                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                var operationCoins = infMonedas.Where(x => x.MonedaId == idMoneda).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                orden.AmmountTotalConversion = orden.tarifas.FirstOrDefault(x => x.InternalId == "AmmountConversion").valor;
                if (operationCoins == null)
                {
                    return new OrdenVentaTransferencia { clientErrorDetail = "No se logro consultar las tasas de la operación.", error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new OrdenVentaTransferencia { clientErrorDetail = "No se logro consultar las tasas de conversión.", error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorVenta ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorCompra.ToString();
                }



                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenVentaTransferencia { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }
                        foreach (var item in orden.tarifas.Where(x => x.IsRate))
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        orden.ClientSurnames = string.Concat(nc1, " ", nc2);
                        orden.ClientNames = string.Concat(nc3, " ", nc4);

                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;
                        #region Order Insert
                        IAnguloLopezBuilder builder = new AnguloLopezBuilder();
                        Solicitudes objSolicitud = new Solicitudes
                        {
                            DETALLE_TIPO_OPERACION = idDetalleTipoOperacion,
                            CLIENTE = orden.idClienteUnique,
                            SUCURSAL = 16,
                            STATUS_SOLICITUD = orden.MixedOperation ? 6 : 1,
                            MONEDA = idMoneda,
                            OFICINA = idOficina,
                            PAIS_DESTINO = orden.CountryCodeIso,
                            CORRESPONSAL = orden.idCorresponsal,
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = orden.montoOrden,
                            MONTO_CAMBIO = (changeAmount) * oficInfo.tasa,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            TASA_DESTINO = oficInfo.tasa,

                            NOMBRES_REMITENTE = nc1 + " " + nc2,
                            APELLIDOS_REMITENTE = nc3 + " " + nc4,
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = nb1 + " " + nb2,
                            APELLIDOS_BENEFICIARIO = nb3 + " " + nb4,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.nombreBancoDestino,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaDestino,
                            DIRECCION_BANCO = orden.direccionBancoDestino,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = idPagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            BANCO_INTERMEDIARIO = orden.bancoIntermediario,
                            NUMERO_CUENTA_INTERMEDIARIO = orden.numeroCuentaIntermediario,
                            DIRECCION_BANCO_INTERMEDIARIO = orden.direccionBancoIntermediario,
                            ABA_INTERMEDIARIO = orden.abaIntermediario,
                            SWIFT_INTERMEDIARIO = orden.swiftIntermediario,
                            IBAN_INTERMEDIARIO = orden.ibanIntermediario,
                            USUARIO_TAQUILLA = user.login,
                            TasaConversion = decimal.Parse(string.IsNullOrEmpty(orden.TasaConversion) ? orden.tasaCambio.ToString() : orden.TasaConversion),
                            MonedaOperacion = orden.MonedaConversion,
                            MontoConversion = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            REFERENCIA_ORDEN = referenciaBCV,
                            CommissionUsd = comisionUs,
                            CommissionBs = comisionBs,
                            MontoAprobado = orden.MixedOperation ? orden.montoOrden : 0,
                            ExchangeDifferential = orden.MixedOperation ? orden.tarifas.FirstOrDefault(x => x.InternalId == "ReferenceInformation" && !x.IsRate).valor : 0,

                            TypeAccountBank = orden.TypeAccountBank

    };

                        var resultInsert = builder.InsertSolicitudes(objSolicitud);
                        if (!resultInsert.Error)
                        {
                            var _idOrden = resultInsert.ReturnId;
                            if (orden.tarifas == null)
                            {
                                var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                            foreach (var item in orden.tarifas.Where(x => x.moneda == "USD" && x.IsRate).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda == "VEB" && x.IsRate).ToList())
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                            }
                        }
                        else
                        {
                            var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultInsert.ErrorMessage) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = resultInsert.ReturnId;
                    }
                }
                else
                {
                    var ret = new OrdenVentaTransferencia { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenVentaTransferencia { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenVentaEfectivo setVentaDivisasEfectivo(OrdenVentaEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "venta-simadi-taq";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 7; //tipo de operacion tabla temporal, para las ventas de divisas por efectivo.
                var idDetalleTipoOperacion = 9; //para las ventas
                var idMoneda = 210; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion

                if (orden.MonedaOperacion != 213)
                {
                    //orden.montoOrden = orden.montoOrden / decimal.Parse(orden.TasaConversion==""?"0": orden.TasaConversion) ;
                    orden.montoOrden = (orden.montoOrden * decimal.Parse(orden.TasaConversion)) / orden.tasaCambio;
                }
                else
                {
                    orden.MontoConversion = orden.montoOrden.ToString();
                    orden.TasaConversion = orden.tasaCambio.ToString();
                }



                //Primer paso, validar si el BCV aprueba la operación
                //var responseBCV = Services.financiero.SetVentaDivisasSIMADITest(tipoMovimiento,
                //               string.Format("{0}{1}", orden.tipoIdCliente, numeroId),
                //                orden.nombresCliente,
                //                orden.montoOrden,
                //                orden.tasaCambio, "840", orden.montoOrden,
                //                Convert.ToInt64(orden.motivoOferta),
                //                n, //string.IsNullOrEmpty(orden.numeroCuentaCliente) ? "01020000000000000000" : orden.numeroCuentaCliente,
                //                string.IsNullOrEmpty(orden.telefonoCliente) ? "04141111111" : orden.telefonoCliente,
                //                string.IsNullOrEmpty(orden.emailCliente) ? "sincorreo@gmail.com" : orden.emailCliente);
                //if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                //{
                //    responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //}
                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //var responseBCV =  string.Format("BCV{0:YYYYMMddHHMISS}", DateTime.Now);
                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        //var ret = new OrdenVentaEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        //return ret;
                        //var responseBCV = new XmlDocument();
                        responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                    }
                    else
                    {
                        orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                        orden.tipoIdBeneficiario = "R";
                        orden.numeroIdBeneficiario = "302075661";

                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        //var referenciaBCV = "dsfffddfdf21211";
                        var num = Services.utilitarios.GetProximoNumero(4, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion

                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }

                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }

                        #region Informacion de la Oficina Seleccionada
                        //XDocument xd = new XDocument();
                        //xd = XDocument.Parse(Services.catalogos.GetDetalleOficina(orden.idOficina, n).OuterXml);
                        //var ofic = (from r in xd.Descendants("RESULTADO")
                        //            select new Oficina
                        //            {
                        //                id = Convert.ToInt32(r.Element("id").Value),
                        //                nombre = r.Element("nombre").Value,
                        //                pagador = r.Element("pagador").Value,
                        //                deposito = r.Element("deposito").Value == "0" ? false : true,
                        //                corresponsal = r.Element("corresponsal").Value
                        //            }).OrderBy(x => x.nombre).FirstOrDefault();
                        var ofic = new Oficina();
                        ofic.pagador = "CAL"; //pagador por defecto para este tipo de operaciones
                        ofic.corresponsal = "CAL"; //pagador por defecto para este tipo de operaciones
                        ofic.id = user.idOficinaExterna;
                        orden.idCiudadDestino = user.idCiudadExterna;
                        orden.emailBeneficiario = n;
                        orden.telefonoBeneficiario = n;
                        orden.idPaisDestino = "VZL";
                        #endregion

                        #region Order Insert
                        var resultOrder = Services.miscelaneos.SetRegistrarOperacionTemporal(
                             user.letraSucursal, _operationNumber, string.Format("{0}{1}", orden.tipoIdCliente, orden.numeroIdCliente),
                             nc1, nc2, nc3, nc4, n,
                             n, nb1, nb2, nb3, nb4,
                             string.Format("{0}{1}", orden.tipoIdBeneficiario, orden.numeroIdBeneficiario),
                             n, orden.telefonoBeneficiario, string.Empty, orden.idPaisDestino,
                             ofic.pagador, orden.idCiudadDestino, ofic.id,
                             montoBolivares, orden.montoOrden - comisionUs,
                             1, orden.montoOrden - comisionUs, comisionUs,
                             comisionBs, 0, 0, n, n, user.login,
                             codigoTipoOperacion, referenciaBCV,
                             1, idTipoOperacion, 0, 0, 0, _idDatosDeposito, 0, "",
                             orden.idCliente, 0, true, false, false, 0, n, n,
                             orden.tasaCambio, idDetalleTipoOperacion, idMoneda,
                             user.idOficinaNew, ofic.corresponsal,
                             orden.fechaValorTasa, Convert.ToInt32(orden.motivoOferta), tipoMovimiento,
                             orden.bancoCliente, orden.numeroCuentaCliente, orden.emailCliente, orden.emailBeneficiario,
                             n, "0", n, n, n, n, n, n, n, n, n, n, orden.MonedaOperacion, decimal.Parse(orden.TasaConversion), decimal.Parse(orden.MontoConversion), "");
                        /*
                            n, n, n, n, n, n
                          string BANCO_INTERMEDIARIO, 
                          string NUMERO_CUENTA_INTERMEDIARIO,
                          string DIRECCION_BANCO_INTERMEDIARIO,
                          string ABA_INTERMEDIARIO,
                          string SWIFT_INTERMEDIARIO,
                          string IBAN_INTERMEDIARIO
                       */
                        if (resultOrder.FirstChild != resultOrder.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(resultOrder.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                            {
                                var _idOrden = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());

                                if (orden.tarifas == null)
                                {
                                    var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                    return ret;
                                }


                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                                {
                                    decimal _comisionUs = 0;
                                    if (item.valor < 1)
                                        _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                    else
                                        _comisionUs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                                }

                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                                {
                                    decimal _comisionBs = 0;

                                    if (item.valor < 1)
                                        _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                    else
                                        _comisionBs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);

                                }
                            }
                            else
                            {
                                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultOrder.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultOrder.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                    }
                }
                else
                {
                    var ret = new OrdenVentaEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenVentaEfectivo InsertCurrencySale(OrdenVentaEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "venta-simadi-taq";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 7; //tipo de operacion tabla temporal, para las ventas de divisas por efectivo.
                var idDetalleTipoOperacion = 9; //para las ventas
                var idMoneda = 213; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                if (orden.MonedaConversion == 0)
                    orden.MonedaConversion = 221;

                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                var operationCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaOperacion).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                if (operationCoins == null)
                {
                    return new OrdenVentaEfectivo { clientErrorDetail = "No se logro consultar las tasas de la operación.", error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new OrdenVentaEfectivo { clientErrorDetail = "No se logro consultar las tasas de conversión.", error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorVenta ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorCompra.ToString();
                }


                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                    }
                    else
                    {
                        orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                        orden.tipoIdBeneficiario = "R";
                        orden.numeroIdBeneficiario = "302075661";
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion

                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }

                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }

                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;

                        #region Informacion de la Oficina Seleccionada
                        var ofic = new Oficina();
                        ofic.pagador = "CAL"; //pagador por defecto para este tipo de operaciones
                        ofic.corresponsal = "CAL"; //pagador por defecto para este tipo de operaciones
                        ofic.id = user.idOficinaExterna;
                        orden.idCiudadDestino = user.idCiudadExterna;
                        orden.emailBeneficiario = n;
                        orden.telefonoBeneficiario = n;
                        orden.idPaisDestino = "VE";
                        orden.ClientSurnames = string.Concat(nc1, " ", nc2);
                        orden.ClientNames = string.Concat(nc3, " ", nc4);
                        #endregion

                        #region Order Insert
                        IAnguloLopezBuilder builder = new AnguloLopezBuilder();
                        Solicitudes objSolicitud = new Solicitudes
                        {
                            DETALLE_TIPO_OPERACION = idDetalleTipoOperacion,
                            CLIENTE = orden.idClienteUnique,
                            SUCURSAL = 16,
                            STATUS_SOLICITUD = 1,
                            MONEDA = orden.MonedaOperacion,
                            OFICINA = ofic.id,
                            PAIS_DESTINO = orden.idPaisDestino,
                            CORRESPONSAL = ofic.corresponsal,
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = changeAmount,
                            MONTO_CAMBIO = changeAmount,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            //NUMERO_CUENTA_DESTINO = orden.numeroCuentaCliente,
                            NOMBRES_REMITENTE = nc3 + " " + nc4,
                            APELLIDOS_REMITENTE = nc1 + " " + nc2,
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = nb1 + " " + nb2,
                            APELLIDOS_BENEFICIARIO = nb3 + " " + nb4,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            //NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            OBSERVACIONES = orden.observaciones,
                            //BANCO_DESTINO = orden.bancoCliente,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = ofic.pagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            USUARIO_TAQUILLA = user.login,
                            TasaConversion = decimal.Parse(orden.TasaConversion),
                            MonedaOperacion = orden.MonedaConversion,
                            MontoConversion = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            REFERENCIA_ORDEN = referenciaBCV,
                            CommissionUsd = comisionUs,
                            BancoPagoTransferencia = orden.bancoCliente,
                            NumeroCuentaPagoTransferencia = orden.numeroCuentaCliente,
                            CommissionBs = comisionBs
                        };
                        var resultInsert = builder.InsertSolicitudes(objSolicitud);
                        if (!resultInsert.Error)
                        {
                            var _idOrden = resultInsert.ReturnId;

                            if (orden.tarifas == null)
                            {
                                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);

                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);

                            }
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultInsert.ErrorMessage) };
                            return ret;
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = resultInsert.ReturnId;
                    }
                }
                else
                {
                    var ret = new OrdenVentaEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenVentaEfectivo setVentaDivisasCorresponsal(OrdenVentaEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "venta-simadi-enc";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 9; //tipo de operacion tabla temporal, para las ventas de divisas pr corresponsal.
                var idDetalleTipoOperacion = 9; //para las ventas
                var idMoneda = 210; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion

                //Primer paso, validar si el BCV aprueba la operación
                //var responseBCV = Services.financiero.SetVentaDivisasSIMADITest(tipoMovimiento,
                //               string.Format("{0}{1}", orden.tipoIdCliente, numeroId),
                //                orden.nombresCliente,
                //                orden.montoOrden,
                //                orden.tasaCambio, "840", orden.montoOrden,
                //                Convert.ToInt64(orden.motivoOferta),
                //                n, //string.IsNullOrEmpty(orden.numeroCuentaCliente) ? "01020000000000000000" : orden.numeroCuentaCliente.ToString(),
                //                string.IsNullOrEmpty(orden.telefonoCliente) ? "04141111111" : orden.telefonoCliente,
                //                string.IsNullOrEmpty(orden.emailCliente) ? "sincorreo@gmail.com" : orden.emailCliente);
                //if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                //{
                //    //responseBCV = new XmlDocument();
                //    responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //}
                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenVentaEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        //orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                        //orden.tipoIdBeneficiario = "R";
                        //orden.numeroIdBeneficiario = "302075661";
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        //var referenciaBCV = "dsdffsfs";
                        var num = Services.utilitarios.GetProximoNumero(4, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion

                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;
                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }
                        foreach (var item in orden.tarifas)
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }


                        #region Informacion de la Oficina Seleccionada
                        XDocument xd = new XDocument();
                        xd = XDocument.Parse(Services.catalogos.GetDetalleOficina(orden.idOficina, n).OuterXml);
                        var ofic = (from r in xd.Descendants("RESULTADO")
                                    select new Oficina
                                    {
                                        id = Convert.ToInt32(r.Element("id").Value),
                                        nombre = r.Element("nombre").Value,
                                        pagador = r.Element("pagador").Value,
                                        deposito = r.Element("deposito").Value == "0" ? false : true,
                                        corresponsal = r.Element("corresponsal").Value,
                                        tasa = Convert.ToDecimal(r.Element("tasa").Value, wsCulture)
                                    }).OrderBy(x => x.nombre).FirstOrDefault();
                        //var ofic = new Oficina();
                        //ofic.pagador = "CAL"; //pagador por defecto para este tipo de operaciones
                        //ofic.corresponsal = "CAL"; //pagador por defecto para este tipo de operaciones
                        //ofic.id = user.idOficinaExterna;
                        //orden.idCiudadDestino = user.idCiudadExterna;
                        //orden.emailBeneficiario = n;
                        //orden.telefonoBeneficiario = n;
                        //orden.idPaisDestino = "VZL";
                        #endregion

                        #region Order Insert
                        var resultOrder = Services.miscelaneos.SetRegistrarOperacionTemporal(
                             user.letraSucursal, _operationNumber, string.Format("{0}{1}", orden.tipoIdCliente, orden.numeroIdCliente),
                             nc1, nc2, nc3, nc4, n,
                             n, nb1, nb2, nb3, nb4,
                             string.Format("{0}{1}", orden.tipoIdBeneficiario, orden.numeroIdBeneficiario),
                             n, orden.telefonoBeneficiario, string.Empty, orden.idPaisDestino,
                             ofic.pagador, orden.idCiudadDestino, ofic.id,
                             montoBolivares, orden.montoOrden - comisionUs,
                             ofic.tasa, (orden.montoOrden - comisionUs) * ofic.tasa, comisionUs,
                             comisionBs, 0, 0, n, n, user.login,
                             codigoTipoOperacion, referenciaBCV,
                             1, idTipoOperacion, 0, 0, 0, _idDatosDeposito, 0, "",
                             orden.idCliente, 0, true, false, false, 0, n, n,
                             orden.tasaCambio, idDetalleTipoOperacion, idMoneda,
                             user.idOficinaNew, ofic.corresponsal,
                             orden.fechaValorTasa, Convert.ToInt32(orden.motivoOferta), tipoMovimiento,
                             orden.bancoCliente, orden.numeroCuentaCliente, orden.emailCliente, orden.emailBeneficiario,
                             n, "0", n, n, n, n, n, n, n, n, n, n, null, null, null, "");

                        if (resultOrder.FirstChild != resultOrder.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(resultOrder.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                            {
                                #region Insertar las Tarifas Aplicadas
                                var _idOrden = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                                if (orden.tarifas == null)
                                {
                                    var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                    return ret;
                                }
                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                                {
                                    decimal _comisionUs = 0;
                                    if (item.valor < 1)
                                        _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                    else
                                        _comisionUs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                                }

                                foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                                {
                                    decimal _comisionBs = 0;

                                    if (item.valor < 1)
                                        _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                    else
                                        _comisionBs = Math.Round(item.valor, 2);

                                    Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                                }

                                #endregion

                            }
                            else
                            {
                                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultOrder.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultOrder.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        #endregion
                        #endregion

                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                    }
                }
                else
                {
                    var ret = new OrdenVentaEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OrdenVentaEfectivo InsertSaleCorrespondent(OrdenVentaEfectivo orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "venta-simadi-enc";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 9; //tipo de operacion tabla temporal, para las ventas de divisas pr corresponsal.
                var idDetalleTipoOperacion = 9; //para las ventas
                var idMoneda = 213; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                if (orden.MonedaConversion == 0)
                    orden.MonedaConversion = 221;
                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                var operationCoins = infMonedas.Where(x => x.MonedaId == idMoneda).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                orden.AmmountTotalConversion = orden.tarifas.FirstOrDefault(x => x.InternalId == "AmmountConversion").valor;
                if (operationCoins == null)
                {
                    return new OrdenVentaEfectivo { clientErrorDetail = "No se logro consultar las tasas de la operación.", error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new OrdenVentaEfectivo { clientErrorDetail = "No se logro consultar las tasas de conversión.", error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorVenta ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorCompra.ToString();
                }

                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenVentaEfectivo { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion

                        #region Agregamos la información a la tabla temporal
                        #region Mapeo de Nombres en Variables
                        var nc = orden.nombresCliente.Split(' ');
                        switch (nc.Length)
                        {
                            case 5:
                                nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                                nc2 = nc[2];
                                nc3 = nc[3];
                                nc4 = nc[4];
                                break;
                            case 4:
                                nc1 = nc[0];
                                nc2 = nc[1];
                                nc3 = nc[2];
                                nc4 = nc[3];
                                break;
                            case 3:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = nc[2];
                                break;
                            case 2:
                                nc1 = nc[0];
                                nc2 = string.Empty;
                                nc3 = nc[1];
                                nc4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        var nb = orden.nombresBeneficiario.Split(' ');
                        switch (nb.Length)
                        {
                            case 5:
                                nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                                nb2 = nb[2];
                                nb3 = nb[3];
                                nb4 = nb[4];
                                break;
                            case 4:
                                nb1 = nb[0];
                                nb2 = nb[1];
                                nb3 = nb[2];
                                nb4 = nb[3];
                                break;
                            case 3:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = nb[2];
                                break;
                            case 2:
                                nb1 = nb[0];
                                nb2 = string.Empty;
                                nb3 = nb[1];
                                nb4 = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;
                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }
                        foreach (var item in orden.tarifas.Where(x => x.IsRate))
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }

                        orden.ClientSurnames = string.Concat(nc1, " ", nc2);
                        orden.ClientNames = string.Concat(nc3, " ", nc4);
                        #region Informacion de la Oficina Seleccionada
                        XDocument xd = new XDocument();
                        xd = XDocument.Parse(Services.catalogos.GetDetalleOficina(orden.idOficina, n).OuterXml);
                        var ofic = (from r in xd.Descendants("RESULTADO")
                                    select new Oficina
                                    {
                                        id = Convert.ToInt32(r.Element("id").Value),
                                        nombre = r.Element("nombre").Value,
                                        pagador = r.Element("pagador").Value,
                                        deposito = r.Element("deposito").Value == "0" ? false : true,
                                        corresponsal = r.Element("corresponsal").Value,
                                        tasa = Convert.ToDecimal(r.Element("tasa").Value, wsCulture)
                                    }).OrderBy(x => x.nombre).FirstOrDefault();
                        #endregion

                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;

                        #region Order Insert
                        IAnguloLopezBuilder builder = new AnguloLopezBuilder();
                        Solicitudes objSolicitud = new Solicitudes
                        {
                            DETALLE_TIPO_OPERACION = idTipoOperacion,
                            CLIENTE = orden.idClienteUnique,
                            SUCURSAL = 16,
                            STATUS_SOLICITUD = orden.MixedOperation ? 6 : 1,
                            MONEDA = idMoneda,
                            OFICINA = ofic.id,
                            PAIS_DESTINO = orden.CountryCodeIso,
                            CORRESPONSAL = ofic.corresponsal,
                            TASA_DESTINO = ofic.tasa,
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = orden.montoOrden,
                            MONTO_CAMBIO = (changeAmount) * ofic.tasa,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaCliente,
                            NOMBRES_REMITENTE = nc3 + " " + nc4,
                            APELLIDOS_REMITENTE = nc1 + " " + nc2,
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = nb1 + " " + nb2,
                            APELLIDOS_BENEFICIARIO = nb3 + " " + nb4,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            EMAIL_BENEFICIARIO = orden.emailBeneficiario,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.nombreBancoDestino,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_BENEFICIARIO = orden.telefonoBeneficiario,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = ofic.pagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            USUARIO_TAQUILLA = user.login,

                            TasaConversion = decimal.Parse(string.IsNullOrEmpty(orden.TasaConversion) ? orden.tasaCambio.ToString() : orden.TasaConversion),
                            MonedaOperacion = orden.MonedaConversion,
                            MontoConversion = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            REFERENCIA_ORDEN = referenciaBCV,
                            CommissionUsd = comisionUs,
                            CommissionBs = comisionBs,
                            MontoAprobado = orden.MixedOperation ? orden.montoOrden : 0,
                            ExchangeDifferential = orden.MixedOperation ? orden.tarifas.FirstOrDefault(x => x.InternalId == "ReferenceInformation" && !x.IsRate).valor : 0,

                            DIRECCION_BANCO = orden.DireccionBancoDestino,
                            TypeAccountBank = orden.TipoCuentaBeneficiario
                            
                        };
                        var resultInsert = builder.InsertSolicitudes(objSolicitud);
                        if (!resultInsert.Error)
                        {
                            #region Insertar las Tarifas Aplicadas
                            var _idOrden = resultInsert.ReturnId;
                            if (orden.tarifas == null)
                            {
                                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                            foreach (var item in orden.tarifas.Where(x => x.moneda == "USD" && x.IsRate))
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);

                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda == "VEB" && x.IsRate))
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);

                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                            }



                            #endregion
                        }
                        else
                        {
                            var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultInsert.ErrorMessage) };
                            return ret;
                        }
                        #endregion
                        #endregion

                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = resultInsert.ReturnId;
                        orden.OrderNumber = _operationNumber.ToString();
                    }
                }
                else
                {
                    var ret = new OrdenVentaEfectivo { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
            }
            catch (Exception ex)
            {
                var ret = new OrdenVentaEfectivo { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static OperacionDeNegocio InsertBusinessOperation(OperacionDeNegocio modelOperation)
        {
            ///Clase del Modelo a Insert 
            OperacionesPorCobrar model = new OperacionesPorCobrar();
            ///Clase Para Buscar Moendas
            MonedasRequest Monedareq = new MonedasRequest();
            ///Clase para Buscar Cliente
            Common.Models.Clientes.SearchClientsRequest clientsRequest = new Common.Models.Clientes.SearchClientsRequest();
            ///Clases Para Buscar Tarifas
            TarifaReq tarifaReq = new TarifaReq();

            try
            {
                #region Datos de Usuario
                model.Ciudad = modelOperation.IdCiudad;
                model.Oficina = modelOperation.IdOficinaExterna;
                model.CiuOrig = modelOperation.CiuOrig;
                model.Usuario = modelOperation.current;
                model.SUCURSAL_NEW = modelOperation.IdSucursal;
                #endregion

                #region Datos del Cliente

                /////Valores Para Buscar Cliente
                clientsRequest.id_cedula = modelOperation.CIREM; clientsRequest.clienteTipo = modelOperation.TipoDocumentoREM;
                clientsRequest.offSet = 0; clientsRequest.limit = 10;
                /////////////////////////////////////////
                IClientsBuilder builderclients = new ClientsBuilder();
                var clients = builderclients.Searchfichas(clientsRequest).FirstOrDefault();

                if (clients == null)
                {
                    var ret = new OperacionDeNegocio { clientErrorDetail = "Error No se ha podido encontrar al cliente suministrado", error = true, apiDetail = "InsertBusinessOperation" };
                    return (ret);
                }

                #endregion

                #region Seteo de Variables por Defecto

                long _idDatosDeposito = 0;
                long _operationNumber = 0;

                ///Datos del Cliente
                model.NomRemA = clients.PrimerNombre; model.NomRemB = clients.SegundoNombre;
                model.ApeRemA = clients.PrimerApellido; model.ApeRemB = clients.SegundoApellido;
                model.NombresRem = clients.NombreCompleto;
                model.TelRem = clients.TelfMovil1;
                model.DirRem = clients.direccion;
                model.EMAIL_CLIENTE = clients.Email;
                model.Cirem = clients.ClienteTipo.Trim() + clients.id_cedula.Trim();
                model.Observaciones = (string.IsNullOrEmpty(modelOperation.Observacion) ? string.Empty : modelOperation.Observacion);
                model.Mensaje = modelOperation.CodigoTipoOperacion;
                /////////////////////////////////////

                //Seteo de Variables vacios que puden cambiar su valor de acuerdo a la operacion a procesar

                ///Datos del Beneficiario
                model.NomDesA = string.Empty; model.NomDesB = string.Empty; model.ApeDesA = string.Empty; model.ApeDesB = string.Empty;
                model.Ccdes = string.Empty; model.DirDes = string.Empty; model.TelDes = string.Empty; model.Tel2Des = string.Empty;
                model.EMAIL_BENEFICIARIO = string.Empty; model.DirDes = string.Empty; model.Tel2Des = string.Empty;model.TypeAccountBank = modelOperation.TIPOCUENTADES;
                ////////////////////////////////////////////////
                ///Otros Datos
                model.Ficha = 0; model.Persona = 0; model.ReciboReTransmite = 0; model.ReTransmite = 0; model.ID_SCD = 0;
                model.ID_PROX_SOL = 0;
                model.Status = 48;// Status revision Prevencion
                model.Status_Temp = 3;
                model.TIPOSOL = "ve";
                model.PagoOtros = 0;
                model.PagoIsv = 0;
                ////////////////////////////////////////
                var ofic = new Oficina();
                ///Informacion de la Oficina 
                model.Pagador = "CAL"; //pagador por defecto para este tipo de operaciones
                model.CORRESPONSAL = "CAL"; //pagador por defecto para este tipo de operaciones
                /////////////////////////////////////////
                ///Pais de Destino
                if (modelOperation.PAISDES == string.Empty || modelOperation.PAISDES == null)
                {
                    model.Pais = "VZL";
                    modelOperation.PAISDES = "VZL";
                }
                else
                {
                    model.Pais = modelOperation.PAISDES;
                }
                /////////////////////////////////////
                ///Valores para Buscar Monedas
                Monedareq.MonedaActiva = true; Monedareq.TipoCambioId = null; Monedareq.MonedaInterna = null;
                ///////////////////////////////////
                var Monedas = SearchMonedas(Monedareq);
                var MonedaOperacion = Monedas.Where(x => x.MonedaId == modelOperation.IdMonedaOperacion).FirstOrDefault();
                var MonedaConversion = Monedas.Where(x => x.MonedaId == modelOperation.IdMonedaConversion).FirstOrDefault();
                model.MONEDA = MonedaOperacion.MonedaId;
                model.MonedaOperacion = MonedaConversion.MonedaId;
                ////////////////////////////////////////
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
                        ofic = GetDetalleOficina(model.Oficina);
                        //////////////////////////////////////////
                        //Valores de Tarifa
                        tarifaReq.idCorresp = modelOperation.OFICIANDES.ToString();
                        tarifaReq.OperationType = OperationType.Corresponsal;
                        ////////////////////////////  
                        model.CORRESPONSAL = ofic.corresponsal;
                        model.Pagador = ofic.pagador;
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
                        model.NomDesA = modelOperation.NOMDES1.ToUpper(); model.NomDesB = (string.IsNullOrEmpty(modelOperation.NOMDES2) ? string.Empty : modelOperation.NOMDES2.ToUpper());
                        model.ApeDesA = modelOperation.APEDES1.ToUpper(); model.ApeDesB = (string.IsNullOrEmpty(modelOperation.APEDES2) ? string.Empty : modelOperation.APEDES2.ToUpper());
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

                #region Busqueda de Tarifa              

                ///Buscar Tipo de Movimiento
                var tm = getTiposMovimientos(modelOperation.TipoDocumentoREM, modelOperation.CodigoTipoOperacion);
                //////////////////////////////////////////

                ///Valores por Defecto para Buscar Tarifas
                tarifaReq.moneda = MonedaOperacion.MonedaCodigoInt;
                tarifaReq.idPais = modelOperation.PAISDES;
                tarifaReq.MonedaOperacion = MonedaOperacion.MonedaId;
                tarifaReq.montoEnviar = modelOperation.montoOrden;
                tarifaReq.MonedaConversion = MonedaConversion.MonedaId;
                tarifaReq.tipoId = clients.ClienteTipo;
                tarifaReq.tipoOperacion = tm.FirstOrDefault().id.ToString();
                tarifaReq.tasa = modelOperation.TasaCambio;
                ////////////////////////////////////////
                /// Buscar Tarifa
                var FinancialSumary = SearchFinancialSummary(tarifaReq);
                model.MontoConversion = Math.Round(FinancialSumary.AmmountConversion);
                var tarifas = FinancialSumary.RateOperation.ToList();
                //////////////////////////////////////////
                #endregion
                ///Tipo de Movimiento o Detalle Tipo de Operacion/Operacion codigo Homologado del BCV/Motivo de Operacion
                var tipoMovimiento = tm.FirstOrDefault().idBcv;
                var idDetalleTipoOperacion = Convert.ToInt32(tm.FirstOrDefault().id);
                model.TIPO_OP_BCV = tipoMovimiento;
                model.MOTIVO_OP_BCV = modelOperation.IdMotivosOperacion;
                model.DETALLE_TIPO_OPERACION = idDetalleTipoOperacion;
                ///////////////////////////////////////

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
                    model.MonedaDest = (modelOperation.montoOrden - comisionUs) * ofic.tasa;
                }
                else
                {
                    model.MonedaDest = modelOperation.montoOrden - comisionUs;
                }
                #endregion

                #endregion

                #region Aprobacion de operacion BCV

                XmlNode responseBCV;
                var referenciaBCV = string.Empty;
                ///Valido La operacion en BCV          
                responseBCV = SetOperacionBCV(model);
                ///////////////////////////////////

                ///Verifica la Respuesta Recibida
                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OperacionDeNegocio { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                    else
                    {
                        #region Asignación Numero de Orden

                        referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? String.Empty : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(4, modelOperation.BranchOffice, model.Usuario, false, DateTime.Now.ToString(), "");
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                            {
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            }
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                            return ret;

                        }
                        if (_operationNumber == 0)
                        {
                            var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception("[Numeración en cero (0)]") };
                            return ret;
                        }
                        #endregion

                        #region Información de Depósito

                        if (modelOperation.CodigoTipoOperacion == "venta-simadi-trf")
                        {
                            var _resultadoFirstInsertXml = Services.operaciones.SetRegistrarCambioDatos(model.CiuOrig,
                                                    _operationNumber, modelOperation.NOMDES,
                                                    modelOperation.CIDES, "A", modelOperation.NUNCUENTADES.ToString(), modelOperation.BANCODES, modelOperation.DIRBANCODES,
                                                    string.Format("ABA: {0}, SWIFT: {1}, IBAN: {2}", modelOperation.ABA, modelOperation.SWIFT, modelOperation.IBAN),
                                                    model.Usuario, 1, 0, model.Usuario, "APROBADO AUTOMATICAMENTE SEGUN INFORMACION SUMINISTRADA POR EL CLIENTE", 1, 0, "");
                            if (_resultadoFirstInsertXml.FirstChild == _resultadoFirstInsertXml.SelectSingleNode("ERROR"))
                            {
                                var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de registrar la información de la transferencia \n [" + _resultadoFirstInsertXml.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception(_resultadoFirstInsertXml.SelectSingleNode("ERROR").InnerText) };
                                return ret;
                            }
                            else
                            {
                                if (Convert.ToBoolean(_resultadoFirstInsertXml.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                                    _idDatosDeposito = Convert.ToInt64(_resultadoFirstInsertXml.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                                else
                                {
                                    var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de registrar la información de la transferencia.", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception("Sin Id de Insert") };
                                    return ret;
                                }
                            }
                        }

                        #endregion

                    }
                }

                #endregion

                model.NroRecibo = Convert.ToInt32(_operationNumber);
                model.NumeroOpTemporal = Convert.ToInt64(_operationNumber);
                modelOperation.Numero = Convert.ToInt64(_operationNumber);
                model.Cadivi = referenciaBCV;
                if (_idDatosDeposito != 0)
                {
                    model.NUMERO_CUENTA_DESTINO = _idDatosDeposito.ToString();
                }    
                IOperacionesBuilder builder = new OperacionesBuilder();

                ///Insert de Modelo en Tabla
                var result = builder.InsertOperacionesPorCobrar(model);
                //////////////////////////////////////////////////
                ///Id Retornado de BDD
                modelOperation.IdOperacion = result.ReturnId;
                model.Id_OPERACION = result.ReturnId;
                ///////////////////////////////////////////////////////
                ///Verifico si se realizo el insert
                if (modelOperation.IdOperacion != 0)
                {
                    #region Insert Tarifas Aplicadas Operacion
                    ///Se insertan tarifas aplicadas de la operacion realizada
                    ///Con el ID generado de la Tabla Temporal OPERACIONES_POR_COBRAR
                    setTarifasAplicadas(modelOperation, tarifas, model.Usuario);
                    ////////////////////////////////////////////////
                    #endregion

                    #region Actualización Parcial del registro de pago enviado por el corresponsal

                    if (modelOperation.CodigoTipoOperacion == "compra-simadi-enc")
                    {
                        var resultUpdate = Services.pagos.SetInfoRemesaEntrante(0, model.Cadivi, Convert.ToInt32(model.Id_OPERACION), DateTime.Now, modelOperation.IdMonedaOperacion, string.Empty);
                        if (resultUpdate.FirstChild != resultUpdate.SelectSingleNode("ERROR"))
                        {
                            if (!Convert.ToBoolean(resultUpdate.SelectSingleNode("//ACTUALIZADO").InnerText.Trim()))
                            {
                                var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultUpdate.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception(resultUpdate.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                    }

                    #endregion

                }
                else
                {
                    var ret = new OperacionDeNegocio { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden: \n [" + result.Error + "].", error = true, apiDetail = "InsertBusinessOperation", errorDetail = new Exception(result.ErrorMessage) };
                    return ret;
                }
                model.error = false;
                return modelOperation;
            }
            catch (Exception ex)
            {
                var ret = new OperacionDeNegocio { clientErrorDetail = "Error al procesar la operacion", error = true, apiDetail = "InsertBusinessOperation", errorDetail = ex };
                return ret;
            }
        }

        public static XmlNode SetOperacionBCV(OperacionesPorCobrar model)
        {
            try
            {
                #region Variables

                string tipoIdentificacion = string.Empty;
                string numeroIdentificacion = string.Empty;
                bool TestMode = true;//En produccion colocar en false
                XmlNode responseBCV;

                ///////////////////////////////////////
                /*Se realiza el Return de este XML cuando el BCV retorna Error.El servicio esta inactivo cuando sea reactivado el servicio 
                 comentar los return con esta variable*/
                var responseBCVInt = new XmlDocument();
                responseBCVInt.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                tipoIdentificacion = model.Cirem.Substring(0, 1);
                numeroIdentificacion = model.Cirem.Replace(tipoIdentificacion, "");

                #endregion
                if (TestMode)
                {
                    if (model.Tipo_Operacion == 8) //Compra
                    {
                        responseBCV = Services.financiero.SetCompraDivisasSIMADITest(model.TIPO_OP_BCV,
                                     string.Format("{0}{1}", tipoIdentificacion, numeroIdentificacion),
                                      model.NombresRem,
                                      model.Dolares,
                                      Convert.ToInt32(model.TasaConversion), "840", model.Dolares,
                                      Convert.ToInt64(model.MOTIVO_OP_BCV),
                                      string.IsNullOrEmpty(model.NUMERO_CUENTA) ? "01020000000000000000" : model.NUMERO_CUENTA.ToString(),
                                      string.IsNullOrEmpty(model.TelRem) ? "04141111111" : model.TelRem.Replace("(","").Replace(")","").Replace("-","").Replace(" ","").Trim(),
                                      string.IsNullOrEmpty(model.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : model.EMAIL_CLIENTE);
                    }
                    else //venta
                    {
                        responseBCV = Services.financiero.SetVentaDivisasSIMADITest(model.TIPO_OP_BCV,
                                    string.Format("{0}{1}", tipoIdentificacion, numeroIdentificacion),
                                      model.NombresRem,
                                      model.Dolares,
                                      Convert.ToInt32(model.TasaConversion), "840", model.Dolares,
                                      Convert.ToInt64(model.MOTIVO_OP_BCV),
                                      string.IsNullOrEmpty(model.NUMERO_CUENTA) ? "01020000000000000000" : model.NUMERO_CUENTA.ToString(),
                                      string.IsNullOrEmpty(model.TelRem) ? "04141111111" : model.TelRem.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim(),
                                      string.IsNullOrEmpty(model.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : model.EMAIL_CLIENTE);
                    }

                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        //responseBCV = new XmlDocument();
                        responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                        return responseBCVInt;
                    }
                    return responseBCV;
                }
                else
                {
                    if (model.Tipo_Operacion == 8) //Compra
                    {
                        responseBCV = Services.financiero.SetCompraDivisasSIMADI(model.TIPO_OP_BCV,
                                     string.Format("{0}{1}", tipoIdentificacion, numeroIdentificacion),
                                      model.NombresRem,
                                      model.Dolares,
                                      Convert.ToInt32(model.TasaConversion), "840", model.Dolares,
                                      Convert.ToInt64(model.MOTIVO_OP_BCV),
                                      string.IsNullOrEmpty(model.NUMERO_CUENTA) ? "01020000000000000000" : model.NUMERO_CUENTA.ToString(),
                                      string.IsNullOrEmpty(model.TelRem) ? "04141111111" : model.TelRem.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim(),
                                      string.IsNullOrEmpty(model.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : model.EMAIL_CLIENTE);
                    }
                    else //venta
                    {
                        responseBCV = Services.financiero.SetVentaDivisasSIMADI(model.TIPO_OP_BCV,
                                    string.Format("{0}{1}", tipoIdentificacion, numeroIdentificacion),
                                      model.NombresRem,
                                      model.Dolares,
                                      Convert.ToInt32(model.TasaConversion), "840", model.Dolares,
                                      Convert.ToInt64(model.MOTIVO_OP_BCV),
                                      string.IsNullOrEmpty(model.NUMERO_CUENTA) ? "01020000000000000000" : model.NUMERO_CUENTA.ToString(),
                                      string.IsNullOrEmpty(model.TelRem) ? "04141111111" : model.TelRem.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim(),
                                      string.IsNullOrEmpty(model.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : model.EMAIL_CLIENTE);
                    }

                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        //responseBCV = new XmlDocument();
                        responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                        return responseBCVInt;
                    }
                    return responseBCV;
                }

            }
            catch (Exception ex)
            {
                var responseErr = new XmlDocument();
                responseErr.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                return responseErr;
            }
        }
        public static void setTarifasAplicadas(OperacionDeNegocio model, List<Common.Models.Common.Tarifa> tarifas, string user)
        {
            var Tarifa = tarifas.Where(x => x.moneda != null);
            foreach (var item in Tarifa.Where(x => x.moneda.Equals("USD")))
            {
                decimal _comisionUs = 0;
                if (item.valor < 1)
                    _comisionUs = Math.Round(item.valor * model.montoOrden, 2);
                else
                    _comisionUs = Math.Round(item.valor, 2);

                Services.ordenes.setTarifasAplicadasOrden(Convert.ToInt64(model.Numero), item.idTarifa, _comisionUs, user);
            }

            foreach (var item in Tarifa.Where(x => x.moneda.Equals("VEB")))
            {
                decimal _comisionBs = 0;

                if (item.valor < 1)
                    _comisionBs = Math.Round(item.valor * Math.Round((model.montoOrden * model.TasaCambio), 2), 2);
                else
                    _comisionBs = Math.Round(item.valor, 2);

                Services.ordenes.setTarifasAplicadasOrden(Convert.ToInt64(model.Numero), item.idTarifa, _comisionBs, user);

            }
        }

        public static XmlNode RegistraOperacionBCV(Solicitudes orden)
        {
            try
            {
                #region Variables

                string tipoIdentificacion = string.Empty;
                string numeroIdentificacion = string.Empty;
                XmlNode responseBCV;
                tipoIdentificacion = orden.IDENTIFICACION_REMITENTE.Substring(0, 1);
                numeroIdentificacion = orden.IDENTIFICACION_REMITENTE.Replace(tipoIdentificacion, "");

                #endregion

                if (orden.DETALLE_TIPO_OPERACION == 8) //Compra
                {
                    responseBCV = Services.financiero.SetCompraDivisasSIMADI(orden.TIPO_OP_BCV,
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
                    responseBCV = Services.financiero.SetVentaDivisasSIMADI(orden.TIPO_OP_BCV,
                                orden.IDENTIFICACION_REMITENTE,
                                orden.NOMBRES_REMITENTE,
                                orden.MONTO,
                                orden.TasaConversion.Value, "840", orden.MONTO,
                                Convert.ToInt64(orden.MOTIVO_OP_BCV), string.Empty,
                                string.IsNullOrEmpty(orden.TELEFONO_CLIENTE) ? "04141111111" : orden.TELEFONO_CLIENTE,
                                string.IsNullOrEmpty(orden.EMAIL_CLIENTE) ? "sincorreo@gmail.com" : orden.EMAIL_CLIENTE);
                }


                if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                {
                    //responseBCV = new XmlDocument();
                    responseBCV.OwnerDocument.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
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

        public static TarifaResult getResumenFinanciero(TarifaReq req)
        {
            try
            {
                var tm = getTiposMovimientos(req.tipoId, "venta-simadi-trf");
                var mov = tm.Where(x => x.id == req.tipoOperacion).FirstOrDefault();
                decimal totalDispBs;
                decimal totalDispUsd;

                //XDocument xdofic = new XDocument();
                //xdofic = XDocument.Parse(Services.catalogos.GetDetalleOficina(req.idPais, string.Empty).OuterXml);
                //var ofic = (from r in xdofic.Descendants("RESULTADO")
                //            select new Oficina
                //            {
                //                id = Convert.ToInt32(r.Element("id").Value),
                //                nombre = r.Element("nombre").Value,
                //                pagador = r.Element("pagador").Value,
                //                deposito = r.Element("deposito").Value == "0" ? false : true,
                //                corresponsal = r.Element("corresponsal").Value
                //            }).OrderBy(x => x.nombre).FirstOrDefault();

                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(mov.requestTarifa, req.montoEnviar, req.moneda, req.idPais, "FURBANO", req.idCorresp, string.Empty).OuterXml);
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();

                var tar = new List<Tarifa>();
                decimal totalComisUS = 0;
                totalDispUsd = req.montoEnviar;
                foreach (var item in tarifas.Where(x => x.moneda.Equals("USD")).ToList()) //sacamos las comisiones en $
                {
                    string simbolo;
                    decimal val = 0;
                    //if (item.moneda.Equals("USD"))
                    //{
                    simbolo = "$";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * req.montoEnviar, 2);
                    else
                        val = Math.Round(item.valor, 2);
                    //totalComisUS += val;
                    totalDispUsd += -val;
                    //}
                    //else
                    //{
                    //    simbolo = "Bs";
                    //    if (item.valor < 1)
                    //        val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                    //    else
                    //        val = Math.Round(item.valor, 2);
                    //    totalDispBs += val;
                    //}
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }
                var total = Math.Round(req.montoEnviar * req.tasa, 2);
                //var total = Math.Round(totalDispUsd * req.tasa, 2);
                var valTotal = string.Format("{0:##,###,##0.00}", total);

                totalDispBs = Math.Round(req.montoEnviar * req.tasa, 2);
                //totalDispBs = Math.Round(totalDispUsd * req.tasa, 2);
                foreach (var item in tarifas.Where(x => !x.moneda.Equals("USD")).ToList()) //sacamos las comisiones en $
                {
                    string simbolo;
                    decimal val = 0;
                    //if (item.moneda.Equals("USD"))
                    //{
                    //    simbolo = "$";
                    //    if (item.valor < 1)
                    //        val = Math.Round(item.valor * req.montoEnviar, 2);
                    //    else
                    //        val = Math.Round(item.valor, 2);

                    //    totalDispUsd += val;
                    //}
                    //else
                    //{
                    simbolo = "Bs";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                    else
                        val = Math.Round(item.valor, 2);
                    totalDispBs += val;
                    //}
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }

                var totalBs = string.Format("{0:##,###,##0.00}", totalDispBs);
                var totalUsd = string.Format("{0:##,###,##0.00}", totalDispUsd);

                var result = new TarifaResult
                {
                    montoCobrar = total,
                    montoCobrar2 = valTotal,
                    tarifas = tar,
                    montoTotalBs = totalBs,
                    montoTotalUsd = totalUsd
                };
                if (totalDispUsd <= 0)
                {
                    result.error = true;
                    result.clientErrorDetail = "La comisión es mayor al monto de la operación.";
                }
                return result;
            }
            catch (Exception ex)
            {
                var ret = new TarifaResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static TarifaResult getResumenFinancieroVentaEncomienda(TarifaReq req)
        {
            try
            {
                var tm = getTiposMovimientos(req.tipoId, "venta-simadi-enc");
                var mov = tm.Where(x => x.id == req.tipoOperacion).FirstOrDefault();

                XDocument xdofic = new XDocument();
                xdofic = XDocument.Parse(Services.catalogos.GetDetalleOficina(Convert.ToInt64(req.idCorresp), string.Empty).OuterXml);
                var ofic = (from r in xdofic.Descendants("RESULTADO")
                            select new Oficina
                            {
                                id = Convert.ToInt32(r.Element("id").Value),
                                nombre = r.Element("nombre").Value,
                                pagador = r.Element("pagador").Value,
                                deposito = r.Element("deposito").Value == "0" ? false : true,
                                corresponsal = r.Element("corresponsal").Value
                            }).OrderBy(x => x.nombre).FirstOrDefault();

                decimal totalDispBs;
                decimal totalDispUsd;
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(mov.requestTarifa, req.montoEnviar, req.moneda, req.idPais, "FURBANO", ofic.corresponsal, string.Empty).OuterXml);
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();

                var tar = new List<Tarifa>();
                decimal totalComisUS = 0;
                totalDispUsd = req.montoEnviar;
                foreach (var item in tarifas.Where(x => x.moneda.Equals("USD")).ToList()) //sacamos las comisiones en $
                {
                    string simbolo;
                    decimal val = 0;
                    //if (item.moneda.Equals("USD"))
                    //{
                    simbolo = "$";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * req.montoEnviar, 2);
                    else
                        val = Math.Round(item.valor, 2);
                    //totalComisUS += val;
                    totalDispUsd += -val;
                    //}
                    //else
                    //{
                    //    simbolo = "Bs";
                    //    if (item.valor < 1)
                    //        val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                    //    else
                    //        val = Math.Round(item.valor, 2);
                    //    totalDispBs += val;
                    //}
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }
                var total = Math.Round(req.montoEnviar * req.tasa, 2);
                //var total = Math.Round(totalDispUsd * req.tasa, 2);
                var valTotal = string.Format("{0:##,###,##0.00}", total);

                totalDispBs = Math.Round(req.montoEnviar * req.tasa, 2);
                //totalDispBs = Math.Round(totalDispUsd * req.tasa, 2);
                foreach (var item in tarifas.Where(x => !x.moneda.Equals("USD")).ToList()) //sacamos las comisiones en $
                {
                    string simbolo;
                    decimal val = 0;
                    //if (item.moneda.Equals("USD"))
                    //{
                    //    simbolo = "$";
                    //    if (item.valor < 1)
                    //        val = Math.Round(item.valor * req.montoEnviar, 2);
                    //    else
                    //        val = Math.Round(item.valor, 2);

                    //    totalDispUsd += val;
                    //}
                    //else
                    //{
                    simbolo = "Bs";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                    else
                        val = Math.Round(item.valor, 2);
                    totalDispBs += val;
                    //}
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }

                var totalBs = string.Format("{0:##,###,##0.00}", totalDispBs);
                var totalUsd = string.Format("{0:##,###,##0.00}", totalDispUsd);

                var result = new TarifaResult
                {
                    montoCobrar = total,
                    montoCobrar2 = valTotal,
                    tarifas = tar,
                    montoTotalBs = totalBs,
                    montoTotalUsd = totalUsd
                };
                if (totalDispUsd <= 0)
                {
                    result.error = true;
                    result.clientErrorDetail = "La comisión es mayor al monto de la operación.";
                }
                return result;
            }
            catch (Exception ex)
            {
                var ret = new TarifaResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static TarifaResult getResumenFinancieroEfectivo(TarifaReq req)
        {
            try
            {
                decimal tasaOperacion = 0;
                string monedaOperacion = "";
                string fechaTasa = "";
                decimal montoOperacion = req.montoEnviar;
                if (req.MonedaOperacion != 213)
                {
                    var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                    if (infMonedas.Count() == 0)
                    {
                        return new TarifaResult
                        {
                            error = true,
                            clientErrorDetail = "No se ha localizado la tasa de conversión para la monedas seleccionadas"
                        };
                    }
                    else
                    {

                        var infMonedaOperacion = infMonedas.Where(x => x.MonedaId == req.MonedaOperacion).FirstOrDefault();
                        req.montoEnviar = ((req.montoEnviar * infMonedaOperacion.MonedaValorVentaReuters ?? 0));
                        tasaOperacion = decimal.Round(req.tasa * (decimal)infMonedaOperacion.MonedaValorVentaReuters, 4);// infMonedaOperacion.MonedaValorVenta ?? 0;
                        monedaOperacion = infMonedaOperacion.MonedaCodigoInt;
                        req.moneda = "USD";
                        fechaTasa = Convert.ToDateTime(infMonedaOperacion.MonedaDateModified).Year <= 1 ? infMonedaOperacion.MonedaDateCreate.ToString() : infMonedaOperacion.MonedaDateModified.ToString();

                    }
                }
                else
                {
                    tasaOperacion = req.tasa;
                    req.moneda = "USD";
                }


                var tm = getTiposMovimientos(req.tipoId, "venta-simadi-taq");

                var mov = tm.Where(x => x.id == req.tipoOperacion).FirstOrDefault();
                decimal totalDispBs;
                decimal totalDispUsd;
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(mov.requestTarifa, req.montoEnviar, monedaOperacion, req.idPais, "FURBANO", req.idCorresp, string.Empty).OuterXml);
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();

                var tar = new List<Tarifa>();
                totalDispBs = Math.Round(req.montoEnviar * req.tasa, 2);
                totalDispUsd = req.montoEnviar;
                foreach (var item in tarifas)
                {
                    string simbolo;
                    decimal val = 0;
                    if (item.moneda.Equals("USD"))
                    {
                        simbolo = "$";
                        if (item.valor < 1)
                            val = Math.Round(item.valor * req.montoEnviar, 2);
                        else
                            val = Math.Round(item.valor, 2);

                        totalDispUsd += -val;
                    }
                    else
                    {
                        simbolo = "Bs";
                        if (item.valor < 1)
                            val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                        else
                            val = Math.Round(item.valor, 2);
                        totalDispBs += val;
                    }
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }
                var total = Math.Round(req.montoEnviar * req.tasa, 2);
                var valTotal = string.Format("{0:##,###,##0.00}", total);

                var totalBs = string.Format("{0:##,###,##0.00}", totalDispBs);
                var totalUsd = string.Format("{0:##,###,##0.00}", totalDispUsd);

                var result = new TarifaResult
                {
                    montoCobrar = total,
                    montoCobrar2 = valTotal,
                    tarifas = tar,
                    montoTotalBs = totalBs,
                    montoTotalUsd = totalUsd,
                    MonedaValorOperacionVenta = string.Format("{0:##,###,####0.0000}", tasaOperacion),
                    //MontoTotalConversion = req.MonedaOperacion != 213?string.Format("{0:##,###,####0.0000}",(totalDispUsd * tasaOperacion)): string.Format("{0:##,###,####0.0000}",req.montoEnviar)
                    //MontoTotalConversion = req.MonedaOperacion != 213?string.Format("{0:##,###,####0.0000}",(totalDispUsd * tasaOperacion)): string.Format("{0:##,###,####0.0000}",req.montoEnviar)
                    MontoTotalConversion = string.Format("{0:##,###,##0.00}", montoOperacion),
                    FechaTasa = fechaTasa
                };
                if (totalDispUsd <= 0)
                {
                    result.error = true;
                    result.clientErrorDetail = "La comisión es mayor al monto de la operación.";
                }

                return result;
            }
            catch (Exception ex)
            {
                var ret = new TarifaResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static TarifaResult getResumenFinancieroCompraEfectivo(TarifaReq req)
        {
            try
            {
                decimal tasaOperacion = 0;
                string monedaOperacion = "";
                string fechaTasa = "";
                decimal montoOperacion = req.montoEnviar;
                if (req.MonedaOperacion != 213)
                {
                    var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                    if (infMonedas.Count() == 0)
                    {
                        return new TarifaResult
                        {
                            error = true,
                            clientErrorDetail = "No se ha localizado la tasa de conversión para la monedas seleccionadas"
                        };
                    }
                    else
                    {

                        var infMonedaOperacion = infMonedas.Where(x => x.MonedaId == req.MonedaOperacion).FirstOrDefault();
                        req.montoEnviar = ((req.montoEnviar * infMonedaOperacion.MonedaValorCompra ?? 0) / req.tasa);
                        tasaOperacion = infMonedaOperacion.MonedaValorCompra ?? 0;
                        req.moneda = "USD";
                        monedaOperacion = infMonedaOperacion.MonedaCodigoInt;
                        fechaTasa = Convert.ToDateTime(infMonedaOperacion.MonedaDateModified).Year <= 1 ? infMonedaOperacion.MonedaDateCreate.ToString() : infMonedaOperacion.MonedaDateModified.ToString();
                    }
                }
                else
                {
                    tasaOperacion = req.tasa;
                    req.moneda = "USD";
                }
                var tm = getTiposMovimientos(req.tipoId, "compra-simadi-taq");
                var mov = tm.Where(x => x.id == req.tipoOperacion).FirstOrDefault();
                decimal totalDispBs;
                decimal totalDispUsd;
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(mov.requestTarifa, req.montoEnviar, monedaOperacion, req.idPais, "FURBANO", req.idCorresp, string.Empty).OuterXml);
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();

                var tar = new List<Tarifa>();
                totalDispBs = Math.Round(req.montoEnviar * req.tasa, 2);
                totalDispUsd = req.montoEnviar;
                foreach (var item in tarifas)
                {
                    string simbolo;
                    decimal val = 0;
                    if (item.moneda.Equals("USD"))
                    {
                        simbolo = "$";
                        if (item.valor < 1)
                            val = Math.Round(item.valor * req.montoEnviar, 2);
                        else
                            val = Math.Round(item.valor, 2);

                        totalDispUsd += -val;
                    }
                    else
                    {
                        simbolo = "Bs";
                        if (item.valor < 1)
                            val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                        else
                            val = Math.Round(item.valor, 2);
                        totalDispBs += val;
                    }
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }
                var total = Math.Round(req.montoEnviar * req.tasa, 2);
                var valTotal = string.Format("{0:##,###,##0.00}", total);

                var totalBs = string.Format("{0:##,###,##0.00}", totalDispBs);
                var totalUsd = string.Format("{0:##,###,##0.00}", totalDispUsd);

                var result = new TarifaResult
                {
                    montoCobrar = total,
                    montoCobrar2 = valTotal,
                    tarifas = tar,
                    montoTotalBs = totalBs,
                    montoTotalUsd = totalUsd,
                    MonedaValorOperacionCompra = string.Format("{0:##,###,##0.0000}", tasaOperacion),
                    //MontoTotalConversion = req.MonedaOperacion != 213?string.Format("{0:##,###,####0.0000}",(totalDispUsd * tasaOperacion)): string.Format("{0:##,###,####0.0000}",req.montoEnviar)
                    MontoTotalConversion = string.Format("{0:##,###,##0.00}", montoOperacion),
                    FechaTasa = fechaTasa
                };
                if (totalDispUsd <= 0)
                {
                    result.error = true;
                    result.clientErrorDetail = "La comisión es mayor al monto de la operación.";
                }
                return result;
            }
            catch (Exception ex)
            {
                var ret = new TarifaResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static RateResult SearchFinancialSummary(TarifaReq req)
        {
            try
            {
                XDocument xdofic = new XDocument();
                decimal tasaOperacion = 0;
                decimal tasaConversion = 0;
                decimal exchangeDifferential = 0;
                string monedaOperacion = string.Empty;
                string fechaTasa = string.Empty;
                string operationType = string.Empty;
                string operationTypeConversion = string.Empty;
                string operationTypeConversionId = string.Empty;
                TarifaReq reqConversion;
                decimal montoOperacion = req.montoEnviar;
                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                decimal factor = 0;
                bool sale = true;
                bool saleConvertion = false;
                var sameCoin = req.MonedaOperacion == req.MonedaConversion;
                var rates = new List<Common.Models.Common.Tarifa>();
                //var rateConvertion = new List<Common.Models.Common.Tarifa>();
                var infMonedaConevrsion = infMonedas.Where(x => x.MonedaId == req.MonedaConversion).FirstOrDefault();
                if (infMonedaConevrsion == null)
                {
                    return new RateResult
                    {
                        error = true,
                        clientErrorDetail = "No se ha localizado la tasa de conversión para la moneda a recibir"
                    };
                }

                switch (req.OperationType)
                {
                    case OperationType.VentaEfectivo:
                        operationType = "venta-simadi-taq";
                        operationTypeConversion = "compra-simadi-taq";

                        break;
                    case OperationType.CompraEfectivo:
                        operationType = "compra-simadi-taq";
                        operationTypeConversion = "venta-simadi-taq";
                        sale = false;
                        saleConvertion = true;

                        break;
                    case OperationType.Corresponsal:
                        operationType = "venta-simadi-enc";
                        operationTypeConversion = "compra-simadi-taq";

                        IOficinasBuilder BuilderOficinas = new OficinasBuilder();
                        var idcorres = Convert.ToInt32(req.idCorresp);
                        var DetalleOficina = BuilderOficinas.SearchDetalleOficina(new DetalleOficinaRequest { idOficina = idcorres }).ToList();

                        var ofic = (from r in DetalleOficina
                                    select new Oficina
                                    {
                                        id = r.id,
                                        nombre = r.nombre,
                                        pagador = r.pagador,
                                        deposito = r.deposito,
                                        corresponsal = r.corresponsal
                                    }).OrderBy(x => x.nombre).FirstOrDefault();
                        
                        req.idCorresp = ofic.corresponsal;
                        break;
                    case OperationType.Transferencia:
                        operationType = "venta-simadi-trf";
                        operationTypeConversion = "compra-simadi-taq";
                        break;
                    default:
                        break;
                }
                tasaConversion = infMonedaConevrsion.MonedaValorCompra ?? 0;
                if (!sale)
                {
                    tasaConversion = infMonedaConevrsion.MonedaValorVenta ?? 0;
                }

                /*Diferente de Dolar*/
                if (req.MonedaOperacion != 213)
                {
                    if (infMonedas.Count() == 0)
                    {
                        return new RateResult
                        {
                            error = true,
                            clientErrorDetail = "No se ha localizado la tasa de conversión para la monedas seleccionadas"
                        };
                    }
                    else
                    {
                        var infMonedaOperacion = infMonedas.Where(x => x.MonedaId == req.MonedaOperacion).FirstOrDefault();
                        switch (req.OperationType)
                        {
                            case OperationType.VentaEfectivo:
                                factor = infMonedaOperacion.MonedaValorVenta ?? 0;
                                break;
                            case OperationType.CompraEfectivo:
                                factor = infMonedaOperacion.MonedaValorCompra ?? 0;
                                break;
                            case OperationType.Corresponsal:
                                factor = infMonedaOperacion.MonedaValorVenta ?? 0;
                                break;
                            case OperationType.Transferencia:
                                factor = infMonedaOperacion.MonedaValorVenta ?? 0;
                                break;
                            default:
                                break;
                        }
                        req.montoEnviar = ((req.montoEnviar * factor) / req.tasa);
                        tasaOperacion = factor;
                        req.moneda = "USD";
                        monedaOperacion = infMonedaOperacion.MonedaCodigoInt;
                        fechaTasa = Convert.ToDateTime(infMonedaOperacion.MonedaDateModified).Year <= 1 ? infMonedaOperacion.MonedaDateCreate.ToString() : infMonedaOperacion.MonedaDateModified.ToString();
                    }
                }
                else
                {
                    tasaOperacion = req.tasa;
                    req.moneda = "USD";
                }

                decimal totalDispBs = 0;
                decimal totalDispUsd = 0;
                decimal totalCommissionBs = 0;
                decimal totalCommissionUsd = 0;

                /*Añadimos tarifas con propositos visuales se coloca los titulos de tasa invertido de cara al cliente*/
                string titleTasa = "Tasa Venta";
                if (sale)
                    titleTasa = "Tasa Compra";

                rates.Add(new Common.Models.Common.Tarifa
                {
                    InternalId = "Rate",
                    IsRate = false,
                    concepto = titleTasa,
                    valor2 = string.Format("{0:##,###,##0.0000}", tasaOperacion),
                    valor = tasaOperacion,
                    Sale = sale,
                    simbolo = "Bs",
                });
                rates.Add(new Common.Models.Common.Tarifa
                {
                    InternalId = "AmountOperation",
                    IsRate = false,
                    concepto = "Monto Operación",
                    simbolo = "Bs",
                    valor2 = string.Format("{0:##,###,##0.00}", (montoOperacion * tasaOperacion)),
                    valor = (montoOperacion * tasaOperacion),
                    Sale = sale
                });
                SearchRate(req, operationType, monedaOperacion, ref rates, ref totalCommissionBs, ref totalCommissionUsd, sale, req.tasa);
                decimal totalAmountBuy = 0;
                if (sale)
                    totalAmountBuy = rates.Where(x => x.Sale && (x.InternalId == "CalculateInformation" || x.InternalId == "AmountOperation")).Sum(x => x.valor);
                else
                    totalAmountBuy = rates.FirstOrDefault(x => !x.Sale && x.InternalId == "AmountOperation").valor - rates.Where(x => !x.Sale && x.InternalId == "CalculateInformation" && x.moneda == "VEB").Sum(x => x.valor);
                rates.Add(new Common.Models.Common.Tarifa
                {
                    InternalId = "SubTotalAmountOperation",
                    IsRate = false,
                    concepto = "Monto Total Compra",
                    simbolo = "Bs",
                    valor2 = string.Format("{0:##,###,##0.00}", totalAmountBuy),
                    valor = totalAmountBuy,
                    Sale = sale
                });


                if (req.MonedaConversion != 221)
                {
                    ISimadiBuilder BuilderSimadi = new SimadiBuilder();
                    var TipoMovimientoRequest = new Common.Models.Angulo_Lopez.Simadi.TipoMovimientoRequest()
                    {
                        TipoOperacion = operationTypeConversion,
                        idTipoIdentidad = req.tipoId
                    };

                    var movementType = BuilderSimadi.SearchTiposMovimientos(TipoMovimientoRequest).FirstOrDefault();

                    if (movementType == null)
                    {
                        return new RateResult
                        {
                            error = true,
                            clientErrorDetail = "No se logro consultar el tipo de movimiento para la conversión."
                        };
                    }
                    reqConversion = req;
                    reqConversion.tipoOperacion = movementType.ID_TIPO.ToString();
                    /*Añadimos tarifas con propositos visuales*/
                    string titleTasaConversion = "Tasa Compra";
                    if (sale)
                        titleTasaConversion = "Tasa Venta";
                    rates.Add(new Common.Models.Common.Tarifa
                    {
                        InternalId = "Rate",
                        IsRate = false,
                        concepto = titleTasaConversion,
                        simbolo = "Bs",
                        valor2 = string.Format("{0:##,###,##0.0000}", tasaConversion),
                        Sale = saleConvertion
                    });
                    rates.Add(new Common.Models.Common.Tarifa
                    {
                        InternalId = "AmountOperation",
                        IsRate = false,
                        concepto = "Monto Operación",
                        simbolo = "Bs",
                        valor = (montoOperacion * tasaConversion),
                        valor2 = string.Format("{0:##,###,##0.00}", (montoOperacion * tasaConversion)),
                        Sale = saleConvertion
                    });
                    
                    SearchRate(req, operationTypeConversion, infMonedaConevrsion.MonedaCodigoInt, ref rates, ref totalCommissionBs, ref totalCommissionUsd, saleConvertion, tasaConversion);
                    
                    var totalAmountConversion = rates.FirstOrDefault(x => x.Sale == saleConvertion && x.InternalId == "AmountOperation").valor -
                        rates.FirstOrDefault(x => x.Sale == saleConvertion && x.moneda == "VEB").valor;

                    rates.Add(new Common.Models.Common.Tarifa
                    {
                        InternalId = "SubTotalAmountOperation",
                        IsRate = false,
                        concepto = "Monto A Cobrar Cliente",
                        simbolo = "Bs",
                        valor2 = string.Format("{0:##,###,##0.00}", totalAmountConversion),
                        valor = totalAmountConversion,
                        Sale = saleConvertion
                    });

                    exchangeDifferential = rates.FirstOrDefault(x => x.InternalId == "AmountOperation" && x.Sale).valor - rates.FirstOrDefault(x => x.InternalId == "AmountOperation" && !x.Sale).valor;
                    var totalOperation = rates.FirstOrDefault(x => x.InternalId == "SubTotalAmountOperation" && x.Sale).valor - rates.FirstOrDefault(x => x.InternalId == "SubTotalAmountOperation" && !x.Sale).valor;

                    rates.Add(new Common.Models.Common.Tarifa
                    {
                        InternalId = "TotaAmountOperation",
                        IsRate = false,
                        concepto = "Total Monto a Pagar(Cliente)",
                        simbolo = "Bs",
                        valor2 = string.Format("{0:##,###,##0.00}", totalOperation),
                        valor = totalOperation,
                        Sale = sale
                    });

                    rates.Add(new Common.Models.Common.Tarifa
                    {
                        InternalId = "ReferenceInformation",
                        IsRate = false,
                        concepto = "Diferencial Cambiario",
                        simbolo = "Bs",
                        valor2 = string.Format("{0:##,###,##0.00}", exchangeDifferential),
                        valor = exchangeDifferential,
                        Sale = sale
                    });
                }

                if (req.TakeCommission)
                {
                    totalDispUsd = req.montoEnviar;
                    totalDispBs += Math.Round((totalDispUsd + totalCommissionUsd) * req.tasa, 2);
                }
                else
                {
                    totalDispUsd = req.montoEnviar - totalCommissionUsd;
                    totalDispBs += Math.Round(req.montoEnviar * req.tasa, 2);
                }

                var total = Math.Round(req.montoEnviar * req.tasa, 2);

                decimal ammountConversion = 0;
                decimal ammountConversionWithoutRate = 0;
                decimal rateConversion = 0;

                if (req.MonedaConversion == 221)
                {
                    rateConversion = req.tasa;
                    ammountConversionWithoutRate = total;
                    ammountConversion = totalDispBs + totalCommissionBs;
                }
                else
                {
                    rateConversion = tasaConversion;
                    ammountConversionWithoutRate = Math.Round(total / (sameCoin == true ? tasaOperacion : tasaConversion), 2);
                    ammountConversion = Math.Round((totalDispBs) / (sameCoin == true ? tasaOperacion : tasaConversion), 2);
                }

                /*Agregamos el monto de conversión para manejar su valor por las listas */
                rates.Add(new Common.Models.Common.Tarifa
                {
                    InternalId = "AmmountConversion",
                    IsRate = false,
                    concepto = "Monto de Conversión",
                    simbolo = "",
                    valor2 = string.Format("{0:##,###,##0.00}", ammountConversion),
                    valor = ammountConversion,
                    Sale = sale,
                    IgnoreInView = true
                });

                rates.Add(new Common.Models.Common.Tarifa
                {
                    InternalId = "AmmountConversionWithoutRate",
                    IsRate = false,
                    concepto = "Monto de Conversión",
                    simbolo = "",
                    valor2 = string.Format("{0:##,###,##0.00}", ammountConversionWithoutRate),
                    valor = ammountConversionWithoutRate,
                    Sale = sale,
                    IgnoreInView = true
                });

                var result = new RateResult
                {
                    RateOperation = rates,
                    AmmountUsdFormat = string.Format("{0:##,###,##0.00}", totalDispUsd),
                    AmmountUsd = totalDispUsd,
                    OperationRate = string.Format("{0:##,###,##0.0000}", tasaOperacion),
                    OperationAmmount = montoOperacion,
                    OperationAmmountFormat = string.Format("{0:##,###,##0.00}", montoOperacion),
                    AmmountConversionWithoutRate = ammountConversionWithoutRate,
                    AmmountConversion = ammountConversion,
                    AmmountConversionFormat = string.Format("{0:##,###,##0.00}", ammountConversion),
                    AmmountConversionWithoutRateFormat = string.Format("{0:##,###,##0.00}", ammountConversionWithoutRate),
                    RateDate = fechaTasa,
                    SimbolConversion = infMonedaConevrsion.MonedaSimbolo,
                    TotalCommissionBs = totalCommissionBs,
                    TotalCommissionBsFormat = string.Format("{0:##,###,##0.00}", totalCommissionBs),
                    ExchangeDifferential = exchangeDifferential,
                    ExchangeDifferentialFormat = string.Format("{0:##,###,##0.00}", exchangeDifferential),
                    //RateConversion = rateConvertion
                };
                if (totalDispUsd <= 0)
                {
                    result.error = true;
                    result.clientErrorDetail = "La comisión es mayor al monto de la operación.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return new RateResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
            }

        }

        public static void SearchRate(TarifaReq req, string operationType, string operationCurrency, ref List<Common.Models.Common.Tarifa> tar, ref decimal totalDispBs, ref decimal totalDispUsd, bool sale, decimal rateOperation)
        {
            ISimadiBuilder BuilderSimadi = new SimadiBuilder();
            var TipoMovimientoRequest = new Common.Models.Angulo_Lopez.Simadi.TipoMovimientoRequest()
            {
                TipoOperacion = operationType,
                idTipoIdentidad = req.tipoId
            };

            var movementType = BuilderSimadi.SearchTiposMovimientos(TipoMovimientoRequest);
            var tipoOperacion = Convert.ToInt32(req.tipoOperacion);
            var mov = movementType.Where(x => x.ID_TIPO == tipoOperacion).FirstOrDefault();

            #region Se conultas las tarifas
            ITarifasBuilder BuilderTarifas = new TarifasBuilder();
            var GetComisionesRequest = new GetComisionesRequest()
            {
                tipo_solicitud = mov.REQUEST_TARIFA,
                monto_enviar_usd = req.montoEnviar,
                pais = req.idPais,
                corresponsal = req.idCorresp
            };

            var Comisiones = BuilderTarifas.SearchGetComisiones(GetComisionesRequest);

            List<GetComisiones> _comisiones = (List<GetComisiones>)Comisiones.Data;

            var tarifas = (from r in _comisiones
                           select new Tarifa
                           {
                               idTarifa = r.ID_COMISION,
                               concepto = r.COMISION,
                               moneda = r.MONEDA,
                               valor = Convert.ToDecimal(r.MONTO, wsCulture)
                           }).ToList();

            #endregion

            //XDocument xd = new XDocument();
            //xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(mov.REQUEST_TARIFA, req.montoEnviar, operationCurrency, req.idPais, "FURBANO", req.idCorresp, string.Empty).OuterXml);
            //var tarifas = (from r in xd.Descendants("tarifa")
            //               select new Tarifa
            //               {
            //                   id = Convert.ToInt32(r.Attribute("id").Value),
            //                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
            //                   concepto = r.Attribute("concepto").Value,
            //                   moneda = r.Attribute("moneda").Value,
            //                   valor = Convert.ToDecimal(r.Value, wsCulture)
            //               }).ToList();

            foreach (var item in tarifas)
            {
                string simbolo;
                string internalId = "CalculateInformation";
                decimal val = 0;
                if (item.moneda.Equals("USD"))
                {
                    internalId = "ReferenceInformation";
                    simbolo = "$";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * req.montoEnviar, 2);
                    else
                        val = Math.Round(item.valor, 2);
                    totalDispUsd += val;
                }
                else
                {
                    simbolo = "Bs";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * Math.Round((req.montoEnviar * rateOperation), 2), 2);
                    else
                        val = Math.Round(item.valor, 2);
                    totalDispBs += val;
                }
                var val2 = string.Format("{0:##,###,##0.00}", val);
                tar.Add(new Common.Models.Common.Tarifa
                {
                    InternalId = internalId,
                    id = item.id,
                    concepto = item.concepto,
                    idTarifa = item.idTarifa,
                    moneda = item.moneda,
                    valor = val,
                    valor2 = val2,
                    simbolo = simbolo,
                    Sale = sale,
                    IsRate = true
                });
            }
            /*Verificamos si */
            //var queryTar = tar.GroupBy(
            //p => p.moneda,
            //(key, g) => new { moneda = key, result = g.ToList() });
            //foreach (var item in queryTar)
            //{
            //    if(item.result.Count > 1)
            //    {

            //    }
            //}

        }
        public static TarifaResult getResumenFinancieroCompraCorresponsal(TarifaReq req)
        {
            try
            {
                var tm = getTiposMovimientos(req.tipoId, "compra-simadi-enc");
                var mov = tm.Where(x => x.id == req.tipoOperacion).FirstOrDefault();
                decimal totalDispBs;
                decimal totalDispUsd;
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa(mov.requestTarifa, req.montoEnviar, req.moneda, req.idPais, "FURBANO", req.idCorresp, string.Empty).OuterXml);
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();

                var tar = new List<Tarifa>();
                totalDispBs = Math.Round(req.montoEnviar * req.tasa, 2);
                totalDispUsd = req.montoEnviar;
                foreach (var item in tarifas)
                {
                    string simbolo;
                    decimal val = 0;
                    if (item.moneda.Equals("USD"))
                    {
                        simbolo = "$";
                        if (item.valor < 1)
                            val = Math.Round(item.valor * req.montoEnviar, 2);
                        else
                            val = Math.Round(item.valor, 2);

                        totalDispUsd += -val;
                    }
                    else
                    {
                        simbolo = "Bs";
                        if (item.valor < 1)
                            val = Math.Round(item.valor * Math.Round((req.montoEnviar * req.tasa), 2), 2);
                        else
                            val = Math.Round(item.valor, 2);
                        totalDispBs += -val;
                    }
                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }
                var total = Math.Round(req.montoEnviar * req.tasa, 2);
                var valTotal = string.Format("{0:##,###,##0.00}", total);

                var totalBs = string.Format("{0:##,###,##0.00}", totalDispBs);
                var totalUsd = string.Format("{0:##,###,##0.00}", totalDispUsd);

                var result = new TarifaResult
                {
                    montoCobrar = total,
                    montoCobrar2 = valTotal,
                    tarifas = tar,
                    montoTotalBs = totalBs,
                    montoTotalUsd = totalUsd
                };
                if (totalDispUsd <= 0)
                {
                    result.error = true;
                    result.clientErrorDetail = "La comisión es mayor al monto de la operación.";
                }
                return result;
            }
            catch (Exception ex)
            {
                var ret = new TarifaResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static TarifaResult getResumenFinancieroOpNacional(TarifaReq req)
        {
            try
            {
                decimal totalDispBs;
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.operaciones.GetTarifaRemesa("cambio-efc-pos", req.montoEnviar, req.moneda, req.idPais, "FURBANO", req.idCorresp, string.Empty).OuterXml);
                var tarifas = (from r in xd.Descendants("tarifa")
                               select new Tarifa
                               {
                                   id = Convert.ToInt32(r.Attribute("id").Value),
                                   idTarifa = Convert.ToInt32(r.Attribute("id_tarifa").Value),
                                   concepto = r.Attribute("concepto").Value,
                                   moneda = r.Attribute("moneda").Value,
                                   valor = Convert.ToDecimal(r.Value, wsCulture)
                               }).ToList();

                var tar = new List<Tarifa>();
                totalDispBs = req.montoEnviar;
                foreach (var item in tarifas)
                {
                    string simbolo;
                    decimal val = 0;

                    simbolo = "Bs";
                    if (item.valor < 1)
                        val = Math.Round(item.valor * Math.Round((req.montoEnviar), 2), 2);
                    else
                        val = Math.Round(item.valor, 2);
                    totalDispBs -= val;

                    var val2 = string.Format("{0} {1:##,###,##0.00}", simbolo, val);
                    tar.Add(new Tarifa
                    {
                        id = item.id,
                        concepto = item.concepto,
                        idTarifa = item.idTarifa,
                        moneda = item.moneda,
                        valor = val,
                        valor2 = val2,
                        simbolo = simbolo
                    });
                }
                var total = Math.Round(req.montoEnviar, 2);
                var valTotal = string.Format("{0:##,###,##0.00}", total);

                var totalBs = string.Format("{0:##,###,##0.00}", totalDispBs);

                var result = new TarifaResult
                {
                    montoCobrar = total,
                    montoCobrar2 = valTotal,
                    tarifas = tar,
                    montoTotalBs = totalBs
                };
                return result;
            }
            catch (Exception ex)
            {
                var ret = new TarifaResult { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static List<Parametro> getParametrosOrden()
        {

            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetParametros(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("PARAMETRO")
                              select new Parametro
                              {
                                  que = r.Element("QUE").Value,
                                  cuanto = Convert.ToInt64(r.Element("CUANTO").Value),
                                  cuando = DateTime.Parse(r.Element("CUANDO").Value, wsCulture),
                                  referencia = r.Element("REFERENCIA").Value,
                                  app = r.Element("APP").Value,
                                  exeName = r.Element("EXE_NAME").Value
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Parametro> { new Parametro { clientErrorDetail = "Ha ocurrido un error al obtener los parametros para inicializar la orden.", error = true, apiDetail = "getParametrosOrden", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Corresponsal> getCorresponsales(string pais)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetCorresponsalesPais(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("CORRESPONSALES")
                              select new Corresponsal
                              {
                                  codigo = r.Element("ID_CORRESPONSAL").Value,
                                  nombre = r.Element("CORRESPONSAL").Value,
                                  idPais = r.Element("ID_PAIS").Value
                              }).Where(x => x.idPais.Equals(pais)).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Corresponsal>() { new Corresponsal { clientErrorDetail = "Error al Cargar el listado de Corresponsales", error = true, apiDetail = "getCorresponsales", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Corresponsal> getCorresponsales()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetCorresponsalesPais(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("CORRESPONSALES")
                              select new Corresponsal
                              {
                                  codigo = r.Element("ID_CORRESPONSAL").Value,
                                  nombre = r.Element("CORRESPONSAL").Value,
                                  idPais = r.Element("ID_PAIS").Value
                              }).OrderBy(x => x.idPais).OrderBy(x => x.nombre).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Corresponsal>() { new Corresponsal { clientErrorDetail = "Error al Cargar el listado de Corresponsales", error = true, apiDetail = "getCorresponsales", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Ciudad> getCiudades(string pais)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetCiudadesPorPaises(pais, string.Empty, string.Empty).OuterXml);
                var result = (from r in xd.Descendants("CIUDADES")
                              select new Ciudad
                              {
                                  id = Convert.ToInt32(r.Element("ID_CIUDAD").Value),
                                  nombre = r.Element("CIUDAD").Value,
                                  idPais = pais
                              }).Where(x => x.idPais.Equals(pais)).OrderBy(x => x.nombre).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Ciudad>() { new Ciudad { clientErrorDetail = "Error al Cargar el listado de Ciudades", error = true, apiDetail = "getCiudades", errorDetail = ex } };
                return ret;
            }

        }

        public static Corresponsal getCorresponsalPaisDeposito(string pais, bool deposito, string tipo_remesa)
        {
            XDocument xd = new XDocument();

            xd = XDocument.Parse(Services.catalogos.GetCorresponsalesPaisTipoPagoDestino(pais, deposito, tipo_remesa, string.Empty).OuterXml);
            var result = (from r in xd.Descendants("CORRESPONSALES")
                          select new Corresponsal
                          {
                              nombre = r.Element("CORRESPONSAL").Value,
                              codigo = r.Element("ID_CORRESPONSAL").Value,
                              idPais = r.Element("ID_PAIS").Value
                          }).OrderBy(x => x.nombre).FirstOrDefault();
            if (result == null)
            {
                result = new Corresponsal()
                {
                    codigo = "TRF",
                    nombre = "Transferencia Directa"
                };
            }
            return result;


        }

        public static List<Oficina> getOficinas(OficinasRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                var tipoBusqueda = 1;
                var distancia = 0;
                bool deposito = false;
                short matricula = 0;
                xd = XDocument.Parse(Services.catalogos.GetOficinasPorCiudad(tipoBusqueda, req.idCiudad, distancia, deposito, req.tipoOperacion, matricula, req.monto, string.Empty).OuterXml);
                var result = (from r in xd.Descendants("RESULTADO")
                              select new Oficina
                              {
                                  id = Convert.ToInt32(r.Element("CODIGO").Value.Split('-')[1]),
                                  nombre = r.Element("NOMBRE").Value,
                                  pagador = r.Element("CODIGO").Value.Split('-')[0],
                                  codigo = r.Element("CODIGO").Value,
                                  deposito = r.Element("DEPOSITO").Value == "0" ? false : true,
                                  corresponsal = r.Element("CORRESPONSAL").Value,
                              }).OrderBy(x => x.nombre).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Oficina>() { new Oficina { clientErrorDetail = "Error al Cargar el listado de Oficinas", error = true, apiDetail = "getOficinas", errorDetail = ex } };
                return ret;
            }

        }

        public static Oficina GetDetalleOficina(int Idoficina)
        {
            XDocument xd = new XDocument();
            Oficina Ofic = new Oficina();
            xd = XDocument.Parse(Services.catalogos.GetDetalleOficina(Idoficina, string.Empty).OuterXml);
            Ofic = (from r in xd.Descendants("RESULTADO")
                    select new Oficina
                    {
                        id = Convert.ToInt32(r.Element("id").Value),
                        nombre = r.Element("nombre").Value,
                        pagador = r.Element("pagador").Value,
                        deposito = r.Element("deposito").Value == "0" ? false : true,
                        corresponsal = r.Element("corresponsal").Value,
                        tasa = Convert.ToDecimal(r.Element("tasa").Value, wsCulture)
                    }).OrderBy(x => x.nombre).FirstOrDefault();
            return Ofic;
        }

        public static List<OrdenAnulacion> getListadoOperacionesAnulables(ClienteRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(req.current);
                var rolUser = Services.users.GetUsuariosPorRol(0, "Cajero", user.idSucursal, "");
                bool esCajero = false;
                if (rolUser.FirstChild != rolUser.SelectSingleNode("ERROR"))
                {
                    if (rolUser.SelectSingleNode("descendant::USUARIO[LOGIN='" + user.login + "']") != null)
                        esCajero = true;
                }



                var p = string.Format("{0}{1}", req.tipoId, req.numeroId);
                xd = XDocument.Parse(Services.miscelaneos.GetOperacioneAnulablesPorSucursalYCliente(p, user.letraSucursal, 0, "").OuterXml);
                var result = (from r in xd.Descendants("OPERACION")
                              select new OrdenAnulacion
                              {
                                  id = Convert.ToInt32(r.Element("ID_OPERACION").Value),
                                  fecha = r.Element("FECHA").Value,
                                  tabla = r.Element("TABLA").Value,
                                  letra = r.Element("CIUORIG").Value,
                                  monto = Convert.ToDecimal(r.Element("MONTO").Value, wsCulture),
                                  nombresCliente = r.TryGetElementValue("CLIENTE"),
                                  numeroIdCliente = r.TryGetElementValue("ID_CLIENTE"),
                                  numero = Convert.ToInt32(r.Element("NRORECIBO").Value),
                                  status = Convert.ToInt32(r.Element("STATUS").Value),
                                  pagoCliente = Convert.ToInt32(string.IsNullOrEmpty(r.Element("PAGO_CLIENTE").Value) ? "0" : r.Element("PAGO_CLIENTE").Value),
                                  descripcionStatus = r.TryGetElementValue("DESCRIPCION_STATUS"),
                                  simadi = string.IsNullOrEmpty(r.Element("SIMADI").Value) ? false : r.Element("SIMADI").Value == "1" ? true : false,
                                  Cajero = r.Element("CAJERO").Value,
                                  Analista = r.Element("ANALISTA").Value,
                                  TypeOperationId = r.Element("TYPEOPERATIONID").Value,
                                  TypeOperationName = r.Element("TYPEOPERATIONNAME").Value,
                                  NumberTypeOperation = int.Parse(r.Element("NUMBERTYPEOPERATION").Value),
                                  Moneda = r.Element("MONEDA").Value,
                                  SimboloMoneda = r.Element("SIMBOLOMONEDA").Value,
                                  SimboloMonedaOperacion = r.Element("MONEDAOPERACIONSIMBOLO").Value,
                                  TasaConversion = Convert.ToDecimal(r.Element("TASACONVERSION").Value, wsCulture).ToString(),
                                  MonedaOperacion = int.Parse(r.Element("MONEDAOPERACION").Value),
                                  MontoConversion = Convert.ToDecimal(r.Element("MONTOCONVERSION").Value, wsCulture).ToString()

                              }).OrderBy(x => x.numero).ToList();
                var retorno = new List<OrdenAnulacion>();
                foreach (var item in result)
                {
                    bool remove = false;
                    if (!esCajero)
                    {
                        if (item.tabla.Trim() != "GIROS_TMP" || item.Analista != user.login)
                            remove = true;
                    }
                    else
                    {
                        if (item.tabla.Trim() == "GIROS_TMP" && item.Analista != user.login)
                            remove = true;
                        else if (item.tabla.Trim() != "GIROS_TMP" && item.Cajero != user.login && item.TypeOperationId != "CCVENC")
                            remove = true;
                        else if (item.status == 3)
                            remove = true;

                    }
                    if (!remove)
                        retorno.Add(item);
                    //retorno.Remove(item);

                }

                return retorno;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenAnulacion>() { new OrdenAnulacion { clientErrorDetail = "Error al Cargar el listado de Oficinas", error = true, apiDetail = "getOficinas", errorDetail = ex } };
                return ret;
            }
        }

        public static List<OrdenAnulacion> getListadoOperacionesAnulables(OrdenRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(req.current);
                var p = string.Empty;
                xd = XDocument.Parse(Services.miscelaneos.GetOperacioneAnulablesPorSucursalYCliente(p, user.letraSucursal, req.numero, "").OuterXml);
                var result = (from r in xd.Descendants("OPERACION")
                              select new OrdenAnulacion
                              {
                                  id = Convert.ToInt32(r.Element("ID_OPERACION").Value),
                                  fecha = r.Element("FECHA").Value,
                                  tabla = r.Element("TABLA").Value,
                                  letra = r.Element("CIUORIG").Value,
                                  monto = Convert.ToDecimal(r.Element("MONTO").Value, wsCulture),
                                  numeroIdCliente = r.TryGetElementValue("ID_CLIENTE"),
                                  numero = Convert.ToInt32(r.Element("NRORECIBO").Value),
                                  status = Convert.ToInt32(r.Element("STATUS").Value),
                                  pagoCliente = Convert.ToInt32(string.IsNullOrEmpty(r.Element("PAGO_CLIENTE").Value) ? "0" : r.Element("PAGO_CLIENTE").Value),
                                  descripcionStatus = r.TryGetElementValue("DESCRIPCION_STATUS"),
                                  simadi = Convert.ToBoolean(r.Element("SIMADI").Value),
                                  TypeOperationId = r.Element("TYPEOPERATIONID").Value,
                                  Moneda = r.Element("MONEDA").Value,
                                  SimboloMoneda = r.Element("SIMBOLOMONEDA").Value,
                                  TypeOperationName = r.Element("TYPEOPERATIONNAME").Value,
                                  NumberTypeOperation = int.Parse(r.Element("NUMBERTYPEOPERATION").Value)
                              }).OrderBy(x => x.numero).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenAnulacion>() { new OrdenAnulacion { clientErrorDetail = "Error al Cargar el listado de Oficinas", error = true, apiDetail = "getOficinas", errorDetail = ex } };
                return ret;
            }
        }

        public static List<OrdenDevolucion> getListadoOperacionesDevolucion(ClienteRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(req.current);
                var p = string.Format("{0}{1}", req.tipoId, req.numeroId);
                xd = XDocument.Parse(Services.miscelaneos.GetOperacionesDevolucionPorSucursalYCliente(p, user.letraSucursal, 0, "").OuterXml);
                var result = (from r in xd.Descendants("OPERACION")
                              select new OrdenDevolucion
                              {
                                  id = Convert.ToInt32(r.Element("ID_OPERACION").Value),
                                  fecha = r.Element("FECHA").Value,
                                  tabla = r.Element("TABLA").Value,
                                  letra = r.Element("CIUORIG").Value,
                                  monto = Convert.ToDecimal(r.Element("MONTO").Value, wsCulture),
                                  numeroIdCliente = r.TryGetElementValue("ID_CLIENTE"),
                                  numero = Convert.ToInt32(r.Element("NRORECIBO").Value),
                                  status = Convert.ToInt32(r.Element("STATUS").Value),
                                  pagoCliente = Convert.ToInt32(string.IsNullOrEmpty(r.Element("PAGO_CLIENTE").Value) ? "0" : r.Element("PAGO_CLIENTE").Value),
                                  descripcionStatus = r.TryGetElementValue("DESCRIPCION_STATUS"),
                                  simadi = string.IsNullOrEmpty(r.Element("SIMADI").Value) ? false : r.Element("SIMADI").Value == "1" ? true : false,
                                  nombresCliente = r.TryGetElementValue("CLIENTE"),
                                  tasaCambio = Convert.ToDecimal(r.Element("TIPO_CAMBIO").Value, wsCulture),
                                  montoDevolver = Convert.ToDecimal(r.Element("MONTO_DEVOLVER").Value, wsCulture),
                                  TarifaUSD = Convert.ToDecimal(r.Element("TARIFAUSD").Value, wsCulture)

                              }).OrderBy(x => x.numero).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenDevolucion>() { new OrdenDevolucion { clientErrorDetail = "Error al Cargar el listado de Oficinas", error = true, apiDetail = "getOficinas", errorDetail = ex } };
                return ret;
            }
        }

        public static List<OrdenDevolucion> getListadoOperacionesDevolucion(OrdenRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(req.current);
                var p = string.Empty;
                xd = XDocument.Parse(Services.miscelaneos.GetOperacionesDevolucionPorSucursalYCliente(p, req.sucursal, req.numero, "").OuterXml);
                var result = (from r in xd.Descendants("OPERACION")
                              select new OrdenDevolucion
                              {
                                  id = Convert.ToInt32(r.Element("ID_OPERACION").Value),
                                  fecha = r.Element("FECHA").Value,
                                  tabla = r.Element("TABLA").Value,
                                  letra = r.Element("CIUORIG").Value,
                                  monto = Convert.ToDecimal(r.Element("MONTO").Value, wsCulture),
                                  numeroIdCliente = r.TryGetElementValue("ID_CLIENTE"),
                                  numero = Convert.ToInt32(r.Element("NRORECIBO").Value),
                                  status = Convert.ToInt32(r.Element("STATUS").Value),
                                  pagoCliente = Convert.ToInt32(string.IsNullOrEmpty(r.Element("PAGO_CLIENTE").Value) ? "0" : r.Element("PAGO_CLIENTE").Value),
                                  descripcionStatus = r.TryGetElementValue("DESCRIPCION_STATUS"),
                                  simadi = string.IsNullOrEmpty(r.Element("SIMADI").Value) ? false : r.Element("SIMADI").Value == "1" ? true : false,
                                  tasaCambio = Convert.ToDecimal(r.Element("TIPO_CAMBIO").Value, wsCulture),
                                  montoDevolver = Convert.ToDecimal(r.Element("MONTO_DEVOLVER").Value, wsCulture)
                              }).OrderBy(x => x.numero).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenDevolucion>() { new OrdenDevolucion { clientErrorDetail = "Error al Cargar el listado de Oficinas", error = true, apiDetail = "getOficinas", errorDetail = ex } };
                return ret;
            }
        }

        public static List<MotivosAnulacionOrdenes> getMotivosAnulaciones()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetMotivosAnulacionesOperaciones(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("MOTIVOS")
                              select new MotivosAnulacionOrdenes
                              {
                                  id = Convert.ToInt32(r.Element("ID_MOTIVO").Value),
                                  nombre = r.Element("MOTIVO").Value,
                                  descripcion = r.Element("DESCRIPCION").Value,
                                  activa = r.Element("ACTIVA").Value == "0" ? false : true
                              }).OrderBy(x => x.nombre).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<MotivosAnulacionOrdenes>() { new MotivosAnulacionOrdenes { clientErrorDetail = "Error al Cargar el listado de Motivos de Anulación de Ordenes", error = true, apiDetail = "getOficinas", errorDetail = ex } };
                return ret;
            }

        }

        public static List<TipoCancelacion> getTiposCancelacion()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetTiposCancelacion(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("TIPOS")
                              select new TipoCancelacion
                              {
                                  id = Convert.ToInt32(r.Element("ID_TIPO").Value),
                                  tipo = r.Element("TIPO_CANCELACION").Value
                              }).OrderBy(x => x.tipo).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<TipoCancelacion>() { new TipoCancelacion { clientErrorDetail = "Error al Cargar el listado de Tipos de Cancelación para la Devolución de Ordenes", error = true, apiDetail = "getTiposCancelacion", errorDetail = ex } };
                return ret;
            }

        }

        public static List<DetalleTipoPago> getDetallesTiposPago(int tipoPago)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetDetalleTiposPagos(string.Empty).OuterXml);
                var result = (from r in xd.Descendants("DETALLE_TIPO_PAGO")
                              select new DetalleTipoPago
                              {
                                  id = Convert.ToInt32(r.Element("ID_DETALLE").Value),
                                  idTipoPago = Convert.ToInt32(r.Element("TIPO_PAGO").Value),
                                  detalleTipoPago = r.Element("DETALLE").Value
                              }).Where(x => x.idTipoPago == tipoPago).OrderBy(x => x.detalleTipoPago).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<DetalleTipoPago>() { new DetalleTipoPago { clientErrorDetail = "Error al Cargar el listado de Tipos de Cancelación para la Devolución de Ordenes", error = true, apiDetail = "getTiposCancelacion", errorDetail = ex } };
                return ret;
            }

        }

        public static OrdenResult setAnularOperaciones(List<OrdenAnulacion> ordenes)
        {
            foreach (var orden in ordenes)
            {
                var user = getUsuario(orden.current);
                XDocument xd;
                switch (orden.tabla.ToUpper())
                {
                    case "FACT":

                        if (orden.motivo != null && orden.motivo.Trim() != "" && !orden.Procesada)
                            orden.AnuladaPor = user.login;
                        else
                        {
                            if (!orden.Procesada)
                            {
                                orden.AnulAutorizadaPor = user.login;
                                orden.idMotivo = 0;
                            }
                        }

                        var fact = Services.miscelaneos.SetAnulacionDivisas(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                          user.nombreCompleto, orden.idMotivo == null ? "" : orden.idMotivo.ToString(), orden.id, orden.observaciones, orden.AnulAutorizadaPor == null ? "" : orden.AnulAutorizadaPor, orden.ReferenciaAnulBcv, orden.Procesada, orden.AnuladaPor, orden.tabla.ToUpper(), "");


                        //var fact = Services.miscelaneos.SetAnulacionFacturaLibre(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                        //  user.nombreCompleto, orden.motivo, orden.numero.ToString(), user.letraSucursal, "");
                        if (fact.FirstChild != fact.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(fact.SelectSingleNode("//ACTUALIZADO").InnerText))
                            {
                                if (orden.Procesada)
                                {
                                    var result2 = Services.operaciones.SetActualizarEstatusRemesaSaliente(Convert.ToInt64(orden.id), 70, "");
                                    if (result2.FirstChild != result2.SelectSingleNode("//ERROR"))
                                    {
                                        if (!setEliminarInformacionFinancieraOrden(orden))
                                            return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la anulación de forma exitosa (Ingresos), por favor notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la anulación de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
                                    }
                                    else
                                        return new OrdenResult() { error = true, clientErrorDetail = "Error: " + result2.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(result2.SelectSingleNode("ERROR").InnerText) };

                                    Services.utilitarios.SetTrazaSeguridad(37, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                                }
                            }
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + fact.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(fact.SelectSingleNode("ERROR").InnerText) };

                        break;
                    case "SIMADI":

                        if (orden.motivo != null && orden.motivo.Trim() != "" && !orden.Procesada)
                            orden.AnuladaPor = user.login;
                        else
                        {
                            if (!orden.Procesada)
                                orden.AnulAutorizadaPor = user.login;
                        }
                        var tipoOpBcv = orden.TypeOperationId;
                        if (orden.TypeOperationId.Trim().ToUpper() != "CCVENV" &&
                            orden.TypeOperationId.Trim().ToUpper() != "CCEENV" &&
                            orden.TypeOperationId.Trim().ToUpper() != "CCPENV" &&
                            orden.TypeOperationId.Trim().ToUpper() != "CCPENC" &&
                            orden.TypeOperationId.Trim().ToUpper() != "CCEENC" &&
                            orden.TypeOperationId.Trim().ToUpper() != "CCVENC" &&
                            orden.status != 10)
                        {
                            xd = new XDocument();
                            xd = XDocument.Parse(Services.operaciones.GetOpSimadiFiltrada(orden.letra, orden.numero, 0, string.Empty).OuterXml);
                            var op = (from r in xd.Descendants("OPERACION")
                                      select new OrdenAnulacion
                                      {
                                          id = Convert.ToInt32(r.Element("NUMERO").Value),
                                          tipoOpBcv = r.Element("TIPO_OP_BCV").Value,
                                          motivoBcv = r.Element("MOTIVO_OP_BCV").Value,
                                          referenciaBCV = r.Element("REFERENCIA_ORDEN").Value
                                      }).FirstOrDefault();



                            if (op.tipoOpBcv.Contains("CCEEFC") ||
                                op.tipoOpBcv.Contains("CCETRC") ||
                                op.tipoOpBcv.Contains("CCPEFC") ||
                                op.tipoOpBcv.Contains("CCPTRC") ||
                                op.tipoOpBcv.Contains("CCVEFC") ||
                                op.tipoOpBcv.Contains("CCVTRC") ||
                                op.tipoOpBcv.Contains("CCPENC") ||
                                op.tipoOpBcv.Contains("CCEENC") ||
                                op.tipoOpBcv.Contains("CCVENC"))
                            {
                                tipoOpBcv = "CCANUC";
                            }
                            else
                                tipoOpBcv = "CCANUV";


                            //var bcv = Services.financiero.SetAnularOperacionSIMADI(tipoOpBcv, string.Format("{0}{1}", orden.tipoIdCliente, orden.numeroIdCliente));
                            if (orden.AnulAutorizadaPor != null && orden.AnulAutorizadaPor.Trim() != "")
                            {
                                string referenciaBCV = op.referenciaBCV;
                                //var bcv = Services.financiero.SetAnularOperacionSIMADI(tipoOpBcv, referenciaBCV);
                                var bcv = new XmlDocument();
                                bcv.LoadXml("<root><referencia_anulacion>" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "</referencia_anulacion></root>");
                                //var bcv = Services.financiero.SetAnularOperacionSIMADI(op.referenciaBCV, tipoOpBcv);
                                if (bcv.FirstChild == bcv.SelectSingleNode("//error"))
                                {
                                    return new OrdenResult()
                                    {
                                        error = true,
                                        clientErrorDetail = "Error del BCV: " + bcv.SelectSingleNode("//error").InnerText,
                                        status = "error",
                                        errorDetail = new Exception(bcv.SelectSingleNode("//error").InnerText)
                                    };
                                }
                                else
                                {
                                    if (bcv.SelectSingleNode("//referencia_anulacion").InnerText.Trim() != string.Empty)
                                        orden.ReferenciaAnulBcv = bcv.SelectSingleNode("//referencia_anulacion").InnerText.Trim();
                                    else
                                    {
                                        return new OrdenResult()
                                        {
                                            error = true,
                                            clientErrorDetail = "Error del BCV: La referencia de anulación del BCV esta en blanco. Por favor verifique ante el BCV si esta operación fue anulada exitosamente.",
                                            status = "error",
                                            errorDetail = new Exception("Error del BCV: La referencia de anulación del BCV esta en blanco. Por favor verifique ante el BCV si esta operación fue anulada exitosamente.")
                                        };
                                    }
                                }
                            }
                        }



                        var simadi = Services.miscelaneos.SetAnulacionDivisas(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                          user.nombreCompleto, orden.idMotivo.ToString(), orden.id, orden.observaciones, orden.AnulAutorizadaPor, orden.ReferenciaAnulBcv, orden.Procesada, orden.AnuladaPor, orden.TypeOperationId.ToUpper(), "");
                        if (simadi.FirstChild != simadi.SelectSingleNode("//ERROR"))
                        {
                            if (Convert.ToBoolean(simadi.SelectSingleNode("//ACTUALIZADO").InnerText))
                            {
                                if (orden.Procesada)
                                {
                                    if (orden.TypeOperationId.Trim().ToUpper() != "CCVENV" &&
                                        orden.TypeOperationId.Trim().ToUpper() != "CCEENV" &&
                                        orden.TypeOperationId.Trim().ToUpper() != "CCPENV" &&
                                        orden.TypeOperationId.Trim().ToUpper() != "CCPENC" &&
                                        orden.TypeOperationId.Trim().ToUpper() != "CCEENC" &&
                                        orden.TypeOperationId.Trim().ToUpper() != "CCVENC")
                                    {
                                        if (setEliminarInformacionFinancieraOrden(orden))
                                            Services.utilitarios.SetTrazaSeguridad(37, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                                        else
                                            return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la anulación de forma exitosa (Ingresos), por favor notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la anulación de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
                                    }
                                    else
                                    {
                                        Services.utilitarios.SetTrazaSeguridad(37, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                                    }
                                }
                            }
                            else
                                return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la anulación de forma exitosa (Ingresos), por favor notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la anulación de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + simadi.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(simadi.SelectSingleNode("ERROR").InnerText) };

                        break;
                    case "GIROSPAY":
                        var gpay = Services.miscelaneos.SetOperacionPorPagar(orden.id.ToString(), "", "", "");
                        if (gpay.FirstChild != gpay.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(gpay.SelectSingleNode("//ELIMINADO").InnerText))
                            {
                                var result2 = Services.operaciones.SetCambioDatos(orden.numero.ToString(), user.letraSucursal, "");
                                if (result2.FirstChild == result2.SelectSingleNode("ERROR"))
                                    return new OrdenResult() { error = true, clientErrorDetail = "Error: " + result2.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(result2.SelectSingleNode("ERROR").InnerText) };

                                Services.utilitarios.SetTrazaSeguridad(35, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                            }
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + gpay.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(gpay.SelectSingleNode("ERROR").InnerText) };

                        break;
                    case "GIROS_TMP":
                        var opt = new OrdenAnulacion();
                        if (orden.motivo != null && orden.motivo.Trim() != "" && !orden.Procesada)
                            orden.AnuladaPor = user.login;
                        else
                        {
                            if (!orden.Procesada)
                                orden.AnulAutorizadaPor = user.login;
                        }
                        if (orden.TypeOperationId.Trim().ToUpper() != "CCVENV" &&
                           orden.TypeOperationId.Trim().ToUpper() != "CCEENV" &&
                           orden.TypeOperationId.Trim().ToUpper() != "CCPENV" &&
                           orden.TypeOperationId.Trim().ToUpper() != "CCPENC" &&
                           orden.TypeOperationId.Trim().ToUpper() != "CCEENC" &&
                           orden.TypeOperationId.Trim().ToUpper() != "CCVENC")
                        {
                            xd = new XDocument();
                            xd = XDocument.Parse(Services.miscelaneos.GetOperacionesTemporales(string.Format("{0}{1}", orden.tipoIdCliente, orden.numeroIdCliente), "", orden.NumberTypeOperation, "").OuterXml);

                            opt = (from r in xd.Descendants("OPERACION")
                                   select new OrdenAnulacion
                                   {
                                       id = Convert.ToInt32(r.Element("ID_REMESA").Value),
                                       tipoOpBcv = r.Element("TIPO_OP_BCV").Value,
                                       motivoBcv = r.Element("MOTIVO_OP_BCV").Value,
                                       referenciaBCV = r.Element("RUSAD_UTILIZADO").Value
                                   }).FirstOrDefault();

                            //var tipoOpBcvT = "";

                            if (opt.tipoOpBcv.Contains("CCEEFC") ||
                                opt.tipoOpBcv.Contains("CCETRC") ||
                                opt.tipoOpBcv.Contains("CCPEFC") ||
                                opt.tipoOpBcv.Contains("CCPTRC") ||
                                opt.tipoOpBcv.Contains("CCVEFC") ||
                                opt.tipoOpBcv.Contains("CCVTRC") ||
                                opt.tipoOpBcv.Contains("CCPENC") ||
                                opt.tipoOpBcv.Contains("CCEENC") ||
                                opt.tipoOpBcv.Contains("CCVENC"))
                            {
                                tipoOpBcv = "CCANUC";
                            }
                            else
                                tipoOpBcv = "CCANUV";
                            if (orden.AnulAutorizadaPor != null && orden.AnulAutorizadaPor.Trim() != "")
                            {
                                string referenciaBCV = opt.referenciaBCV;
                                // var bcv = Services.financiero.SetAnularOperacionSIMADI(tipoOpBcv, referenciaBCV);
                                //var bcv2 = Services.financiero.SetAnularOperacionSIMADI(tipoOpBcv, referenciaBCV);
                                var bcv2 = new XmlDocument();
                                bcv2.LoadXml("<root><referencia_anulacion>" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "</referencia_anulacion></root>");

                                if (bcv2.FirstChild == bcv2.SelectSingleNode("//error"))
                                {
                                    return new OrdenResult() { error = true, clientErrorDetail = "Error del BCV: " + bcv2.SelectSingleNode("//error").InnerText, status = "error", errorDetail = new Exception(bcv2.SelectSingleNode("//error").InnerText) };
                                }
                                orden.ReferenciaAnulBcv = bcv2.SelectSingleNode("//referencia_anulacion").InnerText.Trim();
                            }
                        }

                        simadi = Services.miscelaneos.SetAnulacionDivisas(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                          user.nombreCompleto, orden.idMotivo.ToString(), orden.id, orden.observaciones, orden.AnulAutorizadaPor, orden.ReferenciaAnulBcv, orden.Procesada, orden.AnuladaPor, "GIROS_TMP", "");
                        if (simadi.FirstChild != simadi.SelectSingleNode("//ERROR"))
                        {
                            if (Convert.ToBoolean(simadi.SelectSingleNode("//ACTUALIZADO").InnerText))
                            {
                                if (orden.Procesada)
                                    Services.utilitarios.SetTrazaSeguridad(34, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                            }
                            else
                                return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la anulación de forma exitosa (Ingresos), por favor notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la anulación de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + simadi.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(simadi.SelectSingleNode("ERROR").InnerText) };

                        break;



                    //var gtmp = Services.miscelaneos.SetOperacionPorCobrar(orden.id.ToString(), "", "", true, "");
                    //if (gtmp.FirstChild != gtmp.SelectSingleNode("ERROR"))
                    //{
                    //    if (Convert.ToBoolean(gtmp.SelectSingleNode("//ELIMINADO").InnerText))
                    //    {
                    //        var result2 = Services.operaciones.SetCambioDatos(orden.numero.ToString(), user.letraSucursal, "");
                    //        if (result2.FirstChild == result2.SelectSingleNode("ERROR"))
                    //            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + result2.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(result2.SelectSingleNode("ERROR").InnerText) };

                    //        Services.utilitarios.SetTrazaSeguridad(34, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                    //    }
                    //}
                    //else
                    //    return new OrdenResult() { error = true, clientErrorDetail = "Error: " + gtmp.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(gtmp.SelectSingleNode("ERROR").InnerText) };

                    //break;
                    case "EGRE":

                        if (orden.motivo != null && orden.motivo.Trim() != "" && !orden.Procesada)
                            orden.AnuladaPor = user.login;
                        else
                        {
                            if (!orden.Procesada)
                            {
                                orden.AnulAutorizadaPor = user.login;
                                orden.idMotivo = 0;
                            }
                        }

                        var egre = Services.miscelaneos.SetAnulacionDivisas(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                          user.nombreCompleto, orden.idMotivo == null ? "" : orden.idMotivo.ToString(), orden.id, orden.observaciones, orden.AnulAutorizadaPor == null ? "" : orden.AnulAutorizadaPor, orden.ReferenciaAnulBcv, orden.Procesada, orden.AnuladaPor, orden.tabla.ToUpper(), "");


                        //var fact = Services.miscelaneos.SetAnulacionFacturaLibre(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                        //  user.nombreCompleto, orden.motivo, orden.numero.ToString(), user.letraSucursal, "");
                        if (egre.FirstChild != egre.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(egre.SelectSingleNode("//ACTUALIZADO").InnerText))
                            {
                                if (orden.Procesada)
                                    Services.utilitarios.SetTrazaSeguridad(37, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                            }
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + egre.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(egre.SelectSingleNode("ERROR").InnerText) };

                        break;
                    case "ONAC":
                    case "OP_NAC":

                        if (orden.motivo != null && orden.motivo.Trim() != "" && !orden.Procesada)
                            orden.AnuladaPor = user.login;
                        else
                        {
                            if (!orden.Procesada)
                            {
                                orden.AnulAutorizadaPor = user.login;
                                orden.idMotivo = 0;
                            }
                        }

                        var opnac = Services.miscelaneos.SetAnulacionDivisas(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                          user.nombreCompleto, orden.idMotivo == null ? "" : orden.idMotivo.ToString(), orden.id, orden.observaciones, orden.AnulAutorizadaPor == null ? "" : orden.AnulAutorizadaPor, orden.ReferenciaAnulBcv, orden.Procesada, orden.AnuladaPor, orden.tabla.ToUpper(), "");


                        //var fact = Services.miscelaneos.SetAnulacionFacturaLibre(String.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now),
                        //  user.nombreCompleto, orden.motivo, orden.numero.ToString(), user.letraSucursal, "");
                        if (opnac.FirstChild != opnac.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(opnac.SelectSingleNode("//ACTUALIZADO").InnerText))
                            {
                                if (orden.Procesada)
                                    Services.utilitarios.SetTrazaSeguridad(37, 3, "ACTIVO", "ANULADO", "ANULACION DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                            }
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: " + opnac.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(opnac.SelectSingleNode("ERROR").InnerText) };

                        break;
                    default:
                        break;
                }
            }
            return new OrdenResult() { status = "ok" };
        }

        static bool setEliminarInformacionFinancieraOrden(OrdenAnulacion orden)
        {
            var idPago = 0;
            var user = getUsuario(orden.current);
            //var p = Services.monetarios.GetDetallePagosRecibidosPorCliente(orden.tipoIdCliente, orden.numeroIdCliente,
            //    string.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 00:00",
            //    string.Format("{0:yyyy-MM-dd}", DateTime.Now) + " 23:59", string.Empty);
            var p = Services.monetarios.SearchDetallePago(user.idSucursal, orden.numero.ToString(), 0);
            if (p.FirstChild != p.SelectSingleNode("//ERROR"))
            {
                if (Convert.ToBoolean(p.SelectSingleNode("//ENCONTRADO").InnerText.Trim()))
                {
                    idPago = Convert.ToInt32(p.SelectSingleNode("//ID_PAGO").InnerText.Trim());
                    var anul = Services.monetarios.SetActualizarIngresosOrdenAnulada(idPago, orden.current, "");
                    if (anul.FirstChild == anul.SelectSingleNode("//ERROR"))
                        return false;

                    if (!Convert.ToBoolean(anul.SelectSingleNode("//ACTUALIZADO").InnerText.Trim()))
                        return false;
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        public static OrdenResult setDevolucionOperaciones(OrdenDevolucion orden)
        {
            //foreach (var orden in ordenes)
            //{
            var user = getUsuario(orden.current);
            XDocument xd;
            switch (orden.tabla.ToUpper())
            {
                case "SIMADI":

                    xd = new XDocument();
                    xd = XDocument.Parse(Services.operaciones.GetOpSimadiFiltrada(orden.letra, orden.numero, 0, string.Empty).OuterXml);

                    var op = (from r in xd.Descendants("OPERACION")
                              select new OrdenDevolucion
                              {
                                  id = Convert.ToInt32(r.Element("ID_ORDEN").Value),
                                  tipoOpBcv = r.Element("TIPO_OP_BCV").Value,
                                  motivoBcv = r.Element("MOTIVO_OP_BCV").Value,
                                  referencia = r.Element("REFERENCIA_ORDEN").Value,
                                  idPais = r.Element("ID_PAIS").Value

                              }).FirstOrDefault();

                    #region Asignación Numero de Orden
                    var num = Services.utilitarios.GetProximoNumero(22, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                    int _operationNumber = 0;
                    if (num.FirstChild != num.SelectSingleNode("ERROR"))
                    {
                        if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                            _operationNumber = Convert.ToInt32(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                        else
                            _operationNumber = 0;
                    }
                    else
                    {
                        var ret = new OrdenResult { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la devolución: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                        return ret;

                    }
                    if (_operationNumber == 0)
                    {
                        var ret = new OrdenResult { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la devolución: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                        return ret;
                    }
                    #endregion

                    var simadi = Services.miscelaneos.SetDevolucionDivisas(orden.id, Convert.ToInt16(orden.documentosRegla ? 1 : 0), orden.tipoCancelacion, user.login, _operationNumber, user.idSucursal, string.Empty);
                    if (simadi.FirstChild != simadi.SelectSingleNode("//ERROR"))
                    {
                        if (Convert.ToBoolean(simadi.SelectSingleNode("//INSERTADO").InnerText))
                        {
                            var update = Services.miscelaneos.SetActualizaOrdenDevolucionDivisas(string.Empty, string.Empty, string.Empty, orden.id, "Devolución de Orden Procesada con Éxito.", string.Empty);
                            //if (orden.denominaciones != null)
                            //{
                            //    if (orden.denominaciones.Count > 0)
                            //    {
                            var n = string.Empty;
                            long id_detalle = 0;
                            var dp = Services.pagos.SetDetallePagoRealizado(orden.id,orden.tipo_pago,orden.detalle_tipo_pago,
                                user.login, string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                                orden.montoDevolver, orden.referencia, n, n,
                                orden.fecha, n);

                            if (dp.FirstChild != dp.SelectSingleNode("//ERROR"))
                            {
                                if (!Convert.ToBoolean(dp.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                                    return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la devolución de forma exitosa, por no poder registrar los detalles del pago realizado, por favot notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la devolución de forma exitosa, por favot notifique al Dpto. de Sistemas.") };
                                //else
                                //{
                                //    id_detalle = Convert.ToInt64(dp.SelectSingleNode("//ID_OPERACION").InnerText.Trim());

                                //    foreach (var d in orden.denominaciones)
                                //    {
                                //        var rd = Services.pagos.SetDesgloseEfectivoPagoRealizado(id_detalle,
                                //            d.idDenominacion, user.login, d.cantidad,
                                //            d.subTotal, string.Format("{0:yyyy-MM-dd}", DateTime.Now), n);
                                //    }
                                //}

                            }
                            //    }
                            //}

                            Services.utilitarios.SetTrazaSeguridad(37, 3, "ACTIVO", "DEVUELTO", "DEVOLUCIÓN DE LA OPERACION NUMERO: " + user.letraSucursal + "-" + orden.numero, user.login, "");
                        }
                        else
                            return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la devolución de forma exitosa, por favot notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la devolución de forma exitosa, por favot notifique al Dpto. de Sistemas.") };
                    }
                    else
                        return new OrdenResult() { error = true, clientErrorDetail = "Error: " + simadi.SelectSingleNode("ERROR").InnerText, status = "error", errorDetail = new Exception(simadi.SelectSingleNode("ERROR").InnerText) };

                    break;
                default:
                    return new OrdenResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la devolución de forma exitosa, por favor notifique al Dpto. de Sistemas.", status = "error", errorDetail = new Exception("No se ha logrado ejecutar la devolución de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
            }
            //}
            return new OrdenResult() { status = "ok", idResult = orden.id.ToString() };
        }

        public static List<Denominaciones> getDenominacionesDisponibles(string usr, string moneda)
        {
            try
            {
                XDocument xd = new XDocument();
                var n = string.Empty;
                xd = XDocument.Parse(Services.monetarios.GetDesgloseEfectivoPorSucursalOUsuario(n, usr, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), n).OuterXml);
                var result = (from r in xd.Descendants("DESGLOSE")
                              select new Denominaciones
                              {
                                  idDenominacion = Convert.ToInt32(r.Element("ID_DENOMINACION").Value),
                                  denominacion = Convert.ToDecimal(r.Element("DENOMINACION").Value, wsCulture),
                                  idTipoDenominacion = Convert.ToInt32(r.Element("ID_TIPO_DENOMINACION").Value),
                                  tipoDenominacion = r.Element("TIPO_DENOMINACION").Value,
                                  cantidad = Convert.ToInt32(r.Element("CANTIDAD_DENOMINACIONES").Value),
                                  moneda = r.Element("MONEDA").Value,
                                  subTotal = Convert.ToDecimal(r.Element("DENOMINACION").Value, wsCulture) * Convert.ToInt32(r.Element("CANTIDAD_DENOMINACIONES").Value)
                              }).ToList();
                if (!string.IsNullOrEmpty(moneda))
                    result = result.Where(x => x.moneda == moneda).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Denominaciones>() { new Denominaciones { clientErrorDetail = "Error al Cargar las denominaciones disponibles para el usuario actual.", error = true, apiDetail = "getDenominaciones", errorDetail = ex } };
                return ret;
            }

        }

        public static List<Sucursal> getSucursales()
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetSucursales(false, string.Empty).OuterXml);
                var result = (from r in xd.Descendants("OFICINAS")
                              select new Sucursal
                              {
                                  region = r.Element("REGION").Value,
                                  letra = r.Element("LETRA").Value,
                                  nombre = r.Element("OFICINA").Value,
                                  direccion = r.TryGetElementValue("DIRECCION"),
                                  telefonos = r.TryGetElementValue("TELEFONOS_CONTACTO"),
                                  codigo = r.Element("ID_SUCURSAL").Value,
                                  esAgencia = string.IsNullOrEmpty(r.TryGetElementValue("ES_AGENCIA")) ? false : Convert.ToBoolean(r.TryGetElementValue("ES_AGENCIA"))
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Sucursal> { new Sucursal { clientErrorDetail = "Ha ocurrido un error al obtener las Sucursales activas de CCAL.", error = true, apiDetail = "getSucursales", errorDetail = ex } };
                return ret;
            }
        }

        public static List<Pase> getPases(PaseRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(req.current);
                xd = XDocument.Parse(Services.monetarios.GetPaseEfectivo(user.idSucursal, req.numero, string.Empty).OuterXml);
                var result = (from r in xd.Descendants("PASES")
                              select new Pase
                              {
                                  monto = Convert.ToDecimal(r.Element("MONTO").Value, wsCulture),
                                  fecha = DateTime.Parse(r.Element("FECHA").Value, wsCulture),
                                  fechaMostrar = r.Element("FECHA").Value,
                                  login = r.Element("LOGIN_USUARIO").Value,
                                  nombreUsuario = r.TryGetElementValue("USUARIO"),
                                  numeroControl = Convert.ToInt64(r.Element("NUMERO_CONTROL").Value),
                                  moneda = r.Element("NOMBRE_MONEDA").Value,
                                  codigoMoneda = r.Element("CODIGO_INT").Value,
                                  simbolo = r.Element("SIMBOLO").Value
                              }).Where(x => x.fecha.Date <= DateTime.Now.Date).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Pase>() { new Pase { clientErrorDetail = "Error al Cargar la información del Pase", error = true, apiDetail = "getPases", errorDetail = ex } };
                return ret;
            }

        }

        public static List<OrdenCompraCorresponsal> getOrdenesDisponiblesPago(BeneficiarioCorresponsalRequest req)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.pagos.GetRemesasDisponiblesPago(req.corresponsal, req.referencia, string.Format("{0}{1}", req.tipoId, req.numeroId), string.Empty).OuterXml);
                var result = (from r in xd.Descendants("OPERACION")
                              select new OrdenCompraCorresponsal
                              {
                                  identificacionBeneficiario = r.Element("IDENTIFICACIONBENEFICIARIO").Value,
                                  nombreBeneficiario = r.Element("NOMBREBENEFICIARIO").Value,
                                  telefonoBeneficiario = r.Element("TELEFONOBENEFICIARIO").Value,
                                  telefono2Beneficiario = r.TryGetElementValue("TELEFONO2BENEFICIARIO"),
                                  referencia = r.Element("REFERENCIA").Value,
                                  referenciaBCV = r.Element("REFERENCIABCV").Value,
                                  secuencia = Convert.ToInt64(r.Element("SECUENCIA").Value),
                                  modo = r.Element("MODO").Value,
                                  corresponsal = r.Element("CORRESPONSAL").Value,
                                  codigoCorresponsal = r.Element("CODIGOCORRESPONSAL").Value,
                                  montoPagar = Convert.ToDecimal(r.Element("MONTOPAGAR").Value, wsCulture),
                                  fechaOrden = DateTime.Parse(r.Element("FECHAORDEN").Value),
                                  fechaOrdenMostrar = r.Element("FECHAORDEN").Value,
                                  monedaMonto = r.Element("MONEDAMONTO").Value,
                                  id = Convert.ToInt64(r.Element("ID").Value),
                                  identificacionOrdenante = r.Element("IDENTIFICACIONORDENANTE").Value,
                                  nombreOrdenante = r.Element("NOMBREORDENANTE").Value,
                                  telefonoOrdenante = r.Element("TELEFONOORDENANTE").Value,
                                  tasaExterna = Convert.ToDecimal(r.Element("TASA").Value, wsCulture),
                                  codigoPais = r.Element("CODIGOPAIS").Value,
                                  nombrePais = r.Element("NOMBREPAIS").Value,
                                  isoCodePais = r.Element("ISOCODEPAIS").Value,
                                  idStatus = Convert.ToInt32(r.Element("STATUSID").Value),
                                  statusOrden = r.Element("STATUSORDEN").Value,
                                  tipoIdBeneficiario = r.Element("IDENTIFICACIONBENEFICIARIO").Value == string.Empty ? string.Empty : r.Element("IDENTIFICACIONBENEFICIARIO").Value.Substring(0, 1),
                                  numeroIdBeneficiario = r.Element("IDENTIFICACIONBENEFICIARIO").Value == string.Empty ? string.Empty : r.Element("IDENTIFICACIONBENEFICIARIO").Value.Replace(r.Element("IDENTIFICACIONBENEFICIARIO").Value.Substring(0, 1), "")
                              }).OrderBy(x => x.fechaOrden).ToList();

                #region Sin Compra BCV
                /*Se  verifica si las operaciones no estan registradas en el BCV y se agrega la tasa del día*/
                foreach (var item in result)
                {
                    if (item.referenciaBCV == null || item.referenciaBCV == "")
                    {
                        var tasa = getTasaCambio();
                        if (tasa != null)
                        {
                            item.tasaExterna = tasa.compra;
                        }
                        else
                        {
                            var ret = new List<OrdenCompraCorresponsal>() { new OrdenCompraCorresponsal { clientErrorDetail = "Error al Cargar el listado de Ordenes Disponibles, no se logro consultar la tasa del día", error = true, apiDetail = "getOrdenesDisponiblesPago" } };
                            return ret;
                        }
                    }
                }
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenCompraCorresponsal>() { new OrdenCompraCorresponsal { clientErrorDetail = "Error al Cargar el listado de Ordenes Disponibles", error = true, apiDetail = "getOrdenesDisponiblesPago", errorDetail = ex } };
                return ret;
            }
        }

        public static List<OrdenAnulacion> SearchOrderAnulByAuthorization(string userLogin)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(userLogin);
                xd = XDocument.Parse(Services.ordenes.SearchOrderAnulByAuthorization(user.idOficinaNew, DateTime.Now).OuterXml);
                var result = (from r in xd.Descendants("ORDEN")
                              select new OrdenAnulacion
                              {
                                  id = int.Parse(r.Element("ID_ORDEN").Value),
                                  letra = r.Element("LETRA").Value,
                                  numero = int.Parse(r.Element("NUMERO").Value),
                                  status = int.Parse(r.Element("STATUS_ORDEN").Value),
                                  fecha = r.Element("FECHA_OPERACION").Value,
                                  monto = decimal.Parse(r.Element("MONTO").Value, wsCulture),
                                  descripcionStatus = r.Element("STATUSNAME").Value,
                                  motivo = r.Element("REFERENCIA_ANULACION").Value,
                                  tipoOpBcv = r.Element("TIPO_OP_BCV").Value,
                                  referenciaBCV = r.Element("REFERENCIA_ORDEN").Value,
                                  AnulAutorizadaPor = r.Element("ANULAUTORIZADAPOR").Value,
                                  ReferenciaAnulBcv = r.Element("REFERENCIAANULBCV").Value,
                                  Procesada = bool.Parse(r.Element("ANULPROCESADA").Value),
                                  AnuladaPor = r.Element("ANULADAPOR").Value,
                                  UsuarioAnula = r.Element("USUARIOANULA").Value,
                                  nombresCliente = r.Element("NOMBRES_REMITENTE").Value + " " +
                                                    r.Element("APELLIDOS_REMITENTE").Value,
                                  BeneficiarioName = r.Element("NOMBRES_BENEFICIARIO").Value + " " +
                                                    r.Element("APELLIDOS_BENEFICIARIO").Value,
                                  tabla = r.Element("TABLA").Value,
                                  simadi = bool.Parse(r.Element("SIMADI").Value),
                                  NumberTypeOperation = int.Parse(r.Element("NUMBERTYPEOPERATION").Value),
                                  Moneda = r.Element("MONEDA").Value,
                                  SimboloMoneda = r.Element("SIMBOLOMONEDA").Value,
                                  TypeOperationName = r.Element("TYPEOPERATIONNAME").Value

                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenAnulacion>() { new OrdenAnulacion { clientErrorDetail = "Error al Cargar el listado de Ordenes por Autorizar", error = true, apiDetail = "SearchOrderAnulByAuthorization", errorDetail = ex } };
                return ret;
            }

        }

        public static bool UpdateRejectAnnulation(int ordenId, string ordenType)
        {
            var p = Services.ordenes.UpdateRejectAnnulation(ordenId, ordenType);
            if (p.FirstChild != p.SelectSingleNode("//ERROR"))
            {
                if (!Convert.ToBoolean(p.SelectSingleNode("//ACTUALIZADO").InnerText.Trim()))
                    return false;
            }
            else
                return false;

            return true;
        }

        public static List<OrdenAnulacion> SearchOrder(int orderId, int clientId)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.ordenes.SearchOrder(orderId, clientId).OuterXml);
                var result = (from r in xd.Descendants("ORDEN")
                              select new OrdenAnulacion
                              {
                                  id = int.Parse(r.Element("ID_ORDEN").Value),
                                  letra = r.Element("LETRA").Value,
                                  numero = int.Parse(r.Element("NUMERO").Value),
                                  status = int.Parse(r.Element("STATUS_ORDEN").Value),
                                  fecha = r.Element("FECHA_OPERACION").Value,
                                  monto = decimal.Parse(r.Element("MONTO").Value, wsCulture),
                                  descripcionStatus = r.Element("STATUSNAME").Value,
                                  motivo = r.Element("REFERENCIA_ANULACION").Value,
                                  tipoOpBcv = r.Element("TIPO_OP_BCV").Value,
                                  referenciaBCV = r.Element("REFERENCIA_ORDEN").Value,
                                  AnulAutorizadaPor = r.Element("ANULAUTORIZADAPOR").Value,
                                  ReferenciaAnulBcv = r.Element("REFERENCIAANULBCV").Value,
                                  Procesada = bool.Parse(r.Element("ANULPROCESADA").Value),
                                  AnuladaPor = r.Element("ANULADAPOR").Value,
                                  UsuarioAnula = r.Element("USUARIOANULA").Value,
                                  nombresCliente = r.Element("NOMBRES_REMITENTE").Value + " " +
                                                    r.Element("APELLIDOS_REMITENTE").Value,
                                  BeneficiarioName = r.Element("NOMBRES_BENEFICIARIO").Value + " " +
                                                    r.Element("APELLIDOS_BENEFICIARIO").Value,
                                  tipoIdCliente = r.Element("IDENTIFICACION_REMITENTE").Value.Substring(0, 1),
                                  numeroIdCliente = r.Element("IDENTIFICACION_REMITENTE").Value.Substring(1,
                                    r.Element("IDENTIFICACION_REMITENTE").Value.Length - 1),
                                  tabla = r.Element("TABLA").Value,
                                  observaciones = r.Element("OBSERVACIONES").Value,
                                  simadi = bool.Parse(r.Element("SIMADI").Value),
                                  NumberTypeOperation = int.Parse(r.Element("NUMBERTYPEOPERATION").Value),
                                  Moneda = r.Element("MONEDA").Value,
                                  SimboloMoneda = r.Element("SIMBOLOMONEDA").Value,
                                  TypeOperationName = r.Element("TYPEOPERATIONNAME").Value,
                                  TypeOperationId = r.Element("TIPO_OP_BCV").Value,


                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<OrdenAnulacion>() { new OrdenAnulacion { clientErrorDetail = "Error al Cargar el listado de Ordenes por Autorizar", error = true, apiDetail = "SearchOrderAnulByAuthorization", errorDetail = ex } };
                return ret;
            }

        }

        public static List<Application> SearchApplication(int applicationId)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.SearchApplications(applicationId).OuterXml);
                var result = (from r in xd.Descendants("APPLICATIONS")
                              select new Application
                              {
                                  ApplicationId = int.Parse(r.Element("APPLICATIONID").Value),
                                  StatusId = r.Element("STATUSID").Value,
                                  StatusName = r.Element("STATUSNAME").Value,
                                  UserCreteId = r.Element("USERCREATEID").Value,
                                  UserCreateName = r.Element("USERCREATENAME").Value,
                                  UserModifiedId = r.Element("USERMODIFIEDID").Value,
                                  UserModifiedName = r.Element("USERMODIFIEDNAME").Value,
                                  ApplicationName = r.Element("APPLICATIONNAME").Value,
                                  ApplicationDateCreate = DateTime.Parse(r.Element("APPLICATIONDATECREATE").Value, wsCulture),
                                  ApplicationDescription = r.Element("APPLICATIONDESCRIPTION").Value,
                                  ApplicationDateModified = r.Element("APPLICATIONDATEMODIFIED").Value.ToString().Trim() != "" ?
                                    DateTime.Parse(r.Element("APPLICATIONDATEMODIFIED").Value, wsCulture) : DateTime.Parse("01-01-1900 12:00:00 a.m.", wsCulture)
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Application>() { new Application { clientErrorDetail = "Error al Cargar el listado de Aplicaciones", error = true, apiDetail = "SearchApplication", errorDetail = ex } };
                return ret;
            }

        }

        public static ParametroResult InserParameter(Parametro parametro)
        {
            var operation = Services.catalogos.InsertParameter(parametro.que, int.Parse(parametro.cuanto.ToString()), parametro.cuando,
                parametro.app, parametro.exeName, parametro.referencia, parametro.RegistradoPor, parametro.Description);
            if (operation.FirstChild != operation.SelectSingleNode("ERROR"))
            {
                if (!Convert.ToBoolean(operation.SelectSingleNode("//INSERTADO").InnerText))
                    return new ParametroResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la el registro del parametro, por favor notifique al Dpto. de Sistemas.", status = false, errorDetail = new Exception("No se ha logrado ejecutar el registro del parametro de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
            }
            else
                return new ParametroResult() { error = true, clientErrorDetail = "Error: " + operation.SelectSingleNode("ERROR").InnerText, status = true, errorDetail = new Exception(operation.SelectSingleNode("ERROR").InnerText) };

            return new ParametroResult { status = true };
        }

        public static ParametroResult UpdateParameter(Parametro parametro)
        {
            var operation = Services.catalogos.UpdateParameter(parametro.IdParametro, parametro.que, int.Parse(parametro.cuanto.ToString()), parametro.cuando,
                parametro.app, parametro.exeName, parametro.referencia, parametro.Description, parametro.ModificadoPor);
            if (operation.FirstChild != operation.SelectSingleNode("ERROR"))
            {
                if (!Convert.ToBoolean(operation.SelectSingleNode("//ACTUALIZADO").InnerText))
                    return new ParametroResult() { error = true, clientErrorDetail = "Error: No se ha logrado ejecutar la actualización del parametro, por favor notifique al Dpto. de Sistemas.", status = false, errorDetail = new Exception("No se ha logrado ejecutar la actualización del parametro de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
            }
            else
                return new ParametroResult() { error = true, clientErrorDetail = "Error: " + operation.SelectSingleNode("ERROR").InnerText, status = true, errorDetail = new Exception(operation.SelectSingleNode("ERROR").InnerText) };

            return new ParametroResult { status = true };
        }

        public static List<Parametro> SearchParameter(ParametroRequest parametro)
        {
            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.SearchParameter(parametro.ParameterId, parametro.ParameterName).OuterXml);
                var result = (from r in xd.Descendants("PARAMETROS")
                              select new Parametro
                              {
                                  IdParametro = int.Parse(r.Element("ID_PARAMETRO").Value),
                                  que = r.Element("QUE").Value,
                                  cuanto = Int64.Parse(r.Element("CUANTO").Value),
                                  cuando = DateTime.Parse(r.Element("CUANDO").Value, wsCulture),
                                  app = r.Element("APP").Value,
                                  exeName = r.Element("EXE_NAME").Value,
                                  referencia = r.Element("REFERENCIA").Value,
                                  RegistradoPor = r.Element("REGISTRADOPOR").Value,
                                  Description = r.Element("DESCRIPTION").Value,
                                  UserName = r.Element("USERNAME").Value,
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<Parametro>() { new Parametro { clientErrorDetail = "Error al Cargar el listado de Parametros", error = true, apiDetail = "SearchParameter", errorDetail = ex } };
                return ret;
            }

        }

        public static List<MonetarySummary> SearchMonetarySummary(string userLogin)
        {
            try
            {
                XDocument xd = new XDocument();
                var user = getUsuario(userLogin);
                xd = XDocument.Parse(Services.monetarios.SearchMonetaryDetail(user.idSucursal, DateTime.Now).OuterXml);
                var result = (from r in xd.Descendants("DISPONIBLE")
                              select new MonetarySummary
                              {
                                  NombreCajero = r.Element("USUARIO").Value,
                                  LoginCajero = r.Element("LOGIN_USUARIO").Value,
                                  //Moneda = r.Element("MONEDA").Value,
                                  CodigoMoneda = r.Element("MONEDA").Value,
                                  Monto = decimal.Parse(r.Element("MONTO").Value, wsCulture)
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                var ret = new List<MonetarySummary>() { new MonetarySummary { clientErrorDetail = "Error al Cargar el resumen monetario por cajero", error = true, apiDetail = "SearchMonetarySummary", errorDetail = ex } };
                return ret;
            }
        }

        public static Remesa SearchBoveda(Remesa remesa)
        {
            try
            {
                //XDocument xd = new XDocument();
                var user = getUsuario(remesa.UserId);
                var xd = Services.monetarios.SearchBoveda(remesa.RemesaId, user.idSucursal);
                if (xd.FirstChild != xd.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(xd.SelectSingleNode("//ENCONTRADO").InnerText))
                    {
                        XDocument xdd = new XDocument();
                        xdd = XDocument.Parse(xd.OuterXml);
                        Remesa result = (from r in xdd.Descendants("BOVEDA")
                                         select new Remesa
                                         {
                                             RemesaId = int.Parse(r.Element("NUMERO_CONTROL").Value),
                                             TransporteId = int.Parse(r.Element("ID_TRANSPORTE").Value),
                                             TransporteName = r.Element("TRANSPORTE_VALORES").Value,
                                             MonedaId = r.Element("ID_MONEDA").Value,
                                             MonedaName = r.Element("MONEDANAME").Value,
                                             Cataporte = r.Element("CATAPORTE").Value,
                                             Precinto = r.Element("PRECINTO").Value,
                                             UserId = r.Element("CAJERO").Value,
                                             UserName = r.Element("USERNAME").Value,
                                             TypeRemesaId = r.Element("TYPEREMESAID").Value,
                                             TypeRemesa = r.Element("TYPEREMESA").Value,
                                             TotalMoneda = 0,
                                             TotalBillete = 0,
                                             Total = 0,
                                             UsuarioAnula = r.Element("USUARIOANULA").Value,
                                             CoordinadorAutoriza = r.Element("COORDINADORAUTORIZA").Value,
                                             MotivoAnulacion = int.Parse(r.Element("MOTIVOANULACION").Value),
                                             ObservacionAnulacion = r.Element("OBSERVACIONANULACION").Value,
                                             MotivoAnulacionName = r.Element("MOTIVOANULACIONNAME").Value,
                                             BranchId = r.Element("SUCURSAL").Value,
                                             LetraBranch = r.Element("LETRABRANCH").Value,
                                             Denominaciones = (from d in xdd.Descendants("BOVEDA")
                                                               select new Denominaciones
                                                               {
                                                                   idDenominacion = int.Parse(d.Element("ID_DENOMINACION").Value),
                                                                   denominacion = decimal.Parse(d.Element("DENOMINACION").Value, wsCulture),
                                                                   idTipoDenominacion = int.Parse(d.Element("TIPO_DENOMINACION").Value),
                                                                   tipoDenominacion = "",
                                                                   cantidad = int.Parse(d.Element("CANTIDAD").Value),
                                                                   moneda = d.Element("ID_MONEDA").Value,
                                                                   subTotal = decimal.Parse(d.Element("TOTAL").Value, wsCulture)
                                                               }).ToList()
                                         }).FirstOrDefault();


                        result.TotalMoneda = result.Denominaciones.Where(x => x.idTipoDenominacion == 1).Sum(s => s.subTotal);
                        result.TotalBillete = result.Denominaciones.Where(x => x.idTipoDenominacion == 2).Sum(s => s.subTotal);
                        result.Total = result.TotalBillete + result.TotalMoneda;
                        return result;
                    }
                    else
                    {
                        return new Remesa { error = false };

                    }

                }
                else
                {
                    return new Remesa { error = true, clientErrorDetail = xd.SelectSingleNode("//ERROR").InnerText };
                }



            }
            catch (Exception ex)
            {
                var ret = new Remesa { clientErrorDetail = "Error al consultar la boveda ", error = true, apiDetail = "SearchBoveda", errorDetail = ex };
                return ret;
            }
        }

        public static Remesa ReverseConsignment(Remesa remesa)
        {
            //try
            //{
            //    var user = getUsuario(remesa.UserId);
            //    var operation = Services.monetarios.ReverseConsignment(remesa.RemesaId, user.idSucursal, remesa.TypeRemesaId);
            //    if (operation.FirstChild != operation.SelectSingleNode("ERROR"))
            //    {
            //        if (!Convert.ToBoolean(operation.SelectSingleNode("//ELIMINADO").InnerText))
            //            return new Remesa{ error = true, clientErrorDetail = "Error: No se ha logrado reversar la remesa.", errorDetail = new Exception("No se ha logrado ejecutar la actualización del parametro de forma exitosa, por favor notifique al Dpto. de Sistemas.") };
            //    }
            //    else
            //        return new Remesa{ error = true, clientErrorDetail = "Error: " + operation.SelectSingleNode("ERROR").InnerText,  errorDetail = new Exception(operation.SelectSingleNode("ERROR").InnerText) };

            //    return new Remesa { error = false };
            //}
            //catch (Exception ex)
            //{
            //    var ret = new Remesa { clientErrorDetail = "Error al consultar la boveda ", error = true, apiDetail = "SearchBoveda", errorDetail = ex };
            //    return ret;
            //}
            var fact = Services.ordenes.UpdateAnulRemesa(remesa.NumeroControl, remesa.ObservacionAnulacion, remesa.MotivoAnulacion == 0 ? "" : remesa.MotivoAnulacion.ToString(),
                    remesa.CoordinadorAutoriza, remesa.Procesado, remesa.UsuarioAnula, remesa.TypeRemesaId, remesa.BranchId);
            if (fact.FirstChild == fact.SelectSingleNode("ERROR"))
                return new Remesa()
                {
                    error = true,
                    clientErrorDetail = "Error: " + fact.SelectSingleNode("ERROR").InnerText,
                    errorDetail = new Exception(fact.SelectSingleNode("ERROR").InnerText)
                };
            return new Remesa() { error = false };
        }

        public static List<Remesa> SearchRemesaByAuthorize(string userLogin)
        {

            try
            {
                var user = getUsuario(userLogin);
                var xd = Services.ordenes.SearchRemesaByAuthorize(user.idSucursal, DateTime.Now);
                List<Remesa> result = new List<Remesa>();
                if (xd.FirstChild != xd.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(xd.SelectSingleNode("//ENCONTRADO").InnerText))
                    {
                        XDocument xdd = new XDocument();
                        xdd = XDocument.Parse(xd.OuterXml);

                        string filter = "";

                        foreach (var r in xdd.Descendants("REMESA"))
                        {
                            var inf = result.Where(x => x.RemesaId == int.Parse(r.Element("NUMERO_CONTROL").Value)).ToList();
                            if (inf.Count == 0)
                            {
                                var obj = new Remesa
                                {
                                    RemesaId = int.Parse(r.Element("NUMERO_CONTROL").Value),
                                    TransporteId = int.Parse(r.Element("ID_TRANSPORTE").Value),
                                    TransporteName = r.Element("TRANSPORTE_VALORES").Value,
                                    MonedaId = r.Element("ID_MONEDA").Value,
                                    MonedaName = r.Element("MONEDANAME").Value,
                                    Cataporte = r.Element("CATAPORTE").Value,
                                    Precinto = r.Element("PRECINTO").Value,
                                    TypeRemesaId = r.Element("TYPEREMESAID").Value,
                                    TypeRemesa = r.Element("TYPEREMESA").Value,
                                    TotalMoneda = 0,
                                    TotalBillete = 0,
                                    Total = 0,
                                    UsuarioAnula = r.Element("USUARIOANULA").Value,
                                    UsuarioAnulaName = r.Element("USUARIOANULANAME").Value,
                                    CoordinadorAutoriza = r.Element("COORDINADORAUTORIZA").Value,
                                    MotivoAnulacion = int.Parse(r.Element("MOTIVOANULACION").Value),
                                    ObservacionAnulacion = r.Element("OBSERVACIONANULACION").Value,
                                    MotivoAnulacionName = r.Element("MOTIVOANULACIONNAME").Value,
                                    BranchId = r.Element("SUCURSAL").Value,
                                    Denominaciones = (from d in xdd.Descendants().Where(d => d.Name == "REMESA"
                                   && d.Descendants().Any(e => e.Name == "NUMERO_CONTROL"
                                     && e.Value == r.Element("NUMERO_CONTROL").Value))
                                                      select new Denominaciones
                                                      {
                                                          idDenominacion = int.Parse(d.Element("ID_DENOMINACION").Value),
                                                          denominacion = decimal.Parse(d.Element("DENOMINACION").Value, wsCulture),
                                                          idTipoDenominacion = int.Parse(d.Element("TIPO_DENOMINACION").Value),
                                                          tipoDenominacion = "",
                                                          cantidad = int.Parse(d.Element("CANTIDAD").Value),
                                                          moneda = d.Element("ID_MONEDA").Value,
                                                          subTotal = decimal.Parse(d.Element("TOTAL").Value, wsCulture)
                                                      }).ToList()
                                };
                                obj.TotalMoneda = obj.Denominaciones.Where(x => x.idTipoDenominacion == 1).Sum(s => s.subTotal);
                                obj.TotalBillete = obj.Denominaciones.Where(x => x.idTipoDenominacion == 2).Sum(s => s.subTotal);
                                obj.Total = obj.TotalBillete + obj.TotalMoneda;
                                result.Add(obj);
                                filter += "," + obj.NumeroControl.ToString();
                            }
                        }
                        return result;
                    }
                    else
                    {
                        return result;

                    }

                }
                else
                {
                    return new List<Remesa> { new Remesa { error = true, clientErrorDetail = xd.SelectSingleNode("//ERROR").InnerText } };
                }



            }
            catch (Exception ex)
            {
                var ret = new List<Remesa> { new Remesa { clientErrorDetail = "Error al consultar la boveda ", error = true, apiDetail = "SearchBoveda", errorDetail = ex } };
                return ret;
            }
        }

        public static bool UpdateRemesaRejectAnulation(Remesa remesa)
        {
            var p = Services.ordenes.UpdateRemesaRejectAnulation(remesa.RemesaId, remesa.TypeRemesaId, remesa.BranchId);
            if (p.FirstChild != p.SelectSingleNode("//ERROR"))
            {
                if (!Convert.ToBoolean(p.SelectSingleNode("//ACTUALIZADO").InnerText.Trim()))
                    return false;
            }
            else
                return false;

            return true;
        }

        public static Remesa SearchRemesaChequesAnul(Remesa remesa)
        {
            try
            {
                //XDocument xd = new XDocument();
                var user = getUsuario(remesa.UserId);
                var xd = Services.Egresos.SearchRemesaChequesAnul(remesa.RemesaId, user.idSucursal);
                if (xd.FirstChild != xd.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(xd.SelectSingleNode("//ENCONTRADO").InnerText))
                    {
                        XDocument xdd = new XDocument();
                        xdd = XDocument.Parse(xd.OuterXml);
                        Remesa result = (from r in xdd.Descendants("REMESA")
                                         select new Remesa
                                         {
                                             RemesaId = int.Parse(r.Element("NUMERO_CONTROL").Value),
                                             TransporteId = int.Parse(r.Element("TRANSPORTEID").Value),
                                             TransporteName = r.Element("TRANSPORTENAME").Value,
                                             MonedaId = r.Element("MONEDAID").Value,
                                             MonedaName = r.Element("MONEDANAME").Value,
                                             Cataporte = r.Element("CATAPORTE").Value,
                                             Precinto = r.Element("PRECINTO").Value,
                                             UserId = r.Element("USERID").Value,
                                             UserName = r.Element("USERNAME").Value,
                                             TypeRemesaId = r.Element("TYPEREMESAID").Value,
                                             TypeRemesa = r.Element("TYPEREMESA").Value,
                                             Total = 0,
                                             UsuarioAnula = r.Element("USERANULAID").Value,
                                             CoordinadorAutoriza = r.Element("USERAUTORIZAID").Value,
                                             MotivoAnulacion = int.Parse(r.Element("MOTIVOANULACION").Value),
                                             ObservacionAnulacion = r.Element("MOTIVOANULACIONNAME").Value,
                                             MotivoAnulacionName = r.Element("MOTIVOANULACIONNAME").Value,
                                             BranchId = r.Element("BRANCHID").Value,
                                             LetraBranch = r.Element("BRANCHLETRA").Value,
                                             UsuarioTransladaId = r.Element("USUARIOTRANSLADAID").Value,
                                             UsuarioTransladaName = r.Element("USUARIOTRANSLADANAME").Value,
                                             Cheques = (from d in xdd.Descendants("REMESA")
                                                        select new Cheques
                                                        {
                                                            NumeroCheque = d.Element("NUMEROCHEQUE").Value,
                                                            Monto = decimal.Parse(d.Element("MONTO").Value, wsCulture),
                                                            BancoId = int.Parse(d.Element("BANCOID").Value),
                                                            BancoName = d.Element("BANCO").Value
                                                        }).ToList()
                                         }).FirstOrDefault();


                        result.Total = result.Cheques.Sum(s => s.Monto);
                        return result;
                    }
                    else
                    {
                        return new Remesa { error = false };

                    }

                }
                else
                {
                    return new Remesa { error = true, clientErrorDetail = xd.SelectSingleNode("//ERROR").InnerText };
                }



            }
            catch (Exception ex)
            {
                var ret = new Remesa { clientErrorDetail = "Error al consultar la Remesa en Cheques ", error = true, apiDetail = "SearchBoveda", errorDetail = ex };
                return ret;
            }
        }

        public static Remesa SearchRemesaChequesAuthorization(Remesa remesa)
        {
            try
            {
                //XDocument xd = new XDocument();
                var user = getUsuario(remesa.UserId);
                var xd = Services.Egresos.SearchRemesaChequesAuthorization(user.idSucursal, remesa.Autorizar, remesa.UserId);
                if (xd.FirstChild != xd.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(xd.SelectSingleNode("//ENCONTRADO").InnerText))
                    {
                        XDocument xdd = new XDocument();
                        xdd = XDocument.Parse(xd.OuterXml);
                        Remesa result = (from r in xdd.Descendants("REMESA")
                                         select new Remesa
                                         {
                                             RemesaId = int.Parse(r.Element("NUMERO_CONTROL").Value),
                                             TransporteId = int.Parse(r.Element("TRANSPORTEID").Value),
                                             TransporteName = r.Element("TRANSPORTENAME").Value,
                                             MonedaId = r.Element("MONEDAID").Value,
                                             MonedaName = r.Element("MONEDANAME").Value,
                                             Cataporte = r.Element("CATAPORTE").Value,
                                             Precinto = r.Element("PRECINTO").Value,
                                             UserId = r.Element("USERID").Value,
                                             UserName = r.Element("USERNAME").Value,
                                             TypeRemesaId = r.Element("TYPEREMESAID").Value,
                                             TypeRemesa = r.Element("TYPEREMESA").Value,
                                             Total = 0,
                                             UsuarioAnula = r.Element("USERANULAID").Value,
                                             CoordinadorAutoriza = r.Element("USERAUTORIZAID").Value,
                                             MotivoAnulacion = int.Parse(r.Element("MOTIVOANULACION").Value),
                                             ObservacionAnulacion = r.Element("MOTIVOANULACIONNAME").Value,
                                             MotivoAnulacionName = r.Element("MOTIVOANULACIONNAME").Value,
                                             BranchId = r.Element("BRANCHID").Value,
                                             LetraBranch = r.Element("BRANCHLETRA").Value,
                                             UsuarioAnulaName = r.Element("USERANULANAME").Value,
                                             UsuarioTransladaId = r.Element("USUARIOTRANSLADAID").Value,
                                             UsuarioTransladaName = r.Element("USUARIOTRANSLADANAME").Value,
                                             Cheques = (from d in xdd.Descendants("REMESA")
                                                        select new Cheques
                                                        {
                                                            NumeroCheque = d.Element("NUMEROCHEQUE").Value,
                                                            Monto = decimal.Parse(d.Element("MONTO").Value, wsCulture),
                                                            BancoId = int.Parse(d.Element("BANCOID").Value),
                                                            BancoName = d.Element("BANCO").Value
                                                        }).ToList()
                                         }).FirstOrDefault();


                        result.Total = result.Cheques.Sum(s => s.Monto);
                        return result;
                    }
                    else
                    {
                        return new Remesa { error = false };

                    }

                }
                else
                {
                    return new Remesa { error = true, clientErrorDetail = xd.SelectSingleNode("//ERROR").InnerText };
                }



            }
            catch (Exception ex)
            {
                var ret = new Remesa { clientErrorDetail = "Error al consultar la Remesa en Cheques ", error = true, apiDetail = "SearchBoveda", errorDetail = ex };
                return ret;
            }
        }

        public static usuarioDisponibilidadRet SearchDisponibilidadMonetaria(string user, decimal monto)
        {
            var ret = new usuarioDisponibilidadRet();
            ret.disponible = false;
            var usr = getUsuario(user);
            decimal montoMaximoCajeroVEF = 0;
            var controlCajero = Services.monetarios.SearchMonetaryDetail(usr.idSucursal, DateTime.Now);
            if (controlCajero.FirstChild != controlCajero.SelectSingleNode("//ERROR"))
            {
                if (Convert.ToBoolean(controlCajero.SelectSingleNode("//ENCONTRADO").InnerText.Trim()))
                {
                    if (controlCajero.SelectSingleNode("descendant::DISPONIBLE[LOGIN_USUARIO='" + usr.login + "' and MONEDA = 'VEB']") != null)
                    {
                        montoMaximoCajeroVEF = decimal.Parse(controlCajero.SelectSingleNode("descendant::DISPONIBLE[LOGIN_USUARIO='" +
                              usr.login + "' and MONEDA = 'VEB']").SelectSingleNode("MONTO").InnerText, wsCulture);
                    }
                    if (montoMaximoCajeroVEF > monto)
                        ret.disponible = true;
                }
                else
                    ret.disponible = false;
            }
            else
                ret.disponible = false;

            return ret;
        }

        public static List<Monedas> SearchMonedas(MonedasRequest moneda)
        {
            try
            {
                ITablasMaestrasBuilder Builder = new TablasMaestrasBuilder();

                var MonedasRequest = new MonedasRequests()
                {
                    TipoCambio = moneda.TipoCambioId,
                    Activa = moneda.MonedaActiva,
                    Interna = moneda.MonedaInterna,
                    ManejoEfectivo = moneda.MonedaManejoEfectivo
                };
                var result = Builder.SearchMonedasEntity(MonedasRequest).ToList();

                var Serialize = JsonConvert.SerializeObject(result);
                var Deserialize = JsonConvert.DeserializeObject<List<Monedas>>(Serialize);

                return Deserialize;
            }
            catch (Exception ex)
            {
                var ret = new List<Monedas> { new Monedas { clientErrorDetail = "Error al consultar las monedas utilizadas por el sistema ", error = true, apiDetail = "SearchMonedas", errorDetail = ex } };
                return ret;
            }
        }

        public static ControlMonetario ValidateAmountEfectivo(ControlMonetarioRequest req)
        {
            try
            {
                var infmoneda = new Monedas();
                if (req.MonedaId != 213)
                {
                    var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                    if (infMonedas.Count() == 0)
                    {
                        return new ControlMonetario
                        {
                            error = true,
                            clientErrorDetail = "No se ha localizado la tasa de conversión para la monedas seleccionadas"
                        };
                    }
                    else
                    {

                        infmoneda = infMonedas.Where(x => x.MonedaId == req.MonedaId).FirstOrDefault();
                        //req.montoEnviar = (req.montoEnviar * infMonedaOperacion.MonedaValorVenta??0);
                        //if (!req.Compra)
                        //    tasa = infMonedaOperacion.MonedaValorVenta ?? 0;
                        //else
                        //    tasa = infMonedaOperacion.MonedaValorCompra ?? 0;
                    }
                }

                //var result = new ControlMonetario
                //{
                //    MontoUsd = (req.MontoOperacion * (req.Compra == true ? infmoneda.MonedaValorCompra ?? 0 : infmoneda.MonedaValorVenta ?? 0)) / req.TasaDolar,
                //    MontoMaximoOperacion = ((req.MontoMaximoUsd * req.TasaDolar) / (req.Compra == true ? infmoneda.MonedaValorCompra ?? 0 : infmoneda.MonedaValorVenta ?? 0)),
                //    MontoMinimoOperacion = decimal.Round((req.MontoMinimoUsd * req.TasaDolar) / (req.Compra == true ? infmoneda.MonedaValorCompra ?? 0 : infmoneda.MonedaValorVenta ?? 0), 4)
                //};
                //modificación para ajustar los calculos de otras monedas por el valor reuters y no el valor nominal

                var result = new ControlMonetario
                {
                    MontoUsd = decimal.Round((req.MontoOperacion * (req.Compra == true ? infmoneda.MonedaValorCompraReuters ?? 0 : infmoneda.MonedaValorVentaReuters ?? 0)), 2),
                    MontoMaximoOperacion = decimal.Round((req.MontoMaximoUsd * (req.Compra == true ? infmoneda.MonedaValorCompraReuters ?? 0 : infmoneda.MonedaValorVentaReuters ?? 0)), 4),
                    MontoMinimoOperacion = decimal.Round((req.MontoMinimoUsd * (req.Compra == true ? infmoneda.MonedaValorCompraReuters ?? 0 : infmoneda.MonedaValorVentaReuters ?? 0)), 4)
                };

                return result;
            }
            catch (Exception ex)
            {
                var ret = new ControlMonetario { clientErrorDetail = "Error al Cargar las Tarifas asociadas a la operación.", error = true, apiDetail = "getResumenFinanciero", errorDetail = ex };
                return ret;
            }

        }

        public static OrdenCompraCorresponsal setCompraDivisasCorresponsalAfter(OrdenCompraCorresponsal orden)
        {
            try
            {
                #region Variables
                long _idDatosDeposito = 0;
                var user = getUsuario(orden.current);
                var codigoTipoOperacion = "compra-simadi-enc";
                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion);
                var tipoMovimiento = tm.Where(x => x.id == orden.tipoOferta).FirstOrDefault().idBcv;
                var idTipoOperacion = 10; //tipo de operacion tabla temporal, para las compra de divisas por transferencias.
                var idPagador = "CAL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = user.idCiudadExterna;
                var idOficina = user.idOficinaExterna;
                var idDetalleTipoOperacion = 8; //para las compras
                var idMoneda = 210; //el codigo de la moneda interna
                string nc1 = "", nc2 = "", nc3 = "", nc4 = ""; //nombres del ordenante
                string nb1 = "", nb2 = "", nb3 = "", nb4 = ""; //nombres del beneficiario
                var n = string.Empty;

                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                #endregion
                //Primer paso, validar si el BCV aprueba la operación

                //Comentado por Johan Cortes 21_07_2016 Proceso que se realiza cuando en el metodo de entrada de la remesa
                //var responseBCV = Services.financiero.SetCompraDivisasSIMADI(tipoMovimiento,
                //               string.Format("{0}{1}", orden.tipoIdCliente, numeroId),
                //                orden.nombresCliente,
                //                orden.montoOrden,
                //                orden.tasaCambio, "840", orden.montoOrden,
                //                Convert.ToInt64(orden.motivoOferta),
                //                n, //string.IsNullOrEmpty(string.Empty) ? "01020000000000000000" : string.Empty,
                //                string.IsNullOrEmpty(orden.telefonoCliente) ? "04141111111" : orden.telefonoCliente,
                //                string.IsNullOrEmpty(orden.emailCliente) ? "sincorreo@gmail.com" : orden.emailCliente);
                //if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                //{
                //    responseBCV = new XmlDocument();
                //    responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                //}
                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>INT" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");
                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        var ret = new OrdenCompraCorresponsal { clientErrorDetail = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", error = true, apiDetail = "setCompraDivisasCorresponsalAfter", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                        return ret;
                    }
                }
                else
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "No se logro establecer comunicación con el BCV razón: object null", error = true, apiDetail = "setCompraDivisasCorresponsalAfter", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                    return ret;
                }
                orden.referenciaBCV = responseBCV.SelectSingleNode("//result").InnerText;
                //orden.nombresBeneficiario = "Casa de Cambio Angulo López";
                //orden.tipoIdBeneficiario = "R";
                //orden.numeroIdBeneficiario = "302075661";

                #region Asignación Numero de Orden
                //var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                var num = Services.utilitarios.GetProximoNumero(4, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                long _operationNumber = 0;
                if (num.FirstChild != num.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                        _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                    else
                        _operationNumber = 0;
                }
                else
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(num.SelectSingleNode("ERROR").InnerText) };
                    return ret;

                }
                if (_operationNumber == 0)
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Numeración en cero (0)]") };
                    return ret;
                }
                #endregion
                #region Agregamos la información a la tabla temporal
                #region Mapeo de Nombres en Variables
                var nc = orden.nombreOrdenante.Split(' ');
                switch (nc.Length)
                {
                    case 5:
                        nc1 = string.Format("{0} {1}", nc[0], nc[1]);
                        nc2 = nc[2];
                        nc3 = nc[3];
                        nc4 = nc[4];
                        break;
                    case 4:
                        nc1 = nc[0];
                        nc2 = nc[1];
                        nc3 = nc[2];
                        nc4 = nc[3];
                        break;
                    case 3:
                        nc1 = nc[0];
                        nc2 = string.Empty;
                        nc3 = nc[1];
                        nc4 = nc[2];
                        break;
                    case 2:
                        nc1 = nc[0];
                        nc2 = string.Empty;
                        nc3 = nc[1];
                        nc4 = string.Empty;
                        break;
                    default:
                        break;
                }
                var nb = orden.nombreBeneficiario.Split(' ');
                switch (nb.Length)
                {
                    case 5:
                        nb1 = string.Format("{0} {1}", nb[0], nb[1]);
                        nb2 = nb[2];
                        nb3 = nb[3];
                        nb4 = nb[4];
                        break;
                    case 4:
                        nb1 = nb[0];
                        nb2 = nb[1];
                        nb3 = nb[2];
                        nb4 = nb[3];
                        break;
                    case 3:
                        nb1 = nb[0];
                        nb2 = string.Empty;
                        nb3 = nb[1];
                        nb4 = nb[2];
                        break;
                    case 2:
                        nb1 = nb[0];
                        nb2 = string.Empty;
                        nb3 = nb[1];
                        nb4 = string.Empty;
                        break;
                    default:
                        break;
                }
                #endregion
                var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                decimal comisionUs = 0, comisionBs = 0;

                if (orden.tarifas == null)
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                    return ret;
                }
                foreach (var item in orden.tarifas)
                {
                    if (item.moneda.Equals("USD"))
                    {
                        if (item.valor < 1)
                            comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                        else
                            comisionUs += Math.Round(item.valor, 2);
                    }
                    else
                    {
                        if (item.valor < 1)
                            comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                        else
                            comisionBs += Math.Round(item.valor, 2);
                    }
                }
                orden.idPaisDestino = "VZL";
                #region Order Insert

                var resultOrder = Services.miscelaneos.SetRegistrarOperacionTemporal(
                 user.letraSucursal, _operationNumber,
                 string.Format("{0}{1}", orden.identificacionOrdenante.Substring(0, 1), orden.identificacionOrdenante.Replace(orden.identificacionOrdenante.Substring(0, 1), "")),
                 nc1, nc2, nc3, nc4, n,
                 n, nb1, nb2, nb3, nb4,
                 string.Format("{0}{1}", orden.tipoIdBeneficiario, orden.numeroIdBeneficiario),
                 orden.telefonoBeneficiario, n, n, orden.idPaisDestino, idPagador, idCiudad, idOficina,
                 montoBolivares - comisionBs, orden.montoOrden - comisionUs,
                 1, orden.montoOrden - comisionUs, comisionUs,
                 comisionBs, 0, 0, n, n, user.login,
                 codigoTipoOperacion, orden.referenciaBCV,
                 1, idTipoOperacion, 0, 0, 0, _idDatosDeposito, 0, "",
                 orden.idCliente, 0, true, false, false, 0, n, n,
                 orden.tasaCambio, idDetalleTipoOperacion, idMoneda,
                 user.idOficinaNew, "TRF",
                 orden.fechaValorTasa, Convert.ToInt32(orden.motivoOferta), tipoMovimiento,
                 n, "0", orden.emailCliente, n,
                 n, "0", n, n, n, n, n, n, n, n, n, n, null, null, null, "");
                /*
                    n, n, n, n, n, n
                  string BANCO_INTERMEDIARIO, 
                  string NUMERO_CUENTA_INTERMEDIARIO,
                  string DIRECCION_BANCO_INTERMEDIARIO,
                  string ABA_INTERMEDIARIO,
                  string SWIFT_INTERMEDIARIO,
                  string IBAN_INTERMEDIARIO
               */
                if (resultOrder.FirstChild != resultOrder.SelectSingleNode("ERROR"))
                {
                    if (Convert.ToBoolean(resultOrder.SelectSingleNode("//INSERTADO").InnerText.Trim()))
                    {
                        #region Tarifas Aplicadas
                        var _idOrden = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                        if (orden.tarifas == null)
                        {
                            var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de obtener la tarifa.", error = true, apiDetail = "Orden Tarifas", errorDetail = new Exception("[Error Desconocido]") };
                            return ret;
                        }
                        foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("USD")).ToList())
                        {
                            decimal _comisionUs = 0;
                            if (item.valor < 1)
                                _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                            else
                                _comisionUs = Math.Round(item.valor, 2);

                            Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                        }

                        foreach (var item in orden.tarifas.Where(x => x.moneda.Equals("VEB")).ToList())
                        {
                            decimal _comisionBs = 0;

                            if (item.valor < 1)
                                _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                            else
                                _comisionBs = Math.Round(item.valor, 2);

                            Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                        }
                        #endregion
                        #region Actualización Parcial del registro de pago enviado por el corresponsal
                        var resultUpdate = Services.pagos.ActualizarRemesaEntranteAfter(orden.referenciaBCV, Convert.ToInt32(orden.id), Convert.ToInt32(_idOrden), orden.tipoIdCliente + orden.numeroIdCliente);
                        if (resultUpdate.FirstChild != resultUpdate.SelectSingleNode("ERROR"))
                        {
                            if (!Convert.ToBoolean(resultUpdate.SelectSingleNode("//ACTUALIZADO").InnerText.Trim()))
                            {
                                var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "SetInfoRemesaEntrante", errorDetail = new Exception("[Error Desconocido]") };
                                return ret;
                            }
                        }
                        else
                        {
                            var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultUpdate.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "SetInfoRemesaEntrante", errorDetail = new Exception(resultUpdate.SelectSingleNode("ERROR").InnerText) };
                            return ret;
                        }
                        #endregion
                    }
                    else
                    {
                        var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar la orden.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("[Error Desconocido]") };
                        return ret;
                    }
                }
                else
                {
                    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultOrder.SelectSingleNode("ERROR").InnerText + "].", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception(resultOrder.SelectSingleNode("ERROR").InnerText) };
                    return ret;
                }
                #endregion
                #endregion
                orden.idResult = orden.referenciaBCV;
                orden.temporal = 1;
                orden.id = Convert.ToInt64(resultOrder.SelectSingleNode("//ID_OPERACION").InnerText.Trim());
                //}
                //}
                //else
                //{
                //    var ret = new OrdenCompraCorresponsal { clientErrorDetail = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = new Exception("No hemos tenido respuesta del BCV para autorizar la operación.") };
                //    return ret;
                //}
            }
            catch (Exception ex)
            {
                var ret = new OrdenCompraCorresponsal { clientErrorDetail = "Ha ocurrido un error al guardar la Orden, por favor intentelo de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", error = true, apiDetail = "setVentaDivisasTransferencia", errorDetail = ex };
                return ret;
            }

            return orden;
        }

        public static GenericResponse ValidateClosingOfOperating(CloseCashierRequest model)
        {
            try
            {
                IOperacionesBuilder BuilderOperaciones = new OperacionesBuilder();
                IPassesBuilder BuilderPasses = new PassesBuilder();
                IContabilidadBuilder BuilderContabilidad = new ContabilidadBuilder();

                GenericResponse result = new GenericResponse();
                result.Valid = true;

                switch (model.TypeOfClosing)
                {
                    case 89:
                        #region Validacion Operaciones Por Cobrar activas en sistema

                        var OperacionesPorCobrarRequest = new OperacionesPorCobrarRequest()
                        {
                            Sucursal = model.BranchOffice,
                            Estatus = Constant.StatusOperacionesTemporales.StatusOperacionesActivas
                        };

                        var SearchOperacionesPorCobrar = BuilderOperaciones.SearchOperacionesPorCobrar(OperacionesPorCobrarRequest);

                        if (SearchOperacionesPorCobrar.Count() > 0)
                        {
                            var UsuariosRequest = new UsuariosRequest()
                            {
                                SUCURSAL = model.BranchOffice,
                            };

                            var ActiveCashiers = BuilderContabilidad.SearchActiveCashiers(UsuariosRequest);

                            if (ActiveCashiers.Count() < 2)
                            {
                                result.Valid = false;
                                result.Message = "No se puede realizar el cierre de Caja. Hay Operaciones Activas no procesadas";
                                break;
                            }
                        }
                        #endregion

                        #region Validacion Operaciones Por Pagar activas en sistema

                        var OPERACIONES_POR_PAGARRequest = new OPERACIONES_POR_PAGARRequest()
                        {
                            Sucursal = model.BranchOffice,
                            Estatus = Constant.StatusOperacionesTemporales.StatusOperacionesActivas
                        };

                        var SearchOperacionesPorPagar = BuilderOperaciones.SearchOperacionesPorPagar(OPERACIONES_POR_PAGARRequest);

                        if (SearchOperacionesPorPagar.Count() > 0)
                        {
                            var UsuariosRequest = new UsuariosRequest()
                            {
                                SUCURSAL = model.BranchOffice,
                            };

                            var ActiveCashiers = BuilderContabilidad.SearchActiveCashiers(UsuariosRequest);

                            if (ActiveCashiers.Count() < 2)
                            {
                                result.Valid = false;
                                result.Message = "No se puede realizar el cierre de Caja. Hay Operaciones Activas no procesadas";
                                break;
                            }
                        }

                        #endregion

                        #region Validacion Pases activos en sistema

                        var PassesRequest1 = new PassesRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            OriginUserId = model.UserId,
                            StatusId = Constant.StatusPasses.Activo,
                            Fecha = DateTime.Now
                        };

                        var SearchPasses1 = BuilderPasses.SearchPasses(PassesRequest1);

                        if (SearchPasses1.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja. Posee pases activos";
                        }

                        var PassesRequest2 = new PassesRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            DestinationUserId = model.UserId,
                            StatusId = Constant.StatusPasses.Activo,
                            Fecha = DateTime.Now
                        };

                        var SearchPasses2 = BuilderPasses.SearchPasses(PassesRequest2);

                        if (SearchPasses2.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja. Posee pases activos";

                        }

                        #endregion

                        #region Validacion Saldo disponible en caja

                        var CashierSummaryRequest = new CashierSummaryRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            StatusId = Constant.Status.Activo,
                            UserId = model.UserId
                        };

                        var data = BuilderContabilidad.SearchCashierSummary(CashierSummaryRequest);
                        var CountCashiert = data.Where(x => x.Quantity > 0).Count();

                        if (CountCashiert > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja posee Saldo Disponible.";
                            break;
                        }

                        #endregion

                        #region Validar si tiene cierre de caja operativa

                        var CloseCashier = BuilderContabilidad.SearchCloseCashier(model);

                        if (CloseCashier.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de la caja. Ya posee un cierre de caja de la fecha actual";
                            break;
                        }

                        #endregion

                        break;
                    case 90:

                        #region Validacion Pases activos en sistema

                        var PassesRequest3 = new PassesRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            OriginUserId = model.UserId,
                            StatusId = Constant.StatusPasses.Activo,
                            Fecha = DateTime.Now
                        };

                        var SearchPasses3 = BuilderPasses.SearchPasses(PassesRequest3);

                        if (SearchPasses3.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja Principal. Posee pases activos";
                        }

                        var PassesRequest4 = new PassesRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            DestinationUserId = model.UserId,
                            StatusId = Constant.StatusPasses.Activo,
                            Fecha = DateTime.Now
                        };

                        var SearchPasses4 = BuilderPasses.SearchPasses(PassesRequest4);

                        if (SearchPasses4.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja Principal. Posee pases activos";
                        }

                        #endregion

                        #region Se Valida si algun cajero tiene Saldo disponible

                        var MainCashierSummaryRequest = new CashierSummaryRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            StatusId = Constant.Status.Activo,
                            ProfileId = Constant.Profiles.Cajero
                        };

                        var MainCashierData = BuilderContabilidad.SearchCashierSummary(MainCashierSummaryRequest);
                        var CountMainCashier = MainCashierData.Where(x => x.Quantity > 0).Count();

                        if (CountMainCashier > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja porque los cajeros poseen Saldo Disponible.";
                            break;
                        }

                        #endregion

                        #region Validacion Saldo disponible en caja

                        var MainCashierSummaryRequest2 = new CashierSummaryRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            StatusId = Constant.Status.Activo,
                            UserId = model.UserId
                        };

                        var MainCashierdata2 = BuilderContabilidad.SearchCashierSummary(MainCashierSummaryRequest2);
                        var CountMainCashiert2 = MainCashierdata2.Where(x => x.Quantity > 0).Count();

                        if (CountMainCashiert2 > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de Caja posee Saldo Disponible.";
                            break;
                        }

                        #endregion

                        #region Validar si tiene cierre de caja operativa

                        var CloseMarinCashier = BuilderContabilidad.SearchCloseCashier(model);

                        if (CloseMarinCashier.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de la caja. Ya posee un cierre de caja de la fecha actual";
                        }

                        #endregion

                        break;
                    case 91:
                        #region Validacion Pases activos en sistema

                        var PassesRequest5 = new PassesRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            OriginUserId = model.UserId,
                            StatusId = Constant.StatusPasses.Activo,
                            Fecha = DateTime.Now
                        };

                        var SearchPasses5 = BuilderPasses.SearchPasses(PassesRequest5);

                        if (SearchPasses5.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de boveda. Posee pases activos";
                        }

                        var PassesRequest6 = new PassesRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            DestinationUserId = model.UserId,
                            StatusId = Constant.StatusPasses.Activo,
                            Fecha = DateTime.Now
                        };

                        var SearchPasses6 = BuilderPasses.SearchPasses(PassesRequest6);

                        if (SearchPasses6.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de boveda. Posee pases activos";
                        }

                        #endregion

                        #region Se Valida si algun Cajeros o Cajero Principal tiene Saldo disponible

                        var VaultSummaryRequest = new CashierSummaryRequest()
                        {
                            BranchOffice = model.BranchOffice,
                            StatusId = Constant.Status.Activo,
                            ProfileId = Constant.Profiles.Cajero
                        };

                        var VaultData = BuilderContabilidad.SearchCashierSummary(VaultSummaryRequest);
                        var CountVault = VaultData.Where(x => x.Quantity > 0).Count();

                        if (CountVault > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de boveda (Cajeros o Cajero Principal poseen Saldo Disponible).";
                            break;
                        }

                        #endregion

                        #region Validar si tiene cierre de caja operativa

                        var CloseVault = BuilderContabilidad.SearchCloseCashier(model);

                        if (CloseVault.Count() > 0)
                        {
                            result.Valid = false;
                            result.Message = "No se puede realizar el cierre de boveda. Ya posee un cierre de caja de la fecha actual";
                        }

                        #endregion

                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Error = true,
                    ErrorMessage = string.Concat("Se ha presentado el siguiente error al intentar hacer cierre de operación: ", ex.InnerException == null ? ex.Message : ex.InnerException.Message)
                };
            }

        }


        #region Alliance
        public static GenericResponse InsertSaleTransferAlliance(OrderAllianceRequest request)
        {
            OrdenVentaTransferencia orden = new OrdenVentaTransferencia();
            try
            {
                var codigoTipoOperacion = "venta-simadi-trf";
                var idMoneda = 213; //el codigo de la moneda interna
                var client = (Common.Models.Clientes.Fichas)SearchClient(new Common.Models.Clientes.SearchClientsRequest { fichaConsecutivo = request.ClientId, limit = 10, offSet = 0 }).Data;
                var companyAlliance = (CompanyAlliance)SearchCompanyAlliance(new CompanyAllianceRequest { AllianceId = request.AllianceId, StatusId = 1 }).Data;
                var companyAllianceBank = (CompanyAllianceBank)SearchCompanyAllianceBank(new CompanyAllianceBankRequest { AllianceId = request.AllianceId, StatusId = 1 }).Data;
                var corresponsal = getCorresponsalPaisDeposito(companyAllianceBank.CountryId, true, codigoTipoOperacion);
                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                orden.MonedaConversion = 221;
                var operationCoins = infMonedas.Where(x => x.MonedaId == idMoneda).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                var user = getUsuario("AUTOMATICO");
                var idDetalleTipoOperacion = 9; //para las ventas


                orden.idClienteUnique = client.PersitentObjectId;
                orden.tipoIdCliente = client.ClienteTipo;
                orden.numeroIdCliente = client.id_cedula;
                orden.emailCliente = client.Email;
                orden.telefonoCliente = client.TelfMovil1;
                orden.idMotivoOferta = 1;
                orden.idPaisDestino = companyAllianceBank.CountryId;
                orden.idCorresponsal = corresponsal.codigo;
                orden.tasaCambio = conversionCoins.MonedaValorVenta ?? 0;
                orden.fechaValorTasa = conversionCoins.MonedaDateCreate;
                orden.montoOrden = Math.Round(request.Amount / orden.tasaCambio, 2);

                orden.nombresBeneficiario = companyAlliance.AllianceName;
                orden.tipoIdBeneficiario = companyAlliance.TypeIdentificationId;
                orden.numeroIdBeneficiario = companyAlliance.AllianceIdentificationNumber.ToString();
                orden.numeroCuentaCliente = string.Empty;
                orden.observaciones = string.Empty;
                orden.numeroCuentaDestino = companyAllianceBank.AllianceBankNumber;
                orden.nombreBancoDestino = companyAllianceBank.BankName;
                orden.direccionBancoDestino = companyAllianceBank.AllianceBankAddress;
                orden.aba = companyAllianceBank.AllianceBankAba;
                orden.swift = companyAllianceBank.AllianceBankSwift;
                orden.iban = companyAllianceBank.AllianceBankIban;
                orden.MonedaConversion = conversionCoins.MonedaId;

                ValidateRequestAmountClient(new ValidateRequestAmount
                {
                    ClientId = client.PersitentObjectId,
                    IdentificationType = client.ClienteTipo,
                    Ammount = orden.montoOrden,
                    CurrencyId = idMoneda,
                    OperationTypeId = 9
                });

                ValidateRangeRequestAmount(9, orden.montoOrden);

                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion).FirstOrDefault();
                orden.tipoOferta = tm.id;
                var tipoMovimiento = tm.idBcv;


                orden.tarifas = (List<Tarifa>)SearchRatesAlliance(new TarifaReq
                {
                    idPais = orden.idPaisDestino,
                    montoEnviar = orden.montoOrden,
                    tasa = conversionCoins.MonedaValorVenta ?? 0,
                    tipoOperacion = tm.id.ToString(),
                    tipoId = orden.tipoIdCliente,
                    OperationType = OperationType.Transferencia
                }).Data;


                var oficInfo = getInfoDepositoByCorresponsal(orden.idPaisDestino, true, codigoTipoOperacion, orden.idCorresponsal);

                if (oficInfo == null)
                {
                    oficInfo = new InfoOficinaDeposito
                    {
                        idPagador = "MUL",
                        idCiudad = 368,
                        idOficina = 448,
                        tasa = 1
                    };
                }

                var idPagador = oficInfo.idPagador; //"MUL"; //pagador por defecto para este tipo de operaciones
                var idCiudad = oficInfo.idCiudad; //368;
                var idOficina = oficInfo.idOficina; //448;

                var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                orden.AmmountTotalConversion = orden.tarifas.FirstOrDefault(x => x.InternalId == "AmmountConversion").valor;
                if (operationCoins == null)
                {
                    return new GenericResponse { ErrorMessage = "No se logro consultar las tasas de la operación.", Error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new GenericResponse { ErrorMessage = "No se logro consultar las tasas de conversión.", Error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorVenta ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorCompra.ToString();
                }



                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        return new GenericResponse { ErrorMessage = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", Error = true };
                    }
                    else
                    {
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? n : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", Error = true };

                        }
                        if (_operationNumber == 0)
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", Error = true };
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la tarifa.", Error = true, };
                        }
                        foreach (var item in orden.tarifas.Where(x => x.IsRate))
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        orden.ClientSurnames = string.Concat(client.PrimerApellido, " ", client.SegundoApellido ?? "");
                        orden.ClientNames = string.Concat(client.PrimerNombre, " ", client.SegundoNombre ?? "");

                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;
                        #region Order Insert
                        IAnguloLopezBuilder builder = new AnguloLopezBuilder();
                        Solicitudes objSolicitud = new Solicitudes
                        {
                            DETALLE_TIPO_OPERACION = idDetalleTipoOperacion,
                            CLIENTE = orden.idClienteUnique,
                            SUCURSAL = 16,
                            STATUS_SOLICITUD = orden.MixedOperation ? 6 : 1,
                            MONEDA = idMoneda,
                            OFICINA = idOficina,
                            PAIS_DESTINO = orden.idPaisDestino,
                            CORRESPONSAL = orden.idCorresponsal,
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = orden.montoOrden,
                            MONTO_CAMBIO = (changeAmount) * oficInfo.tasa,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            FECHA_OPERACION = DateTime.Now,
                            TIPO_OP_BCV = tipoMovimiento,
                            TASA_DESTINO = oficInfo.tasa,
                            NOMBRES_REMITENTE = orden.ClientNames,
                            APELLIDOS_REMITENTE = orden.ClientSurnames,
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = orden.nombresBeneficiario,
                            APELLIDOS_BENEFICIARIO = string.Empty,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.nombreBancoDestino,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaDestino,
                            DIRECCION_BANCO = orden.direccionBancoDestino,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,

                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = idPagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            BANCO_INTERMEDIARIO = orden.bancoIntermediario,
                            NUMERO_CUENTA_INTERMEDIARIO = orden.numeroCuentaIntermediario,
                            DIRECCION_BANCO_INTERMEDIARIO = orden.direccionBancoIntermediario,
                            ABA_INTERMEDIARIO = orden.abaIntermediario,
                            SWIFT_INTERMEDIARIO = orden.swiftIntermediario,
                            IBAN_INTERMEDIARIO = orden.ibanIntermediario,
                            USUARIO_TAQUILLA = user.login,
                            TasaConversion = decimal.Parse(string.IsNullOrEmpty(orden.TasaConversion) ? orden.tasaCambio.ToString() : orden.TasaConversion),
                            MonedaOperacion = orden.MonedaConversion,
                            MontoConversion = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            REFERENCIA_ORDEN = referenciaBCV,
                            CommissionUsd = comisionUs,
                            CommissionBs = comisionBs,
                            MontoAprobado = orden.MixedOperation ? orden.montoOrden : 0,
                            ExchangeDifferential = orden.MixedOperation ? orden.tarifas.FirstOrDefault(x => x.InternalId == "ReferenceInformation" && !x.IsRate).valor : 0

                        };

                        var resultInsert = builder.InsertSolicitudes(objSolicitud);
                        if (!resultInsert.Error)
                        {
                            var _idOrden = resultInsert.ReturnId;
                            if (orden.tarifas == null)
                            {
                                return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la tarifa.", Error = true };
                            }
                            foreach (var item in orden.tarifas.Where(x => x.moneda == "USD" && x.IsRate).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda == "VEB" && x.IsRate).ToList())
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                            }
                        }
                        else
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", Error = true };
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = resultInsert.ReturnId;
                    }
                }
                else
                {
                    return new GenericResponse { ErrorMessage = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", Error = true };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponse { ErrorMessage = ex.Message, Error = true };
            }

            return new GenericResponse { Data = orden };
        }

        public static GenericResponse InsertSaleCorrespondentAlliance(OrderAllianceRequest request)
        {
            OrdenVentaEfectivo orden = new OrdenVentaEfectivo();
            try
            {
                var codigoTipoOperacion = "venta-simadi-enc";
                var idMoneda = 213; //el codigo de la moneda interna
                var client = (Common.Models.Clientes.Fichas)SearchClient(new Common.Models.Clientes.SearchClientsRequest { fichaConsecutivo = request.ClientId, limit = 10, offSet = 0 }).Data;
                var companyAlliance = (CompanyAlliance)SearchCompanyAlliance(new CompanyAllianceRequest { AllianceId = request.AllianceId, StatusId = 1 }).Data;
                var companyAllianceBank = (CompanyAllianceBank)SearchCompanyAllianceBank(new CompanyAllianceBankRequest { AllianceId = request.AllianceId, StatusId = 1 }).Data;
                var corresponsal = getCorresponsalPaisDeposito(companyAllianceBank.CountryId, true, codigoTipoOperacion);
                var infMonedas = SearchMonedas(new MonedasRequest { MonedaActiva = true, TipoCambioId = 8 }).ToList();
                orden.MonedaConversion = 221;
                var operationCoins = infMonedas.Where(x => x.MonedaId == idMoneda).FirstOrDefault();
                var conversionCoins = infMonedas.Where(x => x.MonedaId == orden.MonedaConversion).FirstOrDefault();
                var user = getUsuario("AUTOMATICO");
                var idTipoOperacion = 9;
                orden.TakeCommission = true;
                orden.idClienteUnique = client.PersitentObjectId;
                orden.tipoIdCliente = client.ClienteTipo;
                orden.numeroIdCliente = client.id_cedula;
                orden.emailCliente = client.Email;
                orden.telefonoCliente = client.TelfMovil1;
                orden.idMotivoOferta = 1;
                orden.idPaisDestino = companyAllianceBank.CountryId;
                orden.tasaCambio = conversionCoins.MonedaValorVenta ?? 0;
                orden.fechaValorTasa = conversionCoins.MonedaDateCreate;
                orden.montoOrden = Math.Round(request.Amount / orden.tasaCambio, 2);
                orden.nombresBeneficiario = companyAlliance.AllianceName;
                orden.tipoIdBeneficiario = companyAlliance.TypeIdentificationId;
                orden.numeroIdBeneficiario = companyAlliance.AllianceIdentificationNumber.ToString();
                orden.numeroCuentaCliente = string.Empty;
                orden.observaciones = string.Empty;
                orden.aba = companyAllianceBank.AllianceBankAba;
                orden.swift = companyAllianceBank.AllianceBankSwift;
                orden.iban = companyAllianceBank.AllianceBankIban;
                orden.MonedaConversion = conversionCoins.MonedaId;
                orden.idOficina = companyAllianceBank.CorrespondentOfficeId;

                ValidateRequestAmountClient(new ValidateRequestAmount
                {
                    ClientId = client.PersitentObjectId,
                    IdentificationType = client.ClienteTipo,
                    Ammount = orden.montoOrden,
                    CurrencyId = idMoneda,
                    OperationTypeId = 9
                });

                ValidateRangeRequestAmount(9, orden.montoOrden);

                var tm = getTiposMovimientos(orden.tipoIdCliente, codigoTipoOperacion).FirstOrDefault();
                orden.tipoOferta = tm.id;
                var tipoMovimiento = tm.idBcv;

                XDocument xd = new XDocument();
                xd = XDocument.Parse(Services.catalogos.GetDetalleOficina(orden.idOficina, string.Empty).OuterXml);
                var ofic = (from r in xd.Descendants("RESULTADO")
                            select new Oficina
                            {
                                id = Convert.ToInt32(r.Element("id").Value),
                                nombre = r.Element("nombre").Value,
                                pagador = r.Element("pagador").Value,
                                deposito = r.Element("deposito").Value == "0" ? false : true,
                                corresponsal = r.Element("corresponsal").Value,
                                tasa = Convert.ToDecimal(r.Element("tasa").Value, wsCulture),
                                corresponsalId = Convert.ToInt32(r.Element("corresponsalId").Value)
                            }).OrderBy(x => x.nombre).FirstOrDefault();


                orden.tarifas = (List<Tarifa>)SearchRatesAlliance(new TarifaReq
                {
                    idPais = orden.idPaisDestino,
                    montoEnviar = orden.montoOrden,
                    tasa = conversionCoins.MonedaValorVenta ?? 0,
                    tipoOperacion = tm.id.ToString(),
                    tipoId = orden.tipoIdCliente,
                    OperationType = OperationType.Corresponsal,
                    idCorresp = orden.idOficina.ToString()

                }).Data;




                //var n = string.Empty;
                var numeroId = getNumeroIdFormato(orden.tipoIdCliente, orden.numeroIdCliente);
                orden.AmmountTotalConversion = orden.tarifas.FirstOrDefault(x => x.InternalId == "AmmountConversion").valor;
                if (operationCoins == null)
                {
                    return new GenericResponse { ErrorMessage = "No se logro consultar las tasas de la operación.", Error = true };
                }
                if (orden.MonedaConversion != 221 && operationCoins == null)
                {
                    return new GenericResponse { ErrorMessage = "No se logro consultar las tasas de conversión.", Error = true };
                }
                orden.tasaCambio = operationCoins.MonedaValorVenta ?? 0;
                orden.TasaConversion = orden.tasaCambio.ToString();
                if (orden.MonedaConversion != 221)
                {
                    orden.TasaConversion = conversionCoins == null ? "0" : conversionCoins.MonedaValorCompra.ToString();
                }



                var responseBCV = new XmlDocument();
                responseBCV.LoadXml("<root><result>SOL" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + "</result> </root>");

                if (responseBCV != null)
                {
                    if (responseBCV.FirstChild == responseBCV.SelectSingleNode("error"))
                    {
                        return new GenericResponse { ErrorMessage = "El BCV no ha autorizado al operación por la siguiente razón: \n [" + responseBCV.SelectSingleNode("error").InnerText + "].", Error = true };
                    }
                    else
                    {
                        #region Asignación Numero de Orden
                        var referenciaBCV = responseBCV.SelectSingleNode("//result") == null ? string.Empty : responseBCV.SelectSingleNode("//result").InnerText;
                        var num = Services.utilitarios.GetProximoNumero(26, user.idSucursal, user.login, false, DateTime.Now.ToString(), "");
                        long _operationNumber = 0;
                        if (num.FirstChild != num.SelectSingleNode("ERROR"))
                        {
                            if (Convert.ToBoolean(num.SelectSingleNode("ENCONTRADO").InnerText.Trim()))
                                _operationNumber = Convert.ToInt64(num.SelectSingleNode("//NUMERO").InnerText.Trim());
                            else
                                _operationNumber = 0;
                        }
                        else
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [" + num.SelectSingleNode("ERROR").InnerText + "].", Error = true };

                        }
                        if (_operationNumber == 0)
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la numeración de la orden: \n [Numeración en cero (0)].", Error = true };
                        }
                        #endregion
                        #region Agregamos la información a la tabla temporal
                        var montoBolivares = Math.Round(orden.montoOrden * orden.tasaCambio, 2);
                        decimal comisionUs = 0, comisionBs = 0;

                        if (orden.tarifas == null)
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la tarifa.", Error = true, };
                        }
                        foreach (var item in orden.tarifas.Where(x => x.IsRate))
                        {
                            if (item.moneda.Equals("USD"))
                            {
                                if (item.valor < 1)
                                    comisionUs += Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    comisionUs += Math.Round(item.valor, 2);
                            }
                            else
                            {
                                if (item.valor < 1)
                                    comisionBs += Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    comisionBs += Math.Round(item.valor, 2);
                            }
                        }
                        orden.ClientSurnames = string.Concat(client.PrimerApellido, " ", client.SegundoApellido ?? "");
                        orden.ClientNames = string.Concat(client.PrimerNombre, " ", client.SegundoNombre ?? "");

                        decimal changeAmount = orden.montoOrden;
                        if (!orden.TakeCommission)
                            changeAmount -= comisionUs;
                        #region Order Insert
                        IAnguloLopezBuilder builder = new AnguloLopezBuilder();
                        Solicitudes objSolicitud = new Solicitudes
                        {
                            DETALLE_TIPO_OPERACION = idTipoOperacion,
                            CLIENTE = orden.idClienteUnique,
                            SUCURSAL = 16,
                            STATUS_SOLICITUD = orden.MixedOperation ? 6 : 1,
                            MONEDA = idMoneda,
                            OFICINA = ofic.id,
                            PAIS_DESTINO = orden.idPaisDestino,
                            CORRESPONSAL = ofic.corresponsal,
                            TASA_DESTINO = ofic.tasa,
                            NUMERO = int.Parse(_operationNumber.ToString()),
                            TIPO_CAMBIO = orden.tasaCambio,
                            MONTO = orden.montoOrden,
                            MONTO_CAMBIO = (changeAmount) * ofic.tasa,
                            FECHA_VALOR_TASA = orden.fechaValorTasa,
                            TIPO_OP_BCV = tipoMovimiento,
                            NUMERO_CUENTA_DESTINO = orden.numeroCuentaCliente,
                            NOMBRES_REMITENTE = string.Concat(client.PrimerNombre, " ", client.SegundoNombre ?? string.Empty),
                            APELLIDOS_REMITENTE = string.Concat(client.PrimerApellido, " ", client.SegundoApellido ?? string.Empty),
                            IDENTIFICACION_REMITENTE = orden.tipoIdCliente + orden.numeroIdCliente,
                            NOMBRES_BENEFICIARIO = orden.nombresBeneficiario,
                            APELLIDOS_BENEFICIARIO = string.Empty,
                            IDENTIFICACION_BENEFICIARIO = orden.tipoIdBeneficiario + orden.numeroIdBeneficiario,
                            NUMERO_CUENTA = orden.numeroCuentaCliente,
                            EMAIL_CLIENTE = orden.emailCliente,
                            EMAIL_BENEFICIARIO = orden.emailBeneficiario,
                            OBSERVACIONES = orden.observaciones,
                            BANCO_DESTINO = orden.bancoCliente,
                            ABA = orden.aba,
                            SWIFT = orden.swift,
                            IBAN = orden.iban,
                            TELEFONO_BENEFICIARIO = orden.telefonoBeneficiario,
                            TELEFONO_CLIENTE = orden.telefonoCliente,
                            REGISTRADOPOR = user.login,
                            AGENTE = ofic.pagador,
                            MOTIVO_OP_BCV = orden.idMotivoOferta,
                            USUARIO_TAQUILLA = user.login,


                            TasaConversion = decimal.Parse(string.IsNullOrEmpty(orden.TasaConversion) ? orden.tasaCambio.ToString() : orden.TasaConversion),
                            MonedaOperacion = orden.MonedaConversion,
                            MontoConversion = orden.MonedaConversion == 221 ? montoBolivares : orden.AmmountTotalConversion,
                            REFERENCIA_ORDEN = referenciaBCV,
                            CommissionUsd = comisionUs,
                            CommissionBs = comisionBs,
                            MontoAprobado = orden.MixedOperation ? orden.montoOrden : 0,
                            ExchangeDifferential = orden.MixedOperation ? orden.tarifas.FirstOrDefault(x => x.InternalId == "ReferenceInformation" && !x.IsRate).valor : 0
                        };

                        var resultInsert = builder.InsertSolicitudes(objSolicitud);
                        if (!resultInsert.Error)
                        {
                            var _idOrden = resultInsert.ReturnId;
                            if (orden.tarifas == null)
                            {
                                return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de obtener la tarifa.", Error = true };
                            }
                            foreach (var item in orden.tarifas.Where(x => x.moneda == "USD" && x.IsRate).ToList())
                            {
                                decimal _comisionUs = 0;
                                if (item.valor < 1)
                                    _comisionUs = Math.Round(item.valor * orden.montoOrden, 2);
                                else
                                    _comisionUs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionUs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionUs, user.login);
                            }

                            foreach (var item in orden.tarifas.Where(x => x.moneda == "VEB" && x.IsRate).ToList())
                            {
                                decimal _comisionBs = 0;

                                if (item.valor < 1)
                                    _comisionBs = Math.Round(item.valor * Math.Round((orden.montoOrden * orden.tasaCambio), 2), 2);
                                else
                                    _comisionBs = Math.Round(item.valor, 2);
                                builder.InsertTarifasAplicadas(new Tarifas_Aplicadas
                                {
                                    SOLICITUD = resultInsert.ReturnId,
                                    TARIFA = item.idTarifa,
                                    MONTO = _comisionBs,
                                    REGISTRADOPOR = user.login
                                });
                                //Services.ordenes.setTarifasAplicadasOrden(_idOrden, item.idTarifa, _comisionBs, user.login);
                            }
                        }
                        else
                        {
                            return new GenericResponse { ErrorMessage = "Ha ocurrido un error al tratar de guardar de la orden: \n [" + resultInsert.ErrorMessage + "].", Error = true };
                        }
                        #endregion
                        #endregion
                        orden.idResult = referenciaBCV;
                        orden.temporal = 1;
                        orden.id = resultInsert.ReturnId;
                    }
                }
                else
                {
                    return new GenericResponse { ErrorMessage = "No hemos tenido respuesta del BCV para autorizar la operación, Por favor intente de nuevo, si el problema persiste, comuniquese con el Dpto. de Sistemas.", Error = true };
                }
            }
            catch (Exception ex)
            {
                return new GenericResponse { ErrorMessage = ex.Message, Error = true };
            }

            return new GenericResponse { Data = orden };
        }

        public static GenericResponse SearchClient(Common.Models.Clientes.SearchClientsRequest request)
        {
            var responseClient = new GenericResponse();
            ClientsBuilder clientBuilder = new ClientsBuilder();
            responseClient.Data = clientBuilder.Searchfichas(request).FirstOrDefault();
            if (responseClient.Data == null)
            {
                throw new Exception("No se ha podido encontrar al cliente en base a la información suministrada. Por favor validar los datos del cliente.");
            }
            return responseClient;
        }

        private static void ValidateRequestAmountClient(ValidateRequestAmount request)
        {
            AnguloLopezBuilder anguloBuilder = new AnguloLopezBuilder();
            var result = anguloBuilder.ValidateRequestAmount(request);
            if (!result.Valid)
            {
                throw new Exception(result.Message);
            }
        }

        private static void ValidateRangeRequestAmount(int operationType, decimal amountReqeust)
        {
            string titleMin = string.Empty;
            string titleMax = string.Empty;
            decimal minValue = 1M;
            decimal maxValue = 300M;
            switch (operationType)
            {
                case 8:
                    titleMin = "MIN_VENTA_EFECT_SIMADI";
                    titleMax = "MAX_VENTA_EFECT_SIMADI";
                    break;
            }
            var parameter = getParametrosOrden();
            if (parameter.FirstOrDefault(x => x.que.Trim().ToUpper() == titleMin) != null)
                minValue = (decimal)parameter.FirstOrDefault(x => x.que.Trim().ToUpper() == titleMin).cuanto;

            if (parameter.FirstOrDefault(x => x.que.Trim().ToUpper() == titleMax) != null)
                maxValue = (decimal)parameter.FirstOrDefault(x => x.que.Trim().ToUpper() == titleMax).cuanto;


            if (amountReqeust < minValue)
                throw new Exception("El monto ingresado es inferior al monto mínimo de la orden, permitido por CCAL. (Monto mínimo = $" + minValue.ToString() + ")");

            //No Implementado aun
            //if (amountReqeust > maxValue)
            //    throw new Exception("El monto ingresado es superior al monto máximo de la orden, permitido por CCAL. (Monto máximo = $" + maxValue.ToString() + ")");
        }

        public static GenericResponse SearchCompanyAlliance(Common.Models.Angulo_Lopez.Operaciones.CompanyAllianceRequest request)
        {
            var responseAlliance = new GenericResponse();
            OperacionesBuilder oparecionesBuilder = new OperacionesBuilder();
            responseAlliance.Data = oparecionesBuilder.SearchCompanyAlliance(request).FirstOrDefault();
            if (responseAlliance.Data == null)
            {
                throw new Exception("No se ha podido encontrar el aliado en base a la información suministrada. Por favor validar los datos del aliado.");
            }
            return responseAlliance;
        }

        public static GenericResponse SearchCompanyAllianceBank(Common.Models.Angulo_Lopez.Operaciones.CompanyAllianceBankRequest request)
        {
            var responseAllianceBank = new GenericResponse();
            OperacionesBuilder oparecionesBuilder = new OperacionesBuilder();
            responseAllianceBank.Data = oparecionesBuilder.SearchCompanyAllianceBank(request).FirstOrDefault();
            if (responseAllianceBank.Data == null)
            {
                throw new Exception("No se ha podido encontrar el banco asociado al aliado en base a la información suministrada. Por favor validar los datos del aliado.");
            }
            return responseAllianceBank;
        }

        public static GenericResponse SearchRatesAlliance(TarifaReq request)
        {
            var resulRate = new GenericResponse();
            request.MonedaConversion = 221;
            request.MonedaOperacion = 213;
            request.TakeCommission = false;
            request.moneda = "USD";

            var tarifas = SearchFinancialSummary(request).RateOperation;

            resulRate.Data = tarifas.Select(x => new Tarifa
            {
                id = x.id,
                idTarifa = x.idTarifa,
                concepto = x.concepto,
                moneda = x.moneda,
                valor = x.valor,
                valor2 = x.valor2,
                simbolo = x.simbolo,
                Sale = x.Sale,
                IsRate = x.IsRate,
                Title = x.Title,
                IgnoreInView = x.IgnoreInView,
                InternalId = x.InternalId
            }).ToList();

            return resulRate;
        }
        #endregion

        #region Extensions
        public static string TryGetElementValue(this XElement parentEl, string elementName, string defaultValue = null)
        {
            var foundEl = parentEl.Element(elementName);

            if (foundEl != null)
            {
                return foundEl.Value;
            }

            return defaultValue;
        }
        #endregion
    }
    public class InvokerRemitee
    {
        #region Variables
        private const string _errorBaseJSON = @"{{'error':true,'clientErrorDetail':'{0}','apiDetail':'{1}' }}";
        private const string _errorBaseListJSON = @"[{{'error':true,'clientErrorDetail':'{0}','apiDetail':'{1}' }}]";
        private string _path = string.Empty;
        private string _tokenUrl = string.Empty;
        #endregion

        public InvokerRemitee()
        {
            try
            {
                _path = ConfigurationManager.AppSettings["serviceUrlRemitee"].ToString(); //variable desencriptada en el web.config
                _tokenUrl = ConfigurationManager.AppSettings["tokenUrlRemitee"].ToString(); //variable desencriptada en el web.config
            }
            catch (Exception ex)
            {
                throw ex; //Regresa valor a una pagina al producirse un error
            }
        }
        public object token(Type dataType)
        {
            var url = string.Empty;
            try
            {
                var client = new HttpClient();

                url = _tokenUrl;
                client.Timeout = client.Timeout = new TimeSpan(0, 5, 0);
                var grant_type = "client_credentials";
                var clientId = ConfigurationManager.AppSettings["clientIdRemitee"].ToString();
                var clientSecret = ConfigurationManager.AppSettings["clientSecretRemitee"].ToString();

                client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
                client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/x-www-form-urlencoded");


                var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                };

                var json = client.PostAsync(url, new FormUrlEncodedContent(form)).Result;

                if (json.IsSuccessStatusCode)
                {
                    var answer = JsonConvert.DeserializeObject(json.Content.ReadAsStringAsync().Result, dataType);
                    return answer;
                }
                else
                    return this.jsonResponseErrorHandler(json, typeof(string), url);
            }
            catch (Exception ex)
            {
                return this.exceptionErrorHandler(ex, typeof(string), url);
            }
        }
        public object post(string apiDefination, object data, Type dataType, string token)
        {
            var url = string.Empty;
            try
            {
                var client = new HttpClient();

                url = string.Format("{0}{1}", _path, apiDefination);

                client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
                client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", token);

                client.Timeout = client.Timeout = new TimeSpan(0, 5, 0);
                var dataPost = string.Empty;
                if (data != null)
                {
                    var dataConverted = Convert.ChangeType(data, data.GetType());
                    dataPost = JsonConvert.SerializeObject(dataConverted);
                }

                var json = client.PostAsync(url, new StringContent(dataPost, Encoding.UTF8, "application/json")).Result;

                if (json.IsSuccessStatusCode)
                {
                    var answer = JsonConvert.DeserializeObject(json.Content.ReadAsStringAsync().Result, dataType);
                    return answer;
                }
                else
                    return this.jsonResponseErrorHandler(json, dataType, url);
            }
            catch (Exception ex)
            {
                return this.exceptionErrorHandler(ex, dataType, url);
            }
        }
        public HttpResponseMessage postHttpResponse(string apiDefination, object data, string token)
        {
            var url = string.Empty;
            try
            {
                var client = new HttpClient();

                url = string.Format("{0}{1}", _path, apiDefination);

                client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
                client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", token);

                client.Timeout = client.Timeout = new TimeSpan(0, 5, 0);
                var dataPost = string.Empty;
                if (data != null)
                {
                    var dataConverted = Convert.ChangeType(data, data.GetType());
                    dataPost = JsonConvert.SerializeObject(dataConverted);
                }

                var json = client.PostAsync(url, new StringContent(dataPost, Encoding.UTF8, "application/json")).Result;

                return json;
            }
            catch (Exception ex)
            {
                var ret = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                var content = CreateHttpContent(new Base() { error = true, errorDetail = ex, apiDetail = apiDefination, clientErrorDetail = ex.Message });
                ret.Content = content;
                return ret;
            }
        }
        public HttpResponseMessage getHttpResponse(string apiDefination, string token)
        {
            var url = string.Empty;
            try
            {
                var client = new HttpClient();

                url = string.Format("{0}{1}", _path, apiDefination);

                client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
                client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", token);

                client.Timeout = client.Timeout = new TimeSpan(0, 5, 0);

                var json = client.GetAsync(url).Result;

                return json;
            }
            catch (Exception ex)
            {
                var ret = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                var content = CreateHttpContent(new Base() { error = true, errorDetail = ex, apiDetail = apiDefination, clientErrorDetail = ex.Message });
                ret.Content = content;
                return ret;
            }
        }
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Newtonsoft.Json.Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public object get(string apiDefination, Type dataType, string token)
        {
            var url = string.Empty;
            try
            {
                var client = new HttpClient();

                url = string.Format("{0}{1}", _path, apiDefination);

                client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
                client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", token);

                client.Timeout = client.Timeout = new TimeSpan(0, 5, 0);


                var json = client.GetAsync(url).Result;

                if (json.IsSuccessStatusCode)
                {
                    var answer = JsonConvert.DeserializeObject(json.Content.ReadAsStringAsync().Result, dataType);
                    return answer;
                }
                else
                    return this.jsonResponseErrorHandler(json, dataType, url);
            }
            catch (Exception ex)
            {
                return this.exceptionErrorHandler(ex, dataType, url);
            }
        }

        object jsonResponseErrorHandler(HttpResponseMessage json, Type type, string apiDetail)
        {
            //var message = (internalError)JsonConvert.DeserializeObject(json.Content.ReadAsStringAsync().Result, typeof(internalError));
            //var exception = new Exception(string.Format("Request Error - {0}", json.StatusCode), new Exception());
            var finalJson = string.Empty;
            if (type.ToString().Contains("List"))
                finalJson = string.Format(_errorBaseListJSON, "Ha ocurrido un error inesperado. Detalle = " + json.StatusCode.ToString(), apiDetail);
            else
                finalJson = string.Format(_errorBaseJSON, "Ha ocurrido un error inesperado. Detalle = " + json.StatusCode.ToString(), apiDetail);

            var answer = JsonConvert.DeserializeObject(finalJson, type);
            return answer;
        }

        object exceptionErrorHandler(Exception ex, Type type, string apiDetail)
        {
            var finalJson = string.Empty;
            if (type.ToString().Contains("List"))
                finalJson = string.Format(_errorBaseListJSON, "Ha ocurrido un error inesperado.", apiDetail);
            else
                finalJson = string.Format(_errorBaseJSON, "Ha ocurrido un error inesperado.", apiDetail);

            var answer = JsonConvert.DeserializeObject(finalJson, type);
            return answer;
        }
    }
}