using System.ComponentModel.DataAnnotations.Schema;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.ViewModel
{
    public class BlogList
    {
        public IEnumerable<Post> Blogs {  get; set; }
        public int NumberOfBlogs { get; set; } = 5;
    }
}
