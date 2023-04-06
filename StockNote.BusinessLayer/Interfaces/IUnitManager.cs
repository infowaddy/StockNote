using StockNote.Models;

namespace StockNote.BusinessLayer.Interfaces
{
    public interface IUnitManager
    {
        /// <summary>
        /// Create a new unit.
        /// After creation is success, new unit ID will be return. 
        /// Otherwise, it will return Guid.Empty.
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        Task<Guid> CreateUnit(Unit _unit);
        
        /// <summary>
        /// Return null or one Unit or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        Task<List<Unit>> GetUnits(bool _includeDelete = false);
        
        /// <summary>
        /// Return null or one Unit or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_unitName"></param>
        /// <returns></returns>
        Task<List<Unit>> GetUnits(string _unitName, bool _includeDelete = false);
        
        /// <summary>
        /// Return null or one Unit which match with parameter _unitID
        /// </summary>
        /// <param name="_unitID"></param>
        /// <returns></returns>
        Task<Unit> GetUnit(Guid _unitID, bool _includeDelete = false);
        
        /// <summary>
        /// Return true or false based on success update
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        Task <bool> UpdateUnit(Unit _unit);
        
        /// <summary>
        /// Deactive (delete) or active a unit. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result. 
        /// </summary>
        /// <param name="_unitID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        Task<bool> ChangeActiveStatusOfUnit(Guid _unitID, bool _active = true);
        
        /// <summary>
        /// [Alert!] This will do hard delete a unit.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data.
        /// </summary>
        /// <param name="_unitID"></param>
        /// <returns></returns>
        Task<bool> DeleteUnit(Guid _unitID);

    }
}
