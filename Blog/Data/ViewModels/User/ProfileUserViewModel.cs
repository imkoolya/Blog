﻿namespace Blog.Data.ViewModels.User
{
    public class ProfileUserViewModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}