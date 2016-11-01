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
using RepairmenModel;
using System.Web.Http.Cors;
using DAL;
using Repairmen.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repairmen.Helpers;
using AutoMapper;
using Repairmen.Controllers.Helpers;

namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Users")]
    public class UsersController : RepairmenApiControllerBase
    {
        private IUnitOfWork unitOfWork;

        public UsersController()
        {
            unitOfWork = new UnitOfWork();
        }

        public UsersController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }


        // POST: api/Users
        [Route("")]
        public HttpResponseMessage Post(UserUpdateModel user)
        {
            User existingUser = new User();
            try
            {
                existingUser = unitOfWork.UserRepository.GetByID(user.UserId);

                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        existingUser.Email = user.Username;
                    }
                    if (!string.IsNullOrEmpty(user.DisplayName))
                    {
                        existingUser.Username = user.DisplayName;
                    }
                    if (user.NotifyEmail.HasValue)
                    {
                        existingUser.NotifyEmail = user.NotifyEmail;
                    }
                    if (user.NotifySMS.HasValue)
                    {
                        existingUser.NotifySMS = user.NotifySMS;
                    }
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        existingUser.PhoneNumber = user.PhoneNumber;
                    }
                    if (!string.IsNullOrEmpty(user.OldPassword) && !string.IsNullOrEmpty(user.NewPassword))
                    {
                        if (existingUser.LoginFlag == "SN")
                        {
                            return Forbidden();
                        }
                        else
                        {
                            if (existingUser.Password.Equals(user.OldPassword))
                            {
                                existingUser.Password = user.NewPassword;
                            }
                            else
                            {
                                return NotFound("Old");
                            }
                        }
                    }
                    existingUser.PasswordChange = false;
                    unitOfWork.UserRepository.Update(existingUser);
                    unitOfWork.Save();
                    return OK(existingUser);
            }
            catch(Exception e)
            {
                return NotFound("Not Acceptable. There is no User with provided Guid.");
            }
        }

        // GET: api/Users
        //[Authorize(Roles = "admin,repairmen")]
        [Route("")]
        public HttpResponseMessage GetUsers()
        {
            IEnumerable<UserModel> userModels;
            try
            {
                userModels = unitOfWork.UserRepository.Get().Select(x => Mapper.Map<UserModel>(x));
                return OK(userModels);

            }
            catch (Exception ex)
            {
                return InternalServerError("Error. Cannot run query over User database.",ex);
            }

            
        }

        //Post:
        //[Authorize(Roles = "admin,repairmen")]
        [Route("Send")]
        [HttpPut]
        public HttpResponseMessage SendMail(EmailModel em)
        {
            MailHelper email = new MailHelper(MailHelper.MailType.Notification, em.From, em.To, em.Subject, em.Body);
            try
            {
                email.SendEmail();
                return OK(new { Message = "Email send successfully!" });
            }
            catch (Exception ex)
            {
                return InternalServerError("Error. Cannot run query over database.", ex);
            }

        }
        // GET: 
        [Route("{id:Guid}")]
        [ResponseType(typeof(UserModel))]
        public HttpResponseMessage GetUser(Guid id)
        {
            UserModel user = new UserModel();
            try
            {
                user = Mapper.Map<UserModel>(unitOfWork.UserRepository.GetByID(id));
                return OK(user);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no User with provided Guid.");
            }
           
        }

        // GET: 
        [Route("user")]
        [ResponseType(typeof(UserModel))]
        public HttpResponseMessage GetIdByUsername(string username)
        {
            UserModel user = new UserModel();
            try
            {
                user =unitOfWork.UserRepository.Get(u => u.Email == username).Select(x=>Mapper.Map<UserModel>(x)).First();
                return OK(user);
            }
            catch(Exception ex)
            {
               
                return NotFound("Not Acceptable. There is no User with provided Guid.");
            }

        }

        [Route("facebook")]
        public HttpResponseMessage GetFacebookUser(string accessToken)
        {
            Facebook.FacebookClient client = new Facebook.FacebookClient(accessToken);
            dynamic me = client.Get("me");
            string email = me.email;
            UserModel user = new UserModel();
            try
            {
                user = unitOfWork.UserRepository.Get(u => u.Email == email).Select(x => Mapper.Map<UserModel>(x)).First();
                return OK(user);
            }
            catch (Exception ex)
            {

                return NotFound("Not Acceptable. There is no User with provided Guid.");
            }

        }
    }
}