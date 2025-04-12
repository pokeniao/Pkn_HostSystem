using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //预加载
        public MainWindow()
        {
            //这个赋值不能少,因为用了框架,少了,下面代码会运行报错,但是也不会影响执行
            DataContext = this;

            //如果系统主题或颜色改变，自动更新应用程序背景。
            //SystemThemeWatcher.Watch(this);

            //启动先自适应电脑主题
            ApplicationThemeManager.ApplySystemTheme();
            InitializeComponent();

            // Loaded:当元素被布局、呈现并准备好进行交互时，将触发此事件
            //设置页面打开根目录
            Loaded += (_, _) => RootNavigation.Navigate(typeof(HomePage));
            //记录日志
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("Config\\log4net.config"));
            PreLoad();
            Starting();
        }

        private void PreLoad()
        {
            _ = Ioc.Default.GetRequiredService<HomePage>();
            _ = Ioc.Default.GetRequiredService<LoadMesPage>();
            _ = Ioc.Default.GetRequiredService<MesTcpPage>();
        }

        private void Starting()
        {
            //判断软件开启,启动的连接,自动进行连接
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            ObservableCollection<ConnectPojo> ConnectPojos = homePageViewModel.HomePageModel.SetConnectDg;
            foreach (var connectPojo in ConnectPojos)
            {
                if (connectPojo.Open == true)
                {
                    homePageViewModel.StartConnectModbus(connectPojo);
                }
            }

            //判断 Http请求是否开启的,自动进行连接
            LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
            ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList = loadMesPageViewModel.LoadMesPageModel.MesPojoList;

            foreach (var mesPojo in mesPojoList)
            {
                if (mesPojo.RunCyc)
                {
                    loadMesPageViewModel.OpenCyc(mesPojo);
                }
            }
        }
        #region 自动伸缩的Navigation

        //起到互锁的作用,用户在操作和当页面缩放,不能同时
        private bool _isUserClosedPane;
        private bool _isPaneOpenedOrClosedFromCode;

        private void RootNavigation_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
        {
            if (_isPaneOpenedOrClosedFromCode)
            {
                return;
            }

            _isUserClosedPane = true;
        }

        private void RootNavigation_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
        {
            if (_isPaneOpenedOrClosedFromCode)
            {
                return;
            }

            _isUserClosedPane = false;
        }


        //页面大小方式改变
        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isUserClosedPane)
            {
                return;
            }

            _isPaneOpenedOrClosedFromCode = true;
            RootNavigation.SetCurrentValue(NavigationView.IsPaneOpenProperty, e.NewSize.Width > 1100);
            _isPaneOpenedOrClosedFromCode = false;
        }

        #endregion
    }
}