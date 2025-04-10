using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCTemplate.Models
{
    public class ProductModel
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
