using StockNote.Models;

namespace StockNote.BusinessLayer.Interfaces
{
    public interface IItemManager
    {
        /// <summary>
        /// Create a new item.
        /// After creation is success, new item ID will be return. 
        /// Otherwise, it will return Guid.Empty.
        /// </summary>
        /// <param name="_item"></param>
        /// <returns></returns>
        Task<Guid> CreateItem(Item _item);
        
        /// <summary>
        /// Return null or one Item or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        Task<List<Item>> GetItems(bool _includeDelete = false);
        
        /// <summary>
        /// Return null or one Item or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_itemName"></param>
        /// <returns></returns>
        Task<List<Item>> GetItems(string _itemName, bool _includeDelete = false);
        
        /// <summary>
        /// Return null or one Item which match with parameter _itemID
        /// </summary>
        /// <param name="_itemID"></param>
        /// <returns></returns>
        Task<Item> GetItem(Guid _itemID, bool _includeDeleted = false);
        
        /// <summary>
        /// Return null or one Item which match with _barcode
        /// </summary>
        /// <param name="_barcode"></param>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        Task<Item> GetItem(string _barcode, bool _includeDelete = false);
        
        /// <summary>
        /// Return true or false based on success update
        /// </summary>
        /// <param name="_item"></param>
        /// <returns></returns>
        Task<bool> UpdateItem(Item _item);
        
        /// <summary>
        /// Deactive (delete) or active a unit. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result. 
        /// </summary>
        /// <param name="_itemID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        Task<bool> ChangeActiveStatusOfItem(Guid _itemID, bool _active = true);
        
        /// <summary>
        /// [Alert!] This will do hard delete a unit.
        /// This action cannot do role back. This is intended to use in unit test.
        /// </summary>
        /// <param name="_itemID"></param>
        /// <returns></returns>
        Task<bool> DeleteItem(Guid _itemID);
    }
}
