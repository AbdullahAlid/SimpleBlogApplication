using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Repositories
{
    public class ReactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<SubmittedReaction> GetReactions()
        {
            return _context.SubmittedReactions;
        }

        public void AddReaction(SubmittedReaction reaction)
        {
            _context.SubmittedReactions.Add(reaction);
            _context.SaveChanges();
        }
        public void UpdateReaction(SubmittedReaction reaction)
        {
            _context.SubmittedReactions.Update(reaction);
            _context.SaveChanges();
        }

        public void RemoveReaction(SubmittedReaction reaction)
        {
            _context.SubmittedReactions.Remove(reaction);
            _context.SaveChanges();
        }
    }
}
