using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.LocalSave;
using System.Windows.Documents;

namespace Pkn_HostSystem.Models.Page
{
    public partial class DesignModel : ObservableObject
    {
        /// <summary>
        ///当前项目名称
        /// </summary>
        [ObservableProperty] private string projectName;

        /// <summary>
        /// 执行任务
        /// </summary>

        [ObservableProperty] private bool runTask;
        /// <summary>
        /// 项目实体对象
        /// </summary>

        [JsonIgnore] public EditorViewModel EditorViewModel { get; set; } = new EditorViewModel();

        /// <summary>
        /// 令牌 循环进程任务
        /// </summary>
        [JsonIgnore]
        public CancellationTokenSource cts { get; set; }

        /// <summary>
        /// 当前Http进程任务
        /// </summary>
        [JsonIgnore]
        public Lazy<Task> Task { get; set; }

        /// <summary>
        /// 当前项目的日志
        /// </summary>
        [JsonIgnore] public LogControl<DesignModel> Log = new LogControl<DesignModel>(new FlowDocument());


        /// <summary>
        /// 项目保存读取对象,用于本地保存
        /// </summary>
        public LocalSaveNodify LocalSaveNodify { get; set; } = new LocalSaveNodify();



    }
}