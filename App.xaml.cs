
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            //String ConnectionStr = config.GetConnectionString("conSQLServer");

            //services.AddDbContext<MyStoreContext>(options => options.UseSqlServer(ConnectionStr));

            //my config
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();
            services.AddDbContext<MyStoreContext>(options => options.UseSqlServer(config.GetConnectionString(@"conSQLServer")));

            //DI
            services.AddSingleton<MainWindow>();
            services.AddSingleton<Login>();

            services.AddSingleton<IDBRepository, DBRepository>();
            services.AddSingleton<IStaffServices, StaffServices>();

            services.AddSingleton<IAuthenticator, Authenticator>();


        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();

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
