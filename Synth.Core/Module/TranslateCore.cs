using System;
using Synth.Core;

namespace Synth.Module
{
    public class TranslateCore : ModuleCoreWithControlOutput
    {
        public TranslateCore()
        {
            InitializeInputs();
            HandleInputChanged();
        }

        public FloatInput SourceValue { get; private set; }

        public FloatInput Scale { get; private set; }

        public FloatInput Center { get; private set; }

        public FloatInput Curvature { get; private set; }

        private void HandleInputChanged()
        {
            Output = Convert(SourceValue.Value);
        }

        protected override void OnIsActiveChanged()
        {
            HandleInputChanged();
        }

        public float Convert(float input)
        {
            float output = 0;

            if (IsActive)
            {
                output = input*Scale.Value;

                if (Curvature.Value != 1)
                    output = (float) (Math.Sign(output)*Math.Pow(Math.Abs(output), Curvature.Value));

                output += Center.Value;
            }
            else
            {
                Output = input;
            }

            return output;
        }

        private void InitializeInputs()
        {
            SourceValue = new FloatInput("Source Value", 0, float.MinValue, float.MaxValue, ValueOutOfRangeStrategy.Accept, (name, v0, v1) => HandleInputChanged());
            Scale = new FloatInput("Scale", 1, float.MinValue, float.MaxValue, ValueOutOfRangeStrategy.Accept, (name, v0, v1) => HandleInputChanged());
            Center = new FloatInput("Center", 0, float.MinValue, float.MaxValue, ValueOutOfRangeStrategy.Accept, (name, v0, v1) => HandleInputChanged());
            Curvature = new FloatInput("Curvature", 1, float.MinValue, float.MaxValue, ValueOutOfRangeStrategy.Accept, (name, v0, v1) => HandleInputChanged());
        }
    }
}
