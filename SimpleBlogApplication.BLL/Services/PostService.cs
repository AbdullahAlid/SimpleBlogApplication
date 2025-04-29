using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.BLL.IServices;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void AddBlog(Post post)
        {
            try
            {
                post.UploadDateTime = DateTime.Now;
                _postRepository.AddBlog(post);
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

        public void UpdateBlog(long id, Status status, long userId)
        {
            try
            {
                var post = GetBlog(id);
                post.CurrentStatus = status;
                post.ApproverId = userId;
                _postRepository.UpdateBlog(post);
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

        public void DeleteBlog(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
