using System.Windows;
using FUMiniTikiSystem.BLL.Services;
using FUMiniTikiSystem.DAL.Repositories;
using FUMiniTikiSystem.DAL;
using Microsoft.Extensions.Configuration;

namespace GROUP7WPF
{
    public partial class LoginWindow : Window
    {
        private readonly CustomerService _customerService;
        private string _adminEmail;
        private string _adminPassword;
        public LoginWindow()
        {
            InitializeComponent();

            var dbContext = new FuminiTikiSystemContext();
            var repo = new CustomerRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            _customerService = new CustomerService(repo, unitOfWork);
            var config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();

            _adminEmail = config["AdminAccount:Email"];
            _adminPassword = config["AdminAccount:Password"];
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            var customer = await _customerService.LoginAsync(email, password);
            bool isAdmin =
string.Equals(email, _adminEmail.Trim(), StringComparison.OrdinalIgnoreCase) &&
string.Equals(password, _adminPassword.Trim(), StringComparison.Ordinal);
            if (customer != null || isAdmin)
            {
                // So sánh với cấu hình trong appsettings.json
                int customerId = int.MaxValue;
                if (!isAdmin)
                {
                    customerId = customer.CustomerId;
                }
                var catalogWindow = new ProductCatalogWindow(isAdmin, customerId);
                catalogWindow.Closed += (s, args) => this.Close();
                catalogWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show($"Login failed");
            }
        }


        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
