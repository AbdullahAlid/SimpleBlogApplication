using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.IRepositories
{
    public interface ICommentRepository
    {
        public IEnumerable<Comment> GetAllComment();
        public Post GetComment(long id);
        public void AddComment(Comment comment);
        public void UpdateComment(Comment comment);
        public void DeleteComment(Comment comment);
    }
}
