using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
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
using System.Reactive;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Windows
{
    public partial class LoginViewModel:ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogControl<LoginViewModel> Log;
        public LoginModel LoginModel { get; set; }

        public LoginViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<LoginViewModel>(SnackbarService);
            //Model初始化
            LoginModel = new LoginModel();
        }

        private CancellationTokenSource timeOutCts;
        [RelayCommand]
        public async void LoginButton(LoginWindowPage1 page)
        {
            //获取到当前密码
            try
            {
                if (timeOutCts != null)
                {

                    timeOutCts.Cancel();
                }
                timeOutCts = new CancellationTokenSource();

                LoginModel.PassWord = page.PasswordBox.Password;
                var homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                if (LoginModel.PassWord == "15577729871" && LoginModel.UserNumber =="pokeniao")
                {
                    userLoginModel.LoginState = true;
                    userLoginModel.Name = "pokeniao";
                    userLoginModel.Emp = "管理员";
                    userLoginModel.Id = "1";
                    timeOutTimer(userLoginModel, timeOutCts);
                    UserContext.Current.Permission = (LoginPermissionEnum)4;
                    page.Close();
                }
                else
                {
                    userLoginModel.LoginState = false;
                    userLoginModel.Name = "";
                    userLoginModel.Emp = "";
                    userLoginModel.Id = "";
                    UserContext.Current.Permission = (LoginPermissionEnum)0;
                }
            }
            catch (Exception e)
            {
                Log.ErrorAndShowTask(e.Message);
            }
        }

        [RelayCommand]
        public async void LoginOutButton(LoginWindowPage1 page)
        {
            //获取到当前密码
            try
            {
                if (timeOutCts != null)
                {

                    timeOutCts.Cancel();
                }
                timeOutCts = new CancellationTokenSource();

                LoginModel.PassWord = page.PasswordBox.Password;
                var homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                userLoginModel.LoginState = false;
                userLoginModel.Name = "";
                userLoginModel.Emp = "";
                userLoginModel.Id = "";
                UserContext.Current.Permission = (LoginPermissionEnum)0;
            }
            catch (Exception e)
            {
                Log.ErrorAndShowTask(e.Message);
            }
        }


        public async Task timeOutTimer(UserLoginModel userLoginModel, CancellationTokenSource cts)
        {
            //延迟5分钟
            // await Task.Delay(1000, cts.Token);
            await Task.Delay(300000, cts.Token);
            userLoginModel.Name = "";
            userLoginModel.Emp = "";
            userLoginModel.Id = "";
            userLoginModel.LoginState = false;
            UserContext.Current.Permission = (LoginPermissionEnum)0;
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