using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockNote.BusinessLayer.Interfaces;
using StockNote.BusinessLayer.Test;
using StockNote.Models;

namespace StockNote.BusinessLayer.Test
{
    [TestClass]
    public class StockTransactionManagerTest
    {
        private static StockTransactionManager stockTransactionManager;
        private static UnitManager unitManager;
        private static ItemManager  itemManager;
        private static WarehouseManager warehouseManager;
        private static StockInWarehouseManager stockInWarehouseManager;
        private static StockNoteDBContext stockNoteDBContext = DatabaseInMemory.StockNoteDBContext;
        [TestInitialize]
        public async Task TestInitialize()
        {
            stockInWarehouseManager = new StockInWarehouseManager(stockNoteDBContext, Common.GenerateLogger<StockInWarehouseManager>());
            unitManager = new UnitManager(stockNoteDBContext, Common.GenerateLogger<UnitManager>());
            itemManager = new ItemManager(stockNoteDBContext, Common.GenerateLogger<ItemManager>());
            warehouseManager   = new WarehouseManager(stockNoteDBContext, Common.GenerateLogger<WarehouseManager>());
            stockTransactionManager = new StockTransactionManager(stockNoteDBContext,  Common.GenerateLogger<StockTransactionManager>());
            await StockTransactionTestCommon.Construct3WarehousesWithStocks(unitManager, warehouseManager,
                itemManager,stockInWarehouseManager);
        }

