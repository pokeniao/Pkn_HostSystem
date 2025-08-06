using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using System.Text;


namespace Pkn_HostSystem.Models.Page;

public partial class MesTcpModel : ObservableObject
{



    #region 需要保存

    /// <summary>
    /// 动态嵌入内容数据,列表
    /// </summary>
    [ObservableProperty] private ObservableCollectionExtended<LoadMesDynContent> dynNetList;

    #endregion




    #region 不需要保存
    /// <summary>
    /// 用于操作当前连接
    /// </summary>
    private ObservableCollectionExtended<NetWork> netWorkList = new ObservableCollectionExtended<NetWork>();
    [JsonIgnore]
    public ObservableCollectionExtended<NetWork> NetWorkList
    {
        get => netWorkList;
        set
        {
            SetProperty(ref netWorkList, value);
        }

    }

    /// <summary>
    /// 用于操作Http连接
    /// </summary>
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> httpList = new ObservableCollectionExtended<LoadMesAddAndUpdateWindowModel>();
    [JsonIgnore]
    public ObservableCollection<LoadMesAddAndUpdateWindowModel> HttpList
    {
        get => httpList;
        set
        {
            SetProperty(ref httpList, value);
        }

    }

    #region 用于点击后显示页面的bool

    /// <summary>
    /// 用于显示设置的页面
    /// </summary>
    private bool veritySet;

    [JsonIgnore]
    public bool VeritySet
    {
        get => veritySet;
        set
        {
            SetProperty(ref veritySet, value);
        }
    }

    /// <summary>
    /// 用于显示设置的页面
    /// </summary>
    private bool httpSet;

    [JsonIgnore]
    public bool HttpSet
    {
        get => httpSet;
        set
        {
            SetProperty(ref httpSet, value);
        }
    }

    /// <summary>
    /// 用于显示switch页面
    /// </summary>
    private bool showSwitchSet;

    [JsonIgnore]
    public bool ShowSwitchSet
    {
        get => showSwitchSet;
        set
        {
            SetProperty(ref showSwitchSet, value);
        }
    }

    #endregion
    /// <summary>
    /// 显示 选中一行的动态嵌入内容数据
    /// </summary>
    private ObservableCollection<DynCondition> dynConditionItemList;
    [JsonIgnore]
    public ObservableCollection<DynCondition> DynConditionItemList
    {
        get => dynConditionItemList;
        set
        {
            SetProperty(ref dynConditionItemList, value);
        }

    }
    private string message;
    [JsonIgnore]
    public string Message
    {
        get => message;
        set
        {
            SetProperty(ref message, value);
        }

    }
    /// <summary>
    /// Modbus,基恩士,功能码的列表
    /// </summary>
    private List<string> functionCodeList;
    [JsonIgnore]
    public List<string> FunctionCodeList
    {
        get => functionCodeList;
        set
        {
            SetProperty(ref functionCodeList, value);
        }

    }
    /// <summary>
    /// 读取消息的方式,用于动态显示,如读写线圈等
    /// </summary>
    private ObservableCollection<string> methodName;
    [JsonIgnore]
    public ObservableCollection<string> MethodName
    {
        get => methodName;
        set
        {
            SetProperty(ref methodName, value);
        }

    }
    /// <summary>
    /// 写入内部返回地址
    /// </summary>
    private STRING interiorReturnMessage;
    [JsonIgnore]
    public STRING InteriorReturnMessage
    {
        get => interiorReturnMessage;
        set
        {
            SetProperty(ref interiorReturnMessage, value);
        }
    }
    /// <summary>
    /// 用于设置UniformGrid的行列
    /// </summary>
    private int setRows = 1;

    public int SetRows
    {
        get => setRows;
        set
        {
            SetProperty(ref setRows, value);
        }

    }
    private int setColumns = 1;

    public int SetColumns
    {
        get => setColumns;
        set
        {
            SetProperty(ref setColumns, value);
        }
    }



    /// <summary>
    /// Tcp服务器连接客户端的名称,用于显示已连接客户端
    /// </summary>
    private ObservableCollection<string> tcpServerConnectionClint;
    [JsonIgnore]
    public ObservableCollection<string> TcpServerConnectionClint
    {
        get => tcpServerConnectionClint;
        set
        {
            SetProperty(ref tcpServerConnectionClint, value);
        }

    }

    /// <summary>
    /// 当前在设置的名称
    /// </summary>
    private string setSwitchSetName;
    [JsonIgnore]
    public string SetSwitchSetName
    {
        get => setSwitchSetName;
        set
        {
            SetProperty(ref setSwitchSetName, value);
        }

    }

    /// <summary>
    /// 用于显示当前选中的Switch属性
    /// </summary>
    private ObservableCollection<DynSwitch> switchList;
    [JsonIgnore]
    public ObservableCollection<DynSwitch> SwitchList
    {
        get => switchList;
        set
        {
            SetProperty(ref switchList, value);
        }

    }

    /// <summary>
    /// 当前在设置的名称
    /// </summary>
    private string setVeritySetName;
    [JsonIgnore]
    public string SetVeritySetName
    {
        get => setVeritySetName;
        set
        {
            SetProperty(ref setVeritySetName, value);
        }

    }

    /// <summary>
    /// 用于显示校验的配置列表
    /// </summary>
    private ObservableCollection<DynVerify> verifyList;
    [JsonIgnore]
    public ObservableCollection<DynVerify> VerifyList
    {
        get => verifyList;
        set
        {
            SetProperty(ref verifyList, value);
        }

    }

    /// <summary>
    /// 用于显示当前转发的页面名字
    /// </summary>
    private string transpondSetName;
    [JsonIgnore]
    public string TranspondSetName
    {
        get => transpondSetName;
        set
        {
            SetProperty(ref transpondSetName, value);
        }

    }


    /// <summary>
    /// 用于显示当前转发的连接名
    /// </summary>
    private TranspondModbusDetailed transpondModbusDetailed;
    [JsonIgnore]
    public TranspondModbusDetailed TranspondModbusDetailed
    {
        get => transpondModbusDetailed;
        set
        {
            SetProperty(ref transpondModbusDetailed, value);
        }

    }

    /// <summary>
    /// 用于显示当前转发的页面
    /// </summary>
    private bool transpondSet;
    [JsonIgnore]
    public bool TranspondSet
    {
        get => transpondSet;
        set
        {
            SetProperty(ref transpondSet, value);
        }
    }


    /// <summary>
    /// 用于显示Http映射的值的配置
    /// </summary>
    private ObservableCollection<GetHttpObject> httpObjects;
    [JsonIgnore]
    public ObservableCollection<GetHttpObject> HttpObjects
    {
        get => httpObjects;
        set
        {
            SetProperty(ref httpObjects, value);
        }

    }

    /// <summary>
    /// 当前在设置的名称
    /// </summary>
    private string setHttpObjectName;
    [JsonIgnore]
    public string SetHttpObjectName
    {
        get => setHttpObjectName;
        set
        {
            SetProperty(ref setHttpObjectName, value);
        }

    }
    #endregion




}