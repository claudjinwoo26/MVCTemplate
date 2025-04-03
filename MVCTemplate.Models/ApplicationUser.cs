using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTemplate.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public bool? isRestricted { get; set; }
    }
}
