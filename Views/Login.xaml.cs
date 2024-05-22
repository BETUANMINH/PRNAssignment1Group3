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
using WPFAssignment1Group3.Services;
using WPFAssignment1Group3.State;

namespace WPFAssignment1Group3.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IAuthenticator _authenticator;
        private readonly IDBRepository _repository;
        private readonly IStaffServices _staffServices;

        public Login(IAuthenticator authenticator, IDBRepository dBRepository, IStaffServices staffServices)
        {
            InitializeComponent();
            _authenticator = authenticator;
            _repository = dBRepository;
            _staffServices = staffServices;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var result = await _authenticator.Login(txtUsername.Text, txtPassword.Text);
            if(result)
            {
                txtArlet.Text = "Success";
                MainWindow mainWindow = new MainWindow(_repository, _authenticator, _staffServices);
                mainWindow.Show();
                this.Close();
            }else
            {
                txtArlet.Text = "Fail";
            }
            btnLogin.IsEnabled = true;

            txtArlet.Visibility = Visibility.Visible;
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }
    }
}
