using System.Windows;

namespace Synth.WPF.Controls
{
    /// <summary>
    /// Allows binding readonly properties of (such as module values/outputs) to dependency properties 
    /// defined in the containing class, by sitting inbetween them and binding in both directions.
    /// The ValueOut property should be bound using OneWayToSource.
    /// </summary>
    // This needed because if you define a custom class with a XAML component, you can't directly set or 
    //  bind dependency properties defined in the same class.
    //  And you can't put a OneWayToSource binding on the property that provides the value, 
    //  because that property is (normally) read-only and a quirk in WPF disallows that.
    // So this class binds to both the data source and the data destination, and moves incoming data 
    //  from one to the other.
    public class BindingInterconnect : FrameworkElement
    {
        public static readonly DependencyProperty ValueInProperty = DependencyProperty.Register("ValueIn", typeof(object), typeof(BindingInterconnect), new PropertyMetadata(HandleInputChanged));
        public object ValueIn
        {
            get { return GetValue(ValueInProperty); }
            set { SetValue(ValueInProperty, value); }
        }

        public static readonly DependencyProperty ValueOutProperty = DependencyProperty.Register("ValueOut", typeof(object), typeof(BindingInterconnect), new PropertyMetadata(HandleInputChanged));
        public object ValueOut
        {
            get { return GetValue(ValueOutProperty); }
            set { SetValue(ValueOutProperty, value); }
        }

        private static void HandleInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BindingInterconnect).ValueOut = e.NewValue;
        }

        public BindingInterconnect()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
