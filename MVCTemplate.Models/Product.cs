using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTemplate.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Product Name")]
        public required string Name { get; set; }
        [DisplayName("Product Description")]
        public string? Description { get; set; }
        [DisplayName("Product Quantity")]
        public int Quantity { get; set; }
    }
}
