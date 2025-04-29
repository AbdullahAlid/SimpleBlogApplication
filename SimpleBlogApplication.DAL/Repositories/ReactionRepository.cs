using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.IRepositories;
using SimpleBlogApplication.DAL.Models;

namespace SimpleBlogApplication.DAL.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<SubmittedReaction> GetAllReaction()
        {
            try
            {
                return _context.SubmittedReactions;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void AddReaction(SubmittedReaction reaction)
        {
            try
            {
                _context.SubmittedReactions.Add(reaction);
                _context.SaveChanges();
            }
            catch (Exception) 
            { 
                throw; 
            }   
        }

        public void UpdateReaction(SubmittedReaction reaction)
        {
            try
            {
                _context.SubmittedReactions.Update(reaction);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveReaction(SubmittedReaction reaction)
        {
            try
            {
                _context.SubmittedReactions.Remove(reaction);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SubmittedReaction GetReaction(long id)
        {
            throw new NotImplementedException();
        }
    }
}
