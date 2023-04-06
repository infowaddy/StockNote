using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Net;

namespace StockNote.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("V{version:apiVersion}/[controller]s")]
    [ApiController]
    public class WarehouseController: ControllerBase
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
        [Route("")] // Post v1/warehouses
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
        [Route("{includeDeleted:bool?}")]
        public async Task<IActionResult> GetWarehouses(bool includeDeleted = false)
        {
            List<Warehouse> warehouses = await warehouseManager.GetWarehouses(includeDeleted);
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
        [Route("{name}/{includeDeleted?}")] // Get {name}/true
        public async Task<IActionResult> GetWarehousesByName(string name, bool includeDeleted=false)
        {
            List<Warehouse> warehouses = await warehouseManager.GetWarehouses(name, includeDeleted);
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
        [Route("{id:guid}/{includeDeleted:bool?}")]
        public async Task<IActionResult> GetWarehouseByID(Guid id,bool includeDeleted=false)
        {
            Warehouse warehouse = await warehouseManager.GetWarehouse(id, includeDeleted);
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
        [Route("")] // Put v1/warehouses
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
        [Route("{id:guid}/{undelete:bool?}")]
        public async Task<IActionResult> DeleteWarehouse(Guid id, bool undelete =false)
        {
            bool result = await warehouseManager.ChangeActiveStatusOfWarehouse(id, undelete);
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
        [Route("{id:guid}")]
        public async Task<IActionResult> HardDeleteWarehouse(Guid id)
        {
            bool result = await warehouseManager.DeleteWarehouse(id);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }
    }
}
