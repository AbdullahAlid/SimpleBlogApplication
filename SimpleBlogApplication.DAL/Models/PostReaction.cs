using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlogApplication.DAL.Models
{
    public class PostReaction
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }
        public Reaction Reaction { get; set; }
        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
