using System.IO;
using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WPF_NET.Views.Pages;


namespace WPF_NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
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
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
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