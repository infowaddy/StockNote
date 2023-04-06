using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Net;

namespace StockNote.WebAPI.Controller.V0
{
    [ApiVersion("0", Deprecated = true)]
    [ApiController]
    [Route("[controller]")]
    public class StockInWarehouseController : ControllerBase
    {
        private IStockInWarehouseManager stockInWarehouseManager;
        private ILogger<StockInWarehouseController> logger;
        private readonly AppSettings appSettings;
        public StockInWarehouseController(IStockInWarehouseManager _stockInWarehouseManager, ILogger<StockInWarehouseController> _logger, IOptions<AppSettings> _stockNoteSetting)
        {
            this.stockInWarehouseManager = _stockInWarehouseManager;
            this.logger = _logger;
            this.appSettings = _stockNoteSetting.Value;
        }

        /// <summary>
        /// Add a stock to warehouse, this is openning stock.
        /// After adding is success, new stock-in-warehouse ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="stockInWarehouse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddStockToWarehouse")]
        public async Task<IActionResult> AddStockToWarehouse([FromBody] StockInWarehouse stockInWarehouse)
        {
            Guid stockInWarehouseID = await stockInWarehouseManager.AddStockToWarehouse(stockInWarehouse);
            if (stockInWarehouseID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, stockInWarehouseID);
        }

        /// <summary>
        /// Updating stock in warehouse.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="stockInWarehouse"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateStockInWarehouse")]
        public async Task<IActionResult> UpdateStockInWarehouse([FromBody] StockInWarehouse stockInWarehouse)
        {
            bool result = await stockInWarehouseManager.UpdateStockInWarehouse(stockInWarehouse);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Updating minimum threshold of stock in warehouse.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="warehouseID"></param>
        /// <param name="minimumThreshold"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("UpdateMinimumThresholdOfStock")]
        public async Task<IActionResult> UpdateMinimumThresholdOfStock([FromQuery] Guid itemID, [FromQuery] Guid warehouseID, [FromQuery] Decimal minimumThreshold)
        {
            bool result = await stockInWarehouseManager.UpdateMinimumThresholdOfStock(itemID, warehouseID, minimumThreshold);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Getting warehouse list by item which not including deleted warehouse.
        /// If success, warehouse list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content. 
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWarehouseListByItem/{itemID}")]
        public async Task<IActionResult> GetWarehouseListByItem(Guid itemID)
        {
            List<Warehouse> warehouses = await stockInWarehouseManager.GetWarehouseListByItem(itemID);
            if (warehouses != null && warehouses.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, warehouses);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting item list by wahrehouse which not including deleted items.
        /// If success, item list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.  
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStockItemListByWarehouse/{warehouseID}")]
        public async Task<IActionResult> GetStockItemListByWarehouse(Guid warehouseID)
        {
            List<Item> items = await stockInWarehouseManager.GetStockItemListByWarehouse(warehouseID);
            if (items != null && items.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, items);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting stock-in-warehouse list which is not including deleted stock items from warehouses.
        /// If success, stock-in-warehouse list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStockInWarehouses")]
        public async Task<IActionResult> GetStockInWarehouses()
        {
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses();
            if (stockInWarehouseList != null && stockInWarehouseList.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, stockInWarehouseList);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Get stock-in-warehouse record by ItemID and WarehouseID
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStockInWarehouse")]
        public async Task<IActionResult> GetStockInWarehouse([FromQuery] Guid itemID, [FromQuery] Guid warehouseID)
        {
            StockInWarehouse stockInWarehouse = await stockInWarehouseManager.GetStockInWarehouse(itemID, warehouseID);
            if (stockInWarehouse != null)
                return StatusCode((int)HttpStatusCode.OK, stockInWarehouse);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting all stock-in-warehouse list which including deleted stock items from warehouses.
        /// If success, stock-in-warehouse list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllStockInWarehouses")]
        public async Task<IActionResult> GetAllStockInWarehouses()
        {
            List<StockInWarehouse> stockInWarehouseList = await stockInWarehouseManager.GetStockInWarehouses(true);
            if (stockInWarehouseList != null && stockInWarehouseList.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, stockInWarehouseList);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Updating stock-in-warehouse IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("DeleteStockInWarehouse/{id}")]
        public async Task<IActionResult> DeleteStockInWarehouse(string id)
        {
            bool result = await stockInWarehouseManager.ChangeActiveStatusOfStockInWarehouse(new Guid(id), false);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Updating stock-in-warehouse IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("UnDeleteStockInWarehouse/{id}")]
        public async Task<IActionResult> UnDeleteStockInWarehouse(string id)
        {
            bool result = await stockInWarehouseManager.ChangeActiveStatusOfStockInWarehouse(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Deleting stock-in-warehouse.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("HardDeleteStockInWarehouse/{id}")]
        public async Task<IActionResult> HardDeleteStockInWarehouse(string id)
        {
            bool result = await stockInWarehouseManager.DeleteStockInWarehouse(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }
    }
}
