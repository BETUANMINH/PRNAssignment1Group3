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
using WPFAssignment1Group3.Services;

namespace WPFAssignment1Group3.Views
{
    /// <summary>
    /// Interaction logic for StaffWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        private readonly IStaffServices _staffServices;

        public StaffWindow(IStaffServices staffServices)
        {
            InitializeComponent();
            _staffServices = staffServices;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStaffs();
        }

        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            var entry = MessageBox.Show("Add Staff?", "Add Staff?", MessageBoxButton.YesNo);
            if (entry == MessageBoxResult.Yes)
            {
                if (string.IsNullOrEmpty(nameTxt.Text) || string.IsNullOrEmpty(passTxt.Text))
                {
                    MessageBox.Show("Can't null");
                    return;

                }

                Staff st = new Staff()
                {
                    Name = nameTxt.Text,
                    Password = passTxt.Text,
                    Role = 1
                };
                var result = await _staffServices.AddOrEditStaff(st);
                if (result)
                {
                    MessageBox.Show("added successfully!");
                    LoadStaffs();
                }
            }

        }

        private void btnUpdate_ClickAsync(object sender, RoutedEventArgs e)
        {
            var entry = MessageBox.Show("Update Staff?", "Update Staff?", MessageBoxButton.YesNo);
            if (entry == MessageBoxResult.Yes)
            {
                if (!int.TryParse(txtStaffID.Text, out int staffID))
                {
                    MessageBox.Show("Input a valid ID");
                    return;

                }
                EditStaff("update", staffID);
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var entry = MessageBox.Show("Delete Staff?", "Delete Staff?", MessageBoxButton.YesNo);
            if (entry == MessageBoxResult.Yes)
            {
                if (!int.TryParse(txtStaffID.Text, out int staffID))
                {
                    MessageBox.Show("Input a valid ID");
                    return;

                }
                EditStaff("delete", staffID);
            }

        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text) || txtSearch.Text.Contains(' '))
            {

                lvStaff.ItemsSource = await _staffServices.GetAlls();
            }
            lvStaff.ItemsSource = await _staffServices.GetStaffsByName(txtSearch.Text);

        }
        private async void LoadStaffs()
        {
            var result = await _staffServices.GetAlls();
            lvStaff.ItemsSource = result;
        }
        private async void EditStaff(string type, int Id)
        {
            var result = false;
            switch (type)
            {
                case @"delete":
                    result = await _staffServices.DeleteStaff(Id);
                    break;
                case @"update":
                    Staff st = new Staff()
                    {
                        StaffId = Id,
                        Name = nameTxt.Text,
                        Password = passTxt.Text,
                        Role = 1
                    };
                    result = await _staffServices.AddOrEditStaff(st);
                    break;
            }
            if (result)
            {
                MessageBox.Show($"{type} successfully!");
                LoadStaffs();
            }
            else
            {
                MessageBox.Show($"{type} fail, try again!");
                return;
            }
        }
    }
}
