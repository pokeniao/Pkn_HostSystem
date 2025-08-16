using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Models.Core
{
    public partial class RegisterItem : ObservableObject
    {
        [ObservableProperty] private string name;

        private readonly int _index; // 在原集合中的索引
        public int Index => _index;
        [JsonIgnore]
        public object Value
        {

            get => StaticArrayRegister.ReadRegisterValue(_index);
            set
            {
                StaticArrayRegister.WriteRegisterValue(_index , value);
                OnPropertyChanged(nameof(Value));
            }
        }

        public RegisterItem(int Index)
        {
            _index = Index;
        }
    }
}