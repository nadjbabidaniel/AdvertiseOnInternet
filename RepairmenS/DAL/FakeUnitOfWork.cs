using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepairmenModel;

namespace DAL
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        private FakeGenericRepository<Ad> adRepository;
        private FakeGenericRepository<User> userRepository;
        private FakeGenericRepository<Category> categoryRepository;
        private FakeGenericRepository<Role> roleRepository;
        private FakeGenericRepository<City> cityRepository;
        private FakeGenericRepository<Rating> ratingRepository;
        private FakeGenericRepository<RepairmenModel.Random> randomRepository;
        private FakeGenericRepository<Comment> commentRepository;
        private FakeGenericRepository<InappropriateComment> inappropriateCommentRepository;
        private FakeGenericRepository<InappropriateAd> inappropriateAdRepository;
        private FakeGenericRepository<CommentVote> commentVoteRepository;

        public Dictionary<Guid, User> Users { get; set; }
        public Dictionary<Guid, Category> Categories { get; set; }
        public Dictionary<Guid, City> Cities { get; set; }
        public Dictionary<Guid, Ad> Ads { get; set; }
        public Dictionary<Guid, Role> Roles { get; set; }
        public Dictionary<Guid, Rating> Ratings { get; set; }
        public Dictionary<Guid, RepairmenModel.Random> Randoms { get; set; }
        public Dictionary<Guid, Comment> Comments { get; set; }
        public Dictionary<Guid, InappropriateComment> InappropriateComments { get; set; }
        public Dictionary<Guid, InappropriateAd> InappropriateAds { get; set; }
        public Dictionary<Guid, CommentVote> CommentVote { get; set; }



        public IGenericRepository<Ad> AdRepository
        {
            get
            {
                if (this.adRepository == null)
                {
                    this.adRepository = new FakeGenericRepository<Ad>(Ads);
                }
                return adRepository;
            }
        }

        public IGenericRepository<Rating> RatingRepository
        {
            get
            {
                if (this.ratingRepository == null)
                {
                    this.ratingRepository = new FakeGenericRepository<Rating>(Ratings);
                }
                return ratingRepository;
            }
        }
        public IGenericRepository<User> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new FakeGenericRepository<User>(Users);
                }
                return userRepository;
            }
        }

        public IGenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new FakeGenericRepository<Category>(Categories);
                }
                return categoryRepository;
            }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get
            {
                if (this.roleRepository == null)
                {
                    this.roleRepository = new FakeGenericRepository<Role>(Roles);
                }
                return roleRepository;
            }
        }

        public IGenericRepository<City> CityRepository
        {
            get
            {
                if (this.cityRepository == null)
                {
                    this.cityRepository = new FakeGenericRepository<City>(Cities);
                }
                return cityRepository;
            }
        }

        public IGenericRepository<RepairmenModel.Random> RandomRepository
        {
            get
            {
                if(this.randomRepository == null)
                {
                    this.randomRepository = new FakeGenericRepository<RepairmenModel.Random>(Randoms);
                }
                return randomRepository;
            }
        }

        public IGenericRepository<Comment> CommentRepository
        {
            get
            {
                if (this.commentRepository == null)
                {
                    this.commentRepository = new FakeGenericRepository<Comment>(Comments);
                }
                return commentRepository;
            }
        }

        public IGenericRepository<InappropriateComment> InappropriateCommentRepository
        {
            get
            {
                if (this.inappropriateCommentRepository == null)
                {
                    this.inappropriateCommentRepository = new FakeGenericRepository<InappropriateComment>(InappropriateComments);
                }
                return inappropriateCommentRepository;
            }
        }

        public IGenericRepository<InappropriateAd> InappropriateAdRepository
        {
            get
            {
                if (this.inappropriateAdRepository == null)
                {
                    this.inappropriateAdRepository = new FakeGenericRepository<InappropriateAd>(InappropriateAds);
                }
                return inappropriateAdRepository;
            }
        }

        public IGenericRepository<CommentVote> CommentVoteRepository
        {
            get
            {
                 if (this.commentVoteRepository == null)
                 {
                     this.commentVoteRepository = new FakeGenericRepository<CommentVote>(CommentVote);
                 }
                return  commentVoteRepository;
            }
        }

        public void Save()
        {
            return;
        }

        public void Dispose()
        {
            return;
        }
    }
}
