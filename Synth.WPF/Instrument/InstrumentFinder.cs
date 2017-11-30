using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synth.WPF.Instrument
{
    public static class InstrumentFinder
    {
        private const string _defaultName = "Noble Lead";
        private const string _defaultAuthor = "Neil McKamey-Gonzalez";
        private const string _defaultDescription = "The build-in demo instrument for Synth.WPF";
        private static readonly Type _defaultType = typeof (DefaultInstrument);

        private static readonly Lazy<InstrumentToken> _defaultInstrument = new Lazy<InstrumentToken>(() => new InstrumentToken(_defaultName, MakeDefaultInstrument, _defaultAuthor, _defaultDescription)); 
        public static InstrumentToken DefaultInstrument
        {
            get { return _defaultInstrument.Value; }
        }

        private static InstrumentBase MakeDefaultInstrument()
        {
            InstrumentBase defaultInstrument = Activator.CreateInstance(_defaultType) as InstrumentBase; 
            return defaultInstrument;
        }

        private static IEnumerable<InstrumentToken> _autoLoadedInstruments;
        public static IEnumerable<InstrumentToken> AutoLoadedInstruments
        {
            get
            {
                if (_autoLoadedInstruments == null)
                    _autoLoadedInstruments = FindAutoLoadInstruments().OrderBy(t => t.Name).ToArray();

                return _autoLoadedInstruments;
            }
        }

        private static IEnumerable<InstrumentToken> FindAutoLoadInstruments()
        {
            List<InstrumentToken> tokens = new List<InstrumentToken>();

            Assembly assembly = Assembly.GetEntryAssembly();
            IEnumerable<Type> instrumentTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                                                .Where(t => t.GetCustomAttributes<AutoLoadInstrumentAttribute>().Any())
                                                .ToArray();

            foreach (Type instrumentType in instrumentTypes)
            {
                if (!typeof(InstrumentBase).IsAssignableFrom(instrumentType))
                {
                    Console.WriteLine("Class " + instrumentType.Name + " has the AutoLoadInstrument attribute, but it is not an instrument.");
                }
                // TODO implement default constructor test
                else if (!typeof(InstrumentBase).IsAssignableFrom(instrumentType))
                {
                    Console.WriteLine("Class " + instrumentType.Name + " has the AutoLoadInstrument attribute, but it does not have a default constructor.");
                }
                else
                {
                    AutoLoadInstrumentAttribute attribute = instrumentType.GetCustomAttributes<AutoLoadInstrumentAttribute>().First();
                    Type localType = instrumentType;
                    Func<InstrumentBase> factoryFunction = () => Activator.CreateInstance(localType) as InstrumentBase;
                    InstrumentToken token = new InstrumentToken(attribute.Name, factoryFunction, attribute.Author, attribute.Description);
                    tokens.Add(token);
                }
            }

            return tokens;
        }
    }
}
