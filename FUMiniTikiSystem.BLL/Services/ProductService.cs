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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public Task AddAsync(Product product)
        {
           var result = _repo.AddAsync(product);
            if(result !=null) _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task DeleteAsync(Product product)
        {
            var result = await _repo.GetByIdAsync(product.ProductId);
            if(result != null)
            {
                _repo.Delete(result);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return (List<Product>)await _repo.GetAllAsync() ;
        }

        public async Task UpdateAsync(Product product)
        {
            var oldProduct = await _repo.GetByIdAsync(product.ProductId);
            if(oldProduct != null)
            {
                oldProduct.Price = product.Price;
                oldProduct.Name = product.Name;
                oldProduct.Description = product.Description;
                oldProduct.ImagePath = product.ImagePath;
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task<List<Product>> SearchAsync(string keyword)
        {
            var allProducts = await _repo.GetAllAsync();

            if (string.IsNullOrWhiteSpace(keyword))
                return allProducts.ToList();

            keyword = keyword.ToLower();

            return allProducts
                .Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(keyword)))
                .ToList();
        }
    }
}
