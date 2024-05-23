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
using WPFAssignment1Group3.Models;

namespace WPFAssignment1Group3
{
    /// <summary>
    /// Interaction logic for OrderForm.xaml
    /// </summary>
    public partial class OrderForm : Window
    {

        public bool IsSaved { get; private set; }
        private Order _order;
        private MyStoreContext _context;
        public OrderForm(Order order)
        {
            InitializeComponent();
            _order = order;
            _context = new MyStoreContext();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string txtOrderId = tbOrderId.Text;
            DateTime orderDate = dpOrderDate.SelectedDate ?? DateTime.Now;
            int staffId = Int32.Parse(txtStaffId.Text);

            if (string.IsNullOrEmpty(txtOrderId))
            {
                Guid guid = Guid.NewGuid();
                byte[] byteArray = guid.ToByteArray();
                int orderId = BitConverter.ToInt32(byteArray, 0);

                var newOrder = new Order
                {
                    OrderId = orderId,
                    OrderDate = orderDate,
                    StaffId = staffId
                };

                try
                {
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    IsSaved = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while saving order: {ex.Message}");
                }

            }
            else
            {
                int orderId = Int32.Parse(txtOrderId);
                var exOrder = await _context.Orders.FindAsync(orderId);

                if (exOrder != null)
                {
                    exOrder.OrderDate = orderDate;
                    exOrder.StaffId = staffId;

                    try
                    {
                        await _context.SaveChangesAsync();
                        IsSaved = true;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occured while saving order: {ex.Message}");
                    }
                }
            }

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
