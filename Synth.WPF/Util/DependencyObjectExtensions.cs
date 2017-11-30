using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;

namespace Synth.WPF.Util
{
    public static class DependencyObjectExtensions
    {
        static DependencyObjectExtensions()
        {
        }

        public static DependencyObject GetRootParent(this DependencyObject source)
        {
            DependencyObject result = source;
            DependencyObject parent;

            while ((parent = VisualTreeHelper.GetParent(result)) != null)
                result = parent;

            return result;
        }

        public static TParent FindAncestor<TParent>(this DependencyObject source)
            where TParent : DependencyObject
        {
            // this would eliminate duplicate code, but I don't have time for regression testing so I'm leaving the original verison
            //return (TParent)FindAncestor(source, typeof(TParent));

            if (source == null)
                return null;

            DependencyObject parent = source.GetParent();

            if (parent == null)
                return null;
            else if (parent is TParent)
                return (parent as TParent);
            else
                return parent.FindAncestor<TParent>();
        }

        public static DependencyObject FindAncestor(this DependencyObject source, Type ancestorType)
        {
            if (ancestorType == null)
                throw new ArgumentNullException("ancestorType");
            if (!typeof(DependencyObject).IsAssignableFrom(ancestorType))
                throw new ArgumentException("The ancestor type does not inherit from DependencyObject");

            if (source == null)
                return null;

            DependencyObject parent = source.GetParent();

            if (parent == null)
                return null;
            else if (ancestorType.IsInstanceOfType(parent))
                return parent;
            else
                return parent.FindAncestor(ancestorType);
        }


        public static DependencyObject GetParent(this DependencyObject source)
        {
            if (source == null)
                return null;

            if (source is FrameworkElement)
                return (source as FrameworkElement).Parent;
            else
                return VisualTreeHelper.GetParent(source);
        }

        public static DependencyObject FindFirstAncestor(this DependencyObject source)
        {
            DependencyObject result = source;

            while ((source = VisualTreeHelper.GetParent(source)) != null)
                result = source;

            return result;
        }

        public static int GetAncestorDepth(this DependencyObject source)
        {
            int result = 0;

            while ((source = VisualTreeHelper.GetParent(source)) != null)
                result++;

            return result;
        }

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject source)
        {
            List<DependencyObject> results = new List<DependencyObject>();
            foreach (int i in Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(source)))
                results.Add(VisualTreeHelper.GetChild(source, i));

            return results;
        }

        public static IEnumerable<TChild> GetDescendents<TChild>(this DependencyObject source)
            where TChild : DependencyObject
        {
            List<TChild> results = new List<TChild>();
            AddDescendentsRecursive(source, results);
            return results;
        }

        private static void AddDescendentsRecursive<TChild>(DependencyObject target, IList<TChild> results)
            where TChild : DependencyObject
        {
            IEnumerable<DependencyObject> allChildren = target.GetChildren<DependencyObject>();
            foreach (DependencyObject child in allChildren)
            {
                if (child is TChild)
                    results.Add(child as TChild);
                AddDescendentsRecursive<TChild>(child, results);
            }
        }

        public static IEnumerable<TChild> GetChildren<TChild>(this DependencyObject source)
            where TChild : DependencyObject
        {
            List<TChild> results = new List<TChild>();
            foreach (int i in Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(source)))
            {
                DependencyObject child = VisualTreeHelper.GetChild(source, i);
                if (child is TChild)
                    results.Add(child as TChild);
            }

            return results;
        }

        public static void SetFocusToChild(this DependencyObject source, FrameworkElement focusTarget)
        {
            if (source != null)
            {
                Action action = delegate ()
                {
                    FocusManager.SetFocusedElement(source, null);
                    FocusManager.SetFocusedElement(source, focusTarget);
                };

                source.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
            }
        }

        public static bool IsInDesignMode(this DependencyObject source)
        {
            return DesignerProperties.GetIsInDesignMode(source);
        }
    }
}
