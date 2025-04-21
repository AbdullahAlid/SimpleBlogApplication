using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.ViewModel;

namespace SimpleBlogApplication.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly ReactionService _reactionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(PostService postService, ReactionService reactionService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _reactionService = reactionService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult LoadPrev(int startfrom)
        {
            return RedirectToAction("Index", new { skip = startfrom - 5 });
        }

        [AllowAnonymous]
        public IActionResult LoadNext(int startfrom)
        {
            return RedirectToAction("Index", new {skip = startfrom });
        }

        [AllowAnonymous]
        public IActionResult Index(int skip = 0, int step = 5)
        {
            ViewData["userId"] = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            var posts = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Approved).Skip(skip).Take(step);
            var blogList = new BlogList()
            {
                Blogs = posts,
                StartFrom = skip,
                TotalBlogs = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Approved).Count()
            };
            return View(blogList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Title, Content")] Post post)
        {
            post.AppUserId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            _postService.SaveBlog(post);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReactionHandler(int id, Reaction type, string page = "")
        {
            int userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            if (page == "create")
            {
                _reactionService.HandleReaction(userId, id, type);
                return RedirectToAction(nameof(Create),nameof(Comment), new {id = id});
            }                      
            _reactionService.HandleReaction(userId, id, type);            
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PendingBlogs(int skip = 0, int step = 5)
        {
            var posts = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Pending).Skip(skip).Take(step);
            var blogList = new BlogList()
            {
                Blogs = posts,
                StartFrom = skip,
                TotalBlogs = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Pending).Count()
            };
            return View(blogList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Approval(long id, Status status)
        {
            long userId = Convert.ToInt64(_userManager.GetUserId(User));
            _postService.UpdatePost(id, status, userId);
            return RedirectToAction(nameof(PendingBlogs));
        }

        public IActionResult LoadNextPending(int startfrom)
        {
            return RedirectToAction(nameof(PendingBlogs), new { skip = startfrom });
        }

        public IActionResult LoadPrevPending(int startfrom)
        {
            return RedirectToAction(nameof(PendingBlogs), new { skip = startfrom - 5 });
        }
    }
}
