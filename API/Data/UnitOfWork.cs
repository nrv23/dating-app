using API.interfaces;
using API.Repository;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public IUserRespository UserRespository => new UserRepository(this.context, this.mapper);

        public IMessageRepository MessageRepository => new MessageRepository(this.context, this.mapper);

        public ILikeRepository LikeRepository => new LikeRepository(this.context);

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> Completed()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges(); // valida si hay cambios rastreados por ef
        }
    }
}