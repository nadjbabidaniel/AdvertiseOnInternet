using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using RepairmenModel;
using Repairmen.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace Repairmen.Controllers.Helpers
{
    public class ResetPasswordHelper
    {
        private string email { get; set; }

        public ResetPasswordHelper(string mail)
        {
            email = mail;
        }

        public string SendResetMail()
        {
            IUnitOfWork uow = new UnitOfWork();
            Guid userID;
            try
            {
                User user = uow.UserRepository.Get(u => u.Email == email).FirstOrDefault();
                if (user.LoginFlag == "SN")
                {
                    return "Forbidden";
                }
                else
                {
                    userID = user.Id;
                    //generate random string and write it to database
                    RandomString rndStr = new RandomString(32);
                    string randomString = rndStr.GenerateRandomString(email);
                    // generate new password
                    string input = randomString + userID.ToString();
                    string hash = "";
                    SHA512 alg = SHA512.Create();
                    byte[] data = alg.ComputeHash(Encoding.Default.GetBytes(input));
                    string hex = BitConverter.ToString(data);
                    hash = hex.Replace("-", "").ToLower();
                    //write new password to database
                    user.Password = hash;
                    user.PasswordChange = true;
                    uow.UserRepository.Update(user);
                    uow.Save();
                    //send email with link for reset password
                    CustomConfig Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
                    string clientRoot = Config.ClientRoot;
                    string actCode = clientRoot + "login/updatePassword/" + input;
                    string body = "You requested reset password on website repairmen.com. \n To reset your password, click this link:\n  " + actCode;
                    MailHelper mail = new MailHelper(MailHelper.MailType.Registration, "noreply@repairmen.com", user.Email, "Reset password", body);
                    mail.SendEmail();

                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
    }
}