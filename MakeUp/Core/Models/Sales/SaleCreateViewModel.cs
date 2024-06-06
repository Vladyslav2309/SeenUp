﻿using System.ComponentModel.DataAnnotations;

namespace Core.Models.Sales
{
    public class SaleCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Image { get; set; }
        public string Description { get; set; }
        [Required]
        public int DecreasePercent { get; set; }
        [Required]
        public DateTime ExpireTime { get; set; }
    }
}
