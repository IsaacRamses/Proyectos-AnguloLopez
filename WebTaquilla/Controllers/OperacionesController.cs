using Common;
using Common.Models.AgilCheck;
using Common.Models.Angulo_Lopez;
using Common.Models.Angulo_Lopez.Ciudades;
using Common.Models.Angulo_Lopez.corresponsal;
using Common.Models.Angulo_Lopez.Oficinas;
using Common.Models.Angulo_Lopez.Operaciones;
using Common.Models.Angulo_Lopez.OrdenesEntrantes;
using Common.Models.Angulo_Lopez.Sudeban;
using Common.Models.Angulo_Lopez.TablasMaestras;
using Common.Models.Angulo_Lopez.Tarifas;
using Common.Models.Angulo_Lopez.Tasas;
using Common.Models.Centralizer;
using Common.Models.Clientes;
using Common.Models.Common;
using Common.Models.Generic;
using Common.Resource;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WebTaquilla.Models.codief;
using WebTaquilla.Utilities;
using static Common.Resource.Constant;
using Monedas = WebTaquilla.Models.codief.Monedas;
using MonedasRequest = WebTaquilla.Models.codief.MonedasRequest;
using Pais = Common.Models.Common.Pais;
using TarifaReq = Common.Models.Angulo_Lopez.Tarifas.TarifaReq;
using TasaCambio = Common.Models.Common.TasaCambio;

namespace WebTaquilla.Controllers
{
    [EnableCors(PolicyName = "InternalTrafficAL")]
    [AutoValidateAntiforgeryToken]
    [AuthorizeMe]
    public class OperacionesController : Controller
    {
        #region Private Variables Member
        private string user = string.Empty;
        private string password = string.Empty;
        private string domain = string.Empty;
        private string app;
        #endregion

        #region Interfaces

        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        public OperacionesController(IConfiguration configuration)
        {
            _configuration = configuration;
            user = _configuration["AppSettings:UserReport"].ToString();
            password = _configuration["AppSettings:PasswordReport"].ToString();
            domain = _configuration["AppSettings:DomainReport"].ToString();
            app = _configuration["AppSettings:AppId"].ToString();
        }

        #endregion

        #region Methods

        #region ValidateCashAvailability

        public ActionResult ValidateCashAvailability(int typeOperations)
        {
            var invoker = new InvokerServices(this).Instance();
            var userID = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

            var Operations = (List<OrdenEntranteBackoffice>)invoker.post(Api.SearchInternationalMoneyOrderPayment, typeOperations, typeof(List<OrdenEntranteBackoffice>));

            return Json(Operations);
        }
        #endregion

        #region InternationalMoneyOrderPayment
        public ActionResult InternationalMoneyOrderPayment()
        {
            return View();
        }
        #endregion

        #region SearchInternationalMoneyOrderPayment
        public ActionResult SearchInternationalMoneyOrderPayment(OrdenesRequest model)
        {
            var invoker = new InvokerServices(this).Instance();
            var Operations = (List<SolicitudesBackoffice>)invoker.post(Api.SearchPaymentOrderVentanilla, model, typeof(List<SolicitudesBackoffice>));

            return Json(Operations);
        }
        #endregion

        #region GetImage
        public JsonResult GetImage(string path)
        {
            var strImage = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                UserCredentials _credentials = new UserCredentials(domain, user, password);
                Impersonation.RunAsUser(_credentials, LogonType.Network, () =>
                {
                    if (System.IO.File.Exists(path))
                {
                    var fileName = Path.GetFileName(path);
                    using (Image image = Image.FromFile(path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            var image64 = string.Concat("data:image/", Path.GetExtension(fileName).Replace(".", string.Empty), ";base64,");
                            strImage = string.Concat(image64, Convert.ToBase64String(imageBytes));
                        }
                    }
                }
                });
            }
            return Json(strImage);
        }
        #endregion

        #region UpdateStatusOrdenes

