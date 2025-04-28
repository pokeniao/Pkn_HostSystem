using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Message;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows;

public partial class LoadMesAddAndUpdateWindowsViewModel : ObservableRecipient
{
    public LoadMesAddAndUpdateWindowModel LoadMesAddAndUpdateWindowModel { get; set; }

    public LogBase<LoadMesAddAndUpdateWindowsViewModel> Log;

    public SnackbarService SnackbarService { get; set; } = new SnackbarService();

    public bool add;
    //当前已存在集合
    public ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;

    public string LastName;

    public ObservableCollection<string> ReqMethodList { get; set; } = ["动态获取", "常量", "方法集"];

    public ObservableCollection<string> TriggerType { get; set; } = ["循环触发", "消息触发"];

    public ObservableCollection<string> MethodCollection { get; set; } = ["当前时间(yyyy-MM-dd HH:mm:ss)", "当前时间(yyyy/MM/dd HH:mm:ss)", "当前时间(yyyy-MM-dd)", "当前时间(yyyy/MM/dd)"];

    public ObservableCollection<string> RequestMethods { get; set; } = ["JSON", "XML", "TEXT"];
    public ObservableCollectionExtended<LoadMesDynContent> Para_dyn { get; set; } = new ObservableCollectionExtended<LoadMesDynContent>();
    //添加
    public LoadMesAddAndUpdateWindowsViewModel()
    {
        LoadMesAddAndUpdateWindowModel = new LoadMesAddAndUpdateWindowModel()
        {
            Ajax = "POST",
            CycTime = 300,
            RequestMethod = "JSON",
            Condition = new ObservableCollection<LoadMesCondition>()
            {
            },
            NetWorkList = new ObservableCollectionExtended<NetWork>()
        };
        Log = new LogBase<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        GlobalMannager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        add = true;
        GlobalMannager.NetWorkDictionary.Connect().Bind(LoadMesAddAndUpdateWindowModel.NetWorkList).Subscribe();
    }
    //修改
    public LoadMesAddAndUpdateWindowsViewModel(LoadMesAddAndUpdateWindowModel _LoadMesAddAndUpdateWindowModel)
    {
        LoadMesAddAndUpdateWindowModel = _LoadMesAddAndUpdateWindowModel;
        Log = new LogBase<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        add = false;
        GlobalMannager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        LastName = _LoadMesAddAndUpdateWindowModel.Name;
        GlobalMannager.NetWorkDictionary.Connect().Bind(LoadMesAddAndUpdateWindowModel.NetWorkList).Subscribe();
    }

    /// <summary>
    /// 点击确定
    /// </summary>
    /// <param name="window"></param>
    [RelayCommand]
    public void Confirm(LoadMesAddWindow window)
    {
        //先判断是否为空
        if (LoadMesAddAndUpdateWindowModel.Name == null)
        {
            Log.WarningAndShow("Name不能为空", "用户添加时,输入的Name参数不正确,Name不能为空");
            return;
        }

        foreach (var item in mesPojoList)
        {
            if (item.Name == LoadMesAddAndUpdateWindowModel.Name && item.Name != LastName)
            {
                Log.WarningAndShow("名称已存在,请修改", "用户添加时,输入的Name参数已存在");
                return;
            }
        }

        if (LoadMesAddAndUpdateWindowModel.HttpPath == null)
        {
            Log.WarningAndShow("请求路径不能为空", "用户添加时,输入的请求路径参数不正确,请求路径不能为空");
            return;
        }

        if (LoadMesAddAndUpdateWindowModel.Api == null)
        {
            Log.WarningAndShow("API不能为空", "用户添加时,输入的API参数不正确,API不能为空");
            return;
        }

        if (LoadMesAddAndUpdateWindowModel.Ajax == "POST" && LoadMesAddAndUpdateWindowModel.Request == null)
        {
            Log.WarningAndShow("POST请求,请求体不能为空", "用户添加时,输入的请求体参数不正确,POST请求,请求体不能为空");
            return;
        }

        string pattern;
        pattern = @"[a-zA-z]+://[^\s]*";
        if (!Regex.IsMatch((string)LoadMesAddAndUpdateWindowModel.HttpPath, pattern))
        {
            Log.WarningAndShow("请求路径格式错误", "用户添加时,输入的请求路径参数不正确");
            return;
        }

        //发送消息
        if (add)
        {
            //发送消息体
            WeakReferenceMessenger.Default.Send(new AddOneMesMessage(LoadMesAddAndUpdateWindowModel));
        }

        window.DialogResult = true;
        window.Close();
    }

    /// <summary>
    /// 点击取消
    /// </summary>
    /// <param name="window"></param>
    [RelayCommand]
    public void Cancel(LoadMesAddWindow window)
    {
        window.Close();
    }

    public void setSnackbarService(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }
}