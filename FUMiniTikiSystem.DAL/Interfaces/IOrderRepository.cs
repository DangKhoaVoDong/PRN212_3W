using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.DAL.Entities;

namespace FUMiniTikiSystem.DAL.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        public Task<List<Order>> GetByCustomerIdAsync(int customerId);
    }
}
