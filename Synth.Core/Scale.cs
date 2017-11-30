using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Synth.Core
{
    public class Scale : IScale
    {
        private readonly Dictionary<string, IScaleNote> _notesByName;
        private readonly List<IScaleNote> _notes = new List<IScaleNote>();

        public Scale(string name, IEnumerable<IScaleNote> notes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The scale must have a name.");

            if (notes == null)
                throw new ArgumentNullException("notes");

            if (!notes.Any())
                throw new AggregateException("The scale must have notes.");

            Name = name;

            _notesByName = notes.ToDictionary<IScaleNote, string, IScaleNote>(n => n.Name, n => n);
            _notes.AddRange(_notesByName.Values);
        }

        public string Name { get; private set; }

        public bool ContainsKey(string key)
        {
            return _notesByName.ContainsKey(key);
        }

        public IScaleNote this[string key]
        {
            get { return _notesByName[key]; }
        }

        public IScaleNote this[int index]
        {
            get { return _notes[index]; }
        }

        public int Count
        {
            get { return _notes.Count; }
        }

        public IEnumerator<IScaleNote> GetEnumerator()
        {
            return _notes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_notes as IEnumerable).GetEnumerator();
        }

        private static readonly Lazy<IScale> _equalTemperedScale = new Lazy<IScale>(LoadEqualTemperedScale);

        public static IScale EqualTemperedScale
        {
            get { return _equalTemperedScale.Value; }
        }

        private static IScale LoadEqualTemperedScale()
        {
            Func<int, float> pitchCalcFunction = midiNumber => (float) Math.Round(Pitch.HalfStep*(midiNumber - 12), 6);

            IEnumerable<IScaleNote> notes =
                new ScaleNote[]
                {
                    new ScaleNote("C0",  12, pitchCalcFunction(12)),
                    new ScaleNote("C#0", 13, pitchCalcFunction(13)),
                    new ScaleNote("D0",  14, pitchCalcFunction(14)),
                    new ScaleNote("D#0", 15, pitchCalcFunction(15)),
                    new ScaleNote("E0",  16, pitchCalcFunction(16)),
                    new ScaleNote("F0",  17, pitchCalcFunction(17)),
                    new ScaleNote("F#0", 18, pitchCalcFunction(18)),
                    new ScaleNote("G0",  19, pitchCalcFunction(19)),
                    new ScaleNote("G#0", 20, pitchCalcFunction(20)),
                    new ScaleNote("A0",  21, pitchCalcFunction(21)),
                    new ScaleNote("A#0", 22, pitchCalcFunction(22)),
                    new ScaleNote("B0",  23, pitchCalcFunction(23)),

                    new ScaleNote("C1",  24, pitchCalcFunction(24)),
                    new ScaleNote("C#1", 25, pitchCalcFunction(25)),
                    new ScaleNote("D1",  26, pitchCalcFunction(26)),
                    new ScaleNote("D#1", 27, pitchCalcFunction(27)),
                    new ScaleNote("E1",  28, pitchCalcFunction(28)),
                    new ScaleNote("F1",  29, pitchCalcFunction(29)),
                    new ScaleNote("F#1", 30, pitchCalcFunction(30)),
                    new ScaleNote("G1",  31, pitchCalcFunction(31)),
                    new ScaleNote("G#1", 32, pitchCalcFunction(32)),
                    new ScaleNote("A1",  33, pitchCalcFunction(33)),
                    new ScaleNote("A#1", 34, pitchCalcFunction(34)),
                    new ScaleNote("B1",  35, pitchCalcFunction(35)),

                    new ScaleNote("C2",  36, pitchCalcFunction(36)),
                    new ScaleNote("C#2", 37, pitchCalcFunction(37)),
                    new ScaleNote("D2",  38, pitchCalcFunction(38)),
                    new ScaleNote("D#2", 39, pitchCalcFunction(39)),
                    new ScaleNote("E2",  40, pitchCalcFunction(40)),
                    new ScaleNote("F2",  41, pitchCalcFunction(41)),
                    new ScaleNote("F#2", 42, pitchCalcFunction(42)),
                    new ScaleNote("G2",  43, pitchCalcFunction(43)),
                    new ScaleNote("G#2", 44, pitchCalcFunction(44)),
                    new ScaleNote("A2",  45, pitchCalcFunction(45)),
                    new ScaleNote("A#2", 46, pitchCalcFunction(46)),
                    new ScaleNote("B2",  47, pitchCalcFunction(47)),

                    new ScaleNote("C3",  48, pitchCalcFunction(48)),
                    new ScaleNote("C#3", 49, pitchCalcFunction(49)),
                    new ScaleNote("D3",  50, pitchCalcFunction(50)),
                    new ScaleNote("D#3", 51, pitchCalcFunction(51)),
                    new ScaleNote("E3",  52, pitchCalcFunction(52)),
                    new ScaleNote("F3",  53, pitchCalcFunction(53)),
                    new ScaleNote("F#3", 54, pitchCalcFunction(54)),
                    new ScaleNote("G3",  55, pitchCalcFunction(55)),
                    new ScaleNote("G#3", 56, pitchCalcFunction(56)),
                    new ScaleNote("A3",  57, pitchCalcFunction(57)),
                    new ScaleNote("A#3", 58, pitchCalcFunction(58)),
                    new ScaleNote("B3",  59, pitchCalcFunction(59)),

                    new ScaleNote("C4",  60, pitchCalcFunction(60)),
                    new ScaleNote("C#4", 61, pitchCalcFunction(61)),
                    new ScaleNote("D4",  62, pitchCalcFunction(62)),
                    new ScaleNote("D#4", 63, pitchCalcFunction(63)),
                    new ScaleNote("E4",  64, pitchCalcFunction(64)),
                    new ScaleNote("F4",  65, pitchCalcFunction(65)),
                    new ScaleNote("F#4", 66, pitchCalcFunction(66)),
                    new ScaleNote("G4",  67, pitchCalcFunction(67)),
                    new ScaleNote("G#4", 68, pitchCalcFunction(68)),
                    new ScaleNote("A4",  69, pitchCalcFunction(69)),
                    new ScaleNote("A#4", 70, pitchCalcFunction(70)),
                    new ScaleNote("B4",  71, pitchCalcFunction(71)),

                    new ScaleNote("C5",  72, pitchCalcFunction(72)),
                    new ScaleNote("C#5", 73, pitchCalcFunction(73)),
                    new ScaleNote("D5",  74, pitchCalcFunction(74)),
                    new ScaleNote("D#5", 75, pitchCalcFunction(75)),
                    new ScaleNote("E5",  76, pitchCalcFunction(76)),
                    new ScaleNote("F5",  77, pitchCalcFunction(77)),
                    new ScaleNote("F#5", 78, pitchCalcFunction(78)),
                    new ScaleNote("G5",  79, pitchCalcFunction(79)),
                    new ScaleNote("G#5", 80, pitchCalcFunction(80)),
                    new ScaleNote("A5",  81, pitchCalcFunction(81)),
                    new ScaleNote("A#5", 82, pitchCalcFunction(82)),
                    new ScaleNote("B5",  83, pitchCalcFunction(83)),

                    new ScaleNote("C6",  84, pitchCalcFunction(84)),
                    new ScaleNote("C#6", 85, pitchCalcFunction(85)),
                    new ScaleNote("D6",  86, pitchCalcFunction(86)),
                    new ScaleNote("D#6", 87, pitchCalcFunction(87)),
                    new ScaleNote("E6",  88, pitchCalcFunction(88)),
                    new ScaleNote("F6",  89, pitchCalcFunction(89)),
                    new ScaleNote("F#6", 90, pitchCalcFunction(90)),
                    new ScaleNote("G6",  91, pitchCalcFunction(91)),
                    new ScaleNote("G#6", 92, pitchCalcFunction(92)),
                    new ScaleNote("A6",  93, pitchCalcFunction(93)),
                    new ScaleNote("A#6", 94, pitchCalcFunction(94)),
                    new ScaleNote("B6",  95, pitchCalcFunction(95)),

                    new ScaleNote("C7",  96, pitchCalcFunction(96)),
                    new ScaleNote("C#7", 97, pitchCalcFunction(97)),
                    new ScaleNote("D7",  98, pitchCalcFunction(98)),
                    new ScaleNote("D#7", 99, pitchCalcFunction(99)),
                    new ScaleNote("E7",  100, pitchCalcFunction(100)),
                    new ScaleNote("F7",  101, pitchCalcFunction(101)),
                    new ScaleNote("F#7", 102, pitchCalcFunction(102)),
                    new ScaleNote("G7",  103, pitchCalcFunction(103)),
                    new ScaleNote("G#7", 104, pitchCalcFunction(104)),
                    new ScaleNote("A7",  105, pitchCalcFunction(105)),
                    new ScaleNote("A#7", 106, pitchCalcFunction(106)),
                    new ScaleNote("B7",  107, pitchCalcFunction(107)),

                    new ScaleNote("C8",  108, pitchCalcFunction(108)),
                    new ScaleNote("C#8", 109, pitchCalcFunction(109)),
                    new ScaleNote("D8",  110, pitchCalcFunction(110)),
                    new ScaleNote("D#8", 111, pitchCalcFunction(111)),
                    new ScaleNote("E8",  112, pitchCalcFunction(112)),
                    new ScaleNote("F8",  113, pitchCalcFunction(113)),
                    new ScaleNote("F#8", 114, pitchCalcFunction(114)),
                    new ScaleNote("G8",  115, pitchCalcFunction(115)),
                    new ScaleNote("G#8", 116, pitchCalcFunction(116)),
                    new ScaleNote("A8",  117, pitchCalcFunction(117)),
                    new ScaleNote("A#8", 118, pitchCalcFunction(118)),
                    new ScaleNote("B8",  119, pitchCalcFunction(119)),

                    new ScaleNote("C9",  120, pitchCalcFunction(120)),
                    new ScaleNote("C#9", 121, pitchCalcFunction(121)),
                    new ScaleNote("D9",  122, pitchCalcFunction(122)),
                    new ScaleNote("D#9", 123, pitchCalcFunction(123)),
                    new ScaleNote("E9",  124, pitchCalcFunction(124)),
                    new ScaleNote("F9",  125, pitchCalcFunction(125)),
                    new ScaleNote("F#9", 126, pitchCalcFunction(126)),
                    new ScaleNote("G9",  127, pitchCalcFunction(127)),
                    new ScaleNote("G#9", 128, pitchCalcFunction(128)),
                    new ScaleNote("A9",  129, pitchCalcFunction(129)),
                    new ScaleNote("A#9", 130, pitchCalcFunction(130)),
                    new ScaleNote("B9",  131, pitchCalcFunction(131)),

                    new ScaleNote("C10", 132, pitchCalcFunction(132))
                };

            IScale scale = new Scale("Equal Tempered Scale", notes);
            return scale;
        }
    }
}
