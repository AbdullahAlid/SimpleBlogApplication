using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication.BLL.Services
{
    public class PostService
    {
        private readonly PostRepository _postRepository;

        public PostService(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void SaveBlog(Post post)
        {
            try
            {
                post.UploadDateTime = DateTime.Now;
                _postRepository.SaveBlog(post);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public IEnumerable<Post> GetAllBlog()
        {
            try
            {
                return _postRepository.GetAllBlog();
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
                return _postRepository.GetBlog(id);
            }
            catch (Exception) 
            {
                throw;
            }
            
        }

        public void UpdatePost(long id, Status status, long userId)
        {
            try
            {
                var post = GetBlog(id);
                post.CurrentStatus = status;
                post.ApproverId = userId;
                _postRepository.UpdatePost(post);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public IEnumerable<Post> GetTopFiveBlogs()
        {
            try
            {
                var posts = _postRepository.GetAllBlog().OrderByDescending(p => (p.SubmittedReactions.Where(l => l.Reaction == Reaction.Like).Count() + p.UploadedComments.Count())).Take(5);
                return posts;
            }
            catch (Exception) 
            {
                throw;
            }       
        }
        
    }
}
