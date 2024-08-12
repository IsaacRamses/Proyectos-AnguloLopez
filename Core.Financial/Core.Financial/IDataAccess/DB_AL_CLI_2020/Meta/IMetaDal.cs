using System.Collections.Generic;
using Interfaces;

namespace IDataAccess.Meta
{
    public interface IMetaDal<T> : IFinalize
    {
        #region Metodos
        /// <summary>
        /// RM - Obtener Listado de Sexos desde la BD
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchSex(object[] param);

        /// <summary>
        /// RM - Obtener listados de sectores
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchSector(object[] param);

        /// <summary>
        /// RM - Obtener listado de tipos de movilizaciones
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchMovilizaciones(object[] param);

        /// <summary>
        /// RM - obtener listados de condicines de vivienda
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCondicionesVivienda(object[] param);

        /// <summary>
        /// RM - Obtener listados de origen de fondos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchOrigenFndos(object[] param);

        /// <summary>
        /// RM - Obtener listado de Destino de fondos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchDestinoFondos(object[] param);

        /// <summary>
        /// RM - Pbtener listados de categorias SB116
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCategoriasSB116(object[] param);

        /// <summary>
        /// RM - obteer listado de estados civiles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchEstadosCiviles(object[] param);

        /// <summary>
        /// RM - obteer listado de actividades economicas
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchActividadesEconomicas(object[] param);

        HashSet<T> SearchOtrosIngresos(object[] param);

        /// <summary>
        /// RM - Obtener listado de tipos de identidad
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchTiposDeIdentidad(object[] param);

        /// <summary>
        /// RM - obtener listado de tipos de ficha
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchTiposFicha(object[] param);

        /// <summary>
        /// RM - Obtener listado de profesiones
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchProfesiones(object[] param);

        /// <summary>
        /// RM - obtener listado de ocupaciones
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchOcupaciones(object[] param);

        /// <summary>
        /// RM - obtener listado de niveles academicos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchNivelesAcademicos(object[] param);

        /// <summary>
        /// RM - obtener una lista de status
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchStatus(object[] param);

        /// <summary>
        /// RM - obtener una lista de productos bancarios
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> searchBankProducts(object[] param);
        
        /// <summary>
        /// RM - obtener una lista de entidades del sector bancario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchBanksInstitutions(object[] param);

        /// <summary>
        /// RM - Obtener una lista de cargos para los tipos de accionistas de un cliente juridico.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCargo(object[] param);

        /// <summary>
        /// RM - obtener una lista de procedencia de ingresos.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchProcedenciaIngresos(object[] param);
        #endregion
    }
}
