using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using HalconDotNet;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.LocalSave;
using Pkn_HostSystem.NodifyControl.Node.DesignTreeNode;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class DesignViewModel : ObservableRecipient
    {
        [ObservableProperty] private ObservableCollection<TreeNodes> nodes;

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

        [ObservableProperty] private EditorViewModel editorViewModel;

        #endregion


        public DesignModel DesignModel { get; set; }

        /// <summary>
        /// 用于显示
        /// </summary>
        [ObservableProperty] private string projectName;


        public ProjectModel ProjectModel { get; set; } =  LocalSaveNodifyMethod.Load();
        public DesignViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<DesignViewModel>(SnackbarService);
            HalconTool = new HalconTool(HalconControl);

            if (ProjectModel == null)
            {
                ProjectModel = new ProjectModel();
            }
            GlobalManager.ProjectDictionary.AddOrUpdate(ProjectModel.ProjectList);
            GlobalManager.ProjectDictionary.Connect().Bind(ProjectModel.ProjectList).Subscribe(); //绑定



        }

        /// <summary>
        /// 需要更具项目单独进行初始化
        /// </summary>
        /// <param name="DesignModel"></param>
        public void init()
        {
            EditorViewModel = DesignModel.EditorViewModel;
            ProjectName = DesignModel.ProjectName;
            Nodes = DesignTreeNode.TreeNodesList;
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
        [RelayCommand]
        public void Save()
        {
            //保存当前的节点和连接线

            LocalSaveNodifyMethod.Save();
            JsonTool<ProjectModel>.Save(ProjectModel);
        }
    }
}