using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Controls;
using WPF_NET.Base;
using WPF_NET.Models;
using WPF_NET.Static;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace WPF_NET.ViewModels;

public partial class HomePageViewModel : ObservableRecipient
{
    private LogBase<HomePageViewModel> log;

    public HomePageModel HomePageModel { get; set; }

    public ModbusToolModel ModbusToolModel { get; set; }

    public SnackbarService SnackbarService { get; set; } = new SnackbarService();
    public ModbusBase ModbusBase { get; set; } = new ModbusBase();

    [ObservableProperty] private bool connectPLCModebusBool;

    //令牌生成
    private CancellationTokenSource cts = new CancellationTokenSource();
    private Lazy<Task> LazyConnectPLCModebus;

    public HomePageViewModel()
    {
        log = new LogBase<HomePageViewModel>(SnackbarService);
        //全局的
        GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out object obj);
        HomePageModel = new HomePageModel()
        {
            LogListBox = (ObservableCollection<string>)obj,
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

        //初始化创建懒加载
        LazyConnectPLCModebus = new Lazy<Task>(() => Task.Run(() => ReconnectionModbus(cts.Token)));
    }

    #region 弹窗SnackbarService

    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion

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
    public void ConnectModbus()
    {
        if (ConnectPLCModebusBool)
        {
            StartConnectModbus();
        }
        else
        {
            StopConnectModbus();
        }
    }

    public async void StartConnectModbus()
    {
        await LazyConnectPLCModebus.Value;
    }

    public void StopConnectModbus()
    {
        cts.Cancel();
        cts = new CancellationTokenSource();
        LazyConnectPLCModebus = new Lazy<Task>(() => Task.Run(() => ReconnectionModbus(cts.Token)));
    }

    public async Task ReconnectionModbus(CancellationToken token)
    {
        int whileTime = 5000;
        while (!token.IsCancellationRequested)
        {
            if (ModbusBase.IsTCPConnect() || ModbusBase.IsRTUConnect())
            { }
            else
            {
                await ModbusBase.OpenTcpMaster(ModbusToolModel.ModbusTcp_Ip_select, ModbusToolModel.ModbusTcp_Port);
                if (ModbusBase.IsTCPConnect())
                {
                    log.SuccessAndShowTask("连接成功");
                }
                else
                {
                    log.WarningAndShowTask("连接失败,请检查IP端口号");
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
}