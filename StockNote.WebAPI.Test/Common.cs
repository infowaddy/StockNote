using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using StockNote.BusinessLayer;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Data.SqlTypes;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockNote.WebAPI.Test
{
    public static class CommonCRUD
    {

        public static string testAPIVersion = "1";

        #region UnitController API URLs
        private static string unitControllerName = "Unit";
        public static string CreateUnitURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/CreateUnit/";
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/";
            }
        } 
        public static string GetAllUnitsURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return  unitControllerName + "/GetAllUnits/"; // including deleted units
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/true/";
            }
        }
        public static string GetActiveUnitByIDURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/GetActiveUnitByID/{0}/"; // not including deleted unit
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/{0}/";
            }
        } 
        public static string GetUnitsURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/GetUnits/"; // not including deleted units
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/";
            }
        } 
        public static string GetUnitByIDURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/GetUnitByID/{0}/"; // including deleted units
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/{0}/true/";
            }
        }
        public static string GetUnitByNameURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/GetUnitsByName/{0}/"; // not including deleted units
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/{0}/";
            }
        }

        public static string UpdateUnitURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/UpdateUnit/"; 
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/";
            }
        }
        public static string HardDeleteUnitURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/HardDeleteUnit/{0}/";
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/{0}/";
            }
        }
        public static string DeleteUnitURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/DeleteUnit/{0}/";
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/{0}/";
            }
        }
        public static string UnDeleteUnitURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return unitControllerName + "/UnDeleteUnit/{0}/";
                else
                    return "V" + testAPIVersion + "/" + unitControllerName + "s/{0}/true";
            }
        }
        #endregion

        #region ItemController API URLs
        private static string itemControllerName = "Item";
        public static string CreateItemURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/CreateItem/";
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/";
            }
        }
        public static string GetAllItemsURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/GetAllItems/"; // including deleted items
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/true/";
            }
        }
        public static string GetItemsURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/GetItems/"; // not including deleted items
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/";
            }
        }
        public static string GetItemByNameURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/GetItemsByName/{0}/"; // not including deleted items
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/{0}/";
            }
        }
        public static string GetItemByIDURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/GetItemByID/{0}/"; // including deleted items
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/{0}/true/";
            }
        }
        public static string GetActiveItemByIDURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/GetActiveItemByID/{0}/"; // not including deleted items
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/{0}/";
            }
        } 
        public static string UpdateItemURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/UpdateItem/"; 
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/";
            }
        }  
        public static string DeleteItemURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/DeleteItem/{0}/"; 
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/{0}/";
            }
        }  
        public static string HardDeleteItemURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/HardDeleteItem/{0}/"; 
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/{0}";
            }
        }
        public static string UnDeleteItemURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return itemControllerName + "/UnDeleteItem/{0}/"; 
                else
                    return "V" + testAPIVersion + "/" + itemControllerName + "s/{0}/true/";
            }
        } 
        #endregion

        #region WarehouseController API URLs
        private static  string warehouseControllerName = "Warehouse";
        public static string CreateWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/CreateWarehouse/";
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/";
            }
        }
        public static string GetWarehousesByNameURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/GetWarehousesByName/{0}/"; // not including deleted warehouse
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/{0}/";
            }
        }
        public static string GetWarehouseByIDURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/GetWarehouseByID/{0}/"; // including deleted warehouse
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/{0}/true/";
            }
        }
        public static string GetActiveWarehouseByIDURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/GetActiveWarehouseByID/{0}/"; // not including deleted warehouses
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/{0}/";
            }
        }
        public static string GetWarehousesURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/GetWarehouses/"; // not including deleted warehouse
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/";
            }
        }
        
        public static string GetAllWarehousesURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/GetAllWarehouses/"; // including deleted warehouse
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/true";
            }
        }

        public static string HardDeleteWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/HardDeleteWarehouse/{0}/"; 
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/{0}/";
            }
        }

        public static string DeleteWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/DeleteWarehouse/{0}/";
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/{0}/";
            }
        }

        public static string UnDeleteWarehouseURL 
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/UnDeleteWarehouse/{0}/";
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/{0}/true/";
            }
        }

        public static string UpdateWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return warehouseControllerName + "/UpdateWarehouse/";
                else
                    return "V" + testAPIVersion + "/" + warehouseControllerName + "s/";
            }
        }
        #endregion

        #region StockInWarehouseController API URLs
        private static string stockInWarehouseControllerName = "StockInWarehouse";
        public static string AddStockToWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/AddStockToWarehouse/";
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/";
            }
        }
        public static string GetStockInWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/GetStockInWarehouse/?itemID={0}&warehouseID={1}";
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{1}/{0}/"; // /warehouseID/itemID
            }
        }
        public static string GetStockInWarehousesURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/GetStockInWarehouses/"; // not including deleted stock-in-warehouse
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/"; 
            }
        }
        public static string GetAllStockInWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/GetAllStockInWarehouses/"; // including deleted stock-in-warehouse
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/true/";
            }
        }
        
        public static string GetStockItemListByWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/GetStockItemListByWarehouse/{0}/"; // not including deleted stock-in-warehouse
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{0}/items/";
            }
        }
        
        public static string GetWarehouseListByItemURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/GetWarehouseListByItem/{0}/"; // not including deleted stock-in-warehouse
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{0}/warehouses/";
            }
        }
        
        public static string UpdateStockInWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/UpdateStockInWarehouse/"; 
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/";
            }
        }
        public static string UpdateMinimumThresholdOfStockURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/UpdateMinimumThresholdOfStock/?itemID={0}&warehouseID={1}&minimumThreshold={2}";
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{1}/{0}/{2}/"; // warehouseID/itemID/minimumThreshold/
            }
        }
        
        public static string DeleteStockInWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/DeleteStockInWarehouse/{0}";
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{0}/";
            }
        }

        public static string HardDeleteStockInWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/HardDeleteStockInWarehouse/{0}";
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{0}/";
            }
        }

        public static string UnDeleteStockInWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockInWarehouseControllerName + "/UnDeleteStockInWarehouse/{0}";
                else
                    return "V" + testAPIVersion + "/" + stockInWarehouseControllerName + "s/{0}/true/";
            }
        }
        #endregion

        #region StockTransactionController API URLs
        private static string stockTransactionControllerName = "StockTransaction";
        public static string CreateSaleTransactionURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockTransactionControllerName + "/CreateSaleTransaction/";
                else
                    return "V" + testAPIVersion + "/" + stockTransactionControllerName + "s/";
            }
        }

        public static string CreateStockTransactionURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockTransactionControllerName + "/CreateStockTransaction/";
                else
                    return "V" + testAPIVersion + "/" + stockTransactionControllerName + "s/";
            }
        }
        
        public static string CreateAmendmentTransactionURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockTransactionControllerName + "/CreateAmendmentTransaction/";
                else
                    return "V" + testAPIVersion + "/" + stockTransactionControllerName + "s/";
            }
        }
        public static string GetTransactionByIDURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockTransactionControllerName + "/GetTransactionByID/{0}";
                else
                    return "V" + testAPIVersion + "/" + stockTransactionControllerName + "s/{0}/";
            }
        }
        public static string GetTransactionHistoryURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockTransactionControllerName + "/GetTransactionHistory/?startDate={0}&endDate={1}";
                else
                    return "V" + testAPIVersion + "/" + stockTransactionControllerName + "s/transactions?startDate={0}&endDate={1}";
            }
        }
        public static string GetTransactionHistoryByWarehouseURL
        {
            get
            {
                if (testAPIVersion == "0")
                    return stockTransactionControllerName + "/GetTransactionHistoryByWarehouse/?startDate={0}&endDate={1}&warehouseID={2}";
                else
                    return "V" + testAPIVersion + "/" + stockTransactionControllerName + "s/{2}/transactions?startDate={0}&endDate={1}"; // warehouseID/startDate/endDate/
            }
        }

        #endregion


        /// <summary>
        /// GET retrieves a representation of the resource at the specified URI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_client"></param>
        /// <param name="_url"></param>
        /// <returns></returns>
        public static async Task<T> GetAPIRequest<T>(HttpClient _client, string _url)
        {
            var response = await _client.GetAsync(_url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string data = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(data);
                return result;
            }
            else
                return default(T);
        }

        /// <summary>
        /// POST creates a new resource at the specified URI.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="_client"></param>
        /// <param name="_url"></param>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static async Task<T1> PostAPIRequest<T1, T2>(HttpClient _client, string _url, T2 _object)
        {
            try
            {
                string serializedObj = JsonConvert.SerializeObject(_object);
                var response = await _client.PostAsync(_url, new StringContent(serializedObj, Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    T1 result = JsonConvert.DeserializeObject<T1>(data);
                    return result;
                }
                else
                    return default(T1);
            }
            catch(Exception ex)
            {
                return default(T1);
            }
        }

        /// <summary>
        /// PUT <Update></Update> either creates or replaces the resource at the specified URI
        /// </summary>
        /// <param name="_client"></param>
        /// <param name="_url"></param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> PutAPIRequest<T>(HttpClient _client, string _url, T _object)
        {
            string serializedObj = JsonConvert.SerializeObject(_object);
            var response = await _client.PutAsync(_url, new StringContent(serializedObj, Encoding.UTF8, "application/json"));
            return response.StatusCode;
        }

        /// <summary>
        /// PATCH performs a partial update of a resource.
        /// </summary>
        /// <param name="_client"></param>
        /// <param name="_url"></param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> PatchAPIRequest(HttpClient _client, string _url)
        {
            var response = await _client.PatchAsync(_url, null);
            return response.StatusCode;
        }

        /// <summary>
        /// DELETE removes the resource at the specified URI.
        /// </summary>
        /// <param name="_client"></param>
        /// <param name="_url"></param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> DeleteAPIRequest(HttpClient _client, string _url)
        {
            var response = await _client.DeleteAsync(_url);
            return response.StatusCode;
        }
      
        /// <summary>
        /// Permanently delete all inserted units for testing
        /// </summary>
        /// <param name="client"></param>
        public static async Task HardDeleteAllUnits(HttpClient _client)
        {
            var allunits = await CommonCRUD.GetAPIRequest<List<Unit>>(_client, CommonCRUD.GetAllUnitsURL);
            List<Task> tasks = new List<Task>();
            foreach (Unit unit in allunits)
            {
                tasks.Add(CommonCRUD.DeleteAPIRequest(_client, string.Format(CommonCRUD.HardDeleteUnitURL, unit.ID)));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Delete all inserted items for testing
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task DeleteAllItems(HttpClient _client)
        {
            var allunits = await CommonCRUD.GetAPIRequest<List<Unit>>(_client, CommonCRUD.GetAllUnitsURL);
            var allitems = await CommonCRUD.GetAPIRequest<List<Item>>(_client, CommonCRUD.GetAllItemsURL);
            List<Task> tasks = new List<Task>();
            foreach (Item item in allitems)
            {
                tasks.Add(CommonCRUD.DeleteAPIRequest(_client, string.Format(CommonCRUD.HardDeleteItemURL, item.ID)));
            }
            foreach (Unit unit in allunits)
            {
                tasks.Add(CommonCRUD.DeleteAPIRequest(_client, string.Format(CommonCRUD.HardDeleteUnitURL, unit.ID)));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Delete all inserted warehouses for testing
        /// </summary>
        /// <param name="client"></param>
        public static async Task DeleteAllWarehouses(HttpClient _client)
        {
            var allwarehouses = await CommonCRUD.GetAPIRequest<List<Warehouse>>(_client, CommonCRUD.GetAllWarehousesURL);
            List<Task> tasks = new List<Task>();
            foreach (Warehouse warehouse in allwarehouses)
            {
                tasks.Add(CommonCRUD.DeleteAPIRequest(_client, string.Format(CommonCRUD.HardDeleteWarehouseURL, warehouse.ID)));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Permanentely delete all stock items from all warehouses
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task DeleteAllStockInWarehouse(HttpClient _client)
        {
            List<StockInWarehouse> stockInWarehouseList =await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(_client, CommonCRUD.GetAllStockInWarehouseURL);

            if (stockInWarehouseList != null)
            {
                List<Task> tasks = new List<Task>();
                foreach (StockInWarehouse stockinWarehouse in stockInWarehouseList)
                {
                    tasks.Add(CommonCRUD.DeleteAPIRequest(_client, string.Format(CommonCRUD.HardDeleteStockInWarehouseURL , stockinWarehouse.ID)));
                }
                await Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Create 3 warehouse, Warehouse-A, Warehouse-B, Warehouse-C
        /// </summary>
        /// <param name="_client"></param>
        /// <returns></returns>
        public static async Task Create3Warehouses(HttpClient _client)
        {
            List<Task> tasks = new List<Task>();
            List<Warehouse> tempwarehouses = new List<Warehouse>()
            {
                new Warehouse("Warehouse-A", "Address-A"),
                new Warehouse("Warehouse-B", "Address-B"),
                new Warehouse("Warehouse-C", "Address-C")
            };
            foreach (Warehouse warehouse in tempwarehouses)
            {
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Warehouse>(_client, CommonCRUD.CreateWarehouseURL, warehouse));
            }
            await Task.WhenAll(tasks.ToArray());
            tasks.Clear();
        }

        /// <summary>
        /// Create 3 items, Item-A, Item-B, Item-C
        /// </summary>
        /// <param name="_client"></param>
        /// <returns></returns>
        public static async Task Create3Items(HttpClient _client)
        {
            Unit unit = new Unit() { Name = "Unit", IsActive = true };
            unit.ID = await CommonCRUD.PostAPIRequest<Guid, Unit>(_client, CommonCRUD.CreateUnitURL, unit);

            List<Task> tasks = new List<Task>();
            List<Item> tempitems = new List<Item>()
            {
                new Item(){Name = "Item-A", IsActive = true, Barcode = "123456", Unit = unit},
                new Item(){Name = "Item-B", IsActive = true, Barcode = "233321", Unit = unit},
                new Item(){Name = "Item-C", IsActive = true, Barcode = "444221", Unit = unit}
            };

            foreach (Item item in tempitems)
            {
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Item>(_client, CommonCRUD.CreateItemURL, item));
            }
            await Task.WhenAll(tasks.ToArray());
            tasks.Clear();
        }
    }
    public static class UnitTestCommon
    {
        /// <summary>
        /// Insert 6 units.
        /// 5 are active and 1 are unactive.
        /// 4 are starting with letter A and 2 are starting with letter U.
        /// </summary>
        /// <param name="client"></param>
        public static async Task Insert6UnitsForTesting(HttpClient _client)
        {
            List<Unit> units = new List<Unit>()
            {
                new Unit(){Name = "AA-1", IsActive = true},
                new Unit(){Name = "AB-2", IsActive = false},
                new Unit(){Name = "AC-3", IsActive = true},
                new Unit(){Name = "Unit-4", IsActive = true},
                new Unit(){Name = "Unit-5", IsActive = true},
                new Unit(){Name = "AC-4", IsActive = true},
            };
            List<Task> tasks = new List<Task>();
            foreach (Unit unit in units)
            {
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Unit>(_client, CommonCRUD.CreateUnitURL, unit));
            }
            await Task.WhenAll(tasks.ToArray());            
        }

        /// <summary>
        /// Insert 3 units for test.
        /// 2 are active and 1 are unactive.
        /// All are starting with A.
        /// </summary>
        /// <param name="client"></param>
        public static async Task Insert3UnitsForTesting(HttpClient _client)
        {
            List<Unit> units = new List<Unit>()
            {
                new Unit(){Name = "AA-1", IsActive = true},
                new Unit(){Name = "AB-2", IsActive = false},
                new Unit(){Name = "AC-3", IsActive = true}
            };
            List<Task> tasks = new List<Task>();
            foreach (Unit unit in units)
            {                
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Unit>(_client, CommonCRUD.CreateUnitURL, unit));
            }
            await Task.WhenAll(tasks.ToArray());
        }
    }

    public static class WarehouseTestCommon
    {
        /// <summary>
        /// Insert 6 warehouses.
        /// 5 are active and 1 are unactive.
        /// 4 are starting with letter A and 2 are starting with letter U.
        /// </summary>
        /// <param name="client"></param>
        public static async Task Insert6WarehousesForTesting(HttpClient _client)
        {
            List<Warehouse> warehouses = new List<Warehouse>()
            {
                new Warehouse(){Name = "AA-1", IsActive = true},
                new Warehouse(){Name = "AB-2", IsActive = false},
                new Warehouse(){Name = "AC-3", IsActive = true},
                new Warehouse(){Name = "Warehouse-4", IsActive = true},
                new Warehouse(){Name = "Warehouse-5", IsActive = true},
                new Warehouse(){Name = "AC-4", IsActive = true},
            };
            List<Task> tasks = new List<Task>();
            foreach (Warehouse warehouse in warehouses)
            {                
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Warehouse>(_client, CommonCRUD.CreateWarehouseURL, warehouse));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Insert 3 warehouses for test.
        /// 2 are active and 1 are unactive.
        /// All are starting with A.
        /// </summary>
        /// <param name="client"></param>
        public static async Task Insert3WarehousesForTesting(HttpClient _client)
        {
            List<Warehouse> warehouses = new List<Warehouse>()
            {
                new Warehouse(){Name = "AA-1", IsActive = true},
                new Warehouse(){Name = "AB-2", IsActive = false},
                new Warehouse(){Name = "AC-3", IsActive = true}
            };
            List<Task> tasks = new List<Task>();
            foreach (Warehouse warehouse in warehouses)
            {
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Warehouse>(_client, CommonCRUD.CreateWarehouseURL, warehouse));
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
        /// <param name="client"></param>
        public static async Task Insert6ItemsForTesting(HttpClient _client)
        {
            Unit unit = new Unit("box");
            unit.ID = await CommonCRUD.PostAPIRequest<Guid, Unit>(_client, CommonCRUD.CreateUnitURL, unit);

            List<Item> items = new List<Item>()
            {
                new Item(){Name = "AA-1", IsActive = true, Barcode = "11111", Unit = unit},
                new Item(){Name = "AB-2", IsActive = false, Barcode = "22222", Unit = unit},
                new Item(){Name = "AC-3", IsActive = true, Barcode = "33333", Unit = unit},
                new Item(){Name = "Item-4", IsActive = true, Barcode = "44444", Unit = unit},
                new Item(){Name = "Item-5", IsActive = true, Barcode = "55555", Unit = unit},
                new Item(){Name = "AC-4", IsActive = true, Barcode = "66666", Unit = unit},
            };
            List<Task> tasks = new List<Task>();
            foreach (Item item in items)
            {
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Item>(_client, CommonCRUD.CreateItemURL, item) );
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Insert 3 items for test.
        /// 2 are active and 1 are unactive.
        /// All are starting with A.
        /// </summary>
        /// <param name="client"></param>
        public static async Task Insert3ItemsForTesting(HttpClient _client)
        {
            Unit unit = new Unit("box");
            unit.ID = await CommonCRUD.PostAPIRequest<Guid, Unit>(_client, CommonCRUD.CreateUnitURL, unit);

            List<Item> items = new List<Item>()
            {
                new Item(){Name = "AA-1", IsActive = true, Barcode = "aaaaa", Unit = unit},
                new Item(){Name = "AB-2", IsActive = false, Barcode = "bbbbb", Unit = unit},
                new Item(){Name = "AC-3", IsActive = true, Barcode = "ccccc", Unit = unit}
            };
            List<Task> tasks = new List<Task>();
            foreach (Item item in items)
            {
                tasks.Add(CommonCRUD.PostAPIRequest<Guid, Item>(_client, CommonCRUD.CreateItemURL, item));
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

        public static async Task ConstructUnitsItemsWarehouse(HttpClient _client)
        {
            List<Warehouse> warehouseList = new List<Warehouse>();
            List<Item> itemList = new List<Item>();

            List<Task> tasks = new List<Task>();
            if (warehouseList.Count < 1)
            {
                var result = await CommonCRUD.GetAPIRequest<List<Warehouse>>(_client, CommonCRUD.GetAllWarehousesURL);
                if (result != null)
                    warehouseList = result;
            }
            if (itemList.Count < 1)
            { 
                var result = await CommonCRUD.GetAPIRequest<List<Item>>(_client, CommonCRUD.GetAllItemsURL); 
                if (result != null) 
                    itemList = result;
            }
            if (warehouseList == null || warehouseList.Count < 1)
                await CommonCRUD.Create3Warehouses(_client);

            if (itemList == null || itemList.Count < 1)
                await CommonCRUD.Create3Items(_client);

            // get default warehouse            
            warehouseList = await CommonCRUD.GetAPIRequest<List<Warehouse>>(_client, CommonCRUD.GetWarehousesURL);
            warehouseA = warehouseList.Where(x => x.Name.ToLower() == "warehouse-a").FirstOrDefault();
            warehouseB = warehouseList.Where(x => x.Name.ToLower() == "warehouse-b").FirstOrDefault();
            warehouseC = warehouseList.Where(x => x.Name.ToLower() == "warehouse-c").FirstOrDefault();

            // get default items
            itemList = await CommonCRUD.GetAPIRequest<List<Item>>(_client, CommonCRUD.GetAllItemsURL);
            itemA = itemList.Where(x => x.Name.ToLower() == "item-a").FirstOrDefault();
            itemB = itemList.Where(x => x.Name.ToLower() == "item-b").FirstOrDefault();
            itemC = itemList.Where(x => x.Name.ToLower() == "item-c").FirstOrDefault();
        }

        public static async Task AddTestStocksToWarehouses(HttpClient _client)
        {
            List<Task> tasks = new List<Task>();

            #region add 3 items to warehouse A 
            StockInWarehouse stockInWarehouseA1 = new StockInWarehouse();
            stockInWarehouseA1.Item = itemA;
            stockInWarehouseA1.Warehouse = warehouseA;
            stockInWarehouseA1.Balance = 100;
            stockInWarehouseA1.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseA1));
            StockInWarehouse stockInWarehouseA2 = new StockInWarehouse();
            stockInWarehouseA2.Item = itemB;
            stockInWarehouseA2.Warehouse = warehouseA;
            stockInWarehouseA2.Balance = 50;
            stockInWarehouseA2.MinBalThreshold = 5;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseA2));
            StockInWarehouse stockInWarehouseA3 = new StockInWarehouse();
            stockInWarehouseA3.Item = itemC;
            stockInWarehouseA3.Warehouse = warehouseA;
            stockInWarehouseA3.Balance = 100;
            stockInWarehouseA3.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseA3));
            #endregion

            #region add 2 items to warehouse B, no ItemC in warehouse B
            StockInWarehouse stockInWarehouseB1 = new StockInWarehouse();
            stockInWarehouseB1.Item = itemA;
            stockInWarehouseB1.Warehouse = warehouseB;
            stockInWarehouseB1.Balance = 100;
            stockInWarehouseB1.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseB1));
            StockInWarehouse stockInWarehouseB2 = new StockInWarehouse();
            stockInWarehouseB2.Item = itemB;
            stockInWarehouseB2.Warehouse = warehouseB;
            stockInWarehouseB2.Balance = 50;
            stockInWarehouseB2.MinBalThreshold = 5;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseB2));
            #endregion

            #region add 3 items to warehouse C, but ItemB stock is zero
            StockInWarehouse stockInWarehouseC1 = new StockInWarehouse();
            stockInWarehouseC1.Item = itemA;
            stockInWarehouseC1.Warehouse = warehouseC;
            stockInWarehouseC1.Balance = 100;
            stockInWarehouseC1.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseC1));
            StockInWarehouse stockInWarehouseC2 = new StockInWarehouse();
            stockInWarehouseC2.Item = itemB;
            stockInWarehouseC2.Warehouse = warehouseC;
            stockInWarehouseC2.Balance = 0;
            stockInWarehouseC2.MinBalThreshold = 5;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseC2));
            StockInWarehouse stockInWarehouseC3 = new StockInWarehouse();
            stockInWarehouseC3.Item = itemC;
            stockInWarehouseC3.Warehouse = warehouseC;
            stockInWarehouseC3.Balance = 100;
            stockInWarehouseC3.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, stockInWarehouseC3));
            #endregion

            await Task.WhenAll(tasks);
        }

    }

    public static class StockTransactionTestCommon
    {
        public static Item itemA = new Item();
        public static Item itemB = new Item();
        public static Item itemC = new Item();

        public static Warehouse warehouseA = new Warehouse();
        public static Warehouse warehouseB = new Warehouse();
        public static Warehouse warehouseC = new Warehouse();

        public static async Task Construct3WarehousesWithStocks(HttpClient _client)
        {
            List<Warehouse> warehouseList = new List<Warehouse>();
            List<Item> itemList = new List<Item>();

            if (warehouseList.Count < 1)
            {
                var result = await CommonCRUD.GetAPIRequest<List<Warehouse>>(_client, CommonCRUD.GetWarehousesURL);
                if (result != null)
                    warehouseList = result;
            }

            if (itemList.Count < 1)
            {                
                var result = await CommonCRUD.GetAPIRequest<List<Item>>(_client, CommonCRUD.GetAllItemsURL);
                if(result != null)
                    itemList = result;
            }

            if (warehouseList == null || warehouseList.Count < 1)
                await CommonCRUD.Create3Warehouses(_client);

            if (itemList == null || itemList.Count < 1)
                await CommonCRUD.Create3Items(_client);

            // get default warehouse            
            warehouseList = await CommonCRUD.GetAPIRequest<List<Warehouse>>(_client, CommonCRUD.GetWarehousesURL);
            warehouseA = warehouseList.Where(x => x.Name.ToLower() == "warehouse-a").FirstOrDefault();
            warehouseB = warehouseList.Where(x => x.Name.ToLower() == "warehouse-b").FirstOrDefault();
            warehouseC = warehouseList.Where(x => x.Name.ToLower() == "warehouse-c").FirstOrDefault();

            // get default items
            itemList = await CommonCRUD.GetAPIRequest<List<Item>>(_client, CommonCRUD.GetAllItemsURL);
            itemA = itemList.Where(x => x.Name.ToLower() == "item-a").FirstOrDefault();
            itemB = itemList.Where(x => x.Name.ToLower() == "item-b").FirstOrDefault();
            itemC = itemList.Where(x => x.Name.ToLower() == "item-c").FirstOrDefault();

            List<Task> tasks = new List<Task>();
            #region add 3 items to Warehouse-A
            StockInWarehouse warehouseAItemA = new StockInWarehouse();
            warehouseAItemA.Item = itemA;
            warehouseAItemA.Warehouse = warehouseA;
            warehouseAItemA.Balance = 100;
            warehouseAItemA.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseAItemA));
            StockInWarehouse warehouseAItemB = new StockInWarehouse();
            warehouseAItemB.Item = itemB;
            warehouseAItemB.Warehouse = warehouseA;
            warehouseAItemB.Balance = 100;
            warehouseAItemB.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseAItemB));
            StockInWarehouse warehouseAItemC = new StockInWarehouse();
            warehouseAItemC.Item = itemC;
            warehouseAItemC.Warehouse = warehouseA;
            warehouseAItemC.Balance = 100;
            warehouseAItemC.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseAItemC));
            #endregion

            #region add 3 items to Warehouse-B
            StockInWarehouse warehouseBItemA = new StockInWarehouse();
            warehouseBItemA.Item = itemA;
            warehouseBItemA.Warehouse = warehouseB;
            warehouseBItemA.Balance = 100;
            warehouseBItemA.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseBItemA));
            StockInWarehouse warehouseBItemB = new StockInWarehouse();
            warehouseBItemB.Item = itemB;
            warehouseBItemB.Warehouse = warehouseB;
            warehouseBItemB.Balance = 100;
            warehouseBItemB.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseBItemB));
            StockInWarehouse warehouseBItemC = new StockInWarehouse();
            warehouseBItemC.Item = itemC;
            warehouseBItemC.Warehouse = warehouseB;
            warehouseBItemC.Balance = 100;
            warehouseBItemC.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseBItemC));
            #endregion

            #region add 3 items to Warehouse-C
            StockInWarehouse warehouseCItemA = new StockInWarehouse();
            warehouseCItemA.Item = itemA;
            warehouseCItemA.Warehouse = warehouseC;
            warehouseCItemA.Balance = 100;
            warehouseCItemA.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseCItemA));
            StockInWarehouse warehouseCItemB = new StockInWarehouse();
            warehouseCItemB.Item = itemB;
            warehouseCItemB.Warehouse = warehouseC;
            warehouseCItemB.Balance = 100;
            warehouseCItemB.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseCItemB));
            StockInWarehouse warehouseCItemC = new StockInWarehouse();
            warehouseCItemC.Item = itemC;
            warehouseCItemC.Warehouse = warehouseC;
            warehouseCItemC.Balance = 100;
            warehouseCItemC.MinBalThreshold = 10;
            tasks.Add(CommonCRUD.PostAPIRequest<Guid, StockInWarehouse>(_client, CommonCRUD.AddStockToWarehouseURL, warehouseCItemC));
            #endregion

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Reset all stock balance from all warehouse.
        /// Default stock balance for all items are 100 boxes. 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task ResetToDefaultStocksBalanceInWarehouses(HttpClient _client)
        {
            List<StockInWarehouse>  stockInWarehouseList = await CommonCRUD.GetAPIRequest<List<StockInWarehouse>>(_client, CommonCRUD.GetAllStockInWarehouseURL);
            
            if (stockInWarehouseList != null)
            {    
                List<Task> tasks = new List<Task>();            
                foreach (StockInWarehouse stockinWarehouse in stockInWarehouseList)
                {
                    stockinWarehouse.Balance = 100;
                    stockinWarehouse.MinBalThreshold = 10;
                    
                    tasks.Add(CommonCRUD.PutAPIRequest<StockInWarehouse>(_client, CommonCRUD.UpdateStockInWarehouseURL, stockinWarehouse));
                }
                await Task.WhenAll(tasks);
            }            
        }
    }


}
