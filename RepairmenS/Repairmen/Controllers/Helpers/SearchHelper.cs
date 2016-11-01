using Common;
using Repairmen.Models;
using RepairmenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Configuration;

namespace Repairmen.Helpers
{
    public class SearchHelper
    {
        public bool AdSearchCriteria(Ad a, AdQueryModel query)
        {
            bool categoryBool = query.Category != null ? (query.Category.Id == a.CategoryId) : true;
            bool cityBool = query.City != null ? (query.City.Id == a.CityId) : true;
            bool keywordBool = !string.IsNullOrWhiteSpace(query.Keyword) ? EvaluateKeywords(query.Keyword, a.Name, a.Description) : true;
            return categoryBool && cityBool && keywordBool;
        }

        public bool PaidAdSearchCriteria(Ad a, AdQueryModel query)
        {
            bool categoryBool = query.Category != null ? (query.Category.Id == a.CategoryId) : true;
            bool cityBool = query.City != null ? (query.City.Id == a.CityId) : true;
            return categoryBool && cityBool && a.IsPaid.Value;
        }

        private bool EvaluateKeywords(string keywords, string name, string description)
        {
            keywords = keywords.Replace(",", " ").Replace(".", " ").Replace("+", " ").Replace("-", " ");
            string[] parts = keywords.ToLowerInvariant().Split(' ');
            string nameDesc = name.ToLower() + description.ToLower();
            foreach (string part in parts)
            {
                if (!nameDesc.Contains(part.Trim()))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<AdModel> SortResults(SortBy sortBy, SortDirection sortDir, int noOfItems, int pageNumber, IEnumerable<AdModel> adModel)
        {
            switch (sortBy)
            {
                case SortBy.Date:
                    if (sortDir == SortDirection.Asc)
                        adModel = adModel.OrderBy(x => x.Date).Page(pageNumber, noOfItems);
                    else
                        adModel = adModel.OrderByDescending(x => x.Date).Page(pageNumber, noOfItems);
                    break;
                case SortBy.Comment:
                    if (sortDir == SortDirection.Asc)
                        adModel = adModel.OrderBy(x => x.CommentCounter).Page(pageNumber, noOfItems);
                    else
                        adModel = adModel.OrderByDescending(x => x.CommentCounter).Page(pageNumber, noOfItems);
                    break;
                case SortBy.Rating:
                    if (sortDir == SortDirection.Asc)
                        adModel = adModel.OrderBy(x => x.AvgRate).Page(pageNumber, noOfItems);
                    else
                        adModel = adModel.OrderByDescending(x => x.AvgRate).Page(pageNumber, noOfItems);
                    break;
                case SortBy.Name:
                    if (sortDir == SortDirection.Asc)
                        adModel = adModel.OrderBy(x => x.Name).Page(pageNumber, noOfItems);
                    else
                        adModel = adModel.OrderByDescending(x => x.Name).Page(pageNumber, noOfItems);
                    break;
                default:
                    adModel = null;
                    break;
            }
            return adModel;
        }
    }
}