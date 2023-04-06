using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StockNote.Models
{
    /// <summary>
    /// Warehouse table
    /// </summary>
    [Table("Warehouses")]
    [Index(nameof(Name))]
    public class Warehouse:BasedModel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Warehouse(){ }

        /// <summary>
        /// Construct with name and address
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_address"></param>
        public Warehouse(string _name, string _address) {
            this.Name = _name;
            this.Address = _address;
        }

        /// <summary>
        /// Warehouse name
        /// </summary>
        [Required]
        [Column("Name", TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        /// <summary>
        /// Warehouse address
        /// </summary>
        [Column("Address", TypeName = "nvarchar(200)")]
        public string Address { get; set; }

    }
}
