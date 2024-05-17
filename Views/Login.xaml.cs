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
using WPFAssignment1Group3.State;

namespace WPFAssignment1Group3.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IAuthenticator _authenticator;

        public Login(IAuthenticator authenticator)
        {
            InitializeComponent();
            _authenticator = authenticator;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var result = await _authenticator.Login(txtUsername.Text, txtPassword.Text);
            if(result)
            {
                txtArlet.Text = "Success";
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
