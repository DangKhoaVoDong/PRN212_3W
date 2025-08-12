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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public Task AddAsync(Category category)
        {
            var result = _repo.AddAsync(category);
            if (result != null) _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task DeleteAsync(Category category)
        {
            var result = await _repo.GetByIdAsync(category.CategoryId);
            if (result != null)
            {
                _repo.Delete(result);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return (List<Category>)await _repo.GetAllAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var oldProduct = await _repo.GetByIdAsync(category.CategoryId);
            if (oldProduct != null)
            {
                oldProduct.Picture = category.Picture;
                oldProduct.Name = category.Name;
                oldProduct.Description = category.Description;
                await _unitOfWork.SaveChangesAsync();
            }
        }
        
    }
}
