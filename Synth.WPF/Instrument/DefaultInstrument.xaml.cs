using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Synth.PerformanceModel;

namespace Synth.WPF.Instrument
{
    /// <summary>
    /// Interaction logic for DefaultInstrument.xaml
    /// </summary>
    public partial class DefaultInstrument : MonoSimpleInstrument
    {
        public DefaultInstrument()
            : this(GetDefaultPerformance())
        {
        }

        public DefaultInstrument(ISimplePerformance performanceSource) 
            : base(performanceSource)
        {
            InitializeComponent();
        }
    }
}
