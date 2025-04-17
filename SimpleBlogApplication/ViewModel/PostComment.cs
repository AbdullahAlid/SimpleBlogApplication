using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.ViewModel
{
    public class PostComment
    {
        public string PostTitle { get; set; }
        public string PostBody { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public string CommentText { get; set; }
        public long PostId { get; set; }
    }
}
