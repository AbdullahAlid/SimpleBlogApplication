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
            post.UserId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User)); ;
            _postService.SaveBlog(post);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReactionHandler(int id, Reaction type)
        {
            int userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            SubmittedReaction reaction;
            var checkAvailablity = _context.SubmittedReactions.FirstOrDefault(x => x.UserId == userId && x.PostId == id && x.CommentId == null);
            if(checkAvailablity == null)
            {
                if (type == Reaction.Like)
                {
                    reaction = new SubmittedReaction()
                    {
                        PostId = id,
                        UserId = userId,
                        Reaction = DAL.Models.Reaction.Like
                    };
                    _context.SubmittedReactions.Add(reaction);
                }
                if (type == Reaction.Dislike)
                {
                    reaction = new SubmittedReaction()
                    {
                        PostId = id,
                        UserId = userId,
                        Reaction = DAL.Models.Reaction.Dislike
                    };
                    _context.SubmittedReactions.Add(reaction);
                }
            }
            else
            {
                if(checkAvailablity.Reaction == type)
                {
                    _context.SubmittedReactions.Remove(checkAvailablity);
                }
                else
                {
                    checkAvailablity.Reaction = type;
                    _context.SubmittedReactions.Update(checkAvailablity);
                }
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
