using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.BLL.IServices;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public void AddComment(long userId, long postId, string comment)
        {
            try
            {
                var submittedComment = new Comment()
                {
                    CommentText = comment,
                    CommentDateTime = DateTime.Now,
                    AppUserId = userId,
                    PostId = postId,
                };
                _commentRepository.AddComment(submittedComment);
            }
            catch(Exception) 
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
