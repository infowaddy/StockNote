using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockNote.Models
{
    /// <summary>
    /// Stock item table
    /// </summary>
    [Table("Items")]
    [Index(nameof(Barcode), IsUnique = true)]
    [Index(nameof(Name))]
    public class Item : BasedModel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Item() { }   

        /// <summary>
        /// Construct with name, barcode and unit
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_barcode"></param>
        /// <param name="_unit"></param>
        public Item(string _name, string _barcode, Unit _unit) 
        {
            this.Name = _name;
            this.Barcode = _barcode;
            this.Unit = _unit;
        }

        /// <summary>
        /// Item name
        /// </summary>
        [Required]
        [Column("Name", TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        /// <summary>
        /// Support for 1D barcode code-39/ code-128
        /// </summary>
        [Required]
        [Column("Barcode", TypeName = "nvarchar(100)")]
        public string Barcode { get; set; }

        [Column("UnitID")]
        public Guid UnitID { get; set; }
        /// <summary>
        /// Counting unit object
        /// </summary>
        [Required]
        [ForeignKey("UnitID")]
        public Unit Unit { get; set; }
    }
}
