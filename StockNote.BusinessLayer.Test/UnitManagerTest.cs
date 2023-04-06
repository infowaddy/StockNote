namespace StockNote.BusinessLayer.Test
{
    [TestClass]
    public class UnitManagerTest
    {
        private static UnitManager unitManager;
        private static StockNoteDBContext stockNoteDBContext = DatabaseInMemory.StockNoteDBContext;
        [TestInitialize]
        public void TestInitialize()
        {
            unitManager = new UnitManager(stockNoteDBContext, Common.GenerateLogger<UnitManager>());
        }

        /// <summary>
        /// Creating a unit test. 
        /// If success, newly inserted unit ID will be return with Guid format
        /// </summary>
        [TestMethod]
        public async Task CreateUnit()
        {
            // prepare
            Unit unit = new Unit();
            unit.Name = "Unit";
            unit.IsActive = true;

            // act
            Guid result = await unitManager.CreateUnit(unit);

            // assert
            Assert.AreNotEqual(Guid.Empty, result);

            // teardown
            await unitManager.DeleteUnit(unit.ID);
        }

        /// <summary>
        /// Retrieving active units test.
        /// If success, active 2 units will be return.
        /// </summary>
        [TestMethod]
        public async Task GetActiveUnitsOnly()
        {
            // prepare
            await UnitTestCommon.Insert3Units(unitManager);

            // act
            List<Unit> result1 = await unitManager.GetUnits();

            // assert
            Assert.AreEqual(2, result1.Count);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Retrieving all units test.
        /// If success, 3 units which including deleted also will be return.
        /// </summary>
        [TestMethod]
        public async Task GetAllUnitsIncludeDeleted()
        {
            // prepare
            await UnitTestCommon.Insert3Units(unitManager);
            
            // act
            List<Unit> result1 = await unitManager.GetUnits(true);

            // assert
            Assert.AreEqual(3, result1.Count);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Retrieving active units test.
        /// If success, 2 units will be return. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetUnits()
        {
            // prepare
            await UnitTestCommon.Insert3Units(unitManager);

            // act
            List<Unit> result1 = await unitManager.GetUnits();

            // assert
            Assert.AreEqual(2, result1.Count);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Getting units filter by name test.
        /// If success, active 3 units will be return and order by name.
        /// </summary>
        [TestMethod]
        public async Task GetUnitsByName()
        {
            // prepare
            await UnitTestCommon.Insert6Units(unitManager);

            // act
            List<Unit> result = await unitManager.GetUnits("A");

            // assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("AC-4", result[2].Name); // sorting test

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Getting a unit by ID.
        /// If success, one unit will be return.
        /// </summary>
        [TestMethod]
        public async Task GetUnitByID()
        {
            // prepare
            await UnitTestCommon.Insert3Units(unitManager);
            List<Unit> units = await UnitTestCommon.GetAllUnits(unitManager);
            Unit unactiveUnit = units[2]; // get default unactive unit from test data.

            // act
            Unit result = await unitManager.GetUnit(unactiveUnit.ID, true);
            // assert
            Assert.AreEqual(unactiveUnit, result);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }
        public async Task GetActiveUnitByID()
        {
            // prepare
            await UnitTestCommon.Insert6Units(unitManager);
            List<Unit> units = await UnitTestCommon.GetAllUnits(unitManager);
            Unit activeUnit = units[0];
            Unit unactiveUnit = units[1];

            // act
            Unit result1 = await unitManager.GetUnit(activeUnit.ID);
            Unit result2 = await unitManager.GetUnit(unactiveUnit.ID);
            // assert
            Assert.AreEqual(activeUnit, result1);
            Assert.IsNull(result2);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Updating an existing unit test.
        /// If success, updated unit will be return with new updated value.
        /// </summary>
        [TestMethod]
        public async Task UpdateUnit()
        {
            // prepare
            await UnitTestCommon.Insert6Units(unitManager);
            List<Unit> units = await UnitTestCommon.GetAllUnits(unitManager);
            Unit unit3 = units[3];
            Unit oldUnit = await unitManager.GetUnit(unit3.ID);

            // act
            oldUnit.Name = "new-name";
            oldUnit.IsActive = false;
            bool result = await unitManager.UpdateUnit(oldUnit);

            // assert
            Unit newUnit = await unitManager.GetUnit(unit3.ID, true);
            Assert.AreEqual("new-name", newUnit.Name);
            Assert.IsFalse(newUnit.IsActive);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Changing a unit active status test. This method will be used in unit delete function.
        /// If success, unit activate status will be changed.
        /// </summary>
        [TestMethod]
        public async Task DeleteUnit()
        {
            // prepare
            await UnitTestCommon.Insert3Units(unitManager);
            List<Unit> units = await UnitTestCommon.GetAllUnits(unitManager);
            // act
            await unitManager.ChangeActiveStatusOfUnit(units[0].ID, false);

            // assert
            List<Unit> result = await unitManager.GetUnits();
            Assert.AreEqual(1, result.Count);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
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
            await UnitTestCommon.Insert3Units(unitManager);
            List<Unit> units = await UnitTestCommon.GetAllUnits(unitManager);
            // act
            await unitManager.ChangeActiveStatusOfUnit(units[2].ID, true);

            // assert
            List<Unit> result = await unitManager.GetUnits();
            Assert.AreEqual(3, result.Count);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        /// <summary>
        /// Permanentely delete the unit
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task HardDeleteUnit()
        {
            // prepare
            await UnitTestCommon.Insert3Units(unitManager);
            List<Unit> units = await UnitTestCommon.GetAllUnits(unitManager);

            // act
            await unitManager.DeleteUnit(units[1].ID);
            
            // assert
            List<Unit> result = await unitManager.GetUnits();
            Assert.AreEqual(1, result.Count);

            // teardown
            await UnitTestCommon.DeleteAllUnits(unitManager);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            unitManager.Dispose();
        }
    }
}