using Microsoft.EntityFrameworkCore;
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
using WPFAssignment1Group3.Common;
using WPFAssignment1Group3.Models;

namespace WPFAssignment1Group3
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private MyStoreContext _context;
        public OrderWindow()
        {
            InitializeComponent();
            _context = new MyStoreContext();

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set default value for dpDate is today
            dpDate.SelectedDate = DateTime.Today;

            //Call Search function
            await SearchOrders();
        }

        private async Task SearchOrders()
        {
            try
            {
                //Get date from DatePicker
                var date = dpDate.SelectedDate.Value;

                //Load Orders which have OrderDate is chosen date from DB
                var orders = await _context.Orders.Where(o => o.OrderDate.Date == date).ToListAsync();

                //Display Orders in ListView
                lvOrders.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }

        private async void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Call search function when dpDate changes value
            await SearchOrders();
        }

        private async void mnuAdd_Click(object sender, RoutedEventArgs e)
        {
            var newOrder = new Order { OrderDate = DateTime.Today };

            var orderForm = new OrderForm(newOrder);
            orderForm.ShowDialog();

            if (orderForm.DialogResult == DialogResult)
            {
                await SearchOrders();
            }
        }

        private async void ctmEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = lvOrders.SelectedItems as Order;

            if (selectedOrder != null)
            {
                var orderForm = new OrderForm(selectedOrder);
                orderForm.ShowDialog();

                if (orderForm.IsSaved)
                {
                    await SearchOrders();
                }
            }
        }

        private async void ctmDelete_Click(object sender, RoutedEventArgs e)
        {
            int orderId = Int32.Parse(gvcOrderId.ToString());

            try
            {
                var exOrder = await _context.Orders.FindAsync(orderId);

                if (exOrder != null)
                {
                    _context.Orders.Remove(exOrder);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    MessageBox.Show("Invalid request!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the order: {ex.Message}");
            }
        }
    }
}
