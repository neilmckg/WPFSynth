using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using Synth.Audio;
using Synth.Core;
using Synth.WPF.Util;

namespace Synth.WPF.Modules
{
    public class Module : Control
    {
        #region dependency properties

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Module), new PropertyMetadata("Module"));
        public string Description
        {
            get{ return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        #endregion dependency properties

        static Module()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Module), new FrameworkPropertyMetadata(typeof(Module)));
        }

        private readonly IAudioLink _audioLink;

        private Module()
        {
        }

        protected Module(IAudioLink audioLink)
        {
            if (audioLink == null && !this.IsInDesignMode())
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            if (this is IClockListener)
                _audioLink.DetachClockListener(this as IClockListener);
            if (this is IAudioSource)
                // May not be attached, but that's ok.
                _audioLink.DetachSource(this as IAudioSource);
        }

        #region templating & other resource support

        protected static void InitializeClassMetadata<T>(ControlTemplate template)
            where T : Module
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(typeof(T)));

            TemplateProperty.SetDefaultValue<T>(template);

            DescriptionProperty.SetDefaultValue<T>(typeof(T).Name);
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            //if (ReadLocalValue(DescriptionProperty) == DependencyProperty.UnsetValue)
            //    SetCurrentValue(DescriptionProperty, GetType().Name);

            if (!this.IsInDesignMode() && this is IClockListener)
                _audioLink.AttachClockListener(this as IClockListener);
        }

        #endregion templating & other resource support

        #region support for wireable control inputs in derived classes

        protected static void BaseControlInputValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Module)
                (d as Module).InputValueChanged(e.Property.Name);
        }

        protected virtual void InputValueChanged(string inputName)
        {
        }

        #endregion support for wireable control inputs in derived classes

        #region support for bindable child items

        protected void RegisterChildCollection<T>(ObservableCollection<T> collection, string collectionKey = null)
        {
            collection.CollectionChanged += (sender, args) =>  HandleChildCollectionChanged(args, collectionKey);
        }

        protected virtual void HandleChildCollectionChanged(NotifyCollectionChangedEventArgs e, string collectionKey)
        {
            // for binding to work, children need to be in the logical tree so the source elements can be found
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FrameworkContentElement item in e.NewItems.OfType<FrameworkContentElement>())
                {
                    AddLogicalChild(item);
                    ChildAdded(item, collectionKey);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (FrameworkContentElement item in e.OldItems.OfType<FrameworkContentElement>())
                {
                    RemoveLogicalChild(item);
                    ChildRemoved(item, collectionKey);
                }
            }

            // TODO other collection changes: handle as appropriate
        }

        protected virtual void ChildAdded(FrameworkContentElement child, string collectionKey)
        {
        }

        protected virtual void ChildRemoved(FrameworkContentElement child, string collectionKey)
        {
        }

        #endregion support for bindable child items

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Description))
                return Description;
            else if (!string.IsNullOrWhiteSpace(Name))
                return Name;
            else
                return GetType().Name;
        }
    }
}
