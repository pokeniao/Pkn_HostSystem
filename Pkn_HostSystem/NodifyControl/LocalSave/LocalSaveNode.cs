using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.LocalSave
{
    public class LocalSaveNode
    {
        public string Id { get; set; }

        public NodeEnum NodeType { get; set; }

        public Point Location { get; set; }

        /// <summary>
        /// 保存节点的重要信息
        /// </summary>
        public List<LocalSaveConnector> Input { get; set; }
        /// <summary>
        /// 保存节点的重要信息
        /// </summary>
        public List<LocalSaveConnector> Output { get; set; }

        /// <summary>
        /// 保存当前节点的输入参数
        /// </summary>
        public List<OperationParamModel> InputParam { get; set; }

        /// <summary>
        /// 保存当前节点的输出参数
        /// </summary>
        public List<OperationParamModel> OutputParam { get; set; }

        /// <summary>
        /// 保存节点的Model
        /// </summary>
        public Object model { get; set; }
    }
}