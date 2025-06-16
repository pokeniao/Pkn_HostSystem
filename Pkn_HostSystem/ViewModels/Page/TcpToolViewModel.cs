using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Views.Pages;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class TcpToolViewModel : ObservableRecipient
    {
        public TcpToolModel TcpToolModel { get; set; } = AppJsonTool<TcpToolModel>.Load();

        private LogBase<TcpToolViewModel> Log;

        public SnackbarService SnackbarService { get; set; } = new();

        public TcpToolViewModel()
        {
            if (TcpToolModel == null)
            {
                TcpToolModel = new TcpToolModel();
            }

            Log = new LogBase<TcpToolViewModel>(SnackbarService);
        }

        /// <summary>
        /// 服务器连接
        /// </summary>
        /// <param name="page"></param>
        [RelayCommand]
        public async void ServerConnectButton(TcpToolPage page)
        {
            TcpNet tcpNet = new();

            TcpTool tcpTool = new();
            bool startServerAsync =
                await tcpTool.StartServerAsync(int.Parse(TcpToolModel.ServerPost), TcpToolModel.IsLearn);

            if (!startServerAsync)
            {
                Log.ErrorAndShow("创建服务器失败, 请检查端口号");
                return;
            }
            else
            {
                Log.SuccessAndShow("服务器创建成功");
            }

            tcpNet.TcpTool = tcpTool;
            tcpNet.Ip = "Server";
            tcpNet.Post = TcpToolModel.ServerPost;
            TcpToolModel.SelectServer.Add(tcpNet);
            tcpTool.OnClientConnected += (ip) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    tcpNet.AcceptMessage += $"[{DateTime.Now:HH:mm:ss}]--[{ip}]连接成功\r\n";
                    tcpNet.ServerConnectClient.Add(ip);
                });
            };
            tcpTool.OnClientDisconnected += (ip) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    tcpNet.AcceptMessage += $"[{DateTime.Now:HH:mm:ss}]--[{ip}]断开连接\r\n";
                    tcpNet.ServerConnectClient.Remove(ip);
                });
            };
            tcpTool.OnMessageReceived += (ip, message) =>
            {
                tcpNet.AcceptMessage += $"[{DateTime.Now:HH:mm:ss}]--[{ip}]--接收:\r\n{message} \r\n";
            };
        }

        /// <summary>
        /// 服务器断开
        /// </summary>
        /// <param name="page"></param>
        [RelayCommand]
        public void ServerCloseButton(TcpToolPage page)
        {
            TcpNet? selectedItem = page.SelectServerComboBox.SelectedItem as TcpNet;
            if (selectedItem == null)
            {
                Log.ErrorAndShow("断开服务器时,未选择任何服务器");
                return;
            }

            selectedItem.TcpTool.StopServer();
            TcpToolModel.SelectServer.Remove(selectedItem);
        }

        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="page"></param>
        [RelayCommand]
        public async void ClientConnectButton(TcpToolPage page)
        {
            string connectServerIp = TcpToolModel.ConnectServerIp;

            string connectServerPost = TcpToolModel.ConnectServerPost;
            TcpTool tcpTool = new TcpTool();

            if (await tcpTool.ConnectToServerAsync(connectServerIp, int.Parse(connectServerPost)))
            {
                Log.SuccessAndShowTask($"[{TraceContext.Name}]--Tcp客户端打开成功");
            }
            else
            {
                Log.WarningAndShowTask($"[{TraceContext.Name}]--Tcp客户端打开失败");
                return;
            }

            Task.Run(() => tcpTool.ClientReceiveFromServer(tcpTool._client.GetStream()));


            TcpNet tcpNet = new TcpNet();
            tcpNet.TcpTool = tcpTool;
            tcpNet.IpAndPost = (tcpTool._client?.Client.LocalEndPoint).ToString(); ;
            TcpToolModel.SelectClient.Add(tcpNet);

            tcpNet.OnMessageReceived = (ip, message) =>
            {
                tcpNet.AcceptMessage += $"[{DateTime.Now:HH:mm:ss}]--[{ip}]--接收:\r\n{message} \r\n";
            };

            tcpTool.OnMessageReceived += tcpNet.OnMessageReceived;
        }

        /// <summary>
        /// 客户端断开
        /// </summary>
        /// <param name="page"></param>
        [RelayCommand]
        public async void ClientCloseButton(TcpToolPage page)
        {
            TcpNet? selectedItem = page.SelectClientComboBox.SelectedItem as TcpNet;

            if (selectedItem == null)
            {
                Log.ErrorAndShow("断开客户端时,未选择任何客户端");
                return;
            }

            selectedItem.TcpTool.OnMessageReceived -= selectedItem.OnMessageReceived;
            //清空防止被应用
            selectedItem.OnMessageReceived = null;
            selectedItem.TcpTool.DisconnectClient();
            TcpToolModel.SelectClient.Remove(selectedItem);
        }

        /// <summary>
        /// 服务器发送按钮
        /// </summary>
        [RelayCommand]
        public async void SendServerButton(TcpToolPage page)
        {
            var item = page.ConnectClientListBox.SelectedItem as string;

            if (TcpToolModel.SelectServerTcpNet != null)
            {
                TcpToolModel.SelectServerTcpNet.TcpTool.ServerSendAsync(item,TcpToolModel.SelectServerTcpNet.SendMessage);
            }
            else
            {
                Log.ErrorAndShow("没有选中发送的服务器");
            }
        }

        /// <summary>
        /// 服务器发送按钮
        /// </summary>
        [RelayCommand]
        public async void SeverBroadCastButton()
        {
            if (TcpToolModel.SelectServerTcpNet != null)
            {
                TcpToolModel.SelectServerTcpNet.TcpTool.BroadcastAsync(TcpToolModel.SelectServerTcpNet.SendMessage);
            }
            else
            {
                Log.ErrorAndShow("没有选中发送的服务器");
            }
        }

        /// <summary>
        /// 客户端发送按钮
        /// </summary>
        [RelayCommand]
        public async void SendClientButton()
        {

            if (TcpToolModel.SelectClientTcpNet != null)
            {
                TcpToolModel.SelectClientTcpNet.TcpTool.ClientSendAsync(TcpToolModel.SelectClientTcpNet.SendMessage);
            }
            else
            {
                Log.ErrorAndShow("没有选中发送的客户端");
            }
      
        }
        /// <summary>
        /// 清除服务器按钮
        /// </summary>
        [RelayCommand]
        public async void ClearServerButton()
        {
            if (TcpToolModel.SelectServerTcpNet != null)
            {
                TcpToolModel.SelectServerTcpNet.AcceptMessage = String.Empty;
            }
            else
            {
                Log.ErrorAndShow("没有选中发送的服务器");
            }
        }
        
        /// <summary>
        /// 清除客户端按钮
        /// </summary>
        [RelayCommand]
        public async void ClearClientButton()
        {
            if (TcpToolModel.SelectClientTcpNet != null)
            {
                TcpToolModel.SelectClientTcpNet.AcceptMessage = String.Empty;
            }
            else
            {
                Log.ErrorAndShow("没有选中发送的客户端");
            }
        }




        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

        #endregion

        #region 保存程序

        [RelayCommand]
        public void Save()
        {
            AppJsonTool<TcpToolModel>.Save(TcpToolModel);
        }

        #endregion
    }
}