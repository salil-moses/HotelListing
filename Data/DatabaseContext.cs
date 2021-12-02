using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    // public class DatabaseContext: DbContext
    // public class DatabaseContext : IdentityDbContext  // this makes use of the built in IdentityUser class
    // Below we are using the customized ApiUser class
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }              

        public DbSet<Country> Countries { get; set; }  // Countries is the DB Table name

        public DbSet<Hotel> Hotels { get; set; }

        /*For seeding some data*/
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*Add this line after installing the identity core package*/
            base.OnModelCreating(builder);
            
            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new HotelConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());

            /*This is shifted out*/
            /*
            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Jamaica",
                    ShortName = "JM"
                },
                new Country
                {
                    Id = 2,
                    Name = "Bahamas",
                    ShortName = "BS"
                },
                new Country
                {
                    Id = 3,
                    Name = "Cayman Islands",
                    ShortName = "CI"
                }
            );            

            builder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Sandals Resort and Spa",
                    Address = "Negril",
                    Rating = 4.5,
                    CountryId = 1
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Grand Palladium",
                    Address = "Nassau",
                    Rating = 4,
                    CountryId = 2
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Comfort Suites",
                    Address = "George Town",
                    Rating = 4.3,
                    CountryId = 3
                }
            );
            */
        }

    }
}
