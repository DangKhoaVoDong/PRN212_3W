using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.BLL.Interfaces;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Interfaces;

namespace FUMiniTikiSystem.BLL.Services
{
    public class OrderService: IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public Task AddAsync(Order order)
        {
            var result = _repo.AddAsync(order);
            if(result != null) _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task DeleteAsync(Order order)
        {
            var result = await _repo.GetByIdAsync(order.OrderId);
            if(result != null)
            {
                _repo.Delete(result);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return (List<Order>)await _repo.GetAllAsync();
        }

        public Task<List<Order>> GetByCustomerIdAsync(int customerId)
        {
            return _repo.GetByCustomerIdAsync(customerId);
        }

        public async Task UpdateAsync(Order order)
        {
            var oldOrder = await _repo.GetByIdAsync(order.OrderId);
            if(oldOrder != null)
            {
                oldOrder.OrderDate = order.OrderDate;
                oldOrder.OrderAmount = order.OrderAmount;
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
