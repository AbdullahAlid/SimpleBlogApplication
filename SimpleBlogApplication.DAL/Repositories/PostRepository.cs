using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Repositories
{
    public class PostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts.Include(p => p.User).Include(p => p.UploadedComments).Include(p => p.SubmittedReactions).OrderByDescending(p => p.Id).ToList();
        }

        public Post? GetById(int id)
        {
            return _context.Posts.FirstOrDefault(p=> p.Id == id);
        }

        public void SaveBlog(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }
    }
}
