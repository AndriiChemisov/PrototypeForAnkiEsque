using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Views;
using PrototypeForAnkiEsque.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using PrototypeForAnkiEsque.Converters;
using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<FlashcardService>();
            services.AddSingleton<BooleanToVisibilityConverter>();
            services.AddSingleton<DIValueConverterProvider>();

            // Register services and views
            services.AddSingleton<Services.NavigationService>();
            services.AddTransient<MainMenuUserControl>();
            services.AddTransient<FlashcardEntryUserControl>();
            services.AddTransient<FlashcardViewUserControl>();

            // Register ViewModels
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<FlashcardEntryViewModel>();
            services.AddTransient<FlashcardViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ensure that the database is created and seeded
            var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
            ApplicationDbContext.Seed(dbContext);

            // Create MainWindow and inject NavigationService
            var navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            var mainWindow = new MainWindow(navigationService);

            // Set MainWindow and show it
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
