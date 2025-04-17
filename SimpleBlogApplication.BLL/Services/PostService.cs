﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication.BLL.Services
{
    public class PostService
    {
        private readonly PostRepository _postRepository;

        public PostService(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void SaveBlog(Post post)
        {
            post.UploadDateTime = DateTime.Now;
            _postRepository.SaveBlog(post);
        }

        public IEnumerable<Post> GetAllBlog()
        {
            return _postRepository.GetAll();
        }
    }
}
