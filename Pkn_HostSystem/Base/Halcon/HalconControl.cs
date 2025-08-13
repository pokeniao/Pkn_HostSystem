using DynamicData;
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
        public List<HObject> hImages = new List<HObject>();

        /// <summary>
        /// 相机句柄
        /// </summary>
        public HTuple hv_AcqHandle;


        private CameraShowSizeEnum _cameraShowSize = CameraShowSizeEnum.适应窗口模式;

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
        /// <summary>
        /// 展示图片
        /// </summary>
        /// <param name="hImage"></param>
        public void ShowImage(HObject hImage)
        {
            HOperatorSet.DispObj(hImage, hWindow);

            if (hImages.IndexOf(hImage) == -1)
            {
                hImages.Add(hImage);
            }
        }

        public void RemoveImage(HObject hImage)
        {
            if (hImages.Remove(hImage))
                hImage.Dispose();
        }

        public void ClearImages()
        {
            foreach (HObject hImage in hImages)
            {
                hImage.Dispose();
            }

            hImages.Clear();
        }


        public void OnChangeCameraShowMethod()
        {
            if (this.hImages.Count == 0)
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

            // 获取控件像素大小
            var dpi = VisualTreeHelper.GetDpi(this.HSmartWindowControl);

            if (dpi.DpiScaleX == 0 || dpi.DpiScaleY == 0)
            {
                return;
            }

            //ActualWidth / ActualHeight 不是物理像素 , 系统缩放是 125%（120 DPI）、150%（144 DPI）  ,相乘之后得到物理像素
            int winPixelW = (int)(this.HSmartWindowControl.ActualWidth * dpi.DpiScaleX);
            int winPixelH = (int)(this.HSmartWindowControl.ActualHeight * dpi.DpiScaleY);

            switch (this.CameraShowSize)
            {
                case CameraShowSizeEnum.适应窗口模式:

                    HTuple maxWidth = 0;
                    HTuple maxHeight = 0;
                    foreach (var image in hImages)
                    {
                        HOperatorSet.GetObjClass(image, out HTuple val);
                        //判断是否是图片 ，而不是轮廓
                        if (val == "image")
                        {
                            //获取图片尺寸
                            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);

                            if (maxWidth < width)
                            {
                                maxWidth = width;
                            }

                            if (maxHeight < height)
                            {
                                maxHeight = height;
                            }
                        }
                    }

                    //计算缩放
                    HTuple w = (double)maxWidth / (double)winPixelW;
                    HTuple h = (double)maxHeight / (double)winPixelH;
                    HTuple max = w > h ? w : h;
                    HTuple wOffice = 0;
                    HTuple hOffice = 0;


                    //计算居中偏移
                    if (winPixelW * max > maxWidth)
                    {
                        //需要宽度偏移
                        wOffice = (winPixelW * max - maxWidth) / 2;
                    }

                    if (winPixelH * max > maxHeight)
                    {
                        hOffice = (winPixelH * max - maxHeight) / 2;
                    }

                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, -hOffice, -wOffice, winPixelH * max - hOffice - 1,
                        winPixelW * max - wOffice - 1);

                    foreach (var image in hImages)
                    {
                        hWindow.DispObj(image);
                    }

                    break;
                case CameraShowSizeEnum._100p:
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, winPixelH - 1, winPixelW - 1);
                    foreach (var image in hImages)
                    {
                        hWindow.DispObj(image);
                    }

                    break;
                case CameraShowSizeEnum._50p:
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, (winPixelH - 1) * 2, (winPixelW - 1) * 2);
                    foreach (var image in hImages)
                    {
                        hWindow.DispObj(image);
                    }

                    break;
                case CameraShowSizeEnum._25p:
                    //设置活跃的图像处理区域，即告诉后续操作只在图像的某个矩形区域内进行(y 起点,x 起点,y 终点,x 终点)
                    HOperatorSet.SetPart(this.hWindow, 0, 0, (winPixelH - 1) * 4, (winPixelW - 1) * 4);
                    foreach (var image in hImages)
                    {
                        hWindow.DispObj(image);
                    }

                    break;
            }
        }
    }
}