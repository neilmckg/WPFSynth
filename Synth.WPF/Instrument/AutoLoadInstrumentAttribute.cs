using System;

namespace Synth.WPF.Instrument
{
    public class AutoLoadInstrumentAttribute : Attribute
    {
        public AutoLoadInstrumentAttribute(string name = "", string author = "", string description = "")
        {
            Name = name;
            Author = author;
            Description = description;
        }

        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
    }
}
