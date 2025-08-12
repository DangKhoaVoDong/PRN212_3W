using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Repositories;
using FUMiniTikiSystem.DAL;
using FUMiniTikiSystem.BLL.Services;

namespace GROUP7WPF
{
    /// <summary>
    /// Interaction logic for AddEditCategoryWindow.xaml
    /// </summary>
    public partial class AddEditCategoryWindow : Window
    {
        private readonly CategoryService _categoryService;
        private readonly Category? _editingCategory;

        public AddEditCategoryWindow(Category? category = null)
        {
            InitializeComponent();

            var dbContext = new FuminiTikiSystemContext();
            var repo = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            _categoryService = new CategoryService(repo, unitOfWork);

            _editingCategory = category;

            if (_editingCategory != null)
            {
                Title = "Edit Category";
                txtName.Text = _editingCategory.Name;
                txtDescription.Text = _editingCategory.Description ?? "";
                
            }
            else
            {
                Title = "Add Category";
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();
            

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.");
                return;
            }

            if (_editingCategory == null)
            {
                // Add
                var newCategory = new Category
                {
                    Name = name,
                    Description = description,
                    
                };
                await _categoryService.AddAsync(newCategory);
            }
            else
            {
                // Update
                var updatedCategory = new Category
                {
                    CategoryId = _editingCategory.CategoryId,
                    Name = name,
                    Description = description,
                    
                };
                await _categoryService.UpdateAsync(updatedCategory);
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
