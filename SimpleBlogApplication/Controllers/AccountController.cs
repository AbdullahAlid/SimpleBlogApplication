﻿using Microsoft.AspNetCore.Identity;
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
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                int a = user.Email.IndexOf('@');
                ApplicationUser appUser = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.Email.Substring(0, a)
                };

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
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Handle successful login
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                // Handle failure
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
    }
}
