using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Post> Posts { get; set; }
        public DbSet<SubmittedReaction> SubmittedReactions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Post>().HasOne(x => x.User).WithMany(x => x.UploadedPost).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.ClientSetNull);

            base.OnModelCreating(builder);
            builder.Entity<Post>().HasOne(x => x.Approver).WithMany(x => x.ApprovedPost).HasForeignKey(x => x.ApproverId).OnDelete(DeleteBehavior.ClientSetNull);
        }

    }
}
