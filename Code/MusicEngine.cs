
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
            public int SeqNumber;
            public int UsedTries;
            public int TimeOut;
        };

        public class TestResult {
            public int Number;
            public EarWorm.Shared.Listener.ListenResult LR;
            public int Tries;
            public int FailedNote;
            public TimeSpan Time;
            public TestDefinition TestDef;
        }

        static Dictionary<string, Instrument> _instruments = new Dictionary<string, Instrument>()
        {
            {"P",   new Instrument("P", "Piano", 0)},
            {"G", new Instrument("G", "Guitar", 12) }
        };

        public static String[] NoteStrings = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        SetGenerator.SetDef _currentSet= null;
        SetGenerator _generator;
        TestDefinition _currentTest;
        public SetGenerator.SetDef CurrentSet { get { return _currentSet; } }


        public void Init(SetGenerator.SetDef def) {
            _currentSet = def;
            _generator = new SetGenerator(def);
        }

        public string GetNoteName(int note) {
            var settings = Application.Settings;
            //note -= settings.GetNoteOffset();
            var noteStr = NoteStrings[note % 12];
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
            Application.Settings.SaveSettings();
        }

  
        public IEnumerable<TestDefinition> GetNextTest() {
            var testIdx = 0;
            foreach (var td in _generator.GetNextTest()) {
                _currentTest = td;
                //_currentTest = new TestDefinition {
                //    Notes = td.Notes,
                //    RootMode = RootMode.RootIncluded,
                //    Numtries = 5,
                //    Difficulty = 1,
                //    SeqNumber = testIdx,
                //    UsedTries = 0
                //};
                testIdx++;
                yield return _currentTest;

            }
        }
        public void ReportTestResult(TestResult result) {


        }
    }
}
