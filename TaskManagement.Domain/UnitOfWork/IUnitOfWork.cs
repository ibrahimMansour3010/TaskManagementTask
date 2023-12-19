using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Respiratory;

namespace TaskManagement.Domain.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepo<T> Repository<T>() where T : class;

        void Complete();
        Task<bool> CompleteAsync();
    }
}
