using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockNote.BusinessLayer.Interfaces;
using StockNote.DataAccess;
using StockNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StockNote.BusinessLayer
{
    public class StockTransactionManager : IStockTransactionManager, IDisposable
    {
        private StockNoteDBContext stockNoteDBContext;
        private ILogger<StockTransactionManager> logger;
        public StockTransactionManager(StockNoteDBContext _stockNoteDBContext, ILogger<StockTransactionManager> _logger)
        {
            this.stockNoteDBContext = _stockNoteDBContext;
            this.logger = _logger;
        }

        /// <summary>
        /// Get StockTransaction by ID
        /// </summary>
        /// <param name="_transactionID"></param>
        /// <returns></returns>
        public async Task<StockTransaction> GetStockTransaction(Guid _transactionID, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.StockTransactions.Where(x=>x.ID == _transactionID)
                    .Include(x=>x.FromWarehouse)
                    .Include(x=>x.ToWarehouse)
                    .Include(x=>x.Item)
                    .ThenInclude(x=>x.Unit).FirstOrDefaultAsync();
            else
                return await this.stockNoteDBContext.StockTransactions.Where(x => x.ID == _transactionID && x.IsActive == true)
                    .Include(x => x.FromWarehouse)
                    .Include(x => x.ToWarehouse)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Unit).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Generic stock transaction method which will insert to 
        /// stock transaction table and update to stock-in-warehouse table
        /// </summary>
        /// <param name="_stockTransaction"></param>
        /// <returns></returns>
        public async Task<Guid> CreateStockTransaction(StockTransaction _stockTransaction)
        {
            try
            {
                // check developer forgot to assign transaction date
                if (_stockTransaction.TransactionDate < new DateTime(1900, 1, 1))
                    _stockTransaction.TransactionDate = DateTime.Now;                

                // get fromStockInWarehouse record
                StockInWarehouse fromWarehouse = this.stockNoteDBContext.StockInWarehouses.Where(x => x.Warehouse.ID == _stockTransaction.FromWarehouse.ID
                && x.Item.ID == _stockTransaction.Item.ID)
                    .Include(x=> x.Warehouse)
                    .Include(x=>x.Item)
                    .ThenInclude(x=>x.Unit).FirstOrDefault();
                
                // reduce from from warehouse
                fromWarehouse.Balance = fromWarehouse.Balance - _stockTransaction.Quantity;
                
                this.stockNoteDBContext.Update<StockInWarehouse>(fromWarehouse);

                _stockTransaction.FromWarehouse = fromWarehouse.Warehouse;
                _stockTransaction.Item = fromWarehouse.Item;

                // get toStockInWarehouse record                
                if (_stockTransaction.ToWarehouse != null && _stockTransaction.IsSold != true) {
                    
                    StockInWarehouse toWarehouse = this.stockNoteDBContext.StockInWarehouses.Where(x => x.Warehouse.ID == _stockTransaction.ToWarehouse.ID
                    && x.Item.ID == _stockTransaction.Item.ID)
                        .Include(x=>x.Warehouse)
                        .Include(x=>x.Item)
                        .ThenInclude(x=>x.Unit).FirstOrDefault();

                    // add to warehouse 
                    toWarehouse.Balance = toWarehouse.Balance + _stockTransaction.Quantity;
                    this.stockNoteDBContext.Update<StockInWarehouse>(toWarehouse);

                    _stockTransaction.ToWarehouse = toWarehouse.Warehouse;
                }

                this.stockNoteDBContext.StockTransactions.Add(_stockTransaction);


                await this.stockNoteDBContext.SaveChangesAsync();

                return _stockTransaction.ID;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateStockTransaction");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockTransaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> CreateSaleTransaction(StockTransaction _stockTransaction)
        {
            try
            {
                // make sure the sale transaction 
                _stockTransaction.IsSold = true;
                _stockTransaction.ToWarehouse = null;

                // update remark for tracing
                if (_stockTransaction.Remark == null || _stockTransaction.Remark.Length == 0)
                    _stockTransaction.Remark = "Sale transaction";  
                else
                    _stockTransaction.Remark += " [Sale transaction]";

                return await CreateStockTransaction(_stockTransaction);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateSaleTransaction");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// There is no real amendment transaction after save.
        /// But can do cancel old transaction and create a new.
        /// </summary>
        /// <param name="_oldStockTransaction"></param>
        /// <param name="_newStockTransaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> AmendmentTransaction(StockTransaction _oldStockTransaction, StockTransaction _newStockTransaction)
        {
            await CancelStockTransaction(_oldStockTransaction);
            return await CreateStockTransaction(_newStockTransaction);
        }

        /// <summary>
        /// Cancellation of stock transaction is reverse of create stock transaction
        /// Need to add back stock amount to from-warehouse and
        /// Need to reduce back stock amount to to-warehouse 
        /// And insert (create) canceled-stock-transaction with cancel remark
        /// </summary>
        /// <param name="_stockTransaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> CancelStockTransaction(StockTransaction _stockTransaction)
        {
            try
            {
                // get fromStockInWarehouse record
                StockInWarehouse fromWarehouse = this.stockNoteDBContext.StockInWarehouses.Where(x => x.Warehouse.ID == _stockTransaction.FromWarehouse.ID
                && x.Item.ID == _stockTransaction.Item.ID)
                    .Include(x=>x.Warehouse)
                    .Include(x=>x.Item)
                    .ThenInclude(x=>x.Unit).FirstOrDefault();
                
                // add back to from warehouse
                fromWarehouse.Balance = fromWarehouse.Balance + _stockTransaction.Quantity;
                this.stockNoteDBContext.Update<StockInWarehouse>(fromWarehouse);

                // get toStockInWarehouse record
                StockInWarehouse toWarehouse = null;
                if (_stockTransaction.ToWarehouse != null && _stockTransaction.IsSold != true)
                {
                    toWarehouse = this.stockNoteDBContext.StockInWarehouses.Where(x => x.Warehouse.ID == _stockTransaction.ToWarehouse.ID
                    && x.Item.ID == _stockTransaction.Item.ID)
                        .Include(x => x.Warehouse)
                        .Include(x => x.Item)
                        .ThenInclude(x => x.Unit).FirstOrDefault();
                   
                    // reduce (deduct) back from to-warehouse 
                    toWarehouse.Balance = toWarehouse.Balance - _stockTransaction.Quantity;
                    this.stockNoteDBContext.Update<StockInWarehouse>(toWarehouse);
                }

                StockTransaction cancelTransaction = new StockTransaction();
                cancelTransaction.IsSold = _stockTransaction.IsSold;
                cancelTransaction.TransactionDate = _stockTransaction.TransactionDate;
                cancelTransaction.FromWarehouse = fromWarehouse.Warehouse;
                if (toWarehouse != null)
                    cancelTransaction.ToWarehouse = toWarehouse.Warehouse;
                cancelTransaction.Quantity = _stockTransaction.Quantity;
                cancelTransaction.Item = fromWarehouse.Item;
                cancelTransaction.Remark = _stockTransaction.Remark;

                // update remark for tracing
                if (cancelTransaction.Remark == null || cancelTransaction.Remark.Length == 0)
                    cancelTransaction.Remark = "Cancel transaction";
                else
                    cancelTransaction.Remark += " [Cancel transaction]";

                this.stockNoteDBContext.Add<StockTransaction>(cancelTransaction);
                await this.stockNoteDBContext.SaveChangesAsync();

                return cancelTransaction.ID;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CancelStockTransaction");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Get all stock transaction history by start date and end date
        /// </summary>
        /// <param name="_startDate"></param>
        /// <param name="_endDate"></param>
        /// <returns></returns>
        public async Task<List<StockTransaction>> GetStockTransactionHistory(DateTime _startDate, DateTime _endDate)
        {
            return await this.stockNoteDBContext.StockTransactions.Where(x => x.TransactionDate >= _startDate && x.TransactionDate <= _endDate)
                    .Include(x => x.FromWarehouse)
                    .Include(x => x.ToWarehouse)
                    .Include(x => x.Item).ToListAsync();
        }

        /// <summary>
        /// Get warehouse specific stock transaction history by start date and end date
        /// </summary>
        /// <param name="_startDate"></param>
        /// <param name="_endDate"></param>
        /// <param name="_warehouseID"></param>
        /// <returns></returns>
        public async Task<List<StockTransaction>> GetStockTransactionHistory(DateTime _startDate, DateTime _endDate, Guid _warehouseID)
        {
            return await this.stockNoteDBContext.StockTransactions.Where(x => x.TransactionDate >= _startDate && x.TransactionDate <= _endDate
            && (x.FromWarehouse.ID == _warehouseID || x.ToWarehouse.ID == _warehouseID))
                    .Include(x => x.FromWarehouse)
                    .Include(x => x.ToWarehouse)
                    .Include(x => x.Item).ToListAsync();
        }
        public void Dispose()
        {
            
        }

    }
}
