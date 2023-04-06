using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Net;

namespace StockNote.WebAPI.Controllers.V0
{
    [ApiVersion("0", Deprecated = true)]
    [ApiController]
    [Route("[controller]")]
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
        [Route("CreateUnit")]
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
        [Route("GetUnits")]
        public async Task<IActionResult> GetUnits()
        {
            List<Unit> units = await unitManager.GetUnits();
            if (units != null && units.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, units);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }
        /// <summary>
        /// Getting all units which including deleted units.
        /// If success, units list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllUnits")]
        public async Task<IActionResult> GetAllUnits()
        {
            List<Unit> units = await unitManager.GetUnits(true);
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
        [Route("GetUnitsByName/{name}")]
        public async Task<IActionResult> GetUnitsByName(string name)
        {
            List<Unit> units = await unitManager.GetUnits(name);
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
        [Route("GetActiveUnitByID/{id}")]
        public async Task<IActionResult> GetActiveUnitByID(string id)
        {
            Unit unit = await unitManager.GetUnit(new Guid(id));
            if (unit != null)
                return StatusCode((int)HttpStatusCode.OK, unit);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }
        /// <summary>
        /// Getting active unit which is filtering by unit ID.
        /// If success, unit will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUnitByID/{id}")]
        public async Task<IActionResult> GetUnitByID(string id)
        {
            Unit unit = await unitManager.GetUnit(new Guid(id), true);
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
        [Route("UpdateUnit")]
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
        [Route("DeleteUnit/{id}")]
        public async Task<IActionResult> DeleteUnit(string id)
        {
            bool result = await unitManager.ChangeActiveStatusOfUnit(new Guid(id), false);
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
        [Route("UnDeleteUnit/{id}")]
        public async Task<IActionResult> UnDeleteUnit(string id)
        {
            bool result = await unitManager.ChangeActiveStatusOfUnit(new Guid(id));
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
        [Route("HardDeleteUnit/{id}")]
        public async Task<IActionResult> HardDeleteUnit(string id)
        {
            bool result = await unitManager.DeleteUnit(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }
    }
}
