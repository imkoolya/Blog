﻿namespace Blog.DAL.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
