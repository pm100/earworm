using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
namespace EarWorm.Code {
    public class SetGenerator {
        [Inject]
        public IJSRuntime JJS { get; set; }
        static Dictionary<Scale, List<int>> S_scales = new Dictionary<Scale, List<int>>() {
            {Scale.Ionian, new List<int>() {0,2,4,5,7,9,11}},
            {Scale.Dorian, new List<int>() {0,2,3,5,7,9,10}},
            {Scale.Phrygian, new List<int>() {0,1,3,5,7,9,10}},
             {Scale.Lydian, new List<int>() {0,2,4,6,7,9,11}},
             {Scale.Aeolian, new List<int>() {0,2,3,5,7,8,10}},
             {Scale.Locrian, new List<int>() {0,1,2,5,6,9,10}},
             {Scale.Mixolydian, new List<int>() {0,2,4,5,7,9,10}},
             {Scale.WholeTone, new List<int>() {0,2,4,6,8,10}},
             {Scale.MelodicMinor, new List<int>() {0,2,3,5,7,9,11}},
             {Scale.HarmonicMinor, new List<int>() {0,2,3,5,7,8,11}},
             {Scale.DiminishedWH, new List<int>() {0,2,3,5,6,8,9,11}},
             {Scale.DiminishedHW, new List<int>() {0,1,3,4,6,7,9,10}},
             {Scale.Altered, new List<int>() {0,1,3,4,6,8,10}},
        };
        public enum Style {
            ScaleRandom,
            ScaleMelodySet,
            CycleOfFifthsRandom
        }
        public enum Scale {
            Ionian,
            Dorian,
            Phrygian,
            Lydian,
            Mixolydian,
            Aeolian,
            Locrian,
            Altered,
            DiminishedWH,
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
            public Style Style { get; set; }
            public Key Key { get; set; }
            public Scale Scale { get; set; }
            public int TestCount { get; set; }
            public int NoteCount { get; set; }
            public bool FirstNoteRoot { get; set; }
            public int RangeStart { get; set; }
            public int RangeEnd { get; set; }
            public string Description { get {
                    var key = EarWorm.Pages.TestSetup.s_keyTable.Find(x => x.Item1 == Key).Item2;
                    var scale = EarWorm.Pages.TestSetup.s_scaleTable.Find(x => x.Item1 == Scale).Item2;
                    return $"Basic Scale, {key} {scale} ";
                }
             }
            public int Retries { get; set; }
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
                                Numtries = _setdef.TestCount,
                                TimeOut = notes.Count * 2
                               
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
