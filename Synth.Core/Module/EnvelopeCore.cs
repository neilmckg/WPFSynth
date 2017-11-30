using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Synth.Audio;
using Synth.Core;
using Synth.Util;

namespace Synth.Module
{
    public class EnvelopeCore : ModuleCoreWithControlOutput, IDisposable
    {
        public event EventHandler<EventArgs<EnvelopePhase>> StepsChanged;
        public event EventHandler PhaseChanged;

        private static readonly EventArgs _phaseArgs = new EventArgs();

        private int _currentStepIndex = -1;
        private float _stepStartValue = 0;
        private float? _timePositionInCurrentStep = null;
        private IReadOnlyList<IEnvelopeStep> _currentSteps = null;
        private float _envTimePerTick;
        private readonly IAudioLink _audioLink;
        private EnvelopePhase? _queuedPhase = null;
        private readonly IEnumerable<StepCollection> _steps;
        private readonly AudioClockDivider _clockSource;

        public EnvelopeCore(IAudioLink audioLink)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            SetOutput(0);

            InitializeInputs();
            InitializeStepCollections();
            _steps = new[] { Attack, Loop, Release };

            _audioLink.SampleRateChanged += HandleQualityChanged;
            _clockSource = new AudioClockDivider(_audioLink, 50, id => HandleControlClockTick());

            HandleQualityChanged(null, null);
        }

        private void HandleQualityChanged(object sender, EventArgs e)
        {
            ApplyTimeScale();
        }

        public StepCollection Attack { get; private set; }
        public StepCollection Loop { get; private set; }
        public StepCollection Release { get; private set; }

        private EnvelopePhase _phase = EnvelopePhase.Inactive;
        public EnvelopePhase Phase
        {
            get { return _phase; }
            private set
            {
                if (value == _phase)
                {
                    return;
                }
                // Redirects to other phases: these will trigger recursive call back into this logic, with a different phase
                else if (value == EnvelopePhase.Attack && Attack.Count == 0)
                {
                    Phase = EnvelopePhase.Loop;
                    return;
                }
                else if (value == EnvelopePhase.Loop && Loop.Count == 0)
                {
                    Phase = EnvelopePhase.Inactive;
                    return;
                }
                else if (value == EnvelopePhase.Release && Release.Count == 0)
                {
                    Phase = EnvelopePhase.Inactive;
                    return;
                }

                _queuedPhase = null;
                InitializePhase(value);

                OnPhaseChanged();
            }
        }

        private float _currentRawOutput = 0;

        private void InitializePhase(EnvelopePhase newPhase)
        {
            _phase = newPhase;
            _currentSteps = _steps.FirstOrDefault(sc => sc.Phase == newPhase);
            _timePositionInCurrentStep = null;
            _currentStepIndex = 0;
            _stepStartValue = _currentRawOutput;
        }

        public FloatInput TimeScale { get; private set; }
        public FloatInput LevelScale { get; private set; }
        public Input<bool> IsTriggered { get; private set; }

        private void HandleControlClockTick()
        {
            // Queuing the phase allows triggered phase changes to be marshalled onto the control clock thread
            if (_queuedPhase.HasValue)
                Phase = _queuedPhase.Value;

            if (Phase != EnvelopePhase.Inactive && _currentSteps.Count > 0)
                CalculateNewPosition();

            SetLevelForPosition();
        }

