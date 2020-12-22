using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shos.Chatter.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime InsertDateTime { get; set; }
        public bool HasDeleted { get; set; } = false;
        public virtual ICollection<Chat>? Chats { get; set; }
    }

    public class Chat
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public DateTime InsertDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }

    public class ChatterContext : DbContext
    {
        public ChatterContext(DbContextOptions options) : base(options)
        {}

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Chat>? Chats { get; set; }
    }
}
