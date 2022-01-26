namespace EarWorm.Code {

    public class MusicEngine {


        SetDef _currentSet = null;
        SetGenerator _generator;
        TestDefinition _currentTest;
        SavedData _saver;
        TestSetResult _currentResultSet;
        SettingsData _settings;
        public SetDef CurrentSet { get { return _currentSet; } }

        public MusicEngine(SavedData saver) {
            _saver = saver;
            _results = new List<TestResult>();
            _settings = _saver.Settings;
            _currentResultSet = new TestSetResult {
                Results = _results,
                DateTime = DateTime.Now
            };

        }
        public void Init(SetDef def) {
            _currentSet = def;
            _generator = new SetGenerator(def);
            
        }

        public string GetNoteName(int note, bool transpose, Lookups.Key key) {
            if(transpose) note += GetCurrentInstrument().NoteOffset;
            var noteStrings = Lookups.KeyTable[key].NoteNames;
            return NoteNameCalc(noteStrings, note);
        }

        public string GetAbsNoteName(int note, bool transpose) {
            if (transpose) note += GetCurrentInstrument().NoteOffset;
            return NoteNameCalc( Lookups.NoteNames, note);  
        }
        private string NoteNameCalc(string[]noteStrings, int noteNum) {
            var noteStr = noteStrings[noteNum % 12];
            var octave = Math.Floor((float)noteNum / 12) - 1;
            return noteStr + octave.ToString();

        }
        public Instrument GetCurrentInstrument() {
           
            var inst = _settings.InstrumentKey;
            return Lookups.Instruments[inst];
        }
        public void UpdateInstrument(string key) {
            _saver.Settings.InstrumentKey = key;
            _saver.SaveSettings();
        }


        public IEnumerable<TestDefinition> GetNextTest() {
            var testIdx = 0;
            foreach (var td in _generator.GetNextTest()) {
                _currentTest = td;
                testIdx++;
                yield return _currentTest;

            }
        }

        List<TestResult> _results;
        public void ReportTestResult(TestResult result) {
            _results.Add(result);
        }

        public List<TestResult> CurrentSetResults {
            get {
                return _results;
            }
        }
    }
}
