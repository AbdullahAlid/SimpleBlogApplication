using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.ViewModel;

namespace SimpleBlogApplication.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;
        private readonly PostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(CommentService commentService, PostService postService, UserManager<ApplicationUser> userManager)
        {
            _commentService = commentService;
            _postService = postService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Create(int id)
        {
            var post = _postService.GetBlog(id);
            var postComment = new PostComment()
            {
                PostTitle = post.Title,
                PostBody = post.Content,
                PostId = post.Id,
                Blogger = $"{post.AppUser?.FirstName ?? ""} {post.AppUser?.LastName ?? ""}",
                Reactions = post.SubmittedReactions ?? new List<SubmittedReaction>(),
                Comments = post.UploadedComments.OrderByDescending(o => o.Id),
                UserId = Convert.ToInt64(_userManager.GetUserId(HttpContext.User))
            };
            return View(postComment);
        }

        [HttpPost]
        public IActionResult Create([Bind("CommentText, PostId")] PostComment post)
        {
            long userId = Convert.ToInt64(_userManager.GetUserId(HttpContext.User));
            _commentService.AddComment(userId, post.PostId, post.CommentText);
            return RedirectToAction(nameof(Create), new { id = post.PostId });
        }
    }
}
