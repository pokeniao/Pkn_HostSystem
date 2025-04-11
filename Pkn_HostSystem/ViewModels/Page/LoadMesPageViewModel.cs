using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Message;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Server.LoadMes;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBox = Pkn_HostSystem.Views.Windows.MessageBox;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class LoadMesPageViewModel : ObservableRecipient, IRecipient<MesMessage>
{
    public LoadMesPageModel LoadMesPageModel { get; set; }

    public LoadMesServer LoadMesServer { get; set; }
    public SnackbarService SnackbarService { get; set; }


    public LogBase<LoadMesPageViewModel> log;

    //手动发送Http请求
    private LoadMesServer loadMesServer;

    public LoadMesPageViewModel()
    {
        GlobalMannager.GlobalDictionary.TryGetValue("MesLogListBox", out object value);
        LoadMesPageModel = new LoadMesPageModel()
        {
            MesPojoList = [],
            ReturnMessageList = (ObservableCollection<string>)value
        };
        SnackbarService = new SnackbarService();
        log = new LogBase<LoadMesPageViewModel>(SnackbarService);
        // 启用监听
        IsActive = true;
        loadMesServer = new LoadMesServer(LoadMesPageModel.MesPojoList);
    }

    [RelayCommand]
    public void AddHttpButton()
    {
        LoadMesAddWindow addWindow = new LoadMesAddWindow("添加HTTP请求", LoadMesPageModel.MesPojoList);
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

        if (item == null)
        {
            log.WarningAndShow("没有选中行", "当前HTTP列表没有数据,用户点击更新操作");
            return;
        }

        LoadMesAddWindow addWindow = new LoadMesAddWindow("更新HTTP请求", item, LoadMesPageModel.MesPojoList);
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

        if (item.RunCyc)
        {
            log.WarningAndShow("删除前请停止运行", $"用户在操作删除,请先停止运行{item.Name}");
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
    public async Task JogHttpButton(LoadMesPage page)
    {
        bool succeed = await loadMesServer.RunAll();

        if (succeed)
        {
            log.SuccessAndShow("手动发送HTTP成功");
        }
    }

    [RelayCommand]
    public void RunHttpCyc(LoadMesPage page)
    {
        //选中当前行数据
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;

        if (item.RunCyc)
        {
            if (item.Task == null)
            {
                item.cts = new CancellationTokenSource();
                item.Task = new Lazy<Task>(() => RunHttpCyc(item));
            }

            //运行
            Task task = item.Task.Value;
        }
        else
        {
            //停止
            item.cts.Cancel();
            item.cts = new CancellationTokenSource();
            item.Task = new Lazy<Task>(() => RunHttpCyc(item));
        }
    }

    public async Task RunHttpCyc(LoadMesAddAndUpdateWindowModel model)
    {
        while (!model.cts.Token.IsCancellationRequested)
        {
            //手动发送Http请求
            bool succeed = await loadMesServer.RunOne(model.Name);
            await Task.Delay(model.CycTime * 1000, model.cts.Token);
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
        log.Info(
            $"添加一行HTTP请求: Name:{loadMesAddAndUpdateWindowModel.Name} 请求方式:{loadMesAddAndUpdateWindowModel.Ajax} 请求路径:{loadMesAddAndUpdateWindowModel.HttpPath}" +
            $"请求消息体:{loadMesAddAndUpdateWindowModel.Request} 请求条件{loadMesAddAndUpdateWindowModel.ToString()}");
    }
}