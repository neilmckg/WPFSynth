using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Synth.WPF.Controls;
using Synth.WPF.Util;

namespace Synth.WPF
{
    public static class Resources
    {
        private class ColorResources : XamlResourceFacade
        {
            public ColorResources() 
                : base("Resources.xaml")
            {
            }

            public Brush ForegroundBrush
            {
                get { return GetValue<Brush>("ForegroundBrush"); }
                set { SetOverride("ForegroundBrush", value); }
            }

            public Brush BackgroundBrush
            {
                get { return GetValue<Brush>("BackgroundBrush"); }
                set { SetOverride("BackgroundBrush", value); }
            }

            public Color TrackColor
            {
                get { return GetValue<Color>("TrackColor"); }
                set { SetOverride("TrackColor", value); }
            }

            public Color PopoutColor
            {
                get { return GetValue<Color>("ThumbColor"); }
                set { SetOverride("ThumbColor", value); }
            }

            public Color AudioMeterColor
            {
                get { return GetValue<Color>("AudioMeterColor"); }
                set { SetOverride("AudioMeterColor", value); }
            }

            public Color ControlMeterColor
            {
                get { return GetValue<Color>("ControlMeterColor"); }
                set { SetOverride("ControlMeterColor", value); }
            }

            public Brush SynthWindowBrush
            {
                get { return GetValue<Brush>("SynthWindowBrush"); }
                set { SetOverride("SynthWindowBrush", value); }
            }
        }

        private class LookAndFeelResources : XamlResourceFacade
        {
            public LookAndFeelResources()
                : base("Resources.xaml")
            {
            }

            public Style DefaultTextStyle
            {
                get { return GetValue<Style>("TextStyle"); }
                set { SetOverride("TextStyle", value); }
            }

            public ControlTemplate SimpleComboBoxTemplate
            {
                get { return GetValue<ControlTemplate>("SimpleComboBoxTemplate"); }
                set { SetOverride("SimpleComboBoxTemplate", value); }
            }

            public ControlTemplate ToggleTemplate
            {
                get { return GetValue<ControlTemplate>("WhiteBorderToggleTemplate"); }
                set { SetOverride("WhiteBorderToggleTemplate", value); }
            }

            public ControlTemplate InstrumentTemplate
            {
                get { return GetValue<ControlTemplate>("InstrumentTemplate"); }
                set { SetOverride("InstrumentTemplate", value); }
            }

            public ControlTemplate SliderThumbTemplate
            {
                get { return GetValue<ControlTemplate>("SliderThumbTemplate"); }
                set { SetOverride("SliderThumbTemplate", value); }
            }

            public Style MeterControlStyle
            {
                get { return GetValue<Style>("MeterControlStyle"); }
                set { SetOverride("MeterControlStyle", value); }
            }

            public Style ControlSliderStyle
            {
                get { return GetValue<Style>("ControlSliderStyle"); }
                set { SetOverride("ControlSliderStyle", value); }
            }

            public ControlTemplate InputColumnTemplate
            {
                get { return GetValue<ControlTemplate>("InputColumnTemplate"); }
                set { SetOverride("InputColumnTemplate", value); }
            }

            public Style InputColumnGroupBoxStyle
            {
                get { return GetValue<Style>("InputColumnGroupBoxStyle"); }
                set { SetOverride("InputColumnGroupBoxStyle", value); }
            }

            public ControlTemplate MonoSimpleInstrumentInputTemplate
            {
                get { return GetValue<ControlTemplate>("MonoSimpleInstrumentInputTemplate"); }
                set { SetOverride("MonoSimpleInstrumentInputTemplate", value); }
            }
            public ControlTemplate MonoSimpleInstrumentOutputTemplate
            {
                get { return GetValue<ControlTemplate>("MonoSimpleInstrumentOutputTemplate"); }
                set { SetOverride("MonoSimpleInstrumentOutputTemplate", value); }
            }

            public Style BoxStyle
            {
                get { return GetValue<Style>("BoxStyle"); }
                set { SetOverride("BoxStyle", value); }
            }

            public Style BoxShyStyle
            {
                get { return GetValue<Style>("BoxShyStyle"); }
                set { SetOverride("BoxShyStyle", value); }
            }

            public Style ModuleBoxStyle
            {
                get { return GetValue<Style>("ModuleBoxStyle"); }
                set { SetOverride("ModuleBoxStyle", value); }
            }

