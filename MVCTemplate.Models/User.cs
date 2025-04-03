using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTemplate.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("First Name")]
        public required string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public required string LastName { get; set; }
        [DisplayName("Middle Name")]
        public string? MiddleName { get; set; }
    }
}
