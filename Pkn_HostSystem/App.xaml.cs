using CommunityToolkit.Mvvm.DependencyInjection;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages;
using System.IO;
using System.Runtime.InteropServices;
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
            AppRunOn();
            LogConfig();
            CreateIoc();
            base.OnStartup(e);
        }

        #region 唯一运行程序
        /// <summary>
        /// 运行唯一App
        /// </summary>
        private static Mutex _mutex;
        private static readonly string MutexName = GlobalMannager.AssemblyName; // 应用唯一标识
        private static readonly string MainWindowTitle = GlobalMannager.AssemblyName; // 用你的窗口标题替换

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;
        private void AppRunOn()
        {
            bool createdNew;
            _mutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                // 找到已有窗口句柄并激活,是根据窗口标题找句柄。
                IntPtr hWnd = FindWindow(null, MainWindowTitle);
                if (hWnd != IntPtr.Zero)
                {
                    // 还原窗口
                    ShowWindow(hWnd, SW_RESTORE);
                    // 激活窗口
                    SetForegroundWindow(hWnd);
                }
                else
                {
                    MessageBox.Show("程序已在运行中。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Shutdown();
                return;
            }

        }


        #endregion

        /// <summary>
        /// 日志配置
        /// </summary>
        private void LogConfig()
        {
            // 设置 log4net 全局变量
            log4net.GlobalContext.Properties["LOG_DIR"] = Path.Combine(GlobalMannager.AppFolder, "Logs");
            // 开启 log4net 内部调试信息输出到控制台
            log4net.Util.LogLog.InternalDebugging = true;
            XmlConfigurator.ConfigureAndWatch(new FileInfo("Config\\log4net.config"));
        }
        /// <summary>
        /// IOC配置
        /// </summary>
        private void CreateIoc()
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
                    .AddSingleton<VisionPageViewModel>()
                    .AddSingleton<StationViewModel>()
                    .AddSingleton<TcpToolViewModel>()

                  //页面单例 ,预加载
                    .AddSingleton<StationPage>()
                    .AddSingleton<VisionPage>()
                    .AddSingleton<MesTcpPage>()
                    .AddSingleton<LoadMesPage>()
                    .AddSingleton<HomePage>()
                    .AddSingleton<ProductivePage>()
                    .AddSingleton<SettingsPage>()
                    .AddSingleton<ModbusToolPage>()
                    .AddSingleton<TcpToolPage>()
                    .BuildServiceProvider()
            );
        }

        /// <summary>
        /// 程序结束的时候
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // SettingsPageViewModel viewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
            // viewModel.Save();
            _mutex.Dispose();
            base.OnExit(e);
        }
    }

}
