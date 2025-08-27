using CommunityToolkit.Mvvm.ComponentModel;
using HalconDotNet;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.Node.DesignTreeNode;
using System.Windows.Documents;
using Wpf.Ui;
using Wpf.Ui.Controls;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class DesignViewModel : ObservableRecipient
    {


        public SnackbarService SnackbarService { get; set; }
        public static LogControl<DesignViewModel> Log;

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

            DesignModel.Nodes = DesignTreeNode.TreeNodesList;
        }


        public void SetLogRichTextBox(RichTextBox richTextBox)
        {
            Log.RichTextBox = richTextBox;
            Log.FlowDocument = richTextBox.Document;

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