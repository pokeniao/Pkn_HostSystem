using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WPF_NET.Models;

public partial class HomePageModel : ObservableObject
{
    
    [ObservableProperty] private ObservableCollection<string> logListBox;


    [ObservableProperty] public ObservableCollection<string> logListBox2;

}