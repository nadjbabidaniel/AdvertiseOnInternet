using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using DAL;
using Repairmen.Models;
using RepairmenModel;
using AutoMapper;

namespace Repairmen.Helpers
{
    public class LoginCheck
    {
        private string bigHash { get; set; }
        private string randomClient { get; set; }
        private string passwordHash { get; set; }

        public LoginCheck( string BigHash, string ClientRandom, string PasswordHash)
        {
            this.bigHash = BigHash.ToLower();
            this.randomClient = ClientRandom;
            this.passwordHash = PasswordHash;
        }

        public bool isOk( string username )
        {
            // get servers random string from database:
            UnitOfWork uow = new UnitOfWork();
            RandomModel randomModel = new RandomModel();
            randomModel = Mapper.Map<RandomModel>(uow.RandomRepository.Get().Where(u => u.Username == username).FirstOrDefault());
            string randomS = randomModel.RandomString;
            Guid modelGuid = randomModel.Id;

            string input = this.randomClient + this.passwordHash + randomS;

            // generate hash
            string hash = "";
            SHA512 alg = SHA512.Create();
            byte[] data = alg.ComputeHash(Encoding.Default.GetBytes(input));
            string hex = BitConverter.ToString(data);
            hash = hex.Replace("-", "").ToLower();

            //compare hash with big hash
            if(hash == this.bigHash)
            {
                uow.RandomRepository.Delete(modelGuid);
                uow.Save();
                return true;
            }
            else
            {
                uow.RandomRepository.Delete(modelGuid);
                uow.Save();
                return false;                
            }

           
        }
    }
}