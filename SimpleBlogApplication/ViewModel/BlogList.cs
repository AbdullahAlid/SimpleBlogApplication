using System.ComponentModel.DataAnnotations.Schema;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.ViewModel
{
    public class BlogList
    {
        public IEnumerable<Post> Blogs {  get; set; }
        public int StartFrom { get; set; }
        public long TotalBlogs { get; set; }
    }
}
