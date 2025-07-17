using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Models.Core
{
    public class GetHttpObject : ObservableObject
    {
        public string Name { get; set; }


        private string method = "常量";

        public string Method
        {
            get => method;
            set
            {
                SetProperty(ref method, value);
                OnPropertyChanged(nameof(ParamShow));
                OnPropertyChanged(nameof(showMethod1));
                OnPropertyChanged(nameof(showMethod2));
                OnPropertyChanged(nameof(showMethod3));
            }
        }
        /// <summary>
        /// 常量参数
        /// </summary>
        public string staticParam { get; set; }
        /// <summary>
        /// JSON解析字符串的参数
        /// </summary>
        public string JsonParam { get; set; }
        /// <summary>
        /// Json解析字符串返回参数
        /// </summary>
        public string JsonReturnValue { get; set; }
        /// <summary>
        /// 方法集
        /// </summary>
        public string ParamMethods { get; set; } = "暂无";
        /// <summary>
        /// 参数显示
        /// </summary>
        private string paramShow;
        public string ParamShow
        {
            get
            {
                string? value =null;
                switch (Method)
                {
                    case "常量":
                        value = $"{staticParam}";
                        break;
                    case "结果Json解析":
                        value = $"解析值:{JsonParam}";
                        break;
                    case "方法集":
                        value = $"{ParamMethods}";
                        break;

                }

                return value;
            }
        }
        public bool showMethod1 => Method == "常量";
        public bool showMethod2 => Method == "结果Json解析";
        public bool showMethod3 => Method == "方法集";
    }
}