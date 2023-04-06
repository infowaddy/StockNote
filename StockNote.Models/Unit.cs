using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StockNote.Models
{
    /// <summary>
    /// Unit table
    /// </summary>
    [Table("Units")]
    [Index(nameof(Name))]
    public class Unit : BasedModel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Unit() { }

        /// <summary>
        /// Constructor with name
        /// </summary>
        /// <param name="_name"></param>
        public Unit(string _name) { 
        this.Name = _name;
        }

        /// <summary>
        /// Unit name
        /// </summary>
        [Required]
        [Column("Name", TypeName = "nvarchar(50)")]
        public string Name { get; set; } 
    }
}
