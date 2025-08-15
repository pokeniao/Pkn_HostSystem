using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Pkn_HostSystem.Models.Core
{
    public partial class DynVerify:ObservableObject
    {
        //校验名
        public string Name { get; set; }

        private string type;

        public string Type
        {
            get => type;
            set
            {
                SetProperty(ref type, value);
                OnPropertyChanged(nameof(showValueTextBox));
                OnPropertyChanged(nameof(showValueCombox));
            }

        }

        public bool showValueTextBox => Type != "自定义复杂逻辑校验";

        public bool showValueCombox => Type == "自定义复杂逻辑校验";

        public string Value { get; set; }

        private Type complexValue;
        [JsonIgnore]
        public Type ComplexValue
        {
            get => complexValue;
            set
            {
                SetProperty(ref complexValue, value);
                Value = value.FullName;
            }
        }

    }
}