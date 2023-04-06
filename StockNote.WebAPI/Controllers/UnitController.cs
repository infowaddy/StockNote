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
    public class UnitController : ControllerBase
    {
        private IUnitManager unitManager;
        private ILogger<UnitController> logger;
        private readonly AppSettings appSettings;
        public UnitController(IUnitManager _unitManager, ILogger<UnitController> _logger, IOptions<AppSettings> _stockNoteSetting)
        {
            this.unitManager = _unitManager;
            this.logger = _logger;
            this.appSettings = _stockNoteSetting.Value;
        }

        /// <summary>
        /// Create a new unit.
        /// After creation is success, new unit ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")] // Post v1/units
        public async Task<IActionResult> CreateUnit([FromBody] Unit _unit)
        {
            Guid unitID = await unitManager.CreateUnit(_unit);
            if (unitID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
            else
                return StatusCode((int)HttpStatusCode.Created, unitID);
        }

        /// <summary>
        /// Getting units which not including deleted units.
        /// If success, units list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{includeDeleted:bool?}")] // Get v1/units
        public async Task<IActionResult> GetUnits(bool includeDeleted = false)
        {
            List<Unit> units = await unitManager.GetUnits(includeDeleted);
            if (units != null && units.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, units);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting units which is filtering by unit name.
        /// If success, units list order by name ascending will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{name:alpha}/{includeDeleted:bool?}")]
        public async Task<IActionResult> GetUnitsByName(string name, bool includeDeleted = false)
        {
            List<Unit> units = await unitManager.GetUnits(name, includeDeleted);
            if (units != null && units.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, units);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting active unit which is filtering by unit ID.
        /// If success, unit will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}/{includeDeleted:bool?}")]
        public async Task<IActionResult> GetUnitByID(Guid id, bool includeDeleted = false)
        {
            Unit unit = await unitManager.GetUnit(id, includeDeleted);
            if (unit != null)
                return StatusCode((int)HttpStatusCode.OK, unit);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Updating unit.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.        
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")] // Put v1/units
        public async Task<IActionResult> UpdateUnit([FromBody] Unit _unit)
        {
            bool result = await unitManager.UpdateUnit(_unit);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }
        /// <summary>
        /// Updating unit IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("{id:guid}/{includeDeleted:bool?}")] // Patch v1/units/
        public async Task<IActionResult> DeleteUnit(Guid id, bool includeDeleted = false)
        {
            bool result = await unitManager.ChangeActiveStatusOfUnit(id, includeDeleted);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }        

        /// <summary>
        /// Deleting unit.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")] // Delete v1/Units
        public async Task<IActionResult> HardDeleteUnit(Guid id)
        {
            bool result = await unitManager.DeleteUnit(id);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }
    }
}
