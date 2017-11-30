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
    /// Interaction logic for SandboxInstrument.xaml
    /// </summary>
    [AutoLoadInstrument("Sandbox", "", "A scratch pad for development.")]
    public partial class SandboxInstrument : MonoSimpleInstrument
    {
        public SandboxInstrument()
            : this(GetDefaultPerformance())
        {
            InitializeComponent();
        }

        public SandboxInstrument(ISimplePerformance performanceSource) 
            : base(performanceSource)
        {
            InitializeComponent();
        }
 
    }
}
