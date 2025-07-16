using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text;

namespace Pkn_HostSystem.Models.Core
{
    public class STRING : ObservableObject
    {
        private StringBuilder _value = new StringBuilder();
        public string Value
        {
            get => _value.ToString();
            set
            {
                if (_value.ToString() != value)
                {
                    SetProperty(ref _value, _value.Clear().Append(value));
                }
            }
        }
    }
}