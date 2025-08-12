using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.DAL.Entities;

namespace FUMiniTikiSystem.BLL.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer?> LoginAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int customerId, string oldPassword, string newPassword);
        Task LogoutAsync();
        Task<List<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
        Task<bool> RegisterAsync(string name, string email, string password);
    }
}
