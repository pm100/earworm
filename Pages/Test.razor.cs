using EarWorm.Shared;
using EarWorm.Code;
using Microsoft.JSInterop;
namespace EarWorm.Pages {
    public partial class Test {

        String SetDescription { get { return _musicEngine.CurrentSet.Description ; } }

        bool _running = false;
        Listener _listener;
        
        MusicEngine _musicEngine = Application.MusicEngine;

        List<MusicEngine.TestResult> _results;
        public Test() {
          
            _results = new List<MusicEngine.TestResult>();

            _musicEngine.Init(Application.Settings.CurrentSet);  
        }
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
                MusicEngine.TestResult result = null;
                for (; test.UsedTries < test.Numtries; test.UsedTries++) {
                    i++;
                    await Task.Delay(1000);
                    PlayNotes(test.Notes);
                    await Task.Delay(test.Notes.Count * 1000);

                    result = await _listener.Show(Listener.Mode.Test, test);
                    StateHasChanged();
                    if(result.LR == Listener.ListenResult.Matched 
                        || result.LR == Listener.ListenResult.Abandoned) { 
                            break;
                    }
                    // otherwise try again (result = failed)
                   
                }
                // dropped out after retry exceeded, we failed
                if(test.UsedTries == test.Numtries) {
                    result = new MusicEngine.TestResult { Number = i, LR = Listener.ListenResult.Failed, Tries = test.UsedTries };
                }
                _musicEngine.ReportTestResult(result);
                StateHasChanged();

            }
            await Task.Delay(1000);
        }

        private IList<MusicEngine.TestResult> Results {
            get {
                return _results;
            }
        }
        private void PlayNotes(IList<int> notes) {
            var nlist = notes.Select(note => _musicEngine.GetNoteName(note));
            Util.JS.InvokeVoidAsync("window.playSeq", nlist.ToList());

        }
    }

}
