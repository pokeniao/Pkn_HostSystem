using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.Views.UserControls.TimePicker
{
    /// <summary>
    /// PknDateTimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class PknDateTimePicker : System.Windows.Controls.UserControl
    {
        public PknDateTimePicker()
        {
            InitializeComponent();

            // 初始化小时和分钟
            for (int i = 0; i < 24; i++) Hours.Add(i);
            for (int i = 0; i < 60; i++) Minutes.Add(i);
        }

        // Hour/Minute 列表（只读属性，不用依赖属性）
        public ObservableCollection<int> Hours { get; } = new();
        public ObservableCollection<int> Minutes { get; } = new();

        #region Start 时间

        public DateTime? StartSelectedDate
        {
            get => (DateTime?)GetValue(StartSelectedDateProperty);
            set => SetValue(StartSelectedDateProperty, value);
        }

        public static readonly DependencyProperty StartSelectedDateProperty =
            DependencyProperty.Register(
                nameof(StartSelectedDate), typeof(DateTime?), typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateTimeChanged));

        public int StartSelectedHour
        {
            get => (int)GetValue(StartSelectedHourProperty);
            set => SetValue(StartSelectedHourProperty, value);
        }

        public static readonly DependencyProperty StartSelectedHourProperty =
            DependencyProperty.Register(
                nameof(StartSelectedHour), typeof(int), typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateTimeChanged));


        public int StartSelectedMinute
        {
            get => (int)GetValue(StartSelectedMinuteProperty);
            set => SetValue(StartSelectedMinuteProperty, value);
        }

        public static readonly DependencyProperty StartSelectedMinuteProperty =
            DependencyProperty.Register(
                nameof(StartSelectedMinute), typeof(int), typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateTimeChanged));

        #endregion


        #region End 时间

        public DateTime? EndSelectedDate
        {
            get => (DateTime?)GetValue(EndSelectedDateProperty);
            set => SetValue(EndSelectedDateProperty, value);
        }

        public static readonly DependencyProperty EndSelectedDateProperty =
            DependencyProperty.Register(nameof(EndSelectedDate), typeof(DateTime?), typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateTimeChanged));




        public int EndSelectedHour
        {
            get => (int)GetValue(EndSelectedHourProperty);
            set => SetValue(EndSelectedHourProperty, value);
        }

        public static readonly DependencyProperty EndSelectedHourProperty =
            DependencyProperty.Register(nameof(EndSelectedHour), typeof(int), typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnDateTimeChanged));




        public int EndSelectedMinute
        {
            get => (int)GetValue(EndSelectedMinuteProperty);
            set => SetValue(EndSelectedMinuteProperty, value);
        }

        public static readonly DependencyProperty EndSelectedMinuteProperty =
            DependencyProperty.Register(nameof(EndSelectedMinute), typeof(int), typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnDateTimeChanged));



        #endregion


        #region 组合完整 DateTime (只读依赖属性)

        public DateTime? StartSelectedDateTime
        {
            get => (DateTime?)GetValue(StartSelectedDateTimeProperty);
            private set => SetValue(StartSelectedDateTimePropertyKey, value);
        }

        private static readonly DependencyPropertyKey StartSelectedDateTimePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(StartSelectedDateTime), typeof(DateTime?),
                typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty StartSelectedDateTimeProperty =
            StartSelectedDateTimePropertyKey.DependencyProperty;

        public DateTime? EndSelectedDateTime
        {
            get => (DateTime?)GetValue(EndSelectedDateTimeProperty);
            private set => SetValue(EndSelectedDateTimePropertyKey, value);
        }

        private static readonly DependencyPropertyKey EndSelectedDateTimePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(EndSelectedDateTime), typeof(DateTime?),
                typeof(PknDateTimePicker),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty EndSelectedDateTimeProperty =
            EndSelectedDateTimePropertyKey.DependencyProperty;



        #endregion


        #region 更新逻辑

        private static void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PknDateTimePicker)d;

            if (control.StartSelectedDate.HasValue)
            {
                control.StartSelectedDateTime = new DateTime(
                    control.StartSelectedDate.Value.Year,
                    control.StartSelectedDate.Value.Month,
                    control.StartSelectedDate.Value.Day,
                    control.StartSelectedHour,
                    control.StartSelectedMinute,
                    0);
            }
            else
            {
                control.StartSelectedDateTime = null;
            }

            if (control.EndSelectedDate.HasValue)
            {
                control.EndSelectedDateTime = new DateTime(
                    control.EndSelectedDate.Value.Year,
                    control.EndSelectedDate.Value.Month,
                    control.EndSelectedDate.Value.Day,
                    control.EndSelectedHour,
                    control.EndSelectedMinute,
                    0);
            }
            else
            {
                control.EndSelectedDateTime = null;
            }
        }

        #endregion
    }
}