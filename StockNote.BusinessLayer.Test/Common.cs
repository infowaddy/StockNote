using StockNote.BusinessLayer.Interfaces;
using System.Data.SqlTypes;

namespace StockNote.BusinessLayer.Test
{   
    public static class Common{

        public static ILogger<T> GenerateLogger<T>()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<T>();
            return logger;
        }

    }
    public static class UnitTestCommon
    {
        /// <summary>
        /// Insert 6 units.
        /// 5 are active and 1 are unactive.
        /// 4 are starting with letter A and 2 are starting with letter U. 
        /// </summary>
        /// <param name="_unitManager"></param>
        /// <returns></returns>
        public static async Task Insert6Units(UnitManager _unitManager)
        {
            List<Unit> units = new List<Unit>()
            {
                new Unit(){Name = "AA-1", IsActive = true},
                new Unit(){Name = "AB-2", IsActive = false},
                new Unit(){Name = "AC-3", IsActive = true},
                new Unit(){Name = "Unit-4", IsActive = true},
                new Unit(){Name = "Unit-5", IsActive = true},
                new Unit(){Name = "AC-4", IsActive = true}
            };
            List<Task> tasks = new List<Task>();
            foreach (Unit unit in units)
            {
                tasks.Add(_unitManager.CreateUnit(unit));
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Insert 3 units for test.
        /// 2 are active and 1 are unactive.
        /// All are starting with A. 
        /// </summary>
        /// <param name="_unitManager"></param>
        public static async Task Insert3Units(UnitManager _unitManager)
        {
            List<Unit> units = new List<Unit>()
            {
                new Unit(){Name = "Unit-1", IsActive = true},
                new Unit(){Name = "Unit-2", IsActive = true},
                new Unit(){Name = "Unit-3", IsActive = false}
            };
            List<Task> tasks = new List<Task>();
            foreach (Unit unit in units)
            {
                tasks.Add(_unitManager.CreateUnit(unit));
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Get all units which are inserted for testing.
        /// </summary>
        /// <param name="_unitManager"></param>
        /// <returns></returns>
        public static async Task<List<Unit>> GetAllUnits(UnitManager _unitManager)
        {
            return await _unitManager.GetUnits(true);
        }
        /// <summary>
        /// Delete all inserted units for testing 
        /// </summary>
        /// <param name="_unitManager"></param>
        /// <returns></returns>
        public static async Task DeleteAllUnits(UnitManager _unitManager)
        {
            List<Unit> units = await GetAllUnits(_unitManager);
            List<Task> tasks = new List<Task>();
            foreach (Unit unit in units)
            {
                tasks.Add(_unitManager.DeleteUnit(unit.ID));
            }
            await Task.WhenAll(tasks.ToArray());
        }
    }

    public static class WarehouseTestCommon
    {
        /// <summary>
        /// Insert 6 warehouses.
        /// 5 are active and 1 are unactive.
        /// 4 are starting with letter A and 2 are starting with letter B. 
        /// </summary>
        /// <param name="_warehouseManager"></param>
        /// <returns></returns>
        public static async Task Insert6Warehouses(WarehouseManager _warehouseManager)
        {
            List<Warehouse> warehouses = new List<Warehouse>()
            {
                new Warehouse(){Name = "A-Warehouse-1", IsActive = true},
                new Warehouse(){Name = "A-Warehouse-2", IsActive = false},
                new Warehouse(){Name = "A-Warehouse-3", IsActive = true},
                new Warehouse(){Name = "B-Warehouse-1", IsActive = true},
                new Warehouse(){Name = "B-Warehouse-2", IsActive = true},
                new Warehouse(){Name = "A-Warehouse-4", IsActive = true}
            };
            List<Task> tasks = new List<Task>();
            foreach (Warehouse warehouse in warehouses)
            {
                tasks.Add(_warehouseManager.CreateWarehouse(warehouse));
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Insert 3 warehouses for test.
        /// 2 are active and 1 are unactive.
        /// All are starting with A. 
        /// </summary>
        /// <param name="_warehouseManager"></param>
        public static async Task Insert3Warehouses(WarehouseManager _warehouseManager)
        {
            List<Warehouse> warehouses = new List<Warehouse>()
            {
                new Warehouse(){Name = "A-Warehouse-1", IsActive = true},
                new Warehouse(){Name = "A-Warehouse-2", IsActive = true},
                new Warehouse(){Name = "A-Warehouse-3", IsActive = false},
            };
            List<Task> tasks = new List<Task>();
            foreach (Warehouse warehouse in warehouses)
            {
                tasks.Add(_warehouseManager.CreateWarehouse(warehouse));
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Get all warehouses which are inserted for testing.
        /// </summary>
        /// <param name="_warehouseManager"></param>
        /// <returns></returns>
        public static async Task<List<Warehouse>> GetAllWarehouses(WarehouseManager _warehouseManager)
        {
            return await _warehouseManager.GetWarehouses(true);
        }
        /// <summary>
        /// Delete all inserted warehouses for testing 
        /// </summary>
        /// <param name="_warehouseManager"></param>
        /// <returns></returns>
        public static async Task DeleteAllWarehouses(WarehouseManager _warehouseManager)
        {
            List<Warehouse> warehouses = await GetAllWarehouses(_warehouseManager);
            List<Task> tasks = new List<Task>();
            foreach (Warehouse warehouse in warehouses)
            {
                tasks.Add(_warehouseManager.DeleteWarehouse(warehouse.ID));
            }
            await Task.WhenAll(tasks.ToArray());
        }
    }

    public static class ItemTestCommon
    {
        /// <summary>
        /// Insert 6 items.
        /// 5 are active and 1 are unactive.
        /// 4 are starting with letter A and 2 are starting with letter U. 
        /// </summary>
        /// <param name="_itemManager"></param>
        /// <returns></returns>
        public static async Task Insert6Items(ItemManager _itemManager, UnitManager _unitManager)
        {
            Unit unit = new Unit("box");
            await _unitManager.CreateUnit(unit);
            List<Item> items = new List<Item>()
            {
                new Item(){Name = "AA-1", IsActive = true, Barcode = "11111", Unit = unit},
                new Item(){Name = "AB-2", IsActive = false,Barcode = "22222", Unit = unit},
                new Item(){Name = "AC-3", IsActive = true, Barcode = "33333", Unit = unit},
                new Item(){Name = "Item-4", IsActive = true,Barcode = "44444", Unit = unit},
                new Item(){Name = "Item-5", IsActive = true, Barcode = "55555", Unit = unit},
                new Item(){Name = "AC-4", IsActive = true, Barcode = "66666", Unit = unit}
            };
            List<Task> tasks = new List<Task>();
            foreach (Item item in items)
            {
                tasks.Add(_itemManager.CreateItem(item));
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Insert 3 items for test.
        /// 2 are active and 1 are unactive.
        /// All are starting with A. 
        /// </summary>
        /// <param name="_itemManager"></param>
        public static async Task Insert3Items(ItemManager _itemManager, UnitManager _unitManager)
        {
            Unit unit = new Unit("box");
            await _unitManager.CreateUnit(unit);
            List<Item> items = new List<Item>()
            {
                new Item(){Name = "Item-1", IsActive = true, Barcode = "aaaaa", Unit = unit},
                new Item(){Name = "Item-2", IsActive = true, Barcode = "bbbbb", Unit = unit},
                new Item(){Name = "Item-3", IsActive = false, Barcode = "ccccc", Unit = unit}
            };
            List<Task> tasks = new List<Task>();
            foreach (Item item in items)
            {
                tasks.Add(_itemManager.CreateItem(item));
            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Get all items which are inserted for testing.
        /// </summary>
        /// <param name="_itemManager"></param>
        /// <returns></returns>
        public static async Task<List<Item>> GetAllItems(ItemManager _itemManager)
        {
            return await _itemManager.GetItems(true);
        }
        /// <summary>
        /// Delete all inserted items for testing 
        /// </summary>
        /// <param name="_itemManager"></param>
        /// <returns></returns>
        public static async Task DeleteAllItems(ItemManager _itemManager, UnitManager _unitManager)
        {
            List<Item> items = await GetAllItems(_itemManager);
            List<Task> tasks = new List<Task>();
            foreach (Item item in items)
            {
                tasks.Add(_itemManager.DeleteItem(item.ID));
            }
            List<Unit> units = await _unitManager.GetUnits();
            foreach (Unit unit in units)
            {
                tasks.Add(_unitManager.DeleteUnit(unit.ID));
            }
            await Task.WhenAll(tasks.ToArray());
        }
    }

    public static class StockInWarehouseTestCommon
    {
        
        public static Item itemA = new Item();
        public static Item itemB = new Item();
        public static Item itemC = new Item();

        public static Warehouse warehouseA = new Warehouse();
        public static Warehouse warehouseB = new Warehouse();
        public static Warehouse warehouseC = new Warehouse();

        /// <summary>
        /// Create one default unit, 3 items and 3 warehouses
        /// </summary>
        /// <param name="_unitManager"></param>
        /// <param name="_warehouseManager"></param>
        /// <param name="_itemManager"></param>
        /// <returns></returns>
        public static async Task ConstructUnitsItemsWarehouse(
            UnitManager _unitManager, WarehouseManager _warehouseManager, ItemManager _itemManager)
        {
            List<Warehouse> warehouseList = new List<Warehouse>();
            List<Item> itemList = new List<Item>();

            List<Task> tasks = new List<Task>();

            if (warehouseList.Count < 1)
                warehouseList = await _warehouseManager.GetWarehouses(true);

            if (itemList.Count < 1)
                itemList = await _itemManager.GetItems(true);


            if (warehouseList.Count < 1)
            {
                tasks.Add(_warehouseManager.CreateWarehouse(new Warehouse("Warehouse-A", "Address-A")));
                tasks.Add(_warehouseManager.CreateWarehouse(new Warehouse("Warehouse-B", "Address-B")));
                tasks.Add(_warehouseManager.CreateWarehouse(new Warehouse("Warehouse-C", "Address-C")));
            }
            if (itemList.Count < 1)
            {
                Unit unit = new Unit("box");
                await _unitManager.CreateUnit(new Unit("box"));

                tasks.Add(_itemManager.CreateItem(new Item("Item-A", "123456", unit)));
                tasks.Add(_itemManager.CreateItem(new Item("Item-B", "233321", unit)));
                tasks.Add(_itemManager.CreateItem(new Item("Item-C", "444221", unit)));
            }
            await Task.WhenAll(tasks);

            // get default warehouse
            warehouseList = await _warehouseManager.GetWarehouses();
            warehouseA = warehouseList.Where(x => x.Name.ToLower() == "warehouse-a").FirstOrDefault();
            warehouseB = warehouseList.Where(x => x.Name.ToLower() == "warehouse-b").FirstOrDefault();
            warehouseC = warehouseList.Where(x => x.Name.ToLower() == "warehouse-c").FirstOrDefault();

            // get default items
            itemList = await _itemManager.GetItems();
            itemA = itemList.Where(x => x.Name.ToLower() == "item-a").FirstOrDefault();
            itemB = itemList.Where(x => x.Name.ToLower() == "item-b").FirstOrDefault();
            itemC = itemList.Where(x => x.Name.ToLower() == "item-c").FirstOrDefault();
        }

        /// <summary>
        /// For testing, there are 3 default warehouses and 3 items.
        /// Warehouse-A is started with 3 items.
        /// Warehouse-B is started with 2 items, no Item-C in Warehouse-B
        /// Warehouse-C is started with 3 items, but Item-B stock balance is zero.
        /// </summary>
        /// <param name="_stockInWarehouseManager"></param>
        /// <param name="_warehouseManager"></param>
        /// <param name="_itemManager"></param>
        /// <returns></returns>
        public static async Task AddTestStocksToWarehouses(StockInWarehouseManager _stockInWarehouseManager, WarehouseManager _warehouseManager, ItemManager _itemManager)
        {
            List<Task> tasks = new List<Task>();

            #region add 3 items to warehouse A 
            StockInWarehouse stockInWarehouseA1 = new StockInWarehouse();
            stockInWarehouseA1.Item = itemA;
            stockInWarehouseA1.Warehouse = warehouseA;
            stockInWarehouseA1.Balance = 100;
            stockInWarehouseA1.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseA1));
            StockInWarehouse stockInWarehouseA2 = new StockInWarehouse();
            stockInWarehouseA2.Item = itemB;
            stockInWarehouseA2.Warehouse = warehouseA;
            stockInWarehouseA2.Balance = 50;
            stockInWarehouseA2.MinBalThreshold = 5;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseA2));
            StockInWarehouse stockInWarehouseA3 = new StockInWarehouse();
            stockInWarehouseA3.Item = itemC;
            stockInWarehouseA3.Warehouse = warehouseA;
            stockInWarehouseA3.Balance = 100;
            stockInWarehouseA3.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseA3));
            #endregion

            #region add 2 items to warehouse B, no ItemC in warehouse B
            StockInWarehouse stockInWarehouseB1 = new StockInWarehouse();
            stockInWarehouseB1.Item = itemA;
            stockInWarehouseB1.Warehouse = warehouseB;
            stockInWarehouseB1.Balance = 100;
            stockInWarehouseB1.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseB1));
            StockInWarehouse stockInWarehouseB2 = new StockInWarehouse();
            stockInWarehouseB2.Item = itemB;
            stockInWarehouseB2.Warehouse = warehouseB;
            stockInWarehouseB2.Balance = 50;
            stockInWarehouseB2.MinBalThreshold = 5;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseB2));
            #endregion

            #region add 3 items to warehouse C, but ItemB stock is zero
            StockInWarehouse stockInWarehouseC1 = new StockInWarehouse();
            stockInWarehouseC1.Item = itemA;
            stockInWarehouseC1.Warehouse = warehouseC;
            stockInWarehouseC1.Balance = 100;
            stockInWarehouseC1.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseC1));
            StockInWarehouse stockInWarehouseC2 = new StockInWarehouse();
            stockInWarehouseC2.Item = itemB;
            stockInWarehouseC2.Warehouse = warehouseC;
            stockInWarehouseC2.Balance = 0;
            stockInWarehouseC2.MinBalThreshold = 5;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseC2));
            StockInWarehouse stockInWarehouseC3 = new StockInWarehouse();
            stockInWarehouseC3.Item = itemC;
            stockInWarehouseC3.Warehouse = warehouseC;
            stockInWarehouseC3.Balance = 100;
            stockInWarehouseC3.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(stockInWarehouseC3));
            #endregion

            await Task.WhenAll(tasks);
        }
        
        /// <summary>
        /// Delete all stock items from all warehouses
        /// </summary>
        /// <param name="_stockInWarehouseManager"></param>
        /// <returns></returns>
        public static async Task DeleteAllStocksInWarehouses(StockInWarehouseManager _stockInWarehouseManager)
        {
            List<StockInWarehouse> stockInWarehouseList = await _stockInWarehouseManager.GetStockInWarehouses(true);
            List<Task> tasks = new List<Task>();
            foreach(StockInWarehouse stockinWarehouse in stockInWarehouseList)
            {
                tasks.Add(_stockInWarehouseManager.DeleteStockInWarehouse(stockinWarehouse.ID));
            }
            await Task.WhenAll(tasks);
        }
    }

    public static class StockTransactionTestCommon
    {
        private static List<Unit> unitList = new List<Unit>();
        private static List<Warehouse> warehouseList = new List<Warehouse>();
        private static List<Item> itemList = new List<Item>();
        private static List<StockInWarehouse> stockInWarehouseList = new List<StockInWarehouse>();

        public static  Item itemA = new Item();
        public static  Item itemB = new Item();
        public static  Item itemC = new Item();

        public static  Warehouse warehouseA = new Warehouse();
        public static  Warehouse warehouseB = new Warehouse();
        public static  Warehouse warehouseC = new Warehouse();
        /// <summary>
        /// Create one default unit, 3 items and 3 warehouses 
        /// Add all item with 100 boxes balance to all warehouses and all minimum balance threshold is 10
        /// </summary>
        /// <param name="_unitManager"></param>
        /// <param name="_warehouseManager"></param>
        /// <param name="_itemManager"></param>
        /// <returns></returns>
        public static async Task Construct3WarehousesWithStocks(
            UnitManager _unitManager, WarehouseManager _warehouseManager, 
            ItemManager _itemManager, StockInWarehouseManager _stockInWarehouseManager)
        {
            List<Task> tasks = new List<Task>();

            if (warehouseList.Count < 1)
                warehouseList = await _warehouseManager.GetWarehouses(true);

            if (itemList.Count < 1)
                itemList = await _itemManager.GetItems(true);


            if (warehouseList.Count < 1)
            {
                tasks.Add(_warehouseManager.CreateWarehouse(new Warehouse("Warehouse-A", "Address-A")));
                tasks.Add(_warehouseManager.CreateWarehouse(new Warehouse("Warehouse-B", "Address-B")));
                tasks.Add(_warehouseManager.CreateWarehouse(new Warehouse("Warehouse-C", "Address-C")));
            }

            if (itemList.Count < 1)
            {
                Unit unit = new Unit("box");
                await _unitManager.CreateUnit(new Unit("box"));

                tasks.Add(_itemManager.CreateItem(new Item("Item-A", "123456", unit)));
                tasks.Add(_itemManager.CreateItem(new Item("Item-B", "233321", unit)));
                tasks.Add(_itemManager.CreateItem(new Item("Item-C", "444221", unit)));
            }

            await Task.WhenAll(tasks);

            // get default warehouse
            warehouseList = await _warehouseManager.GetWarehouses();
            warehouseA = warehouseList.Where(x => x.Name.ToLower() == "warehouse-a").FirstOrDefault();
            warehouseB = warehouseList.Where(x => x.Name.ToLower() == "warehouse-b").FirstOrDefault();
            warehouseC = warehouseList.Where(x => x.Name.ToLower() == "warehouse-c").FirstOrDefault();
            // get default items
            itemList = await _itemManager.GetItems();
            itemA = itemList.Where(x => x.Name.ToLower() == "item-a").FirstOrDefault();
            itemB = itemList.Where(x => x.Name.ToLower() == "item-b").FirstOrDefault();
            itemC = itemList.Where(x => x.Name.ToLower() == "item-c").FirstOrDefault();

            tasks.Clear();
            #region add 3 items to Warehouse-A
            StockInWarehouse warehouseAItemA = new StockInWarehouse();
            warehouseAItemA.Item = itemA;
            warehouseAItemA.Warehouse = warehouseA;
            warehouseAItemA.Balance = 100;
            warehouseAItemA.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseAItemA));
            StockInWarehouse warehouseAItemB = new StockInWarehouse();
            warehouseAItemB.Item = itemB;
            warehouseAItemB.Warehouse = warehouseA;
            warehouseAItemB.Balance = 100;
            warehouseAItemB.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseAItemB));
            StockInWarehouse warehouseAItemC = new StockInWarehouse();
            warehouseAItemC.Item = itemC;
            warehouseAItemC.Warehouse = warehouseA;
            warehouseAItemC.Balance = 100;
            warehouseAItemC.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseAItemC));
            #endregion

            #region add 3 items to Warehouse-B
            StockInWarehouse warehouseBItemA = new StockInWarehouse();
            warehouseBItemA.Item = itemA;
            warehouseBItemA.Warehouse = warehouseB;
            warehouseBItemA.Balance = 100;
            warehouseBItemA.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseBItemA));
            StockInWarehouse warehouseBItemB = new StockInWarehouse();
            warehouseBItemB.Item = itemB;
            warehouseBItemB.Warehouse = warehouseB;
            warehouseBItemB.Balance = 100;
            warehouseBItemB.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseBItemB));
            StockInWarehouse warehouseBItemC = new StockInWarehouse();
            warehouseBItemC.Item = itemC;
            warehouseBItemC.Warehouse = warehouseB;
            warehouseBItemC.Balance = 100;
            warehouseBItemC.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseBItemC));
            #endregion

            #region add 3 items to Warehouse-C
            StockInWarehouse warehouseCItemA = new StockInWarehouse();
            warehouseCItemA.Item = itemA;
            warehouseCItemA.Warehouse = warehouseC;
            warehouseCItemA.Balance = 100;
            warehouseCItemA.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseCItemA));
            StockInWarehouse warehouseCItemB = new StockInWarehouse();
            warehouseCItemB.Item = itemB;
            warehouseCItemB.Warehouse = warehouseC;
            warehouseCItemB.Balance = 100;
            warehouseCItemB.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseCItemB));
            StockInWarehouse warehouseCItemC = new StockInWarehouse();
            warehouseCItemC.Item = itemC;
            warehouseCItemC.Warehouse = warehouseC;
            warehouseCItemC.Balance = 100;
            warehouseCItemC.MinBalThreshold = 10;
            tasks.Add(_stockInWarehouseManager.AddStockToWarehouse(warehouseCItemC));
            #endregion

            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Reset all stock balance from all warehouse.
        /// Default stock balance for all items are 100 boxes.
        /// </summary>
        /// <param name="_stockInWarehouseManager"></param>
        /// <returns></returns>
        public static async Task ResetToDefaultStocksBalanceInWarehouses(StockInWarehouseManager _stockInWarehouseManager)
        {
            List<StockInWarehouse> stockInWarehouseList = await _stockInWarehouseManager.GetStockInWarehouses();
            List<Task> tasks = new List<Task>();
            foreach (StockInWarehouse stockinWarehouse in stockInWarehouseList)
            {
                stockinWarehouse.Balance = 100;
                stockinWarehouse.MinBalThreshold = 10;
                tasks.Add(_stockInWarehouseManager.UpdateStockInWarehouse(stockinWarehouse));
            }
            await Task.WhenAll(tasks);
        }
    }
}
