using Common.Models.Angulo_Lopez.Seguridad;
using Common.Models.Angulo_Lopez.TablasMaestras;
using System.Collections.Generic;

namespace WebTaquilla.Resources
{
    public static class SystemConfiguration
    {
        public static string AppId { get; set; }
        public static string LoginUrl { get; set; }
        public static string ApiUrl { get; set; }
        public static string CookieName { get; set; }
        public static string SessionName { get; set; }
        public static bool ChangePassword { get; set; }
    }
    public static class EmailConfiguration
    {
        public static string Server { get; set; }
        public static int Port { get; set; }
        public static string UserAuthEmail { get; set; }
        public static string UserAuthPassword { get; set; }
        public static string FromEmail { get; set; }
        public static string FromEmailDisplay { get; set; }
        public static bool IsSsl { get; set; }
    }

    public static class NivelSeguridad
    {
        public static int AdministradorBanco = 6;
        public static int CajeroBanco = 7;
        public static int PrevencionBanco = 8;
    }

    public static class CompanyData
    {
        public const string Rif = "J302075661";
        public const string CompanyName = "Casa de Cambio Angulo Lopez CA";
        public const string PaymentConcept = "Pago de Remesa";
        public const string AccountNumberVenezuela = "12345678901234567890";
        public const string AccountNumberBanesco = "12345678901234567890";
        public const string AccountNumberPlaza = "12345678901234567890";
        public const string AccountNumberFondoComun = "12345678901234567890";

    }

    public static class Listas
    {
        public static int OperacionesBoveda = 18;
        public static int TipoOperacionesBoveda = 19;
        public static int TipoOperacionesCaja = 20;
    }

}
