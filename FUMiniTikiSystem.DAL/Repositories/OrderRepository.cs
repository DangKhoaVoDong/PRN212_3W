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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(FuminiTikiSystemContext context) : base(context)
        {
        }
        public Task<List<Order>> GetByCustomerIdAsync(int customerId)
        {
            return _dbSet.Where(p => p.CustomerId == customerId).ToListAsync();
        }
    }
}
