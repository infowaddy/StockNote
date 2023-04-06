using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockNote.BusinessLayer.Interfaces;
using StockNote.DataAccess;
using StockNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockNote.BusinessLayer
{
    public class WarehouseManager : IWarehouseManager, IDisposable
    {
        private StockNoteDBContext stockNoteDBContext;
        private ILogger<WarehouseManager> logger;

        /// <summary>
        /// Construstor with DBContext injection 
        /// </summary>
        /// <param name="_stockNoteDBContext"></param>
        public WarehouseManager(StockNoteDBContext _stockNoteDBContext, ILogger<WarehouseManager> _logger)
        {
            this.stockNoteDBContext = _stockNoteDBContext;
            this.logger = _logger;
        }

        /// <summary>
        /// Create a new warehouse.
        /// After creation is success, new warehouse ID will be return. 
        /// Otherwise, it will return Guid.Empty. 
        /// </summary>
        /// <param name="_warehouse"></param>
        /// <returns></returns>
        public async Task<Guid> CreateWarehouse(Warehouse _warehouse)
        {
            try
            {
                this.stockNoteDBContext.Add<Warehouse>(_warehouse);
                await this.stockNoteDBContext.SaveChangesAsync();
                return _warehouse.ID;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateWarehouse");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Return null or one warehouse or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Warehouse>> GetWarehouses(bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Warehouses.OrderBy(o => o.Name).ToListAsync<Warehouse>();
            else
                return await this.stockNoteDBContext.Warehouses.Where<Warehouse>(x => x.IsActive == true).OrderBy(o => o.Name).ToListAsync<Warehouse>();
        }

        /// <summary>
        /// Return null or one warehouse or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_warehouseName"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Warehouse>> GetWarehouses(string _warehouseName, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Warehouses.Where<Warehouse>(x => x.Name.ToLower().StartsWith(_warehouseName.ToLower())).OrderBy(o => o.Name).ToListAsync();
            else
                return await this.stockNoteDBContext.Warehouses.Where<Warehouse>(x => x.Name.ToLower().StartsWith(_warehouseName.ToLower()) && x.IsActive == true).OrderBy(o => o.Name).ToListAsync();
        }

        /// <summary>
        /// Return null or one warehouse which match with parameter _warehouseID
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<Warehouse> GetWarehouse(Guid _warehouseID, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Warehouses.SingleOrDefaultAsync(x => x.ID == _warehouseID);
            else
                return await this.stockNoteDBContext.Warehouses.SingleOrDefaultAsync(x => x.ID == _warehouseID && x.IsActive == true);
        }

        /// <summary>
        /// Return true or false based on success update
        /// </summary>
        /// <param name="_warehouse"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdateWarehouse(Warehouse _warehouse)
        {
            try
            {
                Warehouse oldWarehouse = await this.stockNoteDBContext.Warehouses.SingleOrDefaultAsync(x => x.ID.Equals(_warehouse.ID));
                this.stockNoteDBContext.Entry(oldWarehouse).CurrentValues.SetValues(_warehouse);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateWarehouse");
                return false;
            }
        }

        /// <summary>
        /// Deactive (delete) or active a warehouse. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result. 
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        public async Task<bool> ChangeActiveStatusOfWarehouse(Guid _warehouseID, bool _active = true)
        {
            try
            {
                Warehouse warehouse = await this.stockNoteDBContext.Warehouses.SingleAsync<Warehouse>(x => x.ID == _warehouseID);
                warehouse.IsActive = _active;
                var result = await this.stockNoteDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ChangeActiveStatusOfWarehouse");
                return false;
            }
        }
        
        /// <summary>
        /// [Alert!] This will do hard delete a stock unit.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data. 
        /// </summary>
        /// <param name="_warehouseID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteWarehouse(Guid _warehouseID)
        {
            try
            {
                Warehouse oldWarehouse = await this.stockNoteDBContext.Warehouses.SingleOrDefaultAsync(x => x.ID.Equals(_warehouseID));
                this.stockNoteDBContext.Warehouses.Remove(oldWarehouse);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteWarehouse");
                return false;
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
