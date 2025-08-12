using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Interfaces;
using FUMiniTikiSystem.DAL.Repositories;

namespace FUMiniTikiSystem.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FuminiTikiSystemContext _context;

        public UnitOfWork(FuminiTikiSystemContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync() =>  _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }

}
