using CommunityToolkit.Mvvm.Messaging.Messages;
using LoadMesAddAndUpdateWindowModel = Pkn_HostSystem.Models.Windows.LoadMesAddAndUpdateWindowModel;

namespace Pkn_HostSystem.Message;

public class MesMessage : ValueChangedMessage<Models.Windows.LoadMesAddAndUpdateWindowModel>
{
    public MesMessage(LoadMesAddAndUpdateWindowModel value) : base(value)
    {
    }
}