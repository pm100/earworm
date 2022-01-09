namespace EarWorm.Code {
    public class SetGenerator {

        static Dictionary<Scale, List<int>> S_scales = new Dictionary<Scale, List<int>>() {
            {Scale.Ionian, new List<int>() {0,2,4,5,7,9,11}},
            {Scale.Dorian, new List<int>() {0,2,3,5,7,9,10}},
            {Scale.Phrigian, new List<int>() {0,1,3,5,7,9,10}},
             {Scale.Lydian, new List<int>() {0,2,4,6,7,9,11}},
             {Scale.Mixolydian, new List<int>() {0,2,4,5,7,9,10}}

        };
        public enum Style {
            ScaleRandom,
            ScaleMelodySet,
            CycleOfFifthsRandom
        }
        public enum Scale {
            Ionian,
            Dorian,
            Phrigian,
            Lydian,
            Mixolydian,
            Aeolian,
            Locrian,
            Altered,
            DiminishWH,
            DiminishedHW,
            WholeTone,
            MelodicMinor,
            HarmonicMinor
        }

        public enum Key {
            A = 21, // A0 midi note
            ASharp, 
            B,
            C, 
            CSharp,
            D,
            DSharp,
            E,
            F,
            FSharp,
            G,
            GSharp,
            BFlat = ASharp,
            DFlat = CSharp,
            EFlat = DSharp,
            GFlat = FSharp,
            AFlat = GSharp
        }

        public class SetDef {
            public Style Style;
            public Key Key;
            public Scale Scale;
            public int TestCount;
            public int NoteCount;
            public bool FirstNoteRoot;
            public int RangeStart;
            public int RangeEnd;
        }

        SetDef _setdef;
        Random _rng = new Random(Guid.NewGuid().GetHashCode());
        public SetGenerator(SetDef setDef) {
            _setdef = setDef;
        }
        public IEnumerable<MusicEngine.TestDefinition> GetNextTest() {
            switch (_setdef.Style) {
                case Style.ScaleRandom: {
                        for (int j = 0; j < _setdef.TestCount; j++) {
                            var notes = ScaleNotes();
                            var td = new MusicEngine.TestDefinition {
                                Notes = notes,
                            };
                            yield return td;
                        }
                        yield break;
                    }
                // break;
                default: {
                        throw new NotSupportedException();
                    }
            }

        }
        List<int> ScaleNotes() {
            var noteCount = _setdef.NoteCount;
            var ret = new List<int>(_setdef.NoteCount);
            var scale = S_scales[_setdef.Scale];
            int previous = -1;
            if (_setdef.FirstNoteRoot) {
                noteCount--;
                ret.Add(0);
                previous = 0;
            }
            for (int i = 0; i < noteCount; i++) {
                while (true) {
                    var noteIdx = _rng.Next(scale.Count);
                    var note = scale[noteIdx];
                    if (note == previous) continue;
                    previous = note;
                    ret.Add(note);
                    break;
                }
            }
            return ret.Select(n => RelToAbs(n)).ToList();

        }

        int RelToAbs(int relNote) {
            // convert a scale relative note to an actual note in range
            var r = (int)_setdef.Key + relNote;
            var candidates = new List<int>();
            for (int octave = 0; octave < 9; octave++) {
                var candidate = r + (octave * 12);
                if (candidate >= _setdef.RangeStart && candidate <= _setdef.RangeEnd) {
                    candidates.Add(candidate);
                }
                if (candidate > _setdef.RangeEnd)
                    break;
            }
            if (candidates.Count == 0)
                throw new NotSupportedException();

            // randomly choose a candidate note

            return candidates[_rng.Next(candidates.Count)];



        }
    }
}
