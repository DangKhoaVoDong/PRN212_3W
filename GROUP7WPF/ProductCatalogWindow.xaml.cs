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
using FUMiniTikiSystem.BLL.Services;
using FUMiniTikiSystem.DAL.Entities;
using FUMiniTikiSystem.DAL.Repositories;
using FUMiniTikiSystem.DAL;

namespace GROUP7WPF
{
    /// <summary>
    /// Interaction logic for ProductCatalogWindow.xaml
    /// </summary>
    public partial class ProductCatalogWindow : Window
    {
        private readonly ProductService _productService;
        private List<Product> _allProducts = new();
        private readonly CategoryRepository _categoryRepo;
        private readonly bool _isAdmin;
        private readonly UnitOfWork _unitOfWork;
        private readonly int _customerId;

        private List<CartItem> cartItems = new List<CartItem>();
        public ProductCatalogWindow(bool isAdmin, int customerId)
        {
            InitializeComponent();
            _isAdmin = isAdmin;
            _customerId = customerId;
            // Ẩn hoặc hiển thị nút admin
            btnAddCategory.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnEditCategory.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnDeleteCategory.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnCategoryManager.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            OrderBorder.Visibility = !_isAdmin ? Visibility.Visible : Visibility.Collapsed;
            if (isAdmin)
            {
                ListProductBorder.Margin = new Thickness(
                    ListProductBorder.Margin.Left,
                    ListProductBorder.Margin.Top,
                    236,
                    ListProductBorder.Margin.Bottom);
            }
            var dbContext1 = new FuminiTikiSystemContext(); // cho sản phẩm
            var dbContext2 = new FuminiTikiSystemContext(); // cho danh mục
            _productService = new ProductService(new ProductRepository(dbContext1), new UnitOfWork(dbContext1));
            _categoryRepo = new CategoryRepository(dbContext2);
            _unitOfWork = new UnitOfWork(dbContext2);

            var repo = new ProductRepository(dbContext1);
            var unitOfWork = new UnitOfWork(dbContext1);

             LoadAllProducts();
            LoadCategories();
        }
        private async void LoadCategories()
        {
            var categories = await _categoryRepo.GetAllAsync();

            // Thêm mục "All" vào đầu danh sách
            var allOption = new Category { CategoryId = 0, Name = "All" };
            var categoryList = new List<Category> { allOption };
            categoryList.AddRange(categories);

            cbCategory.ItemsSource = categoryList;
            cbCategory.SelectedIndex = 0;
        }

        private async Task LoadAllProducts()
        {
            lvProducts.ItemsSource = null;
            _allProducts = await _productService.GetAllAsync(); // ✅ gán vào _allProducts

            lvProducts.ItemsSource = _allProducts;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
        

        private void ApplyFilter()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string selectedCategoryName = (cbCategory.SelectedItem as Category)?.Name ?? "All";

            // Bước 1: Lọc theo keyword trước
            var filtered = _allProducts
                .Where(p =>
                    string.IsNullOrWhiteSpace(keyword) ||
                    p.Name.ToLower().Contains(keyword) ||
                    (p.Description?.ToLower().Contains(keyword) ?? false)
                )
                .ToList();

            // Bước 2: Nếu không phải "All", tiếp tục lọc theo danh mục
            if (selectedCategoryName != "All")
            {
                filtered = filtered
                    .Where(p => p.Category.Name == selectedCategoryName)
                    .ToList();
            }

            lvProducts.ItemsSource = filtered;
        }
        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddProductWindow();
            if (window.ShowDialog() == true)
            {
               await  LoadAllProducts(); // refresh lại danh sách
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Please select a product to edit.");
                return;
            }

            var window = new AddProductWindow(selected);
            if (window.ShowDialog() == true)
            {
                 LoadAllProducts(); // refresh
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Please select a product to delete.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete '{selected.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                await _productService.DeleteAsync(selected); // Gọi xóa từ service
                LoadAllProducts(); // Làm mới danh sách sau khi xóa
                MessageBox.Show("Product deleted successfully.");
            }
        }

        private void CategoryManager_Click(object sender, RoutedEventArgs e)
        {
            var categoryWindow = new CategoryManagementWindow();
            categoryWindow.ShowDialog();

            // Reload category combobox in case categories were updated
            LoadCategories();
        }

        private void ViewOrderDetail_Click(object sender, RoutedEventArgs e)
        {
            var window = new OrderHistoryWindow(_customerId);
            window.ShowDialog();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Application.Current.MainWindow = loginWindow; // Gán lại MainWindow để giữ app sống
                this.Close();
            }
        }

        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txtCashReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtCashReceived.Text, out decimal cash))
            {
                decimal total = cartItems.Sum(x => x.TotalPrice);
                decimal change = cash - total;
                txtChange.Text = change >= 0 ? $" {change:N0}" : "Tiền đưa chưa đủ";
            }
            else
            {
                txtChange.Text = "0";
            }
        }
        private void lvProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var product = lvProducts.SelectedItem as Product;
            if (product == null) return;

            var item = cartItems.FirstOrDefault(x => x.Product.ProductId == product.ProductId);
            if (item != null)
                item.Quantity++;
            else
                cartItems.Add(new CartItem { Product = product, Quantity = 1 });

            RefreshCart();
        }
        private void RefreshCart()
        {
            lvSelectedProducts.ItemsSource = null;
            lvSelectedProducts.ItemsSource = cartItems;

            decimal total = cartItems.Sum(x => x.TotalPrice);
            txtTotalAmount.Text = $"Tổng tiền: {total:N0}";
        }

        private void lvSelectedProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
