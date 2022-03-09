using EarWorm.Code;
using System.Collections;
using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components;
namespace EarWorm.Pages {
    public partial class TestSetup {

        [Inject] protected IHxMessageBoxService MessageBox { get; set; }
        protected override void OnInitialized() {

            _currentSet = _saver.CurrentSet;
        }
        private int SetSize {
            get { return _currentSet.TestCount; }
            set {
                if (value < 1) {
                    var showResult = MessageBox.ShowAsync("Info", "Must be more than 0", MessageBoxButtons.Ok);
                }
                else
                    _currentSet.TestCount = value;
            }
        }
        private int Retries { get { return _currentSet.Retries; } set { _currentSet.Retries = value; } }
        private int TimeAllowed {
            get { return _currentSet.TimeAllowed; }
            set {
                if (value < 1) {
                    var showResult = MessageBox.ShowAsync("Info", "Must be more than 1", MessageBoxButtons.Ok);
                }
                else {
                    _currentSet.TimeAllowed = value;
                }
            }
        }
        private int NotesInTest { get { return _currentSet.NoteCount; } set {
                if (value < 1 || value > 6) {
                    var showResult = MessageBox.ShowAsync("Info", "Must be more than 1 and less than 6", MessageBoxButtons.Ok);
                }
                else
                _currentSet.NoteCount = value; } }
        private List<string> ScaleItems {

            get {
                return Lookups.ScaleNames.Select(x => x.Item2).ToList();
            }
        }
        private void ScaleChanged(int sel) {
            _currentSet.Scale = Lookups.ScaleNames[sel].Item1;
        }

        private int SelectedScale {
            get {
                return Lookups.ScaleNames.FindIndex(x => x.Item1 == _currentSet.Scale);
            }

        }
        private IList<string> KeyItems {
            get {
                return Lookups.KeyNames.Select(x => x.Item2).ToList();
            }
        }
        private void KeyChanged(int sel) {
            _currentSet.Key = Lookups.KeyNames[sel].Item1;
        }

        private int SelectedKey {
            get {
                return Lookups.KeyNames.FindIndex(x => x.Item1 == _currentSet.Key);
            }
        }

        SetDef _currentSet;

        public void Dispose() {
            _saver.SaveSetDefs();

        }

        public static IList<string> RootModes {
            get {

                var t = new string[] {
                    "Include  Root",
                 // "Include High Root",
                  //  "Include Low Root",
                    "Play Triad",
                    "None"

                };
                return t;
            }
        }
        public int SelectedRootMode => (int)_currentSet.RootMode;
        public void OnRootChanged(int sel) {
            _currentSet.RootMode = (Lookups.RootMode)sel;
        }

        public static IList<string> TestTypes {
            get {

                var t = new string[] {
                    "Random from one scale",
                  //  "Melody samples",
                    "Random notes moving thru cycle",
                };
                return t;
            }
        }
        public int SelectedTestType => (int)_currentSet.Style;
        public void OnTestTypeChanged(int sel) {
            _currentSet.Style = (Lookups.Style)sel;
        }
    }
}
