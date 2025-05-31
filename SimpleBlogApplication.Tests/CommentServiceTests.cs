using NSubstitute;
using SimpleBlogApplication.BLL.IServices;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlogApplication.Tests
{
    
    public class CommentServiceTests
    {
        private readonly ICommentRepository _commentRepositoryMock;
        private readonly ICommentService _sut;

        public CommentServiceTests()
        {
            _commentRepositoryMock = Substitute.For<ICommentRepository>();
            _sut = new CommentService(_commentRepositoryMock);
        }


    }
}
