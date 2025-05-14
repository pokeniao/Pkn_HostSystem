using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Message;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Server.LoadMes;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Documents;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBox = Pkn_HostSystem.Views.Windows.MessageBox;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class LoadMesPageViewModel : ObservableRecipient, IRecipient<AddOneMesMessage>
{
    public LoadMesPageModel LoadMesPageModel { get; set; }

    public SnackbarService SnackbarService { get; set; }


    public LogBase<LoadMesPageViewModel> Log;

    //手动发送Http请求
    public LoadMesService LoadMesService { get; set; }

    public LoadMesPageViewModel()
    {
        LoadMesPageModel = AppJsonTool<LoadMesPageModel>.Load();
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
        Log = new LogBase<LoadMesPageViewModel>(SnackbarService);

        LoadMesService = new LoadMesService(LoadMesPageModel.MesPojoList);
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
            Log.SuccessAndShow("添加MES成功");
        }
    }

    [RelayCommand]
    public void UpdateHttpButton(LoadMesPage page)
    {
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;

        if (item == null)
        {
            Log.WarningAndShow("没有选中行", "当前HTTP列表没有数据,用户点击更新操作");
            return;
        }

        LoadMesAddWindow addWindow = new LoadMesAddWindow("更新HTTP请求", item, LoadMesPageModel.MesPojoList);
        bool? b = addWindow.ShowDialog();
        if (b == true)
        {
            Log.SuccessAndShow($"更新MES成功 name:{item.Name}");
        }
    }

    [RelayCommand]
    public void DeleteHttpButton(LoadMesPage page)
    {
        //获取当前行
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;
        if (item == null)
        {
            Log.WarningAndShow("没有数据不需要删除", "用户在操作删除,但HTTP数据已删除完");
            return;
        }

        if (item.RunCyc)
        {
            Log.WarningAndShow("删除前请停止运行", $"用户在操作删除,请先停止运行{item.Name}");
            return;
        }

        MessageBox messageBox = new MessageBox("删除此条Http");
        bool? boxResult = messageBox.ShowDialog();
        if (boxResult == true)
        {
            string name = item.Name;
            LoadMesPageModel.MesPojoList.Remove(item);
            Log.SuccessAndShow($"删除HTTP成功 name:{name}");
        }
    }

    #endregion

    #region 手动触发发送 与 开启Http

    [RelayCommand]
    public async Task JogHttpButton(LoadMesPage page)
    {
        bool succeed = await LoadMesService.RunAll();

        if (succeed)
        {
            Log.SuccessAndShow("手动发送HTTP成功");
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
            Log.Info($"{nameof(LoadMesPageViewModel)}--{item.Name}--任意类型,任务已关闭");
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
        Log.Info($"{nameof(LoadMesPageViewModel)}--{item.Name}--循环型,任务已开启");
    }

    public async Task RunHttpCyc(LoadMesAddAndUpdateWindowModel model)
    {
        while (!model.cts.Token.IsCancellationRequested)
        {
            //进行一次数据组装
            await ExecutionCondition(model);
            await Task.Delay(model.CycTime * 1000, model.cts.Token);
        }
    }

    #endregion

    #region 触发和循环共同的代码

    public async Task<(bool succeed, string? message)> ExecutionCondition(LoadMesAddAndUpdateWindowModel model)
    {
        Log.Info($"{model.Name} 开始执行ExecutionCondition");
        //从MesServer中取出绑定好的item
        LoadMesAddAndUpdateWindowModel item = LoadMesService.SelectByName(model.Name);
        Log.Info($"{model.Name}--消息体--准备组装");
        //消息体打包
        var (succeed, request) = await LoadMesService.PackRequest(item.Name, model.cts);
        if (!succeed)
        {
            //消息体组装失败
            Log.Error($"{model.Name}消息体--组装完成--返回组装失败");
            return (succeed, request);
        }

        Log.Info($"{model.Name}消息体--组装完成--返回request  \r\n {request}");
        //判断一下是否需要发送Http

        string? response = null;
        if (model.HttpNeed)
        {
            //发送Http请求
            (bool succeed2, response) = await LoadMesService.RunOne(model.Name, model.cts);
            if (!succeed2)
            {
                //消息体发送失败
                Log.Error($"{model.Name}--Http请求发送,返回失败");
                return (succeed2, response);
            }
        }

        //判断是否需要本地保存,
        if (model.LocalSave)
        {
            //打包后本地保存
            LocalSave(model, request);
            //如果是Http请求就需要进行对结果保存
            if (model.HttpNeed)
            {
                LocalSave(model, response, "-response");
            }
        }

        //判断一下是否需要转发
        if (model.TranspondNeed)
        {
            Transpond(model, response);
        }

        return (true, response);
    }

    #endregion

    #region 消息转发

    /// <summary>
    /// 转发
    /// </summary>
    /// <param name="model"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string message)> Transpond(LoadMesAddAndUpdateWindowModel model, string response)
    {
        //通过名字搜索id
        string forwardingName = model.ForwardingName;
        //获得网络名
        string netKey = LoadMesService.getNetKey(forwardingName);
        //获得网络
        var netWork = GlobalMannager.NetWorkDictionary.Lookup(netKey).Value;

        //判断当前转发的通讯是什么类型的
        string networkDetailedNetMethod = netWork.NetworkDetailed.NetMethod;
        switch (networkDetailedNetMethod)
        {
            case "ModbusTcp":
                ModbusBase modbusBase = netWork.ModbusBase;
                List<ushort> list = new List<ushort>();
                try
                {
                    for (int i = 0; i < response.Length; i += 2)
                    {
                        char high = response[i];
                        char low = (i + 1 < response.Length) ? response[i + 1] : '\0'; // 补0
                        ushort packed = (ushort)((high << 8) | low);
                        list.Add(packed);
                    }

                    ushort[] result = list.ToArray();

                    await modbusBase.WriteRegisters_10(byte.Parse(model.StationAddress),
                        ushort.Parse(model.StartAddress), result);
                }
                catch (Exception e)
                {
                    return (false, null);
                }

                break;
        }

        return (true, null);
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
        Log.Info($"{nameof(LoadMesPageViewModel)}--{item.Name}--触发型,任务启动成功");
    }

    public async Task RunTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        Log.Info($"{nameof(LoadMesPageViewModel)}--{model.Name} -- 进入循环触发");
        //1.启动后消息触发循环
        try
        {
            while (!model.cts.Token.IsCancellationRequested)
            {
                //2. 判读更具什么进行的通讯
                string modelTriggerConnectName = model.TriggerConnectName;
                HomePageViewModel homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                ObservableCollection<NetworkDetailed> networkDetaileds = homePageViewModel.HomePageModel.SetConnectDg;

                NetWork netWork = null;
                foreach (var detailed in networkDetaileds)
                {
                    if (detailed.Name == modelTriggerConnectName)
                    {
                        var lookup = GlobalMannager.NetWorkDictionary.Lookup(detailed.Id);
                        if (lookup.HasValue == false)
                        {
                            Log.Info($"{nameof(LoadMesPageViewModel)}--{model.Name}--没有触发的通讯对象,请检查通讯对象是否打开");
                        }
                        else
                        {
                            netWork = lookup.Value;
                        }
                    }
                }

                if (netWork != null)
                {
                    switch (netWork.NetworkDetailed.NetMethod)
                    {
                        case "ModbusTcp":
                            //获取触发位
                            string currentMessage1 = await ModbusTcpTrigger(model);
                            //判断是否触发
                            if (IsTrigger(model.TriggerMessage, currentMessage1))
                            {
                                Log.Info($"{model.Name} modbusTcp触发,已被触发");
                                (bool succeed, string? message) = await ExecutionCondition(model);
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
                                Log.Info($"{model.Name} 触发型modbusRtu,已被触发");
                                (bool succeed, string? message) = await ExecutionCondition(model);
                                //完成后给触发位停止
                                if (succeed)
                                {
                                    if (!await ModbusTcpTriggerWrite(model, true))
                                    {
                                        Log.Error(
                                            $"{model.Name} 触发型modbusRtu,写入成功触发消息:{model.SuccessResponseMessage}--时发生失败");
                                    }
                                }
                                else
                                {
                                    if (!await ModbusTcpTriggerWrite(model, false))
                                    {
                                        Log.Error(
                                            $"{model.Name} 触发型modbusRtu,写入失败触发消息:{model.FailResponseMessage}--时发生失败");
                                    }
                                }
                            }

                            break;
                        case "Socket":
                            break;
                    }
                }

                await Task.Delay(model.CycTime, model.cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            Log.Info($"{nameof(LoadMesPageViewModel)}--{model.Name} -- 触发任务被取消");
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(LoadMesPageViewModel)}--{model.Name} -- 触发任务出现异常: {ex}");
        }
        finally
        {
            Log.Info($"{nameof(LoadMesPageViewModel)}--{model.Name} -- 退出循环触发");
        }
    }

    private async Task<string> ModbusTcpTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        //获取当前通讯对象
        LoadMesAddAndUpdateWindowModel item = LoadMesService.SelectByName(model.Name);
        string key = LoadMesService.getNetKey(item.TriggerConnectName);
        var netWork = GlobalMannager.NetWorkDictionary.Lookup(key).Value;
        //获得ModBase对象
        ModbusBase modbusBase = netWork.ModbusBase;

        //读取寄存器
        ushort[] readHoldingRegisters03 = null;
        try
        {
            readHoldingRegisters03 = await modbusBase.ReadHoldingRegisters_03(
                byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                1);
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(LoadMesPageViewModel)}--{model.Name}--触发寄存器,在试图启动读取Modbus寄存器时失败,启动未成功");
            return string.Empty;
        }

        return readHoldingRegisters03[0].ToString();
    }

    private async Task<bool> ModbusTcpTriggerWrite(LoadMesAddAndUpdateWindowModel model, bool succeed)
    {
        try
        {
            //获取当前通讯对象
            LoadMesAddAndUpdateWindowModel item = LoadMesService.SelectByName(model.Name);
            string key = LoadMesService.getNetKey(item.TriggerConnectName);
            var netWork = GlobalMannager.NetWorkDictionary.Lookup(key).Value;
            //获得ModBase对象
            ModbusBase modbusBase = netWork.ModbusBase;
            if (succeed)
            {
                Log.Info($"{model.Name} : modbusTcp触发 返回成功触发消息:{model.SuccessResponseMessage}");
                await modbusBase.WriteRegister_06(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                    ushort.Parse(model.SuccessResponseMessage));
            }
            else
            {
                Log.Error($"{model.Name} : modbusTcp触发 返回失败触发消息:{model.FailResponseMessage}");
                await modbusBase.WriteRegister_06(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                    ushort.Parse(model.FailResponseMessage));
            }
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(LoadMesPageViewModel)}--触发型ModbusTcp写回失败 ,{e}");
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
    /// <param name="model">当前HTTP请求的数据</param>
    /// <param name="json">需要本地保存csv的Json</param>
    /// <param name="lastName">需要本地保存的lastName</param>
    public void LocalSave(LoadMesAddAndUpdateWindowModel model, string json, string lastName = "")
    {
        //不存在,创建
        if (!Directory.Exists(SaveFile))
            Directory.CreateDirectory(SaveFile);
        string FilePath = Path.Combine(SaveFile, model.Name + lastName + ".csv");
        CsvHelper csvHelper = new CsvHelper(FilePath);
        csvHelper.Load();
        csvHelper.AddRowFromJson(json);
        csvHelper.Save();
        Log.Info($"本地保存{model.Name}+{lastName}.csv  成功");
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
        Log.Info(
            $"添加一行HTTP请求: Name:{loadMesAddAndUpdateWindowModel.Name} 请求方式:{loadMesAddAndUpdateWindowModel.Ajax} 请求路径:{loadMesAddAndUpdateWindowModel.HttpPath}" +
            $"请求消息体:{loadMesAddAndUpdateWindowModel.Request} 请求条件{loadMesAddAndUpdateWindowModel.ToString()}");
    }

    #endregion

    #region 保存当前Model

    [RelayCommand]
    public void Save()
    {
        AppJsonTool<LoadMesPageModel>.Save(LoadMesPageModel);
    }

    #endregion
}