        [HttpPost]
        public JsonResult UpdateStatusOrdenes(Ordenes model)
        {
            var invoker = new InvokerServices(this).Instance();
            model.ModificadoPor = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            model.SucursalNombre = User.Claims.FirstOrDefault(c => c.Type == "SucursalNombre").Value;
            model.SucursalProcesaId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Oficina").Value);
            model.LogIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            model.ApplicationId = int.Parse(app);
            var result = (GenericResponse)invoker.post(Api.UpdateStatusOrdenes, model, typeof(GenericResponse));
            return Json(result);
        }


        #endregion

        #region PurchaseOfcurrencyInCash
        public ActionResult PurchaseOfcurrencyInCash()
        {
            var invoker = new InvokerServices(this).Instance();

            MonedasRequest req = new MonedasRequest
            {
                MonedaActiva = true,
                TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33,
            MonedaInterna = false,
            };
            var result = invoker.post(Api.SearchMonedas, req, typeof(List<Monedas>)) as List<Monedas>;
            result.Insert(0, new Monedas { MonedaName = "Seleccione...", MonedaId = 0 });
            ViewBag.ListaMoneda = new SelectList(result, "MonedaId", "MonedaName");

            var MotivosOperacion = invoker.post(Api.SearchReasonOrder, new ReasonOrderRequest() {StatusId = null }, typeof(List<ReasonOrder>)) as List<ReasonOrder>;
            MotivosOperacion.Insert(0, new ReasonOrder { ReasonName = "Seleccione...", ReasonCodeBcv = "" });
            ViewBag.ListaMotivosOperacion = new SelectList(MotivosOperacion, "ReasonCodeBcv", "ReasonName");

            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");

            var TasaCambio = (List<Historial>)invoker.post(Api.SearchHistorial, new HistorialRequest() { Date = DateTime.Now }, typeof(List<Historial>));

            if (TasaCambio.Count() > 0)
            {
                ViewBag.tasa = TasaCambio.LastOrDefault().valorCompra;
            }
            else
            {
                ViewBag.tasa = "0,0";
            }

            return View();
        }
        #endregion

        #region SaleOfCurrencyInCash
        public ActionResult SaleOfCurrencyInCash()
        {
            var invoker = new InvokerServices(this).Instance();

            MonedasRequest req = new MonedasRequest
            {
                MonedaActiva = true,
                TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33,
            MonedaInterna = false,
            };

            var result = invoker.post(Api.SearchMonedas, req, typeof(List<Monedas>)) as List<Monedas>;
            result.Insert(0, new Monedas { MonedaName = "Seleccione...", MonedaId = 0 });
            ViewBag.ListaMoneda = new SelectList(result, "MonedaId", "MonedaName" );

            var MotivosOperacion = invoker.post(Api.SearchReasonOrder, new ReasonOrderRequest() { StatusId = null }, typeof(List<ReasonOrder>)) as List<ReasonOrder>;
            MotivosOperacion.Insert(0, new ReasonOrder { ReasonName = "Seleccione...", ReasonCodeBcv = "" });
            ViewBag.ListaMotivosOperacion = new SelectList(MotivosOperacion, "ReasonCodeBcv", "ReasonName");

            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");

            var TasaCambio = (List<Historial>)invoker.post(Api.SearchHistorial, new HistorialRequest() { Date = DateTime.Now }, typeof(List<Historial>));

            if (TasaCambio.Count() > 0)
            {
                ViewBag.tasa = TasaCambio.LastOrDefault().valorVenta;
            }
            else
            {
                ViewBag.tasa = "0,0";
            }

            return View();
        }
        #endregion

