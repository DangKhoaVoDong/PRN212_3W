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
    public class CustomerRepository :Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(FuminiTikiSystemContext context) : base(context) { }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            return await  _dbSet.FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }

        public async Task<bool> ChangePasswordAsync(int customerId, string oldPassword, string newPassword)
        {
            var customer = await _dbSet.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if(customer == null || customer.Password != oldPassword)
                return false;
            customer.Password = newPassword;
            _context.Update(customer);
            return true;
        }
        public async Task<bool> RegisterAsync(string name, string email, string password)
        {

            var existingCustomer = await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
            if (existingCustomer != null)
                return false;

            var newCustomer = new Customer
            {
                Name = name,
                Email = email,
                Password = password
            };

            await _dbSet.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
