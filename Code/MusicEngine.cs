namespace EarWorm.Code {

    public class MusicEngine {


        SetDef _currentSet = null;
        SetGenerator _generator;
        TestDefinition _currentTest;
        SavedData _saver;
        public SetDef CurrentSet { get { return _currentSet; } }

        public MusicEngine(SavedData saver) {
            _saver = saver;
        }
        public void Init(SetDef def) {
            _currentSet = def;
            _generator = new SetGenerator(def);
            _results = new List<TestResult>();
        }

        public string GetNoteName(int note) {
            var settings = _saver.Settings;
            var noteStr = Lookups.NoteStrings[note % 12];
            var octave = Math.Floor((float)note / 12) - 1;
            return noteStr + octave.ToString();
        }

        public Instrument GetCurrentInstrument() {
            var settings = _saver.Settings;
            var inst = settings.InstrumentKey;
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
