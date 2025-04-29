using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.BLL.IServices
{
    public interface ICommentService
    {
        public IEnumerable<Comment> GetAllComment();
        public Post GetComment(long id);
        public void AddComment(long userId, long postId, string comment);
        public void UpdateComment(Comment comment);
        public void DeleteComment(Comment comment);
    }
}
