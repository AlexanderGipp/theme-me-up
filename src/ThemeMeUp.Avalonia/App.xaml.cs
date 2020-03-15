using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ThemeMeUp.Avalonia.ViewModels;
using ThemeMeUp.Avalonia.Views;

namespace ThemeMeUp.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = ActivatorUtilities.CreateInstance(InversionOfControl.Provider, typeof(MainWindowViewModel))
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}