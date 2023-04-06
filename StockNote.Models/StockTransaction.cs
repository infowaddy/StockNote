using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockNote.Models
{
    /// <summary>
    /// Stock transaction history table
    /// </summary>
    [Table("StockTransactions")]
    [Index(nameof(TransactionDate), IsDescending = new[] {true} )]
    public class StockTransaction:BasedModel
    {
        /// <summary>
        /// Foreign key
        /// </summary>        
        [Column("ItemID")]
        public Guid ItemID { get; set; }

        /// <summary>
        /// Link object to item
        /// </summary>
        [Required]
        [ForeignKey("ItemID")]
        public Item  Item {get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        [Column("Quantity", TypeName = "decimal(8, 2)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Flag of sale transaction where no to warehouse
        /// </summary>
        [Column("IsSold", TypeName = "bit")]
        [DefaultValue(false)]
        public bool IsSold { get; set; } = false;

        /// <summary>
        /// Foreign key
        /// </summary>        
        [Column("FromWarehouseID")]
        public Guid FromWarehouseID { get; set; }

        /// <summary>
        /// Link object to warehouse to detect amount from balance
        /// </summary>
        [Required]
        [ForeignKey("FromWarehouseID")]
        public Warehouse FromWarehouse { get; set; }

        /// <summary>
        /// Foreign key
        /// </summary>
        [Column("ToWarehouseID")]
        public Guid? ToWarehouseID { get; set; }

        /// <summary>
        /// Link object to warehouse to add amount to balance
        /// </summary>
        [ForeignKey("ToWarehouseID")]
        public Warehouse ToWarehouse { get; set; }

        /// <summary>
        /// Remark for some notes
        /// </summary>
        [Column("Remark", TypeName = "nvarchar(500)")]
        public string Remark { get; set; }

        /// <summary>
        /// Transaction date which can be back date
        /// </summary>
        [Column("TransactionDate", TypeName = "datetime")]
        public DateTime TransactionDate { get; set; }


    }
}
