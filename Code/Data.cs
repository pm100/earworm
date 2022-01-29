namespace EarWorm.Code {

    public static class Lookups {
        public enum RootMode {
            //    PlayRoot,  // play root note before the test is played
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
            Stop,
            Init
        }

        static readonly Dictionary<string, Instrument> _instruments = new()
        {
            { "P", new Instrument { Key = "P", Name = "Piano", NoteOffset = 0, Clef=Clef.Treble } },
            { "G", new Instrument { Key = "G", Name = "Guitar", NoteOffset = 12, Clef = Clef.Treble } },
            { "B", new Instrument { Key = "B", Name = "Bass", NoteOffset = 12, Clef = Clef.Bass } },
        };
        public static Dictionary<string, Instrument> Instruments => _instruments;

        static readonly String[] _noteNames = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public static String[] NoteNames => _noteNames;

        // relative notes in each scale
        static readonly Dictionary<Scale, List<int>> _scales = new() {
            { Scale.Ionian, new List<int>() { 0, 2, 4, 5, 7, 9, 11 } },
            { Scale.Dorian, new List<int>() { 0, 2, 3, 5, 7, 9, 10 } },
            { Scale.Phrygian, new List<int>() { 0, 1, 3, 5, 7, 9, 10 } },
            { Scale.Lydian, new List<int>() { 0, 2, 4, 6, 7, 9, 11 } },
            { Scale.Aeolian, new List<int>() { 0, 2, 3, 5, 7, 8, 10 } },
            { Scale.Locrian, new List<int>() { 0, 1, 2, 5, 6, 9, 10 } },
            { Scale.Mixolydian, new List<int>() { 0, 2, 4, 5, 7, 9, 10 } },
            { Scale.WholeTone, new List<int>() { 0, 2, 4, 6, 8, 10 } },
            { Scale.MelodicMinor, new List<int>() { 0, 2, 3, 5, 7, 9, 11 } },
            { Scale.HarmonicMinor, new List<int>() { 0, 2, 3, 5, 7, 8, 11 } },
            { Scale.DiminishedWH, new List<int>() { 0, 2, 3, 5, 6, 8, 9, 11 } },
            { Scale.DiminishedHW, new List<int>() { 0, 1, 3, 4, 6, 7, 9, 10 } },
            { Scale.Altered, new List<int>() { 0, 1, 3, 4, 6, 8, 10 } },
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
            A,
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
            BFlat,
            DFlat,
            EFlat,
            GFlat,
            AFlat
        }
        public enum Clef {
            Treble,
            Bass
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
        public record KeyDef(
              Key Key,
             String Name,
             int Base,
             string[] NoteNames

        );
        // key enum to key info
        // 21 = A0 midi note
        // the note names drive the staff display X@ menas display X natural
        static readonly Dictionary<Key, KeyDef> _keyTable = new() {
            { Key.A, new KeyDef(Key.A, "A", 21, new string[] { "C@", "C", "D", "D#", "E", "F@", "F#", "G@", "G#", "A", "A#", "B" }) },
            { Key.ASharp, new KeyDef(Key.ASharp, "A#", 22, new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.BFlat, new KeyDef(Key.BFlat, "Bb", 22, new string[] { "C", "Db", "D", "E", "E@", "F", "Gb", "G", "Ab", "A", "B", "B@" }) },
            { Key.B, new KeyDef(Key.B, "B", 23, new string[] { "C@", "C#", "D@", "D#", "E", "F@", "F#", "G@", "G#", "A@", "A", "B" }) },
            { Key.C, new KeyDef(Key.C, "C", 24, new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.CSharp, new KeyDef(Key.CSharp, "C#", 25, new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.DFlat, new KeyDef(Key.DFlat, "Db", 25, new string[] { "C@", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.D, new KeyDef(Key.D, "D", 26, new string[] { "C@", "C", "D", "D#", "E", "F@", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.DSharp, new KeyDef(Key.DSharp, "D#", 27, new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.EFlat, new KeyDef(Key.EFlat, "Eb", 27, new string[] { "C", "Db", "D", "E", "E@", "F", "Gb", "G", "A", "A@", "Bb", "B@" }) },
            { Key.E, new KeyDef(Key.E, "E", 28, new string[] { "C@", "C#", "D@", "D#", "E", "F@", "F#", "G@", "G#", "A", "A#", "B" }) },
            { Key.F, new KeyDef(Key.F, "F", 29, new string[] { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Gb", "A", "Bb", "B@" }) },
            { Key.FSharp, new KeyDef(Key.FSharp, "F#", 30, new string[] { "C@", "C", "D", "D#", "E", "F@", "F", "G", "G#", "A", "A#", "B" }) },
            { Key.GFlat, new KeyDef(Key.GFlat, "Gb", 30, new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.G, new KeyDef(Key.G, "G", 31, new string[] { "C", "C#", "D", "D#", "E", "F@", "F", "G", "G#", "A", "A#", "B" }) },
            { Key.GSharp, new KeyDef(Key.GSharp, "G#", 32, new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }) },
            { Key.AFlat, new KeyDef(Key.AFlat, "Ab", 32, new string[] { "C", "D", "D@", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B@" }) },
        };
        public static Dictionary<Key, KeyDef> KeyTable => _keyTable;
        public static List<(Key, string)> KeyNames => _keyTable.Select(kv => (kv.Key, kv.Value.Name)).ToList();

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
                        TestCount = 5,
                        Retries = 5,
                        TimeAllowed = 2
                    };
            }
        }
        public static SettingsData DefaultSettings {
            get {
                return new SettingsData {
                    InstrumentKey = "P",
                    NoSleep = true,
                    Version = 1,
                    KeySig = true
                };
            }
        }
    }
    // records
    public record Instrument {
        public string Key { get; set; }
        public string Name { get; set; }
        public int NoteOffset { get; set; }// /transpositon + means instrument tx down, - means tx up
        public Lookups.Clef Clef { get; set; }  
        public int RangeLow { get; set; }     
        public int RangeHigh { get; set; }
    };
    // represents one test to be presented to the user
    public record TestDefinition {
        public int Numtries { get; set; }
        public int Difficulty { get; set; }
        public IList<int> Notes { get; set; }
        public Lookups.RootMode RootMode { get; set; }
        public int SeqNumber { get; set; }
        public int UsedTries { get; set; }
        public int TimeOut { get; set; }
        public Lookups.Key Key { get; set; }
    };

    public record TestResult {
       // public int Number { get; set; }
        public Lookups.ListenResult LR { get; set; }
        public int Tries { get; set; }
        public int FailedNote { get; set; }
        public TimeSpan Time { get; set; }
        public TestDefinition TestDef { get; set; }
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
        public int TimeAllowed { get; set; }
    }


    // persistent data
    public record SettingsData {
        public string InstrumentKey { get; set; }
        public bool NoSleep { get; set; }
        public bool KeySig { get; set; }
        public int Version { get; set; }
    };


    public record class SetDefData {
        public SetDef Current { get; set; }
        public List<SetDef> Saved { get; set; }
    }

    public record TestSetResult {
        public List<TestResult> Results { get; set; }
        public SetDef SetDefinition { get; set; }   
        public DateTime DateTime { get; set; }
    }
    public record ResultsDB {
        public TestSetResult Current { get; set; }
        public List<TestSetResult> Results { get; set; }
    }
}

