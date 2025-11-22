using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.ViewModels.Page;
using Pkn_HostSystem.Views.Pages.LoginWindowPage;
using RestSharp;
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

        [RelayCommand]
        public async void LoginButton(LoginWindowPage1 page)
        {
            //获取到当前密码
            LoginModel.PassWord = page.PasswordBox.Password;
            //执行HTTP请求

            RestClient restClient = new RestClient("http://10.169.253.53:9005/lx-test-mesapi");
            RestRequest restRequest = new RestRequest("/LoginCheck", Method.Post);

            // 设置 Content-Type
            //restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var data = new
            {
                MachineId = LoginModel.MachineId,
                Mi = LoginModel.Mi,
                UserNumber = LoginModel.UserNumber,
                PassWord = LoginModel.PassWord,
                Formula=""
            };
            string json = JsonConvert.SerializeObject(data);
            restRequest.AddParameter("jsonData",json);
            RestResponse response = await restClient.ExecuteAsync(restRequest);

            if (response.IsSuccessStatusCode)
            {
                JObject jObject = JObject.Parse(response.Content);

                var homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();

                UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                if (jObject["status"]?.ToString() == "true")
                {
                    //登入账号result储存
                    string result = jObject["result"]?.ToString();
                    userLoginModel.Id = result;
               
                    //姓名
                    string name = jObject.SelectToken("testResultDetails[0].EmpName")?.ToString();
                    userLoginModel.Name = name;
                    //职位
                    string emp = jObject.SelectToken("testResultDetails[0].TcdpCode")?.ToString();
                    userLoginModel.Emp = emp;
                    Log.SuccessAndShowTask("登录成功");
                }
                else
                {
                    Log.ErrorAndShowTask($"登录失败,{jObject["result"]?.ToString()}");
                }
            }
            else
            {
                 Log.Error(response.ErrorMessage == null
                     ? $"登入失败:\r\n{response.Content}"
                     : $"登入失败:\r\n{response.ErrorMessage}");
            }
        }


        #region 弹窗SnackbarService
        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        #endregion
    }
}