using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Base.Log.Interface;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Documents;

namespace Pkn_HostSystem.Models.Core
{
    //由于 C# 接口实现必须严格匹配接口的返回类型，而你使用的 [ObservableProperty] 属性生成器没有自动实现接口成员
    public partial class EachStation<T> :ObservableObject,IEachStation
    {
        public string Header { get; set; } //Tab标题

        [ObservableProperty] private ObservableCollection<T> items =new();  // DataGrid数据
        [ObservableProperty] private LogControl<T> userLog = new(new FlowDocument());// 富文本的LogDocument , 用户查看日志
        [ObservableProperty] private LogControl<T> errorLog = new(new FlowDocument()); // 错误日志
        [ObservableProperty] private LogControl<T> devLog = new(new FlowDocument()); // 开发者查看日志

        /// <summary>
        /// DataGrid的添加方法
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(T item)
        {
            if (Items.Count >= 100)
            {
                Items.RemoveAt(0); // 删除最早的
              
            }
            Items.Add(item);
        }


        // 👇👇👇 显式实现接口成员 👇👇👇
        ObservableCollection<object> IEachStation.Items
        {
            get => Items as ObservableCollection<object>;
            set => Items = new ObservableCollection<T>(value.Cast<T>());
        }
        //set 这里做了向下转型,因为你只能保证接口传进来的 value 是 ILogControl，不能保证一定是 LogControl<T>，所以需要强制转换。
        ILogControl IEachStation.UserLog
        {
            get => UserLog;
            set => UserLog = (LogControl<T>)value;
        }
        
        ILogControl IEachStation.ErrorLog
        {
            get => ErrorLog;
            set => ErrorLog = (LogControl<T>)value;
        }
        
        ILogControl IEachStation.DevLog
        {
            get => DevLog;
            set => DevLog = (LogControl<T>)value;
        }
    }
}