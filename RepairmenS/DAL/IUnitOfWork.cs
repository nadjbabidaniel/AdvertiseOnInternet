using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepairmenModel;

namespace DAL
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        IGenericRepository<Ad> AdRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<City> CityRepository { get; }
        IGenericRepository<Rating> RatingRepository { get; }
        IGenericRepository<RepairmenModel.Random> RandomRepository { get; }
        IGenericRepository<Comment> CommentRepository { get; }
        IGenericRepository<InappropriateComment> InappropriateCommentRepository { get; }
        IGenericRepository<InappropriateAd> InappropriateAdRepository { get; }
        IGenericRepository<CommentVote> CommentVoteRepository { get; }

    }
}
