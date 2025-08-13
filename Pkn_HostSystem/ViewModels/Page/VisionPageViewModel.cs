using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;
using HalconDotNet;
using Pkn_HostSystem.Models.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class VisionPageViewModel : ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; } = new SnackbarService();
        public LogControl<VisionPageViewModel> Log;
        public VisionPageModel VisionPageModel { get; set; }
        public List<string> CameraShowMethodList { get; set; } = ["适应窗口模式", "100%", "50%", "25%"];
        //页面显示Control
        private HSmartWindowControlWPF HSmartWindowControl { get; set; }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;  // 禁用滚轮缩放
        }
        [RelayCommand]
        public async void RunTest()
        {

            HSmartWindowControl.PreviewMouseWheel += OnPreviewMouseWheel;
            HWindow hWindow = HSmartWindowControl.HalconWindow;
            HObject image;
            HOperatorSet.ReadImage(out image, "printer_chip/printer_chip_01");
            HOperatorSet.DispObj(image, hWindow);
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.SetLineWidth(hWindow, 1);
            HOperatorSet.SetColor(hWindow, "green");
            HObject ho_ROI_0;

            HTuple row1 = null, column1 = null, row2 = null, column2 = null;

            // 开启异步模式
            await Task.Run(async () =>
            {

            HOperatorSet.DrawRectangle1Mod(hWindow, 50, 50, 150, 150, out row1, out column1, out row2, out column2);
            });
            // HOperatorSet.DrawRectangle1(hWindow, out row1, out column1, out row2, out column2);

            HOperatorSet.GenRectangle1(out ho_ROI_0, row1, column1, row2, column2);
            HOperatorSet.SetDraw(hWindow,"fill");
            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.DispObj(ho_ROI_0, hWindow);

            HSmartWindowControl.PreviewMouseWheel -= OnPreviewMouseWheel;
        }


        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

        #endregion

        #region 赋值

        public void setHSmartWindowControl(HSmartWindowControlWPF HalconControl)
        {
            HSmartWindowControl = HalconControl;
        }

        #endregion

        #region 保存程序

        [RelayCommand]
        public void Save()
        {
            JsonTool<VisionPageModel>.Save(VisionPageModel);
        }

        #endregion
    }
}