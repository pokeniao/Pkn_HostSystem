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
        //全局的
        GlobalMannager.GlobalDictionary.TryGetValue("LogListBox", out object obj);
        HomePageModel = new HomePageModel()
        {
            LogListBox = (ObservableCollection<string>)obj,
        };
    }

    [RelayCommand]
    public void ScrollToBottom(ListBox LogListBox)
    {
        if (LogListBox.Items.Count == 0)
        {
            return;
        }

        LogListBox.ScrollIntoView(LogListBox.Items[^1]);
    }
}