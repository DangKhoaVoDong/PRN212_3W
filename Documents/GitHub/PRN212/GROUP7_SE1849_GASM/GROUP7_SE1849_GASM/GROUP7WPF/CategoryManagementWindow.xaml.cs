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
    /// Interaction logic for CategoryManagementWindow.xaml
    /// </summary>
    public partial class CategoryManagementWindow : Window
    {
        private readonly CategoryService _categoryService;

        public CategoryManagementWindow()
        {
            InitializeComponent();

            var dbContext = new FuminiTikiSystemContext();
            var repo = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            _categoryService = new CategoryService(repo, unitOfWork);

            LoadCategories();
        }

        private async Task LoadCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            lvCategories.ItemsSource = categories;
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditCategoryWindow();
            if (addWindow.ShowDialog() == true)
            {
               await  LoadCategories();
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvCategories.SelectedItem as Category;
            if (selected == null)
            {
                MessageBox.Show("Please select a category to edit.");
                return;
            }

            var editWindow = new AddEditCategoryWindow(selected);
            if (editWindow.ShowDialog() == true)
            {
                await LoadCategories();
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvCategories.SelectedItem as Category;
            if (selected == null)
            {
                MessageBox.Show("Please select a category to delete.");
                return;
            }

            var confirm = MessageBox.Show($"Delete '{selected.Name}'?", "Confirm", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                await _categoryService.DeleteAsync(selected);
                LoadCategories();
            }
        }
    }
}
