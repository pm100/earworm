namespace EarWorm.Code {
    public record Instrument(
        string Key,
        string Name,
        int NoteOffset // /transpositon + means instrument tx down, - means tx up
     );
    public static class Lookups {
        public enum RootMode {
            PlayRoot,  // play root note before the test is played
            IncludeRoot, // include random root in test
            IncludeRootHigh, // include highest root in range
            IncludeRootLow, // include lowest root in range
            PlayTriad,// play triad
            Silent // nothing
        }

        public enum ListenResult {
            Matched, // passed the test
            Failed, // wrong note played
            Timeout, // took too long
            Abandoned, // gave up (clicked skip)
            RetryLimit, // too many goes
            Init 
        }

        static readonly Dictionary<string, Instrument> _instruments = new()
        {
            { "P", new Instrument("P", "Piano", 0) },
            { "G", new Instrument("G", "Guitar", 12) },
            { "B", new Instrument("B", "Bass", 12) },
        };
        public static Dictionary<string, Instrument> Instruments => _instruments;

        static readonly String[] _noteStrings = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public static String[] NoteStrings => _noteStrings;

        // relative notes in each scale
        static readonly Dictionary<Scale, List<int>> _scales = new() {
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
        static readonly List<(Lookups.Scale, String)> _scaleTable = new() {
            (Scale.Ionian, "Major"),
            (Scale.Mixolydian, "Dominant"),
            (Scale.Dorian, "Dorian"),
            (Scale.Aeolian, "Minor"),
            (Scale.Lydian, "Lydian"),
            (Scale.Phrygian, "Phrygian"),
            (Scale.Locrian, "Locrian")
        };
        public static List<(Lookups.Scale, String)> ScaleNames => _scaleTable;
        
        // key enum to key name
        static readonly List<(Key, String)> _keyTable = new() {
            (Key.C, "C"),
            (Key.CSharp, "C# / Db"),
            (Key.D, "D"),
            (Key.DSharp, "D# / Eb"),
            (Key.E, "E"),
            (Key.F, "F"),
            (Key.FSharp, "F# / Gb"),
            (Key.G, "G"),
            (Key.GSharp, "G# / Ab"),
            (Key.A, "A"),
            (Key.ASharp, "A# / Bb"),
            (Key.B, "D"),
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
        public static SettingsData DefaultSettings {
            get {
                return new SettingsData {
                    InstrumentKey = "P",
                    NoSleep = true,
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


    public record SetDef {
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
                var key = Lookups.KeyNames.Find(x => x.Item1 == Key).Item2;
                var scale = Lookups.ScaleNames.Find(x => x.Item1 == Scale).Item2;
                return $"Basic Scale, {key} {scale} ";
            }
        }
        public int Retries { get; set; }
    }


    // persistent data
    public record SettingsData {
        public string InstrumentKey { get; set; }
        public bool NoSleep { get; set; }
    };


    public record class SetDefData {
        public SetDef Current { get; set; }
        public List<SetDef> Saved { get; set; }
    }

    public record TestSetResult {
        public TestResult Result { get; set; }
        public DateTime DateTime { get; set; }
    }
}

