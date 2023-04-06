using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockNote.BusinessLayer.Interfaces;
using StockNote.DataAccess;
using StockNote.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StockNote.BusinessLayer
{
    public class UnitManager : IUnitManager, IDisposable
    {
        private StockNoteDBContext stockNoteDBContext;
        private ILogger<UnitManager> logger;

        /// <summary>
        /// Construstor with DBContext injection 
        /// </summary>
        /// <param name="_stockNoteDBContext"></param>
        public UnitManager(StockNoteDBContext _stockNoteDBContext, ILogger<UnitManager> _logger)
        {
            this.stockNoteDBContext = _stockNoteDBContext;
            this.logger = _logger;
        }

        /// <summary>
        /// Create a new unit.
        /// After creation is success, new unit ID will be return. 
        /// Otherwise, it will return Guid.Empty.
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> CreateUnit(Unit _unit)
        {
            try
            {
                this.stockNoteDBContext.Add<Unit>(_unit);
                await this.stockNoteDBContext.SaveChangesAsync();
                return _unit.ID;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "CreateUnit");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Return null or one Unit or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_includeDelete"></param>
        /// <returns></returns>
        public async Task<List<Unit>> GetUnits(bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Units.OrderBy(o => o.Name).ToListAsync<Unit>();
            else
                return await this.stockNoteDBContext.Units.Where<Unit>(x => x.IsActive == true).OrderBy(o=> o.Name).ToListAsync<Unit>();
        }
        
        /// <summary>
        /// Return null or one Unit or more sorting by name ascending based on search result
        /// </summary>
        /// <param name="_unitName"></param>
        /// <returns></returns>
        public async Task<List<Unit>> GetUnits(string _unitName, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Units.Where<Unit>(x => x.Name.ToLower().StartsWith(_unitName.ToLower())).OrderBy(o=> o.Name).ToListAsync();
            else
                return await this.stockNoteDBContext.Units.Where<Unit>(x => x.Name.ToLower().StartsWith(_unitName.ToLower()) && x.IsActive == true).OrderBy(o => o.Name).ToListAsync();
        }

        /// <summary>
        /// Return null or one Unit which match with parameter _unitID
        /// </summary>
        /// <param name="_unitID"></param>
        /// <returns></returns>
        public async Task<Unit> GetUnit(Guid _unitID, bool _includeDelete = false)
        {
            if (_includeDelete)
                return await this.stockNoteDBContext.Units.SingleOrDefaultAsync(x => x.ID == _unitID);
            else
                return await this.stockNoteDBContext.Units.SingleOrDefaultAsync(x => x.ID == _unitID && x.IsActive == true);
        }

        /// <summary>
        /// Return true or false based on success update
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdateUnit(Unit _unit)
        {
            try
            {
                Unit oldUnit = await this.stockNoteDBContext.Units.SingleOrDefaultAsync(x => x.ID.Equals(_unit.ID));
                this.stockNoteDBContext.Entry(oldUnit).CurrentValues.SetValues(_unit);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateUnit");
                return false;
            }

        }

        /// <summary>
        /// Deactive (delete) or active a unit. 
        /// Default is active which mean deactivate  = false
        /// Return true or false based on status change result.
        /// </summary>
        /// <param name="_unitID"></param>
        /// <param name="_active"></param>
        /// <returns></returns>
        public async Task<bool> ChangeActiveStatusOfUnit(Guid _unitID, bool _active = true)
        {
            try
            {
                Unit unit = await this.stockNoteDBContext.Units.SingleAsync<Unit>(x => x.ID == _unitID);
                unit.IsActive = _active;
                var result = await this.stockNoteDBContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) 
            {
                logger.LogError(ex, "ChangeActiveStatusOfUnit");
                return false;
            }
        }

        /// <summary>
        /// [Alert!] This will do hard delete a unit.
        /// This action cannot do role back. This is intended to use in unit test and other places where necessary to clean the data.
        /// </summary>
        /// <param name="_unitID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUnit(Guid _unitID)
        {
            try
            {
                Unit oldUnit = await this.stockNoteDBContext.Units.SingleOrDefaultAsync(x => x.ID.Equals(_unitID));
                this.stockNoteDBContext.Units.Remove(oldUnit);
                this.stockNoteDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteUnit");
                return false;
            }
        }

        public void Dispose()
        {

        }
    }
}
