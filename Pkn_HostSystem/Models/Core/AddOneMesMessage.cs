using CommunityToolkit.Mvvm.Messaging.Messages;
using LoadMesAddAndUpdateWindowModel = Pkn_HostSystem.Models.Windows.LoadMesAddAndUpdateWindowModel;

namespace Pkn_HostSystem.Models.Core;

/// <summary>
/// 添加一个Http消息的时候, 使用了MVVM消息体间通讯的方式
/// </summary>
public class AddOneMesMessage : ValueChangedMessage<LoadMesAddAndUpdateWindowModel>
{
    public AddOneMesMessage(LoadMesAddAndUpdateWindowModel value) : base(value)
    {
    }
}