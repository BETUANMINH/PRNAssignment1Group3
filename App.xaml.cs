
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows;
using WPFAssignment1Group3.Common;
using WPFAssignment1Group3.Models;
using WPFAssignment1Group3.Services;
using WPFAssignment1Group3.State;
using WPFAssignment1Group3.Views;


namespace WPFAssignment1Group3
{

    public partial class App : Application
    {
        private readonly IHost _host;

        public static AccountStore AccountStore;

        public static DefaultAccount defAcc;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            String ConnectionStr = config.GetConnectionString("conn");

            Console.WriteLine("Connection String:" + ConnectionStr);
            if (string.IsNullOrEmpty(ConnectionStr))
            {
                throw new Exception("Connection string is null or empty.");
            }

            services.AddDbContext<MyStoreContext>(options => options.UseSqlServer(ConnectionStr));
            try
            {
                defAcc = config.GetSection("DefaultAdminCredentials").Get<DefaultAccount>();
            }catch (Exception ex)
            {
                throw new Exception();
            }
            //DI
            services.AddSingleton<MainWindow>();
            services.AddSingleton<Report>();
            services.AddSingleton<Login>();
            services.AddSingleton<ProductWindow>();
            services.AddSingleton<OrderWindow>();

            services.AddSingleton<IDBRepository, DBRepository>();
            services.AddSingleton<IStaffServices, StaffServices>();

            services.AddSingleton<IAuthenticator, Authenticator>();


            services.AddDbContext<MyStoreContext>(options => options.UseSqlServer(ConnectionStr));

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            Login mainWindow = _host.Services.GetRequiredService<Login>();
            mainWindow.Show();

            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
            base.OnExit(e);
        }

    }

}