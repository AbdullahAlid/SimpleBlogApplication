using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.IRepositories
{
    public interface IPostRepository
    {
        public IEnumerable<Post> GetAllBlog();
        public Post GetBlog(long id);
        public void AddBlog(Post post);
        public void UpdateBlog(Post post);
        public void DeleteBlog(Post post);
    }
}
