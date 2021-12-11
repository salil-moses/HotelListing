using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    /*
    public class CountryDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:50, ErrorMessage = "Country Name is too Long")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Short code is too Long")]
        public string ShortName { get; set; }
    }
    */

    public class CreateCountryDTO
    {        
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country Name is too Long")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Short code is too Long")]
        public string ShortName { get; set; }
    }

    public class CountryDTO: CreateCountryDTO
    {
        public int Id { get; set; }
        public IList<HotelDTO> Hotels { get; set; }
    }

    public class UpdateCountryDTO: CreateCountryDTO
    {
        public IList<CreateHotelDTO> Hotels { get; set; }
    }
}
