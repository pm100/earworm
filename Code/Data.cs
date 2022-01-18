namespace EarWorm.Code {
    public record Instrument(string key, string Name, int NoteOffset);
    public static class Lookups {
        public enum RootMode {
            PlayRoot,
            IncludeRoot,
            IncludeRootHigh,
            IncludeRootLow,
            PlayTriad,
            Silent
        }

        public enum ListenResult {
            Matched,
            Failed,
            Timeout,
            Abandoned,
            RetryLimit,
            Init
        }

        static Dictionary<string, Instrument> _instruments = new()
    {
            {"P", new Instrument("P", "Piano", 0)},
            {"G", new Instrument("G", "Guitar", 12) }
        };
        public static Dictionary<string, Instrument> Instruments => _instruments;

        static String[] _noteStrings = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public static String[] NoteStrings => _noteStrings;


        static Dictionary<Scale, List<int>> _scales = new Dictionary<Scale, List<int>>() {
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
        public static Dictionary<Scale, List<int>> Scales => _scales;

        // public enum RootMode
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



        // scale enum to scale name
        static List<(Lookups.Scale, String)> _scaleTable = new List<(Lookups.Scale, string)> {
        (Lookups.Scale.Ionian, "Major"),
        (Lookups.Scale.Mixolydian, "Dominant"),
        (Lookups.Scale.Dorian, "Dorian"),
        (Lookups.Scale.Aeolian,"Minor"),
        (Lookups.Scale.Lydian,"Lydian"),
        (Lookups.Scale.Phrygian,"Phrygian"),
        (Lookups.Scale.Locrian,"Locrian")
    };
        public static List<(Lookups.Scale, String)> ScaleNames => _scaleTable;
        // key enum to key name
        static List<(Lookups.Key, String)> _keyTable = new List<(Lookups.Key, string)> {
        (Lookups.Key.C, "C"),
        (Lookups.Key.CSharp, "C# / Db"),
        (Lookups.Key.D, "D"),
        (Lookups.Key.DSharp, "D# / Eb"),
        (Lookups.Key.E, "E"),
        (Lookups.Key.F, "F"),
        (Lookups.Key.FSharp, "F# / Gb"),
        (Lookups.Key.G, "G"),
        (Lookups.Key.GSharp, "G# / Ab"),
        (Lookups.Key.A, "A"),
        (Lookups.Key.ASharp, "A# / Bb"),
        (Lookups.Key.B, "D"),
    };
        public static List<(Key, string)> KeyNames => _keyTable;

    }

    public static class Defaults {
        public static SetDef DefaultSetDef {
            get {
                return
                    new SetDef {
                        RootMode = Lookups.RootMode.IncludeRoot,
                        Key = Lookups.Key.C,
                        NoteCount = 3,
                        RangeStart = 60,
                        RangeEnd = 72,
                        Scale = Lookups.Scale.Ionian,
                        Style = Lookups.Style.ScaleRandom,
                        TestCount = 10,
                        Retries = 5,
                    };
            }
        }
    }

    // records

    // represents one test to be presented to the user
    public class TestDefinition {
        public int Numtries;
        public int Difficulty;
        public IList<int> Notes;
        public Lookups.RootMode RootMode;
        public int SeqNumber;
        public int UsedTries;
        public int TimeOut;
    };

    public class TestResult {
        public int Number;
        public Lookups.ListenResult LR;
        public int Tries;
        public int FailedNote;
        public TimeSpan Time;
        public TestDefinition TestDef;
    }

    public class SetDef {
        public Lookups.Style Style { get; set; }
        public Lookups.Key Key { get; set; }
        public Lookups.Scale Scale { get; set; }
        public int TestCount { get; set; }
        public int NoteCount { get; set; }
        public Lookups.RootMode RootMode { get; set; }
        public int RangeStart { get; set; }
        public int RangeEnd { get; set; }
        public string Description {
            get {
                var key =Lookups.KeyNames.Find(x => x.Item1 == Key).Item2;
                var scale = Lookups.ScaleNames.Find(x => x.Item1 == Scale).Item2;
                return $"Basic Scale, {key} {scale} ";
            }
        }
        public int Retries { get; set; }
    }
}
