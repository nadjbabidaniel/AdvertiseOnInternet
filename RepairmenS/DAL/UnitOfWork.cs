using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepairmenModel;


namespace DAL
{
    

    public class UnitOfWork : IUnitOfWork
    {
        private repairmenEntities context = new repairmenEntities();
        private GenericRepository<Ad> adRepository;
        private GenericRepository<User> userRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Role> roleRepository;
        private GenericRepository<Rating> ratingRepository;
        private GenericRepository<City> cityRepository;

        private GenericRepository<RepairmenModel.Random> randomRepository;
        private GenericRepository<Comment> commentRepository;
        private GenericRepository<InappropriateComment> inappropriateCommentRepository;
        private GenericRepository<InappropriateAd> inappropriateAdRepository;
        private GenericRepository<CommentVote> commentVoteRepository;

        public IGenericRepository<Ad> AdRepository
        {
            get
            {
                if (this.adRepository == null)
                {
                    this.adRepository = new GenericRepository<Ad>(context);
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
                    this.ratingRepository = new GenericRepository<Rating>(context);
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
                    this.userRepository = new GenericRepository<User>(context);
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
                    this.categoryRepository = new GenericRepository<Category>(context);
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
                    this.roleRepository = new GenericRepository<Role>(context);
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
                    this.cityRepository = new GenericRepository<City>(context);
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
                    this.randomRepository = new GenericRepository<RepairmenModel.Random>(context);
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
                    this.commentRepository = new GenericRepository<Comment>(context);
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
                    this.inappropriateCommentRepository = new GenericRepository<InappropriateComment>(context);
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
                    this.inappropriateAdRepository = new GenericRepository<InappropriateAd>(context);
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
                    this.commentVoteRepository = new GenericRepository<CommentVote>(context);
                }
                return commentVoteRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

   
}