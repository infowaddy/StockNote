using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockNote.BusinessLayer.Interfaces;
using StockNote.DataAccess;
using StockNote.Models;

namespace StockNote.BusinessLayer
{
    public class StockInWarehouseManager : IStockInWarehouseManager, IDisposable
    {
        private StockNoteDBContext stockNoteDBContext;
        private ILogger<StockInWarehouseManager> logger;

        /// <summary>
        /// Construstor with DBContext injection 
        /// </summary>
        /// <param name="_stockNoteDBContext"></param>
        public StockInWarehouseManager(StockNoteDBContext _stockNoteDBContext, ILogger<StockInWarehouseManager> _logger)
        {
            this.stockNoteDBContext = _stockNoteDBContext;
            this.logger = _logger;
        }

        /// <summary>
        /// Add stock item to warehouse, it could be openning the stock in warehouse. 
        /// </summary>
        /// <param name="_stockInWarehouse"></param>
        /// <returns></returns>
        public async Task<Guid> AddStockToWarehouse(StockInWarehouse _stockInWarehouse)
        {
            try
            {
                // check duplicate item
                StockInWarehouse existing = this.stockNoteDBContext.StockInWarehouses.Where(x=>x.Item == _stockInWarehouse.Item
                && x.Warehouse == _stockInWarehouse.Warehouse).FirstOrDefault();
                if (existing != null)
                {
                    // same item is existing in same warehouse.
                    return existing.ID;
                }

                // avoid duplicate child objects creation
                Unit unit = this.stockNoteDBContext.Units.Where(x => x.ID == _stockInWarehouse.Item.Unit.ID).FirstOrDefault();
                Item item = this.stockNoteDBContext.Items.Where(x => x.ID == _stockInWarehouse.Item.ID).FirstOrDefault();
                Warehouse warehouse = this.stockNoteDBContext.Warehouses.Where(x => x.ID == _stockInWarehouse.Warehouse.ID).FirstOrDefault();
                if (item != null)
                    _stockInWarehouse.Item = item;
                if (unit != null)
                    _stockInWarehouse.Item.Unit = unit;
                if (warehouse != null)
                    _stockInWarehouse.Warehouse = warehouse;

                // check stockInWarehouse ID. If null, assign generic GUID
                // because ID is not primary key in stockInWarehouse table
                if (_stockInWarehouse.ID == Guid.Empty)
                    _stockInWarehouse.ID = Guid.NewGuid();
                this.stockNoteDBContext.Add<StockInWarehouse>(_stockInWarehouse);
                await this.stockNoteDBContext.SaveChangesAsync();
                return _stockInWarehouse.ID;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "AddStockToWarehouse");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// To update minimum threshold of stock item in specific warehouse
        /// </summary>
        /// <param name="_stockInWarehouse"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMinimumThresholdOfStock(Guid itemID, Guid warehouseID, Decimal minimumThreshold)
        {
            StockInWarehouse stockInWarehouse = await GetStockInWarehouse(itemID, warehouseID);
            stockInWarehouse.MinBalThreshold = minimumThreshold;
            return await UpdateStockInWarehouse(stockInWarehouse);
        }

        /// <summary>
        /// Get stock-in-warehouse object by warehouse id and item id
        /// One warehouse cannot be duplicate item
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public async Task<StockInWarehouse> GetStockInWarehouse(Guid itemID, Guid warehouseID)
        {
            StockInWarehouse stockInWarehouse = await stockNoteDBContext.StockInWarehouses.Where(x=>x.ItemID == itemID && x.WarehouseID == warehouseID)
                .Include(x=>x.Warehouse)
                .Include(x=>x.Item)
                .ThenInclude(x=>x.Unit).FirstOrDefaultAsync();
            return stockInWarehouse;
        }

        /// <summary>
        /// To update stock-in-warehouse item which will be called from UpdateMinimumThresholdOfStock method.
        /// </summary>
        /// <param name="_stockInWarehouse"></param>
        /// <returns></returns>
        public async Task<bool> UpdateStockInWarehouse(StockInWarehouse _stockInWarehouse)
        {
            try
            {
                StockInWarehouse oldStockInWarehouse= await this.stockNoteDBContext.StockInWarehouses.SingleOrDefaultAsync(x => x.ID.Equals(_stockInWarehouse.ID));
                this.stockNoteDBContext.Entry(oldStockInWarehouse).CurrentValues.SetValues(_stockInWarehouse);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateStockInWarehouse");
                return false;
            }
        }

        /// <summary>
        /// Get stock item list by warehouse with balance 
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Item>> GetStockItemListByWarehouse(Guid _warehouseID, bool _includeDelete = false)
        {
            if (_includeDelete)
               return await this.stockNoteDBContext.StockInWarehouses.Where(x => x.Warehouse.ID == _warehouseID)
                    .Include(x=>x.Warehouse)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Unit)
                    .Select(y => y.Item).OrderByDescending(y => y.Name)
                    .ToListAsync();
            else
                return await this.stockNoteDBContext.StockInWarehouses.Where(x => x.Warehouse.ID == _warehouseID && x.IsActive == true)
                    .Include(x=>x.Warehouse)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Unit)
                    .Select(y => y.Item).Where(y=>y.IsActive == true).OrderByDescending(y => y.Name)
                    .ToListAsync();
        }

