using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IReactionService _reactionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PostController(IPostService postService, IReactionService reactionService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _postService = postService;
            _reactionService = reactionService;
            _userManager = userManager;
            _signInManager = signInManager;          
        }

        [AllowAnonymous]
        public IActionResult LoadPrev(int startfrom, bool loadPrevPage)
        {
            if (loadPrevPage)
            {   
                return RedirectToAction(nameof(OwnBlogs), new { skip = startfrom - 5 });
            }
            return RedirectToAction("Index", new { skip = startfrom - 5 });
        }

        [AllowAnonymous]
        public IActionResult LoadNext(int startfrom, bool loadNextPage)
        {

            if (loadNextPage)
            {               
                return RedirectToAction(nameof(OwnBlogs), new { skip = startfrom });
            }

            return RedirectToAction(nameof(Index), new {skip = startfrom });
        }

        [AllowAnonymous]
        public IActionResult Index(int skip = 0, int step = 5)
        {
            ViewData["userId"] = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            try
            {
                var posts = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Approved).Skip(skip).Take(step);               
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Approved).Count()
                };
                
                return View(blogList);
            }
            catch(Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong!";
                return View();
            }

        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Title, Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.AppUserId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
                try
                {
                    _postService.AddBlog(post);
                }
                catch(Exception ex)
                {
                    Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                    TempData["Message"] = "Something went worng";
                    return View();
                }
                
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(post);
            }
            
        }

        public IActionResult ReactionHandler(int id, Reaction type, string page = "")
        {
            int userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            try
            {
                _reactionService.HandleReaction(userId, id, type);

                if (page == "TopFive")
                {
                    return RedirectToAction(nameof(TopBlogs));
                }

                if (page == "own")
                {
                    return RedirectToAction(nameof(OwnBlogs));
                }

                if (page == "create")
                {
                    return RedirectToAction(nameof(Create), nameof(Comment), new { id = id });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PendingBlogs(int skip = 0, int step = 5)
        {
            try
            {
                var posts = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Pending).Skip(skip).Take(step);
                ViewData["Id"] = 0;
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = _postService.GetAllBlog().Where(p => p.CurrentStatus == Status.Pending).Count()
                };
                return View(blogList);
            }
            catch(Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went worng";
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Approval(long id, Status status)
        {
            if(status != Status.Pending && status != Status.Approved && status != Status.Rejected)
            {
                TempData["Id"] = (int)id;
                TempData["selectPlease"] = "Please Select An option";
                return RedirectToAction(nameof(PendingBlogs));
            }
            long userId = Convert.ToInt64(_userManager.GetUserId(User));
            try
            {
                _postService.UpdateBlog(id, status, userId);
                return RedirectToAction(nameof(PendingBlogs));
            }
            catch(Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong!";
                return RedirectToAction(nameof(PendingBlogs));
            }
            
        }

        public IActionResult LoadNextPending(int startfrom)
        {
            return RedirectToAction(nameof(PendingBlogs), new { skip = startfrom });
        }

        public IActionResult LoadPrevPending(int startfrom)
        {
            return RedirectToAction(nameof(PendingBlogs), new { skip = startfrom - 5 });
        }

        public IActionResult OwnBlogs(int skip = 0, int step = 5)
        {
            var role = HttpContext.User.IsInRole("BlockedUser");
            long userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            ViewData["userId"] = userId;
            ViewData["isFilterable"] = true;
            try
            {
                var posts = _postService.GetAllBlog().Where(p => p.AppUserId == userId).Skip(skip).Take(step);
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = _postService.GetAllBlog().Where(p => p.AppUserId == userId).Count()
                };
                return View(nameof(Index), blogList);
            }
            catch(Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong!";
                return View(nameof(Index));
            }
            
        }

        [HttpPost]
        public IActionResult PostFilter(Status filteredValue, int skip = 0, int step = 5)
        {            
            return RedirectToAction(nameof(FilteredBlogs), new { filteredValue = filteredValue });
        }

        public IActionResult FilteredBlogs(Status filteredValue, int skip = 0, int step = 5)
        {
            if(filteredValue != Status.Pending && filteredValue != Status.Approved && filteredValue != Status.Rejected)
            {
                TempData["Message"] = "Please Select a Status to Filter";
                return RedirectToAction(nameof(OwnBlogs));
            }
            long userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            ViewData["userId"] = userId;
            ViewData["isFilterable"] = true;
            ViewData["specificFilter"] = true;
            ViewData["filteredValue"] = filteredValue;
            try
            {
                var posts = _postService.GetAllBlog().Where(p => p.AppUserId == userId && p.CurrentStatus == filteredValue).Skip(skip).Take(step);
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = _postService.GetAllBlog().Where(p => p.AppUserId == userId && p.CurrentStatus == filteredValue).Count()
                };
                return View(nameof(Index), blogList);
            }
            catch(Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong";
                return View(nameof(Index));
            }
        }

        public IActionResult LoadPrevStatusWise(int startfrom, Status status)
        {
            return RedirectToAction(nameof(FilteredBlogs), new { skip = startfrom - 5, filteredValue = status});
        }

        public IActionResult LoadNextStatusWise(int startfrom, Status status)
        {            
            return RedirectToAction(nameof(FilteredBlogs), new { skip = startfrom, filteredValue = status });
        }

        public IActionResult TopBlogs()
        {
            long userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            ViewData["userId"] = userId;
            ViewData["pageType"] = "TopFive";
            try
            {
                var posts = _postService.GetTopFiveBlogs();
                var blogList = new BlogList()
                {
                    Blogs = posts,
                };
                return View(nameof(Index), blogList);
            }
            catch (Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong";
                return View(nameof(Index));
            }

        }
    }
}
