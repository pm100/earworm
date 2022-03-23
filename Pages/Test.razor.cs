using EarWorm.Code;
using EarWorm.Shared;
using Microsoft.JSInterop;
namespace EarWorm.Pages {
    public partial class Test {

        bool _rollNext = true;
        bool _running = false;
       // bool _first = true;
        bool _showRoll;
    
        int Time { get; set; }
        int MaxTime { get; set; }
        Timer _timer;
        MusicEngine.State _initState;
        Listener _listener;
        bool ShowButtons => _running == false;
        bool ShowResume => _initState == MusicEngine.State.InSet;
        protected override void OnInitialized() {
            _initState = _musicEngine.Init(_saver.CurrentSet);
        }
        String SetDescription => _musicEngine.CurrentSet.Description();
        private string StartButtonText {
            get {
                return _initState == MusicEngine.State.InSet ? "Restart" : "Start";
            }
        }
        private bool ShowRoll => _showRoll;
        bool RollNext {
            get {
                return _rollNext;
            }
            set {
                _rollNext = value;
                if (!_rollNext) {
                    StopTimer();
                }
                else {
                    StartTimer(Time, true);
                }
            }
        }
        private async void StartClick() {
            StopTimer();
            _musicEngine.Clear();
            _running = !_running;
            if (_running)
                await StartTestSet();

        }
        private async void ResumeClick() {
            _running = !_running;
            if (_running)
                await StartTestSet();
        }
        private async Task StartTestSet() {
            _showRoll = false;
            if (_saver.Settings.NoSleep)
                Util.NoSleep(true);
            foreach (var test in _musicEngine.GetNextTest()) {
                if (!_running) break;
                TestResult result = null;
               
                var tries = 0;
                for (tries =0; tries < test.Numtries; tries++) {
                    if (_musicEngine.CurrentSet.RootMode == Lookups.RootMode.PlayTriad) {
                        _musicEngine.PlayTestTriad(test);
                        await Task.Delay(2000);
                    }
                    
                    await _musicEngine.PlayNotes(test.Notes);
                    test.UsedTries = tries + 1;
                    await Task.Delay(test.Notes.Count * 1250);
                    result = await _listener.Show(Listener.Mode.Test, test,
                        _musicEngine.CurrentSet.RangeEnd + 6,
                        _musicEngine.CurrentSet.RangeEnd - 6);
                    result.TestDef = test;
                    StateHasChanged();
                    if (result.LR == Lookups.ListenResult.Stop) {
                        _running = false;
                        _initState = MusicEngine.State.InSet;
                        StateHasChanged();
                        return;
                    }

                    if (result.LR == Lookups.ListenResult.Matched
                        || result.LR == Lookups.ListenResult.Abandoned) { 
                        result.Tries = tries+1;
                        break;
                    }
                    // otherwise try again (result = failed)

                }
                
                // dropped out after retry exceeded, we failed
                if (tries == test.Numtries) {
                    result = new TestResult { LR = Lookups.ListenResult.Failed, Tries = tries+1, TestDef=test };
                }
                _musicEngine.ReportTestResult(result);
        
                StateHasChanged();
            }
            _musicEngine.EndSet();
            _running = false;
            _showRoll = true;
            StartTimer(5000, false);
            StateHasChanged();
        }
        void StopTimer() {
            if (_timer != null) {
                _timer.Dispose();
                _timer = null;
            }
        }
 
        void StartTimer(int max, bool resume) {
            const int TIME_SLICE = 100;
            if (!resume)
                MaxTime = Time = max;
            _timer = new Timer(async _ => {
                Time -= TIME_SLICE;
                //Util.Log($"time = {Time} max = {MaxTime}");
                if (Time == 0) {
                    // timeout
                    await InvokeAsync(StartClick);
                }
                await InvokeAsync(StateHasChanged);
            }, null, 0, TIME_SLICE);
        }
        private TestSetResult CurrentResults {
            get {
                return _musicEngine.CurrentSetResults;
            }
        }
        public void Dispose() {
            if (_saver.Settings.NoSleep)
                Util.NoSleep(false);
        }

    }

}
