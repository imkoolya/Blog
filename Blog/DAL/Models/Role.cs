namespace Blog.DAL.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required ICollection<User> Users { get; set; }
    }
}
