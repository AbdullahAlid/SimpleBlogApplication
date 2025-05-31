using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Filters
{
    public class CheckUserValidityFilter : IAsyncAuthorizationFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CheckUserValidityFilter(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(user);
                var appUser = await _userManager.FindByIdAsync(userId);

                if (appUser == null || appUser.ValidityStatus == UserValidityStatus.Blocked)
                {
                    await _signInManager.SignOutAsync();
                    context.Result = new RedirectToActionResult("Login", "Account", new {});
                }
            }           
        }
    }
}
