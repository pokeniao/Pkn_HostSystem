using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pkn_HostSystem.Base;
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
        LoadMesPageModel = AppJsonStorage<LoadMesPageModel>.Load();
        if (LoadMesPageModel == null)
        {
            GlobalMannager.GlobalDictionary.TryGetValue("MesLogListBox", out object value);
            LoadMesPageModel = new LoadMesPageModel()
            {
                MesPojoList = [],
                ReturnMessageList = (ObservableCollection<string>)value
            };
        }
        else
        {
            GlobalMannager.GlobalDictionary["MesLogListBox"] = LoadMesPageModel.ReturnMessageList;
        }

        SnackbarService = new SnackbarService();
        log = new LogBase<LoadMesPageViewModel>(SnackbarService);
        loadMesServer = new LoadMesServer(LoadMesPageModel.MesPojoList);
        // 启用监听
        IsActive = true;
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
            switch (item?.TriggerType)
            {
                case "循环触发":
                    OpenCyc(item);
                    break;
                case "消息触发":
                    Trigger(item);
                    break;
            }
        }
        else
        {
            //停止
            item.cts.Cancel();
            item.cts = new CancellationTokenSource();
            item.Task = new Lazy<Task>(() => RunHttpCyc(item));
        }
    }

    /// <summary>
    /// 触发型
    /// </summary>
    public void Trigger(LoadMesAddAndUpdateWindowModel item)
    {
        if (item.Task == null)
        {
            item.cts = new CancellationTokenSource();
            item.Task = new Lazy<Task>(() => TriggerCyc(item));
        }

        //运行
        Task task = item.Task.Value;
    }

    public async Task TriggerCyc(LoadMesAddAndUpdateWindowModel model)
    {
        while (!model.cts.Token.IsCancellationRequested)
        {
            //是什么的触发类型
            switch (model.NetTrigger)
            {
                case "ModbusTcp":

                    break;
                case "ModbusRtu":

                    break;
                case "Socket":

                    break;
            }

            await Task.Delay(100, model.cts.Token);
        }
    }

    /// <summary>
    /// 启动循环的方法
    /// </summary>
    /// <param name="item"></param>
    public void OpenCyc(LoadMesAddAndUpdateWindowModel item)
    {
        if (item.Task == null)
        {
            item.cts = new CancellationTokenSource();
            item.Task = new Lazy<Task>(() => RunHttpCyc(item));
        }

        //运行
        Task task = item.Task.Value;
    }

    public async Task RunHttpCyc(LoadMesAddAndUpdateWindowModel model)
    {
        while (!model.cts.Token.IsCancellationRequested)
        {
            //手动发送Http请求
            bool succeed = await loadMesServer.RunOne(model.Name);


            //判断是否需要本地保存
            if (model.LocalSave)
            {
                //获取嵌入好的内容
                LoadMesAddAndUpdateWindowModel item = loadMesServer.SelectByName(model.Name);
                var request = await loadMesServer.PackRequest(item.Name);
                LocalSave(model, request);
            }

            await Task.Delay(model.CycTime * 1000, model.cts.Token);
        }
    }

    private static readonly string AppFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Pkn_HostSystem" // 文件夹名
        );

    private static readonly string SaveFile = Path.Combine(AppFolder, "Mes上传记录");

    /// <summary>
    /// 本地保存
    /// </summary>
    public void LocalSave(LoadMesAddAndUpdateWindowModel model, string json)
    {
        //不存在,创建
        if (!Directory.Exists(SaveFile))
            Directory.CreateDirectory(SaveFile);
        string FilePath = Path.Combine(SaveFile, model.Name + ".csv");
        CsvHelper csvHelper = new CsvHelper(FilePath);
        csvHelper.Load();
        csvHelper.AddRowFromJson(json);
        csvHelper.Save();
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

    [RelayCommand]
    public void Save()
    {
        AppJsonStorage<LoadMesPageModel>.Save(LoadMesPageModel);
    }
}