using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;
using RepairmenModel;
using DAL;
using Repairmen.Models;
using AutoMapper;
using System.Text;
using System.Data.Entity.Core;
using Repairmen.Helpers;
using System.Configuration;
using Repairmen.Controllers.Helpers;


namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Comments")]
    public class CommentsController : RepairmenApiControllerBase
    {
        public static CustomConfig Config { get; internal set; }

        private IUnitOfWork unitOfWork;

        public CommentsController()
        {
            unitOfWork = new UnitOfWork();
        }

        public CommentsController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }


        public HttpResponseMessage Post(CommentModel comment)
        {
            return OK(comment);
        }


        // GET api/Comments/Inappropriate
        [Route("Inappropriate")]
        public HttpResponseMessage GetInappropriateComments()
        {
            IEnumerable<CommentModel> commentModels;
            try
            {
                Config= ConfigurationManager.GetSection("customSection") as CustomConfig;
                commentModels = unitOfWork.CommentRepository.Get(c => c.Counter > Config.CommentsCount).Select(x => Mapper.Map<CommentModel>(x));
                return OK(commentModels);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Inappropriate Comments.");
            }
        }

        // GET api/Comments/Inappropriate
        [Route("Inappropriate/{commentId:Guid}")]
        public HttpResponseMessage GetDescriptions(System.Guid commentId)
        {
            IEnumerable<InappropriateCommentModel> inappCommModels;
            try
            {
                inappCommModels = unitOfWork.InappropriateCommentRepository.Get(c => c.CommentId==commentId).Select(x => Mapper.Map<InappropriateCommentModel>(x));
                return OK(inappCommModels);
            }
            catch 
            {
                return NotFound("Not Acceptable. There is no Description for provided CommentId.");
            }
        }

        // GET api/Comments/UserId/{Guid}
        [Route("UserId/{userId:Guid}")]
        [ResponseType(typeof(CommentModel))]
        public HttpResponseMessage GetCommentsByUserId(System.Guid userId)
        {
            IEnumerable<CommentModel> commentModel;
            try
            {
                commentModel = unitOfWork.CommentRepository.Get(a => a.UserId == userId).OrderByDescending(a => a.Date).Select(x => Mapper.Map<CommentModel>(x));
                return OK(commentModel);
            }
            catch
            {
                 return NotFound("Not Acceptable. There is no Comment with provided UserId.");
            }
        }

        // GET api/Comments/AdId/{Guid}
        [Route("AdId/{adId:Guid}")]
        [ResponseType(typeof(CommentModel))]
        public HttpResponseMessage GetCommentsByAdId(System.Guid adId)
        {
            IEnumerable<CommentModel> commentModel;
            try
            {
                commentModel = unitOfWork.CommentRepository.Get(a => a.AdId == adId).OrderByDescending(a => a.Date).Select(x => Mapper.Map<CommentModel>(x));
                return OK(commentModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Comment with provided AdId.");
            }
        }

        // GET api/Comments/Id/{Guid}
        [Route("Id/{Id:Guid}")]
        [ResponseType(typeof(CommentModel))]
        public HttpResponseMessage GetCommentById(System.Guid id)
        {
            CommentModel commentModel;
            try
            {
                commentModel = Mapper.Map<CommentModel>(unitOfWork.CommentRepository.Get(a => a.Id == id).FirstOrDefault());
                return OK(commentModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Comment with provided Id.");
            }
        }

        // PUT api/Comments
        [Route("")]
        [Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage PutComment(CommentModel commentModel)
        {
            AdModel adModel;
            UserModel userModel;
            try
            {
                adModel = unitOfWork.AdRepository.Get(a => a.Id == commentModel.AdId).Select(x => Mapper.Map<AdModel>(x)).FirstOrDefault();
                adModel.CommentCounter++;
                var comment = Mapper.Map<Comment>(commentModel);
                comment.Counter=0;
                comment.PositiveVote = 0;
                comment.NegativeVote = 0;
                comment.Date = DateTime.Now;
                //get username by userid
                userModel = unitOfWork.UserRepository.Get(u => u.Id == comment.UserId).Select(x => Mapper.Map<UserModel>(x)).FirstOrDefault();
                comment.Username = userModel.Username;
                unitOfWork.CommentRepository.Insert(comment);
                var ad = Mapper.Map<Ad>(adModel);
                unitOfWork.AdRepository.Update(ad);
                unitOfWork.Save();
                //Send Notification about new comment:
                NotificationHelper nh = new NotificationHelper(ad.Id, ad.Name, ad.UserId);
                nh.Send();
               
                return Create(comment);
            }
            catch (ObjectNotFoundException)
            {
                return NotFound("Not Found. There is no Ad here");

            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not insert Comments in database.", ex);
            }
        }

        // PUT api/Comments/Inappropriate
        [Route("Inappropriate")]
        [Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage PutCommentInappropriate(InappropriateCommentModel inappropriateModel)
        {
            CommentModel commentModel;
            try
            {
                if (InappropriateExists(inappropriateModel) == null)               
                {
                    commentModel = unitOfWork.CommentRepository.Get(a => a.Id == inappropriateModel.CommentId).Select(x => Mapper.Map<CommentModel>(x)).FirstOrDefault();
                    commentModel.Counter++;
                    var inappropriate = Mapper.Map<InappropriateComment>(inappropriateModel);
                    unitOfWork.InappropriateCommentRepository.Insert(inappropriate);
                    var comment = Mapper.Map<Comment>(commentModel);
                    unitOfWork.CommentRepository.Update(comment);
                    unitOfWork.Save();
                    return Create(inappropriate);
                }
                else
                {
                    return Forbidden();
                }
            }
            catch (ObjectNotFoundException)
            {
                return NotFound("Not Found. There is no Comment");

            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not set Inappropriate flag for this Comment.", ex);
            }

        }

        // POST api/Comments 
        //[ValidationResponseFilter]
        [Route("")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public HttpResponseMessage PostComment(List<CommentModel> commentsInput)
        {
            foreach (CommentModel commentInput in commentsInput)
            {               
                var comment = Mapper.Map<Comment>(commentInput);
                IEnumerable<InappropriateCommentModel> inappModel;
                try
                {
                    //whether the comment is approved or not, we are deleting the entry for it in inappropriateComment table
                    inappModel = unitOfWork.InappropriateCommentRepository.Get(a => a.CommentId == comment.Id).Select(x => Mapper.Map<InappropriateCommentModel>(x));
                    foreach (InappropriateCommentModel inapp in inappModel)
                    {
                        var inappComm = Mapper.Map<InappropriateComment>(inapp);
                        unitOfWork.InappropriateCommentRepository.Delete(inappComm);
                    }
                    
                    //if comment is flagged to be deleted by admin, we need to delete if from Comments table, and decrease comments counter on Ad
                    if (commentInput.Delete == true)
                    {                        
                        if (commentInput.Counter == 0)
                            unitOfWork.CommentRepository.Update(comment);
                        else
                        {
                            // first delete votes of comment (from CommentVote datatable)
                            try
                            {
                                IEnumerable<CommentVote> votingModel = unitOfWork.CommentVoteRepository.Get(i => i.CommentId == comment.Id);
                                foreach(CommentVote cmv in votingModel)
                                {
                                    unitOfWork.CommentVoteRepository.Delete(cmv);
                                }
                            }
                            catch
                            {

                            }
                            unitOfWork.CommentRepository.Delete(comment);
                            AdModel adModel = unitOfWork.AdRepository.Get(a => a.Id == comment.AdId).Select(x => Mapper.Map<AdModel>(x)).FirstOrDefault();                         
                            var ad = Mapper.Map<Ad>(adModel);
                            ad.CommentCounter--;
                            unitOfWork.AdRepository.Update(ad);
                            unitOfWork.Save();   
                        }
                    }    
                    //need to set the counter for inappropriate flags to 0 since the admin approved comment
                    else if (commentInput.Approved == true)
                    {
                        comment.Counter = 0;
                        unitOfWork.CommentRepository.Update(comment);
                        unitOfWork.Save();   
                    }
                       
                                    
                }
                catch (ObjectNotFoundException)
                {
                    return NotFound("Not Found. There is no Inappropriate comment or there is no Ad.");

                }
                catch (Exception ex)
                {
                    return InternalServerError("Server Error: Can not update Comments in database.", ex);
                }

            }
            return NoContent();
        }

        // PUT api/Comments/Vote
        [Route("Vote")]
        [Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage PutCommentVote(CommentVoteModel commentVoteModel)
        {
            CommentModel commentModel = unitOfWork.CommentRepository.Get(a => a.Id == commentVoteModel.CommentId).Select(x => Mapper.Map<CommentModel>(x)).FirstOrDefault();
            try
            {
                CommentVote vote;
                if (CommentVoteExists(commentVoteModel) == null)
                {
                    vote = Mapper.Map<CommentVote>(commentVoteModel);
                    unitOfWork.CommentVoteRepository.Insert(vote);
                }
                else
                {

                    vote = Mapper.Map<CommentVote>(CommentVoteExists(commentVoteModel));

                    vote.Vote = commentVoteModel.Vote;

                    unitOfWork.CommentVoteRepository.Update(vote);

                }
                unitOfWork.Save();
                try
                {
                    commentModel.NegativeVote = (short)unitOfWork.CommentVoteRepository.Get(v => v.CommentId == commentModel.Id && v.Vote == false).Select(x => Mapper.Map<CommentVoteModel>(x)).Count();
                }
                catch (Exception)
                {
                    commentModel.NegativeVote = 0;
                }
                try
                {
                    commentModel.PositiveVote = (short)unitOfWork.CommentVoteRepository.Get(v => v.CommentId == commentModel.Id && v.Vote == true).Select(x => Mapper.Map<CommentVoteModel>(x)).Count();
                }
                catch (Exception)
                {
                    commentModel.PositiveVote = 0;
                }
                var comment = Mapper.Map<Comment>(commentModel);
                unitOfWork.CommentRepository.Update(comment);
                unitOfWork.Save();
                return Create();

            }
            catch (ObjectNotFoundException)
            {
                return NotFound("Not Found. There is no Comment");

            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not add User vote for this Comment.", ex);
            }

        }

        private InappropriateCommentModel InappropriateExists(InappropriateCommentModel inappModel)
        {
            try
            {
                return unitOfWork.InappropriateCommentRepository.Get(r => r.UserId == inappModel.UserId && r.CommentId == inappModel.CommentId).Select(x => Mapper.Map<InappropriateCommentModel>(x)).First();

            }
            catch
            {
                return null;
            }
        }

        // help function, to examine if user are already voted for this comment.
        private CommentVoteModel CommentVoteExists(CommentVoteModel commentVoteModel)
        {
            try
            {
                return unitOfWork.CommentVoteRepository.Get(r => r.UserId == commentVoteModel.UserId && r.CommentId == commentVoteModel.CommentId).Select(x => Mapper.Map<CommentVoteModel>(x)).First();

            }
            catch
            {
                return null;
            }
        }

    }
   
}
