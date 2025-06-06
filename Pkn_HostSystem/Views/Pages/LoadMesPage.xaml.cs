﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;

namespace Pkn_HostSystem.Views.Pages
{
    /// <summary>
    /// LoadMesPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoadMesPage : Page
    {

        public LoadMesPageViewModel LoadMesPageViewModel { get; set; }


        public LogBase<LoadMesPage> Log = new LogBase<LoadMesPage>();
        public LoadMesPage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
            LoadMesPageViewModel = (LoadMesPageViewModel)DataContext;
            LoadMesPageViewModel.setSnackbarService(SnackbarPresenter);
            //添加为ReturnMessageList监听
            LoadMesPageViewModel.LoadMesPageModel.ReturnMessageList.CollectionChanged += Item_CollectionChanged;
        }

        private void Item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //不在聚焦,自动下滑
                if (!HttpLogListBox.IsKeyboardFocusWithin)
                {
                    if (HttpLogListBox != null && HttpLogListBox.Items.Count > 0)
                    {
                        try
                        {
                            HttpLogListBox.ScrollIntoView(HttpLogListBox.Items[^1]);
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"页面下滑刷新报错:--{exception}");
                        }
                    }
                }
            });
        }
        /// <summary>
        /// 滑动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (HttpLogListBox.Items.Count == 0)
            {
                return;
            }
            HttpLogListBox.ScrollIntoView(HttpLogListBox.Items[^1]);
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            GlobalMannager.GlobalDictionary.TryGetValue("MesLogListBox", out var obj);
            ObservableCollection<string> list = (ObservableCollection<string>)obj;
            list.Clear();
        }
        /// <summary>
        /// 页面大小发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadMesPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataGrid.MaxHeight = e.NewSize.Height-100;
            HttpLogListBoxBorder.MaxHeight = e.NewSize.Height -100;
        }
    }
}
