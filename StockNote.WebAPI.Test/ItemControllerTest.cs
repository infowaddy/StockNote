using Azure;
using Microsoft.VisualBasic;
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
    public class ItemControllerTest
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
        /// Testing "Item/CreateItem" URL for creating item controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateItem()
        {
            // arrange
            Unit unit = new Unit("box");
            unit.ID = await CommonCRUD.PostAPIRequest<Guid, Unit>(client, CommonCRUD.CreateUnitURL, unit);
            


            // act
            Item item = new Item() { Name = "Item", IsActive = true, Barcode = "34567", Unit = unit };
            Guid result = await CommonCRUD.PostAPIRequest<Guid, Item>(client, CommonCRUD.CreateItemURL, item);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteUnitURL, unit.ID));
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteItemURL, result));
        }      
        
        /// <summary>
        /// Testing "Item/GetItems" URL for getting active items only controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetItems()
        {
            // arrange            
            await ItemTestCommon.Insert3ItemsForTesting(client);
            
            // act
            List<Item> result = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetItemsURL);

            // assert 
            Assert.AreEqual(2, result.Count);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }
        /// <summary>
        /// Testing "Item/GetAllItems" URL for getting all items controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAllItems()
        {
            // arrange
            await ItemTestCommon.Insert3ItemsForTesting(client);

            // act
            List<Item> result = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);

            // assert
            Assert.AreEqual(3, result.Count);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }

        /// <summary>
        /// Testing "Item/GetItemsByName" URL for getting all items filter by name with sorting order controller method. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetItemsByName()
        {
            // arrange
            await ItemTestCommon.Insert6ItemsForTesting(client);

            // act
            List<Item> result = await CommonCRUD.GetAPIRequest<List<Item>>(client, string.Format(CommonCRUD.GetItemByNameURL, "A"));

            // assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("AC-4", result[2].Name); // sorting test

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }

        /// <summary>
        /// Testing "Item/GetItemByID" URL for getting a item filter by ID controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetItemByID()
        {
            // arrange
            await ItemTestCommon.Insert6ItemsForTesting(client);
            List<Item> itemslist = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            Item unactiveItem = itemslist[1];// get unactive item

            // act
            Item result = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetItemByIDURL, unactiveItem.ID));

            // assert
            Assert.AreEqual(unactiveItem.Name, result.Name);
            Assert.AreEqual(unactiveItem.ID, result.ID);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }

        /// <summary>
        /// Testing "Item/GetActiveItemByID" URL for getting a item filter by ID controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetActiveItemByID()
        {
            // arrange
            await ItemTestCommon.Insert6ItemsForTesting(client);
            List<Item> itemslist = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            Item activeItem = itemslist[0]; // get default active item
            Item unactiveItem = itemslist[1]; // get default unactive item

            // act
            Item result1 = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetActiveItemByIDURL,activeItem.ID));
            Item result2 = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetActiveItemByIDURL, unactiveItem.ID));

            // assert
            Assert.AreEqual(activeItem.Name, result1.Name);
            Assert.AreEqual(activeItem.ID, result1.ID);
            Assert.AreEqual(null, result2);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }
        /// <summary>
        /// Testing "Item/UpdateItem" URL for updating a item controller method.
        /// </summary>
        [TestMethod]
        public async Task UpdateItem()
        {
            // prepare
            await ItemTestCommon.Insert3ItemsForTesting(client);
            List<Item> itemslist = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            Item oldItem = itemslist[2];
                        
            // act
            oldItem.Name = "new-name";
            oldItem.IsActive = false;
            HttpStatusCode statusCode = await CommonCRUD.PutAPIRequest<Item>(client, CommonCRUD.UpdateItemURL, oldItem);

            // assert
            Item updatedItem = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetItemByIDURL, oldItem.ID));
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            Assert.AreEqual("new-name", updatedItem.Name);
            Assert.IsFalse(updatedItem.IsActive);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }
        /// <summary>
        /// Changing a item active status test. This method will be used in item delete function.
        /// If success, item activate status will be changed.
        /// </summary>
        [TestMethod]
        public async Task DeleteItem()
        {
            // prepare
            await ItemTestCommon.Insert3ItemsForTesting(client);
            List<Item> itemslist = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            Item oldItem = itemslist[2];

            // act
            HttpStatusCode statusCode = await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.DeleteItemURL, oldItem.ID));
            Item deletedItem = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetItemByIDURL, oldItem.ID));

            // assert            
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            Assert.IsFalse(deletedItem.IsActive);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
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
            await ItemTestCommon.Insert3ItemsForTesting(client);
            List<Item> itemslist = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            Item unactiveItem = itemslist[1]; // get unactive item

            // act
            HttpStatusCode statusCode = await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.UnDeleteItemURL, unactiveItem.ID));
            Item undeletedItem  = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetActiveItemByIDURL, unactiveItem.ID));
 

            // assert            
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            Assert.IsTrue(undeletedItem.IsActive);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }

        /// <summary>
        /// Permanentely delete the item
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HardDeleteItem()
        {
            // prepare
            await ItemTestCommon.Insert3ItemsForTesting(client);
            List<Item> itemslist = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            Item oldItem = itemslist[2];

            // act
            HttpStatusCode deleteStatus = await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteItemURL, oldItem.ID));
            List<Item> items = await CommonCRUD.GetAPIRequest<List<Item>>(client, CommonCRUD.GetAllItemsURL);
            var response = await CommonCRUD.GetAPIRequest<Item>(client, string.Format(CommonCRUD.GetItemByIDURL, oldItem.ID));
            
            // assert            
            Assert.AreEqual(HttpStatusCode.OK, deleteStatus);
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(null , response);

            // teardown
            await CommonCRUD.DeleteAllItems(client);
        }        
        [TestCleanup]
        public void TestCleanup()
        {
            client.Dispose();
        }

    }
}