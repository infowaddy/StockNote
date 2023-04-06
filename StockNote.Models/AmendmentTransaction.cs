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

    [NotMapped]
    public class AmendmentTransaction
    {
        [NotMapped]
        public StockTransaction OldStockTransaction { get; set; }

        [NotMapped]
        public StockTransaction NewStockTransaction { get; set; }
    }
}
