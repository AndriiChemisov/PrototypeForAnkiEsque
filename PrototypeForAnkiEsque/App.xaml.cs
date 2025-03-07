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
// This file is the entry point for the application. It is used to configure the services and set up the dependency injection container.
// The App class inherits from the Application class provided by WPF. The ConfigureServices method is used to configure the services and set up the dependency injection container.
// The ConfigureServices method adds the required services to the service collection. The service collection is then used to build the service provider.
// The service provider is stored in the ServiceProvider property and is used to resolve services from the DI container.
// The ConfigureServices method adds the required services to the service collection. These services include the database context, converters, services, views, and view models.
// The ConfigureServices method also sets up the database connection using the connection string from the appsettings.json file.
// The OnStartup method is overridden to create the main window, ensure that the database is created and seeded, and show the main window.
// Simple explanation: This class is the entry point for the application. It is used to configure the services and set up the dependency injection container.
// Side note: ensure that whenever anything changes in the project, this file is updated to reflect those changes.
namespace PrototypeForAnkiEsque
{
    public partial class App : Application
    {
        #region PROPERTIES
        public static IServiceProvider ServiceProvider { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
        #endregion

        #region METHODS
        private static void ConfigureServices(IServiceCollection services)
        {
            #region DATABASE SETUP
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            #endregion

            #region CONVERTERS
            services.AddSingleton<BooleanToVisibilityConverter>();
            services.AddSingleton<DIValueConverterProvider>();
            services.AddSingleton<EaseRatingToStringConverter>();
            services.AddSingleton<DifficultyConverter>();
            services.AddTransient<BooleanToDictionaryValueMultiConverter>();
            #endregion

            #region SERVICES
            services.AddSingleton<IMainMenuNavigationService, NavigationService>();
            services.AddSingleton<IFlashcardNavigationService, NavigationService>();
            services.AddSingleton<IDeckNavigationService, NavigationService>();
            services.AddSingleton<ILastNavigatedViewService, NavigationService>();
            services.AddTransient<IFlashcardService, FlashcardService>();
            services.AddTransient<IDeckService, DeckService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<ISettingsManager, SettingsManager>();
            #endregion

            #region VIEWS
            services.AddTransient<MainWindow>();
            services.AddTransient<MainMenuUserControl>();
            services.AddTransient<FlashcardEntryUserControl>();
            services.AddTransient<FlashcardViewUserControl>();
            services.AddTransient<FlashcardDatabaseUserControl>();
            services.AddTransient<FlashcardEditorUserControl>();
            services.AddTransient<FlashcardDeckCreatorUserControl>();
            services.AddTransient<FlashcardDeckSelectionUserControl>();
            services.AddTransient<FlashcardDeckEditorUserControl>();
            #endregion

            #region VIEWMODELS
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<FlashcardEntryViewModel>();
            services.AddTransient<FlashcardViewModel>();
            services.AddTransient<FlashcardDatabaseViewModel>();
            services.AddTransient<FlashcardEditorViewModel>();
            services.AddTransient<FlashcardDeckCreatorViewModel>();
            services.AddTransient<FlashcardDeckSelectionViewModel>();
            services.AddTransient<FlashcardDeckEditorViewModel>();
            #endregion
        }

        #region OVERRIDES
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                // Ensure that the database is created and seeded
                var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.EnsureCreated();
                ApplicationDbContext.Seed(dbContext);// Note that the database is only seeded if it is empty

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
        #endregion

        #endregion
    }
}
