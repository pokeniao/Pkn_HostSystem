using CommunityToolkit.Mvvm.Messaging.Messages;
using WPF_NET.Models;

namespace WPF_NET.Message;

public class MesMessage : ValueChangedMessage<LoadMesAddAndUpdateWindowModel>
{
    public MesMessage(LoadMesAddAndUpdateWindowModel value) : base(value)
    {
    }
}