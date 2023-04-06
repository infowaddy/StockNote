using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNote.BusinessLayer;
using StockNote.BusinessLayer.Interfaces;
using StockNote.Models;
using System.Linq.Expressions;
using System.Net;

namespace StockNote.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("V{version:apiVersion}/[controller]s")]
    [ApiController]
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
        [Route("")] // Post v1/items
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
        [Route("{includeDeleted:bool?}")] // Get v1/items, Get v1/items/true
        public async Task<IActionResult> GetItems(bool includeDeleted = false)
        {
            List<Item> items = await itemManager.GetItems(includeDeleted);
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
        [Route("{name:alpha}/{includeDeleted:bool?}")]
        public async Task<IActionResult> GetItemsByName(string name, bool includeDeleted = false)
        {
            List<Item> items = await itemManager.GetItems(name, includeDeleted);
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
        [Route("{id:guid}/{includeDeleted:bool?}")] //Get v1/items/{id}/true
        public async Task<IActionResult> GetItemByID(Guid id, bool includeDeleted = false)
        {
            Item item = await itemManager.GetItem(id, includeDeleted);
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
        [Route("")] // Put v1/items
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
        [Route("{id:guid}/{undelete:bool?}")] // Patch v1/items/{id}/true
        public async Task<IActionResult> DeleteItem(Guid id, bool undelete = false)
        {
            bool result = await itemManager.ChangeActiveStatusOfItem(id, undelete);
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
        [Route("{id:guid}")] // Delete v1/items/{id}
        public async Task<IActionResult> HardDeleteItem(Guid id)
        {
            bool result = await itemManager.DeleteItem(id);
            if (result)
                return StatusCode((int)HttpStatusCode.OK);
            else
                return StatusCode((int)HttpStatusCode.ExpectationFailed);
        }

    }
}
