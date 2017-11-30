using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using Synth.Audio;
using Synth.Module;
using Synth.Util;

namespace Synth.WPF.Modules
{
    public sealed class Glide : Module
    {
        #region I/O dependency properties

        #region Rate

        public static readonly DependencyProperty RateProperty = DependencyProperty.Register("Rate",
                                                                         typeof(double),
                                                                         typeof(Glide),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Rate
        {
            get { return (double)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }

        #endregion Rate

        #region SourceValue (input)

        public static readonly DependencyProperty SourceValueProperty = DependencyProperty.Register("SourceValue",
                                                                                           typeof(double),
                                                                                           typeof(Glide),
                                                                                           new FrameworkPropertyMetadata(0d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double SourceValue
        {
            get { return (double)GetValue(SourceValueProperty); }
            set { SetValue(SourceValueProperty, value); }
        }

        #endregion SourceValue (input)

        #region Value (output)

        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value",
                                                                                     typeof(double),
                                                                                     typeof(Glide),
                                                                                     new PropertyMetadata(0d));

        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        [Bindable(true)]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            private set { SetValue(ValuePropertyKey, value); }
        }

        #endregion Value (output)

        //#region ResetTrigger

        //public static readonly DependencyProperty ResetTriggerProperty = DependencyProperty.Register("ResetTrigger",
        //                                                                 typeof(bool),
        //                                                                 typeof(Glide),
        //                                                                 new PropertyMetadata(false, BaseControlInputValueChanged));
        //[Bindable(true)]
        //public bool ResetTrigger
        //{
        //    get { return (bool)GetValue(ResetTriggerProperty); }
        //    set { SetValue(ResetTriggerProperty, value); }
        //}

        //#endregion ResetTrigger

        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive",
                                                                         typeof(bool),
                                                                         typeof(Glide),
                                                                         new PropertyMetadata(false, BaseControlInputValueChanged));
        [Bindable(true)]
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        #endregion IsActive

        #endregion I/O dependency properties

        static Glide()
        {
            InitializeClassMetadata<Glide>(WPF.Resources.GlideTemplate);
        }

        private readonly GlideCore _core;

        public Glide()
            : this(AudioLink.Instance)
        {
        }

        public Glide(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new GlideCore(audioLink);
            _core.OutputChanged += HandleCoreOutputChanged;
            InputValueChanged(null);
            Unloaded += (s, e) => _core.Dispose();
        }

        private void HandleCoreOutputChanged(object sender, EventArgs<float> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Value = _core.Output));
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == IsActiveProperty.Name)
            {
                _core.IsActive = IsActive;
            }

            if (string.IsNullOrWhiteSpace(inputName) || inputName == SourceValueProperty.Name)
            {
                _core.SourceValue.Value = (float)SourceValue;
            }

            if (string.IsNullOrWhiteSpace(inputName) || inputName == RateProperty.Name)
            {
                _core.Rate.Value = (float)Rate;
            }
        }
    }
}
