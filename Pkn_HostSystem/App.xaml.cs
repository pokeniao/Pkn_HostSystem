using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;
namespace Pkn_HostSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 程序加载的时候
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {


            // IOC 容器
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddNavigationViewPageProvider()
                    .AddSingleton<INavigationService, NavigationService>()
                    .AddSingleton<HomePageViewModel>()
                    .AddSingleton<LoadMesPageViewModel>()
                    .AddSingleton<MesTcpViewModel>()
                    .AddSingleton<ModbusToolViewModel>()
                    .AddSingleton<SettingsPageViewModel>()
                    .AddSingleton<ProductiveViewModel>()
                    .AddSingleton<MainWindow>()

                    //页面单例 ,预加载
                    .AddSingleton<MesTcpPage>()
                    .AddSingleton<LoadMesPage>()
                    .AddSingleton<HomePage>()
                    .AddSingleton<ProductivePage>()
                    .AddSingleton<SettingsPage>()
                    .AddSingleton<ModbusToolPage>()
                    .BuildServiceProvider()
                );
            base.OnStartup(e);
        }


        /// <summary>
        /// 程序结束的时候
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // SettingsPageViewModel viewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
            // viewModel.Save();

            base.OnExit(e);
        }
    }

}
