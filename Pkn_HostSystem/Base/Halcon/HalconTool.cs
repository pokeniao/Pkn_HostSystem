using HalconDotNet;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pkn_HostSystem.Base.Halcon
{
    public class HalconTool
    {
        /// <summary>
        /// 触发/实时显示的信号量, 用于互锁
        /// </summary>
        private static SemaphoreSlim VisionSemaphoreSlim = new SemaphoreSlim(1);


        public HalconControl HalconControl { get; set; }


        public HalconTool(HalconControl halconControl)
        {
            HalconControl = halconControl;
        }

        public async Task<(bool succeed, string message)> VisionRealTime(bool run,
            CameraInterfaceEnum CameraInterfaceEnum, string param, CancellationTokenSource cts)
        {
            //等待获取锁
            bool waitAsync = await VisionSemaphoreSlim.WaitAsync(1000);
            if (!waitAsync)
            {
                return (false, $"[{TraceContext.Name}]--等待获取锁超时");
            }

            try
            {
                if (run)
                {
                    try
                    {
                        HalconControl.hv_AcqHandle = new HTuple();
                        HalconControl.hv_AcqHandle.Dispose();
                        // 启动相机
                        switch (CameraInterfaceEnum)
                        {
                            case CameraInterfaceEnum.图片:
                                if (param == null)
                                {
                                    return (false, "请先选择图片");
                                }

                                HOperatorSet.OpenFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1,
                                    "false",
                                    param, "default", 1, -1,
                                    out HalconControl.hv_AcqHandle);
                                break;
                            case CameraInterfaceEnum.GenICamTL:
                                if (param == null)
                                {
                                    return (false, "未选中相机,GenICamTL未填写");
                                }

                                HOperatorSet.OpenFramegrabber("GenICamTL", 0, 0, 0, 0, 0, 0, "progressive", -1,
                                    "default",
                                    -1,
                                    "false", "default", param, 0, -1,
                                    out HalconControl.hv_AcqHandle);
                                break;
                            case CameraInterfaceEnum.电脑摄像头:
                                HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1,
                                    "false",
                                    "default", "[0] ", 0, -1, out HalconControl.hv_AcqHandle);
                                break;
                        }

                        HOperatorSet.GrabImageStart(HalconControl.hv_AcqHandle, -1);
                        // 实时采集线程
                        Task.Run(() => ContinuesGrab(cts));
                        return (true, "相机连接成功,开始实时采集");
                    }
                    catch (Exception e)
                    {
                        // 释放
                        cts?.Cancel();
                        HOperatorSet.CloseFramegrabber(HalconControl.hv_AcqHandle);
                        HalconControl.hv_AcqHandle.Dispose();
                        return (false, $"{e}");
                    }
                }
                else
                {
                    // 释放
                    cts?.Cancel();
                    HOperatorSet.CloseFramegrabber(HalconControl.hv_AcqHandle);
                    HalconControl.hv_AcqHandle.Dispose();
                    return (true, "");
                }
            }
            finally
            {
                VisionSemaphoreSlim.Release(); // 释放信号量
            }
        }
        HObject hImage = new HObject();

        public async Task<(bool succeed, string message)> VisionTrigger(CameraInterfaceEnum CameraInterfaceEnum,
            string param)
        {
            bool waitAsync = await VisionSemaphoreSlim.WaitAsync(1000);

            if (!waitAsync)
            {
                return (false, $"[{TraceContext.Name}]--等待获取锁超时");
            }

            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        //将窗口赋值
                        HalconControl.hWindow = HalconControl.HSmartWindowControl.HalconWindow;
                        HalconControl.hv_AcqHandle = new HTuple();
                     
                        // HOperatorSet.GenEmptyObj(out HObject hImage);
                        HalconControl.hv_AcqHandle.Dispose();
                        // 启动相机,创建连接
                        //HalconControl.hv_AcqHandle连接句柄
                        switch (CameraInterfaceEnum)
                        {
                            case CameraInterfaceEnum.图片:
                                if (param == null)
                                {
                                    return (false, "请选择图片");
                                }

                                HOperatorSet.OpenFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1,
                                    "false",
                                    param, "default", 1, -1,
                                    out HalconControl.hv_AcqHandle);
                                break;
                            case CameraInterfaceEnum.GenICamTL:
                                if (param == null)
                                {
                                    return (false, "未选中相机,GenICamTL未填写");
                                }

                                HOperatorSet.OpenFramegrabber("GenICamTL", 0, 0, 0, 0, 0, 0, "progressive", -1,
                                    "default",
                                    -1,
                                    "false", "default", param, 0, -1,
                                    out HalconControl.hv_AcqHandle);
                                break;
                            case CameraInterfaceEnum.电脑摄像头:
                                HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1,
                                    "false",
                                    "default", "[0] ", 0, -1, out HalconControl.hv_AcqHandle);
                                break;
                        }

                        HOperatorSet.GrabImageStart(HalconControl.hv_AcqHandle, -1);

                        HalconControl.RemoveImage(hImage);
                        //获取图片
                        HOperatorSet.GrabImageAsync(out hImage,
                            HalconControl.hv_AcqHandle, -1);
                        HalconControl.ShowImage(hImage);
                        HalconControl.OnChangeCameraShowMethod();
                        return (true, "相机拍照完成");
                    }
                    finally
                    {
                        // 关闭摄像头
                        HOperatorSet.CloseFramegrabber(HalconControl.hv_AcqHandle);
                        HalconControl.hv_AcqHandle.Dispose();
                    }
                });
            }
            catch (Exception e)
            {
                return (false, $"{e}");
            }
            finally
            {
                VisionSemaphoreSlim.Release(); // 释放信号量
            }
        }


        /// <summary>
        /// 定义实时采集函数
        /// </summary>
        private async Task ContinuesGrab(CancellationTokenSource cts)
        {
            HalconControl.hWindow = HalconControl.HSmartWindowControl.HalconWindow;
            while (!cts.Token.IsCancellationRequested)
            {
                HalconControl.RemoveImage(hImage);
                HOperatorSet.GrabImageAsync(out hImage, HalconControl.hv_AcqHandle,
                    -1);
                HalconControl.ShowImage(hImage);

                // // 在新的线程里用如下方式更新到界面
                // _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                // {
                //     HalconControl.HSmartWindowControl.SetFullImagePart();
                // });
                await Task.Delay(100);
            }
        }
    }
}