using StockNote.Models;

namespace StockNote.BusinessLayer.Interfaces
{
    public interface IStockInWarehouseManager
    {
        /// <summary>
        /// Add stock item to warehouse, it could be openning the stock in warehouse. 
        /// </summary>
        /// <param name="stockInWarehouse"></param>
        /// <returns></returns>
        Task<Guid> AddStockToWarehouse(StockInWarehouse _stockInWarehouse);
        
        /// <summary>
        /// To update minimum threshold of stock item in specific warehouse
        /// </summary>
        /// <param name="_stockInWarehouse"></param>
        /// <returns></returns>
        Task<bool> UpdateMinimumThresholdOfStock(Guid itemID, Guid warehouseID, Decimal minimumThreshold);
        
        /// <summary>
        /// Get stock-in-warehouse object by warehouse id and item id
        /// One warehouse cannot be duplicate item 
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        Task<StockInWarehouse> GetStockInWarehouse(Guid itemID, Guid warehouseID);

        /// <summary>
        /// To update stock-in-warehouse item which will be called from UpdateMinimumThresholdOfStock method.
        /// </summary>
        /// <param name="_stockInWarehouse"></param>
        /// <returns></returns>
        Task<bool> UpdateStockInWarehouse(StockInWarehouse _stockInWarehouse);

        /// <summary>
        /// All records from StockInWarehouse tables.
        /// This methods for internal use only.
        /// </summary>
        /// <returns></returns>
        Task<List<StockInWarehouse>> GetStockInWarehouses(bool _includeDelete = false);

        /// <summary>
        /// Get stock item list by warehouse with balance 
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <returns></returns>
        Task<List<Item>> GetStockItemListByWarehouse(Guid _warehouseID, bool _includeDelete = false);

        /// <summary>
        /// Get warehouse list where item is available
        /// </summary>
        /// <param name="_itemID"></param>
        /// <returns></returns>
        Task<List<Warehouse>> GetWarehouseListByItem(Guid _itemID, bool _includeDelete = false);
        
        /// <summary>
        /// Deactive (delete) or active a stock-in-warehouse. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result. 
        /// </summary>
        /// <param name="_stockInWarehouseID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        Task<bool> ChangeActiveStatusOfStockInWarehouse(Guid _stockInWarehouseID, bool _active = true);
        
        /// <summary>
        /// [Alert!] This will do hard delete a stockInWarehouse.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data.
        /// </summary>
        /// <param name="_stockInWarehouseID"></param>
        /// <returns></returns>
        Task<bool> DeleteStockInWarehouse(Guid _stockInWarehouseID);
    }
}