        #region SaleOfCurrencyForElectronicParcel
        public ActionResult SaleOfCurrencyForElectronicParcel()
        {
            var invoker = new InvokerServices(this).Instance();

            MonedasRequest req = new MonedasRequest
            {
                MonedaActiva = true,
                TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33,
            MonedaInterna = false,
            };
            var result = invoker.post(Api.SearchMonedas, req, typeof(List<Monedas>)) as List<Monedas>;
            result.Insert(0, new Monedas { MonedaName = "Seleccione...", MonedaId = 0 });
            ViewBag.ListaMoneda = new SelectList(result, "MonedaId", "MonedaName");

            var MotivosOperacion = invoker.post(Api.SearchReasonOrder, new ReasonOrderRequest() { StatusId = null }, typeof(List<ReasonOrder>)) as List<ReasonOrder>;
            MotivosOperacion.Insert(0, new ReasonOrder { ReasonName = "Seleccione...", ReasonCodeBcv = "" });
            ViewBag.ListaMotivosOperacion = new SelectList(MotivosOperacion, "ReasonCodeBcv", "ReasonName");

            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");

            var TasaCambio = (List<Historial>)invoker.post(Api.SearchHistorial, new HistorialRequest() { Date = DateTime.Now }, typeof(List<Historial>));

            if (TasaCambio.Count() > 0)
            {
                ViewBag.tasa = TasaCambio.LastOrDefault().valorVenta;
            }
            else
            {
                ViewBag.tasa = "0,0";
            }

            var r = invoker.post(Api.SearchPaisesEnvios,null, typeof(List<PaisesEnvios>)) as List<PaisesEnvios>;
            r.Insert(0, new PaisesEnvios { PAIS = "Seleccione...", ID_PAIS = "" });
            ViewBag.ListaPaisesEnvios = new SelectList(r, "ID_PAIS", "PAIS");
            return View();
        }
        #endregion

        #region SaleOfCurrencyByTransfer

        public ActionResult SaleOfCurrencyByTransfer() 
        {
            var invoker = new InvokerServices(this).Instance();

            MonedasRequest req = new MonedasRequest
            {
                MonedaActiva = true,
                TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33,
            MonedaInterna = false,
            };
            var result = invoker.post(Api.SearchMonedas, req, typeof(List<Monedas>)) as List<Monedas>;
            result.Insert(0, new Monedas { MonedaName = "Seleccione...", MonedaId = 0 });
            ViewBag.ListaMoneda = new SelectList(result, "MonedaId", "MonedaName");

            var MotivosOperacion = invoker.post(Api.SearchReasonOrder, new ReasonOrderRequest() { StatusId = null }, typeof(List<ReasonOrder>)) as List<ReasonOrder>;
            MotivosOperacion.Insert(0, new ReasonOrder { ReasonName = "Seleccione...", ReasonCodeBcv = "" });
            ViewBag.ListaMotivosOperacion = new SelectList(MotivosOperacion, "ReasonCodeBcv", "ReasonName");

            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");
            ViewBag.ListaTiposDeIdentidadDes = new SelectList(TiposDeIdentidad.Where(x => x.tipoFicha == 1 || x.acronimo == ""), "acronimo", "nombre");

            var TasaCambio = (List<Historial>)invoker.post(Api.SearchHistorial, new HistorialRequest() { Date = DateTime.Now }, typeof(List<Historial>));

            if (TasaCambio.Count() > 0)
            {
                ViewBag.tasa = TasaCambio.LastOrDefault().valorVenta;
            }
            else
            {
                ViewBag.tasa = "0,0";
            }

            var r = invoker.post(Api.SearchPaisesEnvios, null, typeof(List<PaisesEnvios>)) as List<PaisesEnvios>;
            r.Insert(0, new PaisesEnvios { PAIS = "Seleccione...", ID_PAIS = "" });
            ViewBag.ListaPaisesEnvios = new SelectList(r, "idPais", "ID_PAIS");
            return View();
        }

        #endregion

        #region PurchaseOfCurrencyByTransfer

