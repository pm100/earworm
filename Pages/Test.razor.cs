using EarWorm.Shared;
using EarWorm.Code;
using Microsoft.JSInterop;
namespace EarWorm.Pages {
    public partial class Test {


        bool _running = false;
        Listener _listener;
        MusicEngine _musicEngine = Application.MusicEngine;
       
        public Test() {
            _musicEngine.Init(Application.Settings.CurrentSet);  
        }
        String SetDescription => _musicEngine.CurrentSet.Description;
        private string StartButtonText {
            get {
                return _running ? "Stop" : "Start";
            }
        }
        private async void StartClick() {
            _running = !_running;
            if(_running)
                await StartTestSet();
            
        }
        private async Task StartTestSet() {
            Util.NoSleep(true);
            foreach(var test in _musicEngine.GetNextTest()) { 
                if (!_running) break;
                test.UsedTries = 0;
                var i = 0;
                TestResult result = null;
                for (; test.UsedTries < test.Numtries; test.UsedTries++) {
                    i++;
                   // await Task.Delay(1000);
                    PlayNotes(test.Notes);
                    await Task.Delay(test.Notes.Count * 1000);

                    result = await _listener.Show(Listener.Mode.Test, test);
                    StateHasChanged();
                    if(result.LR == Lookups.ListenResult.Matched 
                        || result.LR == Lookups.ListenResult.Abandoned) { 
                            break;
                    }
                    // otherwise try again (result = failed)
                   
                }
                // dropped out after retry exceeded, we failed
                if(test.UsedTries == test.Numtries) {
                    result = new TestResult { Number = i, LR = Lookups.ListenResult.Failed, Tries = test.UsedTries };
                }
                _musicEngine.ReportTestResult(result);
                StateHasChanged();

            }
           // await Task.Delay(1000);
        }

        private IList<TestResult> Results {
            get {
                return _musicEngine.CurrentSetResults;
            }
        }
        private void PlayNotes(IList<int> notes) {
            var nlist = notes.Select(note => _musicEngine.GetNoteName(note));
            Util.JS.InvokeVoidAsync("window.playSeq", nlist.ToList());

        }
    }

}
