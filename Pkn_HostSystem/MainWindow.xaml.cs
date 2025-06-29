﻿using CommunityToolkit.Mvvm.DependencyInjection;
using log4net.Config;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui;
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
            // // Loaded += (_, _) => RootNavigation.Navigate(typeof(HomePage));
            Loaded += (_, _) =>
            {
                var navigation = Ioc.Default.GetRequiredService<INavigationService>();
                navigation.SetNavigationControl(RootNavigation);

                // 默认导航页面
                navigation.Navigate(typeof(HomePage));

                PreLoad();
                _ = Starting();
            };
       
        }

        private void PreLoad()
        {
            _ = Ioc.Default.GetRequiredService<HomePage>();
            _ = Ioc.Default.GetRequiredService<LoadMesPage>();
            _ = Ioc.Default.GetRequiredService<MesTcpPage>();
            _ = Ioc.Default.GetRequiredService<SettingsPage>();
        }

        private async Task Starting()
        {
            //判断软件开启,启动的连接,自动进行连接
            HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            ObservableCollection<NetworkDetailed> ConnectPojos = homePageViewModel.HomePageModel.SetConnectDg;
            List<Task> connectTasks = new List<Task>();
            foreach (var connectPojo in ConnectPojos)
            {
                if (connectPojo.Open == true)
                {
                    TraceContext.Name = connectPojo.Name;
                    Task startConnectModbusTask = homePageViewModel.StartConnect(connectPojo);
                    connectTasks.Add(startConnectModbusTask);
                    TraceContext.Name =null;
                }
            }

            //判断 Http请求是否开启的,自动进行连接
            LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
            ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList =
                loadMesPageViewModel.LoadMesPageModel.MesPojoList;

            foreach (var mesPojo in mesPojoList)
            {
                if (mesPojo.RunCyc)
                {
                    TraceContext.Name = mesPojo.Name;
                    loadMesPageViewModel.IsRun(mesPojo);
                    TraceContext.Name = null;
                }
            }

            //判断消费者和生产者模式是否开启,自动连接
            ProductiveViewModel productiveViewModel = Ioc.Default.GetRequiredService<ProductiveViewModel>();
            ObservableCollection<Productive> productiveModelProductives =
                productiveViewModel.ProductiveModel.Productives;

            foreach (var productive in productiveModelProductives)
            {
                if (productive.Run)
                {
                    TraceContext.Name = productive.Name;
                    productiveViewModel.TriggerCyc(productive);
                    TraceContext.Name = null;
                }
            }

            //执行每隔5分钟进行一次自动保存
            Task.Run(async () =>
            {
                await Task.Delay(300 * 1000);
                var settingsPageViewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
                settingsPageViewModel.Save();
            });
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