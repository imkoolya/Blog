using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models
{
    public class User : IdentityUser
    {
        public List<Article> Articles { get; set; } = new List<Article> { };
        public List<Comment> Comments { get; set; } = new List<Comment> { };
    }
}
