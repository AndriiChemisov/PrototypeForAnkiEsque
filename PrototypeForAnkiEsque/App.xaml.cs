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
                options.UseSqlite(connectionString));

            services.AddSingleton<BooleanToVisibilityConverter>();
            services.AddSingleton<DIValueConverterProvider>();
            services.AddSingleton<EaseRatingToStringConverter>();
            services.AddSingleton<DifficultyConverter>();
            services.AddTransient<BooleanToDictionaryValueMultiConverter>();

            // Register services and views
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IFlashcardService, FlashcardService>();
            services.AddTransient<IDeckService, DeckService>();
            services.AddTransient<MainWindow>();
            services.AddTransient<MainMenuUserControl>();
            services.AddTransient<FlashcardEntryUserControl>();
            services.AddTransient<FlashcardViewUserControl>();
            services.AddTransient<FlashcardDatabaseUserControl>();
            services.AddTransient<FlashcardEditorUserControl>();
            services.AddTransient<FlashcardDeckCreatorUserControl>();
            services.AddTransient<FlashcardDeckSelectionUserControl>();
            services.AddTransient<FlashcardDeckEditorUserControl>();

            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<FlashcardEntryViewModel>();
            services.AddTransient<FlashcardViewModel>();
            services.AddTransient<FlashcardDatabaseViewModel>();
            services.AddTransient<FlashcardEditorViewModel>();
            services.AddTransient<FlashcardDeckCreatorViewModel>();
            services.AddTransient<FlashcardDeckSelectionViewModel>();
            services.AddTransient<FlashcardDeckEditorViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                // Ensure that the database is created and seeded
                var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.EnsureCreated();
                ApplicationDbContext.Seed(dbContext);

                // Create MainWindow and inject NavigationService
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

                // Set MainWindow and show it
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
