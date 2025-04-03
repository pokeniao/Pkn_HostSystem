using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WPF_NET.Base;
using WPF_NET.Models;
using WPF_NET.Pojo;
using WPF_NET.Static;
using WPF_NET.Views.Pages;

namespace WPF_NET.ViewModels;

public partial class HomePageViewModel : ObservableRecipient
{
    private LogBase<HomePageViewModel> log;

    public HomePageModel HomePageModel { get; set; }

    public ModbusToolModel ModbusToolModel { get; set; }

    public SnackbarService SnackbarService { get; set; } = new SnackbarService();
    public ModbusBase ModbusBase { get; set; } = new ModbusBase();


    public HomePageViewModel()
    {
        log = new LogBase<HomePageViewModel>(SnackbarService);
        //全局的
        GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out object obj);
        HomePageModel = new HomePageModel()
        {
            LogListBox = (ObservableCollection<string>)obj,
            SetConnectDg = new ObservableCollection<ConnectPojo>()
        };
        //初始化Model
        ModbusToolModel = new ModbusToolModel()
        {
            ModbusTcp_Ip = ModbusBase.getIpAddress().ToList(),
            ModbusRtu_COM = ModbusBase.getCOM().ToList(),
            ModbusTcp_Port = int.Parse("502"),
            ModbusRtu_baudRate = new List<string>() { "9600", "14400", "19200" },
            ModbusRtu_dataBits = new List<string>() { "8", "7" },
            ModbusRtu_stopBits = Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList(),
            ModbusRtu_parity = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList(),
            FuntionCode = new List<string>()
                { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" },
            SlaveAddress = 1,
            StartAddress = 0,
            ReadCount = 1,
        };
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
        if (LogListBox.Items.Count == 0)
        {
            return;
        }

        LogListBox.ScrollIntoView(LogListBox.Items[^1]);
    }


    [RelayCommand]

    #endregion

    #region 连接网络

    public void ConnectModbus(HomePage page)
    {
        ConnectPojo? selectedItem = page.setConnectDg.SelectedItem as ConnectPojo;
        if (selectedItem.Open)
        {
            StartConnectModbus(selectedItem);
        }
        else
        {
            StopConnectModbus(selectedItem);
        }
    }

    public async void StartConnectModbus(ConnectPojo connectPojo)
    {
        string key = connectPojo.Id;
        string Name = connectPojo.Name;
        if (Name == null)
        {
            log.ErrorAndShow("请先填写好连接名,并且回车确认");
            return;
        }

        if (GlobalMannager.NetWorkDictionary.TryGetValue(key, out NetWorkPoJo netWorkPoJo))
        {
            await netWorkPoJo.Task.Value;
        }
        else
        {
            //令牌生成
            CancellationTokenSource cts = new CancellationTokenSource();
            ModbusBase modbusBase = new ModbusBase();
            NetWorkPoJo workPoJo = new NetWorkPoJo()
            {
                NetWorkId = key,
                CancellationTokenSource = cts,
                ModbusBase = modbusBase,
                ConnectPojo = connectPojo
            };
            Lazy<Task> lazy = new Lazy<Task>(() => ReconnectionModbus(cts.Token, workPoJo));
            //创建网络连接
            workPoJo.Task = lazy;
            GlobalMannager.NetWorkDictionary.TryAdd(key, workPoJo);
            await lazy.Value;
        }
    }

    public void StopConnectModbus(ConnectPojo connectPojo)
    {
        string key = connectPojo.Id;
        string name = connectPojo.Name;
        if (name == null)
        {
            return;
        }
        GlobalMannager.NetWorkDictionary.TryGetValue(key, out NetWorkPoJo netWorkPoJo);

        //取出密钥
        CancellationTokenSource cts = netWorkPoJo.CancellationTokenSource;
        cts.Cancel();
        //创建新密钥
        cts = new CancellationTokenSource();

        //更新网络连接
        netWorkPoJo.CancellationTokenSource = cts;
        netWorkPoJo.Task = new Lazy<Task>(() => Task.Run(() => ReconnectionModbus(cts.Token, netWorkPoJo)));

        //更新网络体
        GlobalMannager.NetWorkDictionary.AddOrUpdate(key, netWorkPoJo,
            (k, oldCts) => netWorkPoJo);

        if (netWorkPoJo.ModbusBase.IsTCPConnect() || netWorkPoJo.ModbusBase.IsRTUConnect())
        {
            //停止Modbus
            netWorkPoJo.ModbusBase.CloseRTU();
            netWorkPoJo.ModbusBase.CloseTCP();
            log.SuccessAndShowTask($"{name}:  连接断开");
        }
       
    }

