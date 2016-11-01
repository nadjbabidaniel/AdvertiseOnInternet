using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using DAL;
using AutoMapper;
using RepairmenModel;
using Repairmen.Models;

namespace Repairmen.Helpers
{
    public class RandomString
    {
        private int strLength { get; set; }   // string length

        public RandomString(int stringLength)
        {
            this.strLength = stringLength;
        }

        public string GenerateRandomString(string username)
        {
            DeleteOldStrings(username);

            string rndStr = "";
            System.Random random = new System.Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < strLength; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            rndStr = builder.ToString();

            // save random string and username into database
            RandomModel randomModel = new RandomModel();
            randomModel.Id = Guid.NewGuid();
            randomModel.Username = username;
            randomModel.RandomString = rndStr;

            var rnd = Mapper.Map<RepairmenModel.Random>(randomModel);
            UnitOfWork uow = new UnitOfWork();
            uow.RandomRepository.Insert(rnd);
            uow.Save();
            return rndStr;
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