using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDataAccess.Geo
{
    public interface IGeoDal<T> : IFinalize
    {
        #region Metodos
        /// <summary>
        /// RM - obtener lista de municipios
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> Searchmunicipios(object[] param);

        /// <summary>
        /// RM - obtener lista de paises
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCountries(object[] param);

        /// <summary>
        /// RM - obtener lista de estados
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchEstados(object[] param);

        /// <summary>
        /// RM - obtener lista de parroquias
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchParroquias(object[] param);

        /// <summary>
        /// RM - obtener lista de codigos postales
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> searhcCodigosPostales(object[] param);

        /// <summary>
        /// RM - obtener lista de ciudades
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCiudades(object[] param);

        #endregion
    }
}
