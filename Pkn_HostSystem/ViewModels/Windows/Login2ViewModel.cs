using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages.LoginWindowPage;
using RestSharp;
using System.Text;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows
{
    public partial class Login2ViewModel:ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogControl<Login2ViewModel> Log;
        public LoginModel LoginModel { get; set; }

        public LoginWindowPage2 Page { get; set; }

        public Login2ViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<Login2ViewModel>(SnackbarService);
            //Model初始化
            LoginModel = new LoginModel();
        }
        //运行
        [RelayCommand]
        public void SwipingCardLoginButton()
        {
            if (LoginModel.SwipingCardLogin == "点击刷卡登入")
            {
                LoginModel.SwipingCardLogin = "刷卡检测中";
                LoginModel.SwipResult ="";
                _hook = Hook.GlobalEvents();
                _hook.KeyPress += KeyboardMouseEvents_KeyPress;
            }
            else
            {
                // 停止监听
                _hook.KeyPress -= KeyboardMouseEvents_KeyPress;
                _hook.Dispose();
                LoginModel.SwipResult = "";
                _cardBuffer.Clear();
                LoginModel.SwipingCardLogin = "点击刷卡登入";
            }

           
        }

        private IKeyboardMouseEvents _hook;
        private StringBuilder _cardBuffer = new();
        private void KeyboardMouseEvents_KeyPress(object? sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // 只收数字
            if (char.IsDigit(e.KeyChar))
            {
                _cardBuffer.Append(e.KeyChar);
                return;
            }

            // 刷卡器通常以回车结束
            if (e.KeyChar == '\r')
            {
                LoginModel.SwipResult = _cardBuffer.ToString();
                _cardBuffer.Clear();
                // 停止监听
                _hook.KeyPress -= KeyboardMouseEvents_KeyPress;
                _hook.Dispose();
                // 执行登录逻辑
                // SwipingCardLogin();
                LoginModel.SwipingCardLogin = "点击刷卡登入";
                Page.Close();
            }
        }

        private CancellationTokenSource timeOutCts;


        public async Task timeOutTimer(UserLoginModel userLoginModel,ModbusBase PlcModbusTcp, CancellationTokenSource cts)
        {
            //延迟5分钟
            await Task.Delay(300000,cts.Token);
            userLoginModel.Name = "";
            userLoginModel.Emp = "";
            userLoginModel.Id = "";
            userLoginModel.LoginState = false;
            UserContext.Current.Permission = (LoginPermissionEnum)0;
            await PlcModbusTcp.WriteRegister_06(1, 300, 0);
            await PlcModbusTcp.WriteRegister_06(1, 302, 1);
            Log.Info($"登入超时,自动退出");
        }


        #region 弹窗SnackbarService
        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        #endregion
    }
}