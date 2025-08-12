using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Repositories;

namespace FUMiniTikiSystem.DAL.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> LoginAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int customerId, string oldPassword, string newPassword);
        Task<bool> RegisterAsync(string name, string email, string password);

    }

}
