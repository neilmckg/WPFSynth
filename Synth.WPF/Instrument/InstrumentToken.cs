using System;
using System.Windows;

namespace Synth.WPF.Instrument
{
    public class InstrumentToken
    {
        public InstrumentToken(String name, Func<InstrumentBase> factory, string author = "", string description = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A name must be provided.");
            if (factory == null)
                throw new ArgumentNullException("factory");

            Name = name;
            Factory = factory;
            Author = author;
            Description = description;
        }
        
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public Func<InstrumentBase> Factory { get; private set; }
    }
}