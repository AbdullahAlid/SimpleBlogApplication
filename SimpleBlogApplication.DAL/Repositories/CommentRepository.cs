using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddComment(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();
            }
            catch (Exception) 
            {
                throw;
            }
        }

        public void DeleteComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comment> GetAllComment()
        {
            throw new NotImplementedException();
        }

        public Post GetComment(long id)
        {
            throw new NotImplementedException();
        }

        public void UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
