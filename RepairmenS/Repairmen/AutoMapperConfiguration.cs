using AutoMapper;
using Repairmen.Models;
using RepairmenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repairmen
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<Ad, AdModel>().ReverseMap();
            Mapper.CreateMap<Category, CategoryModel>().ReverseMap();
            Mapper.CreateMap<City, CityModel>().ReverseMap();
            Mapper.CreateMap<Role, RoleModel>().ReverseMap();
            Mapper.CreateMap<User, UserModel>().ReverseMap();
            Mapper.CreateMap<RepairmenModel.Random, RandomModel>().ReverseMap();
            Mapper.CreateMap<Rating, RatingModel>().ReverseMap();
            Mapper.CreateMap<Comment, CommentModel>().ReverseMap();
            Mapper.CreateMap<InappropriateComment, InappropriateCommentModel>().ReverseMap();
            Mapper.CreateMap<InappropriateAd, InappropriateAdModel>().ReverseMap();
            Mapper.CreateMap<CommentVote, CommentVoteModel>().ReverseMap();
            Mapper.Configuration.AllowNullDestinationValues = false;
        }
    }
}