using FUMiniTikiSystem.DAL.Entities;
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

namespace GROUP7WPF
{
    /// <summary>
    /// Interaction logic for InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        public List<CartItem> CartItems { get; set; }
        public decimal CashReceived { get; set; }

        public InvoiceWindow(List<CartItem> cartItems, decimal cashReceived)
        {
            InitializeComponent();
            CartItems = cartItems;
            CashReceived = cashReceived;

            lvInvoiceItems.ItemsSource = CartItems;

            decimal total = cartItems.Sum(x => x.TotalPrice);
            decimal change = cashReceived - total;

            DataContext = new
            {
                TotalAmount = $"Tổng tiền: {total:N0} đ",
                CashReceived = $"Khách đưa: {cashReceived:N0} đ",
                Change = change >= 0 ? $"Tiền thối lại: {change:N0} đ" : "Tiền đưa chưa đủ"
            };
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng in / xuất PDF có thể thêm sau.");
        }
    }
}
