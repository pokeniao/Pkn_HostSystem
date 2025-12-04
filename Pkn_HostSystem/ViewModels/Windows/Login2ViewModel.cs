using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
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
                SwipingCardLogin();
                LoginModel.SwipingCardLogin = "点击刷卡登入";
            }
        }



        private async void SwipingCardLogin()
        {
            //获取到当前密码
            try
            {
                var homePageViewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                UserLoginModel userLoginModel = homePageViewModel.UserLoginModel;
                HomePageModel homePageModel = homePageViewModel.HomePageModel;
                //执行HTTP请求
        
                RestClient restClient = new RestClient(homePageModel.VocPojo.MesHttp);
                RestRequest restRequest = new RestRequest("/LoginDevice", Method.Post);
        
                // 设置 Content-Type
                //restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                
                var data = new
                {
                    MachineId = homePageModel.VocPojo.MachineId,
                    CardNumber = LoginModel.SwipResult,
                };
                string json = JsonConvert.SerializeObject(data);

                Log.Info($"设备登录权限管控接口请求发送json:\r\n{json}");
                restRequest.AddParameter("jsonData", json);
                RestResponse response = await restClient.ExecuteAsync(restRequest);
                Log.Info($"设备登录权限管控接口:\r\n{response.Content}");
                if (response.IsSuccessStatusCode)
                {
                   
                    JObject jObject = JObject.Parse(response.Content);
                    if (jObject["status"]?.ToString() == "true")
                    {
                        //员工权限等级
                        string testResult = jObject.SelectToken("testResultDetails[0].EmpLevel")?.ToString();
                        ushort write = 0;
                        switch (testResult)
                        {
                            case "1":
                                userLoginModel.Name = "操作员 ";
                                write = 1;
                                break;
                            case "2":
                                userLoginModel.Name = "技术员 ";
                                write = 2;
                                break;
                            case "3":
                                userLoginModel.Name = "工程师 ";
                                write = 3;
                                break;
                            case "4":
                                userLoginModel.Name = "MES权限 ";
                                write = 5;
                                break;
                            case "5":
                                userLoginModel.Name = "品质管理权限 ";
                                write = 6;
                                break;
                            case "6":
                                userLoginModel.Name = "管理员 ";
                                write = 4;
                                break;
                        }


                        //姓名
                        string name = jObject.SelectToken("testResultDetails[0].EmpName")?.ToString();
                        userLoginModel.Name = userLoginModel.Name + name;
                        //职位
                        string emp = jObject.SelectToken("testResultDetails[0].TcdpCode")?.ToString();
                        userLoginModel.Emp = emp;
                        Log.SuccessAndShowTask("登录成功");

                        //获取上岗校验
                        restClient = new RestClient(homePageModel.VocPojo.MesHttp);
                        restRequest = new RestRequest("/LoginCheckLicense", Method.Post);
                        var data3 = new
                        {
                            MachineId = homePageModel.VocPojo.MachineId,
                            MI = homePageModel.VocPojo.Mi,
                            CardNo = LoginModel.SwipResult,
                        };
                        json = JsonConvert.SerializeObject(data3);
                        Log.Info($"获取上岗校验发送json:\r\n{json}");
                        restRequest.AddParameter("jsonData", json);
                        response = await restClient.ExecuteAsync(restRequest);
                        Log.Info($"上岗证校验:\r\n{response.Content}");
                        if (response.IsSuccessStatusCode)
                        {
                            if (jObject["status"]?.ToString() == "true")
                            {
                                //登入账号result储存
                                userLoginModel.Id = jObject["result"]?.ToString();
                                Log.Info($"登入成功返回Id:{userLoginModel.Id}");

                                //获取PLC
                                var netWork = GlobalManager.GetNetWork("PLC(不能修改名称不然会运行失败)");
                                if (netWork == null)
                                {
                                    userLoginModel.Name = "";
                                    userLoginModel.Emp = "";
                                    userLoginModel.Id = "";
                                    Log.Error($"登入成功但与PLC通讯连接失败");
                                    return;
                                }
                                //获取PLC的 ModbusBase
                                ModbusBase PlcModbusTcp = netWork.ModbusBase;
                                //写入权限等级
                                await PlcModbusTcp.WriteRegister_06(1, 300, write);


                            }
                            else
                            {
                                userLoginModel.Name = "";
                                userLoginModel.Emp = "";
                                userLoginModel.Id = "";
                                Log.Error($"上岗证校验失败,{jObject["result"]?.ToString()}");
                                return;
                            }
                        }
                        else
                        {
                            userLoginModel.Name = "";
                            userLoginModel.Emp = "";
                            userLoginModel.Id = "";
                            Log.Error(response.ErrorMessage == null
                                ? $"上岗证校验获取失败:\r\n{response.Content}"
                                : $"上岗证校验获取失败:\r\n{response.ErrorMessage}");
                            return;
                        }

                        //获取参数下发
                        restClient = new RestClient(homePageModel.VocPojo.MesHttp);
                        restRequest = new RestRequest("/GetSpecifications", Method.Post);
                        var data2 = new
                        {
                            MachineId = homePageModel.VocPojo.MachineId,
                            TimeStamp = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                            OperatorId = userLoginModel.Id,
                            Sn = homePageModel.VocPojo.Mi,
                            Type = "MI",
                            GroupCode = homePageModel.VocPojo.GroupCode,
                        };
                        json = JsonConvert.SerializeObject(data2);
                        Log.Info($"获取参数下发发送json:\r\n{json}");
                        restRequest.AddParameter("jsonData", json);
                        response = await restClient.ExecuteAsync(restRequest);
                        jObject = JObject.Parse(response.Content);
                        Log.Info($"参数下发:\r\n{response.Content}");

                        if (response.IsSuccessStatusCode)
                        {
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
                                userLoginModel.Name = "";
                                userLoginModel.Emp = "";
                                userLoginModel.Id = "";
                                Log.Error($"参数下发获取失败,{jObject["result"]?.ToString()}");
                                return;
                            }
                        }
                        else
                        {
                            userLoginModel.Name = "";
                            userLoginModel.Emp = "";
                            userLoginModel.Id = "";
                            Log.Error(response.ErrorMessage == null
                                ? $"参数下发获取失败:\r\n{response.Content}"
                                : $"参数下发获取失败:\r\n{response.ErrorMessage}");
                            return;
                        }
                    }
                    else
                    {
                        userLoginModel.Name = "";
                        userLoginModel.Emp = "";
                        userLoginModel.Id = "";
                        Log.ErrorAndShowTask($"登录失败,{jObject["result"]?.ToString()}");
                    }
                }
                else
                {
                    userLoginModel.Name = "";
                    userLoginModel.Emp = "";
                    userLoginModel.Id = "";
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