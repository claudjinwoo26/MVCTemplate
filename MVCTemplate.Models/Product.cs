using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Product()
        {
            CreatedAt = DateTime.Now;
        }

        public void GenerateUpdatedAt() 
        {
            this.UpdatedAt = DateTime.Now;
        }
    }
}
