using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Synth.PerformanceModel;
using Synth.WPF.Instrument;

namespace Synth.WPF.Demo.Instruments
{
    /// <summary>
    /// Interaction logic for MinimumInstrument.xaml
    /// </summary>
    [AutoLoadInstrument("Minimum Instrument", "", "The miminum instrument needed to make a meaningul synth.")]
    public partial class MinimumInstrument : MonoSimpleInstrument
    {
        public MinimumInstrument()
            : this(GetDefaultPerformance())
        {
            InitializeComponent();
        }

        public MinimumInstrument(ISimplePerformance performanceSource) 
            : base(performanceSource)
        {
            InitializeComponent();
        }

        //private void HandleLoaded(object sender, RoutedEventArgs e)
        //{
        //    SynthWindow parentWindow = Window.GetWindow(this) as SynthWindow;
        //    if (parentWindow != null)
        //    {
        //        parentWindow.VelocityLabel = "VOLUME";
        //    }
        //}
    }
}
