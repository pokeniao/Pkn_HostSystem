using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
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

    public List<string> VerifyType { get; set; } = ["字符长度检测=", "字符长度检测!=", "字符长度检测>", "字符长度检测<", "字符长度检测>=", "字符长度检测=<", "字符=", "字符!=", "正则表达式检测"];

    public List<string> GetMessageType { get; set; } = ["HTTP", "通讯"];

    public ObservableCollection<string> ForwardingMethod { get; set; } = ["ModbusTcp", "队列"];
    public MesTcpViewModel()
    {
        SnackbarService = new SnackbarService();


        MesTcpModel = AppJsonTool<MesTcpModel>.Load();
        if (MesTcpModel == null)
        {
            //Model初始化
            MesTcpModel = new MesTcpModel()
            {
                NetWorkList = new ObservableCollectionExtended<NetWork>(),
                HttpList = new ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel>(),
                DynNetList = new ObservableCollectionExtended<LoadMesDynContent>(),
            };
            GlobalMannager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe();
            GlobalMannager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
            MesTcpModel.HttpList = Ioc.Default.GetRequiredService<LoadMesPageViewModel>().LoadMesPageModel.MesPojoList;
        }
        else
        {
            GlobalMannager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe(); //绑定
            GlobalMannager.DynDictionary.AddOrUpdate(MesTcpModel.DynNetList); //存入到缓存,后面在绑定
            GlobalMannager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
            MesTcpModel.HttpList = Ioc.Default.GetRequiredService<LoadMesPageViewModel>().LoadMesPageModel.MesPojoList;
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
            LoadMesDynContent loadMesDynContent = new LoadMesDynContent()
            {
                Name = addDynWindow.viewModel.Name,
                DynCondition = new ObservableCollection<DynCondition>(),
            };
            if (GlobalMannager.DynDictionary.Lookup(addDynWindow.viewModel.Name).HasValue)
            {
                log.WarningAndShow("添加动态通讯名称已存在", $"添加动态通讯名称已存在{addDynWindow.viewModel.Name}");
                return;
            }

            GlobalMannager.DynDictionary.AddOrUpdate(loadMesDynContent);
        }
    }

    /// <summary>
    /// 删除一行数据
    /// </summary>
    [RelayCommand]
    public void DeleteDyn(MesTcpPage page)
    {
        LoadMesDynContent? mesTcpPojo = page.DynNameListBox.SelectedItem as LoadMesDynContent;
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
        DynCondition? item = page.DynConditionDataGrid.SelectedItem as DynCondition;

        LoadMesDynContent? mesTcpPojo = page.DynNameListBox.SelectedItem as LoadMesDynContent;
        if (item != null)
        {
            if (mesTcpPojo != null && mesTcpPojo.DynCondition.Remove(item))
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
        AppJsonTool<MesTcpModel>.Save(MesTcpModel);
    }
}