using System;
using System.Windows;

namespace Synth.WPF.Util
{
    public static class DependencyPropertyExtensions
    {
        public static void SetDefaultValue<TForClass>(this DependencyProperty property, object value)
            where TForClass : DependencyObject
        {
            if (value == null)
            {
                if (property.PropertyType.IsValueType)
                    throw new ArgumentException("The new default value cannot be null.");
            }
            else
            {
                if (!property.PropertyType.IsInstanceOfType(value))
                    throw new ArgumentException("The new default value is not a valid type.");
            }

            PropertyMetadata newPmd;

            PropertyMetadata pmd = property.GetMetadata(DependencyObjectType.FromSystemType(typeof(TForClass).BaseType ?? typeof(object)));

            if (pmd is FrameworkPropertyMetadata)
            {
                FrameworkPropertyMetadata fpmd = pmd as FrameworkPropertyMetadata;
                newPmd = new FrameworkPropertyMetadata()
                {
                    IsAnimationProhibited = fpmd.IsAnimationProhibited,
                    DefaultUpdateSourceTrigger = fpmd.DefaultUpdateSourceTrigger,
                    AffectsRender = fpmd.AffectsRender,
                    AffectsMeasure = fpmd.AffectsMeasure,
                    AffectsArrange = fpmd.AffectsArrange,
                    AffectsParentMeasure = fpmd.AffectsParentMeasure,
                    AffectsParentArrange = fpmd.AffectsParentArrange,
                    BindsTwoWayByDefault = fpmd.BindsTwoWayByDefault,
                    Inherits = fpmd.Inherits,
                    IsNotDataBindable = fpmd.IsNotDataBindable,
                    Journal = fpmd.Journal,
                    OverridesInheritanceBehavior = fpmd.OverridesInheritanceBehavior,
                    SubPropertiesDoNotAffectRender = fpmd.SubPropertiesDoNotAffectRender
                };
            }
            else
            {
                newPmd = new PropertyMetadata();
            }

            if (pmd != null)
            {
                newPmd.CoerceValueCallback = pmd.CoerceValueCallback;
                newPmd.PropertyChangedCallback = pmd.PropertyChangedCallback;
            }

            newPmd.DefaultValue = value;

            property.OverrideMetadata(typeof(TForClass), newPmd);
        }
    }
}
