using EarWorm.Code;
using EarWorm.Shared;
using Microsoft.JSInterop;
namespace EarWorm.Pages {
    public partial class Test {


        bool _running = false;
        Listener _listener;


        protected override void OnInitialized() {
            _musicEngine.Init(_saver.CurrentSet);
        }
        String SetDescription => _musicEngine.CurrentSet.Description;
        private string StartButtonText {
            get {
                return _running ? "Stop" : "Start";
            }
        }
        private async void StartClick() {
            _running = !_running;
            if (_running)
                await StartTestSet();

        }
        private async Task StartTestSet() {
            if (_saver.Settings.NoSleep)
                Util.NoSleep(true);
            var testNum = 1;
            foreach (var test in _musicEngine.GetNextTest()) {
                if (!_running) break;
                TestResult result = null;
                var tries = 0;
                for (tries =0; tries < test.Numtries; tries++) {
                    PlayNotes(test.Notes);
                    test.UsedTries = tries + 1;
                    await Task.Delay(test.Notes.Count * 1000);
                    result = await _listener.Show(Listener.Mode.Test, test);
                    StateHasChanged();
                    if (result.LR == Lookups.ListenResult.Matched
                        || result.LR == Lookups.ListenResult.Abandoned) {
                        result.Tries = tries+1;
                        result.Number = testNum;
                        break;
                    }
                    // otherwise try again (result = failed)

                }
                // dropped out after retry exceeded, we failed
                if (tries == test.Numtries) {
                    result = new TestResult { Number = testNum, LR = Lookups.ListenResult.Failed, Tries = tries+1 };
                }
                _musicEngine.ReportTestResult(result);
                testNum++; 
                StateHasChanged();
            }
         }

        private IList<TestResult> Results {
            get {
                return _musicEngine.CurrentSetResults;
            }
        }
        private async void PlayNotes(IList<int> notes) {
            // we want 'real' note names
            var nlist = notes.Select(note => _musicEngine.GetAbsNoteName(note, false));
            await Util.JS.InvokeVoidAsync("window.playSeq", nlist.ToList());
        }
    }

}
