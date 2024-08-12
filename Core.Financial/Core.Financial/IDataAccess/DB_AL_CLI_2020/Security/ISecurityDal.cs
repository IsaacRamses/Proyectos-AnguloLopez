using Common.Models.Common;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDataAccess.Security
{
    public interface ISecurityDal<T> : IFinalize
    {
        #region Metodos
        #region EmailRegistration
        /// <summary>
        /// RM - Metdo para agregar un email en la tabla de preRegistro de clientes
        /// </summary>
        /// <param name="param">Email a registar</param>
        /// <returns></returns>
        int RegisterEmail(object[] param);

        /// <summary>
        /// RM - metodo para buscar un email en la tabla de preregistro de clientes
        /// </summary>
        /// <param name="param">Email que se desea bscar</param>
        /// <returns></returns>
        T SearchEmailregistration(object[] param);

        /// <summary>
        /// RM - metodo para confirmar el registro de un email
        /// </summary>
        /// <param name="param">los datos para confirmar el registro</param>
        /// <returns></returns>
        int ConfirmEmailregistration(object[] param);

        /// <summary>
        /// RM - metodo para crear un loguin a partir de un email previamente verificado
        /// </summary>
        /// <param name="param">parametros para la insersion</param>
        /// <returns></returns>
        int InsertUserLogin(object[] param);

        /// <summary>
        /// RM - metodo para hacer login desde la base de datos
        /// </summary>
        /// <param name="param">datos del loguin</param>
        /// <returns></returns>
        T Login(object[] param);
        /// <summary>
        /// SP que debe devolver un usuario que sea válido para iniciar sesión, esta consulta se utliza en el filtro de autorización
        /// </summary>
        /// <param name="param">datos del loguin</param>
        /// <returns></returns>
        T SearchUserValidForSessionPortal(object[] param);
        /// <summary>
        /// RM - cambiar la contraseña de un usuario
        /// </summary>
        /// <param name="param">parametros para el cambio de contraseña</param>
        /// <returns></returns>
        T ChangeUserPassword(object[] param);

        /// <summary>
        /// RM - Metodo para guardar preguntas de seguridad
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool InsertSecurityQuestions(object[] param);

        /// <summary>
        /// RM - Metodo para buscar preguntas de seguridad
        /// </summary>
        /// <param name="param">parametros de busqueda</param>
        /// <returns></returns>
        HashSet<T> SearchQuestions(object[] param);

        /// <summary>
        /// RM - metodo para mostrar las preguntas de seguridad de un susario
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        HashSet<T> SearchSecurityQuestions(object[] param);

        /// <summary>
        /// RM - metodo para validar si las preguntas y respuestas de seguridad de un usuario son correctas
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        bool ValidateSecurityQuestions(object[] param);

        /// <summary>
        /// RM - Metodo para resetear la contraseña de un usuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string ResetUserPassword(object[] param);

        /// <summary>
        /// RM - Determinar si un usuario ha cargado du informacion personal o no.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool CheckForUserPersonalData(object[] param);


        HashSet<T> SearchCustomer(object[] param);
        T ResetCustomerPassword(object[] param);
        #endregion
        #endregion
    }
}