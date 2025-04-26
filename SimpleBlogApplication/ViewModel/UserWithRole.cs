using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.ViewModel
{
    public class UserWithRole
    {
        public ApplicationUser User { get; set; }
        public string Role { get; set; }
    }
}
