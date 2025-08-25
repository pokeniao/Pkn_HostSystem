using CommunityToolkit.Mvvm.ComponentModel;
using HalconDotNet;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Editor;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class DesignViewModel : ObservableRecipient
    {


        public SnackbarService SnackbarService { get; set; }
        public LogControl<DesignViewModel> Log;

        #region 视觉参数

        //页面显示Control.
        public HalconControl HalconControl { get; set; } = new HalconControl();

        public HalconTool HalconTool { get; set; }

        public List<ComBoxEnumItem<CameraShowSizeEnum>> CameraShowMethodList { get; set; } = Enum
            .GetValues(typeof(CameraShowSizeEnum)).Cast<CameraShowSizeEnum>().Select(v =>
                new ComBoxEnumItem<CameraShowSizeEnum>(
                    )
                { Value = v, Display = v.GetDescription() }).ToList();

        #endregion

        #region Nodify

        public EditorViewModel EditorViewModel { get; set; }

        #endregion


        public DesignModel DesignModel { get; set; } = JsonTool<DesignModel>.Load();

        public DesignViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<DesignViewModel>(SnackbarService);
            HalconTool = new HalconTool(HalconControl);
            EditorViewModel = new EditorViewModel();
            if (DesignModel == null)
            {
                DesignModel = new DesignModel();
            }

            DesignModel.Nodes = new ObservableCollection<TreeNodes>()
            {
                new TreeNodes()
                {
                    Name = "Mes",
                    Children = new ObservableCollection<TreeNodes>()
                    {
                        new TreeNodes()
                        {
                            Name = "Http"
                        }
                    }
                }
            };
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