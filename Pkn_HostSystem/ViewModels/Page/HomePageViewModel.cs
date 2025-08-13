using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using HalconDotNet;
using Microsoft.Win32;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using Pkn_HostSystem.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class HomePageViewModel : ObservableRecipient
{
    [ObservableProperty] private UserLoginEnum curLoginState;

    private LogControl<HomePageViewModel> Log;

    public HomePageModel HomePageModel { get; set; } = JsonTool<HomePageModel>.Load();

    //页面显示Control
    public HalconControl HalconControl { get; set; } = new HalconControl();

    public HalconTool HalconTool { get; set; }

    public HomeSetConnectModel HomeSetConnectModel { get; set; } = new();
    public SnackbarService SnackbarService { get; set; } = new();

    public List<string> NetMethod { get; set; } =
        ["ModbusTcp", "ModbusRtu", "Tcp客户端", "Tcp服务器", "基恩士上位链路通讯", "串口232/485"];


    public List<CameraInterfaceEnum> CameraConnectMethodDictionary { get; set; } = Enum
        .GetValues(typeof(CameraInterfaceEnum)).Cast<CameraInterfaceEnum>().ToList();


    public List<ComBoxEnumItem<CameraShowSizeEnum>> CameraShowMethodList { get; set; } = Enum
        .GetValues(typeof(CameraShowSizeEnum)).Cast<CameraShowSizeEnum>().Select(v =>
            new ComBoxEnumItem<CameraShowSizeEnum>(
            ) { Value = v, Display = v.GetDescription() }).ToList();

    public HomePageViewModel()
    {
        if (HomePageModel == null)
        {
            HomePageModel = new HomePageModel()
            {
                SetConnectDg = new ObservableCollection<NetworkDetailed>(), //创建 设置连接列表的DataGrid 绑定对象
                NetWorkList = new ObservableCollectionExtended<NetWork>()
            };
        }


        HalconTool = new HalconTool(HalconControl);


        GlobalManager.jdbcPath = HomePageModel.RealJdbcUrl; //设置全局jdbc连接路径
        //先添加 ,后绑定
        GlobalManager.NetWorkDictionary.AddOrUpdate(HomePageModel.NetWorkList);
        GlobalManager.NetWorkDictionary.Connect().Bind(HomePageModel.NetWorkList).Subscribe();

        //获取到 HTTP的集合 引用类型并且绑定到HomePageModel.HttpLists
        var vm = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
        HomePageModel.HttpLists = vm.LoadMesPageModel.MesPojoList;

        Log = new LogControl<HomePageViewModel>(SnackbarService);
    }

    #region 弹窗SnackbarService

    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #endregion

    #region 滚动到底部

    [RelayCommand]
    public void ScrollToBottom()
    {
    }

    #endregion

    #region 连接网络

    [RelayCommand]
    public async void Connect(HomePage page)
    {
        var selectedItem = page.setConnectDg.SelectedItem as NetworkDetailed;
        TraceContext.Name = selectedItem.Name;
        if (selectedItem.Open)
            await StartConnect(selectedItem);
        else
            StopConnect(selectedItem);
        TraceContext.Name = null;
    }

    public async Task StartConnect(NetworkDetailed networkDetailed)
    {
        //1. 获取到当前网络连接的细节
        var key = networkDetailed.Id;
        var Name = networkDetailed.Name;
        Log.Info($"[{TraceContext.Name}]--正在启动.");
        if (Name == null)
        {
            Log.ErrorAndShowTask("请先填写好连接名,并且回车确认后启动");
            return;
        }

        //2. 从静态网络连接池中获取对应的 网络对象类
        var lookup = GlobalManager.NetWorkDictionary.Lookup(key);

        //2.1 能获取到网络对象类
        if (lookup.HasValue)
        {
            var netWorkPoJo = lookup.Value;
            //不存在需要创建
            if (netWorkPoJo.Task == null)
            {
                var cts = new CancellationTokenSource(); // 创建新的CTS令牌, 控制流程
                netWorkPoJo.CancellationTokenSource = cts; //将令牌给入 网络对象类 ,方便获取
                netWorkPoJo.ModbusBase = new ModbusBase(); //创建Modbus通讯类
                netWorkPoJo.TcpTool = new TcpTool(); //创建Tcp通讯类
                netWorkPoJo.KeyenceHostLinkTool = new KeyenceHostLinkTool(); //创建基恩士通讯类
                netWorkPoJo.ScpiSerialTool = new ScpiSerialTool(); //创建SCPI通讯类
                //赋值一个单例懒加载的任务,用于连接和重连
                netWorkPoJo.Task = new Lazy<Task>(() => RunAndReconnection(cts, netWorkPoJo));

                //更新到静态全局网络字典中进行管理
                GlobalManager.NetWorkDictionary.AddOrUpdate(netWorkPoJo);
            }

            //执行RunAndReconnection用于连接和重连的任务
            await netWorkPoJo.Task.Value;
        }
        else
        {
            //2.2 不能获取到网络对象类
            var cts = new CancellationTokenSource(); // 创建新的CTS令牌, 控制流程
            var modbusBase = new ModbusBase(); //创建Modbus通讯类
            var tcpTool = new TcpTool(); //创建Tcp通讯类
            var keyenceHostLinkTool = new KeyenceHostLinkTool(); //创建基恩士通讯类
            ScpiSerialTool scpiSerialTool = new ScpiSerialTool(); //创建SCPI通讯类
            var workPoJo = new NetWork()
            {
                NetWorkId = key,
                CancellationTokenSource = cts,
                ModbusBase = modbusBase,
                TcpTool = tcpTool,
                NetworkDetailed = networkDetailed,
                KeyenceHostLinkTool = keyenceHostLinkTool,
                ScpiSerialTool = scpiSerialTool
            };
            //赋值一个单例懒加载的任务,用于连接和重连
            workPoJo.Task = new Lazy<Task>(() => RunAndReconnection(cts, workPoJo));
            //添加到静态全局网络字典中进行管理
            GlobalManager.NetWorkDictionary.AddOrUpdate(workPoJo);
            //执行RunAndReconnection用于连接和重连的任务
            await workPoJo.Task.Value;
        }
    }

    public void StopConnect(NetworkDetailed networkDetailed)
    {
        var key = networkDetailed.Id;
        var name = networkDetailed.Name;
        Log.Info($"[{TraceContext.Name}]--正在停止");
        if (name == null) return;

        var b = GlobalManager.NetWorkDictionary.Lookup(key).HasValue;
        NetWork netWork;
        if (b)
            netWork = GlobalManager.NetWorkDictionary.Lookup(key).Value;
        else
            return;


        //取出密钥
        var cts = netWork.CancellationTokenSource;

        cts?.Cancel();

        if (netWork.ModbusBase.IsTCPConnect())
        {
            //停止Modbus
            netWork.ModbusBase.CloseTCP();
            Log.SuccessAndShowTask($"[{TraceContext.Name}]--ModbusTCP连接断开");
        }

        if (netWork.ModbusBase.IsRTUConnect())
        {
            netWork.ModbusBase.CloseRTU();
            Log.SuccessAndShowTask($"[{TraceContext.Name}]--ModbusRTU连接断开");
        }

        //停止Tcp服务器或者Tcp客户端
        if (netWork.TcpTool.IsClientConnected)
        {
            netWork.TcpTool.DisconnectClient();
            Log.SuccessAndShowTask($"[{TraceContext.Name}]--TcpClint连接断开");
        }

        if (netWork.TcpTool.IsServerRunning)
        {
            netWork.TcpTool.StopServer();
            Log.SuccessAndShowTask($"[{TraceContext.Name}]--TcpServer连接断开");
        }

        //停止
        if (netWork.KeyenceHostLinkTool.IsConnected)
        {
            netWork.KeyenceHostLinkTool.Disconnect();
            Log.SuccessAndShowTask($"[{TraceContext.Name}]--上位链路通讯连接断开");
        }

        if (netWork.ScpiSerialTool.IsOpen)
        {
            netWork.ScpiSerialTool.Close();
            Log.SuccessAndShowTask($"[{TraceContext.Name}]--串口232/485断开");
        }

        //从全局变量中移除
        GlobalManager.NetWorkDictionary.Remove(netWork);
    }

    public async Task RunAndReconnection(CancellationTokenSource cts, NetWork netWork)
    {
        //连接方式
        string netMethod = netWork.NetworkDetailed.NetMethod;
        var whileTime = 100;
        while (!cts.Token.IsCancellationRequested)
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
                    await TcpClintConnect(netWork);
                    break;
                case "Tcp服务器":
                    await TcpServerConnect(netWork);
                    break;
                case "基恩士上位链路通讯":
                    await KeyneceHostLinkConnect(netWork);
                    break;
                case "串口232/485":
                    await ScpiSerialConnect(netWork);
                    break;
            }

            //五秒检查一次
            try
            {
                await Task.Delay(whileTime, cts.Token);
            }
            catch (Exception e)
            {
            }
        }
    }

    public async Task ScpiSerialConnect(NetWork netWork)
    {
        if (!netWork.ScpiSerialTool.IsOpen)
        {
            bool open = netWork.ScpiSerialTool.Open(netWork.NetworkDetailed.Com,
                int.Parse(netWork.NetworkDetailed.BaudRate),
                netWork.NetworkDetailed.Parity,
                int.Parse(netWork.NetworkDetailed.DataBits),
                netWork.NetworkDetailed.StopBits,
                netWork.NetworkDetailed.TimeOut,
                netWork.NetworkDetailed.NewLine);
            if (open)
            {
                Log.SuccessAndShowTask($"[{TraceContext.Name}]--串口232/485打开成功");
            }
            else
            {
                Log.WarningAndShowTask($"[{TraceContext.Name}]--串口232/485打开失败");
            }
        }
    }

    public async Task KeyneceHostLinkConnect(NetWork netWork)
    {
        if (!netWork.KeyenceHostLinkTool.IsConnected)
        {
            bool connect =
                await netWork.KeyenceHostLinkTool.Connect(netWork.NetworkDetailed.IP, netWork.NetworkDetailed.Port);
            if (connect)
            {
                if (netWork.KeyenceHostLinkTool.IsConnected)
                {
                    Log.SuccessAndShowTask($"[{TraceContext.Name}]--基恩士上位链路协议连接成功");
                }
                else
                {
                    Log.WarningAndShowTask($"[{TraceContext.Name}]--连接失败,请检查设置");
                }
            }
            else
            {
                Log.WarningAndShowTask($"[{TraceContext.Name}]--连接失败,请检查设置");
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
                Log.ErrorAndShowTask($"[{TraceContext.Name}]--网络无配置,请配置好重新连接!");
                return;
            }

            if (modbusBase.IsTCPConnect())
            {
                Log.SuccessAndShowTask($"[{TraceContext.Name}]--ModbusTCP连接成功");
            }
            else
            {
                Log.WarningAndShowTask($"[{TraceContext.Name}]--连接失败,正在等待尝试重连");
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
                Log.ErrorAndShowTask($"[{TraceContext.Name}]--网络无配置,请配置好重新连接!");
                return;
            }

            if (modbusBase.IsRTUConnect())
                Log.SuccessAndShowTask($"[{TraceContext.Name}]-- ModbusRtu连接成功");
            else
                Log.WarningAndShowTask($"[{TraceContext.Name}]--连接失败,请检查设置");
        }
    }

    public async Task TcpClintConnect(NetWork netWork)
    {
        if (!netWork.TcpTool.IsClientConnected)
        {
            if (await netWork.TcpTool.ConnectToServerAsync(netWork.NetworkDetailed.IP, netWork.NetworkDetailed.Port))
            {
                Log.SuccessAndShowTask($"[{TraceContext.Name}]--Tcp客户端打开成功");
            }
            else
            {
                Log.WarningAndShowTask($"[{TraceContext.Name}]--Tcp客户端打开失败");
            }
        }
    }

    public async Task TcpServerConnect(NetWork netWork)
    {
        if (!netWork.TcpTool.IsServerRunning)
        {
            if (await netWork.TcpTool.StartServerAsync(netWork.NetworkDetailed.Port,
                    netWork.NetworkDetailed.IsServerListen))
            {
                Log.SuccessAndShowTask($"[{TraceContext.Name}]--Tcp服务器打开成功");
            }
            else
            {
                Log.WarningAndShowTask($"[{TraceContext.Name}]--Tcp服务器打开失败");
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
                    GlobalManager.NetWorkDictionary.Remove(item.Id);
                    Log.SuccessAndShowTask("删除成功!", $"{item.Name}->连接被删除");
                }
                else
                {
                    Log.WarningAndShowTask($"{item.Name}处于运行状态不能删除,请先停止");
                    return;
                }
            }
    }

    #endregion


    #region 删除相机

    [RelayCommand]
    public void DeleteCameraDvg(HomePage page)
    {
        var item = page.CameraDvg.SelectedItem as CameraDetailed;
        var source = page.CameraDvg.ItemsSource as ObservableCollection<CameraDetailed>;
        if (item != null)
            if (source.Count > 0 && item.CameraName != null)
            {
                HomePageModel.CameraList.Remove(item);
                Log.SuccessAndShowTask("删除成功!", $"{item.CameraName} 相机被删除");
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
            HomeSetConnectModel.Ip = item.IP;
            HomeSetConnectModel.Port = item.Port;
            HomeSetConnectModel.TcpServerNeedListen = item.IsServerListen;
        }

        if (item.Com != null)
        {
            HomeSetConnectModel.Com = item.Com;
            HomeSetConnectModel.BaudRate = item.BaudRate;
            HomeSetConnectModel.DataBit = item.DataBits;
            HomeSetConnectModel.Parity = item.Parity;
            HomeSetConnectModel.StopBits = item.StopBits;
            HomeSetConnectModel.NetMethod = item.NetMethod;
            HomeSetConnectModel.TimeOut = item.TimeOut;
            HomeSetConnectModel.NewLine = item.NewLine;
        }
    }

    [RelayCommand]
    public void CommitConfig(HomePage page)
    {
        var item = page.setConnectDg.SelectedItem as NetworkDetailed;

        page.IpSet.Visibility = Visibility.Collapsed;
        item.IP = HomeSetConnectModel.Ip;
        item.Port = HomeSetConnectModel.Port;
        item.Com = HomeSetConnectModel.Com;
        item.BaudRate = HomeSetConnectModel.BaudRate;
        item.DataBits = HomeSetConnectModel.DataBit;
        item.Parity = HomeSetConnectModel.Parity;
        item.StopBits = HomeSetConnectModel.StopBits;
        item.NetMethod = HomeSetConnectModel.NetMethod;
        item.IsServerListen = HomeSetConnectModel.TcpServerNeedListen;
        item.TimeOut = HomeSetConnectModel.TimeOut;
        item.NewLine = HomeSetConnectModel.NewLine;
    }

    #endregion


    #region 设置当前数据库信息

    [RelayCommand]
    public void SetJDBC(HomePage page)
    {
        HomePageModel.JdbcUrl.Pwd = page.PasswordBox.Password;
        string path =
            $"Server={HomePageModel.JdbcUrl.Server};DataBase={HomePageModel.JdbcUrl.DataBase};Uid={HomePageModel.JdbcUrl.Uid};Pwd={HomePageModel.JdbcUrl.Pwd};TrustServerCertificate=True;";

        string showPwd = new string('*', HomePageModel.JdbcUrl.Pwd.Length);

        HomePageModel.ShowJdbcUrl =
            $"Server={HomePageModel.JdbcUrl.Server};DataBase={HomePageModel.JdbcUrl.DataBase};Uid={HomePageModel.JdbcUrl.Uid};Pwd={showPwd};TrustServerCertificate=True;";
        HomePageModel.RealJdbcUrl = path;
        GlobalManager.jdbcPath = path;
    }

    #endregion

    #region 登入

    [RelayCommand]
    public void Login()
    {
        LoginWindow loginWindow = new LoginWindow();
        loginWindow.ShowDialog();
    }

    #endregion


    #region 视觉

    /// <summary>
    /// 触发画面
    /// </summary>
    [RelayCommand]
    public async void VisionTrigger()
    {
        bool succeed = false;
        string? message = null;
        switch (HomePageModel.CollectMethod)
        {
            case CameraInterfaceEnum.图片:
                ( succeed,  message) =
                    await HalconTool.VisionTrigger(HomePageModel.CollectMethod, HomePageModel.PicturePath);

                break;
            case CameraInterfaceEnum.GenICamTL:
                (succeed, message) =
                    await HalconTool.VisionTrigger(HomePageModel.CollectMethod, HomePageModel.SelectCamera);
                break;
            case CameraInterfaceEnum.电脑摄像头:
                (succeed, message) = await HalconTool.VisionTrigger(HomePageModel.CollectMethod, null);
                break;
        }
        if (!succeed)
        {
            Log.ErrorAndShowTask( message);
        }
    }


    CancellationTokenSource cts = null;

    /// <summary>
    /// 实时画面
    /// </summary>
    [RelayCommand]
    public async void VisionRealTime()
    {
 
        bool succeed = false;
        string? message = null;
        if (HomePageModel.RealTimeName == "实时")
        {
            cts = new CancellationTokenSource();
            switch (HomePageModel.CollectMethod)
            {
                case CameraInterfaceEnum.图片:
                    (succeed, message) =
                        await HalconTool.VisionRealTime(true, HomePageModel.CollectMethod, HomePageModel.PicturePath, cts);
                    break;
                case CameraInterfaceEnum.GenICamTL:
                    (succeed, message) =
                        await HalconTool.VisionRealTime(true, HomePageModel.CollectMethod, HomePageModel.SelectCamera, cts);
                    break;
                case CameraInterfaceEnum.电脑摄像头:
                    (succeed, message) = await HalconTool.VisionRealTime(true, HomePageModel.CollectMethod,null, cts);
                    break;
            }
            if (!succeed)
            {
                Log.ErrorAndShowTask( message);
                return;
            }
            // 切换到停止状态
            HomePageModel.RealTimeName = "停止";
        }
        else if(HomePageModel.RealTimeName == "停止")
        {
            await HalconTool.VisionRealTime(false, HomePageModel.CollectMethod, null, cts);
            // 切换到实时状态
            HomePageModel.RealTimeName = "实时";
        }


    }


    [RelayCommand]
    public void OnOpenPicture()
    {
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
        };

        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        string fileName = openFileDialog.FileName;
        if (!File.Exists(fileName))
        {
            return;
        }

        HomePageModel.PicturePath = fileName;
    }

    #endregion


    [RelayCommand]
    public void Save()
    {
        JsonTool<HomePageModel>.Save(HomePageModel);
    }

    //页面显示Control设置
    public void setHSmartWindowControl(HSmartWindowControlWPF _halconControl)
    {
        HalconControl.HSmartWindowControl = _halconControl;
    }
}