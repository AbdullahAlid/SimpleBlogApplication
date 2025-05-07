using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using SimpleBlogApplication.BLL.IServices;
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
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, IPostService postService, UserManager<ApplicationUser> userManager, IMemoryCache cache, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _postService = postService;
            _userManager = userManager;
            _cache = cache;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Create(int id)
        {
            var post = _postService.GetBlog(id);

            try
            {
                if(post != null )
                {
                    if (post.CurrentStatus != Status.Approved)
                    {
                        throw new Exception("Tried to comment in unapproved data");
                    }
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
            catch (Exception ex)
            {
                _logger.LogError($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
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
                foreach (string key in PostController.cachedKeyNames)
                {
                    _cache.Remove(key);
                }
                return RedirectToAction(nameof(Create), new { id = post.PostId });
            }
            catch(Exception ex)
            {
                _logger.LogError($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong";
                return RedirectToAction(nameof(Create), new { id = post.PostId });
            }
            
        }
    }
}
