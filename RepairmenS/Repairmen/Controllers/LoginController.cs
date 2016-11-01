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
using Repairmen.Models;
using DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repairmen.Helpers;
using System.Web.Http.Cors;
using System.Runtime.ExceptionServices;
using System.Diagnostics;
using AutoMapper;
using System.Data.Entity.Core;
using Facebook;
using Repairmen.Controllers.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;


namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Login")]
    public class LoginController : RepairmenApiControllerBase
    {
        private IUnitOfWork unitOfWork;

        public LoginController()
        {
            unitOfWork = new UnitOfWork();
        }

        public LoginController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public HttpResponseMessage Post(UserModel user)
        {
            return Create(user);
        }


        // GET: api/Login/UserExists/email
        [Route("UserExists")]
        [HttpGet]
        public HttpResponseMessage UserExists(string email)
       {
            UserModel user;
            object result;
            try
            {
                // try to get user by username
                user = Mapper.Map<UserModel>(unitOfWork.UserRepository.Get(u => u.Email == email).FirstOrDefault());
                result = new { isUnique = "false" };
            }
            catch (ObjectNotFoundException)
            {
                result = new { isUnique = "true" };
            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error. Can not create json object.",ex);
            }
            return OK(result);
        }

        // GET: api/login/initiate
        [Route("initiate")]
        [HttpGet]
        public HttpResponseMessage RandomString(string username)
        {
             RandomString randomString = new RandomString(32);
             try
             {
                 string rndStr = randomString.GenerateRandomString(username);
                 return OK (new { RndSrv = rndStr});
             }
            catch (Exception ex)
             {
                 return InternalServerError("Server Error: Can not create Random String.",ex);
             }             
        }

        //GET: api/login/activate/{code}
        [Route("activate/{code}")]
        [HttpGet]
        public HttpResponseMessage Authenticate(string code)
        {
            try
            {
                ActivateHelper ah = new ActivateHelper(code);
                string rez = ah.Authenticate();
                if (rez == "true")
                {
                    return OK(new { x = "You've successfully authenticated! You can Log In now." });
                }
                else
                {
                    Exception ex = new Exception("Sorry, cannot authenticate");
                    return InternalServerError("Sorry, cannot authenticate.", ex);
                }
   
            }
            catch (Exception ex)
            {
                 return InternalServerError("Sorry, cannot authenticate.",ex);
            }
        }

        //PUT: api/Login/resetPassword/{email}
        [Route("resetPassword")]
        [HttpPut]
        public HttpResponseMessage ResetPassword(UserModel userModel)
        {
            try
            {
                ResetPasswordHelper passHelper = new ResetPasswordHelper(userModel.Email);
                string rez = passHelper.SendResetMail();
                if (rez == "OK")
                {
                    return OK(new { Message = "Success. Check your email for reset password." });
                }
                else if(rez=="Forbidden")
                {
                    return Forbidden();
                }
                else
                {
                    Exception ex = new Exception(rez);
                    return InternalServerError("Sorry, cannot reset password.", ex);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError("Sorry, cannot reset password.", ex);
            }
        }
        // PUT api/Login
        //[ValidationResponseFilter]
        [Route("")]
        [HttpPut]
        public HttpResponseMessage PutUser(UserModel userModel)
        {
            // Get Guid for RoleName:
            string defaultRoleName = "repairmen";
            RoleModel role = Mapper.Map<RoleModel>(unitOfWork.RoleRepository.Get(r => r.Name == defaultRoleName).FirstOrDefault());
            try
            {
                var user = Mapper.Map<User>(userModel);
                user.Locked = true;
                user.SignupDate = DateTime.Now;
                user.NotifyEmail = false;
                user.NotifySMS = false;
                user.PasswordChange = false;
                if (!string.IsNullOrEmpty(userModel.PhoneNumber))
                {
                    user.PhoneNumber = "+38111111111";
                }
                unitOfWork.UserRepository.Insert(user);
                unitOfWork.Save();
                sendAuthentication(user);
                return Create();
            }
            catch (Exception ex)
            {
                return InternalServerError("Insert Denied: Unique key constraint violated. Cannot insert in LoginController duplicate username. Username must be unique.", ex);
            }
        }

        // Helper function to create and send authentication mail....
        private void sendAuthentication(User um)
        {
            RandomString randomString = new RandomString(32);
            string rndStr = randomString.GenerateRandomString(um.Id.ToString());
            string idToVerify = um.Id.ToString();
                //generate hash:
            string input = rndStr.ToLower() + um.Password;
            string hash = "";
            SHA512 alg = SHA512.Create();
            byte[] data = alg.ComputeHash(Encoding.Default.GetBytes(input));
            string hex = BitConverter.ToString(data);
            hash = hex.Replace("-", "").ToLower();

            CustomConfig Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
            string clientRoot = Config.ClientRoot;
            string actCode = clientRoot + "login/activate/" + idToVerify + "_" + hash;

            string body = "You create account on repairmen.com. \n To activate your account, click this link:\n  " + actCode;
            MailHelper mail = new MailHelper(MailHelper.MailType.Registration, "noreply@repairmen.com", um.Email, "Activation email", body);
            mail.SendEmail();
        }

    }
}