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
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, IPasswordHasher<ApplicationUser> passwordHasher, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            long userId = Convert.ToInt64(_userManager.GetUserId(User));
            var users = _userManager.Users.Where(u=>u.Id != userId);
            return View(users);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "Post");
        }
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //fetch the User Details
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    //If User does not exists, redirect to the Login Page
                    return RedirectToAction("Login", "Account");
                }

                // ChangePasswordAsync Method changes the user password
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or the current password is incorrect.
                // Add these errors to the ModelState and rerender ChangePassword view
                if (result.Succeeded)
                {

                    // Upon successfully changing the password refresh sign-in cookie
                    await _signInManager.RefreshSignInAsync(user);

                    //Then redirect the user to the ChangePasswordConfirmation view
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

        [Authorize]
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        public IActionResult BlockedUser()
        {
            return View();
        }

        public IActionResult Block(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if(user != null)
            {
                user.ValidityStatus = UserValidityStatus.Blocked;
                _context.Users.Update(user);
                _context.SaveChanges();
            }          
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Unblock(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.ValidityStatus = UserValidityStatus.Unblocked;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
