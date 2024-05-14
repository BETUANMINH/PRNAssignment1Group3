
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WPFAssignment1Group3.Common;
using WPFAssignment1Group3.Models;


namespace WPFAssignment1Group3
{

    public partial class App : Application
    {
        private readonly IHost _host;
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

            services.AddSingleton<MainWindow>();
            services.AddSingleton<Report>();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            String ConnectionStr = config.GetConnectionString("conn");

            services.AddDbContext<MyStoreContext>(options => options.UseSqlServer(ConnectionStr));
            services.AddSingleton<IDBRepository,DBRepository>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
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
