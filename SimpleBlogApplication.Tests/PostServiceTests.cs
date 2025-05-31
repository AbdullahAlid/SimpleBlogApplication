using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlogApplication.BLL.IServices;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Models;
using System.Linq.Expressions;

namespace SimpleBlogApplication.Tests
{
    public class PostServiceTests
    {
        private readonly IPostRepository _postRepositoryMock;
        private readonly IPostService _sut;
        
        public PostServiceTests() 
        {
            _postRepositoryMock = Substitute.For<IPostRepository>();
            _sut = new PostService(_postRepositoryMock); 
        }

        [Fact]
        public void GetAllBlogTest_RepoReturnsPosts_ReturnValidList()
        {
            // Arrange
            var expectedList = new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                },
                new Post()
                {
                    Id = 2,
                }
            };

            _postRepositoryMock.GetAllBlog().Returns(expectedList);

            // Act
            var result = _sut.GetAllBlog();

            // Assert
            Assert.Equal(expectedList, result);
        }


        [Fact]
        public void GetAllBlogTest_RepoReturnsException_ReturnException()
        {
            // Arrange
            var expectedMessage = "Exception occured when tried to access blogs";
            _postRepositoryMock.GetAllBlog().Throws(new Exception(expectedMessage));

            // Act
            var result = Assert.Throws<Exception>(() =>_sut.GetAllBlog());

            // Assert
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public void AddPost_SetUploadDateTimeAndCallRepository() 
        {
            // Arrange
            var post = new Post()
            {
                Title = "Test",
                Content = "Content",
            };

            // Act
            _sut.AddBlog(post);

            // Assert
            Assert.True(post.UploadDateTime <= DateTime.Now);
            _postRepositoryMock.Received(1).AddBlog(post);
        }

        [Fact]
        public void AddPost_ExceptionOccur_ReturnException()
        {
            // Arrange
            var post = new Post()
            {
                Title = "Test",
                Content = "Content",
            };

            var errorMessage = "Exception Occure when Adding";

            _postRepositoryMock.AddBlog(post).Throws(new Exception(errorMessage));

            // Act & Assert
            var result = Assert.Throws<Exception>(() => _sut.AddBlog(post));

            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public void GetBlog_ValidInput_ReturnPostById()
        {
            // Arrange
            var blog = new Post()
            {
                Id = 1,
            };
            _postRepositoryMock.GetBlog(1).Returns(blog);

            // Act
            var result = _sut.GetBlog(1);

            // Assert
            Assert.Equal(blog, result);
        }

        [Fact]
        public void GetBlog_InvalidInput_ReturnException()
        {
            // Arrange
            var expectedErrorMessage = " Somehow error occur";
            _postRepositoryMock.GetBlog(Arg.Any<long>()).Throws(new Exception(expectedErrorMessage));

            // Act 
            var result = Record.Exception(()=> _sut.GetBlog(-1));

            // Assert
            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public void UpdateBlog_ForStatusAndApprover_ReturnTrue()
        {
            // Arrange
            var post = new Post()
            {
                Id = 1,
            };
            _postRepositoryMock.GetBlog(1).Returns(post);
            _postRepositoryMock.UpdateBlog(post).Returns(true);

            // Act
            var result = _sut.UpdateBlog(1, Status.Approved, 5);

            // Assert
            Assert.True(result);
            _postRepositoryMock.Received(1).GetBlog(1);
            _postRepositoryMock.Received(1).UpdateBlog(post);
        }

        [Fact]
        public void UpdateBlog_ForAnyThing_ReturnException()
        {
            // Arrange
            var post = new Post()
            {
                Id = 1,
            };

            var errorMessage = "update failed";
            _postRepositoryMock.GetBlog(1).Throws(new Exception(errorMessage));
            _postRepositoryMock.UpdateBlog(post).Throws(new Exception(errorMessage));


            // Act & Assert
            var result = Assert.Throws<Exception>(() => _sut.UpdateBlog(1, Status.Approved, 5));
            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public void GetTopFiveBlogs_AllOk_ReturnsFiveBlogs()
        {
            // Arrange
            var blogs = new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                },
                new Post()
                {
                    Id = 2,
                }
            };
            _postRepositoryMock.GetTopFiveBlogs().Returns(blogs);

            // Act
            var results = _sut.GetTopFiveBlogs();

            // Assert
            Assert.Equal(blogs, results);
        }

        [Fact]
        public void GetTopFiveBlogs_ReturnException()
        {
            // Arrange
            var errorMessage = "Exception occured when accessing top five";
            _postRepositoryMock.GetTopFiveBlogs().Throws(new Exception(errorMessage));

            // Act & Assert
            var result = Record.Exception(() => _sut.GetTopFiveBlogs());
            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public async Task GetStatusWisePost_AllOk_ReturnStatusWisePostAsync()
        {
            //Arrange
            var posts = new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                },
                new Post()
                {
                    Id = 2,
                }
            };

            _postRepositoryMock.GetStatusWisePost(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<Post, bool>>>()).Returns(posts);

            // Act
            var results = await _sut.GetStatusWisePost(4, 10, p => p.CurrentStatus == Status.Pending);

            // Assert
            Assert.Equal(posts.Count(), results.Count());
        }

        [Fact]
        public async Task GetStatusWisePost_SomethingWrong_ReturnException()
        {
            // Arrange
            var errorMessage = "Status wise post Throws exception";
            _postRepositoryMock.GetStatusWisePost(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<Post, bool>>>()).Throws(new Exception(errorMessage));

            // Act & Assert
            var result = await Record.ExceptionAsync(() => _sut.GetStatusWisePost(4, 10, p => p.CurrentStatus == Status.Pending));

            Assert.Equal(errorMessage, result.Message);

        }

        [Fact]
        public async Task GetStatusWisePostCount_AllOk_ReturnPostCount()
        {
            // Arrange
            long postsCount = 100;
            _postRepositoryMock.GetStatusWisePostCount(Arg.Any<Expression<Func<Post, bool>>>()).Returns(postsCount);

            // Act
            var result = await _sut.GetStatusWisePostCount(p => p.CurrentStatus == Status.Approved);

            // Assert
            Assert.Equal(postsCount, result);
        }

        [Fact]
        public async Task GetStatusWisePostCount_SomethingWrong_ReturnException()
        {
            // Arrange
            var errorMessage = "Exception occured";
            _postRepositoryMock.GetStatusWisePostCount(Arg.Any<Expression<Func<Post, bool>>>()).Throws(new Exception(errorMessage));

            // Act & Assert
            var result = await Record.ExceptionAsync(() =>_sut.GetStatusWisePostCount(p => p.CurrentStatus == Status.Approved));

            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public void DeleteBlog_ForBlog_ReturnTrue()
        {
            // Arrange
            var post = new Post()
            {
                Id = 1,
            };
            _postRepositoryMock.DeleteBlog(post).Returns(true);

            // Act
            var res = _sut.DeleteBlog(post);

            // Assert
            Assert.True(res);
            _postRepositoryMock.Received(1).DeleteBlog(post);

        }

        [Fact]
        public void DeleteBlog_ForAnyBlog_ThrowException()
        {
            // Arrange
            var post = new Post()
            {
                Id = 1,
            };
            var errorMessage = "When tried to delete exception occured";
            _postRepositoryMock.DeleteBlog(Arg.Any<Post>()).Throws(new Exception(errorMessage));

            // Act & Assert
            var res = Record.Exception(() => _sut.DeleteBlog(post));
            
            Assert.Equal(errorMessage, res.Message);


        }
    }
}
