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
    public class WarehouseControllerTest
    {
        static WebAppTestFactory factory = new WebAppTestFactory();
        static HttpClient client;


        [TestInitialize]
        public void TestInitialize()
        {
            client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("api-version", CommonCRUD.testAPIVersion);
        }
        /// <summary>
        /// Testing "Warehouse/CreateWarehouse" URL for creating warehouse controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateWarehouse()
        {
            // arrange
            Warehouse warehouse = new Warehouse(){Name= "Warehouse",IsActive = true};

            // act
            Guid result = await CommonCRUD.PostAPIRequest<Guid, Warehouse>(client, CommonCRUD.CreateWarehouseURL, warehouse);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteWarehouseURL, result));
        }

        /// <summary>
        /// Testing "Warehouse/GetWarehouses" URL for getting active warehouses only controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetWarehouses()
        {
            // arrange            
            await WarehouseTestCommon.Insert3WarehousesForTesting(client);
            
            // act
            List<Warehouse> result = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetWarehousesURL);

            // assert
            Assert.AreEqual(2, result.Count);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Testing "Warehouse/GetAllWarehouses" URL for getting all warehouses controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAllWarehouses()
        {
            // arrange
            await WarehouseTestCommon.Insert3WarehousesForTesting(client);

            // act
            List<Warehouse> result = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);

            // assert
            Assert.AreEqual(3, result.Count);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Testing "Warehouse/GetWarehousesByName" URL for getting all warehouses filter by name with sorting order controller method. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetWarehousesByName()
        {
            // arrange
            await WarehouseTestCommon.Insert6WarehousesForTesting(client);

            // act
            List<Warehouse> result = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, string.Format(CommonCRUD.GetWarehousesByNameURL ,"A"));

            // assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("AC-4", result[2].Name); // sorting test

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Testing "Warehouse/GetWarehouseByID" URL for getting a warehouse filter by ID controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetWarehouseByID()
        {
            // arrange
            await WarehouseTestCommon.Insert6WarehousesForTesting(client);
            List<Warehouse> warehouseslist = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            Warehouse unactiveWarehouse = warehouseslist[1];// get unactive warehouse

            // act
            Warehouse result = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetWarehouseByIDURL, unactiveWarehouse.ID));

            // assert
            Assert.AreEqual(unactiveWarehouse.Name, result.Name);
            Assert.AreEqual(unactiveWarehouse.ID, result.ID);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Testing "Warehouse/GetActiveWarehouseByID" URL for getting a warehouse filter by ID controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetActiveWarehouseByID()
        {
            // arrange
            await WarehouseTestCommon.Insert6WarehousesForTesting(client);
            List<Warehouse> warehouseslist = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            Warehouse activeWarehouse = warehouseslist[0]; // get default active warehouse
            Warehouse unactiveWarehouse = warehouseslist[1]; // get default unactive warehouse

            // act;
            Warehouse result1 = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetActiveWarehouseByIDURL, activeWarehouse.ID));
            Warehouse result2 = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetActiveWarehouseByIDURL, unactiveWarehouse.ID));

            // assert
            Assert.AreEqual(activeWarehouse.Name, result1.Name);
            Assert.AreEqual(activeWarehouse.ID, result1.ID);
            Assert.AreEqual(null, result2);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Testing "Warehouse/UpdateWarehouse" URL for updating a warehouse controller method.
        /// </summary>
        [TestMethod]
        public async Task UpdateWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert3WarehousesForTesting(client);
            List<Warehouse> warehouseslist = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            Warehouse oldWarehouse = warehouseslist[2];
                        
            // act
            oldWarehouse.Name = "new-name";
            oldWarehouse.IsActive = false;      
            var response = await CommonCRUD.PutAPIRequest<Warehouse>(client, CommonCRUD.UpdateWarehouseURL, oldWarehouse);

            // assert
            Warehouse updatedWarehouse = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetWarehouseByIDURL, oldWarehouse.ID));
            Assert.AreEqual("new-name", updatedWarehouse.Name);
            Assert.IsFalse(updatedWarehouse.IsActive);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Changing a warehouse active status test. This method will be used in warehouse delete function.
        /// If success, warehouse activate status will be changed.
        /// </summary>
        [TestMethod]
        public async Task DeleteWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert3WarehousesForTesting(client);
            List<Warehouse> warehouseslist = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            Warehouse oldWarehouse = warehouseslist[2];

            // act
            HttpStatusCode deleteStatus = await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.DeleteWarehouseURL, oldWarehouse.ID));
            Warehouse deletedWarehouse = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetWarehouseByIDURL, oldWarehouse.ID));            

            // assert            
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.IsFalse(deletedWarehouse.IsActive);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
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
            await WarehouseTestCommon.Insert3WarehousesForTesting(client);
            List<Warehouse> warehouseslist = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            Warehouse unactiveWarehouse = warehouseslist[1]; // get unactive warehouse

            // act
            HttpStatusCode undeleteStatus = await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.UnDeleteWarehouseURL, unactiveWarehouse.ID));
            Warehouse undeletedWarehouse = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetActiveWarehouseByIDURL, unactiveWarehouse.ID));

            // assert            
            Assert.AreEqual(HttpStatusCode.OK, undeleteStatus);
            Assert.IsTrue(undeletedWarehouse.IsActive);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }

        /// <summary>
        /// Permanentely delete the warehouse
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HardDeleteWarehouse()
        {
            // prepare
            await WarehouseTestCommon.Insert3WarehousesForTesting(client);
            List<Warehouse> warehouseslist = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            Warehouse oldWarehouse = warehouseslist[2];

            // act
            HttpStatusCode deleteStatus = await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteWarehouseURL, oldWarehouse.ID));
            List<Warehouse> warehouses = await CommonCRUD.GetAPIRequest<List<Warehouse>>(client, CommonCRUD.GetAllWarehousesURL);
            var response = await CommonCRUD.GetAPIRequest<Warehouse>(client, string.Format(CommonCRUD.GetWarehouseByIDURL, oldWarehouse.ID));
            
            // assert            
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.AreEqual(2, warehouses.Count);
            Assert.AreEqual(null, response);

            // teardown
            await CommonCRUD.DeleteAllWarehouses(client);
        }       
        
        [TestCleanup]
        public void TestCleanup()
        {
            client.Dispose();
        }

    }
}