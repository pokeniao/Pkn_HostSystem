using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Core.Interface;
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

    public TimeSpan TimeSpan { get; set; } = new TimeSpan(1, 1, 1, 1);
    public LoadMesAddAndUpdateWindowModel LoadMesAddAndUpdateWindowModel { get; set; }

    public LoadMesAddAndUpdateWindowModel LoadMesAddAndUpdateWindowModelBefore { get; set; }

    public LogControl<LoadMesAddAndUpdateWindowsViewModel> Log;

    public SnackbarService SnackbarService { get; set; } = new();

    public bool add;

    //当前已存在集合
    public ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel> mesPojoList;


    public ObservableCollection<string> ReqMethodList { get; set; } = ["动态获取", "常量", "方法集"];

    public ObservableCollection<string> TriggerType { get; set; } = ["循环触发", "通讯触发" , "内部触发"];

    public ObservableCollection<string> MethodCollection { get; set; } =
        ["当前时间(yyyy-MM-dd HH:mm:ss)", "当前时间(yyyy/MM/dd HH:mm:ss)", "当前时间(yyyy-MM-dd)", "当前时间(yyyy/MM/dd)","当前时间(13位时间戳)"];

    public ObservableCollection<string> RequestMethods { get; set; } = ["JSON", "XML", "TEXT"];

    public ObservableCollection<string> ForwardingMethod { get; set; } = ["ModbusTcp", "队列"];


    public List<string> TriggerReturnMethodList { get; set; } = ["常量返回", "内部寄存器"];


    public List<string> LocalSaveMethod { get; set; } = ["直接保存", "特定时间保存(满足时间要求保存多次)", "特定小时保存(满足时间要求保存多次)", "特定时间保存(当前时间内只保存一次)", "特定小时保存(当前时间内只保存一次)"];

    public List<string> LocalSaveDirectoryMethod { get; set; } = ["默认", "指定目录名"];
    public List<string> LocalSaveFileNameMethod { get; set; } = ["默认", "指定文件名","时间命名(按天)","时间命名(按月)"];

    public ObservableCollectionExtended<LoadMesDynContent> Para_dyn { get; set; } = new();

    /// <summary>
    /// 可选择的工单任务
    /// </summary>
    [ObservableProperty] private ObservableCollectionExtended<IEachStation> stations = new ObservableCollectionExtended<IEachStation>();
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
        Log = new LogControl<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        add = true;
        GlobalManager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        GlobalManager.StationDictionary.Connect().Bind(Stations).Subscribe();
        GlobalManager.NetWorkDictionary.Connect().Bind(LoadMesAddAndUpdateWindowModel.NetWorkList).Subscribe();
    }

    //修改
    public LoadMesAddAndUpdateWindowsViewModel(LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel)
    {
        //原地址
        LoadMesAddAndUpdateWindowModelBefore = loadMesAddAndUpdateWindowModel;
        //深拷贝一份进行修改
        LoadMesAddAndUpdateWindowModel = JsonTool<LoadMesAddAndUpdateWindowModel>.DeepClone(loadMesAddAndUpdateWindowModel);

        Log = new LogControl<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        add = false;
        GlobalManager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        GlobalManager.StationDictionary.Connect().Bind(Stations).Subscribe();
        GlobalManager.NetWorkDictionary.Connect().Bind(LoadMesAddAndUpdateWindowModel.NetWorkList).Subscribe();
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
            JsonTool<LoadMesAddAndUpdateWindowModel>.PopulateObject(LoadMesAddAndUpdateWindowModel,
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
            Log.WarningAndShowTask("Name不能为空", "用户添加时,输入的Name参数不正确,Name不能为空");
            return false;
        }

        foreach (var item in mesPojoList)
        {
            if (add)
            {
                if (item.Name == verifyObject.Name) 
                {
                    Log.WarningAndShowTask("名称已存在,请修改", "用户添加时,输入的Name参数已存在");
                    return false;
                }
            }
            else
            {
                if (item.Name == verifyObject.Name && item.Name != LoadMesAddAndUpdateWindowModelBefore?.Name)
                {
                    Log.WarningAndShowTask("名称已存在,请修改", "用户添加时,输入的Name参数已存在");
                    return false;
                }
            }
        }

        //如果需要发送HTTP请求进行校验
        if (verifyObject.HttpNeed)
        {
            if (verifyObject.HttpPath == null)
            {
                Log.WarningAndShowTask("请求路径不能为空", "用户添加时,输入的请求路径参数不正确,请求路径不能为空");
                return false;
            }

            if (verifyObject.Api == null)
            {
                Log.WarningAndShowTask("API不能为空", "用户添加时,输入的API参数不正确,API不能为空");
                return false;
            }
            if (verifyObject.Ajax == "POST" && verifyObject.Request == null)
            {
                Log.WarningAndShowTask("POST请求,请求体不能为空", "用户添加时,输入的请求体参数不正确,POST请求,请求体不能为空");
                return false;
            }
            string pattern;
            pattern = @"[a-zA-z]+://[^\s]*";
            if (!Regex.IsMatch(LoadMesAddAndUpdateWindowModel.HttpPath, pattern))
            {
                Log.WarningAndShowTask("请求路径格式错误", "用户添加时,输入的请求路径参数不正确");
                return false;
            }
        }

        if (verifyObject.TriggerType == "通讯触发")
        {
            if (verifyObject.TriggerConnectName == null)
            {
                Log.WarningAndShowTask("通讯触发对象不能为空", "用户添加或修改时,通讯触发未选择");
                return false;
            }

            if (verifyObject.TriggerMessage == null)
            {
                Log.WarningAndShowTask("触发消息不能未null", "用户添加或修改时,触发消息不能未null");
                return false;
            }

            if (verifyObject.SuccessResponseMessage == null)
            {
                Log.WarningAndShowTask("触发成功返回消息不能未null", "用户添加或修改时,触发成功返回消息不能未null");
                return false;
            }

            if (verifyObject.FailResponseMessage ==null)
            {
                Log.WarningAndShowTask("触发失败消息不能未null", "用户添加或修改时,触发失败返回消息不能未null");
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