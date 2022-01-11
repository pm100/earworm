using EarWorm.Shared;
using EarWorm.Code;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.JSInterop;
namespace EarWorm.Pages {
    public partial class Test {

        String SetDescription { get { return _set.Description ?? string.Empty; } }

        bool _running = false;
        Listener _listener;
        MusicEngine.TestSet _set;
        MusicEngine _musicEngine = Application.MusicEngine;

        List<MusicEngine.TestResult> _results;
        public Test() {
            _set = _musicEngine.GetTestSet();
            _results = new List<MusicEngine.TestResult>();
            var setDef = new SetGenerator.SetDef {
                FirstNoteRoot = true,
                Key = SetGenerator.Key.C,
                NoteCount = 3,
                RangeStart = 60,
                RangeEnd = 72,
                Scale = SetGenerator.Scale.Ionian,
                Style = SetGenerator.Style.ScaleRandom,
                TestCount = 10


            };
            _musicEngine.Init(setDef);  
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
                var tries = 0;
                var i = 0;
                for (; tries < test.Numtries; tries++) {
                    i++;
                    await Task.Delay(1000);
                    PlayNotes(test.Notes);
                    await Task.Delay(5000);

                    var result = await _listener.Show(Listener.Mode.Test, test.Notes);
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
                if(tries == test.Numtries) {
                    _results.Add(new MusicEngine.TestResult { Number = i, LR = Listener.ListenResult.Failed, Tries = tries});
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
