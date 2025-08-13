using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.BLL.Interfaces;
using FUMiniTikiSystem.DAL;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Interfaces;
using FUMiniTikiSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FUMiniTikiSystem.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(ICustomerRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ChangePasswordAsync(int customerId, string oldPassword, string newPassword)
        {
            var result = await _repo.ChangePasswordAsync(customerId, oldPassword, newPassword);
            if(result) await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            return await _repo.LoginAsync(email, password);
        }


        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return (List<Customer>)await _repo.GetAllAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _repo.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            Customer? oldCustomer = await _repo.GetByIdAsync(customer.CustomerId);
            if(oldCustomer != null)
            {
                oldCustomer.Password = customer.Password;
                oldCustomer.Email = customer.Email;
                oldCustomer.Name = customer.Name;
               await  _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Customer customer)
        {
            Customer? oldCustomer = await _repo.GetByIdAsync(customer.CustomerId);
            if(oldCustomer != null)
            {
                _repo.Delete(customer);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task<bool> RegisterAsync(string name, string email, string password)
        {
            return await _repo.RegisterAsync(name, email, password);
        }
    }
}
