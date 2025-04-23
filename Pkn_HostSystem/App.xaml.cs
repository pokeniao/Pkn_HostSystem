using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages;

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
                    .AddSingleton<HomePageViewModel>()
                    .AddSingleton<LoadMesPageViewModel>()
                    .AddSingleton<MesTcpViewModel>()
                    .AddSingleton<ModbusToolViewModel>()
                    .AddSingleton<SettingsPageViewModel>()
                    .AddSingleton<ProductiveViewModel>()

                    //页面单例 ,预加载
                    .AddSingleton<MesTcpPage>()
                    .AddSingleton<LoadMesPage>()
                    .AddSingleton<HomePage>()
                    .AddSingleton<ProductivePage>()

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
            SettingsPageViewModel viewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
            viewModel.Save();

            base.OnExit(e);
        }
    }

}
