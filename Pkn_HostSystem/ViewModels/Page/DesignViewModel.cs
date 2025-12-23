using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using HalconDotNet;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Halcon;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.NodifyControl.LocalSave.Services;
using Pkn_HostSystem.NodifyControl.Nodes.Core;
using Pkn_HostSystem.NodifyControl.ViewModels.Editor;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
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


        public ProjectModel ProjectModel { get; set; }
        public DesignViewModel()
        {
            SnackbarService = new SnackbarService();
            HalconTool = new HalconTool(HalconControl);
            ProjectModel = LocalSaveNodifyMethod.Load();
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
            //改变Notify的 EditorViewModel
            EditorViewModel = DesignModel.EditorViewModel;
            //赋值项目名
            ProjectName = DesignModel.ProjectName;
            //赋值树状节点
            Nodes = DesignTreeNode.TreeNodesList;
            //赋值日志记录
            DesignPage designPage = Ioc.Default.GetRequiredService<DesignPage>();
            designPage.LogRichTextBox.Document = DesignModel.Log.FlowDocument;
            designPage.LogRichTextBox.Document.FontSize = 9;

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

        public void SetRichTextBox(RichTextBox richTextBox)
        {
            if (DesignModel != null)
            {
                DesignModel.Log.RichTextBox = richTextBox;
            }
        }
    }
}