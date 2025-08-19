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
using System.Xml.Linq;
using FUMiniTikiSystem.BLL.Services;
using Microsoft.Win32;
using IOPath = System.IO.Path;
using System.IO;

namespace GROUP7WPF
{
    /// <summary>
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private readonly ProductService _productService;
        private readonly CategoryRepository _categoryRepo;
        private readonly Product? _editingProduct;

        public AddProductWindow(Product? product = null)
        {
            InitializeComponent();

            var context = new FuminiTikiSystemContext();
            _productService = new ProductService(new ProductRepository(context), new UnitOfWork(context));
            _categoryRepo = new CategoryRepository(context);

            _editingProduct = product;
            LoadCategories();

            if (_editingProduct != null)
            {
                Title = "Edit Product";
                txtName.Text = _editingProduct.Name;
                txtPrice.Text = _editingProduct.Price.ToString();
                txtDescription.Text = _editingProduct.Description;
                txtImagePath.Text = _editingProduct.ImagePath;
            }
            else
            {
                Title = "Add Product";
            }
        }

        private async void LoadCategories()
        {
            var categories = await _categoryRepo.GetAllAsync();
            cbCategory.ItemsSource = categories;

            if (_editingProduct != null)
            {
                cbCategory.SelectedItem = categories.FirstOrDefault(c => c.CategoryId == _editingProduct.CategoryId);
            }
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Product Image",
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp;*.gif)|*.png;*.jpeg;*.jpg;*.bmp;*.gif|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtImagePath.Text = openFileDialog.FileName;
            }
        }

        private string CopyImageToLocalFolder(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return null;

            try
            {
                // Tạo thư mục Images nếu chưa tồn tại
                string imagesFolder = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                // Tạo tên file mới với timestamp để tránh trùng lặp
                string fileName = IOPath.GetFileName(sourcePath);
                string fileNameWithoutExt = IOPath.GetFileNameWithoutExtension(fileName);
                string extension = IOPath.GetExtension(fileName);
                string newFileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                string destinationPath = IOPath.Combine(imagesFolder, newFileName);

                // Copy file
                File.Copy(sourcePath, destinationPath, true);

                // Trả về đường dẫn tuyệt đối để lưu vào database
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string priceText = txtPrice.Text.Trim();
            string description = txtDescription.Text.Trim();
            string imagePath = txtImagePath.Text.Trim();
            var selectedCategory = cbCategory.SelectedItem as Category;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceText) || selectedCategory == null)
            {
                MessageBox.Show("Please fill all fields and select a category.");
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Invalid price.");
                return;
            }

            // Copy image to local folder if provided
            string finalImagePath = null;
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                finalImagePath = CopyImageToLocalFolder(imagePath);
            }

            if (_editingProduct == null)
            {
                var newProduct = new Product
                {
                    Name = name,
                    Price = price,
                    Description = description,
                    ImagePath = finalImagePath,
                    CategoryId = selectedCategory.CategoryId
                };

                await _productService.AddAsync(newProduct);
            }
            else
            {
                _editingProduct.Name = name;
                _editingProduct.Price = price;
                _editingProduct.Description = description;
                _editingProduct.ImagePath = finalImagePath ?? _editingProduct.ImagePath;
                _editingProduct.CategoryId = selectedCategory.CategoryId;

                await _productService.UpdateAsync(_editingProduct);
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
