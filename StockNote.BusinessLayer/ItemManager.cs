using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockNote.BusinessLayer.Interfaces;
using StockNote.DataAccess;
using StockNote.Models;

// Ref: For knowledge
// https://stackoverflow.com/questions/25441027/how-do-i-stop-entity-framework-from-trying-to-save-insert-child-objects
// https://learn.microsoft.com/en-us/ef/core/querying/related-data/
namespace StockNote.BusinessLayer
{
    public class ItemManager : IItemManager, IDisposable
    {
        private StockNoteDBContext stockNoteDBContext;
        private ILogger<ItemManager> logger;

        /// <summary>
        /// Construstor with DBContext injection 
        /// </summary>
        /// <param name="_stockNoteDBContext"></param>
        public ItemManager(StockNoteDBContext  _stockNoteDBContext, ILogger<ItemManager> _logger) 
        {
            this.stockNoteDBContext = _stockNoteDBContext;
            this.logger = _logger;
        }

        /// <summary>
        /// Create a new item.
        /// After creation is success, new item ID will be return. 
        /// Otherwise, it will return Guid.Empty.
        /// </summary>
        /// <param name="_item"></param>
        /// <returns></returns>
        public async Task<Guid> CreateItem(Item _item)
        {            
            try
            {
                // avoid duplicate child objects creation
                Unit unit = this.stockNoteDBContext.Units.Where(x => x.ID == _item.Unit.ID).FirstOrDefault();
                if (unit != null)
                {
                    _item.Unit = unit;
                }

                this.stockNoteDBContext.Add<Item>(_item);
                await this.stockNoteDBContext.SaveChangesAsync();
                return _item.ID;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateItem");
                return Guid.Empty;
            }
        }

        /// <summary>
        ///  Return null or one Item or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Item>> GetItems(bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Items.OrderBy(o => o.Name)
                    .Include(x=> x.Unit)
                    .ToListAsync<Item>();
            else
                return await this.stockNoteDBContext.Items.Where<Item>(x => x.IsActive == true).OrderBy(o => o.Name)
                    .Include(x => x.Unit)
                    .ToListAsync<Item>();
        }

        /// <summary>
        /// Return null or one Item or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_itemName"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Item>> GetItems(string _itemName, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Items.Where<Item>(x => x.Name.ToLower().StartsWith(_itemName.ToLower())).OrderBy(o => o.Name)
                    .Include(x => x.Unit)
                    .ToListAsync();
            else
                return await this.stockNoteDBContext.Items.Where<Item>(x => x.Name.ToLower().StartsWith(_itemName.ToLower()) && x.IsActive == true).OrderBy(o => o.Name)
                    .Include(x => x.Unit)
                    .ToListAsync();
        }

        /// <summary>
        /// Return null or one Unit which match with parameter _unitID
        /// </summary>
        /// <param name="_itemID"></param>
        /// <param name="_includeDeleted"></param>
        /// <returns></returns>
        public async Task<Item> GetItem(Guid _itemID, bool _includeDeleted = false)
        {
            if (_includeDeleted)
                return await this.stockNoteDBContext.Items.Include(x => x.Unit).SingleOrDefaultAsync(x => x.ID == _itemID);
            else
                return await this.stockNoteDBContext.Items.Include(x => x.Unit).SingleOrDefaultAsync(x => x.ID == _itemID && x.IsActive == true);
        }

        /// <summary>
        /// Return null or one Unit which match with parameter _barcode
        /// </summary>
        /// <param name="_barcode"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<Item> GetItem(string _barcode, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Items.Include(x => x.Unit).SingleOrDefaultAsync(x => x.Barcode.ToLower() == _barcode.ToLower());
            else
                return await this.stockNoteDBContext.Items.Include(x => x.Unit).SingleOrDefaultAsync(x => x.Barcode.ToLower() == _barcode.ToLower() && x.IsActive == true);
        }

        /// <summary>
        /// Return true or false based on success update
        /// </summary>
        /// <param name="_item"></param>
        /// <returns></returns>
        public async Task<bool> UpdateItem(Item _item)
        {
            try
            {
                Item oldItem = await this.stockNoteDBContext.Items.SingleOrDefaultAsync(x => x.ID.Equals(_item.ID));
                this.stockNoteDBContext.Entry(oldItem).CurrentValues.SetValues(_item);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateItem");
                return false;
            }
        }

        /// <summary>
        /// Deactive (delete) or active a unit. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result.
        /// </summary>
        /// <param name="_itemID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        public async Task<bool> ChangeActiveStatusOfItem(Guid _itemID, bool _active = true)
        {
            try
            {
                Item item = await this.stockNoteDBContext.Items.SingleAsync<Item>(x => x.ID == _itemID);
                item.IsActive = _active;
                var result = await this.stockNoteDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ChangeActiveStatusOfItem");
                return false;
            }
        }

        /// <summary>
        /// [Alert!] This will do hard delete a item.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data.
        /// </summary>
        /// <param name="_itemID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteItem(Guid _itemID)
        {
            try
            {
                Item oldItem = await this.stockNoteDBContext.Items.SingleOrDefaultAsync(x => x.ID.Equals(_itemID));
                this.stockNoteDBContext.Items.Remove(oldItem);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteItem");
                return false;
            }
        }
        public void Dispose()
        {

        }

    }
}