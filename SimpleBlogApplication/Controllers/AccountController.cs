using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.ViewModel;

namespace SimpleBlogApplication.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;           
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            long userId = Convert.ToInt64(_userManager.GetUserId(User));
            var users = _userManager.Users.Where(u=>u.Id != userId);
            //var user = await _userManager.GetUsersForClaimAsync("Admin").ToList();
            var usersToShow = _context.Roles.Where(r => r.Name != "Admin").Join(_context.UserRoles, r => r.Id, ur => ur.RoleId,(r,ur)=> new
            {
                UserId = ur.UserId
            }).Join(_context.Users,us=>us.UserId, u=> u.Id, (us, u) => new{
                user = u
            });
            var userList = new List<ApplicationUser>();
            foreach(var individualUser in usersToShow)
            {
               userList.Add(individualUser.user);
            }
            return View(userList);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.Email
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded) 
                {
                    if (_userManager.Users.ToList().Count == 1)
                    {
                        await _userManager.AddToRoleAsync(appUser, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(appUser, "User");
                    }
                    await _signInManager.SignInAsync(appUser, false);
                    return RedirectToAction("Index", "Post");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        
        public IActionResult Login(string returnUrl = "/")
        {
            LoginViewModel login = new LoginViewModel();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.UserName==model.Username && u.ValidityStatus == UserValidityStatus.Blocked);
                if(user != null)
                {
                    return View(nameof(BlockedUser));
                }
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Handle successful login
                    return Redirect(model.ReturnUrl ?? "/");
                }
                else
                {
                    // Handle failure
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "Post");
        }
        [Authorize(Roles = "Admin, User")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("ChangePasswordConfirmation", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        public IActionResult BlockedUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Block(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if(user != null)
            {
                
                user.ValidityStatus = UserValidityStatus.Blocked;
                await _userManager.AddToRoleAsync(user, "BlockedUser");
                await _userManager.RemoveFromRoleAsync(user, "User");
                _context.Users.Update(user);
                _context.SaveChanges();
            }          
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unblock(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.ValidityStatus = UserValidityStatus.Unblocked;
                await _userManager.RemoveFromRoleAsync(user, "BlockedUser");
                await _userManager.AddToRoleAsync(user, "User");
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
