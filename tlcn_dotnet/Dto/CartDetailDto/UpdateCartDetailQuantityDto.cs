﻿using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.CartDetailDto
{
    public class UpdateCartDetailQuantityDto
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        public double Quantity { get; set; }
    }
}
