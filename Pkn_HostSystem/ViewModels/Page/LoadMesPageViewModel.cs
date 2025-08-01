using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NModbus.Logging;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Core.Interface;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Service.LoadMes;
using Pkn_HostSystem.Service.LoadMes.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Subjects;
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
    // public ILoadMesService LoadMesService { get; set; }

    public LoadMesPageViewModel()
    {
        LoadMesPageModel = JsonTool<LoadMesPageModel>.Load();
        if (LoadMesPageModel == null)
        {
            GlobalManager.GlobalDictionary.TryGetValue("MesLogListBox", out object value);
            LoadMesPageModel = new LoadMesPageModel()
            {
                MesPojoList = [], ReturnMessageList = (ObservableCollection<string>)value
            };
        }
        else
        {
            GlobalManager.GlobalDictionary["MesLogListBox"] = LoadMesPageModel.ReturnMessageList;
        }

        SnackbarService = new SnackbarService();
        Log = new LogBase<LoadMesPageViewModel>(SnackbarService);
        //创建LoadMesService服务
        // LoadMesService = new LoadMesService(LoadMesPageModel.MesPojoList);
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

        if (item.RunCyc == true)
        {
            Log.WarningAndShowTask("请停止后修改");
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
        //1. 选中当前行数据
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;

        if (item == null)
        {
            Log.WarningAndShowTask("没有选中行");
            return;
        }

        if (item.RunCyc == true)
        {
            Log.WarningAndShowTask("请停止后手动");
            return;
        }


        //初始化
        InitRun(item);

        //保存堆栈信息
        TraceContext.Name = item.Name;
        TraceContext.Param = new Dictionary<string, dynamic>();

        //进行一次数据组装
        (bool succeed, string? message) = await ExecutionCondition(item);
        if (succeed)
        {
            Log.SuccessAndShow($"[{TraceContext.Name}]--手动执行成功 ,返回: {message}");
        }
        else
        {
            Log.ErrorAndShowTask($"[{TraceContext.Name}]--手动执行失败,返回: {message}");
        }

        //清空
        TraceContext.Name = null;
    }

    [RelayCommand]
    public void RunHttpCyc(LoadMesPage page)
    {
        //1. 选中当前行数据
        LoadMesAddAndUpdateWindowModel? item = page.DataGrid.SelectedItem as LoadMesAddAndUpdateWindowModel;

        TraceContext.Name = item.Name;
        TraceContext.Param = new Dictionary<string, dynamic>();
        //2. 判断是否循环触发,还是通讯触发的方式
        IsRun(item);
        TraceContext.Name = null;
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
                case "通讯触发":
                    TriggerCyc(item);
                    break;
                case "内部触发":
                    TriggerCyc(item);
                    break;
            }
        }
        //3. 停止任务逻辑
        else
        {
            //停止
            item.cts.Cancel();
            item.Task = new Lazy<Task>(() => RunHttpCyc(item));
            item.Task = new Lazy<Task>(() => RunTrigger(item));
            Log.Info($"[{TraceContext.Name}]--任务已关闭");
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
        //需要再取消一下之前的,保险措施
        if (item.cts != null)
        {
            item.cts.Cancel();
        }

        //初始化
        InitRun(item);
        item.Task = new Lazy<Task>(() => RunHttpCyc(item));

        //运行
        Task task = item.Task.Value;
        Log.Info($"[{TraceContext.Name}]--循环型,任务已开启");
    }

    public async Task RunHttpCyc(LoadMesAddAndUpdateWindowModel model)
    {
        try
        {
            while (!model.cts.Token.IsCancellationRequested)
            {
                //进行一次数据组装
                await ExecutionCondition(model);
                await Task.Delay(model.CycTime * 1000, model.cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            Log.Info($"[{TraceContext.Name}]--触发任务被取消");
        }
        catch (Exception ex)
        {
            Log.Error($"[{TraceContext.Name}]--触发任务出现异常: {ex}");
        }
        finally
        {
            model.RunCyc = false;
            Log.Info($"[{TraceContext.Name}]--退出循环触发");
        }
    }

    #endregion

    #region 触发和循环共同的代码

    public async Task<(bool succeed, string? message)> ExecutionCondition(LoadMesAddAndUpdateWindowModel model)
    {
        Log.Info($"[{TraceContext.Name}]--开始执行触发和循环共同的代码");
        //从MesServer中取出绑定好的item
        // LoadMesAddAndUpdateWindowModel item = LoadMesService.SelectByName(model.Name);
        //维护一个集合,用于判断动态嵌入HTTP请求不会循环嵌套;
        // model.UseHttpList = new List<string>();
        // model.UseHttpList.Add(model.Name);

        Log.Info($"[{TraceContext.Name}]--消息体准备组装");
        //消息体打包
        var (succeed, request) = await model.LoadMesService.PackRequest(model.Name, model.cts);
        if (!succeed)
        {
            //消息体组装失败
            Log.Error($"[{TraceContext.Name}]--消息体组装完成,返回组装失败");
            return (succeed, request);
        }

        Log.Info($"[{TraceContext.Name}]--组装完成,返回request  \r\n {request}");
        //判断一下是否需要发送Http

        string? response = null;
        if (model.HttpNeed)
        {
            //发送Http请求
            (bool succeed2, response) = await model.LoadMesService.RunOne(model.Name, request, model.cts);
            if (!succeed2)
            {
                //消息体发送失败
                Log.Error($"[{TraceContext.Name}]--Http请求发送,返回失败");
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

        response ??= request; //如果response 为null 则赋值为request
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
        //获得网络
        var netWork = GlobalManager.GetNetWork(forwardingName);
        if (netWork == null)
        {
            Log.Error($"[{TraceContext.Name}]--转发时,无法获取到网络");
            return (false, null);
        }

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

    #region 通讯触发

    /// <summary>
    /// 触发型
    /// </summary>
    public void TriggerCyc(LoadMesAddAndUpdateWindowModel item)
    {
        //需要再取消一下之前的,保险措施
        if (item.cts != null)
        {
            item.cts.Cancel();
        }

        //初始化
        InitRun(item);

        switch (item?.TriggerType)
        {
            case "通讯触发":
                item.Task = new Lazy<Task>(() => RunTrigger(item));
                break;
            case "内部触发":
                item.Task = new Lazy<Task>(() => RunInteriorTrigger(item));
                break;
        }

        //运行
        Task task = item.Task.Value;
        Log.Info($"[{TraceContext.Name}]--触发型,任务启动成功");
    }

    public async Task RunTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        Log.Info($"[{TraceContext.Name}]-- 进入循环触发");
        //1.启动后通讯触发循环
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
                        var lookup = GlobalManager.NetWorkDictionary.Lookup(detailed.Id);
                        if (lookup.HasValue == false)
                        {
                            Log.Error($"[{TraceContext.Name}]--无触发通讯对象,请检查通讯对象是否打开");
                            await Task.Delay(500, model.cts.Token);
                        }
                        else
                        {
                            netWork = lookup.Value;
                        }
                    }
                }

                //判断是什么通讯
                if (netWork != null)
                {
                    switch (netWork.NetworkDetailed.NetMethod)
                    {
                        case "ModbusTcp":
                            //获取触发位
                            string currentMessage1 = await ModbusTrigger(model);
                            //判断是否触发
                            if (IsTrigger(model.TriggerMessage, currentMessage1))
                            {
                                Log.Info($"[{TraceContext.Name}]--ModbusTcp已被触发");
                                (bool succeed, string? message) = await ExecutionCondition(model);
                                //完成后给触发位停止
                                if (succeed)
                                {
                                    //需要进行内部触发
                                    if (model.NeedInteriorTrigger)
                                    {
                                        //进行寄存器触发
                                        Volatile.Write(ref GlobalManager.Register[model.NeedInteriorTriggerIndex], 1);

                                        //等待寄存器响应
                                        succeed = await Task.Run(async () =>
                                        {
                                            var startTime = Environment.TickCount; // 记录开始时间
                                            while (!model.cts.Token.IsCancellationRequested)
                                            {
                                                var endTime = Environment.TickCount - startTime;

                                                if (endTime > model.NeedInteriorTriggerTimeOut * 1000)
                                                {
                                                    Log.Error(
                                                        $"[{TraceContext.Name}]--等待内部触发返回超时,请检查内部触发对象是否运行,或是否响应时间超出设置时间");
                                                    break;
                                                }

                                                if (Volatile.Read(
                                                        ref GlobalManager.Register[model.NeedInteriorTriggerIndex]) ==
                                                    2)
                                                {
                                                    return true;
                                                }

                                                if (Volatile.Read(
                                                        ref GlobalManager.Register[model.NeedInteriorTriggerIndex]) ==
                                                    3)
                                                {
                                                    return false;
                                                }

                                                await Task.Delay(100);
                                            }

                                            return false;
                                        });
                                        if (succeed)
                                        {
                                            await ModbusTriggerWrite(model, true);
                                        }
                                        else
                                        {
                                            await ModbusTriggerWrite(model, false);
                                        }
                                    }
                                    else
                                    {
                                        await ModbusTriggerWrite(model, true);
                                    }
                                }
                                else
                                {
                                    await ModbusTriggerWrite(model, false);
                                }
                            }

                            break;
                        case "ModbusRtu":
                            //获取触发位
                            string currentMessage2 = await ModbusTrigger(model);
                            //判断是否触发
                            if (IsTrigger(model.TriggerMessage, currentMessage2))
                            {
                                Log.Info($"[{TraceContext.Name}]--ModbusRtu已被触发");
                                (bool succeed, string? message) = await ExecutionCondition(model);
                                //完成后给触发位停止
                                if (succeed)
                                {
                                    //需要进行内部触发
                                    if (model.NeedInteriorTrigger)
                                    {
                                    }

                                    await ModbusTriggerWrite(model, true);
                                }
                                else
                                {
                                    await ModbusTriggerWrite(model, false);
                                }
                            }

                            break;
                        case "基恩士上位链路通讯":
                            //获取触发位
                            //判断是否触发
                            if (IsTrigger(model.TriggerMessage, await KeyenceHostLinkTrigger(model)))
                            {
                                Log.Info($"[{TraceContext.Name}]--基恩士上位链路通讯已被触发");
                                (bool succeed, string? message) = await ExecutionCondition(model);
                                //完成后给触发位停止
                                if (succeed)
                                {
                                    //需要进行内部触发
                                    if (model.NeedInteriorTrigger)
                                    {
                                        //进行寄存器触发
                                        Volatile.Write(ref GlobalManager.Register[model.NeedInteriorTriggerIndex], 1);

                                        //等待寄存器响应
                                        succeed = await Task.Run(async () =>
                                        {
                                            var startTime = Environment.TickCount; // 记录开始时间
                                            while (!model.cts.Token.IsCancellationRequested)
                                            {
                                                var endTime = Environment.TickCount - startTime;

                                                if (endTime > model.NeedInteriorTriggerTimeOut * 1000)
                                                {
                                                    Log.Error(
                                                        $"[{TraceContext.Name}]--等待内部触发返回超时,请检查内部触发对象是否运行,或是否响应时间超出设置时间");
                                                    break;
                                                }

                                                if (Volatile.Read(
                                                        ref GlobalManager.Register[model.NeedInteriorTriggerIndex]) ==
                                                    2)
                                                {
                                                    return true;
                                                }

                                                if (Volatile.Read(
                                                        ref GlobalManager.Register[model.NeedInteriorTriggerIndex]) ==
                                                    3)
                                                {
                                                    return false;
                                                }

                                                await Task.Delay(100);
                                            }

                                            return false;
                                        });
                                        if (succeed)
                                        {
                                            await ModbusTriggerWrite(model, true);
                                        }
                                        else
                                        {
                                            await ModbusTriggerWrite(model, false);
                                        }
                                    }
                                    else
                                    {
                                        await KeyenceHostLinkTriggerWrite(model, true);
                                    }
                                }
                                else
                                {
                                    await KeyenceHostLinkTriggerWrite(model, false);
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
            Log.Info($"[{TraceContext.Name}]--触发任务被取消");
        }
        catch (Exception ex)
        {
            Log.Error($"[{TraceContext.Name}]--触发任务出现异常: {ex}");
        }
        finally
        {
            model.RunCyc = false;
            Log.Info($"[{TraceContext.Name}]--退出循环触发");
        }
    }

    /// <summary>
    /// ModbusTcp触发
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private async Task<string> ModbusTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        //获取当前通讯对象
        LoadMesAddAndUpdateWindowModel item = model.LoadMesService.SelectByName(model.Name);
        var netWork = GlobalManager.GetNetWork(item.TriggerConnectName);
        if (netWork == null)
        {
            Log.Error($"[{TraceContext.Name}]--循环读取ModbusTcp触发,GetNetWork未找到连接");
            return string.Empty;
        }

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
            Log.Error($"[{TraceContext.Name}]--循环读取:触发寄存器:{model.StartAddress},发送错误 :{e}");
            return string.Empty;
        }

        return readHoldingRegisters03[0].ToString();
    }


    private async Task<bool> ModbusTriggerWrite(LoadMesAddAndUpdateWindowModel model, bool succeed)
    {
        try
        {
            //获取当前通讯对象
            LoadMesAddAndUpdateWindowModel item = model.LoadMesService.SelectByName(model.Name);
            var netWork = GlobalManager.GetNetWork(item.TriggerConnectName);
            if (netWork == null)
            {
                Log.Error($"[{TraceContext.Name}]--触发型Modbus写回时,GetNetWork获取不到网络");
                return false;
            }

            //获得ModBase对象
            ModbusBase modbusBase = netWork.ModbusBase;
            if (succeed)
            {
                Log.Info($"[{TraceContext.Name}]--modbus触发 返回成功触发消息:{model.SuccessResponseMessage}");
                await modbusBase.WriteRegister_06(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                    ushort.Parse(model.SuccessResponseMessage));
            }
            else
            {
                Log.Error($"[{TraceContext.Name}]--modbus触发 返回失败触发消息:{model.FailResponseMessage}");
                await modbusBase.WriteRegister_06(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                    ushort.Parse(model.FailResponseMessage));
            }
        }
        catch (Exception e)
        {
            Log.Error($"[{TraceContext.Name}]--触发型Modbus写回失败 ,{e}");
            return false;
        }

        return true;
    }


    private async Task<string> KeyenceHostLinkTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        //获取当前通讯对象
        LoadMesAddAndUpdateWindowModel item = model.LoadMesService.SelectByName(model.Name);
        var netWork = GlobalManager.GetNetWork(item.TriggerConnectName);
        if (netWork == null)
        {
            Log.Error($"[{TraceContext.Name}]--循环读取ModbusTcp触发,GetNetWork未找到连接");
            return string.Empty;
        }

        //获取基恩士对象
        KeyenceHostLinkTool netWorkKeyenceHostLinkTool = netWork.KeyenceHostLinkTool;

        (bool item1, ushort item2) =
            await netWorkKeyenceHostLinkTool.ReadDM<ushort>(int.Parse(model.StartAddress), model.cts,true);

        if (!item1)
        {
            Log.Error($"[{TraceContext.Name}]--循环读取:触发寄存器DM:{model.StartAddress} 失败");
            return string.Empty;
        }

        return item2.ToString();
    }


    private async Task<bool> KeyenceHostLinkTriggerWrite(LoadMesAddAndUpdateWindowModel model, bool succeed)
    {
        try
        {
            //获取当前通讯对象
            LoadMesAddAndUpdateWindowModel item = model.LoadMesService.SelectByName(model.Name);
            var netWork = GlobalManager.GetNetWork(item.TriggerConnectName);
            if (netWork == null)
            {
                Log.Error($"[{TraceContext.Name}]--触发型Modbus写回时,GetNetWork获取不到网络");
                return false;
            }

            //获取基恩士对象
            KeyenceHostLinkTool keyenceHostLinkTool = netWork.KeyenceHostLinkTool;
            if (succeed)
            {
                Log.Info($"[{TraceContext.Name}]--KeyenceHostLink触发 返回成功触发消息:{model.SuccessResponseMessage}");
                return await keyenceHostLinkTool.WriteDM<ushort>(int.Parse(model.StationAddress),
                    ushort.Parse(model.SuccessResponseMessage), model.cts);
            }
            else
            {
                Log.Error($"[{TraceContext.Name}]--KeyenceHostLink触发 返回失败触发消息:{model.FailResponseMessage}");
                return await keyenceHostLinkTool.WriteDM<ushort>(int.Parse(model.StationAddress),
                    ushort.Parse(model.FailResponseMessage), model.cts);
            }
        }
        catch (Exception e)
        {
            Log.Error($"[{TraceContext.Name}]--KeyenceHostLink触发写回失败 ,{e}");
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

    #region 内部触发

    public async Task RunInteriorTrigger(LoadMesAddAndUpdateWindowModel model)
    {
        Log.Info($"[{TraceContext.Name}]-- 进入内部循环触发");
        //1.启动后通讯触发循环
        try
        {
            while (!model.cts.Token.IsCancellationRequested)
            {
                //获取触发位


                int i = Volatile.Read(ref GlobalManager.Register[model.InteriorArrayIndex]);

                //判断是否触发
                if (IsTrigger("1", i.ToString()))
                {
                    Log.Info($"[{TraceContext.Name}]--内部寄存器{model.InteriorArrayIndex}已被触发");
                    (bool succeed, string? message) = await ExecutionCondition(model);
                    //完成后给触发位停止
                    if (succeed)
                    {
                        Volatile.Write(ref GlobalManager.Register[model.InteriorArrayIndex], 2);
                    }
                    else
                    {
                        Volatile.Write(ref GlobalManager.Register[model.InteriorArrayIndex], 3);
                    }
                }


                await Task.Delay(model.CycTime, model.cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            Log.Info($"[{TraceContext.Name}]--触发任务被取消");
        }
        catch (Exception ex)
        {
            Log.Error($"[{TraceContext.Name}]--触发任务出现异常: {ex}");
        }
        finally
        {
            model.RunCyc = false;
            Log.Info($"[{TraceContext.Name}]--退出循环触发");
        }
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
        json = JsonTool<object>.TryFormatJson(json, out bool isJson);
        csvHelper.AddRowFromJson(json);
        csvHelper.Save(model.cts.Token);
        Log.Info($"[{TraceContext.Name}] --本地保存{model.Name}{lastName}.csv  成功");
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
        JsonTool<LoadMesPageModel>.Save(LoadMesPageModel);
    }

    #endregion


    #region private方法

    /// <summary>
    /// 主要初始化 令牌 和 LoadMesService
    /// </summary>
    /// <param name="item"></param>
    private void InitRun(LoadMesAddAndUpdateWindowModel item)
    {
        //1. 初始化令牌
        item.cts = new CancellationTokenSource();
        //2. LoadMesService 初始化
        if (item.NeedStationLog)
        {
            if (item.Station == null)
            {
                item.LoadMesService = new LoadMesService(LoadMesPageModel.MesPojoList);
                return;
            }

            bool hasValue = GlobalManager.StationDictionary.Lookup(item.Station).HasValue;
            if (hasValue)
            {
                var eachStation = GlobalManager.StationDictionary.Lookup(item.Station).Value;
                //通过委托创建出特别的 LoadMesService
                //每个站点的LoadMesService都不一样,所以需要通过委托来创建
                item.LoadMesService = eachStation.CreateDecoratorFunc(new LoadMesService(LoadMesPageModel.MesPojoList));
            }
            else
            {
                item.LoadMesService = new LoadMesService(LoadMesPageModel.MesPojoList);
                return;
            }
        }
        else
        {
            item.LoadMesService = new LoadMesService(LoadMesPageModel.MesPojoList);
        }
    }

    #endregion
}