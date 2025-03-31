using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WPF_NET.Base;
using WPF_NET.Message;
using WPF_NET.Models;
using WPF_NET.Server.LoadMes;
using WPF_NET.Views.Pages;
using WPF_NET.Views.Windows;
using MessageBox = WPF_NET.Views.Windows.MessageBox;

namespace WPF_NET.ViewModels;

public partial class LoadMesPageViewModel : ObservableRecipient, IRecipient<MesMessage>
{
    public LoadMesPageModel LoadMesPageModel { get; set; }

    public LoadMesServer LoadMesServer { get; set; }
    public SnackbarService SnackbarService { get; set; }


    public LogBase<LoadMesPageViewModel> log;

    public LoadMesPageViewModel()
    {
        LoadMesPageModel = new LoadMesPageModel()
        {
            MesPojoList = []
        };
        SnackbarService = new SnackbarService();
        log = new LogBase<LoadMesPageViewModel>(SnackbarService);
        // 启用监听

        IsActive = true;
    }

    [RelayCommand]
    public void AddHttpButton()
    {
        LoadMesAddWindow addWindow = new LoadMesAddWindow("添加HTTP请求");
        bool? b = addWindow.ShowDialog();
        if (b == true)
        {
            log.SuccessAndShow("添加MES成功");
        }
    }

    [RelayCommand]
    public void UpdateHttpButton(LoadMesPage page)
    {
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;
        LoadMesAddWindow addWindow = new LoadMesAddWindow("更新HTTP请求", item);
        bool? b = addWindow.ShowDialog();
        if (b == true)
        {
            log.SuccessAndShow($"更新MES成功 name:{item.Name}");
        }
    }

    [RelayCommand]
    public void DeleteHttpButton(LoadMesPage page)
    {
        //获取当前行
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;
        if (item == null)
        {
            log.WarningAndShow("没有数据不需要删除", "用户在操作删除,但HTTP数据已删除完");
            return;
        }
        MessageBox messageBox = new MessageBox("删除此条Http");
        bool? boxResult = messageBox.ShowDialog();
        if (boxResult == true)
        {
            string name = item.Name;
            LoadMesPageModel.MesPojoList.Remove(item);
            log.SuccessAndShow($"删除HTTP成功 name:{name}");
        }
    }
    [RelayCommand]
    public void JogHttpButton(LoadMesPage page)
    {
        LoadMesServer loadMesServer = new LoadMesServer(LoadMesPageModel.MesPojoList);
        bool succeed = loadMesServer.runJog();



        if (succeed)
        {
            log.SuccessAndShow("手动发送HTTP成功");
        }
    }

    #region SnackBar弹窗

    public void setSnackbarService(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion

    /// <summary>
    /// 接受消息处理
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Receive(MesMessage message)
    {
        LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel = message.Value;
        LoadMesPageModel.MesPojoList.Add(loadMesAddAndUpdateWindowModel);
        log.Info($"添加一行HTTP请求: Name:{loadMesAddAndUpdateWindowModel.Name} 请求方式:{loadMesAddAndUpdateWindowModel.Ajax} 请求路径:{loadMesAddAndUpdateWindowModel.HttpPath}" +
                 $"请求消息体:{loadMesAddAndUpdateWindowModel.Request} 请求条件{loadMesAddAndUpdateWindowModel.ToString()}");
    }
}