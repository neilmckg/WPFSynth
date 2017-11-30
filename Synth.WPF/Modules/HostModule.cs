using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

using Synth.Audio;

namespace Synth.WPF.Modules
{
    /// <summary>
    /// A Module that can encapsulate other modules
    /// </summary>
    [ContentProperty("Content")]    
    public class HostModule : Module
    {
        // TODO can I provide static methods to simplify defining custom dependency properties?

        public static readonly DependencyProperty InputTemplateProperty = DependencyProperty.Register("InputTemplate", typeof(DataTemplate), typeof(HostModule));

        public DataTemplate InputTemplate
        {
            get { return (DataTemplate)GetValue(InputTemplateProperty); }
            set { SetValue(InputTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", 
                                                                                                typeof (object), 
                                                                                                typeof (HostModule),
                                                                                                new FrameworkPropertyMetadata(null, OnContentChanged));
        [Bindable(true)]
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        static HostModule()
        {
            InitializeClassMetadata<HostModule>(WPF.Resources.HostModuleTemplate);
        }

        public HostModule(IAudioLink audioLink)
            : base(audioLink)
        {
            DataContext = this;
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HostModule target = d as HostModule;
            if (target != null)
            {
                target.RemoveLogicalChild(e.OldValue);
                target.AddLogicalChild(e.NewValue);
            }
        }
    }
}
