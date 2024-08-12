using System.Collections.Generic;
using Interfaces;

namespace IDataAccess.Common
{
    public interface ICommonDal<T> : IFinalize
    {
        #region Metodos

        /// <summary>
        /// RM - Obtener una lista de tipos de documentos
        /// </summary>
        /// <param name="paaram"></param>
        /// <returns></returns>
        HashSet<T> SearchDocumentTypes(object[] paaram);

        /// <summary>
        /// RM - obetner paramaetros de configuracion de un aplicacion
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchApllicationParameter(object[] param);
        #endregion
    }
}
