namespace StockNote.BusinessLayer.Test
{
    [TestClass]
    public class ItemManagerTest
    {
        private static ItemManager itemManager;
        private static UnitManager unitManager;
        private static StockNoteDBContext stockNoteDBContext = DatabaseInMemory.StockNoteDBContext;
        [TestInitialize]
        public void TestInitialize()
        {
            itemManager = new ItemManager(stockNoteDBContext, Common.GenerateLogger<ItemManager>());
            unitManager = new UnitManager(stockNoteDBContext, Common.GenerateLogger<UnitManager>());
        }

        /// <summary>
        /// Creating a item test. 
        /// If success, newly inserted item ID will be return with Guid format
        /// </summary>
        [TestMethod]
        public async Task CreateItem()
        {
            // prepare
            Item item = new Item("Item", "3456789", new Unit("box"));
            //item.Name = "Item";
            //item.IsActive = true;
            //item.Barcode = "3456789";

            // act
            Guid result = await itemManager.CreateItem(item);

            // assert
            Assert.AreNotEqual(result, Guid.Empty);

            // teardown
            await itemManager.DeleteItem(item.ID);
        }

        /// <summary>
        /// Retrieving active items test.
        /// If success, active 2 items will be return.
        /// </summary>
        [TestMethod]
        public async Task GetActiveItemsOnly()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);

            // act
            List<Item> result1 = await itemManager.GetItems();

            // assert
            Assert.AreEqual(2, result1.Count);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager,unitManager);
        }

        /// <summary>
        /// Retrieving all items test.
        /// If success, 3 items which including deleted also will be return.
        /// </summary>
        [TestMethod]
        public async Task GetAllItemsIncludeDeleted()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);
            
            // act
            List<Item> result1 = await itemManager.GetItems(true);

            // assert
            Assert.AreEqual(3, result1.Count);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager,unitManager);
        }

        /// <summary>
        /// Retrieving active items test.
        /// If success, 2 items will be return. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetItems()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);

            // act
            List<Item> result1 = await itemManager.GetItems();

            // assert
            Assert.AreEqual(2, result1.Count);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        /// <summary>
        /// Getting items filter by name test.
        /// If success, active 3 items will be return and order by name.
        /// </summary>
        [TestMethod]
        public async Task GetItemsByName()
        {
            // prepare
            await ItemTestCommon.Insert6Items(itemManager, unitManager);

            // act
            List<Item> result = await itemManager.GetItems("A");

            // assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("AC-4", result[2].Name); // sorting test

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        /// <summary>
        /// Getting a item by ID.
        /// If success, one item will be return.
        /// </summary>
        [TestMethod]
        public async Task GetItemByID()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);
            List<Item> items = await ItemTestCommon.GetAllItems(itemManager);
            Item unactiveItem = items[2]; // get default unactive item from test data.

            // act
            Item result = await itemManager.GetItem(unactiveItem.ID, true);
            // assert
            Assert.AreEqual(unactiveItem, result);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }
        public async Task GetActiveItemByID()
        {
            // prepare
            await ItemTestCommon.Insert6Items(itemManager, unitManager);
            List<Item> items = await ItemTestCommon.GetAllItems(itemManager);
            Item activeItem = items[0];
            Item unactiveItem = items[1];

            // act
            Item result1 = await itemManager.GetItem(activeItem.ID);
            Item result2 = await itemManager.GetItem(unactiveItem.ID);
            // assert
            Assert.AreEqual(activeItem, result1);
            Assert.IsNull(result2);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        /// <summary>
        /// Updating an existing item test.
        /// If success, updated item will be return with new updated value.
        /// </summary>
        [TestMethod]
        public async Task UpdateItem()
        {
            // prepare
            await ItemTestCommon.Insert6Items(itemManager, unitManager);
            List<Item> items = await ItemTestCommon.GetAllItems(itemManager);
            Item item3 = items[3];
            Item oldItem = await itemManager.GetItem(item3.ID);

            // act
            oldItem.Name = "new-name";
            oldItem.IsActive = false;
            bool result = await itemManager.UpdateItem(oldItem);

            // assert
            Item newItem = await itemManager.GetItem(item3.ID, true);
            Assert.AreEqual("new-name", newItem.Name);
            Assert.IsFalse(newItem.IsActive);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        /// <summary>
        /// Changing a item active status test. This method will be used in item delete function.
        /// If success, item activate status will be changed.
        /// </summary>
        [TestMethod]
        public async Task DeleteItem()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);
            List<Item> items = await ItemTestCommon.GetAllItems(itemManager);
            // act
            await itemManager.ChangeActiveStatusOfItem(items[0].ID, false);

            // assert
            List<Item> result = await itemManager.GetItems();
            Assert.AreEqual(1, result.Count);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        /// <summary>
        /// Changing a item active status test. This method will be used in item un delete function.
        /// If success, item activate status will be changed. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UnDeleteItem()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);
            List<Item> items = await ItemTestCommon.GetAllItems(itemManager);
            // act
            await itemManager.ChangeActiveStatusOfItem(items[2].ID, true);

            // assert
            List<Item> result = await itemManager.GetItems();
            Assert.AreEqual(3, result.Count);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        /// <summary>
        /// Permanentely delete the item
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HardDeleteItem()
        {
            // prepare
            await ItemTestCommon.Insert3Items(itemManager, unitManager);
            List<Item> items = await ItemTestCommon.GetAllItems(itemManager);

            // act
            await itemManager.DeleteItem(items[1].ID);
            
            // assert
            List<Item> result = await itemManager.GetItems();
            Assert.AreEqual(1, result.Count);

            // teardown
            await ItemTestCommon.DeleteAllItems(itemManager, unitManager);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            itemManager.Dispose();
        }
    }
}