    public async Task ReconnectionModbus(CancellationToken token, NetWorkPoJo netWorkPoJo)
    {
        ModbusBase modbusBase = netWorkPoJo.ModbusBase;

        string Name = netWorkPoJo.ConnectPojo.Name;
        int whileTime = 5000;
        while (!token.IsCancellationRequested)
        {
            if (modbusBase.IsTCPConnect() || modbusBase.IsRTUConnect())
            {
            }
            else
            {
                await modbusBase.OpenTcpMaster(netWorkPoJo.ConnectPojo.IP, netWorkPoJo.ConnectPojo.Port);
                if (modbusBase.IsTCPConnect())
                {
                    log.SuccessAndShowTask($"{Name}:  ModbusTCP连接成功");
                }
                else
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
                        log.ErrorAndShowTask($"{Name}:  网络无配置,请配置好重新连接!");
                        return;
                    }

                    if (modbusBase.IsRTUConnect())
                    {
                        log.SuccessAndShowTask($"{Name}:  ModbusRtu连接成功");
                    }
                    else
                    {
                        log.WarningAndShowTask($"{Name}:  连接失败,请检查设置");
                    }
                }
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

    #endregion


    #region 删除网络设置行

    [RelayCommand]
    public void DeleteReadRegDvg(HomePage page)
    {
        ConnectPojo? item = page.setConnectDg.SelectedItem as ConnectPojo;
        var source = page.setConnectDg.ItemsSource as ObservableCollection<ConnectPojo>;
        if (item != null)
        {
            if (source.Count > 0 && item.Name != null )
            {
                if (item.Open != true)
                {
                    HomePageModel.SetConnectDg.Remove(item);
                    GlobalMannager.NetWorkDictionary.Remove(item.Id, out _);
                    log.SuccessAndShow("删除成功!", $"{item.Name}->连接被删除");
                }
                else
                {
                    log.WarningAndShow($"{item.Name}处于运行状态不能删除,请先停止");
                    return;
                }
                
            }
        }
    }

    #endregion


    #region 设置网络配置

    [RelayCommand]
    public void SetConnectConfig(HomePage page)
    {
        ConnectPojo? item = page.setConnectDg.SelectedItem as ConnectPojo;

        page.IpSet.Visibility = Visibility.Visible;
        string currentState = "当前状态: 未配置";
        if (item.IP != null || item.Com != null)
        {
            currentState = "当前状态: 已配置";
        }

        HomePageModel.CurrentSetName = "当前配置:" + item.Name+"    "+ currentState;

        
        if (item.IP != null )
        {
            ModbusToolModel.ModbusTcp_Ip_select = item.IP;
            ModbusToolModel.ModbusTcp_Port = item.Port;

        }
        if(item.Com != null)
        {
            ModbusToolModel.ModbusRtu_COM_select = item.Com;
            ModbusToolModel.ModbusRtu_baudRate_select = item.BaudRate;
            ModbusToolModel.ModbusRtu_dataBits_select = item.DataBits;
            ModbusToolModel.ModbusRtu_parity_select = item.Parity;
            ModbusToolModel.ModbusRtu_stopBits_select = item.StopBits;
        }
    }

    [RelayCommand]
    public void Commitconfig(HomePage page)
    {
        ConnectPojo? item = page.setConnectDg.SelectedItem as ConnectPojo;

        page.IpSet.Visibility = Visibility.Collapsed;
        item.IP = ModbusToolModel.ModbusTcp_Ip_select;
        item.Port = ModbusToolModel.ModbusTcp_Port;
        item.Com = ModbusToolModel.ModbusRtu_COM_select;
        item.BaudRate = ModbusToolModel.ModbusRtu_baudRate_select;
        item.DataBits = ModbusToolModel.ModbusRtu_dataBits_select;
        item.Parity = ModbusToolModel.ModbusRtu_parity_select;
        item.StopBits = ModbusToolModel.ModbusRtu_stopBits_select;
    }

    #endregion
}