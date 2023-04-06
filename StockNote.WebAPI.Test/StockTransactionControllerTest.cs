using Azure;
using Newtonsoft.Json;
using StockNote.BusinessLayer;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StockNote.WebAPI.Test
{    
    [TestClass]
    public class StockTransactionControllerTest
    {
        static WebAppTestFactory factory = new WebAppTestFactory();
        static HttpClient client;


        [TestInitialize]
        public async Task TestInitialize()
        {
            client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("api-version", CommonCRUD.testAPIVersion);
            await StockTransactionTestCommon.Construct3WarehousesWithStocks(client);
        }

        /// <summary>
        /// Test get stock transaction by ID
        /// Check child objects are included in getTransactionByID method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetTransactionByID()
        {
            // Prepare the transaction for 95 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction = new StockTransaction();
            stockTransaction.Item = StockTransactionTestCommon.itemA;
            stockTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction.ToWarehouse = StockTransactionTestCommon.warehouseB;
            stockTransaction.Quantity = 95;
            stockTransaction.TransactionDate = DateTime.Now;
            stockTransaction.Remark = "Test stock out 95 boxes item-A from warehouse-A to warehouse-B";
            Guid transactionID = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateStockTransactionURL, stockTransaction);
            
            // ေေ်ေေေact
            StockTransaction transaction = await CommonCRUD.GetAPIRequest<StockTransaction>(client, string.Format(CommonCRUD.GetTransactionByIDURL, transactionID));

            // assert
            Assert.IsNotNull(transaction);
            Assert.AreEqual(stockTransaction.Item.ID, transaction.Item.ID); // test child object is included
            Assert.AreEqual(stockTransaction.Item.Unit.ID, transaction.Item.Unit.ID); // test child object is included
            Assert.AreEqual(stockTransaction.FromWarehouse.ID, transaction.FromWarehouse.ID); // test child object is included
            Assert.AreEqual(stockTransaction.ToWarehouse.ID, transaction.ToWarehouse.ID); // test child object is included

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(client);
        }

        /// <summary>
        /// Test create stock transaction 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateSaleTransaction()
        {
            // arrange
            // Already prepare 3 warehouses with 3 items in test initialization state
            // Prepare the sale transaction for 95 boxes item-A from warehouse-A
            StockTransaction stockTransaction = new StockTransaction();
            stockTransaction.Item = StockTransactionTestCommon.itemA;
            stockTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction.Quantity = 95;
            stockTransaction.TransactionDate = DateTime.Now;
            stockTransaction.Remark = "Test sale out 95 boxes item-A from warehouse-A";

            // act
            Guid transactionID = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateSaleTransactionURL, stockTransaction);

            // assert
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            StockInWarehouse warehouseAItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseA.ID
            && x.ItemID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            List<StockTransaction> stockTransactionList = await CommonCRUD.GetAPIRequest<List<StockTransaction>>(client, string.Format(CommonCRUD.GetTransactionHistoryURL , DateTime.Now.AddDays(-1) , DateTime.Now.AddDays(1)));
            stockTransaction = stockTransactionList.Where(x => x.ID == transactionID).FirstOrDefault();

            Assert.AreEqual(5, warehouseAItemA.Balance);
            Assert.IsTrue(warehouseAItemA.IsLowerThanMinBal);
            Assert.AreNotEqual(Guid.Empty, transactionID);
            Assert.IsTrue(stockTransaction.Remark.StartsWith("Test sale out 95 boxes"));

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(client);
        }

        /// <summary>
        /// Test a stock transaction creation
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateStockTransaction()
        {
            // rpepare
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
            Guid transactionID = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateStockTransactionURL, stockTransaction);

            // assert
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            StockInWarehouse warehouseAItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseA.ID
            && x.ItemID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            StockInWarehouse warehouseBItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseB.ID
            && x.ItemID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            
            List<StockTransaction> stockTransactionList = await CommonCRUD.GetAPIRequest<List<StockTransaction>>(client, string.Format(CommonCRUD.GetTransactionHistoryURL , DateTime.Now.AddDays(-1) , DateTime.Now.AddDays(1)));
            stockTransaction = stockTransactionList.Where(x => x.ID == transactionID).FirstOrDefault();

            Assert.AreEqual(5, warehouseAItemA.Balance);
            Assert.AreEqual(195, warehouseBItemA.Balance);
            Assert.IsTrue(warehouseAItemA.IsLowerThanMinBal);
            Assert.AreNotEqual(Guid.Empty, transactionID);
            Assert.IsTrue(stockTransaction.Remark.StartsWith("Test stock out 95 boxes"));

            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(client);
        }

        /// <summary>
        /// Test transaction amendment for cancel transaction and create a new transaction
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AmendmentStockTransaction()
        {
            // Prepare the transaction for 95 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction = new StockTransaction();
            stockTransaction.Item = StockTransactionTestCommon.itemA;
            stockTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction.ToWarehouse = StockTransactionTestCommon.warehouseB;
            stockTransaction.Quantity = 95;
            stockTransaction.TransactionDate = DateTime.Now;
            stockTransaction.Remark = "Test stock out 95 boxes item-A from warehouse-A to warehouse-B";
            Guid transactionID = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateStockTransactionURL, stockTransaction);
            StockTransaction oldTransaction = await CommonCRUD.GetAPIRequest<StockTransaction>(client, string.Format(CommonCRUD.GetTransactionByIDURL, transactionID));


            // prepare new transaction
            StockTransaction newTransaction = new StockTransaction();
            newTransaction.Item = StockTransactionTestCommon.itemB;
            newTransaction.FromWarehouse = StockTransactionTestCommon.warehouseB;
            newTransaction.ToWarehouse = StockTransactionTestCommon.warehouseC;
            newTransaction.Quantity = 30;
            newTransaction.TransactionDate = DateTime.Now;
            newTransaction.Remark = "Amend transaction with different warehouses and different quantity.";

            // prepare amend transaction
            AmendmentTransaction amendmentTransaction = new AmendmentTransaction();
            amendmentTransaction.OldStockTransaction = oldTransaction;
            amendmentTransaction.NewStockTransaction= newTransaction;

            // act
            await CommonCRUD.PutAPIRequest(client, CommonCRUD.CreateAmendmentTransactionURL, amendmentTransaction);

            // assert
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            StockInWarehouse warehouseAItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseA.ID
            && x.Item.ID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            StockInWarehouse warehouseBItemA = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseB.ID
            && x.Item.ID == StockTransactionTestCommon.itemA.ID).FirstOrDefault();
            StockInWarehouse warehouseBItemB = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseB.ID
            && x.Item.ID == StockTransactionTestCommon.itemB.ID).FirstOrDefault();
            StockInWarehouse warehouseCItemB = stockInWarehouseList.Where(x => x.Warehouse.ID == StockTransactionTestCommon.warehouseC.ID
            && x.Item.ID == StockTransactionTestCommon.itemB.ID).FirstOrDefault();


            Assert.AreEqual(100, warehouseAItemA.Balance);
            Assert.AreEqual(100, warehouseBItemA.Balance);
            Assert.AreEqual(70, warehouseBItemB.Balance);
            Assert.AreEqual(130, warehouseCItemB.Balance);
            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(client);

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
            //Prepare the transaction for 20 boxes item-A from warehouse-A to warehouse-B
            StockTransaction stockTransaction1 = new StockTransaction();
            stockTransaction1.Item = StockTransactionTestCommon.itemA;
            stockTransaction1.FromWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction1.ToWarehouse = StockTransactionTestCommon.warehouseB;
            stockTransaction1.Quantity = 20;
            stockTransaction1.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            stockTransaction1.Remark = "Test stock out 20 boxes item-A from warehouse-A to warehouse-B";

            Guid transactionID1 = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateStockTransactionURL, stockTransaction1);
            stockTransaction1 = await CommonCRUD.GetAPIRequest<StockTransaction>(client, string.Format(CommonCRUD.GetTransactionByIDURL, transactionID1));
            #endregion

            #region Create stock-out-transaction, Item-B from Warehouse-C to Warehouse-A
            //Prepare the transaction for 20 boxes item-A from warehouse-C to warehouse-A
            StockTransaction stockTransaction2 = new StockTransaction();
            stockTransaction2.Item = StockTransactionTestCommon.itemA;
            stockTransaction2.FromWarehouse = StockTransactionTestCommon.warehouseC;
            stockTransaction2.ToWarehouse = StockTransactionTestCommon.warehouseA;
            stockTransaction2.Quantity = 20;
            stockTransaction2.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            stockTransaction2.Remark = "Test stock in 20 boxes item-A from warehouse-C to warehouse-A";

            Guid transactionID2 = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateStockTransactionURL, stockTransaction2);
            stockTransaction2 = await CommonCRUD.GetAPIRequest<StockTransaction>(client, string.Format(CommonCRUD.GetTransactionByIDURL, transactionID2));
            #endregion

            #region Create sale transaction, Sale Item-A from Warehouse-A
            // Prepare the sale transaction for 35 boxes item-A from warehouse-A
            StockTransaction saleTransaction = new StockTransaction();
            saleTransaction.Item = StockTransactionTestCommon.itemB;
            saleTransaction.FromWarehouse = StockTransactionTestCommon.warehouseA;
            saleTransaction.Quantity = 35;
            saleTransaction.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            saleTransaction.Remark = "Test sale out 35 boxes item-A from warehouse-A";
            
            Guid saleTransactionID = await CommonCRUD.PostAPIRequest<Guid, StockTransaction>(client, CommonCRUD.CreateStockTransactionURL, saleTransaction);
            saleTransaction = await CommonCRUD.GetAPIRequest<StockTransaction>(client, string.Format(CommonCRUD.GetTransactionByIDURL, saleTransactionID));
            #endregion

            #region Amend Transaction-1, 
            StockTransaction newTransaction = new StockTransaction();
            newTransaction.Item = StockTransactionTestCommon.itemA;
            newTransaction.FromWarehouse = StockTransactionTestCommon.warehouseB;
            newTransaction.ToWarehouse = StockTransactionTestCommon.warehouseC;
            newTransaction.Quantity = 30;
            newTransaction.TransactionDate = DateTime.Now.AddDays(-5); // to avoid same date transaction from other test
            newTransaction.Remark = "Amend transaction with different warehouses and different quantity.";

            AmendmentTransaction amendmentTransaction= new AmendmentTransaction();
            amendmentTransaction.OldStockTransaction = saleTransaction;
            amendmentTransaction.NewStockTransaction= newTransaction;
            await CommonCRUD.PutAPIRequest<AmendmentTransaction>(client, CommonCRUD.CreateAmendmentTransactionURL, amendmentTransaction);
            #endregion

            // act
            List<StockTransaction> stockTransactionListByDate = await CommonCRUD.GetAPIRequest<List<StockTransaction>>(client, string.Format(CommonCRUD.GetTransactionHistoryURL , DateTime.Now.AddDays(-6) , DateTime.Now.AddDays(-1)));
            List<StockTransaction> stockTransactionListByWarehouse = await CommonCRUD.GetAPIRequest<List<StockTransaction>>(client, string.Format(CommonCRUD.GetTransactionHistoryByWarehouseURL , DateTime.Now.AddDays(-6) , DateTime.Now.AddDays(-1) , StockTransactionTestCommon.warehouseA.ID));
 
            // assert
            Assert.AreEqual(5, stockTransactionListByDate.Count);
            Assert.AreEqual(4, stockTransactionListByWarehouse.Count);


            // teardown
            await StockTransactionTestCommon.ResetToDefaultStocksBalanceInWarehouses(client);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            client.Dispose();
        }

    }
}