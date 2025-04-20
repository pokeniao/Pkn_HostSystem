using CommunityToolkit.Mvvm.ComponentModel;

namespace Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;

public class LoadMesCondition : ObservableObject
{
    /// <summary>
    /// 当前名称
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 选择请求方式类型
    /// </summary>
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

    /// <summary>
    /// 控制参数显示内容
    /// </summary>
    public bool show_para_dyn => method == "动态获取";
    public bool show_para_static => method == "常量";
    public bool show_method => method == "方法集";

    /// <summary>
    /// 动态参数
    /// </summary>
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
    /// <summary>
    /// 静态参数
    /// </summary>
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

    /// <summary>
    /// 方法集
    /// </summary>
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

    /// <summary>
    /// 显示参数值
    /// </summary>
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

    /// <summary>
    /// 判断是否是新行
    /// </summary>
    public bool IsNewLine { get; set; }
}