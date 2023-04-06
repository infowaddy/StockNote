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
    public class UnitControllerTest
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
        /// Testing "Unit/CreateUnit" URL for creating unit controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateUnit()
        {
            // arrange
            Unit unit = new Unit(){Name= "Unit",IsActive = true};

            // act       
            Guid result = await CommonCRUD.PostAPIRequest<Guid, Unit>(client,CommonCRUD.CreateUnitURL,  unit);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteUnitURL, result));
        }

        /// <summary>
        /// Testing "Unit/GetUnits" URL for getting active units only controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetUnits()
        {
            // arrange            
            await UnitTestCommon.Insert3UnitsForTesting(client);
            
            // act
            List<Unit> result = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetUnitsURL);

            // assert 
            Assert.AreEqual(2, result.Count);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }

        /// <summary>
        /// Testing "Unit/GetAllUnits" URL for getting all units controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAllUnits()
        {
            // arrange
            await UnitTestCommon.Insert3UnitsForTesting(client);

            // act
            List<Unit> result = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);

            // assert
            Assert.AreEqual(3, result.Count);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }

        /// <summary>
        /// Testing "Unit/GetUnitsByName" URL for getting all units filter by name with sorting order controller method. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetUnitsByName()
        {
            // arrange
            await UnitTestCommon.Insert6UnitsForTesting(client);

            // act            
            List<Unit> result = await  CommonCRUD.GetAPIRequest<List<Unit>>(client, string.Format(CommonCRUD.GetUnitByNameURL, "A"));

            // assert 
            //Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("AC-4", result[2].Name); // sorting test

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }
        /// <summary>
        /// Testing "Unit/GetUnitByID" URL for getting a unit filter by ID controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetUnitByID()
        {
            // arrange
            await UnitTestCommon.Insert6UnitsForTesting(client);
            List<Unit> unitslist = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            Unit unactiveUnit = unitslist[1];// get unactive unit

            // act
            Unit result = await CommonCRUD.GetAPIRequest<Unit>(client,string.Format(CommonCRUD.GetUnitByIDURL, unactiveUnit.ID));
            
            // assert 
            Assert.AreEqual(unactiveUnit.Name, result.Name);
            Assert.AreEqual(unactiveUnit.ID, result.ID);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }
        /// <summary>
        /// Testing "Unit/GetActiveUnitByID" URL for getting a unit filter by ID controller method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetActiveUnitByID()
        {
            // arrange
            await UnitTestCommon.Insert6UnitsForTesting(client);
            List<Unit> unitslist =  await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            Unit activeUnit = unitslist[0]; // get default active unit
            Unit unactiveUnit = unitslist[1]; // get default unactive unit

            // act            
            Unit result1 = await CommonCRUD.GetAPIRequest<Unit>(client, string.Format(CommonCRUD.GetActiveUnitByIDURL, activeUnit.ID));
            var result2 = await CommonCRUD.GetAPIRequest<Unit>(client, string.Format(CommonCRUD.GetActiveUnitByIDURL, unactiveUnit.ID));

            // assert 
            // for active unit
            Assert.AreEqual(activeUnit.Name, result1.Name);
            Assert.AreEqual(activeUnit.ID, result1.ID);
            Assert.AreEqual(null, result2);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }
        /// <summary>
        /// Testing "Unit/UpdateUnit" URL for updating a unit controller method.
        /// </summary>
        [TestMethod]
        public async Task UpdateUnit()
        {
            // prepare
            await UnitTestCommon.Insert3UnitsForTesting(client);
            List<Unit> unitslist = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            Unit oldUnit = unitslist[2];
                        
            // act
            oldUnit.Name = "new-name";
            oldUnit.IsActive = false;
            await CommonCRUD.PutAPIRequest<Unit>(client, CommonCRUD.UpdateUnitURL, oldUnit);

            // assert
            Unit updatedUnit = await CommonCRUD.GetAPIRequest<Unit>(client, string.Format(CommonCRUD.GetUnitByIDURL, oldUnit.ID));

            Assert.AreEqual("new-name", updatedUnit.Name);
            Assert.IsFalse(updatedUnit.IsActive);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }
        /// <summary>
        /// Changing a unit active status test. This method will be used in unit delete function.
        /// If success, unit activate status will be changed.
        /// </summary>
        [TestMethod]
        public async Task DeleteUnit()
        {
            // prepare
            await UnitTestCommon.Insert3UnitsForTesting(client);
            List<Unit> unitslist = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            Unit oldUnit = unitslist[2];

            // act
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.DeleteUnitURL, oldUnit.ID));
            Unit deletedUnit = await CommonCRUD.GetAPIRequest<Unit>(client, string.Format(CommonCRUD.GetUnitByIDURL, oldUnit.ID));
           
            // assert
            Assert.IsFalse(deletedUnit.IsActive);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }
        /// <summary>
        /// Changing a unit active status test. This method will be used in unit un delete function.
        /// If success, unit activate status will be changed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UnDeleteUnit()
        {
            // prepare
            await UnitTestCommon.Insert3UnitsForTesting(client);
            List<Unit> unitslist = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            Unit unactiveStockUnit = unitslist[1]; // get unactive unit

            // act
            await CommonCRUD.PatchAPIRequest(client, string.Format(CommonCRUD.UnDeleteUnitURL, unactiveStockUnit.ID));
            Unit undeletedUnit = await CommonCRUD.GetAPIRequest<Unit>(client, string.Format(CommonCRUD.GetActiveUnitByIDURL, unactiveStockUnit.ID));

            // assert
            Assert.IsTrue(undeletedUnit.IsActive);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }
        /// <summary>
        /// Permanentely delete the unit
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HardDeleteUnit()
        {
            // prepare
            await UnitTestCommon.Insert3UnitsForTesting(client);
            List<Unit> unitslist = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            Unit oldUnit = unitslist[2];

            // act
            await CommonCRUD.DeleteAPIRequest(client, string.Format(CommonCRUD.HardDeleteUnitURL, oldUnit.ID));
            List<Unit> units = await CommonCRUD.GetAPIRequest<List<Unit>>(client, CommonCRUD.GetAllUnitsURL);
            var response = await CommonCRUD.GetAPIRequest<Unit>(client, string.Format(CommonCRUD.GetUnitByIDURL, oldUnit.ID));
            
            // assert
            Assert.AreEqual(2, units.Count);
            Assert.AreEqual(null, response);

            // teardown
            await CommonCRUD.HardDeleteAllUnits(client);
        }        
        [TestCleanup]
        public void TestCleanup()
        {
            client.Dispose();
        }

    }
}