        /// <summary>
        /// Test create stock transaction 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateStockTransaction()
        {
            // prepare
            // Already prepare 3 warehouses with 3 items in test initialization state
            // Prepare the transaction for 95 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction = new StockTransaction();
            stockTransaction.Item = StockTransactionTestCommon.itemA;
            stockTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction.ToWarehouse = StockTransactionTestCommon.warehouseB;
            stockTransaction.Quantity = 95;
            stockTransaction.TransactionDate = DateTime.Now;
            stockTransaction.Remark = "Test stock out 95 boxes item-A from warehouse-A to warehouse-B";

            // act
            Guid transactionID = await stockTransactionManager.CreateStockTransaction(stockTransaction);

            // assert
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            StockInWarehouse warehouseAItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseA.ID
            && x.ItemID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            StockInWarehouse warehouseBItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseB.ID
            && x.ItemID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            
            List<StockTransaction> stockTransactionList = await stockTransactionManager.GetStockTransactionHistory(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            stockTransaction = stockTransactionList.Where(x=>x.ID == transactionID).FirstOrDefault();
            
            Assert.AreEqual(5, warehouseAItemA.Balance);
            Assert.AreEqual(195, warehouseBItemA.Balance);
            Assert.IsTrue(warehouseAItemA.IsLowerThanMinBal);
            Assert.AreNotEqual(Guid.Empty, transactionID);
            Assert.IsTrue(stockTransaction.Remark.StartsWith("Test stock out 95 boxes"));

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Test a sale transaction creation
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateSaleTransaction()
        {
            // prepare
            // Already prepare 3 warehouses with 3 items in test initialization state
            // Prepare the sale transaction for 95 boxes item-A from warehouse-A
            StockTransaction stockTransaction = new StockTransaction();
            stockTransaction.Item = StockTransactionTestCommon.itemA;
            stockTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction.Quantity = 95;
            stockTransaction.TransactionDate = DateTime.Now;
            stockTransaction.Remark = "Test sale out 95 boxes item-A from warehouse-A";

            // act
            Guid transactionID = await stockTransactionManager.CreateSaleTransaction(stockTransaction);

            // assert
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            StockInWarehouse warehouseAItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseA.ID
            && x.ItemID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            List<StockTransaction> stockTransactionList = await stockTransactionManager.GetStockTransactionHistory(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            stockTransaction = stockTransactionList.Where(x => x.ID == transactionID).FirstOrDefault();

            Assert.AreEqual(5, warehouseAItemA.Balance);
            Assert.IsTrue(warehouseAItemA.IsLowerThanMinBal);
            Assert.AreNotEqual(Guid.Empty, transactionID);
            Assert.IsTrue(stockTransaction.Remark.StartsWith("Test sale out 95 boxes"));
            Assert.IsTrue(stockTransaction.Remark.Contains("Sale transaction"));

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Test transaction amendment for cancel transaction and create a new transaction
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AmendmentStockTransaction()
        {
            // prepare
            // Prepare the transaction for 95 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction = new StockTransaction();
            stockTransaction.Item = StockTransactionTestCommon.itemA;
            stockTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction.ToWarehouse = StockTransactionTestCommon.warehouseB;
            stockTransaction.Quantity = 95;
            stockTransaction.TransactionDate = DateTime.Now;
            stockTransaction.Remark = "Test stock out 95 boxes item-A from warehouse-A to warehouse-B";
            Guid transactionID = await stockTransactionManager.CreateStockTransaction(stockTransaction);
            StockTransaction oldTransaction = await stockTransactionManager.GetStockTransaction(transactionID);

            // act
            StockTransaction newTransaction = new StockTransaction();
            newTransaction.Item = StockTransactionTestCommon.itemB;
            newTransaction.FromWarehouse = StockTransactionTestCommon.warehouseB;
            newTransaction.ToWarehouse = StockTransactionTestCommon.warehouseC;
            newTransaction.Quantity = 30;
            newTransaction.TransactionDate = DateTime.Now;
            newTransaction.Remark = "Amend transaction with different warehouses and different quantity.";
            await stockTransactionManager.AmendmentTransaction(oldTransaction, newTransaction);

            // assert
            List<StockInWarehouse> warehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            StockInWarehouse warehouseAItemA = warehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseA.ID
            && x.Item.ID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            StockInWarehouse warehouseBItemA = warehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseB.ID
            && x.Item.ID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            StockInWarehouse warehouseBItemB = warehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseB.ID
            && x.Item.ID == StockTransactionTestCommon.itemB.ID).FirstOrDefault();
            StockInWarehouse warehouseCItemB = warehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseC.ID
            && x.Item.ID == StockTransactionTestCommon.itemB.ID).FirstOrDefault();


            Assert.AreEqual(100, warehouseAItemA.Balance);
            Assert.AreEqual(100, warehouseBItemA.Balance);
            Assert.AreEqual(70, warehouseBItemB.Balance);
            Assert.AreEqual(130, warehouseCItemB.Balance);

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Test get transaction history by date and by warehouse
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetTransactionHistoryByWarehouse()
        {
            // prepare
            #region Create stock-out-transaction, Item-A from Warehouse-A to Warehouse-B
            //Prepare the transaction for 95 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction1 = new StockTransaction();
            stockTransaction1.Item = StockTransactionTestCommon.itemA;
            stockTransaction1.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction1.ToWarehouse = StockTransactionTestCommon.warehouseB;
            stockTransaction1.Quantity = 20;
            stockTransaction1.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            stockTransaction1.Remark = "Test stock out 95 boxes item-A from warehouse-A to warehouse-B";

            Guid transactionID1 = await stockTransactionManager.CreateStockTransaction(stockTransaction1);
            stockTransaction1 =await stockTransactionManager.GetStockTransaction(transactionID1);
            #endregion

            #region Create stock-out-transaction, Item-B from Warehouse-C to Warehouse-A
            //Prepare the transaction for 95 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction2 = new StockTransaction();
            stockTransaction2.Item = StockTransactionTestCommon.itemA;
            stockTransaction2.FromWarehouse = StockTransactionTestCommon.warehouseC;
            stockTransaction2.ToWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction2.Quantity = 20;
            stockTransaction2.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            stockTransaction2.Remark = "Test stock in 20 boxes item-A from warehouse-C to warehouse-A";

            Guid transactionID2 = await stockTransactionManager.CreateStockTransaction(stockTransaction2);
            stockTransaction2 = await stockTransactionManager.GetStockTransaction(transactionID2);
            #endregion

            #region Create sale transaction, Sale Item-A from Warehouse-A
            // Prepare the sale transaction for 95 boxes item-A from warehouse-A
            StockTransaction saleTransaction = new StockTransaction();
            saleTransaction.Item = StockTransactionTestCommon.itemB;
            saleTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            saleTransaction.Quantity = 35;
            saleTransaction.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            saleTransaction.Remark = "Test sale out 95 boxes item-A from warehouse-A";

            Guid saleTransactionID = await stockTransactionManager.CreateSaleTransaction(saleTransaction);
            saleTransaction = await stockTransactionManager.GetStockTransaction(saleTransactionID);
            #endregion

            #region Amend Transaction-1, 
            StockTransaction amendTransaction = new StockTransaction();
            amendTransaction.Item = StockTransactionTestCommon.itemA;
            amendTransaction.FromWarehouse = StockTransactionTestCommon.warehouseB;
            amendTransaction.ToWarehouse = StockTransactionTestCommon.warehouseC;
            amendTransaction.Quantity = 30;
            amendTransaction.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            amendTransaction.Remark = "Amend transaction with different warehouses and different quantity.";
            Guid amendTransactionID = await stockTransactionManager.AmendmentTransaction(saleTransaction, amendTransaction);
            amendTransaction = await stockTransactionManager.GetStockTransaction(amendTransactionID);
            #endregion

            // act
            List<StockTransaction> stockTransactionListByDate = await stockTransactionManager.GetStockTransactionHistory(DateTime.Now.AddDays(-6), DateTime.Now.AddDays(-1));
            List<StockTransaction> stockTransactionListByWarehouse = await stockTransactionManager.GetStockTransactionHistory(DateTime.Now.AddDays(-6), DateTime.Now.AddDays(-1), StockTransactionTestCommon.warehouseA.ID);

            // assert
            Assert.AreEqual(5, stockTransactionListByDate.Count);
            Assert.AreEqual(4, stockTransactionListByWarehouse.Count);

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// All manager classes were disposed
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            stockTransactionManager.Dispose();
            unitManager.Dispose();
            itemManager.Dispose();
            warehouseManager.Dispose();
            stockInWarehouseManager.Dispose();
        }
    }
}