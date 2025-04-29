using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.BLL.IServices
{
    public interface IReactionService
    {
        public IEnumerable<SubmittedReaction> GetAllReaction();
        public SubmittedReaction GetReaction(long id);
        public void AddReaction(SubmittedReaction reaction);
        public void UpdateReaction(SubmittedReaction reaction);
        public void RemoveReaction(SubmittedReaction reaction);
        public void HandleReaction(int userId, int postId, Reaction type);
    }
}
