using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_NET.Models;

public partial class LoadMesAddAndUpdateWindowModel : ObservableObject
{
    //动态绑定
    [ObservableProperty] private string name;
    [ObservableProperty] private string ajax;
    [ObservableProperty] private string httpPath;
    [ObservableProperty] private string api;
    [ObservableProperty] private int cycTime ;
    [ObservableProperty] private string requestMethod;
    [ObservableProperty] private string request;
    [ObservableProperty] private ObservableCollection<ConditionItem> condition;
    [ObservableProperty] private string response;
    [ObservableProperty] private bool runCyc;



    //令牌 控制线程开启关闭
    public CancellationTokenSource cts { get; set; }
    //任务
    public Lazy<Task> Task { get; set; }


    public override string ToString()
    {
        return string.Join(',', Condition.Select(c =>$"Key={c.Key} Value ={c.Value}"));
    }
}

