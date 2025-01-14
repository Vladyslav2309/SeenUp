﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infastructure.Entities
{
    [Table("tblOrderStatuses")]
    public class OrderStatusEntity : BaseEntity<int>
    {
        [Required, StringLength(255)]
        public string Name { get; set; }
        public virtual ICollection<OrderEntity> Orders { get; set; }
    }
}
