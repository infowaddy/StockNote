using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Linq.Expressions;
using System.Net;

namespace StockNote.WebAPI.Controllers.V0
{
    [ApiVersion("0", Deprecated = true)]
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private IItemManager itemManager;
        private ILogger<ItemController> logger;
        private readonly AppSettings appSettings;
        public ItemController(IItemManager _itemManager, ILogger<ItemController> _logger, IOptions<AppSettings> _stockNoteSetting)
        {
            this.itemManager = _itemManager;
            this.logger = _logger;
            this.appSettings = _stockNoteSetting.Value;
        }

        /// <summary>
        /// Create a new item.
        /// After creation is success, new item ID will be return with HttpStatusCode 201 for Created.
        /// Otherwise, it will return HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <param name="_item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateItem")]
        public async Task<IActionResult> CreateItem([FromBody] Item _item)
        {
            Guid itemID = await itemManager.CreateItem(_item);
            if (itemID == Guid.Empty)
                return StatusCode((int)HttpStatusCode.ExpectationFailed, itemID);
            else
                return StatusCode((int)HttpStatusCode.Created, itemID);
        }

        /// <summary>
        /// Getting items which not including deleted items.
        /// If success, items list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItems")]
        public async Task<IActionResult> GetItems()
        {
            List<Item> items = await itemManager.GetItems();
            if (items != null && items.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, items);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting all items which is including deleted items.
        /// If success, items list will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            List<Item> items = await itemManager.GetItems(true);
            if (items != null && items.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, items);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting items which is filtering by item name.
        /// If success, items list order by name ascending will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItemsByName/{name}")]
        public async Task<IActionResult> GetItemsByName(string name)
        {
            List<Item> items = await itemManager.GetItems(name);
            if (items != null && items.Count > 0)
                return StatusCode((int)HttpStatusCode.OK, items);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting active item which is filtering by item ID.
        /// If success, item will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetActiveItemByID/{id}")]
        public async Task<IActionResult> GetActiveItemByID(string id)
        {
            Item item = await itemManager.GetItem(new Guid(id));
            if (item != null)
                return StatusCode((int)HttpStatusCode.OK, item);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Getting active item which is filtering by item ID.
        /// If success, item will be return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return HttpStatusCode 204 for processing success but no content.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("GetItemByID/{id}")]
        public async Task<IActionResult> GetItemByID(string id)
        {
            Item item = await itemManager.GetItem(new Guid(id), true);
            if (item != null)
                return StatusCode((int)HttpStatusCode.OK, item);
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Updating item.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.        
        /// </summary>
        /// <param name="_item"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateItem")]
        public async Task<IActionResult> UpdateItem([FromBody] Item _item)
        {
            bool result = await itemManager.UpdateItem(_item);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Updating item IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("DeleteItem/{id}")]
        public async Task<IActionResult> DeleteItem(string id)
        {
            bool result = await itemManager.ChangeActiveStatusOfItem(new Guid(id), false);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Updating item IsActivate status.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("UnDeleteItem/{id}")]
        public async Task<IActionResult> UnDeleteItem(string id)
        {
            bool result = await itemManager.ChangeActiveStatusOfItem(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// Deleting item.
        /// If success, It will return with HttpStatusCode 200 for processing Ok.
        /// Otherwise, it will return  HttpStatusCode 417 for ExpectationFailed.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("HardDeleteItem/{id}")]
        public async Task<IActionResult> HardDeleteItem(string id)
        {
            bool result = await itemManager.DeleteItem(new Guid(id));
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

    }
}
