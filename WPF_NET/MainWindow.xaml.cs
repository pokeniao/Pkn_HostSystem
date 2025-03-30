using System.IO;
using Wpf.Ui.Appearance;
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
            SystemThemeWatcher.Watch(this);
            InitializeComponent();

            // Loaded:当元素被布局、呈现并准备好进行交互时，将触发此事件
            //设置页面打开根目录
            Loaded += (_, _) => RootNavigation.Navigate(typeof(HomePage));
            //记录日志
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
        }
    }
}