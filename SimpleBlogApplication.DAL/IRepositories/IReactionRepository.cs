using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.IRepositories
{
    public interface IReactionRepository
    {
        public IEnumerable<SubmittedReaction> GetAllReaction();
        public SubmittedReaction? GetReaction(Expression<Func<SubmittedReaction, bool>> filter);
        public void AddReaction(SubmittedReaction reaction);
        public void UpdateReaction(SubmittedReaction reaction);
        public void RemoveReaction(SubmittedReaction reaction);
    }
}
