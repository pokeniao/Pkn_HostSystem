using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO.Ports;
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
            ReadRegDvg = new ObservableCollection<SetModbusPojo>(),
            ReadCoiDvg = new ObservableCollection<SetModbusPojo>()
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
            ModbusBase.CloseRTU();
            ModbusBase.CloseTCP();
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

        _ = GlobalMannager.GlobalDictionary.TryRemove("HomeRegDvg", out object value);
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
                    log.SuccessAndShowTask("ModbusTCP连接成功");
                    GlobalMannager.GlobalDictionary.TryAdd("HomeRegDvg", HomePageModel.ReadRegDvg);
                }
                else
                {
                    //串口连接
                    await ModbusBase.OpenRTUMaster(ModbusToolModel.ModbusRtu_COM_select,
                        int.Parse(ModbusToolModel.ModbusRtu_baudRate_select),
                        int.Parse(ModbusToolModel.ModbusRtu_dataBits_select),
                        ModbusToolModel.ModbusRtu_stopBits_select, ModbusToolModel.ModbusRtu_parity_select);

                    if (ModbusBase.IsRTUConnect())
                    {
                        log.SuccessAndShowTask("ModbusRtu连接成功");
                        GlobalMannager.GlobalDictionary.TryAdd("HomeRegDvg", HomePageModel.ReadRegDvg);
                    }
                    else
                    {
                        log.WarningAndShowTask("连接失败,请检查设置");

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


    #region 删除行
    [RelayCommand]
    public void DeleteReadRegDvg(HomePage page)
    {
        SetModbusPojo? item = page.readRegDvg.SelectedItem as SetModbusPojo;

        if (item != null)
        {
            if (page.readRegDvg.Items.Count > 2)
            {
                HomePageModel.ReadRegDvg.Remove(item);
            }
        }
    }
    #endregion

}