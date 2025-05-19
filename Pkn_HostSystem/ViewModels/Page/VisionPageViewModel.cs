using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Wpf.Ui;
using Wpf.Ui.Controls;
using HalconDotNet;
using System.Windows;
using System.Windows.Threading;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class VisionPageViewModel : ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogBase<VisionPageViewModel> Log;
        public VisionPageModel VisionPageModel { get; set; }


        private HSmartWindowControlWPF HSmartWindowControl { get; set; }
        private HTuple hv_AcqHandle = null;
        private HObject ho_Image = null;
        private HWindow ho_Window;
        private Thread ho_thread;
 

        private bool IsFirstGrab { get; set; } = true;
        public VisionPageViewModel()
        {
            VisionPageModel = AppJsonTool<VisionPageModel>.Load();
            if (VisionPageModel == null)
            {
                //Model初始化
                VisionPageModel = new VisionPageModel();
            }
            else
            {
            }

            SnackbarService = new SnackbarService();
            Log = new LogBase<VisionPageViewModel>(SnackbarService);
        }

        /// <summary>
        /// 触发画面
        /// </summary>
        [RelayCommand]
        public void VisionTrigger()
        {
            ho_Window = HSmartWindowControl.HalconWindow;
            hv_AcqHandle = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_AcqHandle.Dispose();
          
            // 启动相机
            HOperatorSet.OpenFramegrabber("GenICamTL", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", VisionPageModel.CameraName, 0, -1, out hv_AcqHandle);
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
            ho_Image.Dispose();
            HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
            HOperatorSet.GetImageSize(ho_Image, out HTuple width, out HTuple height);
            HOperatorSet.SetPart(ho_Window, 0, 0, height - 1, width - 1);
            // Halcon 图像自适应显示
            if (IsFirstGrab)
            {
                IsFirstGrab = false;
                HSmartWindowControl.SetFullImagePart();
            }
            ho_Window.DispObj(ho_Image);
            // 关闭摄像头
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);
            ho_Image.Dispose();
            hv_AcqHandle.Dispose();
        }
        CancellationTokenSource cts = null;
        /// <summary>
        /// 实时画面
        /// </summary>
        [RelayCommand]
        public void VisionRealTime()
        {
            if (VisionPageModel.RealTimeName == "实时")
            {
                hv_AcqHandle = new HTuple();
                HOperatorSet.GenEmptyObj(out ho_Image);
                hv_AcqHandle.Dispose();
                // 启动相机
                // 启动笔记本自带摄像头
                // HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1, "false", "default", "[0] ", 0, -1, out hv_AcqHandle);
                HOperatorSet.OpenFramegrabber("GenICamTL", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", VisionPageModel.CameraName, 0, -1, out hv_AcqHandle);
                HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
                VisionPageModel.RealTimeName = "停止";
                // 实时采集线程
                cts = new();
                Task.Run(() =>ContinuesGrab(cts));
            }
            else
            {
                // 释放
                cts?.Cancel();
                VisionPageModel.RealTimeName = "实时";
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                ho_Image.Dispose();
                hv_AcqHandle.Dispose();
            }
        }
        /// <summary>
        /// 定义实时采集函数
        /// </summary>
        private async Task ContinuesGrab(CancellationTokenSource cts)
        {
            ho_Window = HSmartWindowControl.HalconWindow;
            while (!cts.Token.IsCancellationRequested)
            {
                // 先释放内存
                ho_Image.Dispose();
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                HOperatorSet.GetImageSize(ho_Image, out HTuple width, out HTuple height);
                HOperatorSet.DispObj(ho_Image, ho_Window);
                // Halcon 图像自适应显示
                if (IsFirstGrab)
                {
                    IsFirstGrab = false;
                    // 在新的线程里用如下方式更新到界面
                    _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        HSmartWindowControl.SetFullImagePart();
                    });
                }
                await Task.Delay(100);
            }
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
            AppJsonTool<VisionPageModel>.Save(VisionPageModel);
        }

        #endregion
    }
}