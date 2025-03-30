using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF_Frame.Base;
using WPF_Frame.Models;

namespace WPF_Frame.ViewModels;

public class ModbusToolViewModel:ObservableRecipient
{
    public ModbusToolModel ModbusToolModel { get; set; }

    public ModbusToolViewModel()
    {
        ModbusToolModel = new ModbusToolModel();
        ModbusToolModel.ModbusTcp_Port = "123";
    }

}