        public ActionResult PurchaseOfCurrencyByTransfer()
        {
            var invoker = new InvokerServices(this).Instance();

            MonedasRequest req = new MonedasRequest
            {
                MonedaActiva = true,
                TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33,
            MonedaInterna = false,
            };
            var result = invoker.post(Api.SearchMonedas, req, typeof(List<Monedas>)) as List<Monedas>;
            result.Insert(0, new Monedas { MonedaName = "Seleccione...", MonedaId = 0 });
            ViewBag.ListaMoneda = new SelectList(result, "MonedaId", "MonedaName");

            var MotivosOperacion = invoker.post(Api.SearchReasonOrder, new ReasonOrderRequest() { StatusId = null }, typeof(List<ReasonOrder>)) as List<ReasonOrder>;
            MotivosOperacion.Insert(0, new ReasonOrder { ReasonName = "Seleccione...", ReasonCodeBcv = "" });
            ViewBag.ListaMotivosOperacion = new SelectList(MotivosOperacion, "ReasonCodeBcv", "ReasonName");

            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");
            ViewBag.ListaTiposDeIdentidadDes = new SelectList(TiposDeIdentidad.Where(x => x.tipoFicha == 1 || x.acronimo == ""), "acronimo", "nombre");

            var TasaCambio = (List<Historial>)invoker.post(Api.SearchHistorial, new HistorialRequest() { Date = DateTime.Now }, typeof(List<Historial>));

            if (TasaCambio.Count() > 0)
            {
                ViewBag.tasa = TasaCambio.LastOrDefault().valorCompra;
            }
            else
            {
                ViewBag.tasa = "0,0";
            }

            var r = invoker.post(Api.SearchPaisesEnvios, null, typeof(List<PaisesEnvios>)) as List<PaisesEnvios>;
            r.Insert(0, new PaisesEnvios { PAIS = "Seleccione...", ID_PAIS = "" });
            ViewBag.ListaPaisesEnvios = new SelectList(r, "idPais", "ID_PAIS");
            return View();
        }

        #endregion

        #region Searchfichas
        public JsonResult Searchfichas(SearchClientsRequest model)
        {
            var invoker = new InvokerServices(this).Instance();
            model.offSet = 0;
            model.limit = 10;
            var fichas = invoker.post(Api.Searchfichas, model, typeof(List<Common.Models.Clientes.Fichas>)) as List<Common.Models.Clientes.Fichas>;

            return Json(fichas);           
        }
        #endregion

        #region SearchMonedas
        [HttpPost]
        public JsonResult SearchMonedas(MonedasRequest req)
        {
            var invoker = new InvokerServices(this).Instance();
            req.MonedaActiva = true;
            req.TipoCambioId = Common.Resource.Constant.MonedaTipoCambio.ConvenioCambiarioN33;
            req.MonedaInterna = false;
            var result = invoker.post(Api.SearchMonedas, req, typeof(List<Monedas>)) as List<Monedas>;
            return Json(result);
        }
        #endregion

        #region SearchBancos

        [HttpPost]
        public JsonResult SearchBancos(BANCOSRequest req)
        {
            var invoker = new InvokerServices(this).Instance();
            req.ACTIVO = true;
            var ListaBancos = (List<BANCOS>)invoker.post(Api.SearchBank, req, typeof(List<BANCOS>));
            ListaBancos.Insert(0, new BANCOS { ID_BANCO = 0, BANCO = "Seleccione..." });
            return Json(ListaBancos);
        }

        #endregion

        #region SearchBankAccountRequiredCity 
        [HttpPost]
        public JsonResult SearchBankAccountRequiredCity(BankAccountRequiredCityRequest req)
        {
            var invoker = new InvokerServices(this).Instance();
            req.StatusId = Constant.Status.Activo;
            var List = (List<BankAccountRequiredCity>)invoker.post(Api.SearchBankAccountRequiredCity, req, typeof(List<BankAccountRequiredCity>));
            return Json(List);
        }

        #endregion

        #region MotivoOperacion
        [HttpPost]
        public JsonResult MotivoOperacion()
        {
            var invoker = new InvokerServices(this).Instance();

            var MotivosOperacion = invoker.post(Api.SearchReasonOrder, new ReasonOrderRequest() { StatusId = null }, typeof(List<ReasonOrder>)) as List<ReasonOrder>;
            return Json(MotivosOperacion);
        }
        #endregion