        /// <summary>
        /// Get warehouse list where item is available
        /// </summary>
        /// <param name="_itemID"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Warehouse>> GetWarehouseListByItem(Guid _itemID, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.StockInWarehouses.Where(x => x.Item.ID == _itemID)
                    .Include(x=>x.Warehouse)
                    .Include(x=>x.Item)
                    .ThenInclude(x=>x.Unit)
                    .Select(y => y.Warehouse).OrderByDescending(y => y.Name)
                    .ToListAsync();
            else
                return await this.stockNoteDBContext.StockInWarehouses.Where(x => x.Item.ID == _itemID && x.IsActive == true)
                    .Include(x => x.Warehouse)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Unit)
                    .Select(y => y.Warehouse).Where(y => y.IsActive == true).OrderByDescending(y => y.Name)
                    .ToListAsync();
        }

        /// <summary>
        /// All records from StockInWarehouse tables.
        /// This methods for internal use only.
        /// </summary>
        /// <returns></returns>
        public async Task<List<StockInWarehouse>> GetStockInWarehouses(bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.StockInWarehouses
                    .Include(x=>x.Warehouse)
                    .Include(x=>x.Item)
                    .ThenInclude(x=>x.Unit).ToListAsync<StockInWarehouse>();
            else
                return await this.stockNoteDBContext.StockInWarehouses.Where(x=>x.IsActive == true)
                    .Include(x => x.Warehouse)
                    .Include(x => x.Item)
                    .ThenInclude(x=>x.Unit).ToListAsync<StockInWarehouse>();
        }

        /// <summary>
        /// Deactive (delete) or active a stock-in-warehouse. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result.
        /// </summary>
        /// <param name="_stockInWarehouseID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        public async Task<bool> ChangeActiveStatusOfStockInWarehouse(Guid _stockInWarehouseID, bool _active = true)
        {
            try
            {
                StockInWarehouse stockInWarehouse = await this.stockNoteDBContext.StockInWarehouses.SingleAsync<StockInWarehouse>(x => x.ID == _stockInWarehouseID);
                stockInWarehouse.IsActive = _active;
                var result = await this.stockNoteDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ChangeActiveStatusOfStockInWarehouse");
                return false;
            }
        }

        /// <summary>
        /// [Alert!] This will do hard delete a stockInWarehouse.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data.
        /// </summary>
        /// <param name="_stockInWarehouseID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteStockInWarehouse(Guid _stockInWarehouseID)
        {
            try
            {
                StockInWarehouse oldStockInWarehouse = await this.stockNoteDBContext.StockInWarehouses.SingleOrDefaultAsync(x => x.ID.Equals(_stockInWarehouseID));
                this.stockNoteDBContext.StockInWarehouses.Remove(oldStockInWarehouse);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteStockInWarehouse");
                return false;
            }
        }
        public void Dispose()
        {
            
        }
    }
}
