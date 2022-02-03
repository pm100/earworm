using Microsoft.JSInterop;
namespace EarWorm.Code {

    public class MusicEngine {

        SetDef _currentSet = null;
        SetGenerator _generator;
        TestDefinition _currentTest;
        SavedData _saver;
        SettingsData _settings;
        int _testIdx;

        public enum State {
            Fresh,
            InSet,
            ForcedClear
        }
        public SetDef CurrentSet { get { return _currentSet; } }

        public MusicEngine(SavedData saver) {
            _saver = saver;
            _settings = _saver.Settings;
            _testIdx = 0;
            _generator = new SetGenerator();
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
                _generator.Init(def);
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
        public void UpdateInstrument(Lookups.InstrumentTags key) {
            _saver.Settings.InstrumentKey = key;
            _saver.SaveSettings();
        }

        public int ScoreTest(TestResult r) {

            if (r.LR == Lookups.ListenResult.Matched) {
                return 100 / r.Tries;
            }
            else return 0;
        }
        public int CalculateScore(TestSetResult resultSet) {

            var total = 0;
            if (resultSet.Results.Count == 0) {
                return 0;
            }

            foreach (var t in resultSet.Results) {
                var tscore = ScoreTest(t);
                total += tscore;
            }
            total /= resultSet.Results.Count;
            return total;




        }
        public IEnumerable<TestDefinition> GetNextTest() {
            
            foreach (var td in _generator.GetNextTest()) {
                _currentTest = td;
                _testIdx++;
                yield return _currentTest;

            }
            _testIdx = 0;
        }

        public async void ReportTestResult(TestResult result) {
            CurrentSetResults.Results.Add(result);
            _saver.CurrentResults.SetDefinition = _currentSet;
            await _saver.SaveCurrentResults();
        }

        internal void EndSet() {
            _saver.WriteResult();
        }

        public TestSetResult CurrentSetResults {
            get {
                return _saver.CurrentResults;
            }
        }
        public Lookups.Key GetCurrentKey() {
            if (_currentSet.Style == Lookups.Style.ScaleRandom) {
                return _currentSet.Key;
            }
            if(_currentSet.Style == Lookups.Style.CycleOfFifthsRandom) {
                return _generator.GetCurrentKey();
            }
            return Lookups.Key.C; // !
        }
        public async void PlayNotes(IList<int> notes) {
            // we want 'real' note names
            var nlist = notes.Select(note => GetAbsNoteName(note, false));
            string func = "";
            if (_saver.Settings.ToneGenerator == Lookups.ToneGenerator.Beep)
                func = "window.playToneSeq";
            else
                func = "window.playSeq";
            await Util.JS.InvokeVoidAsync(func, nlist.ToList(), 1);
        }
    }
}
