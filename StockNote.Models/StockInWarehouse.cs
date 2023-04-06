using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockNote.Models
{
    /// <summary>
    /// Stock in warehouse table
    /// </summary>    
    [Table("StockInWarehouses")]
    [PrimaryKey(nameof(ItemID), nameof(WarehouseID))]
    [Index(nameof(WarehouseID))]
    public class StockInWarehouse:BasedModel
    {
        /// <summary>
        /// Foreign key of Item and alternative composite keys of StockInWarehouse
        /// </summary>
        [Column("ItemID")]
        public Guid ItemID { get; set; }

        /// <summary>
        /// Link object to item
        /// </summary>
        [Required]
        [ForeignKey("ItemID")]
        public Item Item { get; set; }

        /// <summary>
        /// Foreign key of Warehouse and alternative composite keys of StockInWarehouse
        /// </summary>
        [Column("WarehouseID")]
        public Guid WarehouseID { get; set; }

        /// <summary>
        /// Link object to warehouse
        /// </summary>
        [Required]
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }

        /// <summary>
        /// Stock balance in the warehouse
        /// </summary>
        [Column("Balance", TypeName = "decimal(8, 2)")]
        public decimal Balance { get; set; }

        /// <summary>
        /// Minimum balance threshold to give alert if stock balance is lower than this amount
        /// </summary>
        [Column("MinBalThreshold", TypeName = "decimal(8, 2)")]
        public decimal MinBalThreshold { get; set; }

        /// <summary>
        /// Flag for item which stock balance is lower than minimum threshold
        /// </summary>
        [NotMapped]
        public bool IsLowerThanMinBal 
        { get
            {
                if (this.Balance < MinBalThreshold)
                    return true;
                else
                    return false;          
            }
        }
    }
}
