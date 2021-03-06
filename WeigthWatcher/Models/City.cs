//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WeigthWatcher.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> TimezoneId { get; set; }
        public int CountryId { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual Region Region { get; set; }
        public virtual Timezone Timezone { get; set; }
    }
}
