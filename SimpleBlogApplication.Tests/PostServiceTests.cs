using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlogApplication.BLL.IServices;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.Tests
{
    public class PostServiceTests
    {
        private readonly IPostService _sut;
        private readonly IPostRepository _postRepository;
        public PostServiceTests() 
        {
            _postRepository = Substitute.For<IPostRepository>();
            _sut = new PostService(_postRepository); 
        }
        [Fact]
        public void GetAllBlogTest_RepoReturnList_ReturnValidList()
        {
            //Arrange
            var expectedList = new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                }
            };

            _postRepository.GetAllBlog().Returns(expectedList);

            //Act
            var result = _sut.GetAllBlog();

            //Assert
            Assert.Equal(expectedList.First().Id, result.First().Id);
            _postRepository.Received().GetAllBlog();
        }

        
    }
}
