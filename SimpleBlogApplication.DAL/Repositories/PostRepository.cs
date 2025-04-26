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

        public IEnumerable<Post> GetAllBlog()
        {
            try
            {
                return _context.Posts.Include(p => p.AppUser).Include(p => p.UploadedComments).Include(p => p.SubmittedReactions).OrderByDescending(p => p.Id);
            }
            catch (Exception) 
            {
                throw;
            }
        }

        public Post? GetBlog(long id)
        {
            try
            {
                return _context.Posts.Include(p => p.AppUser).Include(p => p.UploadedComments).ThenInclude(p => p.AppUser).Include(p => p.SubmittedReactions).FirstOrDefault(p => p.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void SaveBlog(Post post)
        {
            try
            {
                _context.Posts.Add(post);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
           
        }  
        
        public void UpdatePost(Post post)
        {
            try
            {
                _context.Posts.Update(post);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
