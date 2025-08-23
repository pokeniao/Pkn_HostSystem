using CommunityToolkit.Mvvm.ComponentModel;
using HalconDotNet;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Base.Log;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class DesignViewModel:ObservableRecipient
    {

        //页面显示Control
        public HalconControl HalconControl { get; set; } = new HalconControl();

        public HalconTool HalconTool { get; set; }


        public List<ComBoxEnumItem<CameraShowSizeEnum>> CameraShowMethodList { get; set; } = Enum
            .GetValues(typeof(CameraShowSizeEnum)).Cast<CameraShowSizeEnum>().Select(v =>
                new ComBoxEnumItem<CameraShowSizeEnum>(
                    )
                    { Value = v, Display = v.GetDescription() }).ToList();

        public SnackbarService SnackbarService { get; set; }
        public LogControl<DesignViewModel> Log;

        public DesignViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<DesignViewModel>(SnackbarService);
            HalconTool = new HalconTool(HalconControl);
        }

        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        //页面显示Control设置
        public void setHSmartWindowControl(HSmartWindowControlWPF _halconControl)
        {
            HalconControl.HSmartWindowControl = _halconControl;
        }
        #endregion              
    }
}