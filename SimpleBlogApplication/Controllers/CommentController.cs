﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.ViewModel;

namespace SimpleBlogApplication.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(int id)
        {
            Post? post = _context.Posts.FirstOrDefault(c => c.Id == id);
            var postComment = new PostComment()
            {
                PostTitle = post.Title,
                PostBody = post.Content,
                Comments = _context.Comments.Where(c => c.PostId == id).OrderByDescending(o => o.Id).ToList(),
                PostId = id
            };
            return View(postComment);
        }
        [HttpPost]
        public IActionResult Create([Bind("CommentText, PostId")] PostComment post)
        {
            Comment comment = new Comment()
            {
                CommentText = post.CommentText,
                CommentDateTime = DateTime.Now,
                PostId = post.PostId,
                UserId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User))
            };
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Create), new { id = post.PostId });
        }
    }
}
