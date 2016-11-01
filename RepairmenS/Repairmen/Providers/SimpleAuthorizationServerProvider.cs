using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DAL;
using System.Security.Claims;
using Repairmen.Models;
using Repairmen.Helpers;
using AutoMapper;
using Facebook;
using RepairmenModel;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Plus.v1;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Threading;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Repairmen.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private string roleName;
        private IUnitOfWork unitOfWork;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            context.Validated();
        }


        // Login for check is users authentication parameters are OK and generate token.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            Guid userid;

            using (UnitOfWork _repo = new UnitOfWork())
            {
                UserModel user = null;
                RoleModel roleMod = null;
                Guid role;
                unitOfWork = new UnitOfWork();
                // trying to get user by given username
                if (context.Password.Length > 100)
                {
                    try
                    {
                        user = Mapper.Map<UserModel>(unitOfWork.UserRepository.Get(u => u.Email == context.UserName).First());
                    }
                    catch
                    {
                        context.SetError("invalid_grant", "The user does not exists.");
                        return;
                    }
                    if(user.Locked == true)
                    {
                        context.SetError("invalid_grant", "Your account is not acivated yet.");
                        return;
                    }
                    // get roleID of user and put in variable "role"
                    role = user.RoleId;
                    roleMod = Mapper.Map<RoleModel>(_repo.RoleRepository.Get().Where(r => r.Id == role).First());
                    roleName = roleMod.Name;
                    userid = user.Id;

                    // Check if username and password are correct
                    bool isLoggedIn = false;
                    string concatenated = context.Password;
                    string rndClient = concatenated.Substring(0, 32);
                    string bigHash = concatenated.Substring(32);
                    string hashPassword = user.Password;
                    LoginCheck logCheck = new LoginCheck(bigHash, rndClient, hashPassword);
                    isLoggedIn = logCheck.isOk(user.Email);

                    if (isLoggedIn == false)
                    {
                        context.SetError("invalid_grant", "Wrong password.");
                        return;
                    }
                }


                else
                {
                    try
                    {
                        if (context.Password.Equals("Gplus"))
                        {
                            user = GPLogin(context, _repo);

                        }
                        else if (context.Password.Equals("FB"))
                        {
                            user = FBLogin(context, _repo);
                        }
                        role = user.RoleId;
                        roleMod = Mapper.Map<RoleModel>(_repo.RoleRepository.Get().Where(r => r.Id == role).FirstOrDefault());
                        roleName = roleMod.Name;
                        userid = user.Id;
                    }
                    catch (Exception e)
                    {
                        context.SetError("invalid_grant", "Server error");
                        return;
                    }

                }
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.UserData, userid.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));

            context.Validated(identity);

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            context.AdditionalResponseParameters.Add("Role", roleName);

            return Task.FromResult<object>(null);
        }

        protected UserModel FBLogin(OAuthGrantResourceOwnerCredentialsContext context, UnitOfWork _repo)
        {
            UserModel user = null;
            FacebookClient client = new FacebookClient(context.UserName);
            dynamic me = client.Get("me");
            user = Mapper.Map<UserModel>(_repo.UserRepository.Get().Where(u => u.Email == me.email).FirstOrDefault());
            if (user == null)
            {
                DateTime signDate = DateTime.Now;
                user = new UserModel()
                {
                    FirstName = me.first_name,
                    LastName = me.last_name,
                    Username = me.first_name + " " + me.last_name,
                    Email = me.email,
                    Password = "FB User",
                    RoleId = Guid.Parse("56dc86b4-1229-e411-9417-a41f7255f9b5"),
                    SignupDate = signDate,
                    LoginFlag = "SN",
                    NotifyEmail = false,
                    NotifySMS = false,
                    Locked = false,
                    PhoneNumber = "+38111111111",
                    PasswordChange = false
                };
                unitOfWork.UserRepository.Insert(Mapper.Map<User>(user));
                unitOfWork.Save();
                user.Id = unitOfWork.UserRepository.Get(u => u.SignupDate.ToString() == signDate.ToString()).FirstOrDefault().Id;
            }
            return user;
        }
        protected UserModel GPLogin(OAuthGrantResourceOwnerCredentialsContext context, UnitOfWork _repo)
        {
            UserModel user = null;
            dynamic data = JObject.Parse(context.UserName);
            string mail = data.email;
            user = Mapper.Map<UserModel>(_repo.UserRepository.Get().Where(u => u.Email == mail).FirstOrDefault());
            if (user == null)
            {
                DateTime signDate = DateTime.Now;
                user = new UserModel()
                {
                    FirstName = data.firstName,
                    LastName = data.lastName,
                    Username = data.firstName+" "+data.lastName,
                    Email = data.email,
                    Password = "G+ User",
                    RoleId = Guid.Parse("56dc86b4-1229-e411-9417-a41f7255f9b5"),
                    SignupDate = signDate,
                    LoginFlag="SN",
                    NotifyEmail = false,
                    NotifySMS = false,
                    Locked = false,
                    PhoneNumber = "+38111111111",
                    PasswordChange = false
                };
                unitOfWork.UserRepository.Insert(Mapper.Map<User>(user));
                unitOfWork.Save();
                user.Id = unitOfWork.UserRepository.Get(u => u.SignupDate.ToString() == signDate.ToString()).FirstOrDefault().Id;
            }
            return user;
        }
    }
}