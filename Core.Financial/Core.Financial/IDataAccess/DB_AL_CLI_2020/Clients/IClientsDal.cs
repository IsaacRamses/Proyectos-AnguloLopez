using Common.Models.Common;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models.Clientes;

namespace IDataAccess.Clients
{
    public interface IClientsDal<T> : IFinalize
    {
        #region Metodos
        #region Clientes
        /// <summary>
        /// RM - metodo para buscar la lista de clientes
        /// </summary>
        /// <param name="param">parametros de busqeda</param>
        /// <returns>lista de clieb¡ntes segun criteriosd e busqueda</returns>
        HashSet<T> Searchfichas(object[] param);
        #endregion

        /// <summary>
        /// RM - Metodo para contar las fichas registradas
        /// </summary>
        /// <returns>numero de fichas registradas activas</returns>
        int CountFichas();

        /// <summary>
        /// RM - metodo para insertar o actualizar una ficha. si existe la actualiza, de lo contrario la crea
        /// </summary>
        /// <param name="param">parametros de la ficha</param>
        /// <returns>entero indicando el numero de registros afectados</returns>
        int InsertUpdateFicha(object[] param);

        /// <summary>
        /// RM - Metodo para eliminar una ficha
        /// </summary>
        /// <param name="param">parametros para eliminar la ficha</param>
        /// <returns>entero indicando el numero de registros afectados</returns>
        int deleteFicha(object[] param);

        /// <summary>
        /// RM - crear una ficha parcial a partir de los datos delc iente el el portal de preregistro.
        /// si existe un cliente con los mismoa datos (cedula y correo) retorna el persistentObjectId del cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns>persistentObjectID del cliente</returns>
        string InsertFichaParcial(object[] param);

        /// <summary>
        /// Actualizar la informacion de un usuario que lleno su informcion personal 
        /// a traves de el portal de preregistro.
        /// </summary>
        /// <param name="param"></param>
        bool CompleteUserInfo(object[] param);

        /// <summary>
        /// RM - Insertar referecnias personales para u cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int InsertPersonalReference(object[] param);

        /// <summary>
        /// RM - obyener referencias personales de un cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchPersonalReferences(object[] param);

        /// <summary>
        /// RM - borrar una referencia personal por id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int DeletePersonalReference(object[] param);

        /// <summary>
        /// RM - Obtener una lista de empresas Relacionadas a un cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchEmpresasRelacionadas(object[] param);

        /// <summary>
        /// RM - Insertar una empresa Relacinada a un cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int InsertEmpresaRelacionada(object[] param);

        /// <summary>
        /// RM - Eliminar una empresa relacionada a un cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int DeleteEmpresaRelacionada(object[] param);

        /// <summary>
        /// RM - obetner una lista de personas asicadas un cliente.
        /// </summary>
        /// <param name="param">filtro con el tipo de persona y el id del cliente.</param>
        /// <returns></returns>
        HashSet<T> SearchPerson(object[] param);

        /// <summary>
        /// RM - eliminar una persona relacionada a un cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int DeletePerson(object[] param);

        /// <summary>
        /// RM - Insertar una persona relacionada a un cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int insertPeson(object[] param);

        /// <summary>
        /// RM - Actualizar el estatus de una ficha de cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int UpdateEstatusFicha(object[] param);

        /// <summary>
        /// RM - Actualizar el estatus de una ficha de cliente por fichaConsecutivo.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int UpdateStausConsecutivoFicha(object[] param);
        /// <summary>
        /// RM - insertar documento para un cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int InsertCustomerDocument(object[] param);

        /// <summary>
        /// RM - borrar documento para un cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int DeleteCustomerDocument(object[] param);

        /// <summary>
        /// RM - obtener una lista de documentos de un cliente
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCustomerDocuments(object[] param);

        /// <summary>
        /// RM - Metodo para agregar referencias bacarias a un cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int InsertBankReference(object[] param);

        /// <summary>
        /// RM - obtener las referencias bancarias de un cliente.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        HashSet<T> SearchCustomerBankReferences(object[] param);

        /// <summary>
        /// RM - Eliminar una referencia bancaria
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        int DeleteBankReference(object[] param);

        Base UpdateAcceptDeclaration(object[] param);

        List<SourceIncome> EntitySearchSourceIncome(Fichas param);

        GenericResponse EntityInsertSourceIncome(List<SourceIncome> param);

        GenericResponse EntityUpdateSourceIncome(List<SourceIncome> param);

        GenericResponse EntityDeleteSourceIncome(List<SourceIncome> param);

        #region CustomerDocuments
        T DeleteCustomerDocuments(object[] param);
        #endregion

        #region EntityValidateClientInCompany
        GenericResponse EntityValidateClientInCompany(SearchClientsRequest param);
        #endregion

        #region EntitySearchStatusFichaLog
        List<StatusFichaLog> EntitySearchStatusFichaLog(StatusFichaLog param);
        #endregion

        #region EntityInsertStatusFichaLog
        GenericResponse EntityInsertStatusFichaLog(StatusFichaLog param);
        #endregion

        #endregion
    }
}
