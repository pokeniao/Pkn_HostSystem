using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Message;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Server.LoadMes;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBox = Pkn_HostSystem.Views.Windows.MessageBox;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class LoadMesPageViewModel : ObservableRecipient, IRecipient<AddOneMesMessage>
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
                MesPojoList = [], ReturnMessageList = (ObservableCollection<string>)value
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


    #region 添加一行,删除一行,修改一行Http

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

    #endregion

    #region 手动触发发送 与 开启Http

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
        //1. 选中当前行数据
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;

        //2. 判断是否循环触发,还是消息触发的方式
        IsRun(item);
    }

    public void IsRun(LoadMesAddAndUpdateWindowModel item)
    {
        if (item.RunCyc)
        {
            switch (item?.TriggerType)
            {
                case "循环触发":
                    OpenCyc(item);
                    break;
                case "消息触发":
                    TriggerCyc(item);
                    break;
            }
        }
        //3. 停止任务逻辑
        else
        {
            //停止
            item.cts.Cancel();
            item.cts = new CancellationTokenSource();
            item.Task = new Lazy<Task>(() => RunHttpCyc(item));
            item.Task = new Lazy<Task>(() => RunTrigger(item));
        }
    }

    #endregion

    #region 循环触发

    /// <summary>
    /// 启动循环的方法
    /// </summary>
    /// <param name="item"></param>
    public void OpenCyc(LoadMesAddAndUpdateWindowModel item)
    {
        item.cts = new CancellationTokenSource();
        item.Task = new Lazy<Task>(() => RunHttpCyc(item));

        //运行
        Task task = item.Task.Value;
    }

    public async Task RunHttpCyc(LoadMesAddAndUpdateWindowModel model)
    {
        while (!model.cts.Token.IsCancellationRequested)
        {

            //判断一下是否需要发送Http
            if (model.HttpNeed)
            {
                //发送Http请求
                bool succeed = await loadMesServer.RunOne(model.Name);
            }
            //进行一次数据组装
            //从MesServer中取出绑定好的item
            LoadMesAddAndUpdateWindowModel item = loadMesServer.SelectByName(model.Name);
            //消息体打包
            var request = await loadMesServer.PackRequest(item.Name);
            //判断是否需要本地保存
            if (model.LocalSave)
            {
                //打包后本地保存
                LocalSave(model, request);
            }
            //判断一下是否需要转发
            if (model.TranspondNeed)
            {
                
            }


            await Task.Delay(model.CycTime * 1000, model.cts.Token);
        }
    }

    #endregion

    #region 消息触发

    /// <summary>
    /// 触发型
    /// </summary>
    public void TriggerCyc(LoadMesAddAndUpdateWindowModel item)
    {
        item.cts = new CancellationTokenSource();
        item.Task = new Lazy<Task>(() => RunTrigger(item));

        //运行
        Task task = item.Task.Value;
    }

    public async Task RunTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        //1.启动后消息触发循环
        while (!model.cts.Token.IsCancellationRequested)
        {
            //2. 判读更具什么进行的通讯
            switch (model.NetTrigger)
            {
                case "ModbusTcp":
                    //获取触发位
                    string currentMessage1 = await ModbusTcpTrigger(model);
                    //判断是否触发
                    if (IsTrigger(model.TriggerMessage, currentMessage1))
                    {
                        bool succeed =false;
                        LoadMesAddAndUpdateWindowModel item = loadMesServer.SelectByName(model.Name); ;
                        string request = await loadMesServer.PackRequest(item.Name); ;

                        if (model.HttpNeed)
                        {
                            //手动发送Http请求
                             succeed = await loadMesServer.RunOne(model.Name);
                        }
                        else
                        {
                            log.Info($"{model.Name}: {request}" );
                            //触发停止需求
                            succeed = true;
                        }
                            
                        //判断是否需要本地保存
                        if (model.LocalSave)
                        {
                            //打包后本地保存
                            LocalSave(model, request);
                        }

                        //完成后给触发位停止
                        if (succeed)
                        {
                            await ModbusTcpTriggerWrite(model, true);
                        }
                        else
                        {
                            await ModbusTcpTriggerWrite(model, false);
                        }
                    }

                    break;
                case "ModbusRtu":
                    //获取触发位
                    string currentMessage2 = await ModbusTcpTrigger(model);
                    //判断是否触发
                    if (IsTrigger(model.TriggerMessage, currentMessage2))
                    {
                        //手动发送Http请求
                        bool succeed = await loadMesServer.RunOne(model.Name);
                        //判断是否需要本地保存
                        if (model.LocalSave)
                        {
                            //从MesServer中取出绑定好的item
                            LoadMesAddAndUpdateWindowModel item = loadMesServer.SelectByName(model.Name);
                            //消息体打包
                            var request = await loadMesServer.PackRequest(item.Name);
                            //打包后本地保存
                            LocalSave(model, request);
                        }

                        //完成后给触发位停止
                        if (succeed)
                        {
                            await ModbusTcpTriggerWrite(model, true);
                        }
                        else
                        {
                            await ModbusTcpTriggerWrite(model, false);
                        }
                    }

                    break;
                case "Socket":

                    break;
            }

            await Task.Delay(model.CycTime, model.cts.Token);
        }
    }

    private async Task<string> ModbusTcpTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        //获取当前通讯对象
        LoadMesAddAndUpdateWindowModel item = loadMesServer.SelectByName(model.Name);
        string key = loadMesServer.getNetKey(item.TriggerConnectName);
        var netWork = GlobalMannager.NetWorkDictionary.Lookup(key).Value;
        //获得ModBase对象
        ModbusBase modbusBase = netWork.ModbusBase;

        //读取寄存器
        ushort[] readHoldingRegisters03 = await modbusBase.ReadHoldingRegisters_03(
            byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
            1);

        return readHoldingRegisters03[0].ToString();
    }

    private async Task<bool> ModbusTcpTriggerWrite(LoadMesAddAndUpdateWindowModel model, bool succeed)
    {
        try
        {
            //获取当前通讯对象
            LoadMesAddAndUpdateWindowModel item = loadMesServer.SelectByName(model.Name);
            string key = loadMesServer.getNetKey(item.TriggerConnectName);
            var netWork = GlobalMannager.NetWorkDictionary.Lookup(key).Value;
            //获得ModBase对象
            ModbusBase modbusBase = netWork.ModbusBase;
            if (succeed)
            {
                await modbusBase.WriteRegister_06(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress), 2);
            }
            else
            {
                await modbusBase.WriteRegister_06(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress), 3);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public bool IsTrigger(string triggerMessage, string currentMessage)
    {
        if (triggerMessage == currentMessage)
            return true;
        return false;
    }

    #endregion

    #region 本地保存当前发送Mes的记录

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

    #endregion

    #region SnackBar弹窗

    public void setSnackbarService(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion

    #region MVVM页面消息通讯

    /// <summary>
    /// 接受消息处理
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Receive(AddOneMesMessage message)
    {
        LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel = message.Value;
        LoadMesPageModel.MesPojoList.Add(loadMesAddAndUpdateWindowModel);
        log.Info(
            $"添加一行HTTP请求: Name:{loadMesAddAndUpdateWindowModel.Name} 请求方式:{loadMesAddAndUpdateWindowModel.Ajax} 请求路径:{loadMesAddAndUpdateWindowModel.HttpPath}" +
            $"请求消息体:{loadMesAddAndUpdateWindowModel.Request} 请求条件{loadMesAddAndUpdateWindowModel.ToString()}");
    }

    #endregion

    #region 保存当前Model

    [RelayCommand]
    public void Save()
    {
        AppJsonStorage<LoadMesPageModel>.Save(LoadMesPageModel);
    }

    #endregion
}