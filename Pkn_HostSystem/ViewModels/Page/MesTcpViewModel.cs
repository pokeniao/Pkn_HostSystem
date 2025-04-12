using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class MesTcpViewModel : ObservableRecipient
{
    public SnackbarService SnackbarService { get; set; }
    public LogBase<MesTcpViewModel> log;

    public MesTcpModel MesTcpModel { get; set; }


    public MesTcpViewModel()
    {
        SnackbarService = new SnackbarService();


        MesTcpModel = AppJsonStorage<MesTcpModel>.Load();
        if (MesTcpModel == null)
        {
            //Model初始化
            MesTcpModel = new MesTcpModel()
            {
                NetWorkList = new ObservableCollectionExtended<NetWorkPoJo>(),
                DynNetList = new ObservableCollectionExtended<MesTcpPojo>(),
            };
            GlobalMannager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe();
            GlobalMannager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
        }
        else
        {
            //GlobalMannager.NetWorkDictionary.AddOrUpdate(MesTcpModel.NetWorkList);
            GlobalMannager.DynDictionary.AddOrUpdate(MesTcpModel.DynNetList);
            GlobalMannager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe();
            GlobalMannager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
        }
        log = new LogBase<MesTcpViewModel>(SnackbarService);
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
                DynCondition = new ObservableCollection<DynConditionItem>(),
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

    [RelayCommand]
    public void DeleteDynCondition(MesTcpPage page)
    {
        DynConditionItem? item = page.DynConditionDataGrid.SelectedItem as DynConditionItem;

        MesTcpPojo? mesTcpPojo = page.DynNameListBox.SelectedItem as MesTcpPojo;
        if (item != null)
        {
            if (mesTcpPojo.DynCondition.Remove(item))
            {
                log.SuccessAndShowTask("删除成功");
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

    [RelayCommand]
    public void Save()
    {
        AppJsonStorage<MesTcpModel>.Save(MesTcpModel);
    }
}