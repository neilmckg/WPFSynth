using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Synth.Module;
using Synth.Util;

namespace Synth.WPF.Converters
{
    public class EnvPhaseToGeometryConverter : IValueConverter
    {
        private static readonly Lazy<EnvPhaseToGeometryConverter> _instance = new Lazy<EnvPhaseToGeometryConverter>(() => new EnvPhaseToGeometryConverter());

        public static EnvPhaseToGeometryConverter Instance
        {
            get { return _instance.Value; }
        }

        private EnvPhaseToGeometryConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is EnvelopeCore.StepCollection))
                throw new ArgumentException("EnvPhaseToGeometryConverter requires a value of type StepCollection");

            double totalSeconds = 0;

            EnvelopeCore.StepCollection coll = value as EnvelopeCore.StepCollection;
            Point[] points = coll.Select(s => new Point(totalSeconds += s.Seconds, 1 - s.TargetValue)).ToArray();

            double startingValue = GetStartingValue(coll);

            // The units don't matter here -- just that the projection is independently proportional on each axis.
            // For display, the path objects of related stepcollections eill need to have the same scaling as each other to line up correctly.

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0, 1 - startingValue);
           
            if (coll.Any())
            {
                points.Execute(p => figure.Segments.Add(new LineSegment(p, true)));

                if (coll.Phase == EnvelopePhase.Release && coll.Last().TargetValue != 0)
                    figure.Segments.Add(new LineSegment(new Point(totalSeconds, 1), true));

                AddFrame(figure, coll.TotalSeconds);
            }
            else if (coll.Phase == EnvelopePhase.Loop)
            {
                figure.Segments.Add(new LineSegment(new Point(coll.TotalSeconds, 1 - startingValue), true));
                AddFrame(figure, coll.TotalSeconds);
            }

            PathGeometry geo = new PathGeometry();
            geo.Figures.Add(figure);
            geo.Freeze();

            return geo;
        }

        private void AddFrame(PathFigure figure, double endSeconds)
        {
            // Paths are scaled based on the bounds of their contents rather than absolute metrics.
            // So these non-rendering lines ensure that all envelope geometries have the same bounding box, and are thus scaled consistently.

            figure.Segments.Add(new LineSegment(new Point(endSeconds, 1), false));
            figure.Segments.Add(new LineSegment(new Point(0, 1), false));
            figure.Segments.Add(new LineSegment(new Point(0, 0), false));
        }

        private double GetStartingValue(EnvelopeCore.StepCollection coll)
        {
            double startingValue = 0;

            if (coll.Phase == EnvelopePhase.Loop)
            {
                if (coll.ParentEnvelope.Attack.Any())
                    startingValue = coll.ParentEnvelope.Attack.Last().TargetValue;
                else
                    startingValue = 1;
            }
            else if (coll.Phase == EnvelopePhase.Release)
            {
                if (coll.ParentEnvelope.Loop.Any())
                    startingValue = coll.ParentEnvelope.Loop.Last().TargetValue;
                else if (coll.ParentEnvelope.Attack.Any())
                    startingValue = coll.ParentEnvelope.Attack.Last().TargetValue;
                else
                    startingValue = 1;
            }

            return startingValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
