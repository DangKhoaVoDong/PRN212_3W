using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUMiniTikiSystem.DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(FuminiTikiSystemContext context) : base(context)
        {    
        }
        public new async Task<IEnumerable<Product>> GetAllAsync() => await _dbSet.Include("Category").ToListAsync();
    }
}
