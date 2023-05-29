using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPIDemo.EntityModels
{
    [Table("Users")]
    public class User : BaseEntity
    {
        public string Name { get; set; }
      
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string FullAddress { get; set; } 
    }
}
