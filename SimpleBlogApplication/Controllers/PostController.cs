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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(PostService postService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _context = context;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Comments"] = _context.Comments.ToList();
            ViewData["Reactions"] = _context.SubmittedReactions.ToList();
            var posts = _postService.GetAllBlog();
            return View(posts);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create([Bind("Title, Content")] Post post)
        {
            post.UserId = Convert.ToInt64(_userManager.GetUserId(HttpContext.User));
            _postService.SaveBlog(post);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReactionHandler(int id, Reaction type)
        {
            SubmittedReaction reaction;
            if (type == Reaction.Like)
            {
                reaction = new SubmittedReaction()
                {
                    PostId = id,
                    Reaction = DAL.Models.Reaction.Like
                };
                _context.SubmittedReactions.Add(reaction);
            }
            if (type == Reaction.Dislike)
            {
                reaction = new SubmittedReaction()
                {
                    PostId = id,
                    Reaction = DAL.Models.Reaction.Dislike
                };
                _context.SubmittedReactions.Add(reaction);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
