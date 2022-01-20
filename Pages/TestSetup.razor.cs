using EarWorm.Code;

namespace EarWorm.Pages {
    public partial class TestSetup {
        protected override void OnInitialized() {
           
         _currentSet = _saver.CurrentSet;   
        }
        private int SetSize { get { return _currentSet.TestCount; } set { _currentSet.TestCount = value; } }
        private int Retries { get { return _currentSet.Retries; } set { _currentSet.Retries = value; } }
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

    }
}
