using System.Collections.Generic;
using Common.Models.Ccal;
using Interfaces;

namespace IDataAccess.Ccal
{
    public interface ICcalDal<T> : IFinalize
    {
        #region metodos
        /// <summary>
        /// RM - obtener listado de oficinas
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchOficina(object[] param);

        List<Oficinamodel> EntitySearchOficina(Oficinamodel param);
        #endregion
    }
}