            public Style ModuleBoxShyStyle
            {
                get { return GetValue<Style>("ModuleBoxShyStyle"); }
                set { SetOverride("ModuleBoxShyStyle", value); }
            }

            public Style MouseOverEmphasisStyle
            {
                get { return GetValue<Style>("MouseOverWhiteGlow"); }
                set { SetOverride("MouseOverWhiteGlow", value); }
            }

            public Style TriggerIndicatorStyle
            {
                get { return GetValue<Style>("TriggerIndicatorStyle"); }
                set { SetOverride("TriggerIndicatorStyle", value); }
            }

            public double DefaultInputColumnWidth
            {
                get { return GetValue<double>("StdInputColWidth"); }
                set { SetOverride("StdInputColWidth", value); }
            }
        }

        private class ModuleTemplateResources : XamlResourceFacade
        {
            public ModuleTemplateResources()
                : base("ModuleTemplates.xaml")
            {
            }

            public ControlTemplate AudioMeterTemplate
            {
                get { return GetValue<ControlTemplate>("AudioMeterModuleTemplate"); }
                set { SetOverride("AudioMeterModuleTemplate", value); }
            }

            public ControlTemplate EchoTemplate
            {
                get { return GetValue<ControlTemplate>("EchoTemplate"); }
                set { SetOverride("EchoTemplate", value); }
            }

            public ControlTemplate EnvelopeTemplate
            {
                get { return GetValue<ControlTemplate>("EnvelopeTemplate"); }
                set { SetOverride("EnvelopeTemplate", value); }
            }

            public ControlTemplate FaderTemplate
            {
                get { return GetValue<ControlTemplate>("FaderTemplate"); }
                set { SetOverride("FaderTemplate", value); }
            }

            public ControlTemplate FuzzTemplate
            {
                get { return GetValue<ControlTemplate>("FuzzTemplate"); }
                set { SetOverride("FuzzTemplate", value); }
            }

            public ControlTemplate GlideTemplate
            {
                get { return GetValue<ControlTemplate>("GlideTemplate"); }
                set { SetOverride("GlideTemplate", value); }
            }

            public ControlTemplate HostModuleTemplate
            {
                get { return GetValue<ControlTemplate>("HostModuleTemplate"); }
                set { SetOverride("HostModuleTemplate", value); }
            }

            public ControlTemplate LFOTemplate
            {
                get { return GetValue<ControlTemplate>("LFOTemplate"); }
                set { SetOverride("LFOTemplate", value); }
            }

            public ControlTemplate LPFTemplate
            {
                get { return GetValue<ControlTemplate>("LPFTemplate"); }
                set { SetOverride("LPFTemplate", value); }
            }

            public ControlTemplate MixerTemplate
            {
                get { return GetValue<ControlTemplate>("MixerTemplate"); }
                set { SetOverride("MixerTemplate", value); }
            }

            public ControlTemplate ModuleNotImplementedTemplate
            {
                get { return GetValue<ControlTemplate>("ModuleNotImplementedTemplate"); }
                set { SetOverride("ModuleNotImplementedTemplate", value); }
            }

            public ControlTemplate NoiseTemplate
            {
                get { return GetValue<ControlTemplate>("NoiseTemplate"); }
                set { SetOverride("NoiseTemplate", value); }
            }

            public ControlTemplate OscillatorTemplate
            {
                get { return GetValue<ControlTemplate>("OscillatorTemplate"); }
                set { SetOverride("OscillatorTemplate", value); }
            }

            public ControlTemplate PanTemplate
            {
                get { return GetValue<ControlTemplate>("PanTemplate"); }
                set { SetOverride("PanTemplate", value); }
            }

            public ControlTemplate TranslateTemplate
            {
                get { return GetValue<ControlTemplate>("TranslateTemplate"); }
                set { SetOverride("TranslateTemplate", value); }
            }
        }

        private class UtilityResources : XamlResourceFacade
        {
            public UtilityResources()
                : base("Resources.xaml")
            {
            }

            public Style VerboseOnlyStyle
            {
                get { return GetValue<Style>("VerboseOnlyStyle"); }
            }

            public ControlTemplate ContentOnlyTemplate
            {
                get { return GetValue<ControlTemplate>("ContentOnlyTemplate"); }
            }

