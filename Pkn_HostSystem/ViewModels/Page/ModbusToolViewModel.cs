using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Views.Pages;
using System.IO.Ports;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page;

public partial class ModbusToolViewModel : ObservableRecipient
{
    public LogBase<ModbusToolViewModel> Log { get; set; }
    public ModbusToolModel ModbusToolModel { get; set; } = new();

    public ModbusBase ModbusBase { get; set; }

    public ISnackbarService SnackbarService { get; set; }

    public ModbusToolViewModel()
    {
        ModbusBase = new ModbusBase();
        SnackbarService = new SnackbarService();
        Log = new LogBase<ModbusToolViewModel>(SnackbarService);
    }

    public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
    {
        SnackbarService.SetSnackbarPresenter(snackbarPresenter);
    }

    #region 连接和断开

    /// <summary>
    /// 连接TCP
    /// </summary>
    [RelayCommand]
    private async Task ConnectTcp(ModbusToolPage page)
    {
        if (ModbusBase.IsTCPConnect() || ModbusBase.IsRTUConnect())
        {
            Log.WarningAndShow("已连接,请勿重复连接");
            return;
        }
        else
        {
            await ModbusBase.OpenTcpMaster(ModbusToolModel.ModbusTcp_Ip_select, ModbusToolModel.ModbusTcp_Port);
        }

        if (ModbusBase.IsTCPConnect())
        {
            Log.SuccessAndShow("连接成功");
            page.Border_Connect.Visibility = Visibility.Visible;
            page.Border_Close.Visibility = Visibility.Collapsed;
        }
        else
        {
            Log.WarningAndShow("连接失败,请检查IP端口号");
            page.Border_Close.Visibility = Visibility.Visible;
            page.Border_Connect.Visibility = Visibility.Collapsed;
        }
    }

    [RelayCommand]
    private async Task ConnectRTU(ModbusToolPage page)
    {
        if (ModbusBase.IsTCPConnect() || ModbusBase.IsRTUConnect())
        {
            Log.WarningAndShow("已连接,请勿重复连接");
            return;
        }
        else
        {
            await ModbusBase.OpenRTUMaster(ModbusToolModel.ModbusRtu_COM_select,
                int.Parse((string)ModbusToolModel.ModbusRtu_baudRate_select),
                int.Parse((string)ModbusToolModel.ModbusRtu_dataBits_select),
                ModbusToolModel.ModbusRtu_stopBits_select, ModbusToolModel.ModbusRtu_parity_select);
        }

        if (ModbusBase.IsRTUConnect())
        {
            Log.SuccessAndShow("连接成功");
            page.Border_Connect.Visibility = Visibility.Visible;
            page.Border_Close.Visibility = Visibility.Collapsed;
        }
        else
        {
            Log.WarningAndShow("连接失败,请检查设置");
            page.Border_Close.Visibility = Visibility.Visible;
            page.Border_Connect.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    [RelayCommand]
    private void Close(ModbusToolPage page)
    {
        ModbusBase.CloseTCP();
        ModbusBase.CloseRTU();
        Log.SuccessAndShow("连接断开");
        page.Border_Close.Visibility = Visibility.Visible;
        page.Border_Connect.Visibility = Visibility.Collapsed;
    }

    #endregion

    #region 发送

    [RelayCommand]
    public async Task SendModbus()
    {
        // { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" };
        switch (ModbusToolModel.FuntionCode_select)
        {
            case "01读线圈":
                bool[] coils01 = null;
                try
                {
                    coils01 = await ModbusBase.ReadCoils_01((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress, (ushort)ModbusToolModel.ReadCount);
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                    break;
                }

                if (coils01 != null) readDGV(coils01);
                Log.SuccessAndShow("01读线圈读取成功");
                break;
            case "02读输入状态":
                bool[] inputs02 = null;
                try
                {
                    inputs02 = await ModbusBase.ReadInputs_02((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress, (ushort)ModbusToolModel.ReadCount);
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                    break;
                }

                if (inputs02 != null) readDGV(inputs02);
                Log.SuccessAndShow("02读输入状态读取成功");
                break;
            case "03读保持寄存器":
                ushort[] holdingRegisters03 = null;
                try
                {
                    holdingRegisters03 = await ModbusBase.ReadHoldingRegisters_03((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress, (ushort)ModbusToolModel.ReadCount);
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                    break;
                }

                if (holdingRegisters03 != null) readDGV(holdingRegisters03);
                Log.SuccessAndShow("03读保持寄存器读取成功");
                break;
            case "04读输入寄存器":
                ushort[] readInputRegisters04 = null;
                try
                {
                    readInputRegisters04 = await ModbusBase.ReadInputRegisters_04((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress, (ushort)ModbusToolModel.ReadCount);
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                    break;
                }

                if (readInputRegisters04 != null) readDGV(readInputRegisters04);
                Log.SuccessAndShow("04读输入寄存器读取成功");
                break;
            case "05写单线圈":
                try
                {
                    await ModbusBase.WriteCoil_05((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress,
                        (bool)ModbusToolModel.WriteDvgList[0].value);
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                }


                Log.SuccessAndShow("05写单线圈成功");
                break;
            case "06写单寄存器":
                try
                {
                    await ModbusBase.WriteRegister_06((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress,
                        ushort.Parse((string)ModbusToolModel.WriteDvgList[0].value.ToString()));
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                }


                Log.SuccessAndShow("06写单寄存器成功");
                break;
            case "0F写多线圈":
                try
                {
                    var coils = new List<bool>();
                    foreach (var modbusPojo in Enumerable.ToArray<ModbusToolPojo<object>>(ModbusToolModel.WriteDvgList))
                        coils.Add((bool)modbusPojo.value);

                    await ModbusBase.WriteCoils_0F((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress,
                        coils.ToArray()
                    );
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                }

                Log.SuccessAndShow("0F写多线圈成功");
                break;
            case "10写多寄存器":
                var registers = new List<ushort>();
                try
                {
                    foreach (ModbusToolPojo<object> modbusPojo in Enumerable.ToArray<ModbusToolPojo<object>>(ModbusToolModel.WriteDvgList))
                        registers.Add(ushort.Parse(modbusPojo.value.ToString()));

                    await ModbusBase.WriteRegisters_10((byte)ModbusToolModel.SlaveAddress,
                        (ushort)ModbusToolModel.StartAddress, registers.ToArray()
                    );
                }
                catch (Exception exception)
                {
                    Log.ErrorAndShow($"读取发生错误:{exception.Message}");
                }

                Log.SuccessAndShow("10写多寄存器成功");
                break;
        }
    }

    #region 显示读DGV

    public void readDGV<T>(T[] value)
    {
        var address = (int)ModbusToolModel.StartAddress;
        var modbusPojos = value.Select((b, index) => new ModbusToolPojo<object>
        { address = address++, value = b }).ToList();
        ModbusToolModel.ReadDvgList = modbusPojos;
    }

    #endregion

    #endregion
}