        private void InitializeInputs()
        {
            TimeScale = new FloatInput("Time Scale", 1, 0.125f, 10, ValueOutOfRangeStrategy.ForceIntoRange, (next, v0, v1) => ApplyTimeScale());
            LevelScale = new FloatInput("Level Scale", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
            IsTriggered = new Input<bool>("Triggered", false, (name, v0, v1) => HandleIsTriggeredChanged());
        }

        private void InitializeStepCollections()
        {
            Attack = new StepCollection(this, EnvelopePhase.Attack);
            Attack.CollectionChanged += (o, e) => OnStepsChanged(EnvelopePhase.Attack);
            Attack.StepsChanged += (o, e) => OnStepsChanged(EnvelopePhase.Attack);

            Loop = new StepCollection(this, EnvelopePhase.Loop);
            Loop.CollectionChanged += (o, e) => OnStepsChanged(EnvelopePhase.Loop);
            Loop.StepsChanged += (o, e) => OnStepsChanged(EnvelopePhase.Loop);

            Release = new StepCollection(this, EnvelopePhase.Release);
            Release.CollectionChanged += (o, e) => OnStepsChanged(EnvelopePhase.Release);
            Release.StepsChanged += (o, e) => OnStepsChanged(EnvelopePhase.Release);
        }

        private void OnStepsChanged(EnvelopePhase phase)
        {
            EventHandler<EventArgs<EnvelopePhase>> evt = StepsChanged;
            if (evt != null)
                evt.Invoke(this, new EventArgs<EnvelopePhase>(phase));
        }

        private void OnPhaseChanged()
        {
            EventHandler evt = PhaseChanged;
            if (evt != null)
                evt.Invoke(this, _phaseArgs);
        }

        private void SetOutput(float rawValue)
        {
            _currentRawOutput = rawValue;
            if (IsActive && LevelScale != null)
                Output = rawValue * LevelScale.Value;
            else
                Output = 0;
        }

        private void AdvancePhase()
        {
            if (Phase == EnvelopePhase.Release)
            {
                SetOutput(0);
                Phase = EnvelopePhase.Inactive;
            }
            else if (Phase == EnvelopePhase.Attack)
                Phase = EnvelopePhase.Loop;
            else if (Phase == EnvelopePhase.Loop)        // stay in same phase, but start over
                InitializePhase(EnvelopePhase.Loop);
            else if (Phase != EnvelopePhase.Loop)
                Phase = EnvelopePhase.Inactive;
        }

        private void CalculateNewPosition()
        {
            // By the end, _currentSteps, _currentStepIndex, and _amountOfStepCompleted should all be set appropriately.
            // That may include advancing within a step, advancing to a new step, or advancing to a new phase

            if (_timePositionInCurrentStep == null)     // not yet started
            {
                _timePositionInCurrentStep = 0;
            }
            else
            {
                _timePositionInCurrentStep = _timePositionInCurrentStep.Value + _envTimePerTick;

                while (Phase != EnvelopePhase.Inactive && _timePositionInCurrentStep.HasValue && _timePositionInCurrentStep.Value > _currentSteps[_currentStepIndex].Seconds)
                {
                    AdvanceStep();
                }
            }
        }

        private void AdvanceStep()
        {
            // if we're already at the last step, advance the phase
            if (_currentStepIndex >= _currentSteps.Count - 1)
            {
                AdvancePhase();
                _timePositionInCurrentStep = 0;
            }
            else
            {
                _stepStartValue = _currentRawOutput;
                // TODO: review what this logic does -- I don't remember and it's not obvious. When I figure it out, document it.
                _timePositionInCurrentStep = _timePositionInCurrentStep - _currentSteps[_currentStepIndex].Seconds;
                _currentStepIndex++;
            }
        }

        private void SetLevelForPosition()
        {
            if (_currentSteps != null && _currentSteps.Count > 0 && _currentStepIndex >= 0 &&
                _currentStepIndex < _currentSteps.Count && _timePositionInCurrentStep.HasValue)
            {
                IEnvelopeStep step = _currentSteps[_currentStepIndex];

                float proportionOfStepCompleted = step.Seconds == 0
                    ? 1f
                    : (_timePositionInCurrentStep.Value/step.Seconds);
                float envLevel = _stepStartValue + (proportionOfStepCompleted*(step.TargetValue - _stepStartValue));
                SetOutput(envLevel);
            }
            else
            {
                // always call, so changes to levelscale will always take effect immediately
                SetOutput(this._currentRawOutput);
            }
        }

        private void HandleIsTriggeredChanged()
        {
            if (IsTriggered.Value)
                _queuedPhase = EnvelopePhase.Attack;
            else
                _queuedPhase = EnvelopePhase.Release;
       }

        private void ApplyTimeScale()
        {
            _envTimePerTick = 1 / (_clockSource.TicksPerSecond * TimeScale.Value);
        }

        public void Dispose()
        {
            if (_clockSource != null)
                _clockSource.Dispose();
        }

        public class StepCollection : ObservableCollection<IEnvelopeStep>
        {
            private const double LOOP_PLACEHOLDER_SECONDS = 0.5;

            public event EventHandler StepsChanged;

            private double _totalSeconds;

            public StepCollection(EnvelopeCore parent, EnvelopePhase phase)
            {
                ParentEnvelope = parent;
                Phase = phase;

                HandleStepChanged(null, null);
            }

            public EnvelopeCore ParentEnvelope { get; private set; }

            protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                // is unsubscribing slower than comparing & culling the two lists so we only subscribe to the new stuff?
                if (e.OldItems != null)
                    e.OldItems.OfType<IEnvelopeStep>().Execute(step => step.Changed -= HandleStepChanged);

                if (e.NewItems != null)
                    e.NewItems.OfType<IEnvelopeStep>().Execute(step => step.Changed += HandleStepChanged);

                HandleStepChanged(null, null);
            
                base.OnCollectionChanged(e);
            }

            private void HandleStepChanged(object sender, EventArgs e)
            {
                if (Phase == EnvelopePhase.Loop && !this.Any())
                    TotalSeconds = LOOP_PLACEHOLDER_SECONDS;
                else
                    TotalSeconds = this.Sum(s => s.Seconds) * ParentEnvelope.TimeScale.Value;
                OnStepsChanged();
            }

            public EnvelopePhase Phase { get; private set; }

            private void OnStepsChanged()
            {
                EventHandler evt = StepsChanged;
                if (evt != null)
                    evt.Invoke(this, new EventArgs());
            }

            public double TotalSeconds
            {
                get { return _totalSeconds; }
                private set
                {
                    if (value == _totalSeconds)
                        return;
                    _totalSeconds = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TotalSeconds"));
                }
            }
        }
    }

}
