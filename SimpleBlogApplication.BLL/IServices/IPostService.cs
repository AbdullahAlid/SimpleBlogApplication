using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.BLL.IServices
{
    public interface IPostService
    {
        public IEnumerable<Post> GetAllBlog();
        public IEnumerable<Post> GetTopFiveBlogs();
        public Post GetBlog(long id);
        public void AddBlog(Post post);
        public void UpdateBlog(long id, Status status, long userId);
        public void DeleteBlog(Post post);
        public Task<IEnumerable<Post>> GetStatusWisePost(int skipped, int amount, Expression<Func<Post, bool>> filter);
        public Task<long> GetStatusWisePostCount(Expression<Func<Post, bool>> filter);
    }
}
