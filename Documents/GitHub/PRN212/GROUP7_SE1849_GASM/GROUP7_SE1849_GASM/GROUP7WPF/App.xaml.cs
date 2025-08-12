using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;
using FUMiniTikiSystem.DAL;
using FUMiniTikiSystem.DAL.Interfaces;
using FUMiniTikiSystem.DAL.Repositories;
using FUMiniTikiSystem.BLL.Interfaces;
using FUMiniTikiSystem.BLL.Services;

namespace GROUP7WPF
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = context.Configuration;

                    // Đăng ký DbContext
                    services.AddDbContext<FuminiTikiSystemContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                    // Đăng ký các repository
                    services.AddScoped<ICustomerRepository, CustomerRepository>();
                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<ICategoryRepository, CategoryRepository>();
                    services.AddScoped<IOrderRepository, OrderRepository>();

                    // Đăng ký các và service
                    services.AddScoped<ICustomerService, CustomerService>();
                    services.AddScoped<IProductService, ProductService>();
                    services.AddScoped<ICategoryService, CategoryService>();
                    services.AddScoped<IOrderService, OrderService>();

                    // Đăng ký unit of work 
                    services.AddScoped<IUnitOfWork, UnitOfWork>();

                    // Đăng ký UI
                    services.AddSingleton<RegisterWindow>();
                    services.AddSingleton<LoginWindow>();
                    services.AddSingleton<ProductCatalogWindow>();
                    services.AddSingleton<AddProductWindow>();
                    services.AddSingleton<AddProductWindow>();
                    services.AddSingleton<OrderHistoryWindow>();
                    services.AddSingleton<CategoryManagementWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            var mainWindow = AppHost.Services.GetRequiredService<LoginWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using(AppHost)
            {
                await AppHost.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }
}
