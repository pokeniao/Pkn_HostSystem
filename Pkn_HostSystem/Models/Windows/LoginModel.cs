using CommunityToolkit.Mvvm.ComponentModel;
using System.IO.Packaging;
using System.Windows.Data;

namespace Pkn_HostSystem.Models.Windows
{
    public partial class LoginModel :ObservableObject
    {
        #region 登入1页面的Model
        [ObservableProperty] private string userNumber;
        [ObservableProperty] private string passWord;
        #endregion

        #region 登入2页面的Model
        //刷卡登入
        [ObservableProperty] private string swipingCardLogin = "点击刷卡登入";


        [ObservableProperty] private string swipResult;

        #endregion


    }
}