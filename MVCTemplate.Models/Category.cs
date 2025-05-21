using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCTemplate.Models
{
    public class Category
    {
        [Key]
        public int IdCategory { get; set; }
        [Required]
        [DisplayName("Category")]
        public required string NameCategory { get; set; }
        [DisplayName("Code")]
        public required string CodeCategory { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Category()
        {
            CreatedAt = DateTime.Now;
        }

        public void GenerateUpdatedAt() 
        {
            this.UpdatedAt = DateTime.Now;
        }
    }
}
