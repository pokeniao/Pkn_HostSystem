using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection.Metadata;

namespace Pkn_HostSystem.Models.Core
{
    public partial class JdbcUrl :ObservableObject
    {
        [ObservableProperty] private string server;
        [ObservableProperty] private string dataBase;
        [ObservableProperty] private string uid;
        [ObservableProperty] private string pwd;
    }
}