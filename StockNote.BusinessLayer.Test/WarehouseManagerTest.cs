namespace StockNote.BusinessLayer.Test
{
    [TestClass]
    public class WarehouseManagerTest
    {
        private static WarehouseManager warehouseManager;
        private static StockNoteDBContext stockNoteDBContext = DatabaseInMemory.StockNoteDBContext;
        
        [TestInitialize]
        public void TestInitialize()
        {
            warehouseManager = new WarehouseManager(stockNoteDBContext, Common.GenerateLogger<WarehouseManager>());
        }

        /// <summary>
        /// Creating a warehouse test. 
        /// If success, newly inserted warehouse ID will be return with Guid format
        /// </summary>
        [TestMethod]
        public async Task CreateWarehouse()
        {
            // prepare
            Warehouse warehouse = new Warehouse();
            warehouse.Name = "A-Warehouse-1";
            warehouse.IsActive = true;

            // act
            Guid result = await warehouseManager.CreateWarehouse(warehouse);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await warehouseManager.DeleteWarehouse(warehouse.ID);
        }

        /// <summary>
        /// Retrieving active warehouses test.
        /// If success, active 2 warehouses will be return.
        /// </summary>
        [TestMethod]
        public async Task GetActiveWarehousesOnly()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);

            // act
            List<Warehouse> result1 = await warehouseManager.GetWarehouses();

            // assert
            Assert.AreEqual(2, result1.Count);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Retrieving all warehoues test.
        /// If success, 3 warehouses which including deleted also will be return.
        /// </summary>
        [TestMethod]
        public async Task GetAllWarehousesIncludeDeleted()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);
            
            // act
            List<Warehouse> result1 = await warehouseManager.GetWarehouses(true);

            // assert
            Assert.AreEqual(3, result1.Count);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Retrieving active warehouses test.
        /// If success, 2 warehouses will be return. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetWarehouses()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);

            // act
            List<Warehouse> result1 = await warehouseManager.GetWarehouses();

            // assert
            Assert.AreEqual(2, result1.Count);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Getting warehouses filter by name test.
        /// If success, active 3 warehouses will be return and order by name.
        /// </summary>
        [TestMethod]
        public async Task GetWarehousesByName()
        {
            // prepare
            await WarehouseTestCommon.Insert6Warehouses(warehouseManager);

            // act
            List<Warehouse> result = await warehouseManager.GetWarehouses("A");

            // assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("A-Warehouse-4", result[2].Name); // sorting test

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Getting a warehouse by ID.
        /// If success, one warehouse will be return.
        /// </summary>
        [TestMethod]
        public async Task GetWarehouseByID()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);
            List<Warehouse> warehouses = await WarehouseTestCommon.GetAllWarehouses(warehouseManager);
            Warehouse unactiveWarehouse = warehouses[2]; // get default unactive warehouse from test data.

            // act
            Warehouse result = await warehouseManager.GetWarehouse(unactiveWarehouse.ID, true);
            // assert
            Assert.AreEqual(unactiveWarehouse, result);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }
        public async Task GetActiveWarehouseByID()
        {
            // prepare
            await WarehouseTestCommon.Insert6Warehouses(warehouseManager);
            List<Warehouse> warehouses = await WarehouseTestCommon.GetAllWarehouses(warehouseManager);
            Warehouse activeWarehouse = warehouses[0];
            Warehouse unactiveWarehouse = warehouses[1];

            // act
            Warehouse result1 = await warehouseManager.GetWarehouse(activeWarehouse.ID);
            Warehouse result2 = await warehouseManager.GetWarehouse(unactiveWarehouse.ID);
            // assert
            Assert.AreEqual(activeWarehouse, result1);
            Assert.IsNull(result2);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Updating an existing warehouse test.
        /// If success, updated warehouse will be return with new updated value.
        /// </summary>
        [TestMethod]
        public async Task UpdateWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert6Warehouses(warehouseManager);
            List<Warehouse> warehouses = await WarehouseTestCommon.GetAllWarehouses(warehouseManager);
            Warehouse warehouse3 = warehouses[3];
            Warehouse oldWarehouse = await warehouseManager.GetWarehouse(warehouse3.ID);

            // act
            oldWarehouse.Name = "new-name";
            oldWarehouse.IsActive = false;
            bool result = await warehouseManager.UpdateWarehouse(oldWarehouse);

            // assert
            Warehouse newWarehouse = await warehouseManager.GetWarehouse(warehouse3.ID, true);
            Assert.AreEqual("new-name", newWarehouse.Name);
            Assert.IsFalse(newWarehouse.IsActive);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Changing a warehouse active status test. This method will be used in warehouse delete function.
        /// If success, warehouse activate status will be changed.
        /// </summary>
        [TestMethod]
        public async Task DeleteWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);
            List<Warehouse> warehouses = await WarehouseTestCommon.GetAllWarehouses(warehouseManager);
            // act
            await warehouseManager.ChangeActiveStatusOfWarehouse(warehouses[0].ID, false);

            // assert
            List<Warehouse> result = await warehouseManager.GetWarehouses();
            Assert.AreEqual(1, result.Count);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Changing a warehouse active status test. This method will be used in warehouse un delete function.
        /// If success, warehouse activate status will be changed. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UnDeleteWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);
            List<Warehouse> warehouses = await WarehouseTestCommon.GetAllWarehouses(warehouseManager);
            // act
            await warehouseManager.ChangeActiveStatusOfWarehouse(warehouses[2].ID, true);

            // assert
            List<Warehouse> result = await warehouseManager.GetWarehouses();
            Assert.AreEqual(3, result.Count);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        /// <summary>
        /// Permanentely delete the warehouse
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HardDeleteWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert3Warehouses(warehouseManager);
            List<Warehouse> warehouses = await WarehouseTestCommon.GetAllWarehouses(warehouseManager);

            // act
            await warehouseManager.DeleteWarehouse(warehouses[1].ID);
            
            // assert
            List<Warehouse> result = await warehouseManager.GetWarehouses();
            Assert.AreEqual(1, result.Count);

            // teardown
            await WarehouseTestCommon.DeleteAllWarehouses(warehouseManager);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            warehouseManager.Dispose();
        }
    }
}