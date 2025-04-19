using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.ViewModel;

namespace SimpleBlogApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, IPasswordHasher<ApplicationUser> passwordHasher, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
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

                //if (_userManager.Users.ToList().Count == 0)
                //{
                //    await _userManager.AddToRoleAsync(appUser, "Admin");
                //}
                //else
                //{
                //    await _userManager.AddToRoleAsync(appUser, "User");
                //}

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded) 
                { 
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


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "Post");
        }
    }
}
