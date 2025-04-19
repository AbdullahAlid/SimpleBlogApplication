using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlogApplication.DAL.Models
{
    public class Post
    {
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime UploadDateTime { get; set; }
        public long? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public ApplicationUser? AppUser { get; set; }
        public Status CurrentStatus { get; set; } = Status.Pending;
        public long? ApproverId { get; set; }
        public ApplicationUser? Approver { get; set; }
        public List<Comment>? UploadedComments { get; set; }
        public List<SubmittedReaction>? SubmittedReactions { get; set; }
    }
}
