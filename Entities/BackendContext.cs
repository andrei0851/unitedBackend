using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend.Entities
{
    public class BackendContext : DbContext
    {
        public BackendContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Like { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(b => b.Followers)
            .WithMany(b => b.Following);
            modelBuilder.Entity<User>().HasMany(b => b.Following)
           .WithMany(b => b.Followers);
        }


    }
}
