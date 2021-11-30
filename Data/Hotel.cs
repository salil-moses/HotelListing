using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }

        [ForeignKey(nameof(Country))]  // ForeignKey("fk_countries_table") also is good        
        public int CountryId { get; set; }
        
        public Country Country { get; set; } // This prop is not added to the DB, it just fills in the related country data
        

    }
}
