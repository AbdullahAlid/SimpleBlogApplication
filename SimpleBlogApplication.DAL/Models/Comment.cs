using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlogApplication.DAL.Models
{
    public class Comment
    {
        public long Id { get; set; }
        [Required]
        public string CommentText { get; set; }
        public DateTime CommentDateTime { get; set; }
        
        public long? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }        
        public long? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public ApplicationUser? AppUser { get; set; }        
    }
}
