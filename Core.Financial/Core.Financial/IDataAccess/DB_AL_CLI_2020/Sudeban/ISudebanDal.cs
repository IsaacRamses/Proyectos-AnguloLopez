using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDataAccess.Sudeban
{
    public interface ISudebanDal<T> : IFinalize
    {
        #region Metodos
        /// <summary>
        /// RM - Obtener una lista de SB03
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchSB03(object[] param);
        #endregion
    }
}
