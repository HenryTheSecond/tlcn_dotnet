﻿using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    public class Supplier
    {
        [Key]
        public long? Id { get; set; }

        public string Name { get; set; }

        public string? CountryCode { get; set; }

        public string? CityCode { get; set; }

        public string? DetailLocation { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}
