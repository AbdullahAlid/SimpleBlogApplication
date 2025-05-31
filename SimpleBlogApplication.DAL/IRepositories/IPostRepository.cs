using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.IRepositories
{
    public interface IPostRepository
    {
        public IEnumerable<Post> GetAllBlog();
        public Post GetBlog(long id);
        public bool AddBlog(Post post);
        public bool UpdateBlog(Post post);
        public bool DeleteBlog(Post post);
        public Task<IEnumerable<Post>> GetStatusWisePost(int skipped, int amount, Expression<Func<Post, bool>> filter);
        public Task<long> GetStatusWisePostCount(Expression<Func<Post, bool>> filter);
        public IEnumerable<Post> GetTopFiveBlogs();

    }
}
