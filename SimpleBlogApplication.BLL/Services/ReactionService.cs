using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication.BLL.Services
{
    public class ReactionService
    {
        private readonly ReactionRepository _repository;

        public ReactionService(ReactionRepository repository) 
        {
            _repository = repository;
        }
        public IEnumerable<SubmittedReaction> GetAllReaction()
        {
            return _repository.GetAllReaction();
        }

        public void HandleReaction(int userId, int postId, Reaction type)
        {
            var checkAvailablity = GetAllReaction().FirstOrDefault(x => x.AppUserId == userId && x.PostId == postId && x.CommentId == null);
            SubmittedReaction reaction;

            if (checkAvailablity == null)
            {

                if (type == Reaction.Like)
                {
                    reaction = new SubmittedReaction()
                    {
                        PostId = postId,
                        AppUserId = userId,
                        Reaction = DAL.Models.Reaction.Like
                    };
                    _repository.AddReaction(reaction);
                }

                if (type == Reaction.Dislike)
                {
                    reaction = new SubmittedReaction()
                    {
                        PostId = postId,
                        AppUserId = userId,
                        Reaction = DAL.Models.Reaction.Dislike
                    };
                    _repository.AddReaction(reaction);
                }
            }
            else
            {
                if (checkAvailablity.Reaction == type)
                {
                    _repository.RemoveReaction(checkAvailablity);
                }
                else
                {
                    checkAvailablity.Reaction = type;
                    _repository.UpdateReaction(checkAvailablity);
                }
            }

        }
    }
}
