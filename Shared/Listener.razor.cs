using EarWorm.Code;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.JSInterop;

namespace EarWorm.Shared {
    public partial class Listener {
        public IconBase _resultIcon = null;
        public class StaffNote {
            public string Note { get; set; }
            public String Color { get; set; }
            // 0 none
            // 1 sharp
            // 2 flat
            // 3 natural
            public int Accidental { get; set; }
        }
        public class Staff {
            public string Clef { get; set; }
            public string Key { get; set; }
            public List<StaffNote> Notes { get; set; }
            public Staff() { Notes = new List<StaffNote>(); }
        }
        static readonly string[] NoNotes = Array.Empty<string>();
        Timer _timer;
        int Time;
        int Max;
        HxModal _modal;
        bool _listening;
        TestResult _result;
        Mode _mode;
        TaskCompletionSource<TestResult> _tcs;
        static Listener s_listenerInstance;
        private string _notes;
        IList<int> _noteList;
        int _noteIdx;
        Staff _staffDef;
        DateTime _startTime;
        TestDefinition _testDef;
        int _rangeHigh;
        int _rangeLow;

        void StartTimer(int max) {
            Max = Time = max * 10;
            _timer = new Timer(async _ => {
                Time -= 1;
                if (Time == 0) {
                    // timeout
                    _resultIcon = BootstrapIcon.Clock;
                    Stop(Lookups.ListenResult.Timeout);
                }
                await InvokeAsync(StateHasChanged);
            }, null, 0, 100);
        }
        public enum Mode {
            Test,
            Train
        }
        public Listener() {
            Util.Log("construct listener");
            s_listenerInstance = this;
        }
        public async Task<TestResult> Show(Mode mode, TestDefinition testDef, int rangeHigh, int rangeLow) {
            _tcs = new TaskCompletionSource<TestResult>();
            _testDef = testDef;
            _rangeHigh = rangeHigh; 
            _rangeLow = rangeLow;
            Init(mode);
            await _modal.ShowAsync();
            StartJSListener(5, 15, 0.2, 0.01);
            _startTime = DateTime.Now;
            return await _tcs.Task;
        }

        private void Init(Mode mode) {
            Util.Log("init");
            if(mode == Mode.Train) {

            }
            else {
                _noteList = _testDef.Notes;
                StartTimer(_testDef.TimeOut);
            }
            _notes = "";
            _mode = mode;
            _noteIdx = 0;
            _result = new TestResult {
                LR = Lookups.ListenResult.Abandoned
            };
            var keyStr = _saver.Settings.KeySig ?
                Lookups.KeyTable[_musicEngine.GetCurrentKey()].Name : "C";
            var inst = _musicEngine.GetCurrentInstrument();
            _staffDef = new Staff { Clef = inst.Clef.ToString().ToLower(), Key=keyStr };
            _resultIcon = null;
        }

        private async Task Note(int n) {
            Util.Log($"note = {n}, listen = {_listening}");
            // to deal with any last note notification left over
            if (!_listening)
                return;

            if (n > _rangeHigh || n < _rangeLow) {
                Util.Log($"note = {n}, out of range");

                return;
            }
            var result = Lookups.ListenResult.Init;
            // convert midi note number to string n => C#4

            // for display of note names, abolute names but transposed
            var absNoteStr = _musicEngine.GetAbsNoteName(n, true);

            // note name for display on staff
            var relNoteStr = _musicEngine.GetNoteName(n, true, _saver.Settings.KeySig ?
                _musicEngine.GetCurrentKey() : Lookups.Key.C);

            var newNote = new StaffNote();
            _staffDef.Notes.Add(newNote);

            // convert to VexFlow format C#4 => C#/4


            newNote.Note = $"{relNoteStr[0]}/{relNoteStr[^1]}";
            newNote.Color = "black";
            if (relNoteStr.Contains('#'))
                newNote.Accidental = 1;
            else if (relNoteStr.Contains('b'))
                newNote.Accidental = 2;
            else if (relNoteStr.Contains('@'))
                newNote.Accidental = 3;
            if (_mode == Mode.Test) { 
                // did they play the correct note?
                if (_noteList[_noteIdx] == n) {
                    // yes, move to the next one
                    _noteIdx++;
                    if (_noteIdx == _noteList.Count) {
                        // all matched - woo hoo
                        _resultIcon = BootstrapIcon.HandThumbsUp;
                        result = Lookups.ListenResult.Matched;
                    }
                }
                else {
                    // wrong note, we are out of here
                    _result.FailedNote = _noteIdx;
                    newNote.Color = "red";
                    _resultIcon = BootstrapIcon.HandThumbsDown;
                    result = Lookups.ListenResult.Failed;
                }
            }
            else {
                if(_noteIdx++>6) {
                    Stop(Lookups.ListenResult.Stop);
                }
            }

            await Util.JS.InvokeVoidAsync("window.drawStaff", "vf", _staffDef);
            _notes += String.Format("{0} ", absNoteStr);

            StateHasChanged();
            if (result != Lookups.ListenResult.Init)
                Stop(result);
        }
        private void StartJSListener(int lockCount, int buffer, double thresh, double silence) {
            // three params control sensitiy
            // buff - size of listen buffer in 128 block samples
            // low notes need a lot of samples
            // thresh - is used to decide the beginning and end of relevant samples 
            // lockCount - how many samples have to return the same not before we lock onto it
            // silnse - rms threshold for decideing that a sample is silnt and thus should not be looked at

            JS.InvokeVoidAsync("pitchStart", "earworm", "NoteHeard", buffer, silence, thresh, lockCount);
            //_listening = true;
        }
        private void StopJSListener() {
            _listening = false;
            JS.InvokeVoidAsync("pitchStop");
        }

        // JS listener call back when note heard
        [JSInvokable]
        public static async void NoteHeard(int note) {
            await s_listenerInstance.Note(note);
        }

        private async void Stop(Lookups.ListenResult result) {
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
            StopJSListener();
            _result.LR = result;
            Util.Log("hide1");
            if (result!= Lookups.ListenResult.Abandoned && result !=Lookups.ListenResult.Stop)
                await Task.Delay(1500);
            await _modal.HideAsync();
            Util.Log(string.Format("stop result={0}", result));
        }

        private void Closed() {
            Util.Log($"closed {_listening}");
            if (_listening) {
                _timer.Dispose();
                _timer = null;
                StopJSListener();
            }
            Util.Log(string.Format("set task={0}", _result));
            _result.Time = DateTime.Now - _startTime;
            _tcs.SetResult(_result);
            _tcs = null;
        }

        private void HandleSkip() {
            Stop(Lookups.ListenResult.Abandoned);
            StateHasChanged();
        }
        private void HandleStop() {
            Stop(Lookups.ListenResult.Stop);
            StateHasChanged();
        }

        private async void Shown() {
            Util.Log($"shown {_listening}");
            await Util.JS.InvokeVoidAsync("window.drawStaff", "vf", _staffDef);
            _listening = true;

        }
        public void Dispose() {
            if(_saver.Settings.NoSleep)
                Util.NoSleep(false);
            Util.Log("dispose l");
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
        }
    }

}
