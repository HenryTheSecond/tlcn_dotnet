﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class Product
    {
        public Product()
        {
            ProductImages = new List<ProductImage>();
        }

        [Key]
        public long? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Decimal? Price { get; set; }

        [Column(TypeName = "varchar(255)")]
        public ProductUnit Unit { get; set; } = ProductUnit.UNIT;

        public double? MinPurchase { get; set; } = 1;
        [Column(TypeName = "varchar(50)")]
        public ProductStatus Status { get; set; }

        public string? Description { get; set; }
        public double? Quantity { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public double Sales { get; set; } = 0;

        public double SalesUntilCheckExpire { get; set; } = 0;
        [DefaultValue(0)]
        public int Weight { get; set; } = 0;

        public virtual Category? Category { get; set; }
        
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; }

        public virtual ICollection<CartDetail> CartDetails { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}
