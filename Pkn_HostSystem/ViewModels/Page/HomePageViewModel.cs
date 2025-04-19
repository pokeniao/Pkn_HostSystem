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
                SetConnectDg = new ObservableCollection<ConnectPojo>()
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
        var selectedItem = page.setConnectDg.SelectedItem as ConnectPojo;
        if (selectedItem.Open)
            StartConnectModbus(selectedItem);
        else
            StopConnectModbus(selectedItem);
    }

    public async void StartConnectModbus(ConnectPojo connectPojo)
    {
        var key = connectPojo.Id;
        var Name = connectPojo.Name;
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
            var workPoJo = new NetWorkPoJo()
            {
                NetWorkId = key,
                CancellationTokenSource = cts,
                ModbusBase = modbusBase,
                WatsonTcpTool = watsonTcpTool,
                ConnectPojo = connectPojo,
                KeyenceHostLinkTool = keyenceHostLinkTool
            };
            var lazy = new Lazy<Task>(() => RunAndReconnection(cts.Token, workPoJo));
            //创建网络连接
            workPoJo.Task = lazy;
            GlobalMannager.NetWorkDictionary.AddOrUpdate(workPoJo);
            await workPoJo.Task.Value;
        }
    }
    public void StopConnectModbus(ConnectPojo connectPojo)
    {
        var key = connectPojo.Id;
        var name = connectPojo.Name;
        if (name == null) return;

        var b = GlobalMannager.NetWorkDictionary.Lookup(key).HasValue;
        NetWorkPoJo netWorkPoJo;
        if (b)
            netWorkPoJo = GlobalMannager.NetWorkDictionary.Lookup(key).Value;
        else
            return;


        //取出密钥
        var cts = netWorkPoJo.CancellationTokenSource;

        cts?.Cancel();
        //创建新密钥
        cts = new CancellationTokenSource();

        //更新网络连接
        netWorkPoJo.CancellationTokenSource = cts;
        netWorkPoJo.Task = new Lazy<Task>(() => Task.Run(() => RunAndReconnection(cts.Token, netWorkPoJo)));

        //更新网络体
        GlobalMannager.NetWorkDictionary.AddOrUpdate(netWorkPoJo);

        if (netWorkPoJo.ModbusBase.IsTCPConnect())
        {
            //停止Modbus
            netWorkPoJo.ModbusBase.CloseTCP();
            log.SuccessAndShowTask($"ModbusTCP:{name}---连接断开");
        }

        if (netWorkPoJo.ModbusBase.IsRTUConnect())
        {
            netWorkPoJo.ModbusBase.CloseRTU();
            log.SuccessAndShowTask($"ModbusRTU:{name}---连接断开");
        }
        //停止Tcp服务器或者Tcp客户端
        if (netWorkPoJo.WatsonTcpTool.IsConnected)
        {
            netWorkPoJo.WatsonTcpTool.CloseClint();
            log.SuccessAndShowTask($"TcpClint:{name}---连接断开");
        }

        if (netWorkPoJo.WatsonTcpTool.IsServerRunning)
        {
            netWorkPoJo.WatsonTcpTool.StopServer();
            log.SuccessAndShowTask($"TcpServer:{name}---连接断开");
        }
        //停止
        if (netWorkPoJo.KeyenceHostLinkTool.IsConnected)
        {
            netWorkPoJo.KeyenceHostLinkTool.Disconnect();
            log.SuccessAndShowTask($"上位链路通讯:{name}---连接断开");
        }
    }

    public async Task RunAndReconnection(CancellationToken token, NetWorkPoJo netWorkPoJo)
    {
        WatsonTcpTool watsonTcpTool = netWorkPoJo.WatsonTcpTool;
        //连接方式
        string netMethod = netWorkPoJo.ConnectPojo.NetMethod;
        var whileTime = 5000;
        while (!token.IsCancellationRequested)
        {
            switch (netMethod)
            {
                case "ModbusTcp":
                    await ModbusTcpConnect(netWorkPoJo);
                    break;
                case "ModbusRtu":
                    await ModbusRtuConnect(netWorkPoJo);
                    break;
                case "Tcp客户端":
                    await WatsonTcpClintConnect(netWorkPoJo);
                    break;
                case "Tcp服务器":
                    await WatsonTcpServerConnect(netWorkPoJo);
                    break;
                case "基恩士上位链路通讯":
                    await KeyneceHostLinkConnect(netWorkPoJo);
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
    public async Task KeyneceHostLinkConnect(NetWorkPoJo netWorkPoJo)
    {
        if (!netWorkPoJo.KeyenceHostLinkTool.IsConnected)
        {
            bool connect = netWorkPoJo.KeyenceHostLinkTool.Connect(netWorkPoJo.ConnectPojo.IP, netWorkPoJo.ConnectPojo.Port);
            if (connect)
            {
                if (netWorkPoJo.KeyenceHostLinkTool.IsConnected)
                {
                    log.SuccessAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  基恩士上位链路协议连接成功");
                }
                else
                {
                    log.WarningAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  连接失败,请检查设置");
                }
            }
            else
            {
                log.WarningAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  连接失败,请检查设置");
            }
        }
    }

    public async Task ModbusTcpConnect(NetWorkPoJo netWorkPoJo)
    {
        var modbusBase = netWorkPoJo.ModbusBase;
        if (!modbusBase.IsTCPConnect())
        {
            try
            {
                await modbusBase.OpenTcpMaster(netWorkPoJo.ConnectPojo.IP, netWorkPoJo.ConnectPojo.Port);
            }
            catch (Exception e)
            {
                log.ErrorAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  网络无配置,请配置好重新连接!");
                return;
            }
            if (modbusBase.IsTCPConnect())
            {
                log.SuccessAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  ModbusTCP连接成功");
            }
            else
            {
                log.WarningAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  连接失败,请检查设置");
            }
        }
    }

    public async Task ModbusRtuConnect(NetWorkPoJo netWorkPoJo)
    {
        var modbusBase = netWorkPoJo.ModbusBase;
        if (!modbusBase.IsRTUConnect())
        {
            //串口连接
            try
            {
                await modbusBase.OpenRTUMaster(netWorkPoJo.ConnectPojo.Com,
                    int.Parse(netWorkPoJo.ConnectPojo.BaudRate),
                    int.Parse(netWorkPoJo.ConnectPojo.DataBits),
                    netWorkPoJo.ConnectPojo.StopBits, netWorkPoJo.ConnectPojo.Parity);
            }
            catch (Exception e)
            {
                log.ErrorAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  网络无配置,请配置好重新连接!");
                return;
            }

            if (modbusBase.IsRTUConnect())
                log.SuccessAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  ModbusRtu连接成功");
            else
                log.WarningAndShowTask($"{netWorkPoJo.ConnectPojo.Name}:  连接失败,请检查设置");
        }
    }

    public async Task WatsonTcpClintConnect(NetWorkPoJo netWorkPoJo)
    {
        if (!netWorkPoJo.WatsonTcpTool.IsClientOpen)
        {
            if (netWorkPoJo.WatsonTcpTool.OpenClint(netWorkPoJo.ConnectPojo.IP, netWorkPoJo.ConnectPojo.Port))
            {
                log.SuccessAndShowTask("Tcp客户端打开成功");
            }
            else
            {
                log.WarningAndShowTask("Tcp服务器打开失败");
            }
        }
    }

    public async Task WatsonTcpServerConnect(NetWorkPoJo netWorkPoJo)
    {
        if (!netWorkPoJo.WatsonTcpTool.IsServerRunning)
        {
            if (netWorkPoJo.WatsonTcpTool.OpenServer(netWorkPoJo.ConnectPojo.Port))
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
        var item = page.setConnectDg.SelectedItem as ConnectPojo;
        var source = page.setConnectDg.ItemsSource as ObservableCollection<ConnectPojo>;
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
        var item = page.setConnectDg.SelectedItem as ConnectPojo;

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
        var item = page.setConnectDg.SelectedItem as ConnectPojo;

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