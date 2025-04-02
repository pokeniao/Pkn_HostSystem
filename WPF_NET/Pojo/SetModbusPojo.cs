using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_NET.Pojo;

public class SetModbusPojo :ObservableObject
{
    public string Name { get; set; }

    public int StartAddress { get; set; }
    public int EndAddress { get; set; }
}