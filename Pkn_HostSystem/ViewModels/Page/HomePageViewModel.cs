using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using KeyenceTool;
using Newtonsoft.Json.Serialization;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class HomePageViewModel : ObservableRecipient
{
    private LogBase<HomePageViewModel> log;

    public HomePageModel HomePageModel { get; set; } = AppJsonStorage<HomePageModel>.Load();

    public ModbusToolModel ModbusToolModel { get; set; }

    public SnackbarService SnackbarService { get; set; } = new();
    public ModbusBase ModbusBase { get; set; } = new();

    public List<string> NetMethod { get; set; } = ["ModbusTcp", "ModbusRtu", "Tcp客户端", "Tcp服务器", "基恩士上位链路通讯"];

    public HomePageViewModel()
    {
        if (HomePageModel == null)
        {
            //全局的
            GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out var obj);
            HomePageModel = new HomePageModel()
            {
                LogListBox = (ObservableCollection<string>)obj,
                SetConnectDg = new ObservableCollection<NetworkDetailed>()
            };
        }
        else
        {
            GlobalMannager.GlobalDictionary["LogListBox"] = HomePageModel.LogListBox;
        }

        //初始化Model
        ModbusToolModel = new ModbusToolModel()
        {
            ModbusTcp_Ip = ModbusBase.getIpAddress().ToList(),
            ModbusTcp_Ip_select = ModbusBase.getIpAddress()[0],
            ModbusRtu_COM = ModbusBase.getCOM().ToList(),
            ModbusRtu_COM_select = ModbusBase.getCOM()[0],
            ModbusTcp_Port = int.Parse("502"),
            ModbusRtu_baudRate = new List<string>() { "9600", "14400", "19200" },
            ModbusRtu_baudRate_select = "9600",
            ModbusRtu_dataBits = new List<string>() { "8", "7" },
            ModbusRtu_dataBits_select = "8",
            ModbusRtu_stopBits = Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList(),
            ModbusRtu_parity = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList(),
            FuntionCode = new List<string>()
                { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" },
            SlaveAddress = 1,
            StartAddress = 0,
            ReadCount = 1
        };
        log = new LogBase<HomePageViewModel>(SnackbarService);
    }

    #region 弹窗SnackbarService
    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }
    #endregion

    #region 滚动到底部
    [RelayCommand]
    public void ScrollToBottom(ListBox LogListBox)
    {
        if (LogListBox.Items.Count == 0) return;

        LogListBox.ScrollIntoView(LogListBox.Items[^1]);
    }
    #endregion

    #region 连接网络
    [RelayCommand]
    public void ConnectModbus(HomePage page)
    {
        var selectedItem = page.setConnectDg.SelectedItem as NetworkDetailed;
        if (selectedItem.Open)
            StartConnectModbus(selectedItem);
        else
            StopConnectModbus(selectedItem);
    }

    public async void StartConnectModbus(NetworkDetailed networkDetailed)
    {
        var key = networkDetailed.Id;
        var Name = networkDetailed.Name;
        if (Name == null)
        {
            log.ErrorAndShow("请先填写好连接名,并且回车确认");
            return;
        }

        var lookup = GlobalMannager.NetWorkDictionary.Lookup(key);
        if (lookup.HasValue)
        {
            var netWorkPoJo = lookup.Value;
            //不存在需要创建
            if (netWorkPoJo.Task == null)
            {
                var cts = new CancellationTokenSource();
                netWorkPoJo.CancellationTokenSource = cts;
                netWorkPoJo.ModbusBase = new ModbusBase();
                netWorkPoJo.WatsonTcpTool = new WatsonTcpTool();
                netWorkPoJo.KeyenceHostLinkTool = new KeyenceHostLinkTool();

                netWorkPoJo.Task = new Lazy<Task>(() => RunAndReconnection(cts.Token, netWorkPoJo));
                //测试的
                GlobalMannager.NetWorkDictionary.AddOrUpdate(netWorkPoJo);
            }

            await netWorkPoJo.Task.Value;
        }
        else
        {
            //令牌生成
            var cts = new CancellationTokenSource();
            var modbusBase = new ModbusBase();
            var watsonTcpTool = new WatsonTcpTool();
            var keyenceHostLinkTool = new KeyenceHostLinkTool();
            var workPoJo = new NetWork()
            {
                NetWorkId = key,
                CancellationTokenSource = cts,
                ModbusBase = modbusBase,
                WatsonTcpTool = watsonTcpTool,
                NetworkDetailed = networkDetailed,
                KeyenceHostLinkTool = keyenceHostLinkTool
            };
            var lazy = new Lazy<Task>(() => RunAndReconnection(cts.Token, workPoJo));
            //创建网络连接
            workPoJo.Task = lazy;
            GlobalMannager.NetWorkDictionary.AddOrUpdate(workPoJo);
            await workPoJo.Task.Value;
        }
    }
    public void StopConnectModbus(NetworkDetailed networkDetailed)
    {
        var key = networkDetailed.Id;
        var name = networkDetailed.Name;
        if (name == null) return;

        var b = GlobalMannager.NetWorkDictionary.Lookup(key).HasValue;
        NetWork netWork;
        if (b)
            netWork = GlobalMannager.NetWorkDictionary.Lookup(key).Value;
        else
            return;


        //取出密钥
        var cts = netWork.CancellationTokenSource;

        cts?.Cancel();
        //创建新密钥
        cts = new CancellationTokenSource();

        //更新网络连接
        netWork.CancellationTokenSource = cts;
        netWork.Task = new Lazy<Task>(() => Task.Run(() => RunAndReconnection(cts.Token, netWork)));

        //更新网络体
        GlobalMannager.NetWorkDictionary.AddOrUpdate(netWork);

        if (netWork.ModbusBase.IsTCPConnect())
        {
            //停止Modbus
            netWork.ModbusBase.CloseTCP();
            log.SuccessAndShowTask($"ModbusTCP:{name}---连接断开");
        }

        if (netWork.ModbusBase.IsRTUConnect())
        {
            netWork.ModbusBase.CloseRTU();
            log.SuccessAndShowTask($"ModbusRTU:{name}---连接断开");
        }
        //停止Tcp服务器或者Tcp客户端
        if (netWork.WatsonTcpTool.IsConnected)
        {
            netWork.WatsonTcpTool.CloseClint();
            log.SuccessAndShowTask($"TcpClint:{name}---连接断开");
        }

        if (netWork.WatsonTcpTool.IsServerRunning)
        {
            netWork.WatsonTcpTool.StopServer();
            log.SuccessAndShowTask($"TcpServer:{name}---连接断开");
        }
        //停止
        if (netWork.KeyenceHostLinkTool.IsConnected)
        {
            netWork.KeyenceHostLinkTool.Disconnect();
            log.SuccessAndShowTask($"上位链路通讯:{name}---连接断开");
        }
    }

    public async Task RunAndReconnection(CancellationToken token, NetWork netWork)
    {
        WatsonTcpTool watsonTcpTool = netWork.WatsonTcpTool;
        //连接方式
        string netMethod = netWork.NetworkDetailed.NetMethod;
        var whileTime = 5000;
        while (!token.IsCancellationRequested)
        {
            switch (netMethod)
            {
                case "ModbusTcp":
                    await ModbusTcpConnect(netWork);
                    break;
                case "ModbusRtu":
                    await ModbusRtuConnect(netWork);
                    break;
                case "Tcp客户端":
                    await WatsonTcpClintConnect(netWork);
                    break;
                case "Tcp服务器":
                    await WatsonTcpServerConnect(netWork);
                    break;
                case "基恩士上位链路通讯":
                    await KeyneceHostLinkConnect(netWork);
                    break;
            }
            //五秒检查一次
            try
            {
                await Task.Delay(whileTime, token);
            }
            catch (OperationCanceledException e)
            {
                break;
            }
        }
    }
    public async Task KeyneceHostLinkConnect(NetWork netWork)
    {
        if (!netWork.KeyenceHostLinkTool.IsConnected)
        {
            bool connect = netWork.KeyenceHostLinkTool.Connect(netWork.NetworkDetailed.IP, netWork.NetworkDetailed.Port);
            if (connect)
            {
                if (netWork.KeyenceHostLinkTool.IsConnected)
                {
                    log.SuccessAndShowTask($"{netWork.NetworkDetailed.Name}:  基恩士上位链路协议连接成功");
                }
                else
                {
                    log.WarningAndShowTask($"{netWork.NetworkDetailed.Name}:  连接失败,请检查设置");
                }
            }
            else
            {
                log.WarningAndShowTask($"{netWork.NetworkDetailed.Name}:  连接失败,请检查设置");
            }
        }
    }

    public async Task ModbusTcpConnect(NetWork netWork)
    {
        var modbusBase = netWork.ModbusBase;
        if (!modbusBase.IsTCPConnect())
        {
            try
            {
                await modbusBase.OpenTcpMaster(netWork.NetworkDetailed.IP, netWork.NetworkDetailed.Port);
            }
            catch (Exception e)
            {
                log.ErrorAndShowTask($"{netWork.NetworkDetailed.Name}:  网络无配置,请配置好重新连接!");
                return;
            }
            if (modbusBase.IsTCPConnect())
            {
                log.SuccessAndShowTask($"{netWork.NetworkDetailed.Name}:  ModbusTCP连接成功");
            }
            else
            {
                log.WarningAndShowTask($"{netWork.NetworkDetailed.Name}:  连接失败,请检查设置");
            }
        }
    }

    public async Task ModbusRtuConnect(NetWork netWork)
    {
        var modbusBase = netWork.ModbusBase;
        if (!modbusBase.IsRTUConnect())
        {
            //串口连接
            try
            {
                await modbusBase.OpenRTUMaster(netWork.NetworkDetailed.Com,
                    int.Parse(netWork.NetworkDetailed.BaudRate),
                    int.Parse(netWork.NetworkDetailed.DataBits),
                    netWork.NetworkDetailed.StopBits, netWork.NetworkDetailed.Parity);
            }
            catch (Exception e)
            {
                log.ErrorAndShowTask($"{netWork.NetworkDetailed.Name}:  网络无配置,请配置好重新连接!");
                return;
            }

            if (modbusBase.IsRTUConnect())
                log.SuccessAndShowTask($"{netWork.NetworkDetailed.Name}:  ModbusRtu连接成功");
            else
                log.WarningAndShowTask($"{netWork.NetworkDetailed.Name}:  连接失败,请检查设置");
        }
    }

    public async Task WatsonTcpClintConnect(NetWork netWork)
    {
        if (!netWork.WatsonTcpTool.IsClientOpen)
        {
            if (netWork.WatsonTcpTool.OpenClint(netWork.NetworkDetailed.IP, netWork.NetworkDetailed.Port))
            {
                log.SuccessAndShowTask("Tcp客户端打开成功");
            }
            else
            {
                log.WarningAndShowTask("Tcp服务器打开失败");
            }
        }
    }

    public async Task WatsonTcpServerConnect(NetWork netWork)
    {
        if (!netWork.WatsonTcpTool.IsServerRunning)
        {
            if (netWork.WatsonTcpTool.OpenServer(netWork.NetworkDetailed.Port))
            {
                log.SuccessAndShowTask("Tcp服务器打开成功");
            }
            else
            {
                log.WarningAndShowTask("Tcp服务器打开失败");
            }
        }
 
    }
    #endregion

    #region 删除网络设置行

    [RelayCommand]
    public void DeleteReadRegDvg(HomePage page)
    {
        var item = page.setConnectDg.SelectedItem as NetworkDetailed;
        var source = page.setConnectDg.ItemsSource as ObservableCollection<NetworkDetailed>;
        if (item != null)
            if (source.Count > 0 && item.Name != null)
            {
                if (item.Open != true)
                {
                    HomePageModel.SetConnectDg.Remove(item);
                    GlobalMannager.NetWorkDictionary.Remove(item.Id);
                    log.SuccessAndShow("删除成功!", $"{item.Name}->连接被删除");
                }
                else
                {
                    log.WarningAndShow($"{item.Name}处于运行状态不能删除,请先停止");
                    return;
                }
            }
    }

    #endregion

    #region 设置网络配置

    [RelayCommand]
    public void SetConnectConfig(HomePage page)
    {
        var item = page.setConnectDg.SelectedItem as NetworkDetailed;

        page.IpSet.Visibility = Visibility.Visible;
        var currentState = "当前状态: 未配置";
        if (item.IP != null || item.Com != null) currentState = "当前状态: 已配置";

        HomePageModel.CurrentSetName = "当前配置:" + item.Name + "    " + currentState;


        if (item.IP != null)
        {
            ModbusToolModel.ModbusTcp_Ip_select = item.IP;
            ModbusToolModel.ModbusTcp_Port = item.Port;
        }

        if (item.Com != null)
        {
            ModbusToolModel.ModbusRtu_COM_select = item.Com;
            ModbusToolModel.ModbusRtu_baudRate_select = item.BaudRate;
            ModbusToolModel.ModbusRtu_dataBits_select = item.DataBits;
            ModbusToolModel.ModbusRtu_parity_select = item.Parity;
            ModbusToolModel.ModbusRtu_stopBits_select = item.StopBits;
            ModbusToolModel.NetMethod_select = item.NetMethod;
        }
    }

    [RelayCommand]
    public void CommitConfig(HomePage page)
    {
        var item = page.setConnectDg.SelectedItem as NetworkDetailed;

        page.IpSet.Visibility = Visibility.Collapsed;
        item.IP = ModbusToolModel.ModbusTcp_Ip_select;
        item.Port = ModbusToolModel.ModbusTcp_Port;
        item.Com = ModbusToolModel.ModbusRtu_COM_select;
        item.BaudRate = ModbusToolModel.ModbusRtu_baudRate_select;
        item.DataBits = ModbusToolModel.ModbusRtu_dataBits_select;
        item.Parity = ModbusToolModel.ModbusRtu_parity_select;
        item.StopBits = ModbusToolModel.ModbusRtu_stopBits_select;
        item.NetMethod = ModbusToolModel.NetMethod_select;
    }
    #endregion

    [RelayCommand]
    public void Save()
    {
        AppJsonStorage<HomePageModel>.Save(HomePageModel);
    }
}