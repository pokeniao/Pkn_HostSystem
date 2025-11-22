using System.Windows;

namespace Pkn_HostSystem.NodifyControl
{
    /// <summary>
    /// BindingProxy 继承自 Freezable。Freezable 有一个特性：它能在资源字典中持有 DataContext，并且可以被绑定。相当于定义了一个全局变量的DataContext,方便在其他页面获取
    /// 
    /// </summary>
    public class BindingProxy : Freezable
    {
        //通过 Freezable，可以在 XAML 资源中声明一个实例并绑定到你想要的 DataContext（比如 EditorViewModel）。
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(nameof(DataContext), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(default(object)));

        /// <summary>
        /// 这里注册了一个 DataContext 属性。
        /// </summary>
        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        protected override Freezable CreateInstanceCore()
            => new BindingProxy();
    }
}
