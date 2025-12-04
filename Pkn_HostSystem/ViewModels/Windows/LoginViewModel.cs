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

        [RelayCommand]
        public async void LoginButton(LoginWindowPage1 page)
        {
            //获取到当前密码
            try
            {
                LoginModel.PassWord = page.PasswordBox.Password;
                var homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                HomePageModel homePageModel = homePageViewModel.HomePageModel;
                //执行HTTP请求

                RestClient restClient = new RestClient(homePageModel.VocPojo.MesHttp);
                RestRequest restRequest = new RestRequest("/LoginCheck", Method.Post);

                // 设置 Content-Type
                //restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                var data = new
                {
                    MachineId = homePageModel.VocPojo.MachineId,
                    Mi = homePageModel.VocPojo.Mi,
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
                        //获取参数下发
                        restClient = new RestClient(homePageModel.VocPojo.MesHttp);
                        restRequest = new RestRequest("/GetSpecifications", Method.Post);
                        var data2 = new
                        {
                            MachineId = homePageModel.VocPojo.MachineId,
                            TimeStamp = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                            OperatorId = result,
                            Sn = homePageModel.VocPojo.Mi,
                            Type = "MI",
                            GroupCode = homePageModel.VocPojo.GroupCode,
                        };
                        json = JsonConvert.SerializeObject(data2);
                        restRequest.AddParameter("jsonData", json);
                        response = await restClient.ExecuteAsync(restRequest);
                        jObject = JObject.Parse(response.Content);
                        if (response.IsSuccessStatusCode)
                        {
                            Log.Info($"参数下发{response.Content}");
                            if (jObject["status"]?.ToString() == "true")
                            {
                                JArray? jArray = jObject["testResultDetails"] as JArray;
                                if (jArray != null)
                                {
                                    foreach (var item in jArray)
                                    {
                                        switch (item["ItemCode"]?.ToString())
                                        {
                                            case "TEST_TIME":
                                                if (double.TryParse(item["LowerLimit"]?.ToString(), out double ttResult))
                                                {
                                                    homePageModel.VocPojo.TestTime = ttResult;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Log.Error($"参数下发获取失败,{jObject["result"]?.ToString()}");
                            }
                        }
                        else
                        {
                            Log.Error(response.ErrorMessage == null
                                ? $"参数下发获取失败:\r\n{response.Content}"
                                : $"参数下发获取失败:\r\n{response.ErrorMessage}");
                        }
                    }
                    else
                    {
                        Log.ErrorAndShowTask($"登录失败,{jObject["result"]?.ToString()}");
                    }
                }
                else
                {
                    Log.ErrorAndShowTask(response.ErrorMessage == null
                        ? $"登入失败:\r\n{response.Content}"
                        : $"登入失败:\r\n{response.ErrorMessage}");
                }
            }
            catch (Exception e)
            {
                Log.ErrorAndShowTask(e.Message);
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