using System.Data;

namespace Blog.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<Role> Roles { get; set; }
    }
}
