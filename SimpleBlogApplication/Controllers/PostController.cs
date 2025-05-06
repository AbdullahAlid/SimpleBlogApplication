using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using SimpleBlogApplication.BLL.IServices;
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
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public static List<string> cachedKeyNames = new List<string>();

        public PostController(IPostService postService, IReactionService reactionService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMemoryCache cache)
        {
            _postService = postService;
            _reactionService = reactionService;
            _userManager = userManager;
            _signInManager = signInManager;
            _cache = cache;
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
        public async Task<IActionResult> Index(int skip = 0, int step = 5)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cacheKey = $"Posts: {skip}";
            ViewData["userId"] = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            string msg = "";
            var blogList = new BlogList();
            try
            {                
                if (_cache.TryGetValue(cacheKey, out blogList))
                {
                    msg = "from cache";
                    Log.Information($"Data is accessing from cache");
                }
                else
                {
                    try
                    {
                        await semaphore.WaitAsync();                        
                        if (_cache.TryGetValue(cacheKey, out blogList))
                        {
                            msg = "from cache";
                            Log.Information($"Data is accessing from cache");
                        }

                        else
                        {

                            var posts = await _postService.GetStatusWisePost(skip, step, p => p.CurrentStatus == Status.Approved);

                            blogList = new BlogList()
                            {
                                Blogs = posts,
                                StartFrom = skip,
                                TotalBlogs = await _postService.GetStatusWisePostCount(p => p.CurrentStatus == Status.Approved)
                            };
                            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300)).SetAbsoluteExpiration(TimeSpan.FromSeconds(3600)).SetPriority(CacheItemPriority.Normal).SetSize(2);
                            _cache.Set(cacheKey, blogList, cacheOptions);
                            cachedKeyNames.Add(cacheKey);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                        msg = "from database";
                    }
                }
                stopwatch.Stop();
                var time = stopwatch.ElapsedMilliseconds;
                Log.Information("Elapsed time {Time} ms when accessing {Msg}", time, msg);
                return View(blogList);
            }
            catch(Exception ex)
            {
                
                Log.Error(ex.Message,ex);
                TempData["Message"] = "Something went wrong!";
                return View(new BlogList());
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

        public IActionResult ReactionHandler(int id, Reaction type, string page = "", int skip = 0)
        {
            int userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            try
            {
                _reactionService.HandleReaction(userId, id, type);
                RemoveCache();

                if (page == "TopFive")
                {
                    return RedirectToAction(nameof(TopBlogs));
                }

                if (page == "own")
                {
                    return RedirectToAction(nameof(OwnBlogs), new { skip = skip });
                }

                if (page == "create")
                {
                    return RedirectToAction(nameof(Create), nameof(Comment), new { id = id });
                }



                return RedirectToAction(nameof(Index), new {skip = skip});
            }
            catch (Exception ex)
            {
                Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                TempData["Message"] = "Something went wrong";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingBlogs(int skip = 0, int step = 5)
        {
            try
            {
                var posts = await _postService.GetStatusWisePost(skip, step, p => p.CurrentStatus == Status.Pending);
                ViewData["Id"] = 0;
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = await _postService.GetStatusWisePostCount(p => p.CurrentStatus == Status.Pending),
                };
                return View(blogList);
            }
            catch(Exception ex)
            {
                //Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                Log.Error(ex.Message, ex);
                TempData["Message"] = "Something went worng";
                return View(new BlogList());
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
                if (status == Status.Approved)
                {
                    RemoveCache();
                }
                _postService.UpdateBlog(id, status, userId);
                return RedirectToAction(nameof(PendingBlogs));
            }
            catch(Exception ex)
            {
                //Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                Log.Error(ex.Message, ex);
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

        public async Task<IActionResult> OwnBlogs(int skip = 0, int step = 5)
        {
            var role = HttpContext.User.IsInRole("BlockedUser");
            long userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            ViewData["userId"] = userId;
            ViewData["isFilterable"] = true;
            try
            {
                var posts = await _postService.GetStatusWisePost(skip, step, p => p.AppUserId == userId);
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = await _postService.GetStatusWisePostCount(p => p.AppUserId == userId)
                };
                return View(nameof(Index), blogList);
            }
            catch(Exception ex)
            {
                //Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                Log.Error(ex.Message, ex);
                TempData["Message"] = "Something went wrong!";
                return View(nameof(Index));
            }
            
        }

        [HttpPost]
        public IActionResult PostFilter(Status filteredValue, int skip = 0, int step = 5)
        {            
            return RedirectToAction(nameof(FilteredBlogs), new { filteredValue = filteredValue });
        }

        public async Task<IActionResult> FilteredBlogs(Status filteredValue, int skip = 0, int step = 5)
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
                var posts = await _postService.GetStatusWisePost(skip, step, p => p.AppUserId == userId && p.CurrentStatus == filteredValue);
                var blogList = new BlogList()
                {
                    Blogs = posts,
                    StartFrom = skip,
                    TotalBlogs = await _postService.GetStatusWisePostCount(p => p.AppUserId == userId && p.CurrentStatus == filteredValue)
                };
                return View(nameof(Index), blogList);
            }
            catch(Exception ex)
            {
                //Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                Log.Error(ex.Message, ex);
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
                //Log.Information($"Source: {RouteData.Values["controller"]}/{RouteData.Values["action"]} Message: {ex.Message}");
                Log.Error(ex.Message, ex);
                TempData["Message"] = "Something went wrong";
                return View(nameof(Index), new BlogList());
            }

        }

        private void RemoveCache()
        {
            foreach (string key in cachedKeyNames)
            {
                _cache.Remove(key);
            }
        }
    }
}
