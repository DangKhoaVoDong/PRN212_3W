using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FUMiniTikiSystem.BLL.Services;
using FUMiniTikiSystem.DAL.Repositories;
using FUMiniTikiSystem.DAL;
using System.Net.Mail;

namespace GROUP7WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly CustomerService _service;

        public RegisterWindow()
        {
            InitializeComponent();

            var dbContext = new FuminiTikiSystemContext();
            var repo = new CustomerRepository(dbContext);
            var uniWork = new UnitOfWork(dbContext);
            _service = new CustomerService(repo,uniWork);
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            // Kiểm tra trống
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            // ✅ Kiểm tra định dạng email
            try
            {
                var mail = new MailAddress(email);
            }
            catch
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            // Kiểm tra mật khẩu và xác nhận khớp nhau
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // Gọi service để đăng ký
            var success = await _service.RegisterAsync(name, email, password);
            if (success)
            {
                MessageBox.Show("Register successful!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Email is already taken.");
            }
        }
    }
}