using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Security.Cryptography;
using RepairmenModel;
using System.Text;
using Repairmen.Models;
using AutoMapper;

namespace Repairmen.Controllers.Helpers
{
    public class ActivateHelper
    {
        private string inputHash { get; set; }
        private string id { get; set; }

        public ActivateHelper (string code)
        {
            id = code.Substring(0, code.IndexOf("_"));          
            inputHash = code.Substring(code.IndexOf("_") + 1);
        }

        public string Authenticate()
        {
            // run logic to authenticate and activate user account.
            Guid uID = new Guid(id);

                UnitOfWork uow = new UnitOfWork();
                User userData = null;
                try
                {
                    userData = uow.UserRepository.Get(u => u.Id == uID).FirstOrDefault();
                }
                catch
                {
                    return "false";
                }
                string password = userData.Password;
                RepairmenModel.Random rnd = uow.RandomRepository.Get(u => u.Username == id).FirstOrDefault();
                string randomStr = rnd.RandomString.ToLower();
                string input = randomStr + password;
                string hash = "";
                SHA512 alg = SHA512.Create();
                byte[] data = alg.ComputeHash(Encoding.Default.GetBytes(input));
                string hex = BitConverter.ToString(data);
                hash = hex.Replace("-", "").ToLower();
                string inHash = inputHash;

                if (hash == inHash)
                {
                    //activate account:
                    userData.Locked = false;
                    uow.UserRepository.Update(userData);
                    uow.Save();
                    DeleteOldStrings(id);
                    return "true";
                    
                }
                else
                {
                    DeleteOldStrings(id);
                    uow.UserRepository.Delete(userData);
                    uow.Save();
                    return "false";
                }
            
        }

        private void DeleteOldStrings(string uName)
        {
            IEnumerable<RandomModel> randomModels;
            UnitOfWork unitOfWork = new UnitOfWork();
            try
            {
                randomModels = unitOfWork.RandomRepository.Get(c => c.Username == uName).Select(x => Mapper.Map<RandomModel>(x));
                foreach (RandomModel rm in randomModels)
                {
                    RepairmenModel.Random rnd = Mapper.Map<RepairmenModel.Random>(rm);
                    unitOfWork.RandomRepository.Delete(rnd.Id);
                    unitOfWork.Save();
                }
            }
            catch
            {

            }
        }
    }
}