            public ItemsPanelTemplate WrapPanelItemsTemplate
            {
                get { return GetValue<ItemsPanelTemplate>("WrapPanelTemplate"); }
            }

            public SliderTick[] StandardLevelTicks
            {
                get { return GetValue<SliderTick[]>("StandardLevelTicks"); }
            }

            public SliderTick[] PitchRangeTicks
            {
                get { return GetValue<SliderTick[]>("PitchRangeTicks"); }
            }
        }

        private static UtilityResources Util { get; } = new UtilityResources();
        private static ColorResources Colors { get; } = new ColorResources();
        private static ModuleTemplateResources Modules { get; } = new ModuleTemplateResources();
        private static LookAndFeelResources LookAndFeel { get; } = new LookAndFeelResources();

        #region non-overridable utility templates 

        public static Style VerboseOnlyStyle
        {
            get { return Util.VerboseOnlyStyle; }
        }

        public static ControlTemplate ContentOnlyTemplate
        {
            get { return Util.ContentOnlyTemplate; }
        }

        public static ItemsPanelTemplate WrapPanelItemsTemplate
        {
            get { return Util.WrapPanelItemsTemplate; }
        }

        public static SliderTick[] StandardLevelTicks
        {
            get { return Util.StandardLevelTicks; }
        }

        public static SliderTick[] PitchRangeTicks
        {
            get { return Util.PitchRangeTicks; }
        }

        #endregion non-overridable utility templates 

        #region look and feel

        public static Style DefaultTextStyle
        {
            get { return LookAndFeel.DefaultTextStyle; }
            set { LookAndFeel.DefaultTextStyle = value; }
        }


        public static ControlTemplate SimpleComboBoxTemplate
        {
            get { return LookAndFeel.SimpleComboBoxTemplate; }
            set { LookAndFeel.SimpleComboBoxTemplate = value; }
        }

        public static ControlTemplate ToggleTemplate
        {
            get { return LookAndFeel.ToggleTemplate; }
            set { LookAndFeel.ToggleTemplate = value; }
        }

        public static ControlTemplate InstrumentTemplate
        {
            get { return LookAndFeel.InstrumentTemplate; }
            set { LookAndFeel.InstrumentTemplate = value; }
        }

        public static ControlTemplate SliderThumbTemplate
        {
            get { return LookAndFeel.SliderThumbTemplate; }
            set { LookAndFeel.SliderThumbTemplate = value; }
        }

        public static Style MeterControlStyle
        {
            get { return LookAndFeel.MeterControlStyle; }
            set { LookAndFeel.MeterControlStyle = value; }
        }

        public static Style ControlSliderStyle
        {
            get { return LookAndFeel.ControlSliderStyle; }
            set { LookAndFeel.ControlSliderStyle = value; }
        }

        public static ControlTemplate InputColumnTemplate
        {
            get { return LookAndFeel.InputColumnTemplate; }
            set { LookAndFeel.InputColumnTemplate = value; }
        }

        public static Style InputColumnGroupBoxStyle
        {
            get { return LookAndFeel.InputColumnGroupBoxStyle; }
            set { LookAndFeel.InputColumnGroupBoxStyle = value; }
        }

        public static ControlTemplate MonoSimpleInstrumentInputTemplate
        {
            get { return LookAndFeel.MonoSimpleInstrumentInputTemplate; }
            set { LookAndFeel.MonoSimpleInstrumentInputTemplate = value; }
        }

        public static ControlTemplate MonoSimpleInstrumentOutputTemplate
        {
            get { return LookAndFeel.MonoSimpleInstrumentOutputTemplate; }
            set { LookAndFeel.MonoSimpleInstrumentOutputTemplate = value; }
        }

        public static Style BoxStyle
        {
            get { return LookAndFeel.BoxStyle; }
            set { LookAndFeel.BoxStyle = value; }
        }

        public static Style BoxShyStyle
        {
            get { return LookAndFeel.BoxShyStyle; }
            set { LookAndFeel.BoxShyStyle = value; }
        }

        public static Style ModuleBoxStyle
        {
            get { return LookAndFeel.ModuleBoxStyle; }
            set { LookAndFeel.ModuleBoxStyle = value; }
        }

