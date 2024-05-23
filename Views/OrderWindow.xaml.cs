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
using WPFAssignment1Group3.State;

namespace WPFAssignment1Group3
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private readonly IDBRepository _repository;
        public OrderWindow(IDBRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            LoadData();
            dpDate.SelectedDate = DateTime.Now;

            lvOrders.SelectionChanged += lvOrders_SelectionChanged;
        }

        private async void LoadData()
        {
            var orders = await _repository.Context.Set<Order>().Where(o => o.StaffId == App.AccountStore.Id).ToListAsync();
            lvOrders.ItemsSource = orders;
        }
        private bool IsValidSelectedDate()
        {
            // Check if SelectedDate has a value and if it is a valid date
            return dpDate.SelectedDate.HasValue;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                try
                {
                    var exOrder = await _repository.Context.Set<Order>().FindAsync(orderId);

                    if (exOrder != null)
                    {
                        _repository.Context.Set<Order>().Remove(exOrder);
                        await _repository.SaveChangesAsync();

                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Selected order could not be found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the order: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.");
            }
        }

        private async void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = new Order
            {
                OrderDate = DateTime.Now,
                StaffId = App.AccountStore.Id,
            };

            await _repository.AddAsync(order);
            await _repository.SaveChangesAsync();
            LoadData();
        }

        private void LoadDetails(Order order)
        {
            var orderDetails = _repository.Context.Set<OrderDetail>().Where(o => o.OrderId == order.OrderId).ToList();
            lvOrderDetails.ItemsSource = orderDetails;
        }

        private void lvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedOrder = lvOrders.SelectedItem as Order;
            if (selectedOrder != null)
            {
                tbOrderDId.Text = selectedOrder.OrderId.ToString();
            }
            LoadDetails(selectedOrder);
        }

        private async void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderDetailId)
            {
                try
                {
                    var exOrderDetail = await _repository.Context.Set<OrderDetail>().FindAsync(orderDetailId);
                    var orderId = exOrderDetail.OrderId;

                    if (exOrderDetail != null)
                    {
                        _repository.Context.Set<OrderDetail>().Remove(exOrderDetail);
                        await _repository.SaveChangesAsync();

                        LoadDetails(await _repository.Context.Set<Order>().Where(o => o.OrderId == orderId && o.StaffId == App.AccountStore.Id).FirstOrDefaultAsync());
                    }
                    else
                    {
                        MessageBox.Show("Selected detail could not be found.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the detail: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a detail to delete.");
                return;
            }
        }

        private async void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            var orderDetailId = tbOrderDetailId.Text;
            var orderId = tbOrderDId.Text;
            var productId = tbProductId.Text;
            var quantity = tbQuantity.Text;
            var unitPrice = tbUnitPrice.Text;

            if (!int.TryParse(orderId, out int orderIdParsed) || !int.TryParse(productId, out int productIdParsed) || !int.TryParse(quantity, out int quantityParsed) || !int.TryParse(unitPrice, out int unitPriceParsed))
            {
                MessageBox.Show("Please enter valid number for these fields: Order ID, Product ID, Quantity, Unit Price.");
                return;
            }
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(unitPrice))
            {
                MessageBox.Show("Please fill all the fields and let Order Detail ID empty.");
                return;
            }
            OrderDetail detail = new OrderDetail
            {
                OrderId = orderIdParsed,
                ProductId = productIdParsed,
                Quantity = quantityParsed,
                UnitPrice = unitPriceParsed
            };
            await _repository.Context.Set<OrderDetail>().AddAsync(detail);
            await _repository.SaveChangesAsync();
            LoadDetails(await _repository.Context.Set<Order>().Where(o => o.OrderId == orderIdParsed && o.StaffId == App.AccountStore.Id).FirstOrDefaultAsync());

            tbOrderDetailId.Text = "";
            tbOrderDId.Text = "";
            tbProductId.Text = "";
            tbQuantity.Text = "";
            tbUnitPrice.Text = "";
        }

        private void lvOrderDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDetail = lvOrderDetails.SelectedItem as OrderDetail;
            if (selectedDetail != null)
            {
                tbOrderDetailId.Text = selectedDetail.OrderDetailId.ToString();
                tbProductId.Text = selectedDetail.ProductId.ToString();
                tbQuantity.Text = selectedDetail.Quantity.ToString();
                tbUnitPrice.Text = selectedDetail.UnitPrice.ToString();
            }
        }

        private void tbProductId_TextChanged(object sender, TextChangedEventArgs e)
        {
            var productId = tbProductId.Text;
            if (int.TryParse(productId, out int productIdParsed))
            {
                var product = _repository.Context.Set<Product>().Where(o => o.ProductId == productIdParsed).FirstOrDefault();

                if (product != null)
                {
                    tbUnitPrice.Text = product.UnitPrice.ToString();
                }
                else
                {
                    tbUnitPrice.Text = "-";
                }
            }
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var date = dpDate.SelectedDate;
            try
            {
                //Get date from DatePicker
                if (IsValidSelectedDate())
                {
                    date = dpDate.SelectedDate.Value;
                }
                else
                {
                    MessageBox.Show("Your date input is invalid.");
                    return;
                }

                //Load Orders which have OrderDate is chosen date from DB
                IQueryable<Order> orders = _repository.Context.Set<Order>().Where(o => o.OrderDate.Date == date && o.StaffId == App.AccountStore.Id);

                //Display Orders in ListView
                lvOrders.ItemsSource = await orders.ToListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }

        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            dpDate.SelectedDate = DateTime.Now;
            LoadData();
        }

        private async void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {
            var orderDetailId = tbOrderDetailId.Text;
            var orderId = tbOrderDId.Text;
            var productId = tbProductId.Text;
            var quantity = tbQuantity.Text;
            var unitPrice = tbUnitPrice.Text;
            if (!int.TryParse(orderDetailId, out int orderDetailIdParsed) || !int.TryParse(orderId, out int orderIdParsed) || !int.TryParse(productId, out int productIdParsed) || !int.TryParse(quantity, out int quantityParsed) || !int.TryParse(unitPrice, out int unitPriceParsed))
            {
                MessageBox.Show("Please enter valid number for these fields: Product ID, Quantity, Unit Price.");
                return;
            }
            var exDetail = await _repository.Context.Set<OrderDetail>().Where(o => o.OrderDetailId == orderDetailIdParsed).FirstOrDefaultAsync();
            exDetail.OrderId = orderIdParsed;
            exDetail.ProductId = productIdParsed;
            exDetail.Quantity = quantityParsed;
            exDetail.UnitPrice = unitPriceParsed;
            await _repository.SaveChangesAsync();
            LoadDetails(await _repository.Context.Set<Order>().Where(o => o.OrderId == orderIdParsed && o.StaffId == App.AccountStore.Id).FirstOrDefaultAsync());
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}