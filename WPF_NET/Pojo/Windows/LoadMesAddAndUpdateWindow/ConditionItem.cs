using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_NET.Models;

public class ConditionItem : ObservableObject
{
    public string Key { get; set; }


    private string method = "常量";

    public string Method
    {
        get => method;
        set
        {
            SetProperty(ref method, value);
            OnPropertyChanged(nameof(show_para_dyn)); // nameof 获取属性名
            OnPropertyChanged(nameof(show_para_static));
            OnPropertyChanged(nameof(show_method));
            OnPropertyChanged(nameof(Value));
        }
    }

    public bool show_para_dyn => method == "动态获取";
    public bool show_para_static => method == "常量";

    public bool show_method => method == "方法集";

    private string dyn_Value;

    public string Dyn_Value
    {
        get => dyn_Value;
        set
        {
            SetProperty(ref dyn_Value, value);
            OnPropertyChanged(nameof(Value));
        }
    }

    private string static_Value;

    public string Static_Value
    {
        get => static_Value;
        set
        {
            SetProperty(ref static_Value, value);
            OnPropertyChanged(nameof(Value));
        }
    }

    private string method_value;

    public string Method_value
    {
        get => method_value;
        set
        {
            SetProperty(ref method_value, value);
            OnPropertyChanged(nameof(Value));
        }
    }

    public string Value
    {
        get
        {
            return method switch
            {
                "常量" => static_Value,
                "动态获取" => dyn_Value,
                "方法集" => method_value,
            };
        }
    }


    public bool IsNewLine { get; set; }

    // 必须添加无参构造函数
    public ConditionItem()
    {
    }
}