        public static Style ModuleBoxShyStyle
        {
            get { return LookAndFeel.ModuleBoxShyStyle; }
            set { LookAndFeel.ModuleBoxShyStyle = value; }
        }

        public static Style MouseOverEmphasisStyle
        {
            get { return LookAndFeel.MouseOverEmphasisStyle; }
            set { LookAndFeel.MouseOverEmphasisStyle = value; }
        }

        public static Style TriggerIndicatorStyle
        {
            get { return LookAndFeel.TriggerIndicatorStyle; }
            set { LookAndFeel.TriggerIndicatorStyle = value; }
        }

        public static double DefaultInputColumnWidth
        {
            get { return LookAndFeel.DefaultInputColumnWidth; }
            set { LookAndFeel.DefaultInputColumnWidth = value; }
        }

        #endregion look and feel

        #region colors

        public static Brush ForegroundBrush
        {
            get { return Colors.ForegroundBrush; }
            set { Colors.ForegroundBrush = value; }
        }

        public static Brush BackgroundBrush
        {
            get { return Colors.BackgroundBrush; }
            set { Colors.BackgroundBrush = value; }
        }

        public static Color TrackColor
        {
            get { return Colors.TrackColor; }
            set { Colors.TrackColor = value; }
        }

        public static Color PopoutColor
        {
            get { return Colors.PopoutColor; }
            set { Colors.PopoutColor = value; }
        }

        public static Color AudioMeterColor
        {
            get { return Colors.AudioMeterColor; }
            set { Colors.AudioMeterColor = value; }
        }

        public static Color ControlMeterColor
        {
            get { return Colors.ControlMeterColor; }
            set { Colors.ControlMeterColor = value; }
        }

        public static Brush SynthWindowBrush
        {
            get { return Colors.SynthWindowBrush; }
            set { Colors.SynthWindowBrush = value; }
        }


        #endregion colors

        #region module templates

        public static ControlTemplate AudioMeterTemplate
        {
            get { return Modules.AudioMeterTemplate; }
            set { Modules.AudioMeterTemplate = value; }
        }

        public static ControlTemplate EchoTemplate
        {
            get { return Modules.EchoTemplate; }
            set { Modules.EchoTemplate = value; }
        }

        public static ControlTemplate EnvelopeTemplate
        {
            get { return Modules.EnvelopeTemplate; }
            set { Modules.EnvelopeTemplate = value; }
        }

        public static ControlTemplate FaderTemplate
        {
            get { return Modules.FaderTemplate; }
            set { Modules.FaderTemplate = value; }
        }

        public static ControlTemplate FuzzTemplate
        {
            get { return Modules.FuzzTemplate; }
            set { Modules.FuzzTemplate = value; }
        }

        public static ControlTemplate GlideTemplate
        {
            get { return Modules.GlideTemplate; }
            set { Modules.GlideTemplate = value; }
        }

        public static ControlTemplate HostModuleTemplate
        {
            get { return Modules.HostModuleTemplate; }
            set { Modules.HostModuleTemplate = value; }
        }

        public static ControlTemplate LFOTemplate
        {
            get { return Modules.LFOTemplate; }
            set { Modules.LFOTemplate = value; }
        }

        public static ControlTemplate LPFTemplate
        {
            get { return Modules.LPFTemplate; }
            set { Modules.LPFTemplate = value; }
        }

        public static ControlTemplate MixerTemplate
        {
            get { return Modules.MixerTemplate; }
            set { Modules.MixerTemplate = value; }
        }

        public static ControlTemplate ModuleNotImplementedTemplate
        {
            get { return Modules.ModuleNotImplementedTemplate; }
            set { Modules.ModuleNotImplementedTemplate = value; }
        }

        public static ControlTemplate NoiseTemplate
        {
            get { return Modules.NoiseTemplate; }
            set { Modules.NoiseTemplate = value; }
        }

        public static ControlTemplate OscillatorTemplate
        {
            get { return Modules.OscillatorTemplate; }
            set { Modules.OscillatorTemplate = value; }
        }

        public static ControlTemplate PanTemplate
        {
            get { return Modules.PanTemplate; }
            set { Modules.PanTemplate = value; }
        }

        public static ControlTemplate TranslateTemplate
        {
            get { return Modules.TranslateTemplate; }
            set { Modules.TranslateTemplate = value; }
        }

        #endregion module templates
    }
}
