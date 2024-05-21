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
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private IDBRepository _repository;
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
            var  currentTime = DateTime.Now;
            var defaultsearch = await _repository.Context.Set<Order>().Where(x => x.OrderDate < currentTime && x.OrderDate > currentTime.AddMonths(-1)).ToListAsync();
            listViewOrder.ItemsSource = defaultsearch;
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            IQueryable<Order> data = _repository.Context.Set<Order>();
            if (startDate.SelectedDate == null || endDate.SelectedDate == null)
            {
                listViewOrder.ItemsSource = null;
                return;
            }
            DateTime start = startDate.SelectedDate.Value;
            DateTime end = endDate.SelectedDate.Value;
            if (start > end)
            {
                MessageBox.Show("Start date must be less than end date");
                return;
            }
            else if (start == end)
            {
                var result = await data.Where(x => x.OrderDate.Date == start).ToListAsync();
                listViewOrder.ItemsSource = result;
            }
            else
            {
                var result = await data.Where(x => x.OrderDate.Date >= start && x.OrderDate.Date <= end).ToListAsync();
                listViewOrder.ItemsSource = result;
            }
        }
    }
}
