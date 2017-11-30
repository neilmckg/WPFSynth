using System;
using System.Windows;

using Synth.Audio;
using Synth.NAudio;
using Synth.WPF.Util;

namespace Synth.WPF
{
    public abstract class LauncherBase
    {
        protected LauncherBase()
        {            
        }

        protected void Run(Quality quality)
        {
            int audioSampleRate = (quality == Quality.High) ? 44100 : 19200;

            Application app = new Application();
            app.Activated += OnActivated;
            app.Exit += OnExit;

            try
            {
                AudioLink.Instance.Initialize(new FinalWaveOutput(audioSampleRate));

                Window window = GetMainWindow();
                app.Run(window);
            }
            catch (Exception ex)
            {
                Environment.ExitCode = ex.HResult;
            }
        }

        private void OnActivated(object sender, EventArgs e)
        {
            if (!new DependencyObject().IsInDesignMode())
                AudioLink.Instance.Activate();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            AudioLink.Instance.Dispose();
            MidiLink.Instance.Dispose();
        }

        protected abstract Window GetMainWindow();
    }
}
