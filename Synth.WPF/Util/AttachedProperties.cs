using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Synth.WPF.Util
{
    public static class AttachedProperties
    {
        #region IsMomentary

        public static readonly DependencyProperty IsMomentaryProperty = DependencyProperty.RegisterAttached("IsMomentary", typeof(bool), typeof(AttachedProperties), new PropertyMetadata(false, HandleIsMomentaryChanged));

        public static void SetIsMomentary(UIElement element, Boolean value)
        {
            element.SetValue(IsMomentaryProperty, value);
        }
        public static bool GetIsMomentary(UIElement element)
        {
            return (bool)element.GetValue(IsMomentaryProperty);
        }

        private static void HandleIsMomentaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToggleButton)
            {
                (d as ToggleButton).PreviewMouseDown -= HandleToggleButtonMouseButtonChanged;
                (d as ToggleButton).PreviewMouseUp -= HandleToggleButtonMouseButtonChanged;

                (d as ToggleButton).PreviewMouseDown += HandleToggleButtonMouseButtonChanged;
                (d as ToggleButton).PreviewMouseUp += HandleToggleButtonMouseButtonChanged;
            }
            else
            {
                throw new InvalidOperationException("The IsMomentary attached property can be set only on ToggleButtons.");
            }
        }

        #endregion IsMomentary

        #region IsToggleOnMouseDown

        public static readonly DependencyProperty IsToggleOnMouseDownProperty = DependencyProperty.RegisterAttached("IsToggleOnMouseDown", typeof(bool), typeof(AttachedProperties), new PropertyMetadata(false, HandleIsToggleOnMouseDownChanged));

        public static void SetIsToggleOnMouseDown(UIElement element, Boolean value)
        {
            element.SetValue(IsToggleOnMouseDownProperty, value);
        }
        public static bool GetIsToggleOnMouseDown(UIElement element)
        {
            return (bool)element.GetValue(IsToggleOnMouseDownProperty);
        }

        private static void HandleIsToggleOnMouseDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToggleButton)
            {
                (d as ToggleButton).PreviewMouseDown -= HandleToggleButtonMouseButtonChanged;

                (d as ToggleButton).PreviewMouseDown += HandleToggleButtonMouseButtonChanged;
            }
            else
            {
                throw new InvalidOperationException("The IsMomentary attached property can be set only on ToggleButtons.");
            }
        }

        #endregion IsToggleOnMouseDown

        #region supporting method for both IsMomentary and IsToggleOnMouseDown

        private static void HandleToggleButtonMouseButtonChanged(object sender, MouseButtonEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb == null)
                return;

            bool isMomentary = GetIsMomentary(tb);
            bool isToggleOnMouseDown = GetIsToggleOnMouseDown(tb);

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (isMomentary || isToggleOnMouseDown)
                    tb.IsChecked = true;
                if (isMomentary)
                    tb.CaptureMouse();
            }
            else
            {
                if (isMomentary)
                {
                    tb.IsChecked = false;
                    tb.ReleaseMouseCapture();
                }
            }
            
            e.Handled = true;
        }

        #endregion support for both IsMomentary and IsToggleOnMouseDown

        #region IsVerbose

        // This attached property has no behavior. It's up to each control's template to do something with it.

        public static readonly DependencyProperty IsVerboseProperty = DependencyProperty.RegisterAttached("IsVerbose", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsVerbose(UIElement element, Boolean value)
        {
            element.SetValue(IsVerboseProperty, value);
        }
        public static bool GetIsVerbose(UIElement element)
        {
            return (bool)element.GetValue(IsVerboseProperty);
        }

        #endregion IsVerbose
    }
}
