using StockNote.Models;

namespace StockNote.BusinessLayer.Interfaces
{
    public interface IWarehouseManager
    {
        /// <summary>
        /// Create a new warehouse.
        /// After creation is success, new warehouse ID will be return. 
        /// Otherwise, it will return Guid.Empty.
        /// </summary>
        /// <param name="_warehouse"></param>
        /// <returns></returns>
        Task<Guid> CreateWarehouse(Warehouse _warehouse);
        
        /// <summary>
        /// Return null or one warehouse or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        Task<List<Warehouse>> GetWarehouses(bool _includeDelete = false);
        
        /// <summary>
        /// Return null or one warehouse or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_warehouseName"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        Task<List<Warehouse>> GetWarehouses(string _warehouseName, bool _includeDelete = false);
        
        /// <summary>
        /// Return null or one warehouse which match with parameter _warehouseID
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <returns></returns>
        Task<Warehouse> GetWarehouse(Guid _warehouseID, bool _includeDelete = false);
        
        /// <summary>
        /// Return true or false based on success update
        /// </summary>
        /// <param name="_Warehouse"></param>
        /// <returns></returns>
        Task<bool> UpdateWarehouse(Warehouse _Warehouse);
        
        /// <summary>
        /// Deactive (delete) or active a warehouse. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result. 
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        Task<bool> ChangeActiveStatusOfWarehouse(Guid _warehouseID, bool _active = true);
        
        /// <summary>
        /// [Alert!] This will do hard delete a stock unit.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data. 
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <returns></returns>
        Task<bool> DeleteWarehouse(Guid _warehouseID);
    }
}
