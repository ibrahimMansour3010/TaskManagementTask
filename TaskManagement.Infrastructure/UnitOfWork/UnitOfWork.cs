using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Respiratory;
using TaskManagement.Domain.Shared.Helpers;
using TaskManagement.Domain.UnitOfWork;
using TaskManagement.Infrastructure.Context;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDBContext _context;
        private UserResolverService _userService;

        public UnitOfWork (AppDBContext context,
            UserResolverService userService)
        {
            _context = context;
            _userService = userService;
        }

        public Dictionary<string, object> repositories;
        public IGenericRepo<T> Repository<T>() where T : class
        {
            if (repositories == null)
                repositories = new Dictionary<string, object>();

            string type = typeof(T).Name;
            if (!repositories.ContainsKey(type))
            {
                var repositoryInstance = Activator.CreateInstance(typeof(GenericRepo<>).MakeGenericType(typeof(T)), _context);
                repositories.Add(type, repositoryInstance);
            }
            return (GenericRepo<T>)repositories[type];
        }

        public void Complete()
        {
            _context.AddCreatedBy(_userService.GetNameIdentifier());
            _context.SaveChanges();
        }

        public async Task<bool> CompleteAsync()
        {
            _context.AddCreatedBy(_userService.GetNameIdentifier());
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
