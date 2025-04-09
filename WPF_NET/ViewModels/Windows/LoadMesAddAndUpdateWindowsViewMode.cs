using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Text.RegularExpressions;
using DynamicData;
using DynamicData.Binding;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WPF_NET.Base;
using WPF_NET.Message;
using WPF_NET.Models;
using WPF_NET.Pojo;
using WPF_NET.Static;
using WPF_NET.Views.Windows;
using WPF_NET.Pojo.Page.MESTcp;

namespace WPF_NET.ViewModels.Windows;

public partial class LoadMesAddAndUpdateWindowsViewModel : ObservableRecipient
{
    public LoadMesAddAndUpdateWindowModel LoadMesAddAndUpdateWindowModel { get; set; }

    public LogBase<LoadMesAddAndUpdateWindowsViewModel> Log;

    public SnackbarService SnackbarService { get; set; } = new SnackbarService();

    public bool add;

    public ObservableCollection<string> ReqMethodList { get; set; } = ["动态获取", "常量", "方法集"];

    public ObservableCollectionExtended<MesTcpPojo> Para_dyn { get; set; } = new ObservableCollectionExtended<MesTcpPojo>();

    public LoadMesAddAndUpdateWindowsViewModel()
    {
        LoadMesAddAndUpdateWindowModel = new LoadMesAddAndUpdateWindowModel()
        {
            Ajax = "POST",
            Condition = new ObservableCollection<ConditionItem>()
            {
            }
        };
        Log = new LogBase<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        GlobalMannager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
        add = true;
    }

    public LoadMesAddAndUpdateWindowsViewModel(LoadMesAddAndUpdateWindowModel _LoadMesAddAndUpdateWindowModel)
    {
        LoadMesAddAndUpdateWindowModel = _LoadMesAddAndUpdateWindowModel;
        Log = new LogBase<LoadMesAddAndUpdateWindowsViewModel>(SnackbarService);
        add = false;
        GlobalMannager.DynDictionary.Connect().Bind(Para_dyn).Subscribe();
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
        if (!Regex.IsMatch(LoadMesAddAndUpdateWindowModel.HttpPath, pattern))
        {
            Log.WarningAndShow("请求路径格式错误", "用户添加时,输入的请求路径参数不正确");
            return;
        }

        //发送消息
        if (add)
        {
            //发送消息体
            WeakReferenceMessenger.Default.Send(new MesMessage(LoadMesAddAndUpdateWindowModel));
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