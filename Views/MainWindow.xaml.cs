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
using WPFAssignment1Group3.Common;
using WPFAssignment1Group3.Models;
using WPFAssignment1Group3.Services;
using WPFAssignment1Group3.State;
using WPFAssignment1Group3.Views;

namespace WPFAssignment1Group3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDBRepository _repository;
        private readonly IAuthenticator _authenticator;

        public MainWindow(IDBRepository repository, IAuthenticator authenticator)
        {
            InitializeComponent();
            _repository = repository;
            _authenticator = authenticator;

            gridDetails.Visibility = Visibility.Hidden;
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (App.AccountStore == null)
            {
                Login login = new Login(_authenticator, _repository);
                login.ShowDialog();
            }
            else
            {
                tbUsername.Text = App.AccountStore.Username;
                tbRole.Text = (App.AccountStore.role == 0) ? StaffRole.Admin.ToString() : StaffRole.Staff.ToString();
                gridDetails.Visibility = Visibility.Visible;
            }
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            App.AccountStore = null;
            gridDetails.Visibility = Visibility.Hidden;
            base.OnClosed(e);
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            Report report = new Report(_repository);
            report.ShowDialog();

        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (App.AccountStore.role == (int)StaffRole.Admin)
            {
                ProductWindow productWindow = new ProductWindow(_repository);

                productWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("You do not have permission to access this feature");
                return;
            }
        }
    }
}