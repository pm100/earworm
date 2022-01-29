namespace EarWorm.Code {

    public class MusicEngine {


        SetDef _currentSet = null;
        SetGenerator _generator;
        TestDefinition _currentTest;
        SavedData _saver;
       // TestSetResult _currentResultSet;
        SettingsData _settings;
        public enum State {
            Fresh,
            InSet,
            ForcedClear
        }
        int _testIdx;
        public SetDef CurrentSet { get { return _currentSet; } }

        public MusicEngine(SavedData saver) {
            _saver = saver;
           // _results = new List<TestResult>();
            _settings = _saver.Settings;
           // _currentResultSet = new TestSetResult {
           //     Results = _results,
           //     DateTime = DateTime.Now
           // };
            _testIdx = 0;
        }
        public State Init(SetDef def) {
            var ret = State.Fresh;
            if (def != _currentSet) {
                ret =  State.ForcedClear;
             
            }
            else if (_testIdx > 0) {
                ret = State.InSet;  
            }
            if (ret != State.InSet) {
                _currentSet = def with { };
                _generator = new SetGenerator(def, _testIdx);
                Clear();
            }
            return ret;
 
        }
        public void Clear() {
            _testIdx = 0;
            _saver.CurrentResults.Results.Clear(); 
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
            
            foreach (var td in _generator.GetNextTest()) {
                _currentTest = td;
                _testIdx++;
                yield return _currentTest;

            }
            _testIdx = 0;
        }

      //  List<TestResult> _results;
        public async void ReportTestResult(TestResult result) {
            CurrentSetResults.Add(result);
            _saver.CurrentResults.SetDefinition = _currentSet;
            await _saver.SaveCurrentResults();
        }

        internal void EndSet() {
            _saver.WriteResult();
        }

        public List<TestResult> CurrentSetResults {
            get {
                return _saver.CurrentResults.Results;
            }
        }
    }
}
