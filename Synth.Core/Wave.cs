using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Synth.Audio;
using Synth.Util;

namespace Synth.Core
{
    public sealed class Wave : IWave
    {
        private const int SAMPLE_COUNT = 65536;     // 2^16  --  for higher quality, use larger power of 2

        public static Wave Sine { get; private set; }
        public static Wave Triangle { get; private set; }
        public static Wave Sawtooth { get; private set; }
        public static Wave Square { get; private set; }

        public static IReadOnlyList<Wave> All { get; private set; }

        static Wave()
        {
            InitializeSineWave();
            InitializeTriangleWave();
            InitializeSawtoothWave();
            InitializeSquareWave();
            All = new Wave[]{ Sine, Triangle, Sawtooth, Square };
        }

        private static void InitializeSineWave()
        {
            float sineIncrement = (float)(Math.PI * 2) / SAMPLE_COUNT;
            IEnumerable<float> sineTable = Enumerable.Range(0, SAMPLE_COUNT).Select(i => (float)Math.Sin(sineIncrement * i));
            Sine = new Wave("Sine", sineTable.Select(f => new AudioSample(f, f)).ToArray());
        }

        private static void InitializeTriangleWave()
        {
            //calculate the triangle wave for just the first quarter-cycle, then reverse and invert to fill out the cycle. 
            float triangleIncrement = 4f / SAMPLE_COUNT;
            List<float> triangleTable = Enumerable.Range(0, SAMPLE_COUNT / 4).Select(i => triangleIncrement * i).ToList();
            triangleTable.AddRange(triangleTable.Select(i => 1 - i).ToArray());
            triangleTable.AddRange(triangleTable.Select(i => -i).ToArray());
            Triangle = new Wave("Triangle", triangleTable.Select(f => new AudioSample(f, f)).ToArray());
        }

        private static void InitializeSawtoothWave()
        {
            float sawtoothIncrement = 1f / (SAMPLE_COUNT - 1);
            IEnumerable<float> sawtoothTable = Enumerable.Range(0, SAMPLE_COUNT).Select(i => 1 - 2 * sawtoothIncrement * i);
            Sawtooth = new Wave("Sawtooth", sawtoothTable.Select(f => new AudioSample(f, f)).ToArray());
        }

        private static void InitializeSquareWave()
        {
            float[] squareTable = new float[SAMPLE_COUNT];
            int sectionSize = SAMPLE_COUNT / 2;

            Enumerable.Range(sectionSize * 0, sectionSize).Execute(i => squareTable[i] = 1);
            Enumerable.Range(sectionSize * 1, sectionSize).Execute(i => squareTable[i] = -1);

            Square = new Wave("Square", squareTable.Select(f => new AudioSample(f,f)).ToArray());
        }

        #region instance members

        private AudioSample[] _waveTable;

        private Wave(string name, AudioSample[] waveTable)
        {
            Name = name;
            _waveTable = waveTable;
        }

        public string Name { get; private set; }

        public AudioSample this[int index]
        {
            get { return _waveTable[index]; }
        }

        public int Count
        {
            get { return _waveTable.Length; }
        }

        public IEnumerator<AudioSample> GetEnumerator()
        {
            return (_waveTable as IEnumerable<AudioSample>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _waveTable.GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion instance members
    }
}
