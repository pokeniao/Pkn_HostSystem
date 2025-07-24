using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
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

    public List<string> VerifyType { get; set; } = ["字符长度检测=", "字符长度检测!=", "字符长度检测>", "字符长度检测<", "字符长度检测>=", "字符长度检测=<", "字符=", "字符!=","数据>", "数据>=", "数据<", "数据<=", "数据=", "数据!=", "正则表达式检测"];

    public List<string> GetMessageType { get; set; } = ["HTTP", "通讯","内部","自定义"];

    public ObservableCollection<string> ForwardingMethod { get; set; } = ["通讯", "内部地址","队列"];

    public List<string> SendMessageMethod { get; set; } = ["常量","内部地址","队列"];

    public List<string> InteriorMethod { get; set; } = ["常量", "结果Json解析","方法集"];

    public List<string> InteriorNames { get; set; } = ["集合","队列"];

    public List<Type> UserDefinedList { get; set; } = GlobalManager.GetUserDefinedTypes();
    public MesTcpViewModel()
    {
        SnackbarService = new SnackbarService();


        MesTcpModel = JsonTool<MesTcpModel>.Load();
        if (MesTcpModel == null)
        {
            //Model初始化
            MesTcpModel = new MesTcpModel()
            {
                NetWorkList = new ObservableCollectionExtended<NetWork>(),
                HttpList = new ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel>(),
                DynNetList = new ObservableCollectionExtended<LoadMesDynContent>(),
            };
            GlobalManager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe();
            GlobalManager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
            MesTcpModel.HttpList = Ioc.Default.GetRequiredService<LoadMesPageViewModel>().LoadMesPageModel.MesPojoList;
        }
        else
        {
            GlobalManager.NetWorkDictionary.Connect().Bind(MesTcpModel.NetWorkList).Subscribe(); //绑定
            GlobalManager.DynDictionary.AddOrUpdate(MesTcpModel.DynNetList); //存入到缓存,后面在绑定
            GlobalManager.DynDictionary.Connect().Bind(MesTcpModel.DynNetList).Subscribe();
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
            if (GlobalManager.DynDictionary.Lookup(addDynWindow.viewModel.Name).HasValue)
            {
                log.WarningAndShow("添加动态通讯名称已存在", $"添加动态通讯名称已存在{addDynWindow.viewModel.Name}");
                return;
            }

            GlobalManager.DynDictionary.AddOrUpdate(loadMesDynContent);
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
            if (GlobalManager.DynDictionary.Lookup(mesTcpPojo.Name).HasValue)
            {
                GlobalManager.DynDictionary.Remove(mesTcpPojo);
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
        JsonTool<MesTcpModel>.Save(MesTcpModel);
    }
}