using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Filters;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.ViewModel;

namespace SimpleBlogApplication.Controllers
{
    [CheckUserValidity]
    [Authorize]
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;
        private readonly PostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CommentController(CommentService commentService, PostService postService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _commentService = commentService;
            _postService = postService;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Create(int id)
        {
            var post = _postService.GetBlog(id);

            try
            {
                if(post != null)
                {
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
                return RedirectToAction(nameof(Index), nameof(Post));
            }
            catch (Exception)
            {
                TempData["Message"] = "The post you requested wasn't found!";
                return RedirectToAction(nameof(Index), nameof(Post));
            }
        }

        [HttpPost]
        public IActionResult Create([Bind("CommentText, PostId")] PostComment post)
        {
            try
            {
                long userId = Convert.ToInt64(_userManager.GetUserId(HttpContext.User));
                _commentService.AddComment(userId, post.PostId, post.CommentText);
                return RedirectToAction(nameof(Create), new { id = post.PostId });
            }
            catch
            {
                TempData["Message"] = "Something went wrong";
                return RedirectToAction(nameof(Create), new { id = post.PostId });
            }
            
        }
    }
}
