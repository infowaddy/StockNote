namespace StockNote.BusinessLayer.Test
{
    [TestClass]
    public class StockInWarehousenManagerTest
    {
        private static StockInWarehouseManager stockInWarehouseManager;
        private static UnitManager unitManager;
        private static ItemManager itemManager;
        private static WarehouseManager warehouseManager;

        private static StockNoteDBContext stockNoteDBContext = DatabaseInMemory.StockNoteDBContext;
        
        /// <summary>
        /// Test initialization will call from all test methods.
        /// Instantiate necessary business manager class with dbContext injection
        /// Construct test data
        /// </summary>
        /// <returns></returns>
        [TestInitialize]
        public async Task TestInitialize()
        {
            stockInWarehouseManager = new StockInWarehouseManager(stockNoteDBContext, Common.GenerateLogger<StockInWarehouseManager>());
            unitManager = new UnitManager(stockNoteDBContext, Common.GenerateLogger<UnitManager>());
            itemManager= new ItemManager(stockNoteDBContext, Common.GenerateLogger<ItemManager>());
            warehouseManager =  new WarehouseManager(stockNoteDBContext, Common.GenerateLogger<WarehouseManager>());
            await StockInWarehouseTestCommon.ConstructUnitsItemsWarehouse(unitManager, warehouseManager, itemManager);
        }

        /// <summary>
        /// Test AddStockToWarehouse
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddStockToWarehouse()
        {
            // prepare
            Warehouse warehouseA = warehouseManager.GetWarehouses("Warehouse-A").Result.FirstOrDefault();
            Item itemA = await itemManager.GetItem("123456");
            StockInWarehouse stockInWarehouse = new StockInWarehouse();
            stockInWarehouse.Item = itemA;
            stockInWarehouse.Warehouse = warehouseA;
            stockInWarehouse.Balance = 100;
            stockInWarehouse.MinBalThreshold = 10;          

            // act
            Guid result = await stockInWarehouseManager.AddStockToWarehouse(stockInWarehouse);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await stockInWarehouseManager.DeleteStockInWarehouse(result);
        }

        /// <summary>
        /// Retrieving all stock-in-warehouse records test
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStockInWarehouseList()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);

            // act
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();

            // assert
            Assert.AreEqual(8, stockInWarehouseList.Count); // default test record is 8.

            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Update to stock balance and verify with IsLowerThanMinBal property
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateStockInWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            List<StockInWarehouse> beforeUpdate = stockInWarehouseList.Where(x=>x.IsLowerThanMinBal).ToList();
            StockInWarehouse warehouseCItemB = stockInWarehouseList
                .Where(x => x.Warehouse.Name.ToLower() == "warehouse-c" && x.Item.Name.ToLower()=="item-b")
                .FirstOrDefault();

            // act
            warehouseCItemB.Balance = 200;
            await stockInWarehouseManager.UpdateStockInWarehouse(warehouseCItemB);
            stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            List<StockInWarehouse> afterUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();
            warehouseCItemB = stockInWarehouseList
                .Where(x => x.Warehouse.Name.ToLower() == "warehouse-c" && x.Item.Name.ToLower() == "item-b")
                .FirstOrDefault();
            // assert
            Assert.AreEqual(1, beforeUpdate.Count); // item-b in warehouse-c is less than minimum balance threshold
            Assert.AreEqual(0, afterUpdate.Count); // after aupdate item-b balance in warehouse-c, there shall not be item which is lower than minimum balance threshold.
            Assert.AreEqual(200, warehouseCItemB.Balance);

            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Updat to MinBalThreshold attribute and verify with IsLowerThanMinBal attribute
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateMinimumThresholdOfStock()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            List<StockInWarehouse> beforeUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();

            StockInWarehouse warehouseBItemA = stockInWarehouseList
                 .Where(x => x.Warehouse.Name.ToLower() == "warehouse-b" && x.Item.Name.ToLower() == "item-a")
                 .FirstOrDefault();
            StockInWarehouse warehouseBItemB = stockInWarehouseList
                .Where(x => x.Warehouse.Name.ToLower() == "warehouse-b" && x.Item.Name.ToLower() == "item-b")
                .FirstOrDefault();

            List<StockInWarehouse> yyy = await stockInWarehouseManager.GetStockInWarehouses();
            // act
            await stockInWarehouseManager.UpdateMinimumThresholdOfStock(warehouseBItemA.ItemID, warehouseBItemA.WarehouseID, 1000);
            await stockInWarehouseManager.UpdateMinimumThresholdOfStock(warehouseBItemB.ItemID, warehouseBItemB.WarehouseID, 1000);
            stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            List<StockInWarehouse> afterUpdate = stockInWarehouseList.Where(x => x.IsLowerThanMinBal).ToList();


            // assert
            Assert.AreNotEqual(beforeUpdate.Count(), afterUpdate.Count());
            Assert.AreEqual(3, afterUpdate.Count());


            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }

        [TestMethod]
        public async Task GetStockInWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);

            // act
            StockInWarehouse itemAWarehouseA = await stockInWarehouseManager.GetStockInWarehouse(StockInWarehouseTestCommon.itemA.ID,
                StockInWarehouseTestCommon.warehouseA.ID);
            StockInWarehouse itemCWarehouseB = await stockInWarehouseManager.GetStockInWarehouse(StockInWarehouseTestCommon.itemC.ID,
                StockInWarehouseTestCommon.warehouseB.ID);

            // assert
            Assert.AreEqual(itemAWarehouseA.ItemID, StockInWarehouseTestCommon.itemA.ID);
            Assert.AreEqual(itemAWarehouseA.WarehouseID, StockInWarehouseTestCommon.warehouseA.ID);
            Assert.IsNull(itemCWarehouseB);

            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Test GetStockItemListByWarehouse
        /// By the default test data, there are 2 items in Warehouse-B
        /// and there are 3 items in Warehouse-C
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStockItemListByWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);
            Warehouse warehouseB = warehouseManager.GetWarehouses("Warehouse-B").Result.FirstOrDefault();
            Warehouse warehouseC = warehouseManager.GetWarehouses("Warehouse-C").Result.FirstOrDefault();
            
            // act
            List<Item> warehouseBItemList = await stockInWarehouseManager.GetStockItemListByWarehouse(warehouseB.ID);
            List<Item> warehouseCItemList = await stockInWarehouseManager.GetStockItemListByWarehouse(warehouseC.ID);

            // assert
            Assert.AreEqual(2, warehouseBItemList.Count);
            Assert.AreEqual(3, warehouseCItemList.Count);
            Assert.IsNull(warehouseBItemList.Where(x => x.Name.ToLower() == "item-c").FirstOrDefault());
            
            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }

        /// <summary>
        /// Test GetWarehouseListByItem
        /// By the default test data, Item-A should be in 3 warehouses
        /// and Item-C should be in 2 warehouses. Warehouse-B does not have Item-C
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetWarehouseListByItem()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);
            Item itemA = await itemManager.GetItem("123456"); // get Item-A by barcode
            Item itemC = await itemManager.GetItem("444221"); // get Item-C by barcode

            // act
            List<Warehouse> itemAWarehouseList = await stockInWarehouseManager.GetWarehouseListByItem(itemA.ID);
            List<Warehouse> itemCWarehouseList = await stockInWarehouseManager.GetWarehouseListByItem(itemC.ID);

            // assert
            Assert.AreEqual(3, itemAWarehouseList.Count); // Item-A shall be in 3 warehouses as per test data
            Assert.AreEqual(2, itemCWarehouseList.Count); // Item-B shall be in 2 warehouses only as per test data
            Assert.AreEqual(null, itemCWarehouseList.Where(x => x.Name.ToLower() == "warehouse-b").FirstOrDefault()); // Item-B is not in Warehouse-B.

            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }
        /// <summary>
        /// Test Delete all records from StockInWarehouse table
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteStockInWarehouse()
        {
            // prepare
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);
            List<StockInWarehouse> beforeDelete = await stockInWarehouseManager.GetStockInWarehouses();

            // act
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
            List<StockInWarehouse> afterDelete = await stockInWarehouseManager.GetStockInWarehouses();


            // assert
            Assert.IsTrue(beforeDelete.Count > 0 );
            Assert.IsTrue(afterDelete.Count == 0);

            // teardown
            // nothing to do.
        }

        /// <summary>
        /// Test soft delete function
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ChangeActiveStatusOfStockInWarehouse()
        {
            // prepare            
            await StockInWarehouseTestCommon.AddTestStocksToWarehouses(stockInWarehouseManager, warehouseManager, itemManager);
            List<StockInWarehouse> beforeChange = await stockInWarehouseManager.GetStockInWarehouses();
            StockInWarehouse warehouseBItemA = await stockInWarehouseManager.GetStockInWarehouse(StockInWarehouseTestCommon.itemA.ID, StockInWarehouseTestCommon.warehouseB.ID);
            StockInWarehouse warehouseBItemB = await stockInWarehouseManager.GetStockInWarehouse(StockInWarehouseTestCommon.itemB.ID, StockInWarehouseTestCommon.warehouseB.ID);

            // act
            warehouseBItemA.IsActive = false;
            warehouseBItemB.IsActive = false;
            await stockInWarehouseManager.ChangeActiveStatusOfStockInWarehouse(warehouseBItemA.ID, false);
            await stockInWarehouseManager.ChangeActiveStatusOfStockInWarehouse(warehouseBItemB.ID, false);
            List<StockInWarehouse> afterChange = await stockInWarehouseManager.GetStockInWarehouses();

            // assert
            Assert.AreEqual(beforeChange.Where(x=> x.IsActive == true).Count(), afterChange.Where(x=>x.IsActive == true).Count());
            Assert.AreEqual(0, afterChange.Where(x => x.IsActive == true && x.Warehouse.Name.ToLower() == "warehouse-b").Count());

            // teardown
            await StockInWarehouseTestCommon.DeleteAllStocksInWarehouses(stockInWarehouseManager);
        }
        
        /// <summary>
        /// all manager classes were disposed.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            stockInWarehouseManager.Dispose();
            unitManager.Dispose();
            itemManager.Dispose();
            stockInWarehouseManager.Dispose();
        }
    }
}