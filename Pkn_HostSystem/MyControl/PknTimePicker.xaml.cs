using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Pkn_HostSystem.MyControl
{
    /// <summary>
    /// PknTimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class PknTimePicker : UserControl
    {
        public PknTimePicker()
        {
            InitializeComponent();

            // 初始化小时和分钟
            for (int i = 0; i < 24; i++) Hours.Add(i);
            for (int i = 0; i < 60; i++) Minutes.Add(i);
        }

        // 小时/分钟列表
        public ObservableCollection<int> Hours { get; } = new();
        public ObservableCollection<int> Minutes { get; } = new();


        // ------------------- Start Hour -------------------
        public int StartSelectedHour
        {
            get => (int)GetValue(StartSelectedHourProperty);
            set => SetValue(StartSelectedHourProperty, value);
        }

        public static readonly DependencyProperty StartSelectedHourProperty =
            DependencyProperty.Register(
                nameof(StartSelectedHour),
                typeof(int),
                typeof(PknTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // ------------------- Start Minute -------------------
        public int StartSelectedMinute
        {
            get => (int)GetValue(StartSelectedMinuteProperty);
            set => SetValue(StartSelectedMinuteProperty, value);
        }

        public static readonly DependencyProperty StartSelectedMinuteProperty =
            DependencyProperty.Register(
                nameof(StartSelectedMinute),
                typeof(int),
                typeof(PknTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // ------------------- End Hour -------------------
        public int EndSelectedHour
        {
            get => (int)GetValue(EndSelectedHourProperty);
            set => SetValue(EndSelectedHourProperty, value);
        }

        public static readonly DependencyProperty EndSelectedHourProperty =
            DependencyProperty.Register(
                nameof(EndSelectedHour),
                typeof(int),
                typeof(PknTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // ------------------- End Minute -------------------
        public int EndSelectedMinute
        {
            get => (int)GetValue(EndSelectedMinuteProperty);
            set => SetValue(EndSelectedMinuteProperty, value);
        }

        public static readonly DependencyProperty EndSelectedMinuteProperty =
            DependencyProperty.Register(
                nameof(EndSelectedMinute),
                typeof(int),
                typeof(PknTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
