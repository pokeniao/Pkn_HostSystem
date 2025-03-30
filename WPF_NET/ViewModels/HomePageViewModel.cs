using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPF_NET.Base;
using WPF_NET.Models;
using WPF_NET.Static;

namespace WPF_NET.ViewModels;

public partial class HomePageViewModel : ObservableRecipient
{
    private LogBase<HomePageViewModel> log;

    public HomePageModel HomePageModel { get; set; }

    public HomePageViewModel()
    {
        log = new LogBase<HomePageViewModel>();

        GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out object obj);
        HomePageModel = new HomePageModel()
        {
            LogListBox = (ObservableCollection<string>)obj,
            LogListBox2 = new ObservableCollection<string>()
        };
    }

    [RelayCommand]
    public void ScrollToBottom(ListBox LogListBox)
    {
        LogListBox.ScrollIntoView(LogListBox.Items[^1]);
    }
}