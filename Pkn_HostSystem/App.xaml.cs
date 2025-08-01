using AspectCore.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using OpenTK.Graphics.ES20;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.ViewModels.Windows;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;
using MessageBox = System.Windows.MessageBox;

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
        protected override async void OnStartup(StartupEventArgs e)
        {
          
                AppRunOn();
                OnStartupWindow onStartupWindow = new OnStartupWindow();
                onStartupWindow.Show();
              

               await Task.Run(async () =>
                {
                    LogConfig();
                    log4net.LogManager.GetLogger(typeof(App)).Info("Pkn_HostSystem程序启动");
                    CreateIoc();
                    LoadDll();
                });
                
                // ✅ 创建主窗口，显示并设置为主窗口
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                base.OnStartup(e); // 只有当 App.Xaml 中设置了 StartupUri="MainWindow.xaml" 才需要
                onStartupWindow.Close();
              
        }



        #region 唯一运行程序

        /// <summary>
        /// 运行唯一App
        /// </summary>
        private static Mutex _mutex;

        private static readonly string MutexName = GlobalManager.AssemblyName; // 应用唯一标识
        private static readonly string MainWindowTitle = GlobalManager.AssemblyName; // 用你的窗口标题替换

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
        private  void LogConfig()
        {
            string path = Path.Combine(GlobalManager.AppFolder, "Logs");
            // 设置 log4net 全局变量
            log4net.GlobalContext.Properties["LOG_DIR"] = path;

            string path2 = Path.Combine(path, "log4net_debug");
            // 确保目录存在
            Directory.CreateDirectory(path2);
            // 开启 log4net 内部调试信息输出到控制台
            log4net.Util.LogLog.InternalDebugging = true;
            //允许将这些内部信息输出到 Console.Error
            log4net.Util.LogLog.EmitInternalMessages = true;
            // 设置输出路径（仅限新版本）
            var debugPath = Path.Combine(path2, $"log4net_debug_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            //这相当于把 Console.WriteLine() 的输出重定向到了文件，log4net 内部错误也会跟着输出进去。
            //AutoFlush = true —— 保证每次写入立即刷到文件，不会缓存在内存里。
            Console.SetOut(new StreamWriter(debugPath, append: true) { AutoFlush = true });

            XmlConfigurator.ConfigureAndWatch(new FileInfo("Config\\log4net.config"));
        }

        /// <summary>
        /// IOC配置
        /// </summary>
        private  void CreateIoc()
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
                    .AddSingleton<LiveChartsTestViewModel>()
                    .AddSingleton<LoginViewModel>()
                    .AddSingleton<SerialToolViewModel>()
                    .AddSingleton<SetLiveChartsParamViewModel>()

                    //页面单例 ,预加载
                    // .AddSingleton<LoginWindowPage1>()
                    // .AddSingleton<LoginWindowPage2>()
                    // .AddSingleton<LoginWindowManagePage>()
                    // .AddSingleton<LoginWindowRegisterPage>()
                    .AddSingleton<SerialToolPage>()
                    .AddSingleton<StationPage>()
                    .AddSingleton<VisionPage>()
                    .AddSingleton<MesTcpPage>()
                    .AddSingleton<LoadMesPage>()
                    .AddSingleton<HomePage>()
                    .AddSingleton<ProductivePage>()
                    .AddSingleton<SettingsPage>()
                    .AddSingleton<ModbusToolPage>()
                    .AddSingleton<TcpToolPage>()
                    .AddTransient<LiveChartsTestPage>() //AddTransient每次导航会new 一个新对象
                    // .BuildServiceProvider()  Microsoft.Extensions.DependencyInjection原生
                    .BuildDynamicProxyProvider() //AspectCore中的Ioc,支持Aop
            );
        }

        /// <summary>
        /// 加载引用的DLL配置
        /// </summary>
        private  void LoadDll()
        {
            // AppDomain.CurrentDomain.BaseDirectory 获取当前程序（即 .exe 可执行文件）所在的根目录路径，结尾自带 \ 
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib");
            //创建临时环境变量 , 下面代码代表 先获取PATH环境变量,然后在PATH前面添加path变量,不是永久的,程序关闭失效
            Environment.SetEnvironmentVariable("PATH", path + ";" + Environment.GetEnvironmentVariable("PATH"));
        }

        /// <summary>
        /// 程序结束的时候
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            SettingsPageViewModel viewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
            if (viewModel.SettingsPageModel.OffSave)
            {
                viewModel.SaveAll();
            }

            log4net.LogManager.GetLogger(typeof(App)).Info("Pkn_HostSystem程序退出");
            //通知 log4net 停止所有日志写入 ,避免程序关闭太快导致缓冲区未刷新
            log4net.LogManager.Shutdown();
            _mutex.Dispose();
            base.OnExit(e);
        }
    }
}