using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.ViewModels.Page;
using Wpf.Ui.Appearance;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
            if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
            {
                DarkThemeRadioButton.IsChecked = true;
            }
            else
            {
                LightThemeRadioButton.IsChecked = true;
            }
        }
        private void OnLightThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            ApplicationThemeManager.Apply(ApplicationTheme.Light);
        }

        private void OnDarkThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        }

        private void OnAutoThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            ApplicationThemeManager.ApplySystemTheme();
        }
    }
}