        #region SearchFinancialSummary
        [HttpPost]
        public JsonResult SearchFinancialSummary(TarifaReq model)
        {
            var invoker = new InvokerServices(this).Instance();

            var RateResult = invoker.post(Api.SearchFinancialSummary, model, typeof(RateResult)) as RateResult;
            return Json(RateResult);
        }
        #endregion

        #region TiposMovimientos
        [HttpPost]
        public JsonResult TiposMovimientos(TipoMovimientosSimadiRequest model)
        {
            var invoker = new InvokerServices(this).Instance();

            var TipoMovimientoRequest = new Common.Models.Angulo_Lopez.Simadi.TipoMovimientoRequest() 
            {
                idTipoIdentidad = model.idTipoIdentidad,
                TipoOperacion = model.keyword
            };
            
            var TiposMovimientos = invoker.post(Api.SearchTiposMovimientosSimadi, TipoMovimientoRequest, typeof(List<Common.Models.Angulo_Lopez.Simadi.TipoMovimientosSimadi>)) as List<Common.Models.Angulo_Lopez.Simadi.TipoMovimientosSimadi>;
            return Json(TiposMovimientos);
        }
        #endregion

        #region TasaCambio
        [HttpPost]
        public JsonResult TasaCambio()
        {
            var invoker = new InvokerServices(this).Instance();

            var TasaCambio = (List<Historial>)invoker.post(Api.SearchHistorial, new HistorialRequest() { Date = DateTime.Now }, typeof(List<Historial>));
            return Json(TasaCambio);
        }
        #endregion

        #region UpdateEstatusFicha
        [HttpPost]
        public JsonResult UpdateEstatusFicha(GenericSearch model)
        {
            var invoker = new InvokerServices(this).Instance();
            var result = (int)invoker.post(Api.UpdateEstatusFicha, model, typeof(int));
            return Json(result);
        }
        #endregion

        #region ParametrosOrden
        [HttpPost]
        public JsonResult ParametrosOrden()
        {
            var invoker = new InvokerServices(this).Instance();
            var result = invoker.get(Api.getParametrosOrden, typeof(List<Models.codief.Parametro>)) as List<Models.codief.Parametro>;
            return Json(result);
        }
        #endregion

        #region SearchCity
        [HttpPost]
        public JsonResult SearchCity(CiudadRequest param)
        {
            var invoker = new InvokerServices(this).Instance();
            var r = invoker.post(Api.SearchCity, param, typeof(List<Common.Models.Angulo_Lopez.Ciudades.Ciudad>)) as List<Common.Models.Angulo_Lopez.Ciudades.Ciudad>;
            return Json(r);
        }
        #endregion

        #region OficinasPorCiudad
        [HttpPost]
        public JsonResult OficinasPorCiudad(OficinasRequest req)
        {
            var i = new InvokerServices(this).Instance();
            var r = i.post(Api.SearchOficinasCiudad, req, typeof(List<Oficina>)) as List<Oficina>;
            return Json(r);
        }
        #endregion

        #region SearchRemesasEntrantesRejectRIA
        public JsonResult SearchRemesasEntrantesRejectRIA(OrdenEntranteRequest model)
        {
            var invoker = new InvokerServices(this).Instance();
            var data = (List<OrdenEntranteBackoffice>)invoker.post(Api.SearchRemesasEntrantesRejectRIA, model, typeof(List<OrdenEntranteBackoffice>));
            return Json(data);
        }

        #endregion

