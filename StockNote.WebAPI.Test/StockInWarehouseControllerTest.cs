using Azure;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
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
    public class StockInWarehouseControllerTest
    {
        static WebAppTestFactory factory = new WebAppTestFactory();
        static HttpClient client;


        [TestInitialize]
        public async Task TestInitialize()
        {
            client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("api-version", CommonCRUD.testAPIVersion);
            await StockInWarehouseTestCommon.ConstructUnitsItemsWarehouse(client);
        }

        [TestMethod]
        public async Task AddStockToWarehouse()
        {
            // prepare
            StockInWarehouse stockInWarehouse = new StockInWarehouse();
            stockInWarehouse.Item = StockInWarehouseTestCommon.itemA;
            stockInWarehouse.Warehouse = StockInWarehouseTestCommon.warehouseA;
            stockInWarehouse.Balance = 100;
            stockInWarehouse.MinBalThreshold = 10;

            // act
            Guid result = await CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouse);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteStockInWarehouseURL ,result));
        }

        [TestMethod]
        public async Task GetStockInWarehouses()
        {
            // prepare
            await  StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);
            StockInWarehouse itemAWarehouseA = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL,StockInWarehouseTestCommon.itemA.ID ,StockInWarehouseTestCommon.warehouseA.ID));
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.DeleteStockInWarehouseURL , itemAWarehouseA.ID));
            
            // act
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);

            // assert
            Assert.AreEqual(7, stockInWarehouseList.Count); // default test record is 8, but Item-A at Warehouse-A is soft deleted.

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task GetAllStockInWarehouses()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);
            
            // act
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetAllStockInWarehouseURL);

            // assert
            Assert.AreEqual(8, stockInWarehouseList.Count); // default test record is 8.

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task UpdateMinimumThresholdOfStock()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            List<StockInWarehouse> beforeUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();

            StockInWarehouse warehouseBItemA = stockInWarehouseList
                 .Where(x => x.Warehouse.Name.ToLower() == "warehouse-b" && x.Item.Name.ToLower() == "item-a")
                 .FirstOrDefault();
            StockInWarehouse warehouseBItemB = stockInWarehouseList
                .Where(x => x.Warehouse.Name.ToLower() == "warehouse-b" && x.Item.Name.ToLower() == "item-b")
                .FirstOrDefault();

            // act
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.UpdateMinimumThresholdOfStockURL, warehouseBItemA.ItemID , warehouseBItemA.WarehouseID , 1000));
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.UpdateMinimumThresholdOfStockURL , warehouseBItemB.ItemID ,warehouseBItemB.WarehouseID , 1000));
            stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            List<StockInWarehouse> afterUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();


            // assert
            Assert.AreNotEqual(beforeUpdate.Count(), afterUpdate.Count());
            Assert.AreEqual(3, afterUpdate.Count());


            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task UpdateStockInWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client); 
            
            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            List<StockInWarehouse> beforeUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();
            StockInWarehouse warehouseCItemB = stockInWarehouseList
                .Where(x => x.Warehouse.Name.ToLower() == "warehouse-c" && x.Item.Name.ToLower() == "item-b")
                .FirstOrDefault();

            // act
            warehouseCItemB.Balance = 200;
            await CommonCRUD.PutAPIRequest<StockInWarehouse>(client, CommonCRUD.UpdateStockInWarehouseURL, warehouseCItemB);
            stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            List<StockInWarehouse> afterUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();
            warehouseCItemB = stockInWarehouseList
                .Where(x => x.Warehouse.Name.ToLower() == "warehouse-c" && x.Item.Name.ToLower() == "item-b")
                .FirstOrDefault();

            // assert
            Assert.AreEqual(1, beforeUpdate.Count); // item-b in warehouse-c is less than minimum balance threshold
            Assert.AreEqual(0, afterUpdate.Count); // after aupdate item-b balance in warehouse-c, there shall not be item which is lower than minimum balance threshold.
            Assert.AreEqual(200, warehouseCItemB.Balance);

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task GetStockInWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);

            // act
            StockInWarehouse itemAWarehouseA = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID , StockInWarehouseTestCommon.warehouseA.ID));
            StockInWarehouse itemCWarehouseB = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemC.ID , StockInWarehouseTestCommon.warehouseB.ID)); 

            // assert
            Assert.AreEqual(itemAWarehouseA.ItemID, StockInWarehouseTestCommon.itemA.ID);
            Assert.AreEqual(itemAWarehouseA.WarehouseID, StockInWarehouseTestCommon.warehouseA.ID);
            Assert.IsNull(itemCWarehouseB);

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task GetStockItemListByWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);

            // act
            List<Item> warehouseBItemList = await CommonCRUD.GetAPIRequest<List<Item>>(client, string.Format(CommonCRUD.GetStockItemListByWarehouseURL, StockInWarehouseTestCommon.warehouseB.ID));
            List<Item> warehouseCItemList = await CommonCRUD.GetAPIRequest<List<Item>>(client, string.Format(CommonCRUD.GetStockItemListByWarehouseURL , StockInWarehouseTestCommon.warehouseC.ID));

            // assert
            Assert.AreEqual(2, warehouseBItemList.Count);
            Assert.AreEqual(3, warehouseCItemList.Count);
            Assert.IsNull(warehouseBItemList.Where(x => x.Name.ToLower() == "item-c").FirstOrDefault());

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task GetWarehouseListByItem()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);

            // act
            List<Warehouse> itemAWarehouseList = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, string.Format(CommonCRUD.GetWarehouseListByItemURL, StockInWarehouseTestCommon.itemA.ID));
            List<Warehouse> itemCWarehouseList = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, string.Format(CommonCRUD.GetWarehouseListByItemURL,StockInWarehouseTestCommon.itemC.ID));

            // assert
            Assert.AreEqual(3, itemAWarehouseList.Count); // Item-A shall be in 3 warehouses as per test data
            Assert.AreEqual(2, itemCWarehouseList.Count); // Item-B shall be in 2 warehouses only as per test data
            Assert.AreEqual(null, itemCWarehouseList.Where(x => x.Name.ToLower() == "warehouse-b").FirstOrDefault()); // Item-B is not in Warehouse-B.

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        /// <summary>
        /// Soft delete test
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteStockInWarehouses()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);
            StockInWarehouse itemAWarehouseA = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));

            // act
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.DeleteStockInWarehouseURL ,itemAWarehouseA.ID));

            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            StockInWarehouse deletedStockInWarehouse = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));

            // assert
            Assert.AreEqual(7, stockInWarehouseList.Count); // default test record is 8, but Item-A at Warehouse-A is soft deleted.
            Assert.IsFalse(deletedStockInWarehouse.IsActive);

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        /// <summary>
        /// undo delete (undelete) test for soft-deleted record
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UneleteStockInWarehouses()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);
            StockInWarehouse itemAWarehouseA = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.DeleteStockInWarehouseURL, itemAWarehouseA.ID));
            List<StockInWarehouse> beforeUnDelete = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            StockInWarehouse deletedStockInWarehouse = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));

            // act
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.UnDeleteStockInWarehouseURL, itemAWarehouseA.ID));
            
            List<StockInWarehouse> afterUnDelete = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetStockInWarehousesURL);
            StockInWarehouse undeletedStockInWarehouse = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));

            // assert
            Assert.AreEqual(7, beforeUnDelete.Count); // default test record is 8, but Item-A at Warehouse-A is soft deleted.
            Assert.AreEqual(8, afterUnDelete.Count);
            Assert.IsFalse(deletedStockInWarehouse.IsActive);
            Assert.IsTrue(undeletedStockInWarehouse.IsActive);

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestMethod]
        public async Task HeardDeleteStockInWarehouses()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(client);
            StockInWarehouse itemAWarehouseA = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));

            // act
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteStockInWarehouseURL, itemAWarehouseA.ID));

            List<StockInWarehouse> stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(client, CommonCRUD.GetAllStockInWarehouseURL);
            StockInWarehouse deletedStockInWarehouse = await CommonCRUD.GetAPIRequest<StockInWarehouse>(client, string.Format(CommonCRUD.GetStockInWarehouseURL, StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseA.ID));

            // assert
            Assert.AreEqual(7, stockInWarehouseList.Count); // default test record is 8, but Item-A at Warehouse-A is permanentely deleted.
            Assert.IsNull(deletedStockInWarehouse);

            // teardown
            await CommonCRUD.DeleteAllStockInWarehouse(client);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            client.Dispose();
        }

    }
}