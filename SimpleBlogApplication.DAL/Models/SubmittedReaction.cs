using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlogApplication.DAL.Models
{
    public class SubmittedReaction
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }
        public Reaction Reaction { get; set; }
        public long? AppUserId { get; set; }
        [ForeignKey(nameof(AppUserId))]
        public ApplicationUser? AppUser { get; set; }

        public long? CommentId { get; set; }
        [ForeignKey(nameof(CommentId))]
        public Comment? Comment { get; set; }
    }
}
