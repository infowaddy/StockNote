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
    public class WarehouseController : ControllerBase
    {
        private IWarehouseManager warehouseManager;
        private ILogger<WarehouseController> logger;
        private readonly AppSettings appSettings;

        public WarehouseController(IWarehouseManager _warehouseManager, ILogger<WarehouseController> _logger, IOptions<AppSettings> _stockNoteSetting)
        {
            this.warehouseManager = _warehouseManager;
            this.logger = _logger;
            this.appSettings = _stockNoteSetting.Value;
        }

        /// <summary>
        /// Create a new warehouse.
        /// After creation is success, new warehouse ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="_warehouse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateWarehouse")]
        public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse _warehouse)
        {
            Guid warehouseID = await warehouseManager.CreateWarehouse(_warehouse);
            if (warehouseID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, warehouseID);
        }

        /// <summary>
        /// Getting warehouses which not including deleted warehouses.
        /// If success, warehouses list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWarehouses")]
        public async Task<IActionResult> GetWarehouses()
        {
            List<Warehouse> warehouses = await warehouseManager.GetWarehouses();
            if (warehouses != null && warehouses.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, warehouses);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting all warehouses which is including deleted warehouses.
        /// If success, warehouses list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllWarehouses")]
        public async Task<IActionResult> GetAllWarehouses()
        {
            List<Warehouse> warehouses = await warehouseManager.GetWarehouses(true);
            if (warehouses != null && warehouses.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, warehouses);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting warehouses which is filtering by warehouse name.
        /// If success, warehouses list order by name ascending will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWarehousesByName/{name}")]
        public async Task<IActionResult> GetWarehousesByName(string name)
        {
            List<Warehouse> warehouses = await warehouseManager.GetWarehouses(name);
            if (warehouses != null && warehouses.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, warehouses);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting active warehouse which is filtering by warehouse ID.
        /// If success, warehouse will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetActiveWarehouseByID/{id}")]
        public async Task<IActionResult> GetActiveWarehouseByID(string id)
        {
            Warehouse warehouse = await warehouseManager.GetWarehouse(new Guid(id));
            if (warehouse != null)
                return StatusCode((int)HttpStatusCode.OK, warehouse);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting active warehouse which is filtering by warehouse ID.
        /// If success, warehouse will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWarehouseByID/{id}")]
        public async Task<IActionResult> GetWarehouseByID(string id)
        {
            Warehouse warehouse = await warehouseManager.GetWarehouse(new Guid(id), true);
            if (warehouse != null)
                return StatusCode((int)HttpStatusCode.OK, warehouse);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Updating warehouse.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.        
        /// </summary>
        /// <param name="_warehouse"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateWarehouse")]
        public async Task<IActionResult> UpdateWarehouse([FromBody] Warehouse _warehouse)
        {
            bool result = await warehouseManager.UpdateWarehouse(_warehouse);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Updating warehouse IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("DeleteWarehouse/{id}")]
        public async Task<IActionResult> DeleteWarehouse(string id)
        {
            bool result = await warehouseManager.ChangeActiveStatusOfWarehouse(new Guid(id), false);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Updating warehouse IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("UnDeleteWarehouse/{id}")]
        public async Task<IActionResult> UnDeleteWarehouse(string id)
        {
            bool result = await warehouseManager.ChangeActiveStatusOfWarehouse(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Deleting warehouse.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("HardDeleteWarehouse/{id}")]
        public async Task<IActionResult> HardDeleteWarehouse(string id)
        {
            bool result = await warehouseManager.DeleteWarehouse(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }
    }
}
