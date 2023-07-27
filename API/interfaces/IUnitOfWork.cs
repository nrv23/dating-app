using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.interfaces
{
    public interface IUnitOfWork
    {
        IUserRespository UserRespository{get;}
        IMessageRepository MessageRepository{get;}
        ILikeRepository LikeRepository{get;}
        Task<bool> Completed(); // esta funcion guarda todos los cambios
        bool HasChanges(); // rastrea los cambios globalmente mediante entity framework

    }
}