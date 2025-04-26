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
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole()
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new ApplicationRole()
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                }
            );



            builder.Entity<Post>().HasOne(x => x.AppUser).WithMany(x => x.UploadedPost).HasForeignKey(x => x.AppUserId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Post>().HasOne(x => x.Approver).WithMany(x => x.ApprovedPost).HasForeignKey(x => x.ApproverId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Comment>().HasOne(x => x.Post).WithMany(x => x.UploadedComments).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Comment>().HasOne(x => x.AppUser).WithMany(x => x.Comments).HasForeignKey(x => x.AppUserId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<SubmittedReaction>().HasOne(x => x.Post).WithMany(x => x.SubmittedReactions).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.ClientSetNull);
        }

    }
}
