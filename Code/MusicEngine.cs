
namespace EarWorm.Code {
    public record Instrument(string key, string Name, int NoteOffset);
    public class MusicEngine {
        public enum RootMode {
            PlayRoot,
            RootIncluded,
            PlayTriad,
            Silent
        }
        // represents one test to be presented to the user
        public class TestDefinition {
            public int Numtries;
            public int Difficulty;
            public IList<int> Notes;
            public RootMode RootMode;
            };
        public class TestSet {
            public string Description;
            public int TestCount;
        }
        public class TestResult {
            public int Number;
            public bool Success;
            public int Tries;
        }

        static Dictionary<string, Instrument> _instruments = new Dictionary<string, Instrument>()
        {
            {"P",   new Instrument("P", "Piano", 0)},
            {"G", new Instrument("G", "Guitar", 12) }
        };
      
        static String[] noteStrings = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        TestSet _currentSet;
        SetGenerator _generator;
        int _currentTest;
        public String CurrentSetName { get { return _currentSet?.Description; } }

        public void Init(SetGenerator.SetDef def) {
            _generator = new SetGenerator(def);
        }

        public string GetNoteName(int note) {
            var settings = Application.Settings;
            //note -= settings.GetNoteOffset();
            var noteStr = noteStrings[note % 12];
            var octave = Math.Floor((float)note / 12) - 1;
            return noteStr + octave.ToString();
        }

        public Dictionary<string, Instrument> GetInstrumentTable() {

            return _instruments;
        }
        public Instrument GetCurrentInstrument() {
            var settings = Application.Settings;
            var inst = settings.InstrumentKey;
            return _instruments[inst];
        }
        public void UpdateInstrument(string key) {
            Application.Settings.InstrumentKey = key;
            Application.Settings.Save();
        }

        public TestSet GetTestSet() {
            return new TestSet { Description = "Test", TestCount = 10 };
        }
        public IEnumerable<TestDefinition> GetNextTest() {
            foreach(var td in _generator.GetNextTest()){
                yield return new TestDefinition {
                    Notes = td.Notes,
                    RootMode = RootMode.RootIncluded,
                    Numtries = 5,
                    Difficulty = 1
                };

            }
        }
    }
}
