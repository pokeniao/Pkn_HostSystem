using HalconDotNet;
using Pkn_HostSystem.Base.Enum;
using System.Windows.Media;

namespace Pkn_HostSystem.Base.Halcon
{
    public class HalconControl 
    {
        /// <summary>
        /// 窗口对象
        /// </summary>
        public HSmartWindowControlWPF HSmartWindowControl { get; set; }
        /// <summary>
        /// 窗口
        /// </summary>
        public HWindow hWindow;
        /// <summary>
        /// 窗口显示图片
        /// </summary>
        public HObject hImage;
        /// <summary>
        /// 相机句柄
        /// </summary>
        public HTuple hv_AcqHandle;


        private CameraShowSizeEnum _cameraShowSize= CameraShowSizeEnum.适应窗口模式;

        public CameraShowSizeEnum CameraShowSize
        {
            get => _cameraShowSize;
            set
            {
                _cameraShowSize = value;
                OnChangeCameraShowMethod();
            }
        }

        /// <summary>
        /// 窗口大小改变信号量
        /// </summary>
        public  ManualResetEvent ChangeSizeManualResetEvent { get; set; } = new ManualResetEvent(false); //默认设置为




        public void OnChangeCameraShowMethod()
        {
            if (this.hImage == null)
            {
                return;
            }

            if (this.hWindow == null)
            {
                return;
            }

            if (this.CameraShowSize == null)
            {
                return;
            }


            bool waitOne = ChangeSizeManualResetEvent.WaitOne(1000);

            if (!waitOne)
            {
                return;
            }
            switch (this.CameraShowSize)
            {
                case CameraShowSizeEnum.适应窗口模式:
                    //获取图片尺寸
                    HOperatorSet.GetImageSize(this.hImage, out HTuple width, out HTuple height);
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, height - 1, width - 1);
                    this.hWindow.DispObj(this.hImage);
                    break;
                case CameraShowSizeEnum._100p:
                    // 获取控件像素大小
                    var dpi = VisualTreeHelper.GetDpi(this.HSmartWindowControl);

                    if (dpi.DpiScaleX == 0 || dpi.DpiScaleY == 0)
                    {
                        return;
                    }

                    //ActualWidth / ActualHeight 不是物理像素 , 系统缩放是 125%（120 DPI）、150%（144 DPI）  ,相乘之后得到物理像素
                    int winPixelW = (int)(this.HSmartWindowControl.ActualWidth * dpi.DpiScaleX);
                    int winPixelH = (int)(this.HSmartWindowControl.ActualHeight * dpi.DpiScaleY);
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, winPixelH - 1, winPixelW - 1);

                    this.hWindow.DispObj(this.hImage);
                    break;
                case CameraShowSizeEnum._50p:
                    // 获取控件像素大小
                    dpi = VisualTreeHelper.GetDpi(this.HSmartWindowControl);

                    if (dpi.DpiScaleX == 0 || dpi.DpiScaleY == 0)
                    {
                        return;
                    }

                    //ActualWidth / ActualHeight 不是物理像素 , 系统缩放是 125%（120 DPI）、150%（144 DPI）  ,相乘之后得到物理像素
                    winPixelW = (int)(this.HSmartWindowControl.ActualWidth * dpi.DpiScaleX);
                    winPixelH = (int)(this.HSmartWindowControl.ActualHeight * dpi.DpiScaleY);
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, (winPixelH - 1) * 2, (winPixelW - 1) * 2);
                    this.hWindow.DispObj(this.hImage);

                    break;
                case CameraShowSizeEnum._25p:

                    // 获取控件像素大小
                    dpi = VisualTreeHelper.GetDpi(this.HSmartWindowControl);

                    if (dpi.DpiScaleX == 0 || dpi.DpiScaleY == 0)
                    {
                        return;
                    }

                    //ActualWidth / ActualHeight 不是物理像素 , 系统缩放是 125%（120 DPI）、150%（144 DPI）  ,相乘之后得到物理像素
                    winPixelW = (int)(this.HSmartWindowControl.ActualWidth * dpi.DpiScaleX);
                    winPixelH = (int)(this.HSmartWindowControl.ActualHeight * dpi.DpiScaleY);
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, (winPixelH - 1) * 4, (winPixelW - 1) * 4);

                    this.hWindow.DispObj(this.hImage);
                    break;
            }
        }
    }
}