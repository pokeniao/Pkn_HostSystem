using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_NET.Models;

public partial class LoadMesAddAndUpdateWindowModel : ObservableObject
{
    [ObservableProperty] private string name;
    [ObservableProperty] private string ajax;
    [ObservableProperty] private string httpPath;
    [ObservableProperty] private string api;
    [ObservableProperty] private string request;
    [ObservableProperty] private ObservableCollection<ConditionItem> condition;
    [ObservableProperty] private string response;

    public override string ToString()
    {
        return string.Join(',', Condition.Select(c =>$"Key={c.Key} Value ={c.Value}"));
    }
}

public class ConditionItem
{
    public string Key { get; set; }
    public string Value { get; set; } 
    // 必须添加无参构造函数
    public ConditionItem() { }

    public ConditionItem(string key, string value)
    {
        Key = key;
        Value = value;
    }
}