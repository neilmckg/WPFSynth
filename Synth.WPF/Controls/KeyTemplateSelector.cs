using System;
using System.Windows;
using System.Windows.Controls;

using Synth.Core;
using Synth.WPF.Util;

namespace Synth.WPF.Controls
{
    public class KeyTemplateSelector : DataTemplateSelector
    {
        private static readonly Lazy<KeyTemplateSelector> _instance = new Lazy<KeyTemplateSelector>(() => new KeyTemplateSelector());
        public static KeyTemplateSelector Instance
        {
            get { return _instance.Value; }
        }

        private static ResourceDictionary _resources;
        private ResourceDictionary Resources
        {
            get { return _resources ?? (_resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/Synth.WPF;component/Resources.xaml") }); }
        }

        private KeyTemplateSelector()
            : base()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement fe = container as FrameworkElement;

            ItemExtender<IScaleNote, bool> extender = item as ItemExtender<IScaleNote, bool>;
            if (extender == null)
                return null;
            else if (extender.Item.Name.StartsWith("C#") || extender.Item.Name.StartsWith("F#"))
                return (DataTemplate)fe.FindResource("BlackKeyTemplateC");
            else if (extender.Item.Name.StartsWith("D#") || extender.Item.Name.StartsWith("A#"))
                return (DataTemplate)fe.FindResource("BlackKeyTemplateD");
            else if (extender.Item.Name.Contains("#"))
                return (DataTemplate)fe.FindResource("BlackKeyTemplateG");
            else
                return (DataTemplate)fe.FindResource("WhiteKeyTemplate");
        }
    }
}
