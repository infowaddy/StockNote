using StockNote.Models;

namespace StockNote.BusinessLayer.Interfaces
{
    public interface IStockTransactionManager
    {   
        /// <summary>
        /// Generic stock transaction method which will insert to 
        /// stock transaction table and update to stock-in-warehouse table
        /// </summary>
        /// <param name="_stockTransaction"></param>
        /// <returns></returns>
        Task<Guid> CreateStockTransaction(StockTransaction _stockTransaction);
        
        /// <summary>
        /// Get StockTransaction by ID
        /// </summary>
        /// <param name="_transactionID"></param>
        /// <returns></returns>
        Task<StockTransaction> GetStockTransaction(Guid _transactionID, bool _includeDelete = false);
        
        /// <summary>
        /// Make stock out transaction for sale
        /// This method will call CreateStockTransaction method
        /// and accordinately will update to StockInWarehouse table
        /// </summary>
        /// <param name="stockTransaction"></param>
        /// <returns></returns>
        Task<Guid> CreateSaleTransaction(StockTransaction stockTransaction);
        
        /// <summary>
        /// Cancel stock transaction which means transaction process will do reverse way with remark.
        /// There is no delete method in transaction.
        /// </summary>
        /// <param name="_stockTransaction"></param>
        /// <returns></returns>
        Task<Guid> CancelStockTransaction(StockTransaction _stockTransaction);
        
        /// <summary>
        /// To amend the transaction,
        /// it need to cancel old transaction and create a new transaction.
        /// There is no delete method in transaction.
        /// </summary>
        /// <param name="_oldStockTransaction"></param>
        /// <param name="_newStockTransaction"></param>
        /// <returns></returns>
        Task<Guid> AmendmentTransaction(StockTransaction _oldStockTransaction, StockTransaction _newStockTransaction);
        
        /// <summary>
        /// Get all stock transaction history by start date and end date
        /// </summary>
        /// <param name="_startDate"></param>
        /// <param name="_endDate"></param>
        /// <returns></returns>
        Task<List<StockTransaction>> GetStockTransactionHistory(DateTime _startDate, DateTime _endDate);
        
        /// <summary>
        /// Get warehouse specific stock transaction history by start date and end date
        /// </summary>
        /// <param name="_startDate"></param>
        /// <param name="_endDate"></param>
        /// <param name="_warehouse"></param>
        /// <returns></returns>
        Task<List<StockTransaction>> GetStockTransactionHistory(DateTime _startDate, DateTime _endDate, Guid _warehouseID);
    }
}
