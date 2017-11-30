using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

using Synth.Util;

namespace Synth.WPF.Util
{
    /// <summary>
    /// Allows access to resource from xaml (using the x:Static markup extension) while encapsulating the management of the asset.
    /// </summary>
    public abstract class XamlResourceFacade : NotifierBase
    {
        protected const string PACKURI_TEMPLATE = "pack://application:,,,/{0};component/{1}";

        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentDictionary<string, object> _overrides = new ConcurrentDictionary<string, object>();
        private readonly ResourceDictionary _dictionary;

        protected XamlResourceFacade(string dictionaryPath)
            : this(Assembly.GetCallingAssembly().GetName().Name, dictionaryPath)
        {

        }

        protected XamlResourceFacade(string assemblyName, string dictionaryPath)
            : this(new Uri(string.Format(PACKURI_TEMPLATE, assemblyName, dictionaryPath)))
        {
        }

        protected XamlResourceFacade(Uri dictionaryLocation)
        {
            _dictionary = new ResourceDictionary { Source = dictionaryLocation };
        }

        protected T GetValue<T>(string key, bool canCache = true, bool freezeIfPossible = true)
        {
            //Console.WriteLine("Getting Value: " + key);

            T result;

            if (!TryGetOverride(key, out result))
            {
                if (!TryGetCachedValue(key, out result))
                {
                    result = LoadFromXAMLResourceFile<T>(key, freezeIfPossible);
                    if (canCache)
                        _cache[key] = result;
                }
            }

            return result;
        }

        private bool TryGetOverride<T>(string key, out T value)
        {
            object rawValue;
            bool gotIt = _overrides.TryGetValue(key, out rawValue);

            if (gotIt)
                value = (T)rawValue;
            else
                value = default(T);

            return gotIt;
        }

        private bool TryGetCachedValue<T>(string key, out T value)
        {
            object rawValue;
            bool gotIt = _cache.TryGetValue(key, out rawValue);

            if (gotIt)
                value = (T)rawValue;
            else
                value = default(T);

            return gotIt;
        }

        //protected static void ClearOverride(Expression<Func<object>> keyExpr)
        //{
        //    PropertyInfo property = ConvertToProperty(keyExpr);
        //    _overrides.Remove(property);

        //    PropertyChangedEventHandler evt = PropertyChanged;
        //    if (evt != null)
        //        evt.Invoke(null, new PropertyChangedEventArgs(property.Name));
        //}

        protected void SetOverride<T>(string key, T value, [CallerMemberName] string propertyName = "")
        {
            _overrides[key] = value;
            OnPropertyChanged(propertyName);
        }

        private T LoadFromXAMLResourceFile<T>(string resourceKey, bool freezeIfPossible)
        {
            T result = (T)_dictionary[resourceKey];

            if (freezeIfPossible && result is Freezable)
                (result as Freezable).Freeze();

            return result;
        }
    }
}
