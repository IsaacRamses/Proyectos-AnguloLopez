using Common.Models.Angulo_Lopez.Ingresos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.Financial.Api.Utils
{
    public class Application : Base
    {
        public virtual int ApplicationId { get; set; }
        [Display(Name = "Status:")]
        public virtual string StatusId { get; set; }
        public virtual string StatusName { get; set; }
        public virtual string UserCreteId { get; set; }
        public virtual string UserCreateName { get; set; }
        public virtual string UserModifiedId { get; set; }
        public virtual string UserModifiedName { get; set; }
        [Display(Name = "Nombre:")]
        public virtual string ApplicationName { get; set; }
        public virtual DateTime ApplicationDateCreate { get; set; }
        [Display(Name = "Descripción:")]
        public virtual string ApplicationDescription { get; set; }
        public virtual DateTime? ApplicationDateModified { get; set; }
        public virtual List<SelectListItem> DropDownListStatus { get; set; }
    }

    public class ApplicationResult : Base
    {
        public virtual bool status { get; set; }

    }

    public class Banco : Base
    {
        public string codBanco { get; set; }
        public string nombreBanco { get; set; }
        public string idPais { get; set; }
        public string activo { get; set; }
        public string ccal { get; set; }
        public string depositoActivo { get; set; }
    }
    public class Base
    {
        public bool error { get; set; }
        public string clientErrorDetail { get; set; }
        public Exception errorDetail { get; set; }
        public string apiDetail { get; set; }
        public string current { get; set; }
        public string idResult { get; set; }

    }
    public class Beneficiario : Base
    {
        public int idPersona { get; set; }
        public int idCliente { get; set; }
        public string tipoIdentificacion { get; set; }
        public string paisNacimiento { get; set; }
        public string numeroIdentificacion { get; set; }
    }
    public class Cheques
    {
        public virtual string NumeroCheque { get; set; }
        public virtual string NumeroCuenta { get; set; }
        public virtual decimal Monto { get; set; }
        public virtual int BancoId { get; set; }
        public virtual string BancoName { get; set; }
    }
    public class Ciudad : Base
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string idPais { get; set; }
    }
    public class Cliente : Base
    {
    }
    [Serializable]
    public partial class Fichas
    {
        public string PersitentObjectId { get; set; }
        [DisplayName("Nro. Ficha")]
        [Required]
        public int FichaConsecutivo { get; set; }
        [DisplayName("Estatus")]
        public int? Estatus { get; set; }
        [DisplayName("Nro. Cédula")]
        [StringLength(14, ErrorMessage = "Máximo 14 caracteres permitido")]
        public string IdCedula { get; set; }
        [DisplayName("Nro. Pasaporte")]
        [StringLength(14, ErrorMessage = "Máximo 14 caracteres permitido")]
        public string IdPasaporte { get; set; }
        [DisplayName("Nro. RIF")]
        [StringLength(14, ErrorMessage = "Máximo 14 caracteres permitido")]
        public string IdRif { get; set; }
        [DisplayName("Primer Apellido")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Debe introducir al menos 3 caracteres y máximo 50")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Por favor introduzca un Nombre")]
        public string PrimerApellido { get; set; }
        [DisplayName("Segundo Apellido")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres permitido")]
        public string SegundoApellido { get; set; }
        [DisplayName("Primer Nombre")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres permitido")]
        public string PrimerNombre { get; set; }
        [DisplayName("Segundo Nombre")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres permitido")]
        public string SegundoNombre { get; set; }
        [DisplayName("Nombres y Apellidos")]
        public string NombreCompleto { get; set; }
        [DisplayName("Fotografía")]
        public string Foto { get; set; }
        [DisplayName("País Emisor Pasaporte")]
        public string PaisEmisorPasaporteFk { get; set; }
        [DisplayName("País Procedencia")]
        public string PaisProcedenciaFk { get; set; }
        [DisplayName("Nacionalidad")]
        public string NacionalidadFk { get; set; }
        [DisplayName("Otra Nacionalidad")]
        public string Nacionalidad2Fk { get; set; }
        [DisplayName("Género")]
        public string Sexo { get; set; }
        [DisplayName("Fecha de Nacimiento")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaNacimiento { get; set; }
        [DisplayName("Lugar de Nacimiento")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres permitido")]
        public string LugarNacimiento { get; set; }
        [DisplayName("País Nacimiento")]
        public string PaisNacimientoFk { get; set; }
        [DisplayName("Estado Civil")]
        public string EstadoCivilFk { get; set; }
        [DisplayName("Nivel Académico")]
        public string NivelAcademicoFk { get; set; }
        [DisplayName("Profesión")]
        public string ProfesionFk { get; set; }
        [DisplayName("Ocupación")]
        public string OcupacionFk { get; set; }
        [DisplayName("Actividad Económica")]
        public string ActividadEconomicaFk { get; set; }
        [DisplayName("Av / Calle")]
        public string DireccionAvenidaCalle { get; set; }
        [DisplayName("Casa / Edificio / Quinta")]
        public string DireccionCasaEdifQuinta { get; set; }
        [DisplayName("Piso / Nivel")]
        public string DireccionPiso { get; set; }
        [DisplayName("Nro Apartamento / Casa")]
        public string DireccionApartamento { get; set; }
        [DisplayName("Urbanización")]
        public string DireccionUrbanizacion { get; set; }
        [DisplayName("Estado")]
        public int? DireccionEstadoFk { get; set; }
        [DisplayName("Municipio")]
        public int? DireccionMunicipioFk { get; set; }
        [DisplayName("Parroquía")]
        public int? DireccionParroquiaFk { get; set; }
        [DisplayName("Ciudad")]
        public int? DireccionCiudadFk { get; set; }
        [DisplayName("Código Postal")]
        public string DireccionCodigoPostalFk { get; set; }
        [DisplayName("Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Teléfono Habitación")]
        public string TelfHabitacion { get; set; }
        [DisplayName("Teléfono Celular")]
        [Required]
        public string TelfMovil1 { get; set; }
        [DisplayName("Teléfono Celular 2")]
        public string TelfMovil2 { get; set; }
        [DisplayName("Teléfono Adicional")]
        public string TelfOtro { get; set; }
        [DisplayName("Fecha de Vencimiento del documento")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CedulaFechaVencimiento { get; set; }
        [DisplayName("Fecha de Vencimiento Pasaporte")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PasaporteFechaVencimiento { get; set; }

        [DisplayName("Fecha de Vencimiento RIF")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RifFechaVencimiento { get; set; }
        [DisplayName("Oficina Preferida")]
        public string OficinaFk { get; set; }
        [DisplayName("Razon Social Empresa")]
        public string EmpresaNombre { get; set; }
        [DisplayName("Sector")]
        public string EmpresaSector { get; set; }
        [DisplayName("RIF")]
        public string EmpresaRif { get; set; }
        [DisplayName("Cargo")]
        public string EmpresaCargo { get; set; }
        [DisplayName("Av / Calle")]
        public string EmpresaAvenida { get; set; }
        [DisplayName("Piso / Nivel")]
        public string EmpresaPiso { get; set; }
        [DisplayName("Casa / Edificio / Quinta")]
        public string EmpresaCasaEdif { get; set; }
        [DisplayName("Nro Apartamento / Oficina")]
        public string EmpresaApartamento { get; set; }
        [DisplayName("Urbanización")]
        public string EmpresaUrbanizacion { get; set; }
        [DisplayName("Estado")]
        public int? EmpresaEstadoFk { get; set; }
        [DisplayName("Ciudad")]
        public int? EmpresaCiudadFk { get; set; }
        [DisplayName("Municipio")]
        public int? EmpresaMunicipioFk { get; set; }
        [DisplayName("Parroquía")]
        public int? EmpresaParroquiaFk { get; set; }
        [DisplayName("Código Postal")]
        public string EmpresaCodigoPostalFk { get; set; }
        [DisplayName("Es Beneficiario?")]
        public bool? EsBeneficiario { get; set; }
        [DisplayName("Ingresos Mensuales")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(14, 3)")]
        public decimal? ClienteIngresos { get; set; }
        [DisplayName("Monto Promedio Compra Mensual")]
        [DataType(DataType.Currency)]
        //[Required]
        public string EfectivoCompraPromedio { get; set; }
        [DisplayName("Monto Promedio Venta Mensual")]
        [DataType(DataType.Currency)]
        //[Required]
        public string EfectivoVentasPromedio { get; set; }
        [DisplayName("Cantidad Transacciones Compra Mensual")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Number")]
        public int? EfectivoCompraNumTransacciones { get; set; }
        [DisplayName("Cantidad Transacciones Venta Mensual")]
        [Range(1, 100)]
        public int? EfectivoVentaNumTransacciones { get; set; }
        [DisplayName("Necesita Enviar Remesas?")]
        public bool? RemesasNecesitaEnviar { get; set; }
        [DisplayName("Necesita Recibir Remesas?")]
        public bool? RemesasNecesitaRecibir { get; set; }
        [DisplayName("Pais donde envía Remesas?")]
        public string RemesasEnviarPaisDestinoFk { get; set; }
        [DisplayName("Monto Promedio Envío de Remesas?")]
        public string RemesasEnviarPromedio { get; set; }
        [DisplayName("Cantidad de Envío de Remesas Mensuales?")]
        public int? RemesasEnviarTransaccionesMensuales { get; set; }
        [DisplayName("Motivo Envío de Remesas?")]
        public string RemesasEnviarMotivo { get; set; }
        [DisplayName("Pais de donde va a recibir Remesas?")]
        public string RemesasRecibirPaisOrigenFk { get; set; }
        [DisplayName("Monto Promedio Recepción de Remesas?")]
        public string RemesasRecibirPromedio { get; set; }
        [DisplayName("Cantidad de recepción de Remesas Mensuales?")]
        public int? RemesasRecibirTransaccionesMensuales { get; set; }
        [DisplayName("Motivo Recepción de Remesas?")]
        public string RemesasRecibirMotivo { get; set; }
        [DisplayName("Elaborado Por")]
        public string ElaboradoPorFk { get; set; }
        [DisplayName("Revisado Por")]
        public string RevizadoPorFk { get; set; }
        [DisplayName("Aprobado Por")]
        public string AprobadoPorFk { get; set; }
        [DisplayName("Observaciones")]
        public string Observaciones { get; set; }

        public string WebRegistroIp { get; set; }
        public string WebRegistroLatitud { get; set; }
        public string WebRegistroLongitug { get; set; }
        public byte[] WebRegistroGoogleMapsUrl { get; set; }
        public int? TotalTransaccionesMensuales { get; set; }
        public bool? IsDeleted { get; set; }
        [DisplayName("Nro Ficha Impresión")]
        public int? FichaImpresionNumero { get; set; }
        [DisplayName("Tipo de Identificación")]
        public string ClienteTipo { get; set; }
        [DisplayName("Tipo de Ficha")]
        public int? fichaTipo { get; set; }
        [DisplayName("Razón Social")]
        public string nombreRazonSocial { get; set; }
        [DisplayName("Condición Vivienda")]
        public string CondicionVivienda { get; set; }
        [DisplayName("Nombre del Cónyuge")]
        public string nombreConyuge { get; set; }
        [DisplayName("Nro Cédula del Cónyuge")]
        public string id_cedula_conyuge { get; set; }
        [DisplayName("El Cliente es PEP?")]
        public bool? is_pep { get; set; }
        [DisplayName("El Cliente esta Relacionado a un PEP?")]
        public bool? is_relacionado_pep { get; set; }
        [DisplayName("El Cliente es cercano a un PEP?")]
        public bool? is_cercano_pep { get; set; }
        [DisplayName("Nombre del PEP")]
        public string nombre_pep { get; set; }
        [DisplayName("El Cliente es asociado de un PEP?")]
        public bool? is_asociado_pep { get; set; }
        [DisplayName("Nombre Empresa del PEP")]
        public string nombre_empresa_pep { get; set; }
        [DisplayName("Cargo en la Empresa del PEP")]
        public string cargo_pep { get; set; }
        [DisplayName("País Relación con el PEP")]
        public string pais_pepFK { get; set; }
        [DisplayName("Cliente con reprensentante legal/apoderado/autorizado")]
        public bool? tiene_representante_legal { get; set; }
        [DisplayName("Nombre completo reprensentante legal/apoderado/autorizado")]
        public string nombre_completo_representante { get; set; }
        [DisplayName("Lugar Nacimiento")]
        public string lugar_nacimiento_representante { get; set; }
        [DisplayName("Fecha Nacimiento")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fecha_nacimiento_representante { get; set; }

        [DisplayName("Telefono")]
        public string telefono_RepresentanteLegal { get; set; }

        [DisplayName("Tipo de documento de identidad")]
        public string tipoDocumentoRepresentanteLegal { get; set; }

        [DisplayName("Nro. Documento")]
        public string numeroDocumentoRepresentanteLegal { get; set; }

        [DisplayName("Datos del Documetno")]
        public string datosDocumentoRepresentanteLegal { get; set; }

        [DisplayName("Categoria Especial")]
        public string categoria_especial { get; set; }
        [DisplayName("Es Independiente?")]
        public bool? is_dependiente { get; set; }
        [DisplayName("Origen de los Fondos")]
        public string origen_fondos { get; set; }
        [DisplayName("Destino de los Fondos")]
        public string destino_fondos { get; set; }
        [DisplayName("Nombre Comercial")]
        public string nombre_comercial_empresa { get; set; }
        [DisplayName("Nombre del Registro Público")]
        public string nombre_registro_empresa { get; set; }
        [DisplayName("Número del Registro")]
        public string numero_registro_empresa { get; set; }
        [DisplayName("Número de Tomo del Registro")]
        public string tomo_registro_empresa { get; set; }
        [DisplayName("Folio del Registro")]
        public string folio_registro_empresa { get; set; }

        [DisplayName("Fecha Constitución Empresa")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string fecha_constitucion_empresa { get; set; }

        [DisplayName("Fecha vencimiento junta directiva actual")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string FechaVencimientoJuntaDirectivaActual { get; set; }

        [DisplayName("Fecha vencimiento junta directiva")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string FechaVencimientoJuntaDirectiva { get; set; }

        [DisplayName("Monto Capital Social")]
        [DataType(DataType.Currency, ErrorMessage = "El valor debe corresponder a un monto")]
        public decimal? capital_social_empresa { get; set; }
        [DisplayName("Nombre del Registro Público actual")]
        public string nombre_actualizacion_empresa { get; set; }
        [DisplayName("Número de la Actualización del registro")]
        public string numero_actualizacion_empresa { get; set; }
        [DisplayName("Número de Tomo de la Actualización")]
        public string tomo_actualizacion_empresa { get; set; }
        [DisplayName("Folio de la Actualización")]
        public string folio_actualizacion_empresa { get; set; }
        [DisplayName("Fecha Actualización de la Empresa")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fecha_actualizacion_empresa { get; set; }
        [DisplayName("Monto Capital Social Actual")]
        [DataType(DataType.Currency, ErrorMessage = "El valor debe corresponder a un monto")]
        public decimal? capital_social_actual_empresa { get; set; }
        [DisplayName("Es un Ente Público?")]
        public bool? is_ente_publico { get; set; }
        [DisplayName("Número Gaceta Oficial")]
        public string numero_gaceta_oficial_ente_publico { get; set; }
        [DisplayName("Fecha Creación")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fecha_creacion_ente_publico { get; set; }
        [DisplayName("Nombre de Adscripción")]
        public string autoridad_adscripcion { get; set; }
        [DisplayName("Código Oficina Nacional del Tesoro")]
        public string codigo_ont_ente_publico { get; set; }
        [DisplayName("Sitio web")]
        public string pagina_web_empresa { get; set; }
        [DisplayName("Av / Calle")]
        public string DireccionAvenidaCalleEntePublico { get; set; }
        [DisplayName("Casa / Edificio / Quinta")]
        public string DireccionCasaEdifQuintaEntePublico { get; set; }
        [DisplayName("Piso / Nivel")]
        public string DireccionPisoEntePublico { get; set; }
        [DisplayName("Nro Apartamento / Casa")]
        public string DireccionApartamentoEntePublico { get; set; }
        [DisplayName("Urbanización")]
        public string DireccionUrbanizacionEntePublico { get; set; }
        [DisplayName("Estado")]
        public int? DireccionEstadoFkEntePublico { get; set; }
        [DisplayName("Municipio")]
        public int? DireccionMunicipioFkEntePublico { get; set; }
        [DisplayName("Parroquía")]
        public int? DireccionParroquiaFkEntePublico { get; set; }
        [DisplayName("Ciudad")]
        public int? DireccionCiudadFkEntePublico { get; set; }
        [DisplayName("Código Postal")]
        public string DireccionCodigoPostalFkEntePublico { get; set; }
        [DisplayName("Teléfonos")]
        public string telefono_ente_publico { get; set; }
        [DisplayName("Correo Electrónico")]
        [DataType(DataType.EmailAddress)]
        public string correo_electronico_ente_publico { get; set; }
        [DisplayName("Carga Familiar")]
        public int? carga_familiar { get; set; }
        [DisplayName("Actividad Económica Cónyugue")]
        public string actividadEconomicaFKConyugue { get; set; }
        [DisplayName("Av / Calle")]
        public string direccionAvenidaCalle_Conyuge { get; set; }
        [DisplayName("Casa / Edificio / Quinta")]
        public string direccionCasaEdifQuinta_Conyuge { get; set; }
        [DisplayName("Piso / Nivel")]
        public string direccionPiso_Conyuge { get; set; }
        [DisplayName("Nro Apartamento / Casa")]
        public string direccionApartamento_Conyuge { get; set; }
        [DisplayName("Urbanización")]
        public string direccionUrbanizacion_Conyuge { get; set; }
        [DisplayName("Estado")]
        public int? direccionEstadoFK_Conyuge { get; set; }
        [DisplayName("Municipio")]
        public int? direccionMunicipioFK_Conyuge { get; set; }
        [DisplayName("Parroquía")]
        public int? direccionParroquiaFK_Conyuge { get; set; }
        [DisplayName("Ciudad")]
        public int? direccionCiudadFK_Conyuge { get; set; }
        [DisplayName("Código Postal")]
        public string direccionCodigoPostalFK_Conyuge { get; set; }
        [DisplayName("Actividad Económica Otros Ingresos")]
        public string actividadEconomicaFKOtrosIngresos { get; set; }
        [DisplayName("Ramo Empresa")]
        public string empresa_ramo { get; set; }
        [DisplayName("Fecha Ingreso Empresa")]
        [DataType(DataType.Date, ErrorMessage = "El valor debe corresponder a una Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? empresa_fecha_ingreso { get; set; }
        [DisplayName("Otros Ingresos")]
        [DataType(DataType.Currency, ErrorMessage = "El valor debe corresponder a un monto")]
        public decimal? monto_otros_ingresos { get; set; }
        [DisplayName("Teléfono")]
        public string empresaTelefono { get; set; }
        [DisplayName("Teléfono Adicional")]
        public string empresaTelefonoOtro { get; set; }
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string empresaEmail { get; set; }
        [DisplayName("Tipo de documento")]
        public string conyugeTipo { get; set; }

        public bool? Poseesubsidiarias { get; set; }

        public bool? declaroISLR { get; set; }

        [DisplayName("Número de subsidiarias")]
        public int? numeroSubsidiarias { get; set; }

        [DisplayName("País con mayor presencia")]
        public string PaisMayorPresencia { get; set; }

        [DisplayName("Número de empleados")]
        public int? NumeroEmpleados { get; set; }

        [DisplayName("Ventas Mensuales")]
        public decimal? VentasMensuales { get; set; }

        [DisplayName("Ingresos mensuales")]
        public decimal? Ingresosmensuales { get; set; }

        [DisplayName("Egresos mensuales")]
        public decimal? Egresosmensuales { get; set; }

        [DisplayName("Año ultima declaración ISLR")]
        public int? AnoUltimaDeclaracionISLR { get; set; }

        [DisplayName("Monto ultima declaración ISLR")]
        public decimal? MontoUltimaDeclaracionISLR { get; set; }

        [DisplayName("Monto delaración ISLR año en curso")]
        public decimal? MontoDeclaracionAnoEnCursoISLR { get; set; }

        public string enteAdscripcionEntePublico { get; set; }

        [DisplayName("Cargo del Representante Legal")]
        public string CargoRepresentanteLegalEmpresa { get; set; }

        [DisplayName("Condicion del Representante Legal")]
        public string CondicionRepresentanteLegalEmpresa { get; set; }

        [DisplayName("Posee referencias Bancarias")]
        public bool? poseeReferenciasBancarias { get; set; }

        public bool? LoadSuccess { get; set; }
        //public List<CustomerDocument> Documents { get; set; } = new List<CustomerDocument>();
        //public List<BankReference> bankReferences { get; set; } = new List<BankReference>();
        //public List<PersonalRefence> personalReferences { get; set; } = new List<PersonalRefence>();
        //public List<RelatedCompany> relatedCompanies { get; set; } = new List<RelatedCompany>();
        //public List<Person> RelatedPersons { get; set; } = new List<Person>();
    }

    public partial class Fichas : Base
    {
        [NotMapped]
        public bool NuevoRegistro { get; set; } = false;
        [DisplayName("Total Registros")]
        [NotMapped]
        public int total { get; set; }
        [DisplayName("Nro. Documento")]
        [StringLength(14, ErrorMessage = "Máximo 14 caracteres permitido")]
        [NotMapped]
        public string id_cedula { get; set; }
        [DisplayName("Nro. Pasaporte")]
        [StringLength(14, ErrorMessage = "Máximo 14 caracteres permitido")]
        [NotMapped]
        public string id_pasaporte { get; set; }
        [DisplayName("Nro. RIF")]
        [StringLength(14, ErrorMessage = "Máximo 14 caracteres permitido")]
        [NotMapped]
        public string id_rif { get; set; }
        [NotMapped]
        public bool? DireccionIgualCliente { get; set; }
    }
    public class ClienteRequest : Base
    {
        public ClienteRequest()
        {
            valida = true;
        }
        public string tipoId { get; set; }
        public string numeroId { get; set; }
        public bool valida { get; set; }
        public string persitentObjectID { get; set; }
    }
    public class BeneficiarioCorresponsalRequest : Base
    {
        public string tipoId { get; set; }
        public string numeroId { get; set; }
        public string referencia { get; set; }
        public string corresponsal { get; set; }
    }
    public partial class SearchClientsRequest : Base
    {
        public int? fichaConsecutivo { get; set; }
        public string persitentObjectID { get; set; }
        public string id_cedula { get; set; }
        public string id_pasaporte { get; set; }
        public string id_rif { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public int? offSet { get; set; }
        public int? limit { get; set; }

    }
    public class ControlMonetario : Base
    {
        public virtual decimal MontoMinimoOperacion { get; set; }
        public virtual decimal MontoMaximoOperacion { get; set; }
        public virtual decimal MontoUsd { get; set; }


    }
    public class ControlMonetarioRequest
    {
        public virtual int MonedaId { get; set; }
        public virtual decimal MontoMinimoUsd { get; set; }
        public virtual decimal MontoMaximoUsd { get; set; }
        public virtual decimal MontoOperacion { get; set; }
        public virtual bool Compra { get; set; }
        public virtual decimal TasaDolar { get; set; }
    }
    public class Corresponsal : Base
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string idPais { get; set; }
    }
    public class InfoOficinaDeposito : Base
    {
        public string idCorresponsal { get; set; }
        public int idOficina { get; set; }
        public int idCiudad { get; set; }
        public string idPagador { get; set; }
        public decimal tasa { get; set; }
    }
    public class Denominaciones : Base
    {
        public int idDenominacion { get; set; }
        public decimal denominacion { get; set; }
        public int idTipoDenominacion { get; set; }
        public string tipoDenominacion { get; set; }
        public int cantidad { get; set; }
        public string moneda { get; set; }
        public decimal subTotal { get; set; }
    }
    public class Monedas : Base
    {
        public virtual int MonedaId { get; set; }
        public virtual int TipoCambioId { get; set; }
        public virtual string TipoCambioName { get; set; }
        public virtual string UserCreateId { get; set; }
        public virtual string UserCreateName { get; set; }
        public virtual string UserModifiedId { get; set; }
        public virtual string UserModifiedName { get; set; }
        public virtual bool MonedaActiva { get; set; }
        public virtual bool MonedaInterna { get; set; }
        public virtual decimal? MonedaComisionCompra { get; set; }
        public virtual decimal? MonedaComisionVenta { get; set; }
        public virtual string MonedaCodigoInt { get; set; }
        public virtual string MonedaName { get; set; }
        public virtual decimal? MonedaValorCompra { get; set; }
        public virtual decimal? MonedaValorVenta { get; set; }
        public virtual DateTime MonedaDateCreate { get; set; }
        public virtual string MonedaSimbolo { get; set; }
        public virtual string MonedaMarcoLegal { get; set; }
        public virtual bool? MonedaManejoEfectivo { get; set; }
        public virtual string MonedaCodigoProfit { get; set; }
        public virtual DateTime? MonedaDateModified { get; set; }
        public virtual decimal? MonedaValorCompraReuters { get; set; }
        public virtual decimal? MonedaValorVentaReuters { get; set; }
    }

    public class MonedasRequest
    {
        public virtual int? TipoCambioId { get; set; }
        public virtual bool? MonedaActiva { get; set; }
        public virtual bool? MonedaInterna { get; set; }
        public virtual bool? MonedaManejoEfectivo { get; set; }
    }
    public class MonetarySummary : Base
    {
        public virtual string NombreCajero { get; set; }
        public virtual string LoginCajero { get; set; }
        public virtual string CodigoMoneda { get; set; }
        public virtual string Moneda { get; set; }
        public virtual decimal Monto { get; set; }
    }
    public class MotivoOperacion : Base
    {
        public string id { get; set; }
        public string motivo { get; set; }
        public virtual bool Interna { get; set; }
    }
    public class MotivosAnulacionOrdenes : Base
    {
        public int id { get; set; }
        public bool activa { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
    }
    public class Oficina : Base
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public int id { get; set; }
        public string pagador { get; set; }
        public bool deposito { get; set; }
        public string corresponsal { get; set; }
        public decimal tasa { get; set; }
        public int corresponsalId { get; set; }
    }

    public class CorresponsalPais
    {
        public string tipo_remesa { get; set; }
        public bool deposito { get; set; }
        public string Pais { get; set; }
        public string Corresponsal { get; set; }
    }


    public class OficinasRequest : Base
    {
        public long idCiudad { get; set; }
        public string tipoOperacion { get; set; }
        public decimal monto { get; set; }
    }
    public class Orden : Base
    {
        public int idCliente { get; set; }
        public string idClienteUnique { get; set; }
        public string tipoIdCliente { get; set; }
        public string numeroIdCliente { get; set; }
        public string nombresCliente { get; set; }
        public string ClientNames { get; set; }
        public string ClientSurnames { get; set; }
        public string telefonoCliente { get; set; }
        public string emailCliente { get; set; }
        public decimal montoOrden { get; set; }
        public decimal tasaCambio { get; set; }
        public DateTime fechaValorTasa { get; set; }
        public byte temporal { get; set; }
        public string observaciones { get; set; }
        public virtual string Cajero { get; set; }
        public virtual string Analista { get; set; }
        public virtual string TypeOperationId { get; set; }
        public virtual string TypeOperationName { get; set; }
        public virtual int NumberTypeOperation { get; set; }
        public virtual string Moneda { get; set; }
        public virtual string SimboloMoneda { get; set; }
        public virtual int MonedaOperacion { get; set; }
        public virtual int MonedaConversion { get; set; }
        public virtual string MontoConversion { get; set; }
        public virtual string TasaConversion { get; set; }
        public virtual string SimboloMonedaOperacion { get; set; }
        public virtual int idMotivoOferta { get; set; }
        public virtual decimal AmmountTotalConversion { get; set; }
        public virtual bool TakeCommission { get; set; }
        public virtual List<Detalle_Pago_Recibido> PaymentDetail { get; set; }
        public virtual Pagos_Recibidos_Cliente Payment { get; set; }
        public virtual bool MixedOperation { get; set; }
        public virtual string OrderNumber { get; set; }
        public virtual string CiuOrig { get; set; }
        public virtual int SucursalProcesa { get; set; }
        public virtual string ModificadoPor { get; set; }
        public virtual DateTime Modificado { get; set; }
        public virtual string ReferenciaPago { get; set; }
        public virtual string CountryCodeIso { get; set; }
        

    }

    public class OrdenAnulacion : Orden
    {
        public int id { get; set; }
        public string letra { get; set; }
        public int numero { get; set; }
        public int status { get; set; }
        public string tabla { get; set; }
        public string fecha { get; set; }
        public decimal monto { get; set; }
        public int pagoCliente { get; set; }
        public int descuento { get; set; }
        public string descripcionStatus { get; set; }
        public int? idMotivo { get; set; }
        public string motivo { get; set; }
        public string motivoBcv { get; set; }
        public string tipoOpBcv { get; set; }
        public bool simadi { get; set; }
        public string referenciaBCV { get; set; }
        public string AnulAutorizadaPor { get; set; }
        public string ReferenciaAnulBcv { get; set; }
        public bool Procesada { get; set; }
        public string AnuladaPor { get; set; }
        public string UsuarioAnula { get; set; }
        public string BeneficiarioName { get; set; }
    }

    public class OrdenDevolucion : Orden
    {
        public int id { get; set; }
        public string letra { get; set; }
        public int numero { get; set; }
        public int status { get; set; }
        public string tabla { get; set; }
        public string fecha { get; set; }
        public decimal monto { get; set; }
        public decimal montoDevolver { get; set; }
        public int pagoCliente { get; set; }
        public int descuento { get; set; }
        public string descripcionStatus { get; set; }
        public int? idMotivo { get; set; }
        public string motivo { get; set; }
        public string motivoBcv { get; set; }
        public string tipoOpBcv { get; set; }
        public bool simadi { get; set; }
        public int idDevolucion { get; set; }
        public bool documentosRegla { get; set; }
        public int tipoCancelacion { get; set; }
        public List<Denominaciones> denominaciones { get; set; }
        public string referencia { get; set; }
        public string idPais { get; set; }
        public virtual decimal TarifaUSD { get; set; }
        public int tipo_pago { get; set; }
        public int detalle_tipo_pago { get; set; }

    }

    public class OrdenResult : Base
    {
        public string status { get; set; }

    }
    public class OrdenRequest : Base
    {
        public string sucursal { get; set; }
        public long numero { get; set; }
    }

    public partial class RequestBCV : Base
    {
        public virtual string TipoIdentidad { get; set; }
        public virtual string NumeroIdentidad { get; set; }
        public virtual int NumeroOperacion { get; set; }
        public virtual decimal MontoOperacion { get; set; }
    }
    public class OrdenCompraEfectivo : Orden
    {
        public OrdenCompraEfectivo()
        {
        }
        public OrdenCompraEfectivo(Common.Models.Angulo_Lopez.Operaciones.OrdenCompraEfectivo model)
        {
            id = model.id;
            bancoCliente = model.bancoCliente;
            numeroCuentaCliente = model.numeroCuentaCliente;
            tipoIdBeneficiario = model.tipoIdBeneficiario;
            numeroIdBeneficiario = model.numeroIdBeneficiario;
            nombresBeneficiario = model.nombresBeneficiario;
            idPaisDestino = model.idPaisDestino;
            nombreBancoDestino = model.nombreBancoDestino;
            numeroCuentaDestino = model.numeroCuentaDestino;
            direccionBancoDestino = model.direccionBancoDestino;
            aba = model.aba;
            swift = model.swift;
            iban = model.iban;
            tipoOferta = model.tipoOferta;
            motivoOferta = model.motivoOferta;
            idCliente = model.idCliente;
            idClienteUnique = model.idClienteUnique;
            tipoIdCliente = model.tipoIdCliente;
            numeroIdCliente = model.numeroIdCliente;
            nombresCliente = model.nombresCliente;
            ClientNames = model.ClientNames;
            ClientSurnames = model.ClientSurnames;
            telefonoCliente = model.telefonoCliente;
            emailCliente = model.emailCliente;
            montoOrden = model.montoOrden;
            tasaCambio = model.tasaCambio;
            fechaValorTasa = model.fechaValorTasa;
            temporal = model.temporal;
            observaciones = model.observaciones;
            Cajero = model.Cajero;
            Analista = model.Analista;
            TypeOperationId = model.TypeOperationId;
            TypeOperationName = model.TypeOperationName;
            NumberTypeOperation = model.NumberTypeOperation;
            Moneda = model.Moneda;
            SimboloMoneda = model.SimboloMoneda;
            MonedaOperacion = model.MonedaOperacion;
            MonedaConversion = model.MonedaConversion;
            MontoConversion = model.MontoConversion;
            TasaConversion = model.TasaConversion;
            SimboloMonedaOperacion = model.SimboloMonedaOperacion;
            idMotivoOferta = model.idMotivoOferta;
            AmmountTotalConversion = model.AmmountTotalConversion;
            TakeCommission = model.TakeCommission;
            MixedOperation = model.MixedOperation;
            OrderNumber = model.OrderNumber;
            tarifas = new List<Tarifa>();
            NombresRemitente = model.NombresRemitente;
            DocumentoRemitente = model.DocumentoRemitente;
            ModoPago = model.ModoPago;
            MontoBolivares = model.MontoBolivares;
            PaisId = model.PaisId;
            CorresponsalId = model.CorresponsalId;
            PorConfirmar = model.PorConfirmar;

        }
        public long id { get; set; }
        public string bancoCliente { get; set; }
        public string numeroCuentaCliente { get; set; }
        public string tipoIdBeneficiario { get; set; }
        public string numeroIdBeneficiario { get; set; }
        public string nombresBeneficiario { get; set; }
        public string idPaisDestino { get; set; }
        public string nombreBancoDestino { get; set; }
        public long numeroCuentaDestino { get; set; }
        public string direccionBancoDestino { get; set; }
        public string aba { get; set; }
        public string swift { get; set; }
        public string iban { get; set; }
        public string tipoOferta { get; set; }
        public string motivoOferta { get; set; }
        public List<Tarifa> tarifas { get; set; }
        public string NombresRemitente { get; set; }
        public string DocumentoRemitente { get; set; }
        public string ModoPago { get; set; }
        public decimal MontoBolivares { get; set; }
        public string PaisId { get; set; }
        public string CorresponsalId { get; set; }
        public bool PorConfirmar { get; set; }

    }
    public class OrdenCompraCorresponsal : Orden
    {
        public string tipoIdBeneficiario { get; set; }
        public string numeroIdBeneficiario { get; set; }
        public string identificacionBeneficiario { get; set; }
        public string nombreBeneficiario { get; set; }
        public string telefonoBeneficiario { get; set; }
        public string telefono2Beneficiario { get; set; }
        public string referencia { get; set; }
        public string referenciaBCV { get; set; }
        public long secuencia { get; set; }
        public string modo { get; set; }
        public string corresponsal { get; set; }
        public string codigoCorresponsal { get; set; }
        public decimal montoPagar { get; set; }
        public DateTime fechaOrden { get; set; }
        public string monedaMonto { get; set; }
        public string fechaOrdenMostrar { get; set; }
        public long id { get; set; }
        public string identificacionOrdenante { get; set; }
        public string nombreOrdenante { get; set; }
        public string telefonoOrdenante { get; set; }
        public decimal tasaExterna { get; set; }
        public string codigoPais { get; set; }
        public string nombrePais { get; set; }
        public string isoCodePais { get; set; }
        public int idStatus { get; set; }
        public string statusOrden { get; set; }
        public string tipoOferta { get; set; }
        public string motivoOferta { get; set; }
        public List<Tarifa> tarifas { get; set; }
        public string idPaisDestino { get; set; }
        public string bancoIntermediario { get; set; }
        public string numeroCuentaIntermediario { get; set; }
        public string direccionBancoIntermediario { get; set; }
        public string abaIntermediario { get; set; }
        public string swiftIntermediario { get; set; }
        public string ibanIntermediario { get; set; }
        /*
           string BANCO_INTERMEDIARIO, 
           string NUMERO_CUENTA_INTERMEDIARIO,
           string DIRECCION_BANCO_INTERMEDIARIO,
           string ABA_INTERMEDIARIO,
           string SWIFT_INTERMEDIARIO,
           string IBAN_INTERMEDIARIO
        */
    }
    public class OrdenVentaTransferencia : Orden
    {
        public long id { get; set; }
        public string bancoCliente { get; set; }
        public string numeroCuentaCliente { get; set; }
        public string tipoIdBeneficiario { get; set; }
        public string numeroIdBeneficiario { get; set; }
        public string nombresBeneficiario { get; set; }
        public string idPaisDestino { get; set; }
        public string nombreBancoDestino { get; set; }
        public string numeroCuentaDestino { get; set; }
        public string direccionBancoDestino { get; set; }
        public string aba { get; set; }
        public string swift { get; set; }
        public string iban { get; set; }
        public string tipoOferta { get; set; }
        public string motivoOferta { get; set; }
        public List<Tarifa> tarifas { get; set; }
        public string bancoIntermediario { get; set; }
        public string numeroCuentaIntermediario { get; set; }
        public string direccionBancoIntermediario { get; set; }
        public string abaIntermediario { get; set; }
        public string swiftIntermediario { get; set; }
        public string ibanIntermediario { get; set; }
        public string idCorresponsal { get; set; }
        /*
           string BANCO_INTERMEDIARIO, 
           string NUMERO_CUENTA_INTERMEDIARIO,
           string DIRECCION_BANCO_INTERMEDIARIO,
           string ABA_INTERMEDIARIO,
           string SWIFT_INTERMEDIARIO,
           string IBAN_INTERMEDIARIO
        */

        public string TypeAccountBank { get; set; }
    }
    public class OrdenVentaEfectivo : Orden
    {
        public long id { get; set; }
        public string bancoCliente { get; set; }
        public string numeroCuentaCliente { get; set; }
        public string tipoIdBeneficiario { get; set; }
        public string numeroIdBeneficiario { get; set; }
        public string nombresBeneficiario { get; set; }
        public string idPaisDestino { get; set; }
        public int idCiudadDestino { get; set; }
        public int idOficina { get; set; }
        public string emailBeneficiario { get; set; }
        public string telefonoBeneficiario { get; set; }
        public string aba { get; set; }
        public string swift { get; set; }
        public string iban { get; set; }
        public string tipoOferta { get; set; }
        public string motivoOferta { get; set; }
        public List<Tarifa> tarifas { get; set; }

        public string nombreBancoDestino { get; set; }
        public string numeroCuentaDestino { get; set; }
        public string TipoCuentaBeneficiario { get; set; }
        public string DireccionBancoDestino { get; set; }

    }
    public class Pagador : Base
    {
        public int id { get; set; }
        public string idPagador { get; set; }
        public string corresponsal { get; set; }
        public string pais { get; set; }
        public bool activo { get; set; }
        public bool retransmision { get; set; }
        public int distance { get; set; }
        public string nombre { get; set; }
    }
    public class Pais : Base
    {
        public string idPais { get; set; }
        public string nombrePais { get; set; }
        public bool activo_envio { get; set; }
        public string codeIso { get; set; }
    }
    public class Parametro : Base
    {
        public int IdParametro { get; set; }
        [Display(Name = "Nombre:")]
        public string que { get; set; }
        [Display(Name = "Valor:")]
        public long cuanto { get; set; }
        [Display(Name = "Desde Cuando:")]
        public DateTime cuando { get; set; }
        [Display(Name = "Aplicación::")]
        public string app { get; set; }
        [Display(Name = "Nombre Ejecutable:")]
        public string exeName { get; set; }
        [Display(Name = "Para Quien:")]
        public string referencia { get; set; }
        public string RegistradoPor { get; set; }
        public string Description { get; set; }
        public string ModificadoPor { get; set; }
        public string UserName { get; set; }
        [JsonIgnore]
        public virtual List<SelectListItem> DropDownListApp { get; set; }
    }

    public class ParametroResult : Base
    {
        public virtual bool status { get; set; }
    }

    public class ParametroRequest
    {
        public virtual int ParameterId { get; set; }
        public virtual string ParameterName { get; set; }
    }
    public class Pase : Base
    {
        //MONTO	FECHA	USUARIO	LOGIN_USUARIO	NUMERO_CONTROL	NOMBRE_MONEDA	CODIGO_INT	SIMBOLO
        public decimal monto { get; set; }
        public DateTime fecha { get; set; }
        public string nombreUsuario { get; set; }
        public string login { get; set; }
        public long numeroControl { get; set; }
        public string moneda { get; set; }
        public string codigoMoneda { get; set; }
        public string simbolo { get; set; }
        public string fechaMostrar { get; set; }


    }
    public class PaseRequest : Base
    {
        public long numero { get; set; }
        public string sucursal { get; set; }
    }
    public class Prueba
    {
        public virtual int operacion { get; set; }
        public virtual string nombre { get; set; }
    }
    public class Sucursal : Base
    {
        public string region { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string telefonos { get; set; }
        public string responsable { get; set; }
        public string codigo { get; set; }
        public string letra { get; set; }
        public bool esAgencia { get; set; }
    }
    public class Remesa : Base
    {
        [Display(Name = "N° de Remesa:")]
        public virtual int RemesaId { get; set; }
        [Display(Name = "Transporte de Valores:")]
        public virtual int TransporteId { get; set; }
        [Display(Name = "Transporte de Valores:")]
        public virtual string TransporteName { get; set; }
        [Display(Name = "Tipo de Moneda:")]
        public virtual string MonedaId { get; set; }
        [Display(Name = "Moneda:")]
        public virtual string MonedaName { get; set; }
        [Display(Name = "Nro de Cataporte:")]
        public virtual string Cataporte { get; set; }
        [Display(Name = "Nro de Precinto:")]
        public virtual string Precinto { get; set; }
        public virtual string UserId { get; set; }
        public virtual string UserName { get; set; }
        [Display(Name = "Monto Total:")]
        public virtual decimal Total { get; set; }
        [Display(Name = "Monto Total Moneda:")]
        public virtual decimal TotalMoneda { get; set; }
        [Display(Name = "Monto Total Billetes:")]
        public virtual decimal TotalBillete { get; set; }
        [Display(Name = "Tipo de Remesa:")]
        public virtual string TypeRemesaId { get; set; }
        public virtual string TypeRemesa { get; set; }
        public virtual string UsuarioAnula { get; set; }
        public virtual string UsuarioAnulaName { get; set; }
        [Display(Name = "Motivo de Anulación:")]
        public virtual int MotivoAnulacion { get; set; }
        public virtual string MotivoAnulacionName { get; set; }
        [Display(Name = "Observación:")]
        public virtual string ObservacionAnulacion { get; set; }
        public virtual string CoordinadorAutoriza { get; set; }
        public virtual bool Procesado { get; set; }
        [Display(Name = "N° de Remesa:")]
        public virtual int NumeroControl { get; set; }
        public virtual string BranchId { get; set; }
        public virtual string LetraBranch { get; set; }
        public virtual List<SelectListItem> DropDownListMotivoAnulacion { get; set; }
        public virtual List<Denominaciones> Denominaciones { get; set; }
        public virtual List<Cheques> Cheques { get; set; }
        public virtual bool Autorizar { get; set; }
        [Display(Name = "Nombres y Apellidos Persona Realizará Depósito:")]
        public virtual string UsuarioTransladaId { get; set; }
        public virtual string UsuarioTransladaName { get; set; }


    }
    public class Tarifa
    {
        public int id { get; set; }
        public int idTarifa { get; set; }
        public string concepto { get; set; }
        public string moneda { get; set; }
        public decimal valor { get; set; }
        public string valor2 { get; set; }
        public string simbolo { get; set; }
        public virtual bool Sale { get; set; }
        public virtual bool IsRate { get; set; }
        public virtual string Title { get; set; }
        public virtual bool IgnoreInView { get; set; }
        public virtual string InternalId { get; set; }
    }

    public class RateRequest: Base
    {

    }

    public class TarifaReq : Base
    {
        public string tipoOperacion { get; set; }
        public decimal montoEnviar { get; set; }
        public string moneda { get; set; }
        public string idPais { get; set; }
        public string idCorresp { get; set; }
        public string tipoId { get; set; }
        public decimal tasa { get; set; }
        public virtual int MonedaOperacion { get; set; }
        public virtual int MonedaConversion { get; set; }
        public virtual bool TakeCommission { get; set; }
        
        public virtual OperationType OperationType { get; set; }
    }
    public class TarifaResult : Base
    {
        public decimal montoCobrar { get; set; }
        public string montoCobrar2 { get; set; }
        public List<Tarifa> tarifas { get; set; }
        public string montoTotalBs { get; set; }
        public string montoTotalUsd { get; set; }
        public virtual string MonedaOperacionSimbolo { get; set; }
        public virtual string MonedaConversionSimbolo { get; set; }
        public virtual string MonedaValorOperacionVenta { get; set; }
        public virtual string MonedaValorOperacionCompra { get; set; }
        public virtual string MonedaValorConversionVenta { get; set; }
        public virtual string MonedaValorConversionCompra { get; set; }
        public virtual string MontoTotalConversion { get; set; }
        public virtual decimal AmmountConversion { get; set; }
        public virtual string AmmountConversionFormat { get; set; }
        public virtual decimal AmmountConversionWithoutRate { get; set; }
        public virtual string AmmountConversionWithoutRateFormat { get; set; }
        public virtual string FechaTasa { get; set; }
        public virtual decimal TotalCommissionBs { get; set; }
        public virtual string TotalCommissionBsFormat { get; set; }

    }
    public class TasaCambio : Base
    {
        public string codigo { get; set; }
        public decimal compra { get; set; }
        public decimal venta { get; set; }
        public string fechaValor { get; set; }
        public bool Internaa { get; set; }
    }

    public class TipoCancelacion : Orden
    {
        public int id { get; set; }
        public string tipo { get; set; }
    }

    public class TipoIdentidad : Base
    {
        public string idTipo { get; set; }
        public string tipoIdentidad { get; set; }
    }
    public class IdentidadLocal : Base
    {
        public bool nacional { get; set; }
    }

    public class TipoMovimientosSimadi : Base
    {
        public string id { get; set; }
        public string tipoMovimiento { get; set; }
        public string idSudeban { get; set; }
        public string requestTarifa { get; set; }
        public string idBcv { get; set; }
    }
    public class TipoMovimientoRequest : Base
    {
        public string idTipoIdentidad { get; set; }
        public string keyword { get; set; }
    }

    public class TiposPago : Base
    {
        public int id { get; set; }
        public string tipoPago { get; set; }
    }
    public class DetalleTipoPago : Base
    {
        public int id { get; set; }
        public int idTipoPago { get; set; }
        public string detalleTipoPago { get; set; }
    }
    public class Usuario : Base
    {
        public string login { get; set; }
        public string apellidos { get; set; }
        public string nombres { get; set; }
        public string cedula { get; set; }
        public string idSucursal { get; set; }
        public string letraSucursal { get; set; }
        public bool cajaIndependiente { get; set; }
        public bool formaLibre { get; set; }
        public bool esAgencia { get; set; }
        public int nivelSeguridad { get; set; }
        public string status { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaCambioClave { get; set; }
        public string fechaUltimoLogin { get; set; }
        public string ultimaVersion { get; set; }
        public string ultimaIp { get; set; }
        public string nombreCompleto { get; set; }
        public string turno { get; set; }
        public string emailCorporativo { get; set; }
        public string emailPersonal { get; set; }
        public string telefonoCelular1 { get; set; }
        public string telefonoCelular2 { get; set; }
        public string tipoIdentidad { get; set; }
        public string numeroIdentidad { get; set; }
        public string nombreSucursal { get; set; }
        public string direccionSucursal { get; set; }
        public string cargo { get; set; }
        public string codigoCargo { get; set; }
        public string codigoEmpleado { get; set; }
        public int idOficinaNew { get; set; }
        public int idOficinaExterna { get; set; }
        public int idCiudadExterna { get; set; }
        public int idSucursal2 { get; set; }
    }
    public class usuarioDisponibilidadReq
    {
        public string user { get; set; }
        public decimal monto { get; set; }
    }
    public class usuarioDisponibilidadRet
    {
        public bool disponible { get; set; }
    }

    public enum OperationType
    {
        VentaEfectivo,
        CompraEfectivo,
        Corresponsal,
        Transferencia
    }
}