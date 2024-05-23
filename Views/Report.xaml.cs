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
using WPFAssignment1Group3.Views;

namespace WPFAssignment1Group3
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private readonly IDBRepository _repository;
        public Report(IDBRepository dB)
        {
            _repository = dB;
            InitializeComponent();
            LoadData();
            startDate.SelectedDateChanged += DatePicker_SelectedDateChanged;
            endDate.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }
        private async void LoadData()
        {
            var currentTime = DateTime.Now;
            if (App.AccountStore.role == (int)StaffRole.Admin)
            {
                var defaultsearch = await _repository.Context.Set<Order>().Where(x => x.OrderDate < currentTime && x.OrderDate > currentTime.AddMonths(-1)).ToListAsync();
                listViewOrder.ItemsSource = defaultsearch;
            }
            else
            {
                var defaultsearch = await _repository.Context.Set<Order>().Where(x => x.OrderDate < currentTime && x.OrderDate > currentTime.AddMonths(-1) && x.StaffId == App.AccountStore.Id).ToListAsync();
                listViewOrder.ItemsSource = defaultsearch;
            }
            startDate.SelectedDate = currentTime.AddMonths(-1);
            endDate.SelectedDate = currentTime;
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private async void listViewOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //show dialog display order detail
            if (listViewOrder.SelectedItem != null)
            {

                Order order = listViewOrder.SelectedItem as Order;
                int orderId = order.OrderId;
                var data = await _repository.Context.Set<OrderDetail>().Where(x => x.OrderId == orderId).ToListAsync();
                listDetail.ItemsSource = data;
                UpdateOrderHeader();
                UpdateOrderDetailHeader();
            }
        }

        private async void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (startDate.SelectedDate == null || endDate.SelectedDate == null)
            {
                listViewOrder.ItemsSource = null;
                UpdateOrderHeader();
                UpdateOrderDetailHeader();
                return;
            }
            IQueryable<Order> data = _repository.Context.Set<Order>();
            if (App.AccountStore.role != (int)StaffRole.Admin)
            {
                data = data.Where(x => x.StaffId == App.AccountStore.Id);
            }
            DateTime start = startDate.SelectedDate.Value;
            DateTime end = endDate.SelectedDate.Value;
            if (start > end)
            {
                MessageBox.Show("Start date must be less than end date");
                listViewOrder.ItemsSource = null;
                UpdateOrderHeader();
                UpdateOrderDetailHeader();
                return;
            }
            else if (start == end)
            {
                var result = await data.Where(x => x.OrderDate.Date == start).ToListAsync();
                listViewOrder.ItemsSource = result;
                if(result.Count == 0)
                {
                    //reset order detail
                    listDetail.ItemsSource = null;
                }
            }
            else
            {
                var result = await data.Where(x => x.OrderDate.Date >= start && x.OrderDate.Date <= end).ToListAsync();
                listViewOrder.ItemsSource = result;
                if (result.Count == 0)
                {
                    //reset order detail
                    listDetail.ItemsSource = null;
                }
            }
            UpdateOrderHeader();
            UpdateOrderDetailHeader();
        }
        private void UpdateOrderHeader()
        {
            groupBoxOrders.Header = $"Orders ({listViewOrder.Items.Count})";
        }

        private void UpdateOrderDetailHeader()
        {
            groupBoxOrderDetails.Header = $"Order Details ({listDetail.Items.Count})";
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
