using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Blog.DAL.Models
{
    public class User
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public ICollection<Role> Roles { get; set; }
    }
}
