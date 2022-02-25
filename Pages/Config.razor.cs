using EarWorm.Code;
using EarWorm.Shared;
using System.Collections;
namespace EarWorm.Pages
{
    public partial class Config
    {

        Dictionary<Lookups.InstrumentTags, Instrument> _instrumentTable;
        List<Lookups.InstrumentTags> _instrumentKeys;
        List<string> _instrumentNames;
        Listener _listener;
        public Config() {
            Util.Log("const config");


            _instrumentTable = Lookups.Instruments;
            _instrumentKeys = _instrumentTable.Keys.ToList();
            _instrumentNames = _instrumentTable.Values.Select(ins => ins.Name).ToList();

        }
        public bool KeySig {
            get {
                return _saver.Settings.KeySig;
            }
            set { _saver.Settings.KeySig = value; }
        }
        public bool SleepChecked {
            get {
                return _saver.Settings.NoSleep;
            }
            set {
                _saver.Settings.NoSleep = value;
            }
        }
        public List<string> Items {
            get {
                return _instrumentNames;
            }
        }
        public int Selected {
            get {
                var sel = _musicEngine.GetCurrentInstrument();
                return _instrumentKeys.FindIndex(x => x == sel.Key);
            }
        }


        private void InstrumentChanged(int value) {
            Util.Log($"inst = {value}");
            var k = _instrumentKeys[value];
            _saver.Settings.InstrumentKey = k;
        }
        private async void TestAudio() {
            var fakeTest = new TestDefinition {
                Key = _saver.CurrentSet.Key
            };
            await _listener.Show(Listener.Mode.Train, fakeTest,
                (int)Lookups.ImportantNotes.RangeHigh,
                (int)Lookups.ImportantNotes.RangeLow);
        }
        List<string> Toners {
            get {
                return new List<string> { "Beep", "Piano" };
            }
        }
        async void  TonerChanged(int sel) {
            _saver.Settings.ToneGenerator = (Lookups.ToneGenerator)sel;

            await _musicEngine.PlayNotes(new List<int> { 60, 64, 67 });
        }
        public int SelectedToner => (int)_saver.Settings.ToneGenerator;
        public void Dispose() {
            Util.Log("displose config");
            _saver.SaveSettings();
            _saver.SaveSetDefs();
        }
    }
}
