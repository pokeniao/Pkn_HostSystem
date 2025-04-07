using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using WPF_NET.Base;
using WPF_NET.Models;
using WPF_NET.Pojo;
using WPF_NET.Pojo.Page.MESTcp;
using WPF_NET.Static;
using WPF_NET.Views.Pages;
using WPF_NET.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace WPF_NET.ViewModels;

public partial class MesTcpViewModel : ObservableRecipient
{
    public SnackbarService SnackbarService { get; set; }
    public LogBase<MesTcpViewModel> log;
    public MesTcpModel MesTcpModel { get; set; }

  
    public MesTcpViewModel()
    {
        SnackbarService = new SnackbarService();
        log = new LogBase<MesTcpViewModel>(SnackbarService);
        //Model初始化
        MesTcpModel = new MesTcpModel()
        {
            NetWorkList = new ObservableCollectionExtended<NetWorkPoJo>(),
            DynNetList = new ObservableCollectionExtended<MesTcpPojo>(),
        };
        GlobalMannager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe();
        GlobalMannager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
    }

    #region dyn添加删除修改

    /// <summary>
    /// 添加一行数据
    /// </summary>
    [RelayCommand]
    public void AddDyn()
    {
        AddDynWindow addDynWindow = new AddDynWindow();
        bool? dialog = addDynWindow.ShowDialog();
        if (dialog == true)
        {
            MesTcpPojo mesTcpPojo = new MesTcpPojo()
            {
                Name = addDynWindow.viewModel.Name,
                DynCondition = new ObservableCollection<DynConditionItem>()
            };
            if (GlobalMannager.DynDictionary.Lookup(addDynWindow.viewModel.Name).HasValue)
            {
                log.WarningAndShow("添加动态通讯名称已存在", $"添加动态通讯名称已存在{addDynWindow.viewModel.Name}");
                return;
            }

            GlobalMannager.DynDictionary.AddOrUpdate(mesTcpPojo);
        }
    }

    /// <summary>
    /// 删除一行数据
    /// </summary>
    [RelayCommand]
    public void DeleteDyn(MesTcpPage page)
    {
        MesTcpPojo? mesTcpPojo = page.DynNameListBox.SelectedItem as MesTcpPojo;
        if (mesTcpPojo != null)
        {
            if (GlobalMannager.DynDictionary.Lookup(mesTcpPojo.Name).HasValue)
            {
                GlobalMannager.DynDictionary.Remove(mesTcpPojo);
            }
            else
            {
                log.WarningAndShow("删除已经不存在");
                return;
            }
        }
    }

    #endregion


    #region 弹窗SnackbarService

    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion
}