using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication.BLL.Services
{
    public class CommentService
    {
        private readonly CommentRepository _commentRepository;

        public CommentService(CommentRepository commentRepository)
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
    }
}
