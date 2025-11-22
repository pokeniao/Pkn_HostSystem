using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Models.Pojo
{
    public partial class VOCStation : ObservableObject
    {
        //良率
        public object Yiele
        {

            get => StaticArrayRegister.ReadRegisterValue(2)+"%";
            set
            {
                StaticArrayRegister.WriteRegisterValue(2, value);
                OnPropertyChanged(nameof(Yiele));
            }
        }
        //入料数量
        public object EntryCount
        {

            get => StaticArrayRegister.ReadRegisterValue(3);
            set
            {
                StaticArrayRegister.WriteRegisterValue(3, value);
                OnPropertyChanged(nameof(EntryCount));
            }
        }

        //良品数量
        public object OkCount
        {

            get => StaticArrayRegister.ReadRegisterValue(4);
            set
            {
                StaticArrayRegister.WriteRegisterValue(4, value);
                OnPropertyChanged(nameof(OkCount));
            }
        }

        //不良品数量
        public object NgCount
        {

            get => StaticArrayRegister.ReadRegisterValue(5);
            set
            {
                StaticArrayRegister.WriteRegisterValue(5, value);
                OnPropertyChanged(nameof(NgCount));
            }
        }

        //运行时间H
        public object RunTimeH
        {

            get => StaticArrayRegister.ReadRegisterValue(6)+"h:";
            set
            {
                StaticArrayRegister.WriteRegisterValue(6, value);
                OnPropertyChanged(nameof(RunTimeH));
            }
        }
        //运行时间M
        public object RunTimeM
        {

            get => StaticArrayRegister.ReadRegisterValue(7) + "min";
            set
            {
                StaticArrayRegister.WriteRegisterValue(7, value);
                OnPropertyChanged(nameof(RunTimeM));
            }
        }
        //停机时间
        public object StopTimeH
        {

            get => StaticArrayRegister.ReadRegisterValue(8) + "h:";
            set
            {
                StaticArrayRegister.WriteRegisterValue(8, value);
                OnPropertyChanged(nameof(StopTimeH));
            }
        }
        public object StopTimeM
        {

            get => StaticArrayRegister.ReadRegisterValue(9) + "min";
            set
            {
                StaticArrayRegister.WriteRegisterValue(9, value);
                OnPropertyChanged(nameof(StopTimeM));
            }
        }

        //报警时间
        public object ErrTimeH
        {

            get => StaticArrayRegister.ReadRegisterValue(10) + "h:";
            set
            {
                StaticArrayRegister.WriteRegisterValue(10, value);
                OnPropertyChanged(nameof(ErrTimeH));
            }
        }
        public object ErrTimeM
        {

            get => StaticArrayRegister.ReadRegisterValue(11)+"min";
            set
            {
                StaticArrayRegister.WriteRegisterValue(11, value);
                OnPropertyChanged(nameof(ErrTimeM));
            }
        }
    }
}