using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SimpleBlogApplication.DAL.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<Post> ApprovedPost { get; set; }
        public List<Post> UploadedPost { get; set; }
        public UserValidityStatus ValidityStatus { get; set; } = UserValidityStatus.Unblocked;
    }
}
