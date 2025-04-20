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
        public IActionResult LoadMore(int numberToIncrease)
        {
            return RedirectToAction("Index", new {increasedNumber = numberToIncrease+5 });
        }

        [AllowAnonymous]
        public IActionResult Index(int increasedNumber = 5)
        {
            var posts = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Approved);
            var blogList = new BlogList()
            {
                Blogs = posts,
                NumberOfBlogs = increasedNumber
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

        public IActionResult ReactionHandler(int id, Reaction type)
        {
            int userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));           
            _reactionService.HandleReaction(userId, id, type);            
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PendingBlogs(int increasedNumber = 5)
        {
            var posts = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Pending);           
            var blogList = new BlogList()
            {
                Blogs = posts,
                NumberOfBlogs = increasedNumber
            };
            return View(blogList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Approval(long id, Status status = Status.Pending)
        {
            _postService.UpdatePost(id, status);
            return RedirectToAction(nameof(PendingBlogs));
        }

        public IActionResult LoadMorePending(int numberToIncrease)
        {
            return RedirectToAction("PendingBlogs", new { increasedNumber = numberToIncrease + 5 });
        }
    }
}