        #region InsertOperacionesPorCobrar
        /// <summary>
        /// Metodo Recibe los Datos necesarios para registar una orden temporal de Operaciones de Negocio 
        /// (Compra/Venta de Divisas en efectivo, Venta de Divisas por Encomienda Electronica o Venta de Divisas por Transferencia,Giro Internacional de Corresponsales diferentes a RIA)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertOperacionesPorCobrar(OperacionDeNegocio model)
        {
            {
                var invoker = new InvokerServices(this).Instance();
                model.current = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                model.IdCiudad = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "CiudadExterna").Value); 
                model.IdOficinaExterna = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "OficinaExterna").Value);
                model.CiuOrig = User.Claims.FirstOrDefault(c => c.Type == "Letra").Value;
                model.IdSucursal = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "Oficina").Value);
                model.BranchOffice = User.Claims.FirstOrDefault(c => c.Type == "Sucursal").Value;

                OperacionDeNegocio Operations = new OperacionDeNegocio();
                if (model.Id_RemesaEntrante != null && model.Id_RemesaEntrante != 0)
                {
                  Operations = (OperacionDeNegocio)invoker.post(Api.InsertOpPorCobrarRemesaEntrante, model, typeof(OperacionDeNegocio));
                }
                else
                {
                    Operations = (OperacionDeNegocio)invoker.post(Api.InsertOperacionesPorCobrar, model, typeof(OperacionDeNegocio));

                    var ClientDocument = String.Empty;
                    var NamesRemitente = String.Empty;
                    var SurnamesRemitente = String.Empty;
                    var Names = String.Empty;
                    var Surnames = String.Empty;

                    var resultClient = (HashSet<Fichas>)invoker.post(Api.Searchfichas, new SearchClientsRequest() { id_cedula = model.CIREM, offSet = 0, limit = 10 }, typeof(HashSet<Fichas>));

                    Names = resultClient.FirstOrDefault().PrimerNombre + " " + resultClient.FirstOrDefault().SegundoNombre;
                    Surnames = resultClient.FirstOrDefault().PrimerApellido + " " + resultClient.FirstOrDefault().SegundoApellido;
                    ClientDocument = model.CIREM;

                    if (model.CodigoTipoOperacion.Trim() != "compra-simadi-taq" &&
                        model.CodigoTipoOperacion.Trim() != "venta-simadi-taq")
                    {
                        var NOMDES2 = Operations.NOMDES2 == null ? "" : Operations.NOMDES2;
                        NamesRemitente = Operations.NOMDES1 + " " + NOMDES2;
                        var APEDES2 = Operations.APEDES2 == null ? "" : Operations.APEDES2;
                        SurnamesRemitente = Operations.APEDES1 + " " + APEDES2;
                    }

                    var request = new CentralizedRequest
                    {
                        NamesRemitente = NamesRemitente,
                        SurnamesRemitente = SurnamesRemitente,
                        ProcessId = (int)eProcess.Taquilla,
                        ClientDocument = ClientDocument,
                        Identifier = Operations.IdOperacion, 
                        Names = Names,
                        Surnames = Surnames,
                        DocumentId = ClientDocument,
                        StatusRowId = StatusOperacionesTemporales.PendienteCobro,
                        Amount = Convert.ToDecimal( model.montoOrden.ToString().Replace(".", ",")),
                    };

                    var resultAgilCheck = (ConsultSearchResponse)invoker.post(Api.ValidateProcess, request, typeof(ConsultSearchResponse));


                }     
                return Json(Operations);
            }
        }
        #endregion

        #region SearchTarifasAplicadas

        public ActionResult SearchTarifasAplicadas(Tarifas_Aplicadas model)
        {
            var invoker = new InvokerServices(this).Instance();
            var data = (List<SearchTarifas_Aplicadas>)invoker.post(Api.SearchTarifasAplicadas, model, typeof(List<SearchTarifas_Aplicadas>));
            return Json(data);
        }

        #endregion

        #region getCorresponsalPais

        [HttpPost]
        public ActionResult SearchCorresponsalPais(string pais, bool deposito, string tipo_remesa, string Corresponsal)
        {

            var invoker = new InvokerServices(this).Instance();
            var result = invoker.post(Api.SearchCorresponsalByCountry, new { Pais = pais, deposito = deposito, tipo_remesa = tipo_remesa }, typeof(List<CorresponsalesPais>)) as List<CorresponsalesPais>;
            return Json(result);
        }

        #endregion

        #region SearchModalityPaymentByCorrespondent

        [HttpPost]
        public ActionResult SearchModalityPaymentByCorrespondent(ModalityPaymentByCorrespondentRequest req)
        {
            var invoker = new InvokerServices(this).Instance();
            try
            {
                var result = invoker.post(Api.SearchModalityPaymentByCorrespondent, req, typeof(List<ModalityPaymentByCorrespondent>)) as List<ModalityPaymentByCorrespondent>;
                return Json(result);
            }
            catch (Exception e)
            {
                var r = new Common.Models.Common.Base();
                r.error = true;
                r.clientErrorDetail = e.Message;
                return Json(r);
            }
        }
        #endregion

        #region SearchInfoByCorresponsal

        [HttpPost]
        public ActionResult SearchInfoByCorresponsal(InfoDepositoByCorresponsalRequest model)
        {

            var invoker = new InvokerServices(this).Instance();
            var result = invoker.post(Api.SearchInfoByCorresponsal, model, typeof(InfoDepositoByCorresponsal)) as InfoDepositoByCorresponsal;
            return Json(result);
        }


        #endregion

        #region CashierAnnulment

        public ActionResult CashierAnnulment()
        {
            var invoker = new InvokerServices(this).Instance();
            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");

            var MotivosAnulacionRequest = new ListDetailRequest { ApplicationId = int.Parse(Resources.SystemConfiguration.AppId), ProcessId = Constant.Process.MotivosAnulaciones, StatusId = Constant.Status.Activo, ListId = Constant.MotivosAnulacionesList.Ordenes };
            var MotivosAnulacion = invoker.post(Api.SearchListDetail, MotivosAnulacionRequest, typeof(List<ListDetail>)) as List<ListDetail>;
            MotivosAnulacion.Insert(0, new ListDetail { DetailId = 0, DetailName = "Seleccione...", DetailCode = "" });
            ViewBag.ListaMotivosAnulacion = new SelectList(MotivosAnulacion, "DetailId", "DetailName");
            return View();
        }

        #endregion

        #region OperationsTempAnnulment

        public ActionResult OperationsTempAnnulment()
        {
            var invoker = new InvokerServices(this).Instance();
            var TiposDeIdentidadRequest = new GenericSearch();
            var TiposDeIdentidad = invoker.post(Api.SearchTiposDeIdentidad, TiposDeIdentidadRequest, typeof(List<GenerincModel>)) as List<GenerincModel>;
            TiposDeIdentidad.Insert(0, new GenerincModel { nombre = "Seleccione...", acronimo = "" });
            ViewBag.ListaTiposDeIdentidad = new SelectList(TiposDeIdentidad, "acronimo", "nombre");

            var MotivosAnulacionRequest = new ListDetailRequest { ApplicationId = int.Parse(Resources.SystemConfiguration.AppId), ProcessId = Constant.Process.MotivosAnulaciones, StatusId = Constant.Status.Activo , ListId =Constant.MotivosAnulacionesList.OperacionesNegocio};
            var MotivosAnulacion = invoker.post(Api.SearchListDetail, MotivosAnulacionRequest, typeof(List<ListDetail>)) as List<ListDetail>;
            MotivosAnulacion.Insert(0, new ListDetail { DetailId =0,DetailName = "Seleccione...", DetailCode = "" });
            ViewBag.ListaMotivosAnulacion = new SelectList(MotivosAnulacion, "DetailId", "DetailName");

            return View();
        }

        #endregion

        #region SearchOperationsTempAnnulment

        public ActionResult SearchOperationsTempAnnulment(OperationsTempAnnulmentRequest model)
        {

            model.Sucursal = User.Claims.FirstOrDefault(c => c.Type == "Sucursal").Value;
            model.IdSucursal = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Oficina").Value);
            model.USUARIO = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            var invoker = new InvokerServices(this).Instance();
            var data = (List<SearchCashierOperations>)invoker.post(Api.SearchOperationsTempAnnulment, model, typeof(List<SearchCashierOperations>));

            return Json(data);
        }

        #endregion

        #region SearchCashierAnnulment

        public ActionResult SearchCashierAnnulment(OperationsCashierAnnulmentRequest model)
        {
            var invoker = new InvokerServices(this).Instance();
            model.REGISTRADOPOR = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            model.SUCURSAL = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Oficina").Value);
            model.SucursalProcesaId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Oficina").Value);
            var data = (List<OrdenAnulables>)invoker.post(Api.SearchOperationsCashierAnnulment, model, typeof(List<OrdenAnulables>));

            return Json(data);
        }

        #endregion

        #region InsertAnnulment

        [HttpPost]
        public ActionResult InsertAnnulment(Annulment model)
        {
            {
                var invoker = new InvokerServices(this).Instance();
                model.CreationUserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                model.CreationUser = User.Claims.FirstOrDefault(c => c.Type == "Login").Value.ToString();
                model.StatusId = Constant.StatusMotivosAnulacion.PendienteAnulacion;

                GenericResponse AnnulmentResponse = new GenericResponse();
                if (model.TableId == Constant.Table.OperacionesPorCobrar)
                {
                    AnnulmentResponse = (GenericResponse)invoker.post(Api.InsertAnnulmentOperationTemp, model, typeof(GenericResponse));
                }
                else { 
                    AnnulmentResponse = (GenericResponse)invoker.post(Api.InsertAnnulmentOrder, model, typeof(GenericResponse));
                }
               
                return Json(AnnulmentResponse);
            }
        }

        #endregion

        #region ApprovalAnnulments

        public ActionResult ApprovalAnnulments()
        {
            return View();
        }

        #endregion

        #region SearchAnnulmentPending

        public ActionResult SearchAnnulmentPending(AnnulmentPendingRequest model)
        {

            var invoker = new InvokerServices(this).Instance();
            model. BranchOfficeId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Oficina").Value);
            var data = (List<AnnulmentPending>)invoker.post(Api.SearchAnnulmentPending, model, typeof(List<AnnulmentPending>));
            return Json(data);
        }

        #endregion

        #region ValidateAproveAnnulment

        public ActionResult ValidateAproveAnnulment(Annulment model)
        {

            var invoker = new InvokerServices(this).Instance();
            model.CreationUser = User.Claims.FirstOrDefault(c => c.Type == "Login").Value.ToString();
            model.UpdateUser = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var data = (GenericResponse)invoker.post(Api.ValidateAproveAnnulment, model, typeof(GenericResponse));
            return Json(model);
        }
        #endregion

        #region ValidateRequestAmount

        public ActionResult ValidateRequestAmount(ValidateRequestAmount model)
        {
            var invoker = new InvokerServices(this).Instance();
            var data = (GenericResponse)invoker.post(Api.ValidateRequestAmount, model, typeof(GenericResponse));
            return Json(data);
        }

        #endregion

        #region OperationsTemp
        public ActionResult OperationsTemp()
        {
            return View();
        }
        #endregion

        #region UpdateStatusOperationsTempEntity
        public ActionResult UpdateStatusOperationsTempEntity(OperacionesPorCobrar model)
        {
            var invoker = new InvokerServices(this).Instance();
            var data = (GenericResponse)invoker.post(Api.UpdateStatusOperationsTempEntity, model, typeof(GenericResponse));
            return Json(data);
        }
        #endregion

        #region SearchOperacionesPorCobrar
        public ActionResult SearchOperacionesPorCobrar(OperacionesPorCobrarRequest model)
        {
            var invoker = new InvokerServices(this).Instance();
            var data = (HashSet<OperacionesPorCobrar>)invoker.post(Api.SearchOperacionesPorCobrar, model, typeof(HashSet<OperacionesPorCobrar>));
            return Json(data);
        }
        #endregion

        #region SearchOperationsTempPending
        public ActionResult SearchOperationsTempPending()
        {
            var invoker = new InvokerServices(this).Instance();
            var data = (List<OperationsTempPending>)invoker.post(Api.SearchOperationsTempPending, null, typeof(List<OperationsTempPending>));
            return Json(data);
        }
        #endregion


        #endregion
    }
}
