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

            foreach(var test in _musicEngine.GetNextTest()) { 
                if (!_running) break;
                test.UsedTries = 0;
                var i = 0;
                for (; test.UsedTries < test.Numtries; test.UsedTries++) {
                    i++;
                    await Task.Delay(1000);
                    PlayNotes(test.Notes);
                    await Task.Delay(5000);

                    var result = await _listener.Show(Listener.Mode.Test, test);
                    StateHasChanged();
                    switch (result.LR) {
                        case Listener.ListenResult.Matched:
                            // woo hoo 
                            _results.Add(result);
                            goto end_retry;

                        case Listener.ListenResult.Abandoned:
                            // nope, i dont like this test
                            _results.Add(result);
                            goto end_retry;
                    }
                    // otherwise try again (result = failed)
                   
                }
                // dropped out after retry exceeded, we failed
                if(test.UsedTries == test.Numtries) {
                    _results.Add(new MusicEngine.TestResult { Number = i, LR = Listener.ListenResult.Failed, Tries = test.UsedTries});
                }
                end_retry:
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
