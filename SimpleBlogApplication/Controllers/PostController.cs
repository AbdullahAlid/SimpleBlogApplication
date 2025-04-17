using Microsoft.AspNetCore.Mvc;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly ApplicationDbContext _context;

        public PostController(PostService postService, ApplicationDbContext context)
        {
            _postService = postService;
            _context = context;
        }
        public IActionResult Index()
        {
            ViewData["Comments"] = _context.Comments.ToList();
            ViewData["Reactions"] = _context.PostReactions.ToList();
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
            _postService.SaveBlog(post);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReactionHandler(int id, Reaction type)
        {
            PostReaction reaction;
            if (type == Reaction.Like)
            {
                reaction = new PostReaction()
                {
                    PostId = id,
                    Reaction = DAL.Models.Reaction.Like
                };
                _context.PostReactions.Add(reaction);
            }
            if (type == Reaction.Dislike)
            {
                reaction = new PostReaction()
                {
                    PostId = id,
                    Reaction = DAL.Models.Reaction.Dislike
                };
                _context.PostReactions.Add(reaction);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
