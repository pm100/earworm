using EarWorm.Code;
using EarWorm.Shared;
using Microsoft.JSInterop;
namespace EarWorm.Pages {
    public partial class Test {

       // bool _midTest = false;
        bool _running = false;
      //  bool _listening = false;
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
        private async void StartClick() {

            
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
         //   _midTest = true;
            if (_saver.Settings.NoSleep)
                Util.NoSleep(true);
           // var testNum = 1;
            foreach (var test in _musicEngine.GetNextTest()) {
                if (!_running) break;
                TestResult result = null;
               
                var tries = 0;
                for (tries =0; tries < test.Numtries; tries++) {
                    _musicEngine.PlayNotes(test.Notes);
                    test.UsedTries = tries + 1;
                    await Task.Delay(test.Notes.Count * 1000);
                  //  _listening = true;
                    result = await _listener.Show(Listener.Mode.Test, test);
                    result.TestDef = test;
                 //   _listening = false;
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
                      //  result.Number = testNum;
                        break;
                    }
                    // otherwise try again (result = failed)

                }
                
                // dropped out after retry exceeded, we failed
                if (tries == test.Numtries) {
                    result = new TestResult { LR = Lookups.ListenResult.Failed, Tries = tries+1, TestDef=test };
                }
                _musicEngine.ReportTestResult(result);
                //testNum++; 
                StateHasChanged();
            }
            // _midTest = false;
            _musicEngine.EndSet();
         }

        private TestSetResult CurrentResults {
            get {
                return _musicEngine.CurrentSetResults;
            }
        }

    }

}
