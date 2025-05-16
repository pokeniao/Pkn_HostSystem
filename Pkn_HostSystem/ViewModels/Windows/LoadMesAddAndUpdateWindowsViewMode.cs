using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using DynamicData.Binding;
using Force.DeepCloner;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
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

    public LoadMesAddAndUpdateWindowModel LoadMesAddAndUpdateWindowModelBefore { get; set; }

    public LogBase<LoadMesAddAndUpdateWindowsViewModel> Log;

    public SnackbarService SnackbarService { get; set; } = new();

    public bool add;

    //当前已存在集合
    public ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;


    public ObservableCollection<string> ReqMethodList { get; set; } = ["动态获取", "常量", "方法集"];

    public ObservableCollection<string> TriggerType { get; set; } = ["循环触发", "消息触发"];

    public ObservableCollection<string> MethodCollection { get; set; } =
        ["当前时间(yyyy-MM-dd HH:mm:ss)", "当前时间(yyyy/MM/dd HH:mm:ss)", "当前时间(yyyy-MM-dd)", "当前时间(yyyy/MM/dd)"];

    public ObservableCollection<string> RequestMethods { get; set; } = ["JSON", "XML", "TEXT"];

    public ObservableCollection<string> ForwardingMethod { get; set; } = ["ModbusTcp", "队列"];

    public ObservableCollectionExtended<LoadMesDynContent> Para_dyn { get; set; } = new();

    //添加
    public LoadMesAddAndUpdateWindowsViewModel()
    {
        LoadMesAddAndUpdateWindowModel = new LoadMesAddAndUpdateWindowModel()
        {
            Ajax = "POST",
            CycTime = 300,
            RequestMethod = "JSON",
            Condition = new ObservableCollection<LoadMesCondition>() { },
            NetWorkList = new ObservableCollectionExtended<NetWork>()
        };
        Log = new LogBase<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        GlobalMannager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        add = true;
        GlobalMannager.NetWorkDictionary.Connect().Bind(LoadMesAddAndUpdateWindowModel.NetWorkList).Subscribe();
    }

    //修改
    public LoadMesAddAndUpdateWindowsViewModel(LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel)
    {
        //原地址
        LoadMesAddAndUpdateWindowModelBefore = loadMesAddAndUpdateWindowModel;
        //深拷贝一份进行修改
        LoadMesAddAndUpdateWindowModel = AppJsonTool<LoadMesAddAndUpdateWindowModel>.DeepClone(loadMesAddAndUpdateWindowModel);

        Log = new LogBase<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        add = false;
        GlobalMannager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        GlobalMannager.NetWorkDictionary.Connect().Bind(LoadMesAddAndUpdateWindowModel.NetWorkList).Subscribe();
    }

    /// <summary>
    /// 点击确定
    /// </summary>
    /// <param name="window"></param>
    [RelayCommand]
    public void Confirm(LoadMesAddWindow window)
    {
        bool succeed;
        if (add)
        {
            succeed = verify(LoadMesAddAndUpdateWindowModel);
            if (!succeed)
            {
                return;
            }
            //发送消息体
            WeakReferenceMessenger.Default.Send(new AddOneMesMessage(LoadMesAddAndUpdateWindowModel));
        }
        else
        {
            succeed = verify(LoadMesAddAndUpdateWindowModel);
            if (!succeed)
            {
                return;
            }
            //提交修改后的
            AppJsonTool<LoadMesAddAndUpdateWindowModel>.ApplyPartialJson(LoadMesAddAndUpdateWindowModel,
                LoadMesAddAndUpdateWindowModelBefore);
        }

        window.DialogResult = true;
        window.Close();
    }

    private bool verify(LoadMesAddAndUpdateWindowModel verifyObject)
    {
        //先判断是否为空
        if (verifyObject.Name == null)
        {
            Log.WarningAndShow("Name不能为空", "用户添加时,输入的Name参数不正确,Name不能为空");
            return false;
        }

        foreach (var item in mesPojoList)
        {
            if (add)
            {
                if (item.Name == verifyObject.Name) 
                {
                    Log.WarningAndShow("名称已存在,请修改", "用户添加时,输入的Name参数已存在");
                    return false;
                }
            }
            else
            {
                if (item.Name == verifyObject.Name && item.Name != LoadMesAddAndUpdateWindowModelBefore?.Name)
                {
                    Log.WarningAndShow("名称已存在,请修改", "用户添加时,输入的Name参数已存在");
                    return false;
                }
            }
        }

        //如果需要发送HTTP请求进行校验
        if (verifyObject.HttpNeed)
        {
            if (verifyObject.HttpPath == null)
            {
                Log.WarningAndShow("请求路径不能为空", "用户添加时,输入的请求路径参数不正确,请求路径不能为空");
                return false;
            }

            if (verifyObject.Api == null)
            {
                Log.WarningAndShow("API不能为空", "用户添加时,输入的API参数不正确,API不能为空");
                return false;
            }
            if (verifyObject.Ajax == "POST" && verifyObject.Request == null)
            {
                Log.WarningAndShow("POST请求,请求体不能为空", "用户添加时,输入的请求体参数不正确,POST请求,请求体不能为空");
                return false;
            }
            string pattern;
            pattern = @"[a-zA-z]+://[^\s]*";
            if (!Regex.IsMatch(LoadMesAddAndUpdateWindowModel.HttpPath, pattern))
            {
                Log.WarningAndShow("请求路径格式错误", "用户添加时,输入的请求路径参数不正确");
                return false;
            }
        }

        if (verifyObject.TriggerType == "消息触发")
        {
            if (verifyObject.TriggerConnectName == null)
            {
                Log.WarningAndShow("消息触发对象不能为空", "用户添加或修改时,消息触发未选择");
                return false;
            }

            if (verifyObject.TriggerMessage == null)
            {
                Log.WarningAndShow("触发消息不能未null", "用户添加或修改时,触发消息不能未null");
                return false;
            }

            if (verifyObject.SuccessResponseMessage == null)
            {
                Log.WarningAndShow("触发成功返回消息不能未null", "用户添加或修改时,触发成功返回消息不能未null");
                return false;
            }

            if (verifyObject.FailResponseMessage ==null)
            {
                Log.WarningAndShow("触发失败消息不能未null", "用户添加或修改时,触发失败返回消息不能未null");
                return false;
            }
        }
        return true;
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