using System;
using System.Windows;
using Synth.Util;
using Synth.WPF.Instrument;

namespace Synth.WPF.Demo
{
    public class Launcher : LauncherBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Launcher().Run(Quality.High);
        }

        protected override Window GetMainWindow()
        {
            SynthWindow window = new SynthWindow();
            window.Instruments.Add(InstrumentFinder.DefaultInstrument);
            InstrumentFinder.AutoLoadedInstruments.Execute(window.Instruments.Add);

            return window;
